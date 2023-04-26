import { ipcMain, app, dialog } from 'electron';
import * as fs from 'fs';
import * as pathModule from 'path';
import { ChildProcess, spawn } from 'child_process';
import * as os from 'os';
import { validateCommand } from './shell-validation';
import { isDevelopment } from './env';

export function initIpcHandlers() {
  ipcMain.handle('shellExecute', handleShellExecute);
  ipcMain.handle('createDirectory', handleCreateDirectory);
  ipcMain.handle('createOrUpdateFile', handleCreateOrUpdateFile);
  ipcMain.handle('copyFile', handleCopyFile);
  ipcMain.handle('readFile', handleReadFile);
  ipcMain.handle('showFolderPickerDialog', handleShowFolderPickerDialog);

  ipcMain.on('getUserDataPathSync', handleGetUserDataPathSync);
  ipcMain.on('getOsArchSync', handleGetOsArchSync);
  ipcMain.on('isProdSync', handleIsProdSync);
  ipcMain.on('getAppVersionSync', handleGetAppVersionSync);
}

function handleShellExecute(e, toolsDirectory, commandInfo, commandParameters, input): Promise<any> {
  return new Promise((resolve, reject) => {
    const result = [] as any[];
    const cwd = pathModule.join(toolsDirectory, commandInfo.directory);
    const commandArgs = buildCommandArgs(commandParameters);

    validateCommand(commandInfo.fileName, commandParameters, isDevelopment);

    const command = spawn(commandInfo.fileName, commandArgs, {
      cwd: cwd,
      shell: isDevelopment,
      windowsHide: true,
    });
    processInput(command, input);
    command.stdout.on('data', data => {
      result.push(data);
    });
    command.stderr.on('data', data => {
      reject(data);
    });
    command.on('close', code => {
      resolve({ exitCode: code, data: result });
    });
  });
}

async function handleCreateDirectory(e, path: string): Promise<void> {
  if (!fs.existsSync(path)) {
    await fs.promises.mkdir(path, { recursive: true });
  }
}

function handleCreateOrUpdateFile(e, path, data): Promise<void> {
  return fs.promises.writeFile(path, data);
}

async function handleCopyFile(e, sourcePath, targetPath): Promise<void> {
  if (!fs.existsSync(sourcePath)) {
    console.error('Source file "' + sourcePath + '" does not exist');
    return;
  }

  await fs.promises.copyFile(sourcePath, targetPath);
}

function handleReadFile(e, path): Promise<string> {
  if (!fs.existsSync(path)) {
    console.log(`File ${path} does not exist`);
    return;
  }

  return fs.promises.readFile(path, 'utf8');
}

function handleShowFolderPickerDialog(e): string {
  const result = dialog.showOpenDialogSync({ properties: ['openDirectory'] });
  return result[0];
}

function handleGetUserDataPathSync(e): void {
  e.returnValue = app.getPath('userData');
}

function handleGetOsArchSync(e): void {
  e.returnValue = os.arch();
}

function handleIsProdSync(e): void {
  e.returnValue = !isDevelopment;
}

function handleGetAppVersionSync(e): void {
  e.returnValue = process.env.APP_VERSION ?? '0.0.0-dev';
}

function processInput(process: ChildProcess, input?: any) {
  if (input && !!process.stdin) {
    process.stdin.setDefaultEncoding('utf-8');
    process.stdin.write(JSON.stringify(input));
    process.stdin.end();
  }
}

function buildCommandArgs(commandParameters) {
  const args = [];

  for (const commandParameter of commandParameters) {
    if (commandParameter.name !== '') {
      args.push(commandParameter.name);
    }

    if (commandParameter.value !== '') {
      if (!isDevelopment) {
        // In production the shell is not used (by setting shell: false on spawn). No additional sanitization or escaping needed.
        args.push(commandParameter.value);
      }
      else {
        // in development the shell is used (by setting shell: true on spawn) and commandParameter.value's such as the filenames need to be in quotes
        // because the args are handled differently.
        // https://nodejs.org/api/child_process.html#child_processspawncommand-args-options
        // No sanitization in dev mode.
        args.push(`"${commandParameter.value}"`);
      }
    }
  }

  return args;
}

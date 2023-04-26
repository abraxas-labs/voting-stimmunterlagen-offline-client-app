import { contextBridge, ipcRenderer } from 'electron';

/**
 * The preload script runs before. It has access to web APIs
 * as well as Electron's renderer process modules and some
 * polyfilled Node.js functions.
 *
 * https://www.electronjs.org/docs/latest/tutorial/sandbox
 */

// ipc channels such as 'shellExecute' cannot be easily outsourced into a separate file because of electron security settings.
contextBridge.exposeInMainWorld('backend', {
  shellExecute: (toolsDir, commandInfo, commandParameters, input) =>
    ipcRenderer.invoke('shellExecute', toolsDir, commandInfo, commandParameters, input),
  createDirectory: path => ipcRenderer.invoke('createDirectory', path),
  createOrUpdateFile: (path, content) => ipcRenderer.invoke('createOrUpdateFile', path, content),
  copyFile: (sourcePath, targetPath) => ipcRenderer.invoke('copyFile', sourcePath, targetPath),
  readFile: path => ipcRenderer.invoke('readFile', path),
  showFolderPickerDialog: () => ipcRenderer.invoke('showFolderPickerDialog'),

  getUserDataPathSync: () => ipcRenderer.sendSync('getUserDataPathSync'),
  getOsArchSync: () => ipcRenderer.sendSync('getOsArchSync'),
  isProdSync: () => ipcRenderer.sendSync('isProdSync'),
  getAppVersionSync: () => ipcRenderer.sendSync('getAppVersionSync'),
});

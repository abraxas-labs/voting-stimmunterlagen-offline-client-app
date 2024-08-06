/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

const whitelistedCommands = [
  'VotingCardGenerator',
  'CryptoTool',
  'ZipTool',
  'EchDeliveryJsonConverter',
  'PdfMerger',

  'sdelete.exe',
  'sdelete64.exe',
]

/**
 * Validates whether the command is allowed (in the whitelist).
 * @param command The command to execute
 */
export function validateCommand(command: string, commandParameters: any[], dotnetEnabled: boolean): void {
  for (const commandParameter of commandParameters) {
    if (!commandParameter.name.match(/^-+[0-9a-zA-Z]+$/) && commandParameter.name !== '') {
      throw new Error(`Invalid command parameter name ${commandParameter.name}`)
    }
  }

  if (whitelistedCommands.includes(command)) {
    return;
  }

  if (dotnetEnabled && command === 'dotnet run --') {
    return;
  }

  throw new Error(`Attempted to execute the forbidden command ${command}`);
}

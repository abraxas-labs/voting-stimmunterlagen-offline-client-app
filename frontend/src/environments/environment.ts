/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

import { CommandInfo } from '../app/models/shell/command-info';

declare const backend: any;

export const environment = {
  production: false,
  toolsDirectory: '../backend/tools',
  commands: {
    votingCardGenerator: new CommandInfo('dotnet run --', 'VotingCardGenerator'),
    secureDelete: new CommandInfo(backend.getOsArchSync() === 'x64' ? 'sdelete64.exe' : 'sdelete.exe', '3rdPartyTools/SDelete'),
    decrypt: new CommandInfo('dotnet run --', 'CryptoTool'),
    zipTool: new CommandInfo('dotnet run --', 'ZipTool'),
    echDeliveryToJsonConverter: new CommandInfo('dotnet run --', 'EchDeliveryJsonConverter'),
    pdfMerge: new CommandInfo('dotnet run --', 'PdfMerger'),
  },
};

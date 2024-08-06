/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { CommandInfo } from '../app/models/shell/command-info';
import { pathCombine } from '../app/services/utils/path.utils';

declare const backend: any;

export const environment = {
  production: true,
  toolsDirectory: pathCombine('resources', 'tools'),
  commands: {
    votingCardGenerator: new CommandInfo('VotingCardGenerator', 'VotingCardGenerator'),
    secureDelete: new CommandInfo(backend.getOsArchSync() === 'x64' ? 'sdelete64.exe' : 'sdelete.exe', '3rdPartyTools/SDelete'),
    decrypt: new CommandInfo('CryptoTool', 'CryptoTool'),
    zipTool: new CommandInfo('ZipTool', 'ZipTool'),
    echDeliveryToJsonConverter: new CommandInfo('EchDeliveryJsonConverter', 'EchDeliveryJsonConverter'),
    pdfMerge: new CommandInfo('PdfMerger', 'PdfMerger'),
  },
};

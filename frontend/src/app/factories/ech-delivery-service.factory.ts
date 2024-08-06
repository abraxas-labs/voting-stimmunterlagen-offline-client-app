/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { ElectronService } from '../services/electron.service';
import { VotingData } from '../models/voting-data';
import { EchDeliveryShellService } from '../services/shell/ech-delivery-shell.service';
import { LogService } from '../services/log.service';

export const EchDeliveryServiceFactory = (
  electronService: ElectronService,
  logService: LogService,
): EchDeliveryShellService<VotingData> | undefined => {
  return electronService.isElectronEnvironment() ? new EchDeliveryShellService<VotingData>(electronService, logService) : undefined;
};

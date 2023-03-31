import { ElectronService } from '../services/electron.service';
import { LogService } from '../services/log.service';
import { CrypticShellService } from '../services/shell/cryptic-shell.service';

export const CrypticServiceFactory = (electronService: ElectronService, logService: LogService): CrypticShellService | undefined => {
  return electronService.isElectronEnvironment() ? new CrypticShellService(electronService, logService) : undefined;
};

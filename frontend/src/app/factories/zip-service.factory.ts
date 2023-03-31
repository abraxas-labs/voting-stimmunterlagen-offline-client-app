import { ElectronService } from '../services/electron.service';
import { ZipShellService } from '../services/shell/zip-shell.service';

export const ZipServiceFactory = (electronService: ElectronService): ZipShellService | undefined => {
  return electronService.isElectronEnvironment() ? new ZipShellService(electronService) : undefined;
};

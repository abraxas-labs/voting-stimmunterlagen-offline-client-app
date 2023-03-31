import { ElectronService } from '../services/electron.service';
import { SecureDeleteShellService } from '../services/shell/secure-delete-shell.service';

export const SecureDeleteServiceFactory = (electronService: ElectronService): SecureDeleteShellService | undefined => {
  return electronService.isElectronEnvironment() ? new SecureDeleteShellService(electronService) : undefined;
};

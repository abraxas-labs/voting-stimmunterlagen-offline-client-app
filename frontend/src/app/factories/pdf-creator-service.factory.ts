import { ElectronService } from '../services/electron.service';
import { PdfCreatorShellService } from '../services/shell/pdf-creator-shell.service';
import { LogService } from '../services/log.service';

export const PdfCreatorServiceFactory = (electronService: ElectronService, logService: LogService): PdfCreatorShellService | undefined => {
  return electronService.isElectronEnvironment() ? new PdfCreatorShellService(electronService, logService) : undefined;
};

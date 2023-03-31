import { ElectronService } from '../services/electron.service';
import { PdfMergeShellService } from '../services/shell/pdf-merge-shell.service';

export const PdfMergeServiceFactory = (electronService: ElectronService): PdfMergeShellService | undefined => {
  return electronService.isElectronEnvironment() ? new PdfMergeShellService(electronService) : undefined;
};

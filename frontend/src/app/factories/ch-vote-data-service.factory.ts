import { ElectronService } from '../services/electron.service';
import { VotingData } from '../models/voting-data';
import { ChVoteDataShellService } from '../services/shell/ch-vote-data-shell.service';
import { LogService } from '../services/log.service';

export const ChVoteDataServiceFactory = (
  electronService: ElectronService,
  logService: LogService,
): ChVoteDataShellService<VotingData> | undefined => {
  return electronService.isElectronEnvironment() ? new ChVoteDataShellService<VotingData>(electronService, logService) : undefined;
};

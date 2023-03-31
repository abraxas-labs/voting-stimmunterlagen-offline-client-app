import { Routes } from '@angular/router';
import { WelcomePageComponent } from './components/welcome-page/welcome-page.component';
import { SettingsPageComponent } from './components/settings-page/settings-page.component';
import { PreviewPageComponent } from './components/preview-page/preview-page.component';
import { JobOverviewPageComponent } from './components/job-overview-page/job-overview-page.component';
import { DownloadComponent } from './components/download/download.component';
import { ValuePrepareComponent } from './components/value-prepare/value-prepare.component';
import { InitialPageComponent } from './components/initial-page/initial-page.component';
import { VotingCardsConfigurationComponent } from './components/voting-cards-configuration/voting-cards-configuration.component';

export const APP_ROUTES: Routes = [
  {
    path: '',
    redirectTo: '/init',
    pathMatch: 'full',
  },
  {
    path: 'init',
    component: InitialPageComponent,
  },
  {
    path: 'welcome',
    component: WelcomePageComponent,
  },
  {
    path: 'settings',
    component: SettingsPageComponent,
  },
  {
    path: 'prepare',
    component: ValuePrepareComponent,
  },
  {
    path: 'voting-cards-configuration',
    component: VotingCardsConfigurationComponent,
  },
  {
    path: 'preview',
    component: PreviewPageComponent,
  },
  {
    path: 'job-overview',
    component: JobOverviewPageComponent,
  },
  {
    path: 'download',
    component: DownloadComponent,
  },
];

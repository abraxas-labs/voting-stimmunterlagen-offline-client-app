/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { BrowserModule } from '@angular/platform-browser';
import { Injector, LOCALE_ID, NgModule } from '@angular/core';

import { AppComponent } from './components/app/app.component';
import { ALL_COMPONENTS } from './components/all.components';
import { TranslationService } from './services/translation/translation.service';
import { ALL_TRANSLATIONS, TranslationsMap } from './models/translations';
import { TRANSLATIONS as de } from './translations/de';
import { TRANSLATIONS as en } from './translations/en';
import { ALL_PIPES } from './pipes/all.pipes';
import { TranslationContext } from './services/translation/translation.context';
import { registerLocaleData } from '@angular/common';
import { ALL_DIRECTIVES } from './directives/all.directives';
import { ValuePrepareComponent } from './components/value-prepare/value-prepare.component';
import { AppStateService } from './services/app-state.service';
import { StepActionsService } from './services/step-actions.service';
import { PDF_CREATOR_SERVCE } from './services/pdf-creator.service';
import { RouterModule } from '@angular/router';
import { ElectronService } from './services/electron.service';
import { APP_ROUTES } from './app.routes';
import { FormsModule } from '@angular/forms';
import { ZipServiceFactory } from './factories/zip-service.factory';
import { ZIP_SERVICE } from './services/zip.service';
import { CrypticServiceFactory } from './factories/cryptic-service.factory';
import { PdfMergeServiceFactory } from './factories/pdf-merge-service.factory';
import { SpinnerComponent } from './common/spinner/spinner.component';
import { SecureDeleteServiceFactory } from './factories/secure-delete-service.factory';
import { JobContext } from './services/jobs/job-context';
import { AbraNavigationModule } from './common/navigation/navigation.module';
import { SECURE_DELETE_SERVICE } from './services/secure-delete.service';
import { PDFMERGE_SERVICE } from './services/pdf-merge.service';
import { AbraStepNavigationModule } from './common/step-navigation/step-navigation.module';
import { BreadcrumbService } from './services/breadcrumb.service';
import { VotingPropertyService } from './services/voting-property.service';
import { ECH_DELIVERY_SERVICE } from './services/ech-delivery.service';
import { EchDeliveryServiceFactory } from './factories/ech-delivery-service.factory';
import { VotingCardService } from './services/voting-card.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CRYPTIC_SERVICE } from './services/cryptic.service';
import { PdfCreatorServiceFactory } from './factories/pdf-creator-service.factory';
import { SettingsService } from './services/settings.service';
import { TranslationContextFactory } from './services/translation/translation-context.factory';
import { HttpClientModule } from '@angular/common/http';
import { LogService } from './services/log.service';

import localeDe from '@angular/common/locales/de';
import localeEn from '@angular/common/locales/en';

registerLocaleData(localeDe);
registerLocaleData(localeEn);

@NgModule({
  declarations: [...ALL_COMPONENTS, ...ALL_DIRECTIVES, ...ALL_PIPES, ValuePrepareComponent, SpinnerComponent],
  imports: [
    BrowserModule,
    RouterModule.forRoot(APP_ROUTES, { useHash: true }),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    AbraStepNavigationModule,
    AbraNavigationModule,
  ],
  providers: [
    { provide: ALL_TRANSLATIONS, useValue: { de, en } as TranslationsMap },
    { provide: TranslationContext, useFactory: TranslationContextFactory, deps: [Injector] },
    { provide: LOCALE_ID, useValue: window.localStorage.getItem('language') },
    { provide: SECURE_DELETE_SERVICE, useFactory: SecureDeleteServiceFactory, deps: [ElectronService] },
    { provide: ZIP_SERVICE, useFactory: ZipServiceFactory, deps: [ElectronService] },
    { provide: PDF_CREATOR_SERVCE, useFactory: PdfCreatorServiceFactory, deps: [ElectronService, LogService] },
    { provide: ECH_DELIVERY_SERVICE, useFactory: EchDeliveryServiceFactory, deps: [ElectronService, LogService] },
    { provide: CRYPTIC_SERVICE, useFactory: CrypticServiceFactory, deps: [ElectronService, LogService] },
    { provide: PDFMERGE_SERVICE, useFactory: PdfMergeServiceFactory, deps: [ElectronService] },
    AppStateService,
    StepActionsService,
    TranslationService,
    VotingCardService,
    ElectronService,
    BreadcrumbService,
    VotingPropertyService,
    SettingsService,
    LogService,
    JobContext,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}

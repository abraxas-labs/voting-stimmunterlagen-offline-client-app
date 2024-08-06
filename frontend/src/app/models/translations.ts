/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { InjectionToken } from '@angular/core';

export interface Translations {
  [resourceKey: string]: string;
}

export interface TranslationsMap {
  [languageKey: string]: Translations;
}

export const ALL_TRANSLATIONS = new InjectionToken<TranslationsMap>('all-translations');

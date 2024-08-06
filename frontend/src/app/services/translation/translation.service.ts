/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Inject, Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { ALL_TRANSLATIONS, TranslationsMap } from '../../models/translations';
import { TranslationContext } from './translation.context';

@Injectable()
export class TranslationService {
  private languageChangeSubject = new Subject();

  constructor(
    @Inject(ALL_TRANSLATIONS) private readonly allTranslations: TranslationsMap,
    private readonly translationContext: TranslationContext,
  ) {}

  public translate(key: string): string {
    const language = this.translationContext.language;
    return this.allTranslations[language] ? this.allTranslations[language][key] : key;
  }

  public changeLanguage(languageKey: string): void {
    window.localStorage.setItem('language', languageKey);
    this.translationContext.language = languageKey;
    this.languageChangeSubject.next(undefined);
  }

  public get languageChange$(): Observable<any> {
    return this.languageChangeSubject.asObservable();
  }
}

import { Component, Inject } from '@angular/core';
import { ALL_TRANSLATIONS, TranslationsMap } from '../../models/translations';
import { TranslationContext } from '../../services/translation/translation.context';
import { TranslationService } from '../../services/translation/translation.service';

@Component({
  selector: 'app-language-switch',
  templateUrl: './language-switch.component.html',
})
export class LanguageSwitchComponent {
  public languages: string[] = [];

  constructor(
    @Inject(ALL_TRANSLATIONS) private readonly translationsMap: TranslationsMap,
    public readonly translationContext: TranslationContext,
    private readonly translationService: TranslationService,
  ) {
    this.languages = Object.keys(translationsMap);
  }

  public change(language: string): void {
    if (language !== this.translationContext.language) {
      this.translationService.changeLanguage(language);
    }
  }
}

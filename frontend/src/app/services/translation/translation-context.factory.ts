import { TranslationContext } from './translation.context';

export const DEFAULT_LANGUAGE = 'de';
export const TranslationContextFactory = (): TranslationContext => {
  return new TranslationContext(window.localStorage.getItem('language') || DEFAULT_LANGUAGE);
};

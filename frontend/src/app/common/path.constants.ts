import { pathCombine } from '../services/utils/path.utils';

declare const backend: any;

export const TEMP_DIR = pathCombine(backend.getUserDataPathSync(), 'temp');
export const TEMP_PDF_DIR = pathCombine(TEMP_DIR, 'pdfRendering');
export const OUT_PDF_DIR = pathCombine(TEMP_DIR, 'printingPdf');
export const E_VOTING_CONFIG_DIR = pathCombine(TEMP_DIR, 'config');
export const VOTING_DATA_DIR = pathCombine(TEMP_DIR, 'votData');
export const LOG_DIR = pathCombine(TEMP_DIR, 'logs');

export const JSON_CONFIG_FILE = pathCombine(E_VOTING_CONFIG_DIR, 'params.json');
export const APP_STATE_FILE = pathCombine(TEMP_DIR, 'state.json');

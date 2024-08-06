/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

/* SystemJS module definition */
// eslint-disable-next-line no-var
declare var module: NodeModule;

interface NodeModule {
  id: string;
}

interface Window {
  require: Function;
}

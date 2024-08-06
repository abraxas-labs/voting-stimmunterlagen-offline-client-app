/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

export function pathCombine(...paths: string[]): string {
  let result = '';
  let isFirstPath = true;

  for (const path of paths) {
    result += (isFirstPath ? '' : '\\') + path;
    isFirstPath = false;
  }

  return result;
}

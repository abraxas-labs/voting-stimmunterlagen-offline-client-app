/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import * as path from 'path';

export const config = {
  devUrl: {
    pathname: 'http://localhost:4200',
  },
  prodUrl: {
    pathname: path.join(__dirname, 'index.html'),
    protocol: 'file:',
    slashes: true,
  },
};

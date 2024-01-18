module.exports = {
  appId: 'voting-stimmunterlagen-offline-client',
  copyright: 'Copyright © 2024 Abraxas Informatik AG',
  extraMetadata: {
    version: process.env.APP_VERSION,
  },
  buildNumber: '0',
  directories: {
    buildResources: 'src/assets',
    output: 'dist',
  },
  files: [
    '**/*',
    '!**/*.ts',
    '!*.map',
    '!package.json',
    '!package-lock.json',
    {
      from: '../dist',
      filter: ['**/*'],
    },
  ],
  extraResources: [
    {
      from: '../backend/dist',
      to: 'tools',
    },
  ],
  win: {
    target: ['dir'],
    icon: 'src/favicon.ico'
  },
};

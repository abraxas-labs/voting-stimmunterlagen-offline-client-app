module.exports = {
  appId: 'voting-stimmunterlagen-offline-client',
  extraMetadata: {
    version: process.env.APP_VERSION,
  },
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
    './node_modules/@electron/remote/**',
    {
      from: '../backend/dist',
      to: 'tools',
    },
  ],
  win: {
    target: ['dir'],
  },
};
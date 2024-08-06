# ✨ Changelog (`v1.22.1`)

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Version Info

```text
This version -------- v1.22.1
Previous version ---- v1.18.1
Initial version ----- v1.12.12
Total commits ------- 11
```

## [v1.22.1] - 2024-07-05

### 🔒 Security

- update voting library

## [v1.22.0] - 2024-07-04

### 🔒 Security

- Migrate to bouncy castle
- Verify post file signature

## [v1.21.0] - 2024-06-20

### 🔄 Changed

- angular 17 update

## [v1.20.2] - 2024-05-28

### 🔄 Changed

- support new ech-0045 format

## [v1.20.1] - 2024-05-17

### 🔄 Changed

- show position of empty candidates or write ins on multi mandate majority election

## [v1.20.0] - 2024-03-26

### 🆕 Added

- Support post config xml v6

## [v1.19.0] - 2024-03-01

### 🔄 Changed

- update to .net 8

### 🔒 Security

- apply patch policy

## [v1.18.4] - 2024-02-28

### 🔒 Security

- patch for dependency vulnerability CVE-2023-45133 (babel)
- patch for dependency vulnerability CVE-2023-28154 (webpack)
- patch for dependency vulnerability CVE-2023-45857 (axios)
- patch for dependency vulnerability CVE-2023-46234 (browsserify-sign)
- patch for dependency vulnerability CVE-2023-26159 (follow-redirects)
- patch for dependency vulnerability CVE-2023-44270 (postcss)

## [v1.18.3] - 2024-02-27

### 🔄 Changed

- Show error message correctly when the import fails

## [v1.18.2] - 2024-02-26

### 🆕 Added

- add SBOM and DependencyTrack integration

### 🔄 Changed

- update ci-cd templates

### ⚠️ Deprecated

- deprecate digital signature

## [v1.18.1] - 2024-01-18

### :arrows_counterclockwise: Changed

- use static copyright to ensure a reproducible build at the turn of the year

## [v1.18.0] - 2023-12-20

### 🆕 Added

- Add updated eCH from voting lib
- Add updated Post models

## [v1.17.3] - 2023-11-30

### 🔄 Changed

- Frontend code improvements

### 🔒 Security

- Update electron version to 23.3.13

### 🔄 Changed

- Change pdf output naming scheme

## [v1.17.2] - 2023-08-24

### 🔄 Changed

- hide internal registry information in bug bounty artifact preparation.

## [v1.17.1] - 2023-08-23

### 🔄 Changed

- Fix async issue in pdf generator

## [v1.17.0] - 2023-08-23

### 🔄 Changed

- Publish dotnet packages as multi file packages

## [v1.16.0] - 2023-08-22

### 🔄 Changed

- Update eai and lib dependency to deterministic version

### 🔄 Changed

- Process elections with write ins and without candidates correctly

### 🔄 Changed

- refactor security and code review findings

### 🔄 Changed

- skip signing process if certificate is not provided.

### 🆕 Added

- Deterministic code signing

### 🔒 Security

- Only accept .p12 certificates as signing certificate

### 🆕 Added

- Support for large input files (several gb)

### 🔄 Changed

- Update Post XSD schema
- Rename chVoteToJsonConverter to EchDeliveryJsonConverter and refactor
- Ech-0045 upload for swiss abroad addresses
- Display accumulated candidate in proportional election correctly on the voting card

### 🔄 Changed

- Deterministic build number

### ❌ Removed

- Salutation on swiss abroad voters

### 🔄 Changed

- Included attachment stations on voting card

### 🔒 Security

- fix(VOTING-3073): upgrade Newtonsoft.Json to mitigate insecure defaults for versions <13.0.1 (CWE-755)

### 🔄 Changed

- Display the app version correctly on the settings page

### 🆕 Added

- Physical address mapping of the voter according ISO

### 🔒 Security

- create hash for release package
- use sha245sum tool for generating and verifying hash values

### 🔄 Changed

- App icon

### 🔄 Changed

- Fixed settings page
- Updated shared lib

### 🔄 Changed

- update crypto library and readme

### 🔄 Changed

- Use latest dockerfile to build
- Update shared library

### 🔄 Changed

- Prevent invalid settings input

### 🔒 Security

- Increase number of overwrite passes for sDelete to '10'

### 🆕 Added

- add hash generated before signing as part of the release

### 🔒 Security

- fix(VOTING-2505): pin docker base image to digest

### 🔄 Changed

- Fixed SCR findings

### 🆕 Added

- add additional hash file for non-signed artefacts

### 🆕 Added

- add frontend and backend linting

### 🔄 Changed

- Display callname instead of firstname of a candidate on a voting card

### 🔄 Changed

- Deterministic raw build
- Fixed Logging

### 🔒 Security

- Removed shell access in production to prevent remote code execution
- Added command whitelist

### 🔄 Changed

- Updated prince to v15

### 🔒 Security

- apply signature to assemblies for every release.

### 🔄 Changed

- show certificate thumprint on preview

### 🔒 Security

- Separated main and renderer process responsibilities

### 🔄 Changed

- apply security review and sonarqube report refinments.

### 🔄 Changed

- Publish backend tools as single file

### 🔒 Security

- Hardened xml parser configuration

### 🔄 Changed

- Cleanup the frontend package jsons
- Restructured the backend

### 🔒 Security

- Added new encryption algorithm

### 🔄 Changed

- Update PDF.js dependency

### 🔄 Changed

- get application version from electron app context.

### 🔒 Security

- protect logging against log forging attacks

### 🔄 Changed

- Updated frontend and backend dependencies

### 🔄 Changed

- include not electable candidates from empty list in majority election

### 🔄 Changed

- set custom technology and source directory for front- and backend Inventaria jobs

### 🆕 Added

- integrate inventaria in ci pipeline

### 🔒 Security

- integrate dependency check for front- and backend part.

## [v1.15.6] - 2023-08-22

### 🔄 Changed

- Process elections with write ins and without candidates correctly

## [v1.15.5] - 2023-08-17

### 🔄 Changed

- refactor security and code review findings

## [v1.15.4] - 2023-08-15

### 🔄 Changed

- skip signing process if certificate is not provided.

## [v1.15.3] - 2023-08-11

### 🔄 Changed

- fix typo in readme, update code docs

## [v1.15.2] - 2023-07-25

### 🆕 Added

- Deterministic code signing

## [v1.15.1] - 2023-07-24

### 🔒 Security

- Only accept .p12 certificates as signing certificate

## [v1.15.0] - 2023-07-10

### 🆕 Added

- Support for large input files (several gb)

## [v1.14.0] - 2023-05-23

### 🔄 Changed

- Update Post XSD schema
- Rename chVoteToJsonConverter to EchDeliveryJsonConverter and refactor
- Ech-0045 upload for swiss abroad addresses
- Display accumulated candidate in proportional election correctly on the voting card

## [v1.13.5] - 2023-05-22

### 🔄 Changed

- Deterministic build number

## [v1.13.4] - 2023-04-26

### ❌ Removed

- Salutation on swiss abroad voters

## [v1.13.3] - 2023-04-19

### 🔄 Changed

- Included attachment stations on voting card

## [v1.13.2] - 2023-04-11

### 🔒 Security

- fix(VOTING-3073): upgrade Newtonsoft.Json to mitigate insecure defaults for versions <13.0.1 (CWE-755)

## [v1.13.1] - 2023-04-06

### 🔄 Changed

- Display the app version correctly on the settings page

## [v1.13.0] - 2023-04-06

### 🆕 Added

- Physical address mapping of the voter according ISO

## [v1.12.14] - 2023-04-05

### 🔒 Security

- create hash for release package
- use sha245sum tool for generating and verifying hash values

## [v1.12.13] - 2023-04-04

### 🔄 Changed

- App icon

## [v1.12.12] - 2023-03-31

### 🎉 Initial release for Bug Bounty

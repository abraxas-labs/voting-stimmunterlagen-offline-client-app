# ✨ Changelog (`v1.26.3`)

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Version Info

```text
This version -------- v1.26.3
Previous version ---- v1.25.0
Initial version ----- v1.12.12
Total commits ------- 6
```

## [v1.26.3] - 2026-03-17

### 🔄 Changed

- improve download ux

## [v1.26.2] - 2026-02-06

### 🔄 Changed

- extend CD pipeline with enhanced bug bounty publication workflow

## [v1.26.1] - 2025-11-18

### 🔄 Changed

- fix empty list mapping in post print v2

## [v1.26.0] - 2025-11-14

### 🔄 Changed

- update angular to 20.3.6 and dependencies to latest patch versions

## [v1.25.1] - 2025-11-12

### 🔄 Changed

- add compatibility for xml sign tool v1.4 and updated system properties in v1.5
- align xml signature tool system properties with v1.5 notation
- update `-Ddirect.trust.keystore.location` to `-Ddirect-trust.keystore.location`
- update `-Ddirect.trust.keystore.password.location` to `-Ddirect-trust.password.location`

## [v1.25.0] - 2025-10-16

### 🆕 Added

- support post config v7 and print v2 xml

## [v1.24.0] - 2025-09-25

### 🆕 Added

- support eCH-0045 v6 import

## [v1.23.7] - 2025-08-28

### 🔄 Changed

- bump Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography to 2.2.4

## [v1.23.6] - 2025-08-08

### 🔄 Changed

- add mono-repository support to report dependencies for independent front- and backend projects.

## [v1.23.5] - 2025-07-25

### 🔒 Security

- update pdf.js due to security vulnerability
- explicitly prevent script execution in pdf.js

## [v1.23.4] - 2025-07-11

### 🆕 Added

- add return address addition to e-voting template

## [v1.23.3] - 2025-06-04

### 🆕 Added

- add attachment stations to municipality

## [v1.23.2] - 2025-04-11

### 🔄 Changed

- batch encrypt pdfs

## [v1.23.1] - 2025-03-03

### ❌ Removed

- remove attachment stations from e-voting config

## [v1.23.0] - 2024-10-18

### 🆕 Added

- add STISTAT flag to municipality

## [v1.22.7] - 2024-10-11

### 🔄 Changed

- optional ech-0045 swiss abroad extensions

## [v1.22.6] - 2024-09-09

### 🔄 Changed

- migrate container registry

## [v1.22.5] - 2024-08-28

### :arrows_counterclockwise: Changed

- update bug bounty template reference
- patch ci-cd template version, align with new defaults

## [v1.22.4] - 2024-08-22

### 🔄 Changed

- update create-hashes script to exclude *.deps.json files

## [v1.22.3] - 2024-08-13

### 🔄 Changed

- enable continuous integration build property for dotnet CLI
- disable source-link feature for release build to ensure deterministic in trusted build procedure.

## [v1.22.2] - 2024-08-12

### 🔄 Changed

- prevent source revision from being included in release builds to preserve deterministic builds.

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

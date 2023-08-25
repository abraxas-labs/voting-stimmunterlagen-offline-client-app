# VOTING Stimmunterlagen Offline Client

This repository contains the source code of the `VOTING Stimmunterlagen Offline-Client`. The client is used to
generate e-voting voting cards based on input files (e-voting codes, print data, layout, contest, etc.) supplied by subsystems.

The generation of e-voting voting cards requires high security standards. For the generation, raw data from the online application VOTING Stimmunterlagen (Abraxas Informatik AG) and the e-voting application (Post AG) must be imported into the application. Before generation, various configurations such as the sorting of the voting cards can be specified. One of the most important features beside the generation of voting cards is the process of securing the voting cards by encrypting and signing the data. This ensures that the voting cards cannot be changed until they are printed.

## Project parts

* [Frontend](./frontend/README.md)  
  Web-based UI and Electron-based desktop application for generating the (e)Voting Card print data.
* [Backend](./backend/README.md)  
  Visual Studio Solution with NetStandard2.0 libraries and .NET Core tools. Used by the UI project.

## Requirements

* NodeJs v16.0.0
* dotnet 6
* PrinceXML License (`backend/3rdPartyLibs/prince/[Architecture]/license/license.dat`)

## Development

To start developing components in the webapp, use `npm run start` in `frontend`.

## Build executable

Run `./build.sh` to build the package to `frontend/packages`.

## Dependencies

### PDF.js

PDF.js is not included as a node module because it does not ship the default [viewer files](https://github.com/mozilla/pdf.js/issues/11274).
The pdfjs bundle can be downloaded from [here](https://mozilla.github.io/pdf.js/getting_started/) and is included per `frontend/src/assets/pdfjs-dist`.

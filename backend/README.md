# Voting.Stimmunterlagen.OfflineClient

## Voting.Stimmunterlagen.OfflineClient.sln

Visual Studio 2017 solution with libraries and tools for Voting card print generation.

## Important notes

* __All__ projects that reference the `HtmlGeneration.RazorLight` project need to include `<PreserveCompilationContext>true</PreserveCompilationContext>` in an unconditional PropertyGroup in their `.csproj` file.
* __All__ executable projects that reference the `PdfGeneration.Prince` project need to include `<Import Project="..\3rdPartyLibs\prince.targets" />` in their `.csproj` file.

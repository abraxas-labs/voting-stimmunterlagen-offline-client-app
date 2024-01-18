rm -rf ./Models

dotnet xscgen ./Schemas/evoting-config-5-0.xsd -n EVoting.Config --netCore --separateFiles --output=./Models --nullable --order --collectionSettersMode=Public --collectionType='System.Collections.Generic.List`1' --commandArgs-
dotnet xscgen ./Schemas/evoting-print-1-3.xsd -n EVoting.Print --netCore --separateFiles --output=./Models --nullable --order --collectionSettersMode=Public --collectionType='System.Collections.Generic.List`1' --commandArgs-
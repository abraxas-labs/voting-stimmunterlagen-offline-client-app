rm -rf ./Models

dotnet xscgen ./Schemas/xmldsig-core-schema.xsd -n EVoting.XmldSig --netCore --separateFiles --output=./Models --nullable --order --collectionSettersMode=Public --collectionType='System.Collections.Generic.List`1' --commandArgs-
dotnet xscgen ./Schemas/evoting-config-7-0.xsd -n EVoting.Config_7_0 -n http://www.w3.org/2000/09/xmldsig#=EVoting.XmldSig --netCore --separateFiles --output=./Models --nullable --order --collectionSettersMode=Public --collectionType='System.Collections.Generic.List`1' --commandArgs-
dotnet xscgen ./Schemas/evoting-print-2-0.xsd -n EVoting.Print_2_0 -n http://www.w3.org/2000/09/xmldsig#=EVoting.XmldSig --netCore --separateFiles --output=./Models --nullable --order --collectionSettersMode=Public --collectionType='System.Collections.Generic.List`1' --commandArgs-

dotnet xscgen ./Schemas/evoting-config-6-0.xsd -n EVoting.Config_6_0 --netCore --separateFiles --output=./Models --nullable --order --collectionSettersMode=Public --collectionType='System.Collections.Generic.List`1' --commandArgs-
dotnet xscgen ./Schemas/evoting-print-1-3.xsd -n EVoting.Print_1_3 --netCore --separateFiles --output=./Models --nullable --order --collectionSettersMode=Public --collectionType='System.Collections.Generic.List`1' --commandArgs-
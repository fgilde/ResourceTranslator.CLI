# ResourceTranslator.CLI
This tool can translate your resource files with Microsoft Cognitive Service for translations
To use this tool an azure TranslateApp is required.
To create an Translate app and leran more about read this https://docs.microsoft.com/en-us/azure/cognitive-services/translator/quickstart-translator?tabs=csharp


## Change log
- 1.0.7 Option to skip existing output files
- 1.0.7 Chunking Targets and Contents for Text files 
- 1.0.6 Allow big text content files as default if no dictionary can created. (For example usefull to translate *.md files)
- 1.0.2 Only tranlate if translations are missing or overwrite option is passed

## General

To install it

```cmd
dotnet tool install --global ResourceTranslator.CLI
```

To update it

```cmd
dotnet tool update ResourceTranslator.CLI -g
```

The easiest way to run it
```cmd
 resourceTranslator --optionsfile "C:\PathToYourOptions\sampleOptions.json"
```

The passed options file can look like this for example
```json
{
  "FileName": "C:\\dev\\myProject\\src\\Shared\\Resources\\en-US.yml",
  "TextTranslationEndpoint": "https://api.cognitive.microsofttranslator.com/",
  "ApiKey": "<your cognitive service api key>",
  "FileOutputFormat": "{FileName}.{Culture}.{Extension}",
  "OutputDir": "C:\\dev\\myProject\\src\\Shared\\Resources",
  "TargetCultures": "de-DE, es-ES, it-IT, sv-SE, en-GB",
  "SourceCulture": null,
  "Region": "germanywestcentral",
  "OutputFormat": null,
  "OverwriteExistingValuesWithNewTranslations": false,
  "AutoSort": true
}
```

Based on your input options for `TargetCultures` this tool creates translations for given input file `FileName`

![image](https://user-images.githubusercontent.com/11070717/138893321-440e103e-74ee-4cfd-99bb-3a0e2bda2069.png)

##### Notice!: If a result file already exists only missing translations will be added.


## Options

| Name          |      Alias   | Sample        | Is Required    | Description
| :---          |    :----:    |:---           |     :----:     |:---                      |
| FileName      | f            | `-f "C:\path\file.json"`   | yes           | The main input file to translate                   |
| TextTranslationEndpoint      | endpoint            | `-endpoint "https://api.cognitive.microsofttranslator.com/"`   | yes           | Endpoint for translation service
| ApiKey      | key            | `-key "<your api key>"`   | yes           | Api key for translation service
| Region      | region            | `-region "germanywestcentral"`   | yes           | Region where your azure cognitive service is stored
| FileOutputFormat      | of            | `-of "Generated_{FileName}_for_culture_{Culture}.{Extension}"`   | no           | Format to save out put file
| OutputDir      | outdir            | `-outdir "C:\otherdir\"`   | no           | Optional path to store result files in. Default is same dir as input file
| TargetCultures      | target            | `-target "de-DE, it-IT"`   | yes           | Target cultures to generate translations for. Split by , or ; possible
| OutputFormat      | format            | `-format "Json"`   | no           | Default same as input but if you want to convert yaml to json for example you can specify a format here
| OverwriteExistingValuesWithNewTranslations      | overwritevalues            | `-overwritevalues "True"`   | no           | If this is true existing target resources will overwridden and not merged
| AutoSort      | sort            | `-sort "True"`   | no           | If this is true all result files and the input file file sorted automatically Asc
| SkipExistingOutputs      | skip            | `-skip "True"`   | no           | If this is true existing output files will not be changed (This setting is ignored for dictionaries and only used for Text based files)


#### Notice!: You can combine passing options file and overwrite only some parameters.
In this example all parameters are stored in a `sampleOptions.json` but api key and source file will be overwridden by call
```cmd
resourceTranslator --optionsfile "C:\PathToYourOptions\sampleOptions.json" -key "diferentApiKey" -f "PathToMyFile.json"
```

Because this tool only translate if translations are missing by default you can easially use it on every Build to ensure your resource files are complete and sorted.

```msbuild
  <Target Name="Translate" AfterTargets="Build" Condition="'$(Configuration)' == 'Debug'">
    <Exec IgnoreExitCode="true" Command="dotnet tool install --global ResourceTranslator.CLI" />
    <Exec Command="resourceTranslator --optionsfile $(ProjectDir)resourceTranslationOptions.json -f $(ProjectDir)Resources\en-US.yml" />
  </Target>
  ```


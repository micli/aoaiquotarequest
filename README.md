# aoaiquotarequest
An automatic tool for quota requesting.

------

This is an automatic tool for request quota by https://aka.ms/oai/stuquotarequest. Becasue Power Platform form always allow only ***Single subscription*** and ***Single region*** per request. It drives sales crazy.

This tool can cross-platform running both on Windows and macOS. Because it written by dotnet7.0. Considering most Windows uers have been already install Microsoft Edge. The simulate operations leverage Microsoft Edge to automatically create quota request.

## How to use?

It is needed to create a configuration file before start requesting. You can make a copy from sample.json file. And then you open it and fill **REAL DATA** into json file. You also need to select regions that you want to ask quota.

And then you can directly run executeable file with json file as ONLY parameter.

```bash
dotnet run myrequest.json
```
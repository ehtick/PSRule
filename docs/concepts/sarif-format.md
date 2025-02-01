# SARIF Output

PSRule uses a JSON structured output format called the

SARIF format to report results.
The SARIF format is a standard format for the output of static analysis tools.
The format is designed to be easily consumed by other tools and services.

## Runs

When running PSRule executed a run will be generated in `runs` containing details about PSRule and configuration details.

## Invocation

The `invocations` property reports runtime information about how the run started.

### RuleConfigurationOverrides

When a rule has been overridden in configuration this invocation property will contain any level overrides.

## Examples

### Successful result

```json
{
  "$schema": "https://schemastore.azurewebsites.net/schemas/json/sarif-2.1.0-rtm.5.json",
  "version": "2.1.0",
  "runs": [
    {
      "tool": {
        "driver": {
          "guid": "0130215d-58eb-4887-b6fa-31ed02500569",
          "name": "PSRule",
          "organization": "Microsoft Corporation",
          "semanticVersion": "2.9.0",
          "informationUri": "https://aka.ms/ps-rule"
        },
        "extensions": [
          {
            "guid": "7bfb5234-1648-4e52-956c-42f303d416cb",
            "name": "PSRule.Rules.MSFT.OSS",
            "organization": "Microsoft Corporation",
            "version": "1.1.0",
            "informationUri": "https://github.com/microsoft/PSRule.Rules.MSFT.OSS",
            "associatedComponent": {
              "name": "PSRule"
            }
          }
        ]
      },
      "automationDetails": {
        "id": "CI repository scan/workspace/1",
        "description": {
          "text": "An analysis scan that checks repository files."
        },
        "correlationGuid": "00000000-0000-0000-0000-000000000000"
      },
      "invocations": [
        {
          "executionSuccessful": true
        }
      ],
      "versionControlProvenance": [
        {
          "repositoryUri": "https://github.com/microsoft/PSRule",
          "revisionId": "2a3671213e5768aad6af7f132e418d1a4368a3c5",
          "branch": "main",
          "mappedTo": {
            "uriBaseId": "REPO_ROOT"
          }
        }
      ],
      "originalUriBaseIds": {
        "REPO_ROOT": {
          "description": {
            "text": "The directory into which the repo was cloned."
          }
        }
      },
      "results": [],
      "columnKind": "utf16CodeUnits"
    }
  ]
}
```

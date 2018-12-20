## What it does ##

It scans a Azure DevOps project.

## Things you need ##

You need to configure the organization, team project and PAT for your project. 
The organization is the part of the url dev.azure.com/<organization> or <organization>.visualstudio.com. For example: **somecompany** for https://somecompany.visualstudio.com. 
Another thing is your PAT, the Personal Access Token. This needs to  be _unencoded_ and configured in appsettings.user.json in the test projects:

```
{
  "token": "asdfjlkjasdf834234lkadf234us02sdf3"
}
```

You can also override the organization and/or project from there.
LEON is my machinename btw.

## How to use ##

By running the Web application the default project will be scanned. It shows the Releasedefinitions found with the Environments and the Builds from the Artifacts.
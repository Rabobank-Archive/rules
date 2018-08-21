## What it does ##

It scans a VSTS-Project.

## Things you need ##

You need to configure the accountname and PAT for your project. The accountname is the part of the url before .visualstudio.com. For example: **somecompany** for https://somecompany.visualstudio.com. Another thing is your PAT, the Personal Access Token. This needs to  be Base64 encoded and configured in appsettings.machinename.json where **machinename** is the name of your machine. You can find it by executing *hostname* in a shell. For example my appsettings.LEON.json contains:

```
{
  "Base64PAT": "OmxQWTQWIOUTQWITWIUHREWHHWOIEHOIEWH=",
  "Accountname": "somecompany"
}
```

LEON is my machinename btw.



## How to use ##

By running the Web application the default project will be scanned. It shows the Releasedefinitions found with the Environments and the Builds from the Artifacts.
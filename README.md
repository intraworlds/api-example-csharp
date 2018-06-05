### Intended use
This is an example connector to the IntraWorlds REST API, written in C#. The entire package is built into an [official image](https://hub.docker.com/r/intraworlds/api-csharp/). This repository contains Visual Studio solution with two projects:

* *IW.API.CSharp* is a project containing the application.

* *IW.API.CSharp.Tests* is a project containing tests for the app.

### Installation

1. Clone this repository
2. cd into IW.API.CSharp
3. ```docker-compose up```

### Importing projects to Visual Studio

1. Clone this repository
2. cd into IW.API.CSharp
3. You should see file IW.API.CSharp.sln
4. Open Visual Studio
5. Click on File -> Open -> Project or Solution
6. Select file IW.API.CSharp.sln mentioned in step 2.
7. Complete, now you should see imported solution with two projects.

### Running tests

1. cd into IW.API.CSharp
2. You should see file IW.API.CSharp.sln
3. Open Visual Studio
4. Click on File -> Open -> Project or Solution
5. Select file IW.API.CSharp.sln mentioned in step 2.
6. Click on Test -> Run -> All tests.

### Integrated StyleCop
While developing, NuGet extension StyleCop will ensure coding standards. It checks automatically all C# files except Manager for OAuth.
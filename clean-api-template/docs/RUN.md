## How to Run Project

There are multiple ways to run this project using two pre-configured environments `local` and `docker`.  The `appsettings.json` files are configured to run the projects in different ways and connect different application logging mechanisms.

## Create Project
The project was created using my nugget package (https://www.nuget.org/packages/MsClean.Template/)

### DOTNET CLI

The CLI makes everything easier in .NET engineering.  Running this application is no different.  This task is aided by the launchsettings.json file that sets all the parameters required to run the API on your local machine.  Ports, environments and logging are all configured in the settings, so the only command required is the following single line statement.  

``` bash
dotnet clean
dotnet build
dotnet test
dotnet run --project ./src/Presentation
```

### EF MIGRATION LOCAL
``` bash
dotnet ef migrations add MigrationName --project src/Infrastructure
dotnet ef database update --project src/Infrastructure


dotnet ef migrations remove --project src/Infrastructure
```

### DOCKER COMPOSE

Docker compose makes it easier to start multiple containers at one time and manage their configuration from one file (`docker-compose.yml`). 

- App - https://localhost:5001/swagger

To start the Containers

``` bash
docker-compose up --build -d
```

To shut down the containers

``` bash
docker-compose down -v
```

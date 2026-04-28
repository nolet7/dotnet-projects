# DotNet Azure Web UI

A beginner-friendly ASP.NET Core Razor Pages web application that can run locally, be pushed to GitHub, and be deployed to Azure App Service using Azure DevOps Pipelines.

## What this project includes

- ASP.NET Core web UI using Razor Pages
- Styled homepage visible in the browser
- `/api/health` endpoint for smoke testing
- Azure DevOps pipeline file with comments
- Detailed Azure setup guide for a new Azure environment

## Project structure

```text
.
├── DotNetAzureWebUi.Web/
│   ├── DotNetAzureWebUi.Web.csproj
│   ├── Program.cs
│   ├── Pages/
│   └── wwwroot/
├── docs/
│   └── azure-setup-guide.md
├── azure-pipelines.yml
└── README.md
```

## Run locally

```bash
cd dotnet-projects

dotnet restore DotNetAzureWebUi.Web/DotNetAzureWebUi.Web.csproj

dotnet build DotNetAzureWebUi.Web/DotNetAzureWebUi.Web.csproj

dotnet run --project DotNetAzureWebUi.Web --urls "http://localhost:5080"
```

Open:

```text
http://localhost:5080
```

Health check:

```bash
curl http://localhost:5080/api/health
```

## Azure setup

Start here:

```text
docs/azure-setup-guide.md
```

That guide covers:

1. Azure CLI installation and login
2. Azure resource group creation
3. Azure App Service Plan creation
4. Azure Linux Web App creation
5. Runtime configuration
6. Azure DevOps project setup
7. Azure DevOps service connection
8. Azure Pipeline setup
9. Deployment validation
10. Troubleshooting

## Azure DevOps pipeline

The deployment pipeline is defined here:

```text
azure-pipelines.yml
```

Before running the pipeline, replace these variables:

```yaml
appName: 'REPLACE_WITH_YOUR_AZURE_APP_SERVICE_NAME'
azureSubscription: 'REPLACE_WITH_YOUR_AZURE_SERVICE_CONNECTION_NAME'
```

## Recommended beginner workflow

1. Clone this repo.
2. Run the app locally.
3. Create Azure resources using `docs/azure-setup-guide.md`.
4. Create the Azure DevOps service connection.
5. Update `azure-pipelines.yml` with your App Service name and service connection name.
6. Run the pipeline.
7. Open the Azure App Service URL in your browser.

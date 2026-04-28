# Azure Setup Guide for DotNet Azure Web UI

This guide creates the Azure resources needed to run the `DotNetAzureWebUi.Web` application from this repository:

```text
https://github.com/nolet7/dotnet-projects.git
```

## Current Azure Status

Your Azure CLI is working.

Your active subscription is:

```text
Name: Subscription 1
SubscriptionId: a6d8cc7d-b602-41b8-a56c-d33b0bb78004
TenantId: 6043f635-7c6a-4834-9dc2-01fa7b457c22
State: Enabled
IsDefault: True
```

Do not hardcode the subscription ID inside public application files.

---

## Issue Encountered

The resource group was created successfully:

```text
rg-dotnet-webui-dev-eastus-001
```

But the App Service Plan failed with:

```text
Operation cannot be completed without additional quota.
Current Limit (Free VMs): 0
Amount required for this deployment (Free VMs): 1
```

This means your Azure subscription currently has no quota for the Free App Service tier in `eastus`.

Because the plan failed, this command also failed:

```text
The plan 'asp-dotnet-webui-dev-eastus-001' doesn't exist.
```

---

# Option 1: Use Basic B1 App Service Plan

Use this option if you want to continue now.

Basic B1 is not free, so delete the resource group after testing if you do not want charges.

## Step 1: Open PowerShell

Use PowerShell, not Git Bash.

## Step 2: Set variables

```powershell
# Resource group already created successfully.
$RG_NAME="rg-dotnet-webui-dev-eastus-001"

# Azure region.
$LOCATION="eastus"

# App Service Plan name.
$APP_SERVICE_PLAN="asp-dotnet-webui-dev-eastus-001"

# Web App name.
# This must be globally unique across Azure.
$WEB_APP_NAME="app-dotnet-webui-dev-nolet7-001"

# Runtime stack.
# Use DOTNETCORE:8.0 if your project targets net8.0.
# Use DOTNETCORE:10.0 only if Azure App Service supports it in your region.
$RUNTIME="DOTNETCORE:8.0"
```

## Step 3: Confirm subscription

```powershell
az account show --output table
```

Expected result should show:

```text
Subscription 1
```

## Step 4: Verify resource group exists

```powershell
az group show `
  --name $RG_NAME `
  --output table
```

## Step 5: Create App Service Plan using B1

```powershell
az appservice plan create `
  --name $APP_SERVICE_PLAN `
  --resource-group $RG_NAME `
  --location $LOCATION `
  --sku B1 `
  --is-linux
```

## Step 6: Verify App Service Plan

```powershell
az appservice plan show `
  --name $APP_SERVICE_PLAN `
  --resource-group $RG_NAME `
  --output table
```

## Step 7: Create Web App

```powershell
az webapp create `
  --name $WEB_APP_NAME `
  --resource-group $RG_NAME `
  --plan $APP_SERVICE_PLAN `
  --runtime $RUNTIME
```

## Step 8: Verify Web App

```powershell
az webapp show `
  --name $WEB_APP_NAME `
  --resource-group $RG_NAME `
  --output table
```

## Step 9: Open Web App

```powershell
Start-Process "https://$WEB_APP_NAME.azurewebsites.net"
```

At this stage, Azure may show a default App Service page because the application code has not been deployed yet.

---

# Option 2: Request Free Tier Quota

Use this option if you want to avoid B1 cost.

In Azure Portal:

```text
Subscriptions
→ Subscription 1
→ Usage + quotas
→ Search for Free VMs
→ Request increase
```

Request:

```text
Free VMs: 1
Region: eastus
```

After quota is approved, rerun:

```powershell
az appservice plan create `
  --name $APP_SERVICE_PLAN `
  --resource-group $RG_NAME `
  --location $LOCATION `
  --sku F1 `
  --is-linux
```

Then create the web app:

```powershell
az webapp create `
  --name $WEB_APP_NAME `
  --resource-group $RG_NAME `
  --plan $APP_SERVICE_PLAN `
  --runtime $RUNTIME
```

---

# Deploy the .NET App Manually First

Before using Azure DevOps, deploy manually once to confirm Azure App Service works.

## Step 1: Go to your local repo

```powershell
cd C:\Users\Lateef\Downloads\dotnet-projects\dotnet-azure-web-ui
```

If your repo is in another location, use that path.

## Step 2: Check project file

```powershell
Get-Content .\DotNetAzureWebUi.Web\DotNetAzureWebUi.Web.csproj
```

If the project targets:

```xml
<TargetFramework>net10.0</TargetFramework>
```

but Azure App Service does not support your selected .NET runtime yet, change it to:

```xml
<TargetFramework>net8.0</TargetFramework>
```

Recommended beginner setting:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
```

## Step 3: Restore dependencies

```powershell
dotnet restore .\DotNetAzureWebUi.Web\DotNetAzureWebUi.Web.csproj
```

## Step 4: Build app

```powershell
dotnet build .\DotNetAzureWebUi.Web\DotNetAzureWebUi.Web.csproj
```

## Step 5: Publish app

```powershell
dotnet publish .\DotNetAzureWebUi.Web\DotNetAzureWebUi.Web.csproj `
  --configuration Release `
  --output .\publish
```

## Step 6: Create ZIP package

```powershell
Compress-Archive `
  -Path .\publish\* `
  -DestinationPath .\dotnet-webui.zip `
  -Force
```

## Step 7: Deploy ZIP to Azure App Service

```powershell
az webapp deploy `
  --resource-group $RG_NAME `
  --name $WEB_APP_NAME `
  --src-path .\dotnet-webui.zip `
  --type zip
```

## Step 8: Open deployed app

```powershell
Start-Process "https://$WEB_APP_NAME.azurewebsites.net"
```

## Step 9: Test health endpoint

```powershell
Invoke-RestMethod "https://$WEB_APP_NAME.azurewebsites.net/api/health"
```

Expected result:

```text
status      : healthy
app         : DotNet Azure Web UI
timeUtc     : ...
```

---

# Azure DevOps Setup

After manual deployment works, configure Azure DevOps.

## Step 1: Create Azure DevOps Project

Go to:

```text
https://dev.azure.com
```

Create project:

```text
dotnet-projects-devops
```

Recommended settings:

```text
Visibility: Private
Version control: Git
Work item process: Basic
```

## Step 2: Create Service Connection

In Azure DevOps:

```text
Project Settings
→ Service connections
→ New service connection
→ Azure Resource Manager
```

Choose:

```text
Workload identity federation
```

Then choose:

```text
Service principal automatic
```

Use this service connection name:

```text
sc-azure-dotnet-webui-dev
```

Scope it to:

```text
Subscription: Subscription 1
Resource group: rg-dotnet-webui-dev-eastus-001
```

Enable:

```text
Grant access permission to all pipelines
```

---

# Update azure-pipelines.yml

Replace the pipeline variables with your real Azure names.

```yaml
variables:
  buildConfiguration: 'Release'

  appName: 'app-dotnet-webui-dev-nolet7-001'

  azureSubscription: 'sc-azure-dotnet-webui-dev'

  projectPath: 'DotNetAzureWebUi.Web/DotNetAzureWebUi.Web.csproj'
```

If your `.csproj` uses `net8.0`, use this SDK version in the pipeline:

```yaml
- task: UseDotNet@2
  displayName: 'Install .NET SDK'
  inputs:
    packageType: 'sdk'
    version: '8.x'
```

If your `.csproj` uses `net10.0`, use:

```yaml
- task: UseDotNet@2
  displayName: 'Install .NET SDK'
  inputs:
    packageType: 'sdk'
    version: '10.x'
```

---

# Recommended azure-pipelines.yml for net8.0

Use this if you change the project to `net8.0`:

```yaml
# This pipeline runs when code is pushed to the main branch.
trigger:
- main

# Use a Microsoft-hosted Ubuntu build agent.
pool:
  vmImage: 'ubuntu-latest'

# Pipeline variables.
variables:
  # Build app in Release mode.
  buildConfiguration: 'Release'

  # Azure App Service name.
  appName: 'app-dotnet-webui-dev-nolet7-001'

  # Azure DevOps service connection name.
  azureSubscription: 'sc-azure-dotnet-webui-dev'

  # Path to the .NET project file.
  projectPath: 'DotNetAzureWebUi.Web/DotNetAzureWebUi.Web.csproj'

steps:
# Install .NET 8 SDK on the pipeline agent.
- task: UseDotNet@2
  displayName: 'Install .NET SDK'
  inputs:
    packageType: 'sdk'
    version: '8.x'

# Show installed .NET SDK information.
- script: |
    dotnet --info
  displayName: 'Show .NET SDK information'

# Restore NuGet packages.
- task: DotNetCoreCLI@2
  displayName: 'Restore dependencies'
  inputs:
    command: 'restore'
    projects: '$(projectPath)'

# Build the app.
- task: DotNetCoreCLI@2
  displayName: 'Build application'
  inputs:
    command: 'build'
    projects: '$(projectPath)'
    arguments: '--configuration $(buildConfiguration) --no-restore'

# Publish the app into a deployable ZIP package.
- task: DotNetCoreCLI@2
  displayName: 'Publish application'
  inputs:
    command: 'publish'
    publishWebProjects: false
    projects: '$(projectPath)'
    arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)/publish'
    zipAfterPublish: true

# Deploy the ZIP package to Azure App Service.
- task: AzureWebApp@1
  displayName: 'Deploy to Azure App Service'
  inputs:
    azureSubscription: '$(azureSubscription)'
    appType: 'webAppLinux'
    appName: '$(appName)'
    package: '$(Build.ArtifactStagingDirectory)/publish/*.zip'
```

---

# Create Azure Pipeline

In Azure DevOps:

```text
Pipelines
→ New pipeline
→ GitHub
→ nolet7/dotnet-projects
→ Existing Azure Pipelines YAML file
→ /azure-pipelines.yml
→ Run
```

Pipeline name:

```text
dotnet-webui-ci-cd
```

---

# Validate After Pipeline

Open:

```text
https://app-dotnet-webui-dev-nolet7-001.azurewebsites.net
```

Health endpoint:

```text
https://app-dotnet-webui-dev-nolet7-001.azurewebsites.net/api/health
```

PowerShell test:

```powershell
Invoke-RestMethod "https://app-dotnet-webui-dev-nolet7-001.azurewebsites.net/api/health"
```

---

# Troubleshooting

## Problem: Free tier quota error

Error:

```text
Current Limit (Free VMs): 0
```

Fix options:

```text
Option 1: Use B1 instead of F1
Option 2: Request quota increase for Free VMs
```

## Problem: The plan does not exist

Error:

```text
The plan 'asp-dotnet-webui-dev-eastus-001' doesn't exist.
```

Cause:

```text
The App Service Plan creation failed earlier.
```

Fix:

```powershell
az appservice plan create `
  --name $APP_SERVICE_PLAN `
  --resource-group $RG_NAME `
  --location $LOCATION `
  --sku B1 `
  --is-linux
```

## Problem: Web app name already exists

Fix:

```powershell
$WEB_APP_NAME="app-dotnet-webui-dev-nolet7-20260428"
```

Then rerun:

```powershell
az webapp create `
  --name $WEB_APP_NAME `
  --resource-group $RG_NAME `
  --plan $APP_SERVICE_PLAN `
  --runtime $RUNTIME
```

## Problem: App opens but shows default Azure page

Cause:

```text
Azure Web App exists, but your app has not been deployed yet.
```

Fix:

```powershell
dotnet publish .\DotNetAzureWebUi.Web\DotNetAzureWebUi.Web.csproj `
  --configuration Release `
  --output .\publish

Compress-Archive `
  -Path .\publish\* `
  -DestinationPath .\dotnet-webui.zip `
  -Force

az webapp deploy `
  --resource-group $RG_NAME `
  --name $WEB_APP_NAME `
  --src-path .\dotnet-webui.zip `
  --type zip
```

---

# Cleanup to Avoid Charges

If you used B1 and are done testing, delete the resource group:

```powershell
az group delete `
  --name rg-dotnet-webui-dev-eastus-001 `
  --yes `
  --no-wait
```

This deletes:

```text
Resource group
App Service Plan
Web App
Deployment content
```
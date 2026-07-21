# Deploy StreamNova to Azure App Service

StreamNova is prepared for Azure App Service on Linux with the .NET 10 runtime and continuous deployment from GitHub Actions.

## Recommended Azure configuration

| Setting | Value |
|---|---|
| Publish | Code |
| Runtime stack | .NET 10 (LTS) |
| Operating system | Linux |
| Region | Southeast Asia, or the closest available region |
| App Service plan | Basic B1 for Always On, or Free F1 for testing |
| Startup command | `dotnet StreamNova.dll` |
| Health-check path | `/health` |

The App Service name must be globally unique. A suggested name is `streamnova-azizulhakim00`; add a short suffix if Azure reports that it is already taken.

## Option A: Create and deploy using Azure Cloud Shell

1. Open Azure Portal and start **Cloud Shell** in Bash mode.
2. Clone this repository:

```bash
git clone https://github.com/AzizulHakim00/StreamNova-ASP.NET-Core.git
cd StreamNova-ASP.NET-Core
```

3. Run the included provisioning script:

```bash
chmod +x scripts/provision-azure-app-service.sh
./scripts/provision-azure-app-service.sh streamnova-azizulhakim00 streamnova-rg southeastasia B1
```

The script creates the Linux App Service, deploys StreamNova, enables persistent `/home` storage, configures production settings, and downloads the publish profile.

4. Copy the complete contents of the generated `.publishprofile` file.
5. In the GitHub repository, open **Settings → Secrets and variables → Actions**.
6. Create this repository secret:

```text
AZURE_WEBAPP_PUBLISH_PROFILE
```

7. Create this repository variable:

```text
AZURE_WEBAPP_NAME=streamnova-azizulhakim00
```

8. Open **Actions → Deploy StreamNova to Azure App Service → Run workflow**.

After this setup, every new push to `main` builds and deploys StreamNova automatically.

## Option B: Create the resource through Azure Portal

1. In Azure Portal, select **Create a resource → Web App**.
2. Select your active Azure subscription.
3. Create or select resource group `streamnova-rg`.
4. Enter a globally unique App Service name.
5. Choose **Code**, **.NET 10 (LTS)**, and **Linux**.
6. Select a nearby region and an App Service plan.
7. Create the Web App.
8. Open **Settings → Environment variables** and add:

| Name | Value |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `STREAMNOVA_DATA_PATH` | `/home/data/streamnova.json` |
| `WEBSITES_ENABLE_APP_SERVICE_STORAGE` | `true` |
| `WEBSITE_WARMUP_PATH` | `/health` |
| `WEBSITE_HTTPLOGGING_RETENTION_DAYS` | `7` |

9. Open **Configuration → General settings** and use this startup command:

```text
dotnet StreamNova.dll
```

10. Open **Monitoring → Health check** and set the path to `/health`.
11. On the App Service overview page, download the publish profile.
12. Add the GitHub secret and variable described in Option A, then run the deployment workflow.

## Continuous deployment workflow

The workflow is located at:

```text
.github/workflows/azure-app-service.yml
```

It performs these operations:

1. Restores the .NET project.
2. Builds StreamNova in Release mode.
3. Publishes the Azure deployment package.
4. Deploys the package using `azure/webapps-deploy@v3`.

If the Azure repository variable is not configured yet, the workflow still validates the build and skips only the deployment step.

## Persistent application data

StreamNova currently stores users, watchlists, reviews, subscriptions, notifications, support tickets, and viewing activity in one JSON file. Azure should use:

```text
/home/data/streamnova.json
```

Keep `WEBSITES_ENABLE_APP_SERVICE_STORAGE=true` so the file survives application restarts. For a future enterprise version, migrate this data layer to Azure SQL Database or Cosmos DB.

## Live endpoints

After deployment:

```text
https://<your-app-name>.azurewebsites.net
https://<your-app-name>.azurewebsites.net/health
```

## Troubleshooting

### The GitHub deployment step is skipped

Create the repository variable `AZURE_WEBAPP_NAME` and rerun the workflow.

### Deployment authentication fails

Download a new publish profile from Azure and replace the `AZURE_WEBAPP_PUBLISH_PROFILE` repository secret.

### The app starts but data disappears

Confirm that `STREAMNOVA_DATA_PATH` is `/home/data/streamnova.json` and `WEBSITES_ENABLE_APP_SERVICE_STORAGE` is `true`.

### The app does not start

Confirm that the App Service runtime is .NET 10 and that the startup command is `dotnet StreamNova.dll`. Review **Monitoring → Log stream** for the startup error.

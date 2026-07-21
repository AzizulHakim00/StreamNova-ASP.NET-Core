# Azure App Service checklist

- [ ] Azure subscription is active.
- [ ] Linux Web App uses .NET 10 (LTS).
- [ ] App Service name is globally unique.
- [ ] `STREAMNOVA_DATA_PATH` is `/home/data/streamnova.json`.
- [ ] `WEBSITES_ENABLE_APP_SERVICE_STORAGE` is `true`.
- [ ] Startup command is `dotnet StreamNova.dll`.
- [ ] Health check path is `/health`.
- [ ] GitHub variable `AZURE_WEBAPP_NAME` is configured.
- [ ] GitHub secret `AZURE_WEBAPP_PUBLISH_PROFILE` is configured.
- [ ] The Azure deployment workflow has completed successfully.
- [ ] The public site and `/health` endpoint both respond successfully.

#!/usr/bin/env bash
set -euo pipefail

APP_NAME="${1:-streamnova-azizulhakim00}"
RESOURCE_GROUP="${2:-streamnova-rg}"
LOCATION="${3:-southeastasia}"
SKU="${4:-B1}"

if ! command -v az >/dev/null 2>&1; then
  echo "Azure CLI is required. Run this script in Azure Cloud Shell or install Azure CLI first." >&2
  exit 1
fi

az account show >/dev/null

echo "Creating and deploying $APP_NAME in $LOCATION..."
az webapp up \
  --name "$APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --sku "$SKU" \
  --os-type linux \
  --runtime "DOTNETCORE:10.0"

echo "Configuring production settings and persistent storage..."
az webapp config appsettings set \
  --name "$APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    STREAMNOVA_DATA_PATH=/home/data/streamnova.json \
    WEBSITES_ENABLE_APP_SERVICE_STORAGE=true \
    WEBSITE_WARMUP_PATH=/health \
    WEBSITE_HTTPLOGGING_RETENTION_DAYS=7

az webapp config set \
  --name "$APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --always-on true \
  --startup-file "dotnet StreamNova.dll"

PUBLISH_PROFILE_FILE="${APP_NAME}.publishprofile"
az webapp deployment list-publishing-profiles \
  --name "$APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --xml > "$PUBLISH_PROFILE_FILE"

HOSTNAME=$(az webapp show \
  --name "$APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --query defaultHostName \
  --output tsv)

echo
echo "Azure App Service created successfully."
echo "Live URL: https://$HOSTNAME"
echo "Publish profile saved to: $PUBLISH_PROFILE_FILE"
echo "Add its complete contents to GitHub secret AZURE_WEBAPP_PUBLISH_PROFILE."
echo "Add GitHub repository variable AZURE_WEBAPP_NAME with value: $APP_NAME"

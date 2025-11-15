# Script para obtener credenciales de Azure
# Ejecuta este script en Azure Cloud Shell o en tu terminal con Azure CLI instalado

# Obtener el App Service
$appName = "SistemaClinica-webapp"
$resourceGroup = "INT_HUM_COMP"

# Obtener Subscription ID
$subscriptionId = (az account show --query id -o tsv)
Write-Host "AZURE_SUBSCRIPTION_ID: $subscriptionId" -ForegroundColor Green

# Obtener Tenant ID
$tenantId = (az account show --query tenantId -o tsv)
Write-Host "AZURE_TENANT_ID: $tenantId" -ForegroundColor Green

# Crear Service Principal y obtener Client ID
Write-Host "`nCreando Service Principal..." -ForegroundColor Yellow
$sp = az ad sp create-for-rbac --name "github-actions-$appName" --role contributor --scopes /subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.Web/sites/$appName --sdk-auth | ConvertFrom-Json

Write-Host "AZURE_CLIENT_ID: $($sp.clientId)" -ForegroundColor Green

Write-Host "`n=== Resumen de Secretos para GitHub ===" -ForegroundColor Cyan
Write-Host "Agrega estos secretos en: https://github.com/MarcoVR2309/SistemaMedico/settings/secrets/actions" -ForegroundColor Yellow
Write-Host "`nAZURE_CLIENT_ID: $($sp.clientId)"
Write-Host "AZURE_TENANT_ID: $tenantId"
Write-Host "AZURE_SUBSCRIPTION_ID: $subscriptionId"

@description('The name of the App Service Plan.')
param appServicePlanName string = 'api-website-plan'

@description('The location of the App Service Plan.')
param location string = resourceGroup().location

@allowed([
  'F1'
  'D1'
  'B1'
  'B2'
  'B3'
  'S1'
  'S2'
  'S3'
  'P1'
  'P2'
  'P3'
])
@description('The pricing tier of the App Service Plan.')
param appServicePlanSku string = 'B1'

@description('The settings of the App Service.')
param appSettings array = []

@secure()
@description('JWT Key for the App Service.')
param jwtKey string

@description('The admin login name of the SQL Server.')
param sqlServerAdministratorLogin string

@secure()
@description('The admin login password of the SQL Server.')
param sqlServerAdministratorLoginPassword string

@description('A unique suffix to add to resource names that need to be globally unique.')
@maxLength(13)
param resourceNameSuffix string = uniqueString(resourceGroup().id)

var sqlDatabaseName = 'TodoDb'

module appServicePlan 'infrastructure/module/appServicePlan.bicep' = {
  name: 'appServicePlanModule'
  params: {
    appServicePlanName: appServicePlanName
    location: location
    sku: appServicePlanSku
  }
}
var appServiceAppName = 'api-website-${resourceNameSuffix}'
var sqlDatabaseConnectionString = 'Server=tcp:${sqlServer.outputs.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabaseName};Persist Security Info=False;User ID=${sqlServerAdministratorLogin};Password=${sqlServerAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

module appService 'infrastructure/module/appService.bicep' = {
  name: 'appServiceModule'
  params: {
    appServiceName: appServiceAppName
    location: location
    appServicePlanName: appServicePlan.outputs.appServicePlanName
    appSettings: appSettings
    jwtKey: jwtKey
    sqlDatabaseConnectionString: sqlDatabaseConnectionString
  }
  dependsOn: [
    appServicePlan
  ]
}

module sqlServer 'infrastructure/module/sqlServer.bicep' = {
  name: 'sqlServerModule'
  params: {
    location: location
    sqlServerAdministratorLogin: sqlServerAdministratorLogin
    sqlServerAdministratorLoginPassword: sqlServerAdministratorLoginPassword
  }
}

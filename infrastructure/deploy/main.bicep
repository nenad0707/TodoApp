@description('The name of the App Service Plan.')
param appServicePlanName string = 'api-website-plan-${environmentType}'

@description('The location of the App Service Plan.')
param location string = resourceGroup().location

@description('Select the type of environment you want to provision. Allowed values are Production and Test.')
@allowed([
  'Production'
  'Test'
])
param environmentType string

@description('The settings of the App Service.')
param appSettings array = []

@secure()
@description('JWT Key for the App Service.')
param jwtKey string

@description('The existing SQL Server name.')
param existingSqlServerName string

@description('The existing SQL Database name.')
param existingDatabaseName string

@description('A unique suffix to add to resource names that need to be globally unique.')
@maxLength(13)
param resourceNameSuffix string = uniqueString(resourceGroup().id)

@description('The administrator login username for the SQL server.')
param sqlServerAdministratorLogin string

@secure()
@description('The administrator login password for the SQL server.')
param sqlServerAdministratorLoginPassword string

module sqlServer '../module/sqlServer.bicep' = {
  name: 'sqlServerModule'
  params: {
    existingSqlServerName: existingSqlServerName
    existingDatabaseName: existingDatabaseName
  }
}

var sqlDatabaseConnectionString = 'Server=tcp:${sqlServer.outputs.sqlServerFullyQualifiedDomainName},1433;Initial Catalog=${existingDatabaseName};Persist Security Info=False;User ID=${sqlServerAdministratorLogin};Password=${sqlServerAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;ConnectRetryCount=5;ConnectRetryInterval=10;Max Pool Size=100;Min Pool Size=0;'

module appServicePlan '../module/appServicePlan.bicep' = {
  name: 'appServicePlanModule'
  params: {
    appServicePlanName: appServicePlanName
    location: location
    sku: 'F1' // Free tier
  }
}

module appService '../module/appService.bicep' = {
  name: 'appServiceModule'
  params: {
    appServiceName: 'api-website-${resourceNameSuffix}'
    location: location
    appServicePlanName: appServicePlan.outputs.appServicePlanName
    appSettings: appSettings
    jwtKey: jwtKey
    sqlDatabaseConnectionString: sqlDatabaseConnectionString
  }
  dependsOn: [
    appServicePlan
    sqlServer
  ]
}

output appServiceAppName string = appService.outputs.appServiceAppName
output appServiceAppHostName string = appService.outputs.appServiceAppHostName
output sqlDatabaseName string = sqlServer.outputs.sqlDatabaseName
output sqlServerFullyQualifiedDomainName string = sqlServer.outputs.sqlServerFullyQualifiedDomainName

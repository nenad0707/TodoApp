@description('Location for the resources')
param location string = resourceGroup().location

@description('The name of the key')
@secure()
param jwtKey string

@description('App settings for the web api app')
param appSettings array = []

@allowed([
  'S1'
  'B1'
])
param appServicePlanSku string = 'B1'

@description('The administrator login username for the SQL server.')
param sqlServerAdministratorLogin string

@secure()
@description('The administrator login password for the SQL server.')
param sqlServerAdministratorLoginPassword string

@description('A unique suffix to add to resource names that need to be globally unique.')
@maxLength(13)
param resourceNameSuffix string = uniqueString(resourceGroup().id)

var appServiceAppName = 'api-website-${resourceNameSuffix}'
var appServicePlanName = 'api-website'

var requiredSettings = [
  {
    name: 'Jwt:Key'
    value: jwtKey
  }
  {
    name: 'Jwt:Issuer'
    value: '${appServicePlanName}-${resourceNameSuffix}.azurewebsites.net'
  }
  {
    name: 'Jwt:Audience'
    value: 'TodoApi'
  }
  {
    name: 'ConnectionStrings:Default'
    value: sqlDatabaseConnectionString
  }
]

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: appServicePlanName
  location: location
  kind: 'linux'
  sku: {
    name: appServicePlanSku
  }
}

resource appServiceApp 'Microsoft.Web/sites@2022-03-01' = {
  name: appServiceAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      appSettings: union(appSettings, requiredSettings)
    }
  }
}

var sqlServerName = 'api-website-${resourceNameSuffix}'
var sqlDatabaseName = 'TodoDb'

var sqlDatabaseConnectionString = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabase.name};Persist Security Info=False;User ID=${sqlServerAdministratorLogin};Password=${sqlServerAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlServerAdministratorLogin
    administratorLoginPassword: sqlServerAdministratorLoginPassword
  }
}

resource sqlServerFirewallRule 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
}

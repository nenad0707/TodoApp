@description('The name of the App Service.')
param appServiceName string

@description('The location of the App Service.')
param location string

@description('The name of the App Service Plan.')
param appServicePlanName string

@description('The settings of the App Service.')
param appSettings array

@description('JWT Key for the App Service.')
param jwtKey string

@description('SQL Database Connection String for the App Service.')
param sqlDatabaseConnectionString string

@description('A unique suffix to add to resource names that need to be globally unique.')
@maxLength(13)
param resourceNameSuffix string = uniqueString(resourceGroup().id)

var requiredSettings = [
  {
    name: 'Jwt__Key'
    value: jwtKey
  }
  {
    name: 'Jwt__Issuer'
    value: '${appServicePlanName}-${resourceNameSuffix}.azurewebsites.net'
  }
  {
    name: 'Jwt__Audience'
    value: 'TodoApi'
  }
  {
    name: 'ConnectionStrings__Default'
    value: sqlDatabaseConnectionString
  }
]

resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' existing = {
  name: appServicePlanName
}

resource appService 'Microsoft.Web/sites@2021-02-01' = {
  name: appServiceName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      healthCheckPath: '/health'
      httpLoggingEnabled: true
      linuxFxVersion: 'DOTNETCORE|8.0' //linuxFxVersion
      appSettings: union(appSettings, requiredSettings)
    }
  }
}

resource appServiceLogs 'Microsoft.Web/sites/config@2022-09-01' = {
  parent: appService
  name: 'logs'
  properties: {
    applicationLogs: {
      fileSystem: {
        level: 'Information'
      }
    }
    httpLogs: {
      fileSystem: {
        retentionInMb: 35
        retentionInDays: 7
      }
    }
  }
}

output appServiceAppHostName string = appService.properties.defaultHostName
output appServiceAppName string = appService.name

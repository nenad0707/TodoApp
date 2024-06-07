@description('The name of the App Service.')
param appServiceName string

@description('The location of the App Service.')
param location string

@description('The name of the App Service Plan.')
param appServicePlanName string

@description('The settings of the App Service.')
param appSettings array

@secure()
@description('JWT Key for the App Service.')
param jwtKey string

@description('SQL Database Connection String for the App Service.')
param sqlDatabaseConnectionString string

var requiredSettings = [
  {
    name: 'Jwt__Key'
    value: jwtKey
  }
  {
    name: 'Jwt__Issuer'
    value: '${appServiceName}.azurewebsites.net'
  }
  {
    name: 'Jwt__Audience'
    value: 'TodoApi'
  }
  {
    name: 'ConnectionStrings__Default'
    value: sqlDatabaseConnectionString
  }
  {
    name: 'ApiUrl'
    value: 'https://${appServiceName}.azurewebsites.net'
  }
]

resource appService 'Microsoft.Web/sites@2021-02-01' = {
  name: appServiceName
  location: location
  properties: {
    serverFarmId: appServicePlanName
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      appSettings: union(appSettings, requiredSettings)
    }
  }
}

output appServiceAppName string = appService.name
output appServiceAppHostName string = appService.properties.defaultHostName

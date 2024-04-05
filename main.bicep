@description('The name of the App Service Plan.')
param appServicePlanName string = 'api-website-plan'

@description('The location of the App Service Plan.')
param location string = resourceGroup().location

@description('Select the type of environment you want to provision. Allowed values are Production and Test.')
@allowed([
  'Production'
  'Test'
])
param environmentType string

// @allowed([
//   'F1'
//   'D1'
//   'B1'
//   'B2'
//   'B3'
//   'S1'
//   'S2'
//   'S3'
//   'P1'
//   'P2'
//   'P3'
// ])
// @description('The pricing tier of the App Service Plan.')
// param appServicePlanSku string = 'B1'
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

var environmentConfigurationMap = {
  Production: {
    appServicePlan: {
      sku: 'B1'
    }
    sqlDatabase: {
      sku: {
        name: 'Basic'
        tier: 'Basic'
      }
    }
  }
  Test: {
    appServicePlan: {
      sku: 'B1'
    }
    sqlDatabase: {
      sku: {
        name: 'Basic'
        tier: 'Basic'
      }
    }
  }
}

module appServicePlan 'infrastructure/module/appServicePlan.bicep' = {
  name: 'appServicePlanModule'
  params: {
    appServicePlanName: appServicePlanName
    location: location
    sku: environmentConfigurationMap[environmentType].appServicePlan.sku
  }
}
var appServiceAppName = 'api-website-${resourceNameSuffix}'
var sqlDatabaseConnectionString = 'Server=tcp:${sqlServer.outputs.sqlServerFullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabaseName};Persist Security Info=False;User ID=${sqlServerAdministratorLogin};Password=${sqlServerAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

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
    sqlServer
  ]
}

module sqlServer 'infrastructure/module/sqlServer.bicep' = {
  name: 'sqlServerModule'
  params: {
    sku: environmentConfigurationMap[environmentType].sqlDatabase.sku
    location: location
    sqlServerAdministratorLogin: sqlServerAdministratorLogin
    sqlServerAdministratorLoginPassword: sqlServerAdministratorLoginPassword
  }
}

output appServiceAppName string = appService.outputs.appServiceAppName
output appServiceAppHostName string = appService.outputs.appServiceAppHostName
output sqlServerFullyQualifiedDomainName string = sqlServer.outputs.sqlServerFullyQualifiedDomainName
output sqlDatabaseName string = sqlServer.outputs.sqlDatabaseName

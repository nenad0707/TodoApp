@description('The location of the App Service Plan.')
param location string

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
param sku string = 'B1'

@description('The name of the App Service Plan.')
param appServicePlanName string = 'api-website'

resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: appServicePlanName
  location: location
  // kind: 'linux'
  properties: {
    reserved: true
  }
  sku: {
    name: sku
  }
}

output appServicePlanName string = appServicePlan.name

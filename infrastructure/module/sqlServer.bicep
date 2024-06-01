// @description('The existing SQL Server name.')
param existingSqlServerName string

@description('The existing SQL Database name.')
param existingDatabaseName string

// @description('The administrator login username for the SQL server.')
// param sqlServerAdministratorLogin string

// @secure()
// @description('The administrator login password for the SQL server.')
// param sqlServerAdministratorLoginPassword string

resource existingSqlServer 'Microsoft.Sql/servers@2021-11-01' existing = {
  name: existingSqlServerName
}

resource existingSqlDatabase 'Microsoft.Sql/servers/databases@2021-11-01' existing = {
  parent: existingSqlServer
  name: existingDatabaseName
}

resource sqlServerFirewallRule 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  parent: existingSqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

output sqlServerFullyQualifiedDomainName string = existingSqlServer.properties.fullyQualifiedDomainName
output sqlDatabaseName string = existingSqlDatabase.name

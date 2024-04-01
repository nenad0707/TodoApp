@description('The location of the SQL Database.')
param location string

@description('The name of the SQL Database.')
param sqlDatabaseName string = 'TodoDb'

@description('The name of the SQL Server.')
param sqlServerName string

resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-02-01-preview' = {
  name: '${sqlServerName}/${sqlDatabaseName}'
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 1073741824
    zoneRedundant: false
    readScale: 'Disabled'
  }
}

output sqlDatabaseName string = sqlDatabase.name
// ovo treba obrisati

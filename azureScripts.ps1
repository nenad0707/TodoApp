# This script sets up Azure resources

# Connect to Azure account
Connect-AzAccount

# Get the Azure subscription context
$context = Get-AzSubscription -SubscriptionName PAYG-Sandboxes

# Set the Azure context to the specified subscription
Set-AzContext $context

# Set the default resource group
Set-AzDefault -ResourceGroupName rg_sb_eastus_89803_1_171259668489
##change resourse group name

# Set the GitHub organization and repository names
$githubOrganizationName = 'nenad0707'
$githubRepositoryName = 'TodoApp'

# Create a new Azure AD application
$testApplicationRegistration = New-AzADApplication -DisplayName 'TodoApp-Test'

# Create a new Azure AD application federated credential
New-AzADAppFederatedCredential `
  -Name 'TodoApp-Test' `
  -ApplicationObjectId $testApplicationRegistration.Id `
  -Issuer 'https://token.actions.githubusercontent.com' `
  -Audience 'api://AzureADTokenExchange' `
  -Subject "repo:$($githubOrganizationName)/$($githubRepositoryName):environment:Test"

# Create a new Azure AD application federated credential for the TodoApp-test-branch
New-AzADAppFederatedCredential `
  -Name 'TodoApp-test-branch' `
  -ApplicationObjectId $testApplicationRegistration.Id `
  -Issuer 'https://token.actions.githubusercontent.com' `
  -Audience 'api://AzureADTokenExchange' `
  -Subject "repo:$($githubOrganizationName)/$($githubRepositoryName):ref:refs/heads/main"

# Get the resource group
$productionApplicationRegistration = New-AzADApplication -DisplayName 'TodoApp-Production'

New-AzADAppFederatedCredential `
  -Name 'TodoApp-Production' `
  -ApplicationObjectId $productionApplicationRegistration.Id `
  -Issuer 'https://token.actions.githubusercontent.com' `
  -Audience 'api://AzureADTokenExchange' `
  -Subject "repo:$($githubOrganizationName)/$($githubRepositoryName):environment:Production"

New-AzADAppFederatedCredential `
  -Name 'TodoApp-production-branch' `
  -ApplicationObjectId $productionApplicationRegistration.Id `
  -Issuer 'https://token.actions.githubusercontent.com' `
  -Audience 'api://AzureADTokenExchange' `
  -Subject "repo:$($githubOrganizationName)/$($githubRepositoryName):ref:refs/heads/main"   

$testResourceGroup = Get-AzResourceGroup -Name rg_sb_southeastasia_89803_3_171259668780  ##change resourse group name

New-AzADServicePrincipal -AppId $($testApplicationRegistration.AppId)
New-AzRoleAssignment `
  -ApplicationId $($testApplicationRegistration.AppId) `
  -RoleDefinitionName Contributor `
  -Scope $($testResourceGroup.ResourceId)

$productionResourceGroup = Get-AzResourceGroup -Name rg_sb_eastus_89803_1_171259668489 ##change resourse group name

New-AzADServicePrincipal -AppId $($productionApplicationRegistration.AppId)
New-AzRoleAssignment `
  -ApplicationId $($productionApplicationRegistration.AppId) `
  -RoleDefinitionName Contributor `
  -Scope $($productionResourceGroup.ResourceId)

$azureContext = Get-AzContext
Write-Host "AZURE_CLIENT_ID_TEST: $($testApplicationRegistration.AppId)"
Write-Host "AZURE_CLIENT_ID_PRODUCTION: $($productionApplicationRegistration.AppId)"
Write-Host "AZURE_TENANT_ID: $($azureContext.Tenant.Id)"
Write-Host "AZURE_SUBSCRIPTION_ID: $($azureContext.Subscription.Id)"   ## write these secrets to github secrets
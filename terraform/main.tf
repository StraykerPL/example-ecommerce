provider "azurerm" {
  features {}
}

terraform {
	backend "azurerm" {
		resource_group_name = "StorageAccount-ResourceGroup"
		storage_account_name = "ookok"
		container_name = "tfstate"
		key = "prod.terraform.tfstate"
		access_key = "Us9siOU7G7GnEsJNPBq8uTWxumEC9pvuJTBZIEbHPj6NJhjEWtW4JBoVlwsI3+zjYAcsSdgvNYbL+AStkVN0Nw=="
	}
  required_providers {
    azuread = {
      source = "hashicorp/azuread"
      version = "~> 2.15.0"
    }
  }
}

provider "azuread" {
  tenant_id = "3e761eec-2ba6-486d-9eca-252c1cd38db6"
}

resource "azurerm_resource_group" "example" {
  name     = "MyResourceGroup"
  location = "East US"
}

resource "azurerm_app_service_plan" "example" {
  name                = "myAppServicePlan574574568568"
  location            = azurerm_resource_group.example.location
  resource_group_name = azurerm_resource_group.example.name
  sku {
    tier = "Free"
    size = "F1"
  }
}

resource "azurerm_app_service" "example" {
  name                = "abctak"
  location            = azurerm_resource_group.example.location
  resource_group_name = azurerm_resource_group.example.name
  app_service_plan_id = azurerm_app_service_plan.example.id
  
  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.example.instrumentation_key,
    "ConnectionStrings:DefaultConnection" = "Server=tcp:${azurerm_sql_server.example.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_sql_database.example.name};Persist Security Info=False;User ID=${azurerm_sql_server.example.administrator_login};Password=${azurerm_sql_server.example.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
    "ConnectionStrings:blobConnect" = "DefaultEndpointsProtocol=https;AccountName=${azurerm_storage_account.example.name};AccountKey=aDa3GEXoAwZHj1JeiizCZjvYjAIrOxqOdAKGcYGfGEiZJjCwtw3qlaNREh9HqiDOZqu4ddMX7i8C+AStk7S4xw==;EndpointSuffix=core.windows.net"
  }

  connection_string {
    name  = "DBConnectionString"
    type  = "SQLAzure"
    value = "Server=tcp:${azurerm_sql_server.example.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_sql_database.example.name};Persist Security Info=False;User ID=${azurerm_sql_server.example.administrator_login};Password=${azurerm_sql_server.example.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}

resource "azurerm_application_insights" "example" {
  name                = "myAppInsights"
  location            = azurerm_resource_group.example.location
  resource_group_name = azurerm_resource_group.example.name
  application_type    = "web"
}

resource "azurerm_sql_server" "example" {
  name                         = "my-sql-server3487568934760892"
  location                     = azurerm_resource_group.example.location
  resource_group_name          = azurerm_resource_group.example.name
  version                      = "12.0"
  administrator_login          = "myAdmin"
  administrator_login_password = "myPassword1234!"
}

resource "azurerm_sql_database" "example" {
  name                = "example-database"
  resource_group_name = azurerm_resource_group.example.name
  server_name         = azurerm_sql_server.example.name
  location            = azurerm_resource_group.example.location
  edition             = "Basic"
}

resource "azurerm_storage_account" "example" {
  name                     = "abctakstorageaccount"
  resource_group_name      = azurerm_resource_group.example.name
  location = azurerm_resource_group.example.location
  account_tier = "Standard"
  account_replication_type = "LRS"
  account_kind = "StorageV2"
}

resource "azurerm_storage_container" "example" {
  name = "abctak-container"
  storage_account_name = azurerm_storage_account.example.name
  container_access_type = "private"
}

resource "azuread_application" "example" {
  display_name = "myWebApp"

  web {
    homepage_url = "https://abctak.azurewebsites.net"
  }
}

resource "azuread_service_principal" "example" {
  application_id = azuread_application.example.application_id
}
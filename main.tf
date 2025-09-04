terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.39.0"
    }
  }

  backend "azurerm" {
    resource_group_name  = "StorageRG"
    storage_account_name = "teamworkflowstorage"
    container_name       = "teamworkflowcontainer"
    key                  = "terraform.tfstate"
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}

resource "azurerm_resource_group" "alex-teamworkflow-rg" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_service_plan" "alex-teamworkflow-service" {
  name                = var.service-plan-name
  resource_group_name = azurerm_resource_group.alex-teamworkflow-rg.name
  location            = azurerm_resource_group.alex-teamworkflow-rg.location
  sku_name            = var.sku_name
  os_type             = var.os_type
}

resource "azurerm_linux_web_app" "alex-teamworkflow-app" {
  name                = var.app-name
  resource_group_name = azurerm_resource_group.alex-teamworkflow-rg.name
  location            = azurerm_resource_group.alex-teamworkflow-rg.location
  service_plan_id     = azurerm_service_plan.alex-teamworkflow-service.id
  https_only = true

  site_config {
    application_stack {
      dotnet_version = var.dotnet_version
    }
    always_on = false
  }

  connection_string {
    name  = var.connection_string_name
    type  = var.connection_string_type
    value = "Data Source=tcp:${azurerm_mssql_server.teamworkflow-mssql-server.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.teamworkflow-mssql-database.name};User ID=${azurerm_mssql_server.teamworkflow-mssql-server.administrator_login};Password=${azurerm_mssql_server.teamworkflow-mssql-server.administrator_login_password};Trusted_Connection=False; MultipleActiveResultSets=True;"
  }

  app_settings = {
    ASPNETCORE_ENVIRONMENT = var.ASPNETCORE_ENVIRONMENT
  }
}

resource "azurerm_app_service_source_control" "alex-teamworkflow-source" {
  app_id                 = azurerm_linux_web_app.alex-teamworkflow-app.id
  repo_url               = var.repo_url
  branch                 = "main"
  use_manual_integration = true
}

resource "azurerm_mssql_server" "teamworkflow-mssql-server" {
  name                         = var.mssqlserver_name
  resource_group_name          = azurerm_resource_group.alex-teamworkflow-rg.name
  location                     = azurerm_resource_group.alex-teamworkflow-rg.location
  version                      = var.mssql_server_version
  administrator_login          = var.mssqlserver_administrator_login
  administrator_login_password = var.mssqlserver_administrator_login_password
}

resource "azurerm_mssql_database" "teamworkflow-mssql-database" {
  name           = var.mssqldatabase_name
  server_id      = azurerm_mssql_server.teamworkflow-mssql-server.id
  collation      = var.collation
  max_size_gb    = 2
  sku_name       = var.database_sku_name
  zone_redundant = false
  license_type   = var.license_type
}

resource "azurerm_mssql_firewall_rule" "alex-teamworkflow-mssql-firewall" {
  name             = var.firewall_rule_name
  server_id        = azurerm_mssql_server.teamworkflow-mssql-server.id
  start_ip_address = var.start_ip_address
  end_ip_address   = var.end_ip_address
}

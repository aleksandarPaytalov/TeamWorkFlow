output "resource_group_name" {
  description = "Name of the created resource group"
  value       = azurerm_resource_group.alex-teamworkflow-rg.name
}

output "web_app_url" {
  description = "URL of the deployed web app"
  value       = "https://${azurerm_linux_web_app.alex-teamworkflow-app.name}.azurewebsites.net"
}

output "web_app_name" {
  description = "Name of the web app"
  value       = azurerm_linux_web_app.alex-teamworkflow-app.name
}

output "sql_server_fqdn" {
  description = "Fully qualified domain name of the SQL server"
  value       = azurerm_mssql_server.teamworkflow-mssql-server.fully_qualified_domain_name
  sensitive   = true
}

output "database_name" {
  description = "Name of the created database"
  value       = azurerm_mssql_database.teamworkflow-mssql-database.name
}

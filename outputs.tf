output "resource_group_name" {
  description = "Name of the created resource group"
  value       = azurerm_resource_group.alex-seminar-rg.name
}

output "web_app_url" {
  description = "URL of the deployed web app"
  value       = "https://${azurerm_linux_web_app.alex-seminar-app.name}.azurewebsites.net"
}

output "web_app_name" {
  description = "Name of the web app"
  value       = azurerm_linux_web_app.alex-seminar-app.name
}

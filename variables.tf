variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "location" {
  description = "Location of the resource group"
  type        = string
}

variable "subscription_id" {
  description = "value of the subscription id"
  type        = string
}

variable "service-plan-name" {
  description = "Name of the teamworkflow service plan"
  type        = string
}

variable "app-name" {
  description = "Name of the teamworkflow app"
  type        = string
}

variable "repo_url" {
  description = "value of the repo url"
  type        = string
}

variable "mssqlserver_name" {
  description = "value of the mssqlserver name"
  type        = string
}

variable "mssqlserver_administrator_login" {
  description = "value of the mssqlserver administrator login"
  type        = string
}

variable "mssqlserver_administrator_login_password" {
  description = "value of the mssqlserver administrator login password"
  type        = string
}

variable "mssqldatabase_name" {
  description = "value of the mssqldatabase name"
  type        = string
}

variable "firewall_rule_name" {
  description = "value of the firewall rule name"
  type        = string
}

variable "sku_name" {
  description = "value of the sku name"
  type        = string
}

variable "os_type" {
  description = "value of the os type"
  type        = string
}

variable "dotnet_version" {
  description = "value of the dotnet version"
  type        = string
}

variable "connection_string_name" {
  description = "value of the connection string name"
  type        = string
}

variable "connection_string_type" {
  description = "value of the connection string type"
  type        = string
}

variable "ASPNETCORE_ENVIRONMENT" {
  description = "value of the ASPNETCORE_ENVIRONMENT"
  type        = string
}

variable "start_ip_address" {
  description = "value of the start ip address"
  type        = string
}

variable "end_ip_address" {
  description = "value of the end ip address"
  type        = string
}

variable "license_type" {
  description = "value of the license type"
  type        = string
}

variable "database_sku_name" {
  description = "value of the database sku name"
  type        = string
}

variable "collation" {
  description = "value of the collation"
  type        = string
}

variable "mssql_server_version" {
  description = "value of the mssql server version"
  type        = string
}

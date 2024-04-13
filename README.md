# Todo API Project 📝

[![deploy-api-website-with-database](https://github.com/nenad0707/TodoApp/actions/workflows/workflow.yml/badge.svg)](https://github.com/nenad0707/TodoApp/actions/workflows/workflow.yml)

This project is a Todo API that allows users to manage their todo tasks.

## About the API 📡

The API provides endpoints for users to create, read, update, and delete todo tasks. It uses JWT for authentication and authorization.

## Technologies Used 🔧

The project is written in C# and uses the following technologies:

- .NET Core for the API
- Dapper for data access
- SQL Server Data Tools (SSDT) for database management
- JWT for authentication
- Azure Bicep for infrastructure as code
- GitHub Actions for CI/CD

## User Flow 👥

To use the API, users need to:

1. Register a new account using the `/register` endpoint.
2. Log in with their new account using the `/login` endpoint. This will return a token.
3. Use the token to authenticate their requests to the other endpoints.

## Azure Bicep and Modules ☁️

The project uses Azure Bicep for infrastructure as code. The Bicep files define the resources needed for the project, such as the App Service and SQL Server.

![Azure Infrastructure](./docs/azure-infrastructure.png)

## ⚠️ Note

The Azure resources used in this project are temporary as they are part of an Azure Cloud Sandbox which was purchased temporarily. Please ensure to replace these resources with your own before deploying the project.

## Azure Scripts 📜

The project includes Azure scripts for deploying the infrastructure and the application. The scripts use the Azure CLI and PowerShell.

## GitHub Actions CI/CD Pipeline 🐙

The project uses GitHub Actions for continuous integration and continuous deployment. The workflows include building the application, running tests, and deploying to Azure.

## Conclusion 🏁

This project demonstrates a complete workflow for a Todo API, from development to deployment. It shows how to use .NET Core, Dapper, SSDT, JWT, Azure Bicep, Azure scripts, and GitHub Actions in a real-world scenario.

## 📝 License

This project is [MIT](https://opensource.org/licenses/MIT) licensed.

# 🏭 TeamWorkFlow

**A comprehensive manufacturing workflow management system built with ASP.NET Core 6.0**

[![.NET](https://img.shields.io/badge/.NET-6.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/6.0)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-6.0-green.svg)](https://docs.microsoft.com/en-us/ef/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red.svg)](https://www.microsoft.com/en-us/sql-server)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Contributing](#contributing)
- [Roadmap](#roadmap)
- [License](#license)

## 🎯 Overview

TeamWorkFlow is a modern, web-based manufacturing workflow management system designed to streamline operations in manufacturing environments. The application provides comprehensive tools for managing tasks, operators, machines (CMMs), projects, and parts inventory with an intuitive, responsive interface.

### Key Benefits

- **Centralized Management**: Single platform for all manufacturing workflow operations
- **Real-time Tracking**: Live updates on task progress, machine status, and operator availability
- **Modern UI/UX**: Responsive design with clean, professional interface
- **Role-based Access**: Secure authentication with Administrator and Operator roles
- **Scalable Architecture**: Clean architecture pattern with separation of concerns
- **Data Integrity**: Comprehensive validation and error handling

## ✨ Features

### 🔧 Task Management
- **Task Creation & Assignment**: Create detailed tasks with priorities, deadlines, and operator assignments
- **Progress Tracking**: Real-time status updates with visual progress indicators
- **Project Integration**: Link tasks to specific projects for better organization
- **Machine Assignment**: Associate tasks with specific CMM machines
- **Date Management**: Start dates, end dates, and deadline tracking with dd/MM/yyyy format
- **Priority System**: High, medium, and low priority classification
- **Status Workflow**: Open → In Progress → Finished → Canceled status flow

### 👥 Operator Management
- **Operator Profiles**: Comprehensive operator information with contact details
- **Capacity Planning**: Working capacity management (4-12 hours per day)
- **Availability Tracking**: Real-time operator availability status
- **Task Assignment**: View and manage operator workloads
- **Performance Metrics**: Track operator productivity and task completion

### 🏭 Machine (CMM) Management
- **Machine Registry**: Complete database of Coordinate Measuring Machines
- **Calibration Scheduling**: Track calibration dates and maintenance schedules
- **Capacity Management**: Monitor machine availability and workload
- **Status Monitoring**: Real-time machine status and availability
- **Image Documentation**: Visual documentation with image URL support

### 📊 Project Management
- **Project Tracking**: Comprehensive project lifecycle management
- **Client Information**: Client details and project specifications
- **Hour Tracking**: Total hours spent tracking (0-5000 hours)
- **Status Management**: Project status workflow management
- **Appliance Categorization**: Project categorization by appliance type

### 🔩 Parts Inventory
- **Parts Catalog**: Complete parts inventory management
- **Article Numbers**: Unique article and client number tracking
- **Tool Numbers**: Tool number management (1000-9999 range)
- **Status Tracking**: Parts status workflow
- **Visual Documentation**: Image URL support for parts documentation

### 🔐 Security & Authentication
- **ASP.NET Core Identity**: Secure user authentication and authorization
- **Role-based Access Control**: Administrator and Operator roles
- **Data Protection**: Secure data handling with validation
- **Session Management**: Secure session handling

### 🎨 User Interface
- **Modern Design**: Clean, professional interface with purple/violet theme
- **Responsive Layout**: Mobile-friendly design that works on all devices
- **Interactive Forms**: Enhanced form validation with real-time feedback
- **Data Tables**: Sortable, searchable data tables with pagination
- **Visual Feedback**: Loading states, success/error messages, and progress indicators

## 🛠 Technology Stack

### Backend
- **Framework**: ASP.NET Core 6.0 MVC
- **Database**: SQL Server with Entity Framework Core 6.0
- **Authentication**: ASP.NET Core Identity
- **Architecture**: Clean Architecture with Repository Pattern
- **Validation**: Model validation with custom attributes

### Frontend
- **Styling**: Custom CSS with modern design principles
- **JavaScript**: Vanilla JavaScript for enhanced interactivity
- **Forms**: Advanced form validation and user experience
- **Responsive**: Mobile-first responsive design
- **Icons**: Modern icon system

### Development Tools
- **IDE**: Visual Studio / Visual Studio Code
- **Version Control**: Git
- **Package Manager**: NuGet
- **Testing**: Unit tests with xUnit
- **Database Migrations**: Entity Framework Core Migrations

## 🏗 Architecture

The application follows Clean Architecture principles with clear separation of concerns:

```
TeamWorkFlow/
├── TeamWorkFlow/                 # Presentation Layer (MVC)
│   ├── Controllers/             # MVC Controllers
│   ├── Views/                   # Razor Views
│   ├── Areas/                   # Admin Area
│   ├── wwwroot/                 # Static files (CSS, JS, images)
│   └── Program.cs               # Application entry point
├── TeamWorkFlow.Core/           # Application Layer
│   ├── Contracts/               # Service interfaces
│   ├── Services/                # Business logic services
│   ├── Models/                  # View models and DTOs
│   ├── Constants/               # Application constants
│   └── Extensions/              # Extension methods
├── TeamWorkFlow.Infrastructure/ # Infrastructure Layer
│   ├── Data/                    # Entity Framework context and models
│   ├── Migrations/              # Database migrations
│   ├── Constants/               # Data constants
│   └── Common/                  # Common infrastructure code
└── UnitTests/                   # Unit test projects
    ├── TaskServiceUnitTests.cs
    ├── OperatorServiceUnitTests.cs
    ├── MachineServiceUnitTests.cs
    ├── ProjectServiceUnitTests.cs
    └── PartServiceUnitTests.cs
```

### Key Design Patterns
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Loose coupling and testability
- **MVC Pattern**: Clear separation of presentation logic
- **Service Layer**: Business logic encapsulation
- **DTO Pattern**: Data transfer optimization

## 🚀 Getting Started

### Prerequisites

Before running the application, ensure you have the following installed:

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Express, or full version)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/) for version control

### System Requirements

- **OS**: Windows 10/11, macOS 10.15+, or Linux (Ubuntu 18.04+)
- **RAM**: 4GB minimum, 8GB recommended
- **Storage**: 2GB free space
- **Browser**: Chrome 90+, Firefox 88+, Safari 14+, Edge 90+

## 📦 Installation

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/TeamWorkFlow.git
cd TeamWorkFlow
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Database Setup

#### Option A: Using SQL Server LocalDB (Recommended for Development)

```bash
# Update the connection string in appsettings.json if needed
# Default connection string uses LocalDB

# Apply database migrations
dotnet ef database update --project TeamWorkFlow.Infrastructure --startup-project TeamWorkFlow
```

#### Option B: Using SQL Server Express/Full

1. Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=TeamWorkFlowDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

2. Apply migrations:

```bash
dotnet ef database update --project TeamWorkFlow.Infrastructure --startup-project TeamWorkFlow
```

### 4. Build the Application

```bash
dotnet build
```

### 5. Run the Application

```bash
cd TeamWorkFlow
dotnet run
```

The application will be available at:
- **HTTPS**: https://localhost:7015
- **HTTP**: http://localhost:5142

## ⚙️ Configuration

### Database Configuration

The application uses Entity Framework Core with SQL Server. Configuration is managed through `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TeamWorkFlowDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Environment-Specific Settings

- **Development**: `appsettings.Development.json`
- **Production**: `appsettings.json`

### Default User Accounts

The application seeds default user accounts:

#### Administrator Account
- **Email**: `ap.softuni@gmail.com`
- **Role**: Administrator
- **Access**: Full system access

#### Operator Accounts
- **Email**: `jon.doe@softuni.bg`
- **Email**: `jane.doe@softuni.bg`
- **Role**: Operator
- **Access**: Limited to operator functions

## 📖 Usage

### User Roles

#### Administrator
- **Full Access**: Complete system administration
- **User Management**: Create and manage operator accounts
- **Data Management**: Full CRUD operations on all entities
- **System Configuration**: Manage system settings and configurations

#### Operator
- **Task Management**: View and update assigned tasks
- **Personal Dashboard**: View personal workload and schedule
- **Data Entry**: Add and update task-related information
- **Reporting**: Generate personal productivity reports

### Core Workflows

#### 1. Task Management Workflow

```
1. Administrator creates a new project
2. Administrator creates tasks linked to the project
3. Tasks are assigned to operators
4. Operators update task status as work progresses
5. Administrator monitors progress and completion
```

#### 2. Machine Management Workflow

```
1. Administrator registers CMM machines
2. Calibration schedules are set
3. Tasks are assigned to specific machines
4. Machine availability is tracked
5. Maintenance and calibration are scheduled
```

#### 3. Parts Inventory Workflow

```
1. Parts are registered in the system
2. Article and tool numbers are assigned
3. Parts are linked to projects
4. Inventory status is tracked
5. Parts availability is monitored
```

### Key Features Usage

#### Creating a New Task
1. Navigate to **Tasks** → **Add New Task**
2. Fill in task details:
   - Task name (5-100 characters)
   - Description (5-1500 characters)
   - Project number (6-10 digits)
   - Start date, end date, deadline (dd/MM/yyyy format)
   - Priority and status
3. Click **Save Task**

#### Managing Operators
1. Navigate to **Operators** → **All Operators**
2. View operator details, capacity, and availability
3. Use **Add New Operator** to register new team members
4. Update operator information as needed

#### Machine Calibration
1. Navigate to **Machines** → **All Machines**
2. View calibration schedules and status
3. Update calibration dates and status
4. Monitor machine availability

## 🧪 Testing

### Running Unit Tests

The project includes comprehensive unit tests for all service layers:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test UnitTests/UnitTests.csproj
```

### Test Coverage

- **TaskService**: Task creation, validation, and management
- **OperatorService**: Operator management and availability
- **MachineService**: Machine registration and calibration
- **ProjectService**: Project lifecycle management
- **PartService**: Parts inventory management

### Test Structure

```
UnitTests/
├── TaskServiceUnitTests.cs      # Task management tests
├── OperatorServiceUnitTests.cs  # Operator management tests
├── MachineServiceUnitTests.cs   # Machine management tests
├── ProjectServiceUnitTests.cs   # Project management tests
└── PartServiceUnitTests.cs      # Parts inventory tests
```

## 🤝 Contributing

We welcome contributions to TeamWorkFlow! Please follow these guidelines:

### Development Setup

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass: `dotnet test`
6. Commit your changes: `git commit -am 'Add some feature'`
7. Push to the branch: `git push origin feature/your-feature-name`
8. Submit a pull request

### Code Standards

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public methods
- Maintain test coverage above 80%
- Follow SOLID principles

### Pull Request Process

1. Update the README.md with details of changes if applicable
2. Update version numbers following semantic versioning
3. Ensure CI/CD pipeline passes
4. Request review from maintainers

## 🗺 Roadmap

### Current Version (v1.0)
- ✅ Core task management
- ✅ Operator management
- ✅ Machine (CMM) management
- ✅ Project management
- ✅ Parts inventory
- ✅ User authentication and authorization
- ✅ Responsive UI with modern design

### Upcoming Features (v1.1)
- 📋 Task dependencies and workflows
- 📊 Advanced reporting and analytics
- 🔔 Email notifications
- 📈 Dashboard with KPIs

### Future Enhancements (v2.0)
- 🤖 Automated task scheduling
- 🌐 Multi-language support
- ☁️ Cloud deployment options

For detailed feature roadmap, see [POTENTIAL_IMPROVEMENTS.md](potential_improvements.md)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

**Built with ❤️ for manufacturing teams worldwide**

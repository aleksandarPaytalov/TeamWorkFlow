# 🏭 TeamWorkFlow

**A comprehensive manufacturing workflow management system built with ASP.NET Core 6.0**

[![.NET](https://img.shields.io/badge/.NET-6.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/6.0)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-6.0-green.svg)](https://docs.microsoft.com/en-us/ef/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red.svg)](https://www.microsoft.com/en-us/sql-server)
[![Test Coverage](https://img.shields.io/badge/Test%20Coverage-90.1%25-brightgreen.svg)](#testing)
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
- [Recent Improvements](#recent-improvements)
- [Contributing](#contributing)
- [Roadmap](#roadmap)
- [License](#license)

## 🎯 Overview

TeamWorkFlow is a modern, web-based manufacturing workflow management system designed to streamline operations in manufacturing environments. The application provides comprehensive tools for managing tasks, operators, machines (CMMs), projects, and parts inventory with an intuitive, responsive interface.

### Key Benefits

- **Centralized Management**: Single platform for all manufacturing workflow operations
- **Real-time Tracking**: Live updates on task progress, machine status, and operator availability
- **Modern UI/UX**: Professional landing page with responsive design and modern animations
- **Role-based Access**: Secure authentication with Administrator and Operator roles
- **Scalable Architecture**: Clean architecture pattern with separation of concerns
- **Data Integrity**: Comprehensive validation and error handling
- **High Test Coverage**: 90.1% core layer coverage with comprehensive unit testing (688 tests)
- **Professional Design**: Modern glassmorphism effects and interactive user experience

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
- **Modern Landing Page**: Professional hero section with gradient backgrounds and animations
- **Glassmorphism Design**: Modern transparent effects with backdrop blur
- **Responsive Layout**: Mobile-first design that works perfectly on all devices
- **Interactive Elements**: Smooth animations, hover effects, and micro-interactions
- **Enhanced Navigation**: Streamlined admin navigation with improved visibility
- **Data Tables**: Sortable, searchable data tables with pagination
- **Visual Feedback**: Loading states, success/error messages, and progress indicators
- **Professional Styling**: Clean, modern interface with consistent design language

## 🛠 Technology Stack

### Backend
- **Framework**: ASP.NET Core 6.0 MVC
- **Database**: SQL Server with Entity Framework Core 6.0
- **Authentication**: ASP.NET Core Identity
- **Architecture**: Clean Architecture with Repository Pattern
- **Validation**: Model validation with custom attributes

### Frontend
- **Styling**: Modern CSS with glassmorphism effects and gradient backgrounds
- **Landing Page**: Professional hero section with animated statistics and features showcase
- **JavaScript**: Enhanced interactivity with smooth animations and scroll effects
- **Forms**: Advanced form validation with real-time feedback
- **Responsive**: Mobile-first responsive design with Tailwind CSS principles
- **Icons**: Modern SVG icon system with consistent styling
- **Animations**: CSS animations, loading states, and interactive hover effects

### Development Tools
- **IDE**: Visual Studio / Visual Studio Code
- **Version Control**: Git
- **Package Manager**: NuGet
- **Testing**: Comprehensive unit tests with NUnit (688 tests, 90.1% coverage)
- **E2E Testing**: Playwright tests for end-to-end UI testing and workflows (124 tests)
- **Code Coverage**: ReportGenerator for detailed coverage analysis
- **Database Migrations**: Entity Framework Core Migrations
- **CI/CD**: GitHub Actions for automated testing

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
├── UnitTests/                   # Comprehensive unit test suite (688 tests)
│   ├── TaskServiceUnitTests.cs      # 92.5% coverage
│   ├── OperatorServiceUnitTests.cs  # 100% coverage
│   ├── MachineServiceUnitTests.cs   # 96.4% coverage
│   ├── ProjectServiceUnitTests.cs   # 100% coverage
│   ├── PartServiceUnitTests.cs      # 100% coverage
│   └── SummaryServiceUnitTests.cs   # 100% coverage
└── TeamWorkFlow.PlaywrightTests/   # End-to-end UI tests
    ├── PageObjects/                 # Page Object Model classes
    ├── Tests/                       # E2E test classes
    ├── appsettings.json            # Test configuration
    └── playwright.config.ts        # Playwright configuration
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
    "DefaultConnection": "[YOUR_CONNECTION_STRING_HERE]"
  }
}
```

> **Security Note**: Replace `[YOUR_CONNECTION_STRING_HERE]` with your actual connection string. Never commit real connection strings to source control.

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
    "DefaultConnection": "[YOUR_CONNECTION_STRING_HERE]"
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
- **Email**: `admin@teamworkflow.local` (development)
- **Role**: Administrator
- **Access**: Full system access

#### Operator Accounts
- **Email**: `operator@teamworkflow.local` (development)
- **Email**: `operator2@teamworkflow.local` (development)
- **Role**: Operator
- **Access**: Limited to operator functions

> **Security Note**: The actual credentials are configured through environment variables or secure configuration files. The above are placeholder examples for development.

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

### Comprehensive Test Coverage

The project maintains **90.1% core layer coverage** with **688 comprehensive unit tests** across all service layers:

```bash
# Run all tests
dotnet test

# Run tests with coverage collection
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults

# Generate detailed coverage report
reportgenerator -reports:"TestResults\*\coverage.cobertura.xml" -targetdir:"TestResults\CoverageReport" -reporttypes:Html

# Run specific test project
dotnet test UnitTests/UnitTests.csproj

# Run Playwright E2E tests (requires running application)
# 1. Start the application first:
cd TeamWorkFlow && dotnet run

# 2. In a separate terminal, run E2E tests:
dotnet test TeamWorkFlow.PlaywrightTests/

# Run specific Playwright test suite
dotnet test TeamWorkFlow.PlaywrightTests/ --filter "TaskManagementTests"

# Use enhanced PowerShell scripts for better test management:
# Run unit tests with coverage analysis
.\run-coverage.ps1

# Install Playwright browsers and dependencies
.\install-playwright.ps1

# Run Playwright tests with enhanced features
.\TeamWorkFlow.PlaywrightTests\run-playwright-tests.ps1
```

### Service Coverage Statistics

| Service | Coverage | Tests | Status |
|---------|----------|-------|--------|
| **TaskService** | 92.5% | 125 tests | ✅ Excellent |
| **OperatorService** | 100% | 61 tests | ✅ Perfect |
| **MachineService** | 96.4% | 58 tests | ✅ Excellent |
| **ProjectService** | 100% | 75 tests | ✅ Perfect |
| **PartService** | 100% | 46 tests | ✅ Perfect |
| **SummaryService** | 100% | 14 tests | ✅ Perfect |
| **Controllers** | 95%+ | 107 tests | ✅ Excellent |
| **Core Components** | 90%+ | 102 tests | ✅ Excellent |
| **Overall Core Layer** | **90.1%** | **688 tests** | ✅ **Excellent** |

### Test Categories

#### Comprehensive Service Testing
- **CRUD Operations**: Complete Create, Read, Update, Delete testing
- **Business Logic**: All business rules and validation logic
- **Error Handling**: Exception scenarios and edge cases
- **Data Validation**: Input validation and constraint testing
- **Sorting & Filtering**: All sorting options and search functionality
- **Pagination**: Page-based data retrieval testing

#### Model Coverage Testing
- **Property Validation**: All model properties and constraints
- **Relationship Testing**: Entity relationships and foreign keys
- **Boundary Conditions**: Min/max values and edge cases
- **Default Values**: Property initialization and defaults

### Test Structure

```
UnitTests/
├── TaskServiceUnitTests.cs      # 125 tests covering all task operations
├── OperatorServiceUnitTests.cs  # 61 tests with 100% coverage
├── MachineServiceUnitTests.cs   # 58 tests covering all machine operations
├── ProjectServiceUnitTests.cs   # 75 tests with 100% coverage
├── PartServiceUnitTests.cs      # 46 tests with 100% coverage
├── SummaryServiceUnitTests.cs   # 14 tests with 100% coverage
├── Controllers/                 # 107 controller tests
├── Core Models & Extensions/    # 102 core component tests
└── Additional Test Suites/      # 100+ additional tests

TeamWorkFlow.PlaywrightTests/           # End-to-end UI tests (124 tests)
├── Tests/
│   ├── AuthenticationUITestsSimple.cs # Login/logout and security tests (7 tests)
│   ├── TaskManagementTests.cs   # Task CRUD workflow tests (11 tests)
│   ├── ProjectManagementTests.cs # Project management tests (12 tests)
│   ├── NavigationAndUITests.cs  # UI/UX and responsive design tests (15 tests)
│   ├── ErrorPageTests.cs        # Error page handling tests (14 tests)
│   ├── ErrorScenarioTests.cs    # Error scenario tests (14 tests)
│   ├── RoleRightsMatrixTests.cs # Role-based access tests (25 tests)
│   ├── SprintToDoTests.cs       # Sprint management tests (19 tests)
│   ├── ProjectCostCalculationTests.cs # Cost calculation tests (7 tests)
│   └── BaseTest.cs              # Common test setup and utilities
└── PageObjects/
    ├── BasePage.cs              # Common page functionality
    ├── LoginPage.cs             # Authentication page objects
    ├── TasksPage.cs             # Task management page objects
    ├── ProjectsPage.cs          # Project management page objects
    └── MachinesPage.cs          # Machine management page objects
```

### Quality Assurance

- **All Tests Passing**: 688/688 unit tests pass consistently
- **E2E Testing**: Playwright tests for UI and workflow validation
- **CI/CD Integration**: Automated testing in GitHub Actions
- **Coverage Reporting**: Detailed HTML coverage reports
- **Performance Testing**: Service performance validation
- **Edge Case Coverage**: Comprehensive boundary testing
- **Cross-Browser Testing**: Chrome, Firefox, Edge, and mobile browsers

## 🚀 Recent Improvements

### December 2024 - Major Quality & Design Updates

#### 🧪 **Comprehensive Test Coverage Achievement**
- **90.1% Core Layer Coverage**: Achieved exceptional test coverage across all services
- **688 Unit Tests**: Comprehensive test suite covering all business logic
- **Perfect Service Coverage**: 5 out of 6 services at 100% coverage
- **Robust Testing**: All CRUD operations, sorting, filtering, and edge cases covered
- **CI/CD Integration**: Automated testing with detailed coverage reporting

#### 🎨 **Modern Landing Page Redesign**
- **Professional Hero Section**: Gradient backgrounds with animated statistics
- **Features Showcase**: Six key features with modern card design
- **Benefits Section**: Compelling value propositions with glassmorphism effects
- **Responsive Design**: Mobile-first approach with smooth animations
- **Enhanced Navigation**: Streamlined admin interface with improved visibility
- **Loading Animations**: Professional loading states and micro-interactions

#### 🔧 **User Experience Enhancements**
- **Glassmorphism Design**: Modern transparent effects with backdrop blur
- **Smooth Animations**: CSS animations, hover effects, and scroll-based interactions
- **Interactive Elements**: Enhanced button states and visual feedback
- **Mobile Optimization**: Touch-friendly design for all screen sizes
- **Performance Optimization**: Hardware-accelerated animations and efficient CSS

#### 📊 **Quality Metrics Achieved**
- **Test Coverage**: 90.1% (exceeding 80% target by 10.1%)
- **Service Reliability**: All 688 tests passing consistently
- **Code Quality**: Comprehensive error handling and validation
- **User Experience**: Professional, modern interface design
- **Performance**: Optimized loading and interaction speeds

### Technical Achievements

#### **Service Coverage Excellence**
| Service | Before | After | Improvement |
|---------|--------|-------|-------------|
| TaskService | Basic | 92.5% | +92.5% |
| OperatorService | Basic | 100% | +100% |
| MachineService | 60% | 96.4% | +36.4% |
| ProjectService | Basic | 100% | +100% |
| PartService | Basic | 100% | +100% |
| SummaryService | 8.6% | 100% | +91.4% |

#### **Design Transformation**
- **Before**: Basic, outdated landing page with minimal styling
- **After**: Professional, modern design with animations and glassmorphism effects
- **Impact**: Significantly improved first impressions and user engagement

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
- Maintain test coverage above 90% (currently at 90.1%)
- Follow SOLID principles
- Comprehensive unit testing for all new features
- Modern UI/UX design principles

### Pull Request Process

1. Update the README.md with details of changes if applicable
2. Update version numbers following semantic versioning
3. Ensure CI/CD pipeline passes
4. Request review from maintainers

## 🗺 Roadmap

### Current Version (v1.0) - ✅ **COMPLETED**
- ✅ Core task management with comprehensive testing
- ✅ Operator management (100% test coverage)
- ✅ Machine (CMM) management (96.4% test coverage)
- ✅ Project management (100% test coverage)
- ✅ Parts inventory (100% test coverage)
- ✅ User authentication and authorization
- ✅ **Modern Landing Page** with professional design
- ✅ **Responsive UI** with glassmorphism effects
- ✅ **90.1% Test Coverage** across core services
- ✅ **Enhanced Navigation** with streamlined admin interface

### Recent Major Updates (December 2024)
- 🎨 **Professional Landing Page**: Modern hero section with animations
- 🧪 **Comprehensive Testing**: 688 unit tests + 124 E2E tests with 90.1% coverage
- 🎯 **Perfect Service Coverage**: 5+ services at 90+% coverage
- 🚀 **Enhanced UX**: Smooth animations and interactive elements
- 🔧 **Streamlined Navigation**: Removed redundant admin buttons

### Upcoming Features (v1.1)
- 📋 Task dependencies and workflows
- 📊 Advanced reporting and analytics
- 🔔 Email notifications
- 📈 Dashboard with KPIs
- 🎯 Performance optimization

### Future Enhancements (v2.0)
- 🤖 Automated task scheduling
- 🌐 Multi-language support
- ☁️ Cloud deployment options
- 📱 Mobile application
- 🔗 API integrations

### Augment short commands for server start and stop when the process is runned in background
- taskkill /F /IM TeamWorkFlow.exe 
- dotnet run 



# 📖 TeamWorkFlow Application Guidance

**Complete User Guide for Manufacturing Workflow Management System**

---

## 🎯 Quick Start Guide

### First Time Login
1. **Navigate** to the application URL (https://localhost:7015 for development)
2. **Click** "Login" on the landing page
3. **Use** your assigned credentials:
   - **Admin**: Full system access
   - **Operator**: Task and data management
   - **Guest**: Read-only access

### Navigation Overview
- **Home**: Landing page with system overview
- **Tasks**: Task management and assignment
- **Operators**: Team member management
- **Machines**: CMM equipment management
- **Projects**: Project lifecycle management
- **Parts**: Inventory and parts tracking
- **Sprint**: Sprint planning and task organization
- **Admin**: Administrative functions (Admin only)

---

## 🔐 User Roles & Permissions

### 👑 Administrator Role
**Full System Access**
- ✅ Create, edit, delete all entities
- ✅ User role management
- ✅ Bulk operations (delete multiple items)
- ✅ Admin dashboard access
- ✅ System configuration
- ✅ Two-admin confirmation for role changes

### 👷 Operator Role
**Operational Access**
- ✅ View and update assigned tasks
- ✅ Create new tasks and projects
- ✅ View all system data
- ✅ Update task status and progress
- ❌ Delete operations (limited)
- ❌ User management
- ❌ Admin functions

### 👀 Guest Role
**Read-Only Access**
- ✅ View all data (tasks, projects, machines, etc.)
- ❌ Create, edit, or delete operations
- ❌ Task assignments
- ❌ Admin functions

---

## 📋 Task Management

### Creating a New Task
1. **Navigate** to Tasks → "Add New Task"
2. **Fill Required Fields**:
   - **Task Name**: 5-100 characters
   - **Description**: 5-1500 characters
   - **Project Number**: 6-10 digits
   - **Start Date**: dd/MM/yyyy format
   - **End Date**: dd/MM/yyyy format
   - **Deadline**: dd/MM/yyyy format
   - **Priority**: High, Medium, Low
   - **Status**: Open, In Progress, Finished, Canceled

3. **Save** the task

### Task Assignment
1. **Open** task details page
2. **Assign Machine**: 
   - Select available CMM machine
   - System validates machine availability
   - Shows error if machine is occupied
3. **Assign Operator**:
   - Select from available operators
   - Multiple operators can be assigned
   - System tracks operator capacity

### Task Status Workflow
```
Open → In Progress → Finished
  ↓         ↓          ↓
Canceled ← Canceled ← Canceled
```

### Task Time Tracking
- **Start Date**: Automatically set when status changes to "In Progress"
- **End Date**: Automatically set when marked as "Finished"
- **Actual Time**: Calculated in hours
- **Person-Hours**: Multiplied by number of assigned operators

### Search and Filtering
- **Search**: By task name or project number
- **Sort**: Name, project number, start date, deadline (ascending/descending)
- **Status Filter**: Open, In Progress, Finished, Canceled, All Statuses
- **Pagination**: Configurable items per page

---

## 👥 Operator Management

### Adding New Operators
1. **Navigate** to Operators → "Add New Operator"
2. **Required Information**:
   - **Full Name**: Operator's complete name
   - **Email**: Must be registered user email
   - **Phone Number**: Contact information
   - **Capacity**: Working hours (4-12 hours/day)
   - **Availability Status**: At work, Sick leave, Vacation, etc.
   - **Active Status**: Active/Inactive

### Operator Status Management
**Availability Statuses**:
- **At Work**: Available for task assignment
- **Sick Leave**: Temporarily unavailable
- **Vacation**: Scheduled time off
- **Training**: In training sessions
- **Other**: Custom status

**Business Rules**:
- Only operators with "At Work" status can be marked as Active
- Admin can use toggle buttons for quick activation
- Activation automatically sets status to "At Work"

### Operator Assignment
- **Multiple Tasks**: Operators can handle multiple tasks
- **Capacity Tracking**: System monitors workload
- **Performance Metrics**: Track completion rates

---

## 🏭 Machine (CMM) Management

### Machine Registration
1. **Navigate** to Machines → "Add New Machine"
2. **Required Fields**:
   - **Machine Name**: Unique identifier
   - **Description**: Machine specifications
   - **Calibration Date**: Last calibration
   - **Next Calibration**: Scheduled maintenance
   - **Capacity**: Working hours
   - **Image URL**: Visual documentation

### Machine Assignment Rules
- **Availability Check**: System validates machine availability
- **Calibration Status**: Warns if calibration is due
- **Capacity Management**: Tracks machine workload
- **Conflict Prevention**: Prevents double-booking

### Machine Status Indicators
- **Available**: Ready for task assignment
- **Occupied**: Currently assigned to task
- **Maintenance**: Under calibration/repair
- **Out of Service**: Temporarily unavailable

---

## 📊 Project Management

### Creating Projects
1. **Navigate** to Projects → "Add New Project"
2. **Project Information**:
   - **Project Name**: Descriptive title
   - **Client Name**: Customer information
   - **Project Number**: Unique identifier
   - **Description**: Project details
   - **Total Hours**: Estimated time (0-5000 hours)
   - **Appliance**: Project category
   - **Status**: Active, Completed, On Hold, Canceled

### Project-Task Integration
- **Link Tasks**: Associate tasks with projects
- **Progress Tracking**: Monitor project completion
- **Resource Allocation**: Track time and resources
- **Parts Association**: Link parts to projects

### Project Details View
- **Task List**: All associated tasks
- **Parts List**: Related inventory items
- **Progress Metrics**: Completion percentage
- **Time Tracking**: Total hours spent

---

## 🔩 Parts Inventory Management

### Adding Parts
1. **Navigate** to Parts → "Add New Part"
2. **Part Information**:
   - **Part Name**: Descriptive name
   - **Article Number**: Unique identifier
   - **Client Number**: Customer reference
   - **Tool Number**: Range 1000-9999
   - **Description**: Part specifications
   - **Project Association**: Link to project
   - **Status**: Available, In Use, Ordered, Discontinued
   - **Image URL**: Visual documentation

### Parts Status Workflow
- **Available**: Ready for use
- **In Use**: Currently assigned
- **Ordered**: Pending delivery
- **Discontinued**: No longer available

### Inventory Tracking
- **Project Links**: Associate parts with projects
- **Status Updates**: Track part lifecycle
- **Search Functionality**: Find parts by various criteria

---

## 🚀 Sprint Planning (Sprint To Do)

### Sprint Overview
GitHub RoadMap-style interface for task organization and capacity planning.

### Features
1. **Drag-and-Drop**: Organize tasks visually
2. **Capacity Planning**: Check if targets are achievable
3. **Status Management**: Update task status directly from cards
4. **Filtering Options**:
   - Search by term
   - Status filter
   - Priority filter
   - Project filter
   - Operator filter
   - Machine filter

### Sprint Task Cards
- **Single Action Button**: Change task status
- **Essential Information**: Task name, priority, assignments
- **Visual Indicators**: Status colors and icons
- **Quick Updates**: Direct status changes

---

## ⚙️ Admin Dashboard

### User Role Management
**Access**: Admin → User Role Management

#### Three-Tier Role System
1. **Guest**: Read-only access (default for new users)
2. **Operator**: Operational access
3. **Admin**: Full system access

#### Role Assignment Process
1. **New Users**: Start as Guest automatically
2. **Role Assignment**: Use "Assign a role" button
3. **Operator Promotion**: Adds user to Operator database
4. **Admin Promotion**: Requires admin privileges

#### Admin Demotion System
- **Two-Admin Confirmation**: Requires second admin approval
- **Self-Demotion Prevention**: Admins cannot demote themselves
- **Request System**: Create demotion requests for review

### Bulk Operations (Admin Only)
**Available for**:
- Tasks
- Operators
- Machines
- Projects
- Parts

**Process**:
1. **Select Items**: Use checkboxes
2. **Choose Action**: Delete/Archive
3. **Confirm**: Review selection
4. **Execute**: Process bulk operation

### Admin Features
- **Active/Inactive Toggles**: Quick operator status management
- **System Monitoring**: Track all assignments and activities
- **Data Management**: Full CRUD operations
- **Security Controls**: Role-based access enforcement

---

## 🔍 Search and Filtering

### Universal Search Features
**Available on all listing pages**:
- **Text Search**: Name, description, numbers
- **Status Filtering**: All status types
- **Sorting Options**: Multiple criteria
- **Pagination**: Configurable page sizes

### Advanced Filtering
- **Date Ranges**: Start date, end date, deadlines
- **Priority Levels**: High, Medium, Low
- **Assignment Status**: Assigned/Unassigned
- **Availability**: Active/Inactive items

---

## 📱 Mobile Responsiveness

### Design Principles
- **Mobile-First**: Optimized for smartphones
- **Responsive Navigation**: Hamburger menu for small screens
- **Touch-Friendly**: Large buttons and touch targets
- **Readable Text**: Appropriate font sizes
- **Optimized Forms**: Mobile-friendly input fields

### Navigation Behavior
- **Home Page**: Full hamburger menu with all options
- **Auth Pages**: Only "Back to Home" button
- **Admin Pages**: No hamburger menu on small devices
- **Profile Pages**: Full navigation in hamburger menu

---

## 🎨 User Interface Features

### Modern Design Elements
- **Glassmorphism Effects**: Transparent backgrounds with blur
- **Smooth Animations**: CSS transitions and hover effects
- **Gradient Backgrounds**: Modern color schemes
- **Interactive Elements**: Visual feedback on actions
- **Loading States**: Progress indicators

### Form Validation
- **Real-Time Feedback**: Immediate validation
- **Error Messages**: Clear, helpful error text (red color)
- **Success Indicators**: Green confirmation feedback
- **Required Fields**: Clear marking of mandatory fields
- **Date Format**: dd/MM/yyyy format enforcement

---

## 🔧 System Configuration

### Date Format
- **Standard**: dd/MM/yyyy
- **Input Fields**: Date pickers with correct format
- **Display**: Consistent formatting throughout

### Validation Rules
- **Server-Side**: Database constraint enforcement
- **Client-Side**: Immediate user feedback
- **Business Rules**: Custom validation logic
- **Error Handling**: Graceful error management

### Security Features
- **Authentication**: ASP.NET Core Identity
- **Authorization**: Role-based access control
- **Session Management**: Secure session handling
- **Data Protection**: Input validation and sanitization

---

## 📞 Support and Troubleshooting

### Common Issues
1. **Login Problems**: Check credentials and role assignment
2. **Permission Errors**: Verify user role and permissions
3. **Validation Errors**: Review required fields and formats
4. **Assignment Conflicts**: Check machine/operator availability

### Best Practices
- **Regular Backups**: Maintain data backups
- **User Training**: Ensure proper role understanding
- **Data Validation**: Always validate input data
- **Status Updates**: Keep task statuses current
- **Regular Maintenance**: Monitor system performance

---

*For technical support or additional features, contact your system administrator.*

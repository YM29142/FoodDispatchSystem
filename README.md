# FoodDispatchSystem



FoodDispatchSystem is a web application for managing food orders, kitchen operations, and dispatch workflows. It was developed with ASP.NET Core MVC as a practical project focused on applying backend development, database management, authentication, authorization, and software testing concepts.



## Technologies



- C# / .NET

- ASP.NET Core MVC

- Entity Framework Core

- SQL Server

- ASP.NET Core Identity

- Razor Views

- HTML5 / CSS3

- Bootstrap

- JavaScript

- Git / GitHub



## Main Features



- Product and category management

- Order creation and management

- Order status workflow

- Kitchen and dispatch operations

- Employee management

- Sales reports with date filters

- Top-selling product reports

- Business configuration

- User authentication

- Role-based authorization

- Password recovery by email

- Automated tests



## Roles and Permissions



The application uses ASP.NET Core Identity and role-based authorization to control access to features and actions.



Current roles include:



- **Administrator** - system administration, employees, reports and configuration
- **Cashier** - order creation and management
- **Kitchen** - preparation workflow
- **Dispatch** - order delivery workflow


## Order Workflow

Orders move through a controlled status workflow:

`Pending -> Preparing -> Ready -> Delivered`



Orders can also be cancelled when applicable.



Each role has access only to the actions required for its responsibilities.



## Project Structure



The solution is organized into two main projects:



FoodDispatchSystem

- src/FoodDispatchSystem.Web

 - Controllers

 - Data

 - Migrations

 - Models

 - Services
 - ViewModels

 - Views

- wwwroot

- FoodDispatchSystem.Tests



The web application follows the MVC pattern and separates models, views, controllers, services, data access and view models.



## Database



The application uses SQL Server with Entity Framework Core for data persistence and database migrations.



The data model supports areas such as users and roles, employees, products, categories, orders, order details, order status history and business configuration.



## Authentication and Security



FoodDispatchSystem uses ASP.NET Core Identity for authentication and authorization.



Implemented security-related features include:



- Role-based access control

- Controller and action authorization

- Anti-forgery validation

- Password recovery by email

- Protected functionality based on authenticated user roles



Sensitive production credentials are not stored in the public repository.



## Testing



The solution includes a dedicated automated test project:



`FoodDispatchSystem.Tests`



Automated tests are used to validate important application behavior and help prevent regressions as new functionality is introduced.



## Running the Project Locally



### Requirements



- .NET SDK

- SQL Server

- Visual Studio or another compatible IDE



### Basic Setup



1. Clone the repository.

2. Configure the local SQL Server connection string if necessary.

3. Apply the Entity Framework Core migrations.

4. Build the solution.

5. Run the web application.



## Project Status



FoodDispatchSystem is under active development.



Current functionality includes the core order, kitchen, dispatch, administration and reporting workflows. Additional functionality is being developed incrementally using feature branches and automated testing.



## Author



**Yader Mauricio Ramírez Baires**



Systems Engineering student focused on .NET development, backend technologies and software systems.



GitHub: https://github.com/YM29142



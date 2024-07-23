# Library API

This project is a Library Management System API built with ASP.NET Core. The API allows users to manage books, book transactions (borrowing, reserving, and returning books), and handle user authentication using JWT (JSON Web Tokens).

## Features

- User registration and login with JWT authentication - Please note that User Registration Action has not been protected, to allow for you to create a user account and get a head-start as soon as you get the setup completed.
- Book management (CRUD operations)
- Book transaction management (borrow, reserve, return, cancel reservation)
- Reservation notifications
- Swagger for API documentation and testing

## Assumptions

- **Database Configuration**: It is assumed that the SQL Server database is properly set up and the connection string provided in `appsettings.json` is correct and accessible
- **User Roles and Permissions**: The current implementation does not differentiate between user roles (e.g., admin, user). All authenticated users have the same level of access
- **JWT Configuration**: The JWT secret key and issuer are securely stored and managed. The secret key is sufficiently complex to ensure security
- **Book Transactions**: It is assumed that a book can only have one active transaction at a time (and only one copy of a title is available in the library), either reserved or borrowed
- **Notification Handling**: Notifications for reservations are simple flags in the database. Advanced notification mechanisms (e.g., email, SMS) are not implemented in this version
- **Environment**: The application is assumed to be running in a development or staging environment. For production deployment, additional configurations and security measures are required

## Future work
- For the future, we'd need to address the assumptions provided above, and implement the functionality in a more flexible manner
- To enhance the architecture and maintain separation of concerns, we can introduce repository patterns to handle data access. This involves creating repositories that encapsulate the logic for accessing the database, and then injecting these repositories into the service layer. This approach decouples the business logic from the data access logic, promoting a cleaner and more maintainable codebase. Instead of directly interacting with the ``DbContext`` within the services, the services will rely on the repositories to perform CRUD operations and other data interactions
- **User Roles and Permissions**: Implement role-based access control (RBAC) to provide different levels of access for admins, librarians, and regular users
- **Enhanced Notifications**: Integrate more sophisticated notification systems, such as email or SMS, to notify users about reservation and borrowing statuses
- **Book Search Improvements**: Enhance the book search functionality to include more filters (e.g., genre, publication date) and sorting options
- **Error Handling and Logging**: Improve error handling and logging mechanisms to provide more detailed information for debugging and monitoring - and use a library like SeriLog to capture and persist logs
- **Unit and Integration Testing**: Add comprehensive unit and integration tests to ensure the reliability and stability of the application
- **Deployment Automation**: Set up CI/CD pipelines for automated testing and deployment to various environments
- **API Versioning**: Introduce API versioning to manage changes and ensure backward compatibility for API consumers
- **Performance Optimization**: Conduct performance profiling and optimization to handle larger datasets and higher traffic volumes efficiently
- **Internationalization**: Add support for multiple languages to cater to a diverse user base

## Technologies Used

- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT (JSON Web Tokens)
- Swagger

## Setup and Installation

### Prerequisites

- .NET 6.0 SDK or later
- SQL Server

### Clone the Repository

```bash
git clone https://github.com/hsvotwa/LibraryApi.git
cd LibraryApi
```

### Configure the Database

Update the appsettings.json file with your database connection string and other settings:

```bash
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "ConnectionString": "Data Source=.; Initial Catalog=LibraryAPI; Integrated Security=true; TrustServerCertificate=true;"
  },
  "LibrarySettings": {
    "ReservationLengthInDays": "1",
    "BookingLengthInDays": "14"
  },
  "Jwt": {
    "Key": "JFKF20952NGW423KLMSG9423MFKKH423",
    "Issuer": "hpsvtw@gmail.com",
    "TokenExpiryInHours": "1"
  },
  "AllowedHosts": "*"
}
```


### Migrate the Database

```bash
dotnet ef database update
```


### Run the Application, API Documentation and Testing with Swagger

Use any of the launch profiles to run the API on Swagger:

```bash
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "dotnetRunMessages": true,
      "applicationUrl": "http://localhost:5238"
    },
    "https": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "dotnetRunMessages": true,
      "applicationUrl": "https://localhost:7113;http://localhost:5238"
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "Container (Dockerfile)": {
      "commandName": "Docker",
      "launchBrowser": true,
      "launchUrl": "{Scheme}://{ServiceHost}:{ServicePort}/swagger",
      "environmentVariables": {
        "ASPNETCORE_HTTPS_PORTS": "8081",
        "ASPNETCORE_HTTP_PORTS": "8080"
      },
      "publishAllPorts": true,
      "useSSL": true
    }
  }
}
```

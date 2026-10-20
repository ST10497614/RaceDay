# RaceDay API — Part 2

## 1. Project Overview

RaceDay is a RESTful Web API developed using ASP.NET Core and C#. The application supports the management of race events, organisers, participants, categories, enrolments and race results.

The API uses SQL Server for data storage, Entity Framework Core for database access, and JSON Web Tokens (JWT) for authentication.

This project builds on the approved Part 1 database design. The existing database schema is retained.

## 2. Technologies Used

- C#
- ASP.NET Core Web API
- .NET 10
- Microsoft SQL Server
- Entity Framework Core
- JWT authentication
- Swagger / OpenAPI
- xUnit or the test framework configured in the test project
- Git and GitHub
- GitHub Actions for continuous integration (CI)

## 3. Project Structure

The repository contains the following main components:

```text
RaceDay/
├── .github/
│   └── workflows/
│       └── dotnet-ci.yml
├── RaceDay.API/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Models/
│   ├── Services/
│   ├── appsettings.json
│   ├── Program.cs
│   └── RaceDay.API.csproj
├── RaceDay.API.Tests/
│   ├── AuthControllerTests.cs
│   ├── CategoriesControllerTests.cs
│   ├── EnrolmentsControllerTests.cs
│   ├── EventsControllerTests.cs
│   ├── ResultsControllerTests.cs
│   ├── UsersControllerTests.cs
│   └── RaceDay.API.Tests.csproj
└── README.md
```

The exact repository structure may differ slightly depending on the location of the solution file and project files.

## 4. Prerequisites

Before running the project, install the following:

1. Visual Studio with the ASP.NET and web development workload.
2. The .NET 10 SDK.
3. Microsoft SQL Server Express or another compatible SQL Server instance.
4. SQL Server Management Studio (SSMS), recommended for database management.
5. Git, if you want to clone and manage the repository locally.

Verify that the .NET SDK is installed by running:

```bash
dotnet --version
```

The installed SDK should be compatible with .NET 10.


## 5. Database Configuration

RaceDay uses SQL Server and the existing RaceDayDB database.

The development environment uses the following SQL Server instance:

```text
LabVM2049939\SQLEXPRESS
```

The database must be created and configured using the approved Part 1 SQL script or the existing database backup.

### Configure the connection string

Open `RaceDay.API/appsettings.json` and locate the `ConnectionStrings` section.

Configure the connection string to match your local SQL Server instance and database name. For example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=LabVM2049939\\SQLEXPRESS;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## 6. Restore Dependencies

Open a terminal in the repository directory containing the solution file.

Restore the project dependencies:

```bash
dotnet restore
```

## 7. Build the Application

Build the solution in Release configuration:

```bash
dotnet build --configuration Release
```

Resolve any build errors before running the application.

## 8. Run the RaceDay API

From the repository root, run:

```bash
dotnet run --project RaceDay.API/RaceDay.API.csproj
```

If your API project is located in a different directory, adjust the path accordingly.

Alternatively, in Visual Studio:

1. Open the RaceDay solution.
2. Set `RaceDay.API` as the startup project.
3. Select the appropriate launch profile.
4. Press **F5** to debug or **Ctrl+F5** to run without debugging.

The application will display its local address in the terminal or Visual Studio output window.

### Access Swagger

When Swagger is enabled in the selected development environment, open the Swagger URL shown by the application, for example:

```text
(https://localhost:7219/swagger/index.html)
```
Swagger provides an interactive interface for viewing and testing the available API endpoints.

## 9. Authentication and JWT

RaceDay supports authentication using JSON Web Tokens (JWT).

The authentication process is:

1. Register a participant or organiser using the appropriate registration endpoint.
2. Submit the login details to the login endpoint.
3. Receive a JWT token if authentication succeeds.
4. In Swagger, click **Authorize**.
5. Enter the token using the format required by the Swagger configuration, typically:

   `Bearer YOUR_JWT_TOKEN`

6. Execute protected API requests.

JWT configuration includes the signing key, issuer, audience and token expiry settings.

For local development, configure these settings using the project's expected configuration keys. Use a secure secret store or environment variables for signing keys and other sensitive values. Never publish real JWT signing keys.

## 10. Automated Testing

The repository includes an automated test project named `RaceDay.API.Tests`.

The test files cover the following areas:

- Authentication
- Categories
- Enrolments
- Events
- Results
- users

Run the automated tests from the repository root:

```bash
dotnet test --configuration Release
```

This command runs the tests included in the solution.

If the test project is not included in the solution, run the tests using its actual repository-relative path:

```bash
dotnet test PATH_TO_TEST_PROJECT.csproj --configuration Release
```

## 11. Continuous Integration with GitHub Actions

RaceDay uses GitHub Actions to automate the build and testing process.

The workflow is stored in:

```text
.github/workflows/dotnet-ci.yml
```

The intended CI process is:

1. Check out the repository.
2. Install the required .NET SDK.
3. Restore project dependencies.
4. Build the API and test project.
5. Run the automated tests.

The workflow is configured to run when changes are pushed to the configured branch or when a pull request targets that branch. It can also be started manually if workflow dispatch is enabled.

### Verify the CI workflow

1. Push the workflow file and project changes to GitHub.
2. Open the repository on GitHub.
3. Select the **Actions** tab.
4. Open the latest RaceDay CI run.
5. Check the build and test steps.
6. Confirm that the workflow completes successfully.

A green check mark indicates that the workflow completed successfully. A failed workflow should be investigated and corrected before submission.

### CI screenshot

Insert a screenshot showing the successful RaceDay CI workflow here.

**Evidence file:** `docs/screenshots/raceday-ci-success.png`

Create the `docs/screenshots` folder and save your screenshot there if you want to keep the evidence in the repository.

## 13. API Functionality

The API includes controllers and related functionality for:

- Authentication and login
- User-related operations
- Participant management
- Organiser management
- Event management
- Category management
- Enrolment management
- Race result management

The exact HTTP methods, request formats, validation rules and authentication requirements can be inspected in the controller implementations and Swagger documentation.

## 14. Troubleshooting

### Database connection fails

- Confirm that SQL Server is running.
- Verify the server instance and database name.
- Check the connection string in your configuration.
- Confirm that the database exists and that your account has access.

### Swagger does not open

- Check the application startup output for the correct URL and port.
- Confirm that Swagger is enabled for the current environment.
- Check for startup exceptions in the terminal or Visual Studio output.

### Login or authentication fails

- Confirm that the supplied login credentials are correct.
- Verify the JWT configuration.
- Check the token expiry and the Swagger Authorize configuration.
- Ensure protected endpoints receive the token in the expected format.

### Automated tests fail

- Run `dotnet test` locally.
- Read the test failure details.
- Confirm that the required dependencies and test configuration are present.
- Check whether the tests require a database or other external service.

### GitHub Actions fails

- Open the failed run under the GitHub Actions tab.
- Expand the failed step to inspect the error.
- Check the repository-relative paths to the API, test project and solution.
- Confirm that the workflow uses the correct .NET SDK version.

## 15. Demonstration Video

**Video link:** [Insert your RaceDay project demonstration video URL here]

The demonstration should show:

1. The RaceDay API running.
2. Swagger and the available endpoints.
3. Authentication and successful login.
4. A JWT-authenticated request.
5. Example event, category or enrolment operations.
6. Automated test execution.
7. The successful GitHub Actions workflow.

Ensure that the video link is accessible to the assessor.


## 17. Conclusion

The RaceDay Part 2 project extends the existing database design into a RESTful API using ASP.NET Core, SQL Server and Entity Framework Core. It includes JWT-based authentication, API endpoints for the main race-management entities, automated testing and a GitHub Actions workflow for continuous integration.

The project documentation provides instructions for configuring the database, running the application, testing the endpoints and verifying the CI process.

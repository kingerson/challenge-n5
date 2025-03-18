# Technical Challenge N5 Company

This project is a technical challenge that implements an API using **.NET 8**, following the principles of **Clean Architecture**, **SOLID**, and **Clean Code**. The CQRS pattern is applied to separate read and write operations, and a modular architecture is used to facilitate scalability and maintainability.

## Table of Contents

- [Project Structure](#project-structure)
- [Endpoints and Usage Examples](#endpoints-and-usage-examples)
- [Prerequisites and Security](#prerequisites-and-security)
- [Docker Configuration](#docker-configuration)
- [Technologies Used](#technologies-used)
- [Test Execution](#test-execution)
- [License](#license)
- [Contact](#contact)

## Project Structure

The solution is organized in a modular way and consists of the following components:

```plaintext

challenge-n5/
├─ clean-api-template/
│  ├─ src/
│  │  ├─ Application/
│  │  │  ├─ Behaviours
│  │  │  ├─ Commands
│  │  │  ├─ Queries
│  │  │  ├─ Extensions
│  │  │  ├─ Mapping
│  │  │  ├─ Notifications
│  │  │  ├─ BusinessExceptionMessages (contains .resx and .Designer.cs files for business exception messages)
│  │  │  └─ Application.csproj
│  │  ├─ Domain/
│  │  │  ├─ Aggregates (domain entities and aggregates)
│  │  │  └─ Domain.csproj
│  │  ├─ Infrastructure/
│  │  │  ├─ Migrations (for EF Core)
│  │  │  ├─ Services
│  │  │  ├─ DependencyInjection.cs
│  │  │  └─ Infrastructure.csproj
│  │  └─ Presentation/
│  │     ├─ Controllers (LoginController, PermissionController, etc.)
│  │     ├─ appsettings.json
│  │     ├─ Program.cs
│  │     └─ Presentation.csproj
│  ├─ tests/
│  │  ├─ Application.Tests/ (unit tests for business logic and CQRS)
│  │  ├─ Integration.Tests/ (integration tests, endpoint validations, etc.)
│  │  └─ Presentation.Tests/ (tests for controllers and endpoints)
│  └─ Other files (global.json, Dockerfile, etc.)
├─ KafkaConsumer/ (host project to consume and process Kafka messages)
├─ docker-compose.yml (orchestrates all solution containers)
├─ LICENSE.md
└─ README.md

```
## Endpoints and Usage Examples
The API is documented with **Swagger** and exposes the following main endpoints:

## Login
- **POST** `/api/Login`
    - Description: Authenticates the user and generates a JWT token.
    - **cURL Example**:

``` bash   
curl -X POST "http://localhost:8081/api/Login" \
     -H "Content-Type: application/json" \
     -d '{"userName": "yourUsername", "password": "yourPassword"}'
```

## Permissions
- **POST** `/api/Permission`
Description: Creates a new permission request.
- **GET** `/api/Permission`
Description: Retrieves the list of registered permissions.
- **PUT** `/api/Permission`
Description: Modifies an existing permission.
- **GET** `/api/Permission/{id}`
Description: Retrieves details of a specific permission.
## cURL Example for Protected Endpoints
Remember that you must first obtain a JWT token via `/api/Login` before calling these endpoints. Then include the token in the `Authorization` header using the format `Bearer {token}`:

``` bash   
curl -X GET "http://localhost:8081/api/Permission" \
     -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Prerequisites and Security
- **JWT Token Generation:**
To access the permission endpoints, you must first generate a token by authenticating via the /api/Login endpoint.

- **Request Format:**
The API expects and returns data in JSON format. In case of errors, the response will follow the ApiError schema documented in Swagger.

- **Security:**
All requests to the permission endpoints must include the JWT token in the Authorization header in the following format:

``` bash   
Authorization: Bearer YOUR_JWT_TOKEN
```

## Docker Configuration
The project is deployed using Docker, and the docker-compose.yml file orchestrates the following services:

- **Zookeeper and Kafka:**
Provide messaging and event processing.
    - **Kafka**: Configured to create the topic test-topic and expose the necessary ports.
    - **Zookeeper**: Acts as the coordinator for Kafka.

- **Kafka Consumer Worker:**
A project responsible for consuming and processing messages from Kafka.

- **Elasticsearch and Kibana:**
    - Elasticsearch: Used for data indexing and search.
    - Kibana: A visualization tool that allows you to explore and analyze documents inserted into Elasticsearch. Access Kibana at http://localhost:5601.

- **SQL Server (sqlserverdev):**
A relational database for data persistence.

- **API (.NET 8):**
The container running the main application, configured to connect to SQL Server, Kafka, and Elasticsearch.


## Execution
To launch all services, run the following command:
``` bash   
docker-compose up --build
```

## Technologies Used
The project has been developed using a range of technologies and tools to ensure quality and scalability:

- **.NET 8:** Platform for API development.
- **Kafka & Zookeeper:** Messaging system for event processing.
- **Serilog:** Advanced logging management.
- **Docker:** Containerization and service orchestration.
- **SQL Server:** Relational database.
- **Dapper:** Micro ORM for fast data access.
- **Entity Framework Core (EF Core):** ORM for data persistence.
- **CQRS:** Pattern to separate read and write operations.
- **Elasticsearch & Kibana:** For indexing, searching, and visualizing logs and documents.
- **Xunit, Moq:** Framework and tools for unit testing.
- **FluentValidation & Fluent Assertions:** For data validation and assertions in tests.
- **JWT:** Implementation for authentication and authorization.
- **SOLID, Clean Code & Clean Architecture:** Principles and patterns that ensure maintainable, scalable, and high-quality code.

## Test Execution
The project includes a suite of unit and integration tests, organized in the tests folder:

- **Application.Tests:** Tests for business logic and validations.
- **Integration.Tests:** Verification of integration between components and services.
- **Presentation.Tests:** Tests for API controllers and endpoints.
``` bash   
dotnet test
```

## License
This project is distributed under the **MIT License**. See the LICENSE file for more details.

## Contact
For any questions or suggestions, please contact me via:

- **Email:** g.navarrope@gmail.com
- **GitHub:** kingerson

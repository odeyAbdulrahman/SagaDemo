# SagaDemo

A demonstration project showcasing the implementation of the Saga pattern for distributed transactions.

## Overview

This project demonstrates how to implement the Saga pattern to manage distributed transactions across multiple microservices. The Saga pattern helps maintain data consistency in a distributed system by breaking down a large transaction into a series of smaller, manageable steps.

## Features

- Distributed transaction management using Saga pattern
- Event-driven architecture
- Microservices communication
- Error handling and compensation mechanisms

## Prerequisites

- .NET 6.0 or later
- Docker (optional, for containerized deployment)
- Visual Studio 2022 or VS Code

## Getting Started

1. Clone the repository:
```bash
git clone https://github.com/yourusername/SagaDemo.git
cd SagaDemo
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run
```

## Project Structure

- `SagaDemo.API` - Main API project
- `SagaDemo.Services` - Core business logic and services
- `SagaDemo.Infrastructure` - Infrastructure components
- `SagaDemo.Tests` - Unit and integration tests

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

[@linkedin](https://www.linkedin.com/in/odey-abdalrahman)

Project Link: [https://github.com/odeyAbdulrahman/SagaDemo](https://github.com/odeyAbdulrahman/SagaDemo)
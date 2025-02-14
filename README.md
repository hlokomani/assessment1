# Hlokomani Khondlo Assessment

A .NET Core application that processes test scores from CSV files and provides both an API and console interface for uploading and getting scores. The SQLite db comes pre-populated with test data.

## Project Structure

- `Scores.Api` - Web API with Swagger UI
- `Scores.Core` - Core Business Logic Code
- `Scores.Console` - Console application for processing CSV files

## Prerequisites

- .NET 7.0 SDK or later
- Visual Studio Code or any preferred IDE

## Getting Started

1. Clone the repository:
```bash
git clone https://github.com/hlokomani/assessment1
cd https://github.com/hlokomani/assessment1
```

2. Run the API:
```bash
cd Scores.Api
dotnet run
```

The API will be available at `http://localhost:5250` with Swagger UI for testing (Port Might Change)

## Using the API

Access Swagger UI at `http://localhost:5250/swagger` to:

### Endpoints
- GET `api/Scores/all` - Get all scores
- GET `/api/Scores/top` - Get the highest scoring students
- GET `/api/Scores/{firstName}/{secondName}` - Get score for a specific student
- POST `/api/Scores` - Add a single score
- POST `/api/Scores/upload` - Upload CSV data

### Testing CSV Upload
Use this format in the request body:
```
"First Name,Second Name,Score\nJohn,Doe,85\nJane,Smith,92"
```

## Database

The application comes with a pre-populated SQLite database (scores.db) containing test data. No additional setup is required.

## Console Application

To process CSV files using the console application:
```bash
cd Scores.Console
dotnet run -- path/to/your/file.csv
```

CSV file format:
```csv
First Name,Second Name,Score
John,Doe,85
Jane,Smith,92
```

## Technical Details

- Built with .NET 9.0.2
- Uses SQLite for data storage
- Custom CSV parser implementation
- RESTful API design
- Swagger/OpenAPI documentation

## Design Choices and Implementation Details

### Architecture Decisions

1. **Project Structure**
   - Split into three projects (API, Core, Console) for separation of concerns
   - Core project contains core business logic and models, basically the CSV handling part.
   - API and Console projects are separate interfaces to the same functionality

2. **Database Choice**
   - SQLite was chosen for:
     - Simplicity in setup and deployment
     - No need for a separate database server
     - Portable database file that can be included with the application
     - Suitable for the scale of this application
   - Pre-populated database included for ease of testing

3. **CSV Processing**
   - Custom CSV parser implementation as per requirements
   - Handles edge cases like:
     - Quoted fields
     - Empty lines
     - Data validation (eg. scores must be between 0 and 100)
   - Provides clear error messages for malformed input

4. **API Design**
   - RESTful principles followed for intuitive endpoint design
   - Swagger UI included for:
     - Easy testing
     - Self-documenting API
     - Interactive documentation
   - Endpoints designed for specific use cases:
     - All scores retrieval
     - Top scorers retrieval
     - Individual score lookup
     - Bulk upload via CSV format
     - Single score addition

### Future Improvements

1. **Potential Improvements**
   - Add authentication and authorization, allowing access by submitting credentials along with a valid JWT in the header. (API)
   - Implement caching for frequently accessed data (API)
   - Add pagination if the dataset grew larger (API)
   - Unit Tests for the CSV parsing code, integrated with CI/CD
   - Developing a frontend application to link with the API

### **Hosting the API**

There are many options available for hosting ASP.NET API's. 
   - Azure App Services (built into Visual Studio)
   - Docker
   - Vercel
   - AWS Elastic Beanstalk


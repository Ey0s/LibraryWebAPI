# Library Management System

This repository contains the full-stack application for a simple Library Management System. It consists of an ASP.NET Core Web API backend and a React frontend.

## Team Members

- **Eyosyas yoseph 1501193** – Backend Developer  
- **Ayele yinesu 1501014** – Backend Developer  
- **Firezer settual 1501744** – Frontend Developer  
- **Lamrot girma 1501316** – Frontend Developer  

## Features

* **User Authentication:** Secure user registration and login using JWT (JSON Web Tokens).
* **Book Management:**
    * Add new books.
    * View all available books.
    * Update book details.
    * Delete books.
* **Borrower Management:** (If you have this)
    * Add new borrowers.
    * View all registered borrowers.
    * Update borrower details.
    * Delete borrowers.
* **Loan Management:** (If you have this)
    * Lend books to borrowers.
    * Return borrowed books.
    * View current loans.
* **Responsive UI:** A user-friendly interface built with React.

## Technologies Used

### Backend (LibraryWebAPI)

* **Framework:** ASP.NET Core
* **Language:** C#
* **Database:** SQL Server (using Entity Framework Core)
* **Authentication:** JWT Bearer Authentication
* **API Documentation:** Swagger

### Frontend (React App)

* **Framework:** React.js
* **State Management:** React Context API
* **Routing:** React Router DOM
* **HTTP Client:** Axios
## Getting Started

Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

Before you begin, ensure you have the following installed:

* **.NET SDK:** [Your .NET version, e.g., .NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/[your-dotnet-version])
* **Node.js & npm:** [Node.js (LTS version recommended)](https://nodejs.org/en/download/) (npm comes with Node.js)
* **SQL Server Instance:** A local SQL Server instance (e.g., SQL Server Express, SQL Server LocalDB)
    * If using LocalDB, it's usually installed with Visual Studio or SQL Server Express.
* **SQL Server Management Studio (SSMS) or Azure Data Studio:** (Recommended for database management)
    * [SSMS Download](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
    * [Azure Data Studio Download](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio)

### Backend Setup

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/your-username/your-repo-name.git](https://github.com/your-username/your-repo-name.git)
    cd your-repo-name
    ```
2.  **Navigate to the backend project:**
    ```bash
    cd LibraryWebAPI # Assuming your backend project is directly in LibraryWebAPI folder
    ```
3.  **Configure Database Connection:**
    * Open `appsettings.json` (and `appsettings.Development.json` if applicable).
    * Update the `ConnectionStrings:DefaultConnection` to match your SQL Server instance.
        ```json
        // appsettings.json
        "ConnectionStrings": {
          "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MyAwesomeLibraryDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
        },
        "Jwt": {
            "Key": "ThisIsAVeryStrongSecretKeyForJWTAuthenticationWhichShouldBeAtLeast32CharactersLongAndVerySecure",
            "Issuer": "LibraryWebAPI",
            "Audience": "LibraryAppUsers", // Ensure this matches your frontend configuration
            "ExpireHours": 1
        }
        ```
    * **Important:** Replace `MyAwesomeLibraryDb` with your desired database name if it's different.
    * Ensure your `Jwt:Key` is strong and secret (the one in the example is good).
    * Verify `Jwt:Issuer` and `Jwt:Audience` match your frontend's expected values.

4.  **Apply Migrations and Create Database:**
    ```bash
    dotnet ef database update
    ```
    This command will create the database (if it doesn't exist) and apply all pending migrations, setting up your tables.

5.  **Run the Backend:**
    * **Using VS Code:**
        * Open the `LibraryWebAPI` folder in VS Code.
        * Go to the "Run and Debug" view (Ctrl+Shift+D).
        * Select the desired launch profile (e.g., "http" for `http://localhost:5291` or "https" for `https://localhost:7003`).
        * Press `F5` to start debugging.
    * **From Terminal (HTTP):**
        ```bash
        dotnet run --launch-profile "http"
        ```
        The API will typically run on `http://localhost:5291`.
    * **From Terminal (HTTPS):**
        ```bash
        dotnet run --launch-profile "https"
        ```
        The API will typically run on `https://localhost:7003`.

    The Swagger UI will be available at `/swagger` (e.g., `http://localhost:5291/swagger` or `https://localhost:7003/swagger`).

### Frontend Setup

1.  **Navigate to the frontend project:**
    ```bash
    cd ../frontend # Assuming your frontend project is in a 'frontend' folder next to 'LibraryWebAPI'
    ```
2.  **Install dependencies:**
    ```bash
    npm install
    ```
3.  **Configure API Base URL:**
    * Open your API configuration file (e.g., `src/api.js` or `src/axiosInstance.js`).
    * Ensure `API_BASE_URL` matches the URL of your running backend.
        ```javascript
        // src/api.js
        const API_BASE_URL = 'http://localhost:5291/api'; // Or 'https://localhost:7003/api' if using HTTPS
        ```
4.  **Run the Frontend:**
    ```bash
    npm start
    ```
    The React development server will start, typically on `http://localhost:3000`, and your browser will open automatically.

## API Endpoints

The API documentation is available via Swagger UI when the backend is running.

* **Swagger UI:** `http://localhost:[Backend_HTTP_Port]/swagger` (e.g., `http://localhost:5291/swagger`)
* **Swagger UI (HTTPS):** `https://localhost:[Backend_HTTPS_Port]/swagger` (e.g., `https://localhost:7003/swagger`)

Common endpoints include:

* `POST /api/Auth/register` - User registration
* `POST /api/Auth/login` - User login (returns JWT)
* `GET /api/Books` - Get all books (requires authentication)
* `POST /api/Books` - Add a new book (requires authentication)
* `GET /api/Books/{id}` - Get book by ID (requires authentication)
* `PUT /api/Books/{id}` - Update book (requires authentication)
* `DELETE /api/Books/{id}` - Delete book (requires authentication)
* `DELETE /api/Admin/clear-users` - (If you added this for dev/admin) Deletes all users. **(Highly sensitive - use with caution, especially in production)**

## Database Schema

(You can briefly describe your main tables and their relationships here)

* **Users:** (Id, Username, PasswordHash)
* **Books:** (Id, Title, Author, ISBN, PublicationYear, IsAvailable)
* **Borrowers:** (Id, Name, ContactInfo) - *If applicable*
* **Loans:** (Id, BookId, BorrowerId, BorrowDate, ReturnDate) - *If applicable*

## Authentication & Authorization

This project uses JWT (JSON Web Tokens) for authenticating users and authorizing access to protected resources.

* **Registration:** Users can register with a unique username and password.
* **Login:** Upon successful login, a JWT is issued.
* **Token Usage:** This token must be included in the `Authorization` header of subsequent requests to protected API endpoints, in the format `Bearer YOUR_JWT_TOKEN`.
* **Middleware:** The ASP.NET Core backend uses `JwtBearerDefaults.AuthenticationScheme` and `[Authorize]` attributes to protect API endpoints.

## CORS Configuration

Cross-Origin Resource Sharing (CORS) is configured in the backend to allow requests from the React frontend running on `http://localhost:3000`. If your frontend runs on a different port or domain, you will need to update the `policy.WithOrigins()` setting in `Program.cs` accordingly.

## AI Use in Project Development

This project was built with help from AI tools in several ways:

- **Code Generation:** AI helped write common code parts like JWT setup, CRUD operations in ASP.NET Core, React components, and Axios calls. This saved time by providing ready-to-use code snippets.  
- **Debugging Help:** AI assisted in finding and fixing errors in both backend (C#, Entity Framework, JWT) and frontend (React, JavaScript) by analyzing error messages and suggesting fixes.  
- **Architecture & Best Practices:** AI gave advice on project structure, API design (REST), database setup, and security measures like token validation and password hashing.  
- **Documentation:** AI helped draft and improve the README file, including setup guides and explanations of key concepts like JWT and CORS.  
- **Learning Support:** AI explained complex ideas like token validation, middleware roles, and React context with clear examples and simple terms.  

Some of the AI tools used include **ChatGPT**, **GitHub Copilot**, and **Gemini**.

Using AI tools made development faster and smoother by providing guidance and automating repetitive tasks.

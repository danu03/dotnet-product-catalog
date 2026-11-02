# Product Catalog Manager (.NET 8 Web API & Razor Interface)

A lightweight ASP.NET Core 8.0 web application with SQLite database persistence and an integrated Razor Pages management interface.

## Assumptions
1. **Database Storage:** Uses an embedded SQLite file (`catalog.db`) created automatically on first run without requiring external database server configuration.
2. **Public Product Endpoints:** All product endpoints (`/api/products/*`) are open for CRUD operations without authentication requirements.

## Quick Start (3 Steps to Run Locally)

### Prerequisites
- .NET 8.0 SDK or higher (`dotnet --version`)

### Steps
1. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

2. **Execute automated unit tests:**
   ```bash
   dotnet test
   ```

3. **Start the web application:**
   ```bash
   dotnet run --project ProductCatalog.Web
   ```

Open your browser at the displayed URL (typically `http://localhost:5000` or `https://localhost:5001`).
Swagger OpenAPI documentation is accessible at `/swagger`.

## Features
- **Product Management:** Full CRUD operations (Create, Read, Update, Delete) on products with Name, Description, Price, and CreatedAt fields.
- **Search and Filtering:** Filter products by name query or price range via `/api/products/search`.
- **Validation:** Model validation using Data Annotations with error details.
- **Logging and Error Handling:** Structured console and daily rolling file logging using Serilog, plus centralized exception middleware.
- **User Interface:** Accessible Razor Pages dashboard supporting search, inline edit, delete confirmation, empty/loading states, responsive mobile reflow, and Light/Dark themes.

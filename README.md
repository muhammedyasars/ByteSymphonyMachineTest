ByteSymphonyTest  REST API

A ready-to-use REST API built with ASP.NET Core 8 and Entity Framework Core. Supports JWT authentication, product and order management with stock handling and concurrency control.

Tech Stack

ASP.NET Core 8

Entity Framework Core 8

SQLite (default) / SQL Server

JWT Authentication

BCrypt.Net for password hashing

FluentValidation

Swagger (Swashbuckle)

Quick Setup
1. Prerequisites

.NET 8 SDK or later

2. Clone the Project
git clone < >
cd ByteSymmetryTest

3. Restore Dependencies
dotnet restore

4. Apply Database Migrations
dotnet ef database update


This will:

Create the database (app.db for SQLite)

Create tables for Users, Products, and Orders

Seed 1 admin user and 3 sample products

5. Run the API
dotnet run


The API will start at:

HTTPS: https://localhost:5001

HTTP: http://localhost:5000

6. Access Swagger UI

Open your browser:

https://localhost:5001/swagger

Default Admin Credentials

Email: admin@gmail.com

Password: Yasar@2005!

Seeded products:

Laptop – 45999 (Stock: 50)

Mouse – 599 (Stock: 194)

Keyboard – 1499 (Stock: 150)

API Endpoints
Authentication
Method	Endpoint	Auth
POST	/api/auth/login	None
POST	/api/auth/register	Admin
Products
Method	Endpoint	Auth
GET	/api/products	User/Admin
GET	/api/products/{id}	User/Admin
POST	/api/products	Admin
PUT	/api/products/{id}	Admin
DELETE	/api/products/{id}	Admin

GET Query Parameters: page, pageSize, search, sort

Orders
Method	Endpoint	Auth
GET	/api/orders	User/Admin
GET	/api/orders/{id}	User/Admin
POST	/api/orders	User/Admin
DELETE	/api/orders/{id}	Admin
Example Workflow

Login
POST /api/auth/login with admin credentials → get JWT token

Authorize in Swagger
Click Authorize, enter:<your-token>

Get Products
GET /api/products?page=1&pageSize=10&search=laptop&sort=price

Create an Order
POST /api/orders with JSON:

{
  "productId": 1,
  "qty": 2
}

Switching to SQL Server

Edit Program.cs to use SQL Server.

Update appsettings.json with your SQL Server connection string.

Create migrations and update the database:

dotnet ef migrations add InitialCreate
dotnet ef database update

Security Notes

Change JWT secret in appsettings.json

Use environment variables for secrets

Change default admin password

Always use HTTPS in production

Keep packages updated

Project Structure
ByteSymmetryTest
. Controllers
. Data/
. DTOs/
. Models/
. Services/
. Properties/
. appsettings.json
. Program.cs
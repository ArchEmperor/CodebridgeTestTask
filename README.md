# 🐾 Dogs API

A simple RESTful API built with **.NET 9**, **Entity Framework Core**, and **PostgreSQL**.  
It supports pagination, sorting, validation, and request per second limitation.

---

## ⚙️ Setup

### 1. Clone the repository
```bash
git clone https://github.com/ArchEmperor/CodebridgeTestTask.git
cd dogs-api
```
### 2. Create the .env file
Copy the example and update your settings:
```bash
cp .env.example .env
```

### 3. Apply database migration
```bash
dotnet ef database update
```
### 4. Run the project
```bash
dotnet run
```
The API will start at:
http://localhost:5000

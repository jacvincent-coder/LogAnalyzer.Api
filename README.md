# 📘 Log Analyzer – Full Stack Application
Analyze log files to extract useful insights such as unique IP count, top URLs, and top IP addresses.
Built using **.NET 8 Web API** + **Angular 17 standalone components**.

---

## 🚀 Features

### ✅ Backend (.NET API)
- Upload log file (`multipart/form-data`)
- Parse logs and produce analytics:
  - Unique IP count
  - Top requested URLs
  - Top IP addresses
- Global exception handling middleware
- API Key authentication
- JWT authentication (optional / can be enabled later)
- CORS configured for Angular UI
- Swagger API documentation

### ✅ Frontend (Angular)
- Standalone component architecture
- File upload UI
- Loading spinner
- Error banner with auto-hide
- Reset button
- Results displayed in a table
- API key automatically added to requests
- Supports HTTPS backend

---

# 📁 Project Structure

```
LogAnalyzer.Api/           → ASP.NET Core Web API backend
log-analyzer-ui/           → Angular frontend
```

---

# 🛠️ Backend Setup (.NET 8)

### 📌 Prerequisites
- .NET 8 SDK
- Visual Studio / VS Code / JetBrains Rider

---

## ▶️ 1. Restore packages
```sh
cd LogAnalyzer.Api
dotnet restore
```

---

## ▶️ 2. Run the API
```sh
dotnet run
```

API will run on:

- https://localhost:7219  
- Swagger → https://localhost:7219/swagger  

---

## ▶️ 3. API Key Authentication
Include this header in all requests:

```
X-API-Key: 9b955bef42a8ecd1c3530863ad0a40922edc2afe0c444215b6968f29af13da5d
```

---

## ▶️ 4. Test Using Swagger
1. Run the API  
2. Open browser → `https://localhost:7219/swagger`  
3. Upload a log file via **POST /api/Log/analyze**

---

# 🖥️ Frontend Setup (Angular 17)

### 📌 Prerequisites
- Node.js (v18+)
- Angular CLI  
```sh
npm install -g @angular/cli
```

---

## ▶️ 1. Install dependencies
```sh
cd log-analyzer-ui
npm install
```

---

## ▶️ 2. Run the Angular app
```sh
ng serve
```

Runs at: **http://localhost:4200**

---

# 🔄 End-to-End Workflow
1. User uploads `.log` file  
2. Angular sends file → .NET API  
3. API parses/returns analytics  
4. UI renders:
   - Unique IP count  
   - Top URLs  
   - Top IP addresses  

---

# 🧪 Testing

### Backend
```sh
dotnet test
```


---

# 🌱 Future Improvements
- JWT login page + role-based auth
- Angular Material redesign
- Pagination / charts
- Docker deployment
- CI/CD pipelines

---


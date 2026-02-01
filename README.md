# FinTrack - Sistema de Finanzas Personales con IA Predictiva

## Descripción del Proyecto
Sistema web para gestión de finanzas personales que permite registrar ingresos/gastos, visualizar reportes y recibir predicciones inteligentes mediante ML.NET. Desarrollado como proyecto académico con arquitectura en capas.

## Objetivo
Desarrollar un sistema web que permita a los usuarios registrar, analizar y visualizar sus ingresos y gastos, incorporando un módulo de inteligencia artificial que genere predicciones y recomendaciones personalizadas para apoyar la toma de decisiones financieras.

## Arquitectura del Proyecto
El sistema sigue una arquitectura en capas con separación clara de responsabilidades:

### **Capas:**
1. **FinTrack.Core** - Lógica de negocio, entidades, interfaces, DTOs y validaciones
2. **FinTrack.Infrastructure** - Acceso a datos, repositorios, Entity Framework, ML.NET
3. **FinTrack.Web** - Presentación (MVC), controladores, vistas, ViewModels

### **Dependencias:**
FinTrack.Web (Presentation Layer)
↓ Depende de:
FinTrack.Core (Business Logic)
FinTrack.Infrastructure (Data Access)
↓ Depende de:
FinTrack.Core

## Estructura del Repositorio
```text
├── FinTrack.Core/            # Lógica de negocio
│   ├── Entities/             # Entidades del dominio (User, Expense, Income, etc.)
│   ├── Interfaces/           # Contratos/Interfaces (Repository, Services)
│   ├── Services/             # Interfaces de servicios de negocio
│   ├── DTOs/                 # Objetos de transferencia de datos
│   └── Validators/           # Reglas de validación con FluentValidation
├── FinTrack.Infrastructure/  # Acceso a datos e implementaciones
│   ├── Data/                 # Contexto EF y configuraciones
│   │   ├── Configurations/   # Configuraciones de entidades
│   │   └── Migrations/       # Migraciones de base de datos
│   ├── Repositories/         # Implementación de repositorios
│   ├── Identity/             # Configuración de ASP.NET Identity
│   ├── ML/                   # Modelos y servicios de ML.NET
│   └── Extensions/           # Extensiones de servicios
├── FinTrack.Web/             # Capa de presentación
│   ├── Controllers/          # Controladores MVC
│   ├── Views/                # Vistas Razor
│   ├── ViewModels/           # Modelos para vistas
│   ├── wwwroot/              # Archivos estáticos (CSS, JS, imágenes)
│   └── Properties/           # Configuración de lanzamiento
├── .gitignore
└── README.md
```

## Tecnologías Utilizadas

### **Backend:**
- **Framework:** ASP.NET Core 8.0
- **Lenguaje:** C# 12.0
- **ORM:** Entity Framework Core 8.0
- **Autenticación:** ASP.NET Core Identity
- **Machine Learning:** ML.NET 3.0
- **Validación:** FluentValidation 11.8
- **Base de datos:** SQL Server 2022 / SQL Express

### **Frontend:**
- **UI Framework:** Bootstrap 5.3
- **Gráficos:** Chart.js 4.4
- **JavaScript:** ES6+
- **Estilos:** CSS3 con variables personalizadas

### **Herramientas de Desarrollo:**
- **IDE:** Visual Studio 2022
- **Control de versiones:** Git + GitHub
- **Gestión de BD:** SQL Server Management Studio 19

## Configuración del Entorno de Desarrollo

### **Requisitos Previos:**
1. [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. [Visual Studio 2022](https://visualstudio.microsoft.com/) (v17.8+)
3. [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads) o SQL Express
4. [Git](https://git-scm.com/)

### **Pasos de Instalación:**
```bash
# 1. Clonar el repositorio
git clone https://github.com/TU_USUARIO/FinTrack.git
cd FinTrack

# 2. Restaurar paquetes NuGet
dotnet restore

# 3. Configurar base de datos
# Copiar appsettings.Template.json a appsettings.json y configurar conexión
cp FinTrack.Web/appsettings.Template.json FinTrack.Web/appsettings.json

# 4. Aplicar migraciones
cd FinTrack.Infrastructure
dotnet ef database update --startup-project ../FinTrack.Web

# 5. Ejecutar aplicación
cd ../FinTrack.Web
dotnet run



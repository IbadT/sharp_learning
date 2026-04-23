http://localhost:5112/scalar

<!-- Создание проекта -->

dotnet new webapi -n MyTextApi

<!-- Установка swagger -->

dotnet add package Swashbuckle.AspNetCore

<!-- Добавить Postgres -->

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design

<!-- Добавить Redis -->

dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis

<!-- Добавить Jwt -->

dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

<!-- Добавить Bcrypt -->

dotnet add package BCrypt.Net-Next

<!-- Создание миграции -->

dotnet ef migrations add InitialCreate

<!-- Создание новой миграции -->

dotnet ef migrations add AddTodosTable

<!-- Применение миграций первый раз -->

dotnet ef database update

dotnet run

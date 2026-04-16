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

<!-- Создание миграции -->

dotnet ef migrations add InitialCreate

<!-- Применение миграций первый раз -->

dotnet ef database update

dotnet run

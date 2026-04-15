<!-- Создание проекта -->

dotnet new webapi -n MyTextApi

<!-- Установка swagger -->

dotnet add package Swashbuckle.AspNetCore

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet run

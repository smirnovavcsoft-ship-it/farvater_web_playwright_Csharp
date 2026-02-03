# 1. Используем официальный образ Microsoft с .NET SDK и Playwright
# Этот образ уже содержит все библиотеки, нужные для запуска Chromium/Firefox
FROM mcr.microsoft.com/playwright/dotnet:v1.49.0-noble

# 2. Указываем рабочую папку внутри контейнера
WORKDIR /app

# 3. Копируем файл проекта (.csproj) и скачиваем зависимости (NuGet пакеты)
# Делаем это отдельным шагом, чтобы Docker закешировал пакеты
COPY *.csproj ./
RUN dotnet restore

# 4. Копируем всё остальное (весь твой код тестов)
COPY . ./

# 5. Собираем проект в режиме Release
RUN dotnet build -c Release

# 6. Устанавливаем браузеры Playwright
# Мы запускаем скрипт, который появился после сборки проекта
RUN pwsh bin/Release/net8.0/playwright.ps1 install

# 7. Команда, которая запустится при старте контейнера
# Добавляем --logger:trx, чтобы потом можно было вытащить файл отчета
ENTRYPOINT ["dotnet", "test", "-c", "Release", "--logger:trx"]
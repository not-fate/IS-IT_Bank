1. Устанавливаем .net core 8 https://dotnet.microsoft.com/ru-ru/download/dotnet/8.0
2. Сайт использует SQL Server. Используется облегченная редакция SQL SERVER EXPRESS LocalDB (https://go.microsoft.com/fwlink/?linkid=2215160).
После скачивания SQL SERVER EXPRESS LocalDB и установки в варианта LocalDB, стартуем его комаднами в cmd:
SqlLocalDB create MSSQLLocalDB
SqlLocalDB start MSSQLLocalDB
Если на хосте уже установлен полноценный SQL Server, то можно использовать его. Для этого надо в
appsettings.json изменить строку подключения DBHopeBank.
3. Запускаем HopeBank.exe в подкаталоге HopeBank\bin\Release\net8.0\publish.
4. При первом запуске создастся база данных HopeBank.
5. В консоли появится адрес, на котором стартанул сайт (http://localhost:5000).

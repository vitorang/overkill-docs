Write-Host "Compilando Projetos" -ForegroundColor Cyan
dotnet build OverkillDocs.Migrator
dotnet build OverkillDocs.Migrator.SqlServer
dotnet build OverkillDocs.Migrator.Sqlite

$timestamp = (Get-Date).ToString("yyyyMMdd_HHmm")
$NameSqlite = "Sqlite_$timestamp"
$NameSqlServer = "SqlServer_$timestamp"

Write-Host "Gerando Migracao SQL Server: $NameSqlServer" -ForegroundColor Cyan
$env:EF_PROVIDER="sqlserver"

dotnet ef migrations add $NameSqlServer `
    --project OverkillDocs.Migrator.SqlServer `
    --startup-project OverkillDocs.Migrator


Write-Host "Gerando Migracao Sqlite: $NameSqlite" -ForegroundColor Cyan
$env:EF_PROVIDER="sqlite"

dotnet ef migrations add $NameSqlite `
    --project OverkillDocs.Migrator.Sqlite `
    --startup-project OverkillDocs.Migrator

$env:EF_PROVIDER=$null

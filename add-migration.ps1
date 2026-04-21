Write-Host "Compilando Projetos" -ForegroundColor Cyan
dotnet build OverkillDocs.Migrator
dotnet build OverkillDocs.Migrator.SqlServer
dotnet build OverkillDocs.Migrator.Sqlite
dotnet build OverkillDocs.Migrator.Postgres

$timestamp = (Get-Date).ToString("yyyyMMdd_HHmm")
$NameSqlite = "Sqlite_$timestamp"
$NameSqlServer = "SqlServer_$timestamp"
$NamePostgres = "Postgres_$timestamp"

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


Write-Host "Gerando Migracao Postgres: $NamePostgres" -ForegroundColor Cyan
$env:EF_PROVIDER="postgres"

dotnet ef migrations add $NamePostgres `
    --project OverkillDocs.Migrator.Postgres `
    --startup-project OverkillDocs.Migrator


$env:EF_PROVIDER=$null

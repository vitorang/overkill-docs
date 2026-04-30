Write-Host "1 - Compose"
Write-Host "2 - Compose E2E"
Write-Host "3 - Dockerfile.prod"

$option = Read-Host "`nIniciar"

switch ($option) {
    "1" {
        docker compose up -d
    }
    "2" {
        docker compose -f docker-compose.e2e.yml --env-file .env.e2e up -d --build
    }
    "3" {
        docker build -f Dockerfile.prod -t okd-prod .
        docker run -d --rm --env-file .env.prod -p 8080:8080 --name okd-prod okd-prod
    }
    Default {
        
    }
}
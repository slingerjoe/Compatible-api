# PowerShell script to deploy to AWS Elastic Beanstalk

Write-Host "Building and deploying CompatibleAPI to AWS Elastic Beanstalk..." -ForegroundColor Green

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean

# Restore packages
Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore

# Build the application
Write-Host "Building application..." -ForegroundColor Yellow
dotnet build --configuration Release

# Publish the application
Write-Host "Publishing application..." -ForegroundColor Yellow
dotnet publish --configuration Release --output ./publish

# Create deployment package
Write-Host "Creating deployment package..." -ForegroundColor Yellow
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$packageName = "compatible-api-$timestamp.zip"

# Navigate to publish directory and create zip
Set-Location ./publish
Compress-Archive -Path * -DestinationPath "../$packageName" -Force
Set-Location ..

Write-Host "Deployment package created: $packageName" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Upload $packageName to AWS Elastic Beanstalk" -ForegroundColor White
Write-Host "2. Configure environment variables in Elastic Beanstalk console:" -ForegroundColor White
Write-Host "   - DB_HOST: Your PostgreSQL RDS endpoint" -ForegroundColor White
Write-Host "   - DB_NAME: Your database name" -ForegroundColor White
Write-Host "   - DB_USER: Your database username" -ForegroundColor White
Write-Host "   - DB_PASSWORD: Your database password" -ForegroundColor White
Write-Host "3. Deploy the application" -ForegroundColor White 
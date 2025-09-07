# AWS Elastic Beanstalk Deployment Guide

This guide will help you deploy your CompatibleAPI to AWS Elastic Beanstalk.

## Prerequisites

1. **AWS Account**: You need an active AWS account
2. **AWS CLI**: Install and configure AWS CLI
3. **PostgreSQL Database**: Set up an RDS PostgreSQL instance
4. **IAM Permissions**: Ensure you have permissions for Elastic Beanstalk, RDS, and related services

## Step 1: Set up PostgreSQL RDS Database

1. Go to AWS RDS Console
2. Create a new PostgreSQL database instance
3. Note down the following details:
   - Endpoint (DB_HOST)
   - Database name (DB_NAME)
   - Username (DB_USER)
   - Password (DB_PASSWORD)
   - Port (usually 5432)

## Step 2: Create Elastic Beanstalk Application

1. Go to AWS Elastic Beanstalk Console
2. Click "Create Application"
3. Enter application name: `compatible-api`
4. Choose platform: `.NET Core on Linux`
5. Choose platform branch: `.NET 8 running on 64bit Amazon Linux 2`
6. Choose platform version: Latest
7. Click "Configure more options"

## Step 3: Configure Environment

### Software Configuration

- Set environment variables:
  - `DB_HOST`: Your RDS endpoint
  - `DB_NAME`: Your database name
  - `DB_USER`: Your database username
  - `DB_PASSWORD`: Your database password
  - `ASPNETCORE_ENVIRONMENT`: Production

### Instances Configuration

- Instance type: t3.small (or your preferred size)
- Instance profile: Create new or use existing

### Security Configuration

- EC2 key pair: Create new or use existing
- IAM instance profile: Create new or use existing

## Step 4: Build and Deploy

### Option 1: Using the Deployment Script

1. Open PowerShell in the CompatibleAPI directory
2. Run the deployment script:
   ```powershell
   .\deploy.ps1
   ```
3. Upload the generated zip file to Elastic Beanstalk

### Option 2: Manual Deployment

1. Build the application:

   ```bash
   dotnet clean
   dotnet restore
   dotnet build --configuration Release
   dotnet publish --configuration Release --output ./publish
   ```

2. Create a zip file containing the published files

3. Upload to Elastic Beanstalk

## Step 5: Configure Security Groups

1. Ensure your RDS security group allows connections from your Elastic Beanstalk security group
2. Configure Elastic Beanstalk security group to allow inbound traffic on port 80

## Step 6: Update CORS Settings

After deployment, update the CORS settings in your application to include your frontend domain:

```csharp
options.AddPolicy("AllowReactApp",
    builder => builder
        .WithOrigins("http://localhost:8081", "http://localhost:19006", "exp://localhost:19000", "https://your-frontend-domain.com")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
```

## Step 7: Test the Deployment

1. Check the health endpoint: `https://your-eb-url/health`
2. Test your API endpoints
3. Monitor logs in the Elastic Beanstalk console

## Troubleshooting

### Common Issues

1. **Database Connection**: Ensure RDS security groups allow connections from EB
2. **Environment Variables**: Verify all environment variables are set correctly
3. **Port Configuration**: Ensure the application listens on port 5000
4. **Health Check**: Check that the `/health` endpoint returns 200 OK

### Logs

- View application logs in Elastic Beanstalk console
- Check `/var/log/nginx/` for nginx logs
- Check `/var/log/dotnet/` for application logs

## Environment Variables Reference

| Variable               | Description             | Example                                  |
| ---------------------- | ----------------------- | ---------------------------------------- |
| DB_HOST                | PostgreSQL RDS endpoint | `compatible-db.region.rds.amazonaws.com` |
| DB_NAME                | Database name           | `compa`                                  |
| DB_USER                | Database username       | `postgres`                               |
| DB_PASSWORD            | Database password       | `your-secure-password`                   |
| ASPNETCORE_ENVIRONMENT | Environment name        | `Production`                             |

## Cost Optimization

- Use t3.small instances for development/testing
- Consider using t3.micro for very light workloads
- Set up auto-scaling rules based on your needs
- Use RDS with appropriate instance size

## Security Best Practices

1. Use strong passwords for database
2. Enable encryption at rest for RDS
3. Use VPC for network isolation
4. Regularly update security groups
5. Monitor access logs
6. Use HTTPS in production (configure SSL certificate)

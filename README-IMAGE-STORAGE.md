# Image Storage Integration

This document describes the image storage integration using AWS S3 and CloudFront for the Compatible API.

## Overview

The image storage system uses:

- **AWS S3**: For secure file storage
- **CloudFront**: As a CDN for fast, global image delivery
- **PostgreSQL**: To store image metadata and relationships

## Configuration

### AWS Configuration

1. **AWS Credentials**: Ensure your AWS credentials are configured via:

   - AWS CLI: `aws configure`
   - Environment variables: `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`
   - IAM roles (for production)

2. **S3 Bucket**: The system uses the bucket `compatible-images-dev`

   - Images are stored in the root of the bucket
   - Public read access is enabled for CloudFront delivery

3. **CloudFront Distribution**:
   - Base URL: `https://d1us4yuvo02gbo.cloudfront.net/`
   - Serves as the public CDN layer
   - S3 bucket is private and only accessible via CloudFront

### Application Configuration

The following settings are configured in `appsettings.json`:

```json
{
  "AWS": {
    "S3": {
      "BucketName": "compatible-images-dev"
    },
    "CloudFront": {
      "Url": "https://d1us4yuvo02gbo.cloudfront.net/"
    }
  }
}
```

## API Endpoints

### Image Upload

```
POST /api/images/upload/{profileId}
Content-Type: multipart/form-data

Parameters:
- profileId: Guid (required)
- file: Image file (required)
- isMain: boolean (optional, default: false)
```

### Get Profile Photos

```
GET /api/images/profile/{profileId}
```

### Get Specific Photo

```
GET /api/images/{photoId}
```

### Set Main Photo

```
PUT /api/images/{photoId}/set-main
```

### Delete Photo

```
DELETE /api/images/{photoId}
```

### Get Upload Limits

```
GET /api/images/limits
```

## File Validation

The system validates uploaded images:

- **Supported formats**: JPG, JPEG, PNG, GIF, WebP
- **Maximum file size**: 10MB
- **Content type validation**: Ensures proper MIME types

## File Naming Convention

Images are stored with the following naming pattern:

```
{profileId}_{guid}.{extension}
```

Example: `12345678-1234-1234-1234-123456789abc_98765432-4321-4321-4321-987654321cba.jpg`

## Database Schema

The `Photo` entity includes:

- `Id`: Unique identifier
- `Url`: CloudFront URL of the image
- `ProfileId`: Reference to the profile
- `IsMain`: Boolean indicating if this is the main photo
- `CreatedAt`: Timestamp of creation
- `UpdatedAt`: Timestamp of last update

## Security Considerations

1. **S3 Bucket Security**: The S3 bucket is private and only accessible via CloudFront
2. **File Validation**: All uploaded files are validated for type and size
3. **Access Control**: Images are associated with specific profiles
4. **Cleanup**: Deleting photos removes both the database record and S3 file

## Error Handling

The system handles various error scenarios:

- Invalid file types or sizes
- S3 upload failures
- Database operation failures
- Missing profiles or photos

## Testing

Use the provided HTTP test file (`CompatibleAPI.http`) to test the image endpoints:

1. **Test upload limits**: `GET /api/images/limits`
2. **Upload an image**: `POST /api/images/upload/{profileId}`
3. **Get profile photos**: `GET /api/images/profile/{profileId}`

## Frontend Integration

The frontend can integrate with these endpoints to:

- Upload profile photos
- Display images using CloudFront URLs
- Manage photo collections
- Set main profile photos

## Deployment Notes

1. **Environment Variables**: Set AWS credentials in production
2. **CORS**: Ensure CloudFront allows requests from your frontend domain
3. **Monitoring**: Monitor S3 costs and CloudFront usage
4. **Backup**: Consider backing up image metadata in the database

## Troubleshooting

### Common Issues

1. **Upload Failures**: Check AWS credentials and S3 permissions
2. **Image Not Loading**: Verify CloudFront distribution is active
3. **Database Errors**: Ensure Photo entity is properly migrated
4. **CORS Issues**: Check CloudFront CORS configuration

### Logs

The system logs important events:

- Successful uploads with file names and profile IDs
- Deletion operations
- Error conditions with details

Check application logs for troubleshooting information.

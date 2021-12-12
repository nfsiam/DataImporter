using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using DataImporter.Common.Models;

namespace DataImporter.Common.Utilities.Aws
{
    public class S3Utility : IS3Utility
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3BucketConfiguration _s3BucketConfig;

        public S3Utility(IAmazonS3 s3Client, S3BucketConfiguration s3BucketConfig)
        {
            _s3Client = s3Client;
            _s3BucketConfig = s3BucketConfig;
        }

        public async Task CreateBucketAsync()
        {
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _s3BucketConfig.BucketName);
            if (bucketExists) return;
            var bucketRequest = new PutBucketRequest
            {
                BucketName = _s3BucketConfig.BucketName,
                UseClientRegion = true
            };
            var response = await _s3Client.PutBucketAsync(bucketRequest);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new InvalidOperationException("Bucket Creation Failed");
        }

        public async Task UploadFileAsync(MemoryStream memoryStream, string key)
        {
            await CreateBucketAsync();
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = memoryStream,
                Key = key,
                BucketName = _s3BucketConfig.BucketName,
                CannedACL = S3CannedACL.PublicRead
            };
            var transferUtility = new TransferUtility(_s3Client);

            await transferUtility.UploadAsync(uploadRequest);
        }

        public async Task<MemoryStream> GetFileStreamAsync(string key)
        {
            var objectRequest = new GetObjectRequest
            {
                BucketName = _s3BucketConfig.BucketName,
                Key = key
            };
            var transferUtility = new TransferUtility(_s3Client);
            var objectResponse = await transferUtility.S3Client.GetObjectAsync(objectRequest);

            if (objectResponse.ResponseStream == null)
                throw new FileNotFoundException(
                    $"File not found in {_s3BucketConfig.BucketName} bucket with key: {key}");

            var memoryStream = new MemoryStream();

            using (Stream responseStream = objectResponse.ResponseStream)
            {
                responseStream.CopyTo(memoryStream);
            }

            return memoryStream;
        }

        public async Task DeleteFileAsync(string key)
        {
            var objectRequest = new DeleteObjectRequest
            {
                BucketName = _s3BucketConfig.BucketName,
                Key = key
            };
            var transferUtility = new TransferUtility(_s3Client);
            var objectResponse = await transferUtility.S3Client.DeleteObjectAsync(objectRequest);
            if (objectResponse.HttpStatusCode != System.Net.HttpStatusCode.NoContent)
                throw new InvalidOperationException("Bucket File delete failed");
        }
    }
}
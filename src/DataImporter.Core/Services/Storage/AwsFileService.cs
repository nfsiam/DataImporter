using System;
using System.IO;
using System.Threading.Tasks;
using DataImporter.Common.Models;
using DataImporter.Common.Utilities.Aws;
using DataImporter.Core.BusinessObjects;
using DataImporter.Core.Enums;

namespace DataImporter.Core.Services.Storage
{
    public class AwsFileService : IFileService
    {
        private readonly IS3Utility _s3Utility;
        private readonly StorageConfiguration _storageConfiguration;

        public AwsFileService(IS3Utility s3Utility, StorageConfiguration storageConfiguration)
        {
            _s3Utility = s3Utility;
            _storageConfiguration = storageConfiguration;
        }

        public async Task<FileBO> UploadAsync(FileBO file, FileOperationType operationType)
        {
            if (file is null)
                throw new InvalidOperationException("No file was provided");

            await _s3Utility.UploadFileAsync(file.Stream, GetKey(file.StorageFileName, operationType));

            await file.Stream.DisposeAsync();

            return file;
        }

        public async Task RemoveAsync(string storageFileName, FileOperationType operationType)
        {
            await _s3Utility.DeleteFileAsync(GetKey(storageFileName, operationType));
        }

        public async Task<MemoryStream> GetFileAsync(string storageFileName, FileOperationType operationType)
        {
            var subDir = operationType == FileOperationType.Import ? "Imports" : "Exports";
            var stream = await _s3Utility.GetFileStreamAsync(GetKey(storageFileName, operationType));
            return stream;
        }

        private string GetKey(string storageFileName, FileOperationType operationType)
        {
            var prefix = operationType == FileOperationType.Import
                ? _storageConfiguration.AwsImport
                : _storageConfiguration.AwsExport;

            return $"{prefix}/{storageFileName}"
                .TrimEnd('/')
                .TrimStart('/')
                .Trim();
        }
    }
}
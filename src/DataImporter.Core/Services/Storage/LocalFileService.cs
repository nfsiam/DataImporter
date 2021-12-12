using System;
using System.IO;
using System.Threading.Tasks;
using DataImporter.Common.Models;
using DataImporter.Core.BusinessObjects;
using DataImporter.Core.Enums;

namespace DataImporter.Core.Services.Storage
{
    public class LocalFileService : IFileService
    {
        private readonly StorageConfiguration _storageConfiguration;

        public LocalFileService(StorageConfiguration storageConfiguration)
        {
            _storageConfiguration = storageConfiguration;
        }

        public async Task<FileBO> UploadAsync(FileBO file, FileOperationType operationType)
        {
            if (file is null)
                throw new InvalidOperationException("No file was provided");
            
            var fileInfo = new FileInfo(GetFilePath(file.StorageFileName,operationType));

            fileInfo.Directory.Create(); // If the directory already exists, this method does nothing.

            await using var fileStream = new FileStream(fileInfo.FullName, FileMode.Create);

            file.Stream.WriteTo(fileStream);

            file.Stream.Dispose();

            return file;
        }

        public async Task RemoveAsync(string storageFileName, FileOperationType operationType)
        {
            var filePath = GetFilePath(storageFileName, operationType);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task<MemoryStream> GetFileAsync(string storageFileName, FileOperationType operationType)
        {
            var filePath = GetFilePath(storageFileName, operationType);

            if (File.Exists(filePath))
            {
                var stream = new MemoryStream();
                await using var fileStream = File.OpenRead(filePath);
                fileStream.Position = 0;
                await fileStream.CopyToAsync(stream);
                return stream;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        private string GetFilePath(string storageFileName, FileOperationType operationType)
        {
            var subDirectory = operationType == FileOperationType.Import
                ? _storageConfiguration.LocalImport
                : _storageConfiguration.LocalExport;

            return Path.Combine(_storageConfiguration.LocalStorage, subDirectory, storageFileName);
        }
    }
}
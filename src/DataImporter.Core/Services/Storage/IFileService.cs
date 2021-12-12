using System.IO;
using System.Threading.Tasks;
using DataImporter.Common.Models;
using DataImporter.Core.BusinessObjects;
using DataImporter.Core.Enums;

namespace DataImporter.Core.Services.Storage
{
    public interface IFileService
    {
        Task<FileBO> UploadAsync(FileBO file, FileOperationType operationType);
        Task RemoveAsync(string storageFileName, FileOperationType operationType);
        Task<MemoryStream> GetFileAsync(string storageFileName, FileOperationType operationType);
    }
}
using System.IO;
using System.Threading.Tasks;

namespace DataImporter.Common.Utilities.Aws
{
    public interface IS3Utility
    {
        Task CreateBucketAsync();
        Task UploadFileAsync(MemoryStream memoryStream, string key);
        Task<MemoryStream> GetFileStreamAsync(string key);
        Task DeleteFileAsync(string key);
    }
}
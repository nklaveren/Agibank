using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Agibank.Domain.Interfaces
{
    public interface IFileService
    {
        IEnumerable<string> GetAllFiles(string path, string extension);
        string GetFileName(string path);
        Stream GetFileContent(string path);
        Task WriteFile(string file, string path, string outputFilename);
        void Delete(string file);
    }
}

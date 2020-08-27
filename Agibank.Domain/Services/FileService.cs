using Agibank.Domain.Interfaces;

using Microsoft.Extensions.FileProviders;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Agibank.Domain.Services
{
    public class FileService : IFileService
    {
        private readonly IFileProvider provider;

        public FileService(IFileProvider provider)
        {
            this.provider = provider;
        }
        public IEnumerable<string> GetAllFiles(string path, string extension)
        {
            var files = provider.GetDirectoryContents(path).Where(x => x.Name.EndsWith(extension));

            foreach (var item in files)
            {
                yield return item.Name;
            }
        }

        public Stream GetFileContent(string path)
        {
            if (File.Exists(path))
            {
                return provider.GetFileInfo(path).CreateReadStream();
            }
            else return default;
        }

        public Task WriteFile(string fileContent, string path, string outputFilename)
        {
            return WriteFile(new[] { fileContent }, path, outputFilename);
        }

        public async Task WriteFile(string[] fileContent, string path, string outputFilename)
        {
            var file = Path.Combine(path, outputFilename);
            Delete(file);
            await File.WriteAllLinesAsync(file, fileContent);
        }

        public void Delete(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}

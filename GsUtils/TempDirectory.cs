using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class TempDirectory : IDisposable
    {
        public IEnumerable<string> Files { get => Directory.EnumerateFiles(Path); }
        public string Path { get; }

        public TempDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) 
                throw new DirectoryNotFoundException($"Directory not found at {directoryPath}");
            Path = directoryPath;
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(Path, true);
            }
            catch { }
        }
    }
}

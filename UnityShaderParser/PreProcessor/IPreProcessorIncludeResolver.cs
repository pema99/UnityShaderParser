using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityShaderParser.PreProcessor
{
    public interface IPreProcessorIncludeResolver
    {
        public string ReadFile(string basePath, string filePath);
    }

    internal sealed class DefaultPreProcessorIncludeResolver : IPreProcessorIncludeResolver
    {
        public string ReadFile(string basePath, string filePath)
        {
            string path = string.IsNullOrEmpty(basePath)
                ? filePath
                : Path.Combine(basePath, filePath);

            if (!File.Exists(path))
            {
                // Attempt current directory instead
                string lastFolder = Path.GetFileName(basePath);
                if (!string.IsNullOrEmpty(lastFolder) && filePath.StartsWith(lastFolder))
                {
                    path = Path.Combine($"{basePath}/..", filePath);
                }
            }

            return File.ReadAllText(path);
        }
    }
}

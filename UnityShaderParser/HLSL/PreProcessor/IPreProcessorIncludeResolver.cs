using System.Collections.Generic;
using System.IO;

namespace UnityShaderParser.HLSL.PreProcessor
{
    public interface IPreProcessorIncludeResolver
    {
        string ReadFile(string basePath, string filePath);
    }

    public sealed class DefaultPreProcessorIncludeResolver : IPreProcessorIncludeResolver
    {
        private List<string> includePaths = new List<string>();

        public DefaultPreProcessorIncludeResolver() { }

        public DefaultPreProcessorIncludeResolver(List<string> includePaths)
        {
            foreach (var includePath in includePaths)
            {
                if (!Directory.Exists(includePath))
                {
                    this.includePaths.Add(Path.GetFullPath(includePath));
                }
                else
                {
                    this.includePaths.Add(includePath);
                }
            }
        }

        public string ReadFile(string basePath, string filePath)
        {
            // Fix windows-specific include paths
            filePath = filePath.Replace("\\", "/");

            string path = string.IsNullOrEmpty(basePath)
                ? filePath
                : Path.Combine(basePath, filePath);

            // Try include paths instead
            if (!File.Exists(path))
            {
                foreach (string includePath in includePaths)
                {
                    string combinedPath = Path.Combine(includePath, filePath);
                    if (File.Exists(combinedPath))
                        return File.ReadAllText(combinedPath);
                    combinedPath = Path.Combine(basePath, includePath, filePath);
                    if (File.Exists(combinedPath))
                        return File.ReadAllText(combinedPath);
                }
            }

            // Still not found, so try current directory instead
            if (!File.Exists(path))
            {
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

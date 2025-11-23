using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Interfaces;

namespace DesignPatternMaster.Infrastructure.Repositories
{
    public class JsonPatternRepository : IPatternRepository
    {
    private string _filePath;
    private List<DesignPattern>? _cachedPatterns;

        public JsonPatternRepository(string filePath = "Data/patterns.json")
        {
            _filePath = filePath;
        }

        // Sanitize JSON string by escaping CR/LF/TAB inside string literals.
        private static string SanitizeJsonString(string json)
        {
            var sb = new System.Text.StringBuilder(json.Length);
            bool inString = false;
            bool escape = false;

            foreach (char ch in json)
            {
                if (escape)
                {
                    sb.Append(ch);
                    escape = false;
                    continue;
                }

                if (ch == '\\')
                {
                    sb.Append(ch);
                    escape = true;
                    continue;
                }

                if (ch == '"')
                {
                    sb.Append(ch);
                    inString = !inString;
                    continue;
                }

                if (inString)
                {
                    if (ch == '\r')
                    {
                        sb.Append("\\r");
                        continue;
                    }
                    if (ch == '\n')
                    {
                        sb.Append("\\n");
                        continue;
                    }
                    if (ch == '\t')
                    {
                        sb.Append("\\t");
                        continue;
                    }
                }

                sb.Append(ch);
            }

            return sb.ToString();
        }

        private async Task EnsureLoadedAsync()
        {
            if (_cachedPatterns != null) return;

            if (!File.Exists(_filePath))
            {
                // Fallback for development/testing if file not found in relative path
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var absolutePath = Path.Combine(baseDir, _filePath);
                
                if (!File.Exists(absolutePath))
                {
                    // Try looking in the project structure if running from source
                    // This is a bit hacky but helps in dev environment
                    var baseDirParent = Directory.GetParent(baseDir);
                    var projectPath = string.Empty;
                    if (baseDirParent != null && baseDirParent.Parent != null && baseDirParent.Parent.Parent != null)
                    {
                        projectPath = Path.Combine(baseDirParent.Parent.Parent.FullName, "DesignPatternMaster.Infrastructure", _filePath);
                    }
                    if (!string.IsNullOrEmpty(projectPath) && File.Exists(projectPath))
                    {
                        absolutePath = projectPath;
                    }
                    else
                    {
                         _cachedPatterns = new List<DesignPattern>();
                         return;
                    }
                }
                _filePath = absolutePath;
            }

            // Read the JSON file into a string first. If the file contains raw control characters
            // inside string literals (e.g., unescaped CR), JsonSerializer might throw. We try a direct
            // deserialize first; if that fails, sanitize string literals to escape control chars, then retry.
            var jsonText = await File.ReadAllTextAsync(_filePath);
            try
            {
                _cachedPatterns = JsonSerializer.Deserialize<List<DesignPattern>>(jsonText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException)
            {
                var sanitized = SanitizeJsonString(jsonText);
                if (!string.Equals(sanitized, jsonText, StringComparison.Ordinal))
                {
                    try
                    {
                        var backup = _filePath + ".bak";
                        File.WriteAllText(backup, jsonText);
                        File.WriteAllText(_filePath, sanitized);
                    }
                    catch
                    {
                        // ignore write-back errors; we still try to use the sanitized content in-memory
                    }
                }
                _cachedPatterns = JsonSerializer.Deserialize<List<DesignPattern>>(sanitized, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            _cachedPatterns = _cachedPatterns ?? new List<DesignPattern>();
        }

        public async Task<IEnumerable<DesignPattern>> GetAllPatternsAsync()
        {
            await EnsureLoadedAsync();
            return _cachedPatterns ?? new List<DesignPattern>();
        }

        public async Task<DesignPattern?> GetPatternByIdAsync(string id)
        {
            await EnsureLoadedAsync();
            return _cachedPatterns?.FirstOrDefault(p => p.Id == id);
        }
    }
}

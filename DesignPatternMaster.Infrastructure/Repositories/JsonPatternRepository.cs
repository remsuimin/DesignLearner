using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Enums;
using DesignPatternMaster.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DesignPatternMaster.Infrastructure.Repositories;

/// <summary>JSON ファイル永続化のリポジトリ実装。読取専用・スレッドセーフ。リロード監視は行わない（起動時ロード＋キャッシュ維持）。</summary>
/// <remarks>スキーマ契約：JSON に Tags・IconPath が欠落した場合は空リスト・null を既定値とする（任意項目。必須化はしない）。</remarks>
public sealed partial class JsonPatternRepository : IPatternRepository, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _filePath;
    private readonly ILogger<JsonPatternRepository> _logger;
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private List<DesignPattern>? _cachedPatterns;
    private bool _disposed;

    public JsonPatternRepository(string filePath = "Data/patterns.json", ILogger<JsonPatternRepository>? logger = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path must not be empty.", nameof(filePath));

        _filePath = filePath;
        _logger = logger ?? NullLogger<JsonPatternRepository>.Instance;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _loadLock.Dispose();
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Pattern data file not found: {Path}")]
    private partial void LogFileNotFound(Exception exception, string path);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Pattern data file is corrupted: {Path}")]
    private partial void LogFileCorrupted(Exception exception, string path);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Loaded {Count} design patterns from {Path}.")]
    private partial void LogLoaded(int count, string path);

    /// <summary>
    /// パスを決定論的に解決する。相対パスは AppContext.BaseDirectory 基準、絶対パスは正規化のみで通過させる。
    /// サンドボックス化（許可ディレクトリ検証）は行わない。パス入力元はコード（DI・テスト）であり
    /// ユーザー入力ではないため。テスト用任意パス指定は仕様として維持する。
    /// </summary>
    private string ResolvePath()
    {
        if (Path.IsPathRooted(_filePath))
            return Path.GetFullPath(_filePath);

        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, _filePath));
    }

    private async Task EnsureLoadedAsync(CancellationToken cancellationToken)
    {
        if (_cachedPatterns is not null)
            return;

        await _loadLock.WaitAsync(cancellationToken);
        try
        {
            if (_cachedPatterns is not null)
                return;

            var resolvedPath = ResolvePath();
            string jsonText;
            try
            {
                jsonText = await File.ReadAllTextAsync(resolvedPath, cancellationToken);
            }
            catch (FileNotFoundException ex)
            {
                LogFileNotFound(ex, resolvedPath);
                throw new FileNotFoundException($"Pattern data file not found: {resolvedPath}", resolvedPath, ex);
            }

            cancellationToken.ThrowIfCancellationRequested();

            List<DesignPattern>? loaded;
            try
            {
                loaded = JsonSerializer.Deserialize<List<DesignPattern>>(jsonText, JsonOptions);
            }
            catch (JsonException ex)
            {
                LogFileCorrupted(ex, resolvedPath);
                throw;
            }

            _cachedPatterns = loaded ?? new List<DesignPattern>();
            LogLoaded(_cachedPatterns.Count, resolvedPath);
        }
        finally
        {
            _loadLock.Release();
        }
    }

    public async Task<IReadOnlyList<DesignPattern>> GetAllPatternsAsync(CancellationToken cancellationToken = default)
    {
        await EnsureLoadedAsync(cancellationToken);
        return _cachedPatterns?.ToList() ?? new List<DesignPattern>();
    }

    public async Task<DesignPattern?> GetPatternByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        await EnsureLoadedAsync(cancellationToken);
        var normalized = id?.Trim() ?? string.Empty;
        return _cachedPatterns?.FirstOrDefault(p => p.Id.Equals(normalized, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IReadOnlyList<DesignPattern>> GetByCategoryAsync(PatternCategory category, CancellationToken cancellationToken = default)
    {
        var all = await GetAllPatternsAsync(cancellationToken);
        return all.Where(p => p.Category == category).ToList();
    }

    public async Task<IReadOnlyList<DesignPattern>> SearchAsync(string keyword, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<DesignPattern>();

        var all = await GetAllPatternsAsync(cancellationToken);
        return all.Where(p =>
            p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            p.Summary.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            p.Tags.Any(t => t.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToList();
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var all = await GetAllPatternsAsync(cancellationToken);
        return all.Count;
    }
}

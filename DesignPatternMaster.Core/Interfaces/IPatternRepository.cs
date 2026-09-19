using DesignPatternMaster.Core.Entities;
using DesignPatternMaster.Core.Enums;

namespace DesignPatternMaster.Core.Interfaces;

/// <summary>デザインパターン集約の読取専用リポジトリ。破壊的変更（Phase 1）。</summary>
public interface IPatternRepository
{
    /// <summary>
    /// 全件を取得する。空の場合も空リストを返す（null 不可）。
    /// 内部キャッシュの防御コピーを返すため、呼出側の変更は保存されない。
    /// データ破損時は <see cref="System.Text.Json.JsonException"/> を送出する（空リストにしない）。
    /// </summary>
    Task<IReadOnlyList<DesignPattern>> GetAllPatternsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ID で取得する。ID は前後空白無視・大小無視で比較する。未存在時は null を返す。
    /// </summary>
    Task<DesignPattern?> GetPatternByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>指定カテゴリに属するパターンを取得する。</summary>
    Task<IReadOnlyList<DesignPattern>> GetByCategoryAsync(PatternCategory category, CancellationToken cancellationToken = default);

    /// <summary>
    /// キーワード検索を行う。<see cref="DesignPattern.Name"/>・
    /// <see cref="DesignPattern.Summary"/>・<see cref="DesignPattern.Tags"/>
    /// の部分一致（大小無視）。空・空白キーワードは空リストを返す。
    /// </summary>
    Task<IReadOnlyList<DesignPattern>> SearchAsync(string keyword, CancellationToken cancellationToken = default);

    /// <summary>全件数を返す。</summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

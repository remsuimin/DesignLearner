# DesignPatternMaster

経験豊富なプログラマー向けの、C#によるデザインパターン学習アプリケーションです。
GoFのデザインパターンを中心に、現代的なC#の実装例と解説を提供します。

## 特徴

- **GoFデザインパターンの網羅**: 生成、構造、振る舞いの各パターンを詳細に解説。
- **モダンなアーキテクチャ**: Onion Architecture (Clean Architecture) を採用し、SOLID原則に基づいた堅牢な設計。
- **リッチなUI**: WPFを使用したモダンで直感的なユーザーインターフェース（ダークモード対応）。
- **実践的なコード例**: 単なる理論だけでなく、実際に動作するC#コードでパターンを学べます。

## 技術スタック

- **言語**: C#
- **フレームワーク**: .NET 8
- **UI**: WPF (Windows Presentation Foundation)
- **アーキテクチャ**: Onion Architecture
  - **Core**: ドメインエンティティ、インターフェース
  - **UseCases**: アプリケーションロジック
  - **Infrastructure**: データアクセス、外部サービス
  - **UI**: プレゼンテーション層 (MVVM)

## 始め方

1. リポジトリをクローンします。
2. Visual Studio または適切なIDEで `DesignPatternMaster.sln` を開きます。
3. ソリューションをビルドします。
4. `DesignPatternMaster.UI` をスタートアッププロジェクトとして実行します。

## ライセンス

このプロジェクトは [MIT License](LICENSE) の下で公開されています。
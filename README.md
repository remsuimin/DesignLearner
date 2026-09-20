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
- **フレームワーク**: .NET 10（`net10.0` / `net10.0-windows`）
- **UI**: WPF (Windows Presentation Foundation)
- **アーキテクチャ**: Onion Architecture
  - **Core**: ドメインエンティティ、インターフェース
  - **UseCases**: アプリケーションロジック
  - **Infrastructure**: データアクセス、外部サービス
  - **UI**: プレゼンテーション層 (MVVM)

## 必須環境

- Windows（WPF のため）
- Visual Studio 2022 17.x 以降 または .NET 10 SDK（`global.json` 参照）

## 始め方

### 1. クローン

```powershell
git clone https://github.com/remsuimin/DesignLearner.git
```

### 2. ビルド・テスト・公開

```powershell
dotnet restore DesignPatternMaster.sln
dotnet build DesignPatternMaster.sln --nologo
dotnet test DesignPatternMaster.sln --nologo
dotnet test DesignPatternMaster.sln --collect:"XPlat Code Coverage" --nologo
dotnet publish DesignPatternMaster.UI/DesignPatternMaster.UI.csproj -c Release -r win-x64 --nologo
```

カバレッジは `TestResults/**/coverage.cobertura.xml` に出力されます。HTML レポートを生成する場合：

```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

（`coverage-report/` は `.gitignore` 対象のためコミットしません。）

### 3. 実行

Visual Studio または適切な IDE で `DesignPatternMaster.sln` を開き、`DesignPatternMaster.UI` をスタートアッププロジェクトとして実行します。

## アセット

- `DesignPatternMaster.UI/Assets/app_icon.png` — ウィンドウアイコン
- `DesignPatternMaster.UI/Assets/splash_screen.png` — スプラッシュ画面

## 開発メモ

- `DesignPatternMaster.UI/Properties/launchSettings.json` は最小プロファイル（`commandName: Project`）のまま追跡しています。シークレットを記載しないでください。
- 書式は `.editorconfig` に従います。変更ファイルのみ `dotnet format` を適用してください。

## ライセンス

このプロジェクトは [MIT License](LICENSE) の下で公開されています。

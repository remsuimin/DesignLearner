# DesignPatternMaster.UseCases

読取専用のクエリ層（CQRS の Q 側のみ）。

## 方針

- `Commands/` は作らない。本コンテキストはデザインパターンの参照に限定され、作成・更新・削除のユースケースが存在しないため。
- DTO は挟まない。当面は `Core.Entities` をそのまま返却する（規模に対して層追加が過剰のため）。フィルタ（例：`IsAntiPattern` 除外）・ソート・ページングが必要になった時点で再検討する。
- 未存在時の取得は `PatternNotFoundException` を送出する（null 透過にしない）。

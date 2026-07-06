# BookStore

[![CI](https://github.com/ianChen806/BookStore/actions/workflows/ci.yml/badge.svg)](https://github.com/ianChen806/BookStore/actions/workflows/ci.yml)

.NET 8 技術練習範本,包含兩個彼此獨立的 ASP.NET Core Web API 專案。

## 專案結構

| 專案 | 說明 |
|---|---|
| `Project1-BookStore` | ASP.NET Core Web API 骨架:健康檢查 ping、EF Core + SQLite、Scalar API 文件;另含 xUnit 單元測試專案。 |
| `Project2-Debug` | ASP.NET Core Web API:會員註冊功能(EF Core + SQLite、NLog、Mediator)。 |

## 需求

- .NET SDK 8.0

## 建置與執行

```bash
# Project1
dotnet run --project Project1-BookStore/Project1-BookStore.csproj

# Project2
dotnet run --project Project2-Debug/Project2-Debug.csproj

# 執行 Project1 單元測試
dotnet test Project1-BookStore/Project1-BookStore.Tests/Project1-BookStore.Tests.csproj
```

啟動後預設開啟 Scalar API 文件頁(`/scalar`)。

## 安全與驗證

本 repo 提供以下可驗證的安全信號,方便任何人在使用前自行確認其乾淨度:

- **持續整合(CI)**:每次 push / PR 由 GitHub Actions 執行 `dotnet build` 與單元測試,狀態見上方 badge。
- **相依套件更新**:啟用 Dependabot,每週自動掃描 NuGet 與 GitHub Actions 的已知漏洞並開 PR。
- **漏洞掃描**:CI 以 `dotnet list package --vulnerable --include-transitive` 掃描所有相依,結果輸出於每次執行的 job summary(報告模式,不阻斷建置)。
- **無機密外洩**:設定檔僅含本地 SQLite 連線字串,repo 內無任何 API key、密碼或憑證。

### 已知殘留漏洞(透明揭露)

| 套件 | 等級 | 說明 |
|---|---|---|
| `SQLitePCLRaw.lib.e_sqlite3` | High（[CVE-2025-6965](https://github.com/advisories/GHSA-2m69-gcr7-jv3q)） | SQLite native 本身的漏洞,由 EF Core Sqlite 以 transitive 相依帶入。上游尚無修正版本,已 pin 至最新 `2.1.11`。觸發前提為「攻擊者能對 SQLite 注入任意 SQL」;本範本使用本地預建資料庫、不將不可信輸入當作 SQL 執行,實際風險可忽略。 |

> Test 專案原有的 `System.Net.Http` 4.3.0 與 `System.Text.RegularExpressions` 4.3.0 兩個 High 漏洞,已透過 pin 至修正版本移除。

## 關於預建資料庫

`Project2-Debug/bookstore.db` 是刻意一併 commit 的 SQLite 檔,內含**測試用的種子資料(非任何真實個人資料)**,目的是讓專案開箱即可執行。

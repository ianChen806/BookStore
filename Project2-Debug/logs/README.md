# 除錯情境 — 依 log 找出原因

以下三個情境各附一段**實際執行時的 server log**(NLog 檔案輸出,格式為
`時間|等級|Logger|訊息`,例外會接在後面附完整 stack trace)。

請依 **log + 程式碼** 推敲每個情境的根本原因並修正。三題難度由低到高,
差別在「線索的明顯程度」:低有例外直接指路、中只有可疑數值、高連錯誤都沒有。

> 不要依賴猜測與 try-and-error;先讀 log、對照程式碼,說明原因後再動手。

---

## Case 1(低)— 註冊回 500

**重現**

```
POST /member/register
Content-Type: application/json

{ "account": "wanda", "name": "Wanda", "email": "wanda@mail.com" }
```

**觀察到的回應**

```
HTTP 500
{ "error": "Internal Server Error" }
```

**Log**：[`case1-register-500.log`](./case1-register-500.log)

**問題**：一筆看起來完全正常的註冊,為什麼會 500?log 裡的例外訊息與
stack trace 指向哪一行?那一行的判斷是否符合它的本意?

---

## Case 2(中)— 註冊回 200,但資料查不到

**重現**(在 Case 1 修正後,以合法資料註冊)

```
POST /member/register
Content-Type: application/json

{ "account": "victor", "name": "Victor", "email": "victor@mail.com" }
```

**觀察到的回應**

```
HTTP 200
{ "memberId": 0, "memberLevelId": 1 }
```

隨後 `GET /member/list` 查無這位新會員。

**Log**：[`case2-register-memberid0.log`](./case2-register-memberid0.log)

**問題**：API 回 200 代表成功,為什麼資料庫查不到?`memberId` 為什麼是 `0`?
對照 log 中的 SQL —— 從進 handler 到回應,你有看到任何 `INSERT INTO "Members"` 嗎?

---

## Case 3(高)— 清單結果正確,但…

**重現**

```
GET /member/list
```

(資料庫中有 8 位會員)

**觀察到的回應**

```
HTTP 200
（結果正確,每位會員都帶出正確的等級名稱）
```

**Log**：[`case3-list-queries.log`](./case3-list-queries.log)

**問題**:這支 API 不會 crash、結果也對,但攤開 log 看它到底對資料庫發了
**幾條** SQL?會員數變成 100、1000 時會怎樣?問題出在 handler 的哪個寫法?

# Restful‑Booker API Test Automation – Technical Assessment  

> A fully‑automated, CI‑driven test suite for the public **Restful‑Booker** API.  
> Built with C#/.NET 8, xUnit, RestSharp and FluentAssertions.  

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Setup & Execution](#setup--execution)
   - 2.1 Local run  
3. [Continuous Integration](#continuous-integration)
4. [Test Strategy & Plan](#test-strategy--plan)
5. [Summary of Findings & Recommendations](#summary-of-findings--recommendations)
6. [Contributing & Contact](#contributing--contact)

---

## 1. Prerequisites

| Tool | Minimum Version |
|------|-----------------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0.x |
| GitHub account (for CI artifacts) | – |

> **Tip** – If you only want to run the tests locally, Docker is not required.

---

## 2. Setup & Execution

### 2.1 Local run

```bash
# 1️⃣ Clone the repo
git clone https://github.com/<you>/restful-booker-tests.git
cd restful-booker-tests

# 2️⃣ Restore, build and run tests (xUnit + ExtentReports)
dotnet restore
dotnet build --configuration Release
dotnet test RestfulBookerTests/RestfulBookerTests.csproj --configuration Release
```
---

## 3. Continuous Integration

The repo contains a GitHub Actions workflow (`.github/workflows/api-tests.yml`) that:

1. Checks out the repo  
2. Sets up .NET 8  
3. Restores & builds the test project  
4. Runs tests – ExtentReports are generated in `extent.html`  
5. Uploads the HTML report as an artifact named **extent‑report**  

You can view the report from the *Artifacts* tab of any workflow run.

---

## 4. Test Strategy & Plan

| Aspect | Implementation |
|--------|----------------|
| **Framework** | xUnit 2.x + RestSharp (HTTP client) |
| **Assertions** | FluentAssertions for expressive checks |
| **CI/CD** | GitHub Actions + Docker (optional) |
| **Test Structure** | <br>• `ApiClient.cs` – HTTP wrapper<br>• `AuthHelper.cs` – token management<br>• `TestDataLoader<T>.cs` – JSON/CSV loader<br>• `BookingDto.cs` – request/response DTOs |
| **Coverage** | <br>• GET `/booking` – filters, pagination, performance (<2s)<br>• PATCH `/booking/{id}` – single/multi field, nested objects, idempotency<br>• DELETE `/booking/{id}` – success, 404, auth validation<br>• Data‑driven (positive & negative) via external JSON files<br>• End‑to‑end flow: create → update → verify → delete |
| **Negative Testing** | Invalid dates, missing fields, malformed JSON, SQL‑injection strings |
| **Performance Testing** | Response time assertions (≤ 2 s) using `Stopwatch` |
| **Parallel Execution** | xUnit’s default parallelism; can be tuned in `xunit.runner.json` |
| **Retry Logic** | Optional – implemented via Polly (not shown in this repo) |

---

## 5. Summary of Findings & Recommendations

| Issue | Observation | Recommendation |
|-------|-------------|----------------|
| **Rate‑Limiting** | API occasionally returns `429 Too Many Requests` during bulk tests. | Add a retry‑with‑exponential backoff (Polly) for idempotent endpoints. |
| **Input Validation** | Missing validation for `checkin`/`checkout` formats leads to unexpected 500 errors. | Enforce stricter validation in the API or add defensive checks in tests. |
| **Authentication** | Token expires after 5 min; tests running long loops sometimes fail. | Cache the token and refresh automatically when a 401 is received. |
| **Performance** | Some GET requests > 2 s under load (simulated with parallel tests). | Investigate caching or pagination on the server side; consider adding a performance monitoring dashboard. |
| **Test Data Management** | JSON files are stored in `src/RestfulBookerTests/src/TestData`. | Move test data to a dedicated `/test-data` folder and use `CopyToOutputDirectory` in the csproj. |
| **CI Parallelism** | GitHub Actions runs tests serially by default. | Enable matrix strategy or split test classes into separate jobs for faster feedback. |
| **Contract Testing** | No contract tests (Pact) currently exist. | Add Pact tests to ensure API consumers are not broken by future changes. |

---

# Restful‑Booker API Test Automation – Technical Assessment  

> A fully‑automated, CI‑driven test suite for the public **Restful‑Booker** API.  
> Built with C#/.NET 8, Nunit, RestSharp and FluentAssertions.  

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

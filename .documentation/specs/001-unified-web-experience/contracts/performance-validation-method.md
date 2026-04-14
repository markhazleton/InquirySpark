# Performance Validation Method

**Spec:** `001-unified-web-experience`  
**Task:** T059  
**Status:** Method Definition — execute per T060

---

## Scope

Performance validation confirms that the unified web experience routes meet the same or better response time thresholds as legacy application equivalents.

## Thresholds

| Page Type | Acceptable (P95) | Warning | Fail |
|---|---|---|---|
| Static pages (Home, Dashboard) | < 200 ms | 200–500 ms | > 500 ms |
| Data list pages (index views) | < 500 ms | 500–1000 ms | > 1000 ms |
| Detail views (single record) | < 300 ms | 300–700 ms | > 700 ms |
| Capability Completion Matrix | < 800 ms | 800–1500 ms | > 1500 ms |
| Operational Readiness Dashboard | < 500 ms | 500–1000 ms | > 1000 ms |
| Health endpoint | < 100 ms | 100–300 ms | > 300 ms |

## Measurement Method

1. Start `InquirySpark.Web` locally in Release mode: `dotnet run --project InquirySpark.Web -c Release`
2. Authenticate using a test account with the Analyst role
3. Use browser DevTools (Network tab → disable cache) or `curl --silent -o /dev/null -w "%{time_total}"` to measure Time-to-First-Byte (TTFB)
4. Record 5 measurements per page, take the P95 value (4th highest of 5)
5. Record: URL, method, auth role, measurement timestamp, 5 raw values, P95 result, PASS/FAIL

## Key URLs to Validate

| Capability | URL | Representative Route |
|---|---|---|
| Decision Workspace — Conversations | `/Unified/DecisionWorkspace/Conversations` | CAP-DS-001 |
| Inquiry Administration — Applications | `/Unified/InquiryAdministration/Applications` | CAP-IA-001 |
| Inquiry Authoring — Surveys | `/Unified/InquiryAuthoring/Surveys` | CAP-IA-006 |
| Operations Support — Health | `/Unified/OperationsSupport/Health` | CAP-DS-007 |
| Capability Completion Matrix | `/Unified/CapabilityCompletionMatrix` | governance |
| Operational Readiness | `/Unified/OperationalReadiness` | governance |

## Gate

Performance validation PASSES when all measured pages meet the "Acceptable" threshold. The result is recorded in `contracts/performance-validation-evidence.md` (T060).

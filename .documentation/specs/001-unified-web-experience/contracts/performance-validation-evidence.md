# Performance Validation Evidence

**Spec:** `001-unified-web-experience`  
**Task:** T060  
**Method:** See `performance-validation-method.md` (T059)  
**Status:** Template — populate with actual measured values before cutover

---

## Instructions

1. Complete performance measurement per `performance-validation-method.md`
2. Record PASS/FAIL for each route below
3. All routes must PASS before any domain cutover proceeds

---

## Measurement Record

| URL | Role | M1 (ms) | M2 (ms) | M3 (ms) | M4 (ms) | M5 (ms) | P95 (ms) | Threshold | Result |
|---|---|---|---|---|---|---|---|---|---|
| `/Unified/DecisionWorkspace/Conversations` | Analyst | — | — | — | — | — | — | 500 ms | PENDING |
| `/Unified/InquiryAdministration/Applications` | Operator | — | — | — | — | — | — | 500 ms | PENDING |
| `/Unified/InquiryAuthoring/Surveys` | Analyst | — | — | — | — | — | — | 500 ms | PENDING |
| `/Unified/InquiryOperations/Deployments` | Operator | — | — | — | — | — | — | 500 ms | PENDING |
| `/Unified/OperationsSupport/Health` | Anonymous | — | — | — | — | — | — | 100 ms | PENDING |
| `/Unified/CapabilityCompletionMatrix` | Executive | — | — | — | — | — | — | 800 ms | PENDING |
| `/Unified/OperationalReadiness` | Executive | — | — | — | — | — | — | 500 ms | PENDING |

---

## Overall Result: PENDING

> This table must be populated by running the application in Release mode and recording actual measurements before any domain cutover is approved.

**Measured by:** [name]  
**Measured at:** [ISO 8601 datetime]  
**App version:** [git commit SHA]  
**Overall:** [ ] PASS / [ ] FAIL

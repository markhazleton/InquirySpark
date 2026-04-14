# Stakeholder Communication Pack

**Spec:** `001-unified-web-experience`  
**Task:** T047  
**Status:** Active — use templates during cutover execution

---

## Audience Map

| Audience | Interest | Communication Channel |
|---|---|---|
| Business stakeholders | Feature availability, no disruption | Email / Slack announcement |
| Development team | Technical changes, decommission timeline | Team channel / sprint review |
| Operations team | Monitoring, incident response | Ops runbook + Slack ops channel |
| End users (Analyst/Operator roles) | New UI location for their workflows | In-app banner + email |

---

## Pre-Cutover Announcement Template

**Subject:** [InquirySpark] Unified Workspace Available — {Domain} Capabilities Moving

---

We are pleased to announce that the **{Domain}** capabilities have been validated in the new InquirySpark unified workspace.

**What's changing:**
- All **{Domain}** workflows are now available at `https://{host}/Unified/{Area}`
- The legacy application ({LegacyApp}) remains available during the transition window

**What you need to do:**
- Update any bookmarks to the new unified URLs (see capability list below)
- Report any discrepancies via the incident channel

**Timeline:**
- Cutover date: {DATE}
- Legacy access available until: {DATE + 30 days}

**New Unified Routes:**
{list of capability routes from CapabilityRoutingMap}

Questions? Contact {contact info}.

---

## Post-Cutover Announcement Template

**Subject:** [InquirySpark] {Domain} Cutover Complete — Legacy Access Window Begins

---

The **{Domain}** cutover to InquirySpark.Web has been completed successfully.

**Current status:**
- All {Domain} capabilities are live at `/Unified/{Area}`
- Legacy {LegacyApp} is available as fallback for 30 days (until {date})

**Action required by {date}:**
- Please migrate any remaining workflows to the unified workspace
- After {date}, legacy access will be disabled

**Incident reporting:**
- P1/P2 parity issues: immediately escalate via ops incident channel
- Minor UX feedback: submit via GitHub issues

---

## Decommission Announcement Template

**Subject:** [InquirySpark] {LegacyApp} Decommission — Access Ends {DATE}

---

The **{LegacyApp}** application will be decommissioned on **{DATE}**.

All capabilities are now fully available in the unified InquirySpark workspace.

After decommission:
- {LegacyApp} URL will redirect to `/Unified/Operations`
- All data remains available via the unified workspace

If you experience any issues after decommission, please contact {contact}.

---

## Cutover Communication Log

_Record actual communications sent during each domain cutover._

| Domain | Date | Audience | Template Used | Sent By |
|---|---|---|---|---|
| (to be filled during execution) | | | | |

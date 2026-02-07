# Feature Specification: Workflow Engine

**Feature Branch**: `005-workflow-engine`
**Created**: 2026-02-07
**Status**: Draft
**Input**: Custom workflow engine with swappable execution for tax resolution. White-label platform where workflows are created and managed at the platform level, then provided to tax professional subscribers who use them to serve their clients.

## Overview

This feature establishes a white-label workflow platform for IRS tax resolution. The platform owner creates and manages workflows based on the IRS Collection Solutions Handbook. Tax professional subscribers use these workflows to analyze taxpayer situations and guide resolution strategies for their clients.

### Business Model

```
┌─────────────────────────────────────────────────────────────────┐
│  PLATFORM LEVEL (Your Product)                                  │
│  - Create and manage workflows, decision tables, calculations   │
│  - Maintain IRS rule updates                                    │
│  - Publish workflows to subscribers                             │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│  SUBSCRIBER LEVEL (Tax Professional Firms)                      │
│  - Subscribe to the platform                                    │
│  - Execute workflows on client cases (no customization)         │
│  - Perform human review tasks                                   │
│  - Manage their own clients                                     │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│  CLIENT LEVEL (Taxpayers)                                       │
│  - Receive services from their tax professional                 │
│  - Upload documents / provide information                       │
│  - Approve recommended strategies                               │
└─────────────────────────────────────────────────────────────────┘
```

### User Roles

| Role | Description | Primary Actions |
|------|-------------|-----------------|
| **Platform Admin** | Platform owner/staff who manage the product | Create/edit workflows, decision tables, formulas; publish updates; manage IRS rule changes |
| **Subscriber Admin** | Tax professional firm owner/manager | Manage team, view all cases, configure firm settings |
| **Subscriber User** | Tax professional staff member | Execute workflows, perform reviews, manage assigned clients |
| **Client** | Taxpayer receiving services (no account - accesses via secure email links) | Upload documents, provide information, approve recommendations |

### Architecture Principles

1. **White-Label Platform**: Workflows managed centrally, delivered to subscribers
2. **Externalized Rules**: All business logic stored in database, not code
3. **Swappable Execution**: Abstraction layer allows future adoption of Temporal or other engines
4. **Version Control**: Workflows versioned with subscriber case-to-version binding
5. **IP Ownership**: 100% custom-built, no external workflow engine dependencies
6. **Multi-Tier Tenancy**: Platform level + subscriber organizations + subscriber clients

### Workflow Categories

- **Analysis Workflows**: Parse transcripts, identify issues, calculate statute dates
- **Decision Workflows**: Evaluate taxpayer situation, recommend resolution strategies
- **Execution Workflows**: Guide users through resolution implementation steps
- **Document Workflows**: Generate forms, letters, and calculations

## Clarifications

### Session 2026-02-07

- Q: Can subscribers customize platform-provided workflows? → A: No customization - subscribers use published workflows exactly as provided
- Q: Will there be multiple subscription tiers with different feature access? → A: Single tier - all subscribers get access to all published workflows
- Q: How do taxpayer clients access the platform? → A: Email links with secure tokens - clients receive links for specific actions (no account needed)
- Q: What level of data protection compliance is required? → A: Standard security best practices - encryption at rest, TLS, audit logs, but no formal certification
- Q: What durability guarantee is needed for workflow execution state? → A: Database-persisted state - workflow state saved after each step, can resume after restart

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Define Decision Table Rules (Priority: P1)

As a **Platform Admin**, I want to define decision rules using a table format so that subscribers' workflows can automatically evaluate taxpayer situations and recommend appropriate resolution strategies.

**Why this priority**: Decision tables are the foundation of the entire workflow system. Without the ability to define conditional logic, no automated analysis or recommendations can occur. This is the core "brain" of the platform.

**Independent Test**: Can be fully tested by creating a decision table with IRS resolution rules (e.g., "If balance < $50k AND compliant, recommend Streamlined IA") and verifying the correct output is selected for various input scenarios.

**Acceptance Scenarios**:

1. **Given** a Platform Admin is logged in, **When** they create a new decision table with columns for input conditions and output actions, **Then** the table is saved and available for use in workflows.

2. **Given** a decision table with multiple rules exists, **When** the system evaluates a taxpayer's data against the table, **Then** the first matching rule (by priority order) determines the output.

3. **Given** a decision table has overlapping conditions, **When** multiple rules could match, **Then** the rule with highest priority (lowest row number) is selected.

4. **Given** a Platform Admin edits a published decision table, **When** they save changes, **Then** a new version is created and existing subscriber cases continue using their bound version.

---

### User Story 2 - Execute Workflow on Client Case (Priority: P1)

As a **Subscriber User** (tax professional), I want to execute a workflow on a client's case so that the system automatically analyzes their transcripts and provides actionable recommendations.

**Why this priority**: Execution is equally critical to definition. A workflow engine that can't run workflows has no value. This enables the core use case of automated transcript analysis.

**Independent Test**: Can be fully tested by running a workflow against a client with uploaded transcripts and verifying the workflow produces expected issues, calculations, and recommendations.

**Acceptance Scenarios**:

1. **Given** a client has uploaded transcripts and a published workflow exists, **When** a Subscriber User initiates the workflow, **Then** the system processes each step and records results.

2. **Given** a workflow step requires client data (e.g., financial information), **When** the data is not yet collected, **Then** the workflow pauses and indicates what information is needed.

3. **Given** a workflow is executing, **When** a step fails or encounters an error, **Then** the workflow records the error and allows retry or manual intervention.

4. **Given** a workflow completes successfully, **When** viewing the case, **Then** all outputs (issues found, calculations, recommendations) are visible and attached to the case.

---

### User Story 3 - Build Calculation Formulas (Priority: P2)

As a **Platform Admin**, I want to define calculation formulas so that subscriber workflows can compute values like Reasonable Collection Potential (RCP), monthly payment amounts, and statute dates.

**Why this priority**: Many IRS resolution decisions depend on calculated values. Without formulas, decision tables can only use static data, limiting their usefulness.

**Independent Test**: Can be fully tested by defining an RCP formula and verifying correct calculation given sample asset and income values.

**Acceptance Scenarios**:

1. **Given** a Platform Admin is defining a formula, **When** they reference client data fields and mathematical operators, **Then** the formula is validated for correctness before saving.

2. **Given** a formula references another formula's output, **When** the workflow executes, **Then** dependent formulas are calculated in the correct order.

3. **Given** a formula uses IRS standard values (e.g., allowable living expenses), **When** those standards change, **Then** the Platform Admin can update the values without modifying the formula definition.

4. **Given** a calculation encounters division by zero or invalid input, **When** the formula executes, **Then** a meaningful error is returned rather than crashing the workflow.

---

### User Story 4 - Version Workflow Definitions (Priority: P2)

As a **Platform Admin**, I want workflow definitions to be versioned so that existing subscriber cases continue using the rules that were in effect when their case started, while new cases use updated rules.

**Why this priority**: IRS rules change annually. Cases can span months or years. Without versioning, rule changes could invalidate in-progress work or cause inconsistent treatment of similar cases.

**Independent Test**: Can be fully tested by modifying a workflow, then verifying that an existing case still uses the old version while a new case uses the updated version.

**Acceptance Scenarios**:

1. **Given** a workflow definition is published, **When** a Platform Admin makes changes, **Then** a new version is created and the previous version remains available.

2. **Given** a subscriber's client case is bound to workflow version 1.0, **When** version 2.0 is published, **Then** the case continues executing under version 1.0 rules.

3. **Given** a case is using an older workflow version, **When** a Subscriber User chooses to migrate it, **Then** the case is re-evaluated under the new version with clear indication of what changed.

4. **Given** multiple workflow versions exist, **When** viewing version history, **Then** users can see what changed between versions and when each was effective.

---

### User Story 5 - Human-in-the-Loop Review Gates (Priority: P2)

As a **Subscriber User** (tax professional), I want workflows to pause for human review at critical decision points so that automated recommendations are verified before being presented to clients.

**Why this priority**: Tax resolution involves significant financial and legal implications. Fully automated decisions without professional review create liability risk. Review gates ensure quality control.

**Independent Test**: Can be fully tested by configuring a workflow with a review step, executing it, and verifying the workflow pauses until a professional approves or modifies the recommendation.

**Acceptance Scenarios**:

1. **Given** a workflow reaches a review step, **When** the step is configured as required, **Then** the workflow pauses until a designated Subscriber User takes action.

2. **Given** a workflow is paused for review, **When** a Subscriber User approves, **Then** the workflow continues to the next step.

3. **Given** a workflow is paused for review, **When** a Subscriber User rejects or modifies, **Then** the workflow records the changes and may route to a different path.

4. **Given** multiple cases are awaiting review, **When** a Subscriber User views their queue, **Then** cases are listed with relevant context for efficient review.

---

### User Story 6 - Client Approval Steps (Priority: P3)

As a **Client** (taxpayer), I want to review and approve recommended strategies so that I am informed and my consent is documented before proceeding.

**Why this priority**: Client consent is legally important and builds trust. However, it depends on having recommendations to approve (US1, US2) and professional review (US5) first.

**Independent Test**: Can be fully tested by sending a strategy recommendation to a client portal and verifying the workflow pauses until client responds.

**Acceptance Scenarios**:

1. **Given** a workflow reaches a client approval step, **When** the step executes, **Then** the Client receives an email with a secure link to view and respond to the recommendation.

2. **Given** a Client views the approval request, **When** they approve, **Then** the workflow continues and their consent is recorded with timestamp.

3. **Given** a Client views the approval request, **When** they have questions or decline, **Then** the workflow routes to the Subscriber User for follow-up.

4. **Given** an approval request has been pending beyond a configured time, **When** the deadline passes, **Then** a reminder is sent and/or the case is flagged for the Subscriber User's attention.

---

### User Story 7 - Visual Workflow Builder (Priority: P3)

As a **Platform Admin**, I want a visual interface to build workflows so that I can define the sequence of steps without writing code.

**Why this priority**: While decision tables (US1) handle the logic, a visual builder makes it easier to compose complex multi-step workflows. This enhances usability but the system can function with code-defined workflows initially.

**Independent Test**: Can be fully tested by building a complete transcript analysis workflow using only the visual interface, then executing it successfully.

**Acceptance Scenarios**:

1. **Given** a Platform Admin opens the workflow builder, **When** they add steps from a palette, **Then** steps appear in a visual sequence that can be reordered.

2. **Given** a workflow has conditional branches, **When** the Platform Admin defines branch conditions, **Then** the visual display shows the branching paths clearly.

3. **Given** a workflow is being built, **When** the Platform Admin adds a decision table reference, **Then** they can select from available tables and configure which outputs map to which branches.

4. **Given** a workflow is complete, **When** the Platform Admin publishes it, **Then** the workflow becomes available to all subscribers.

---

### User Story 8 - Issue Detection from Transcripts (Priority: P1)

As a **Subscriber User** (tax professional), I want the system to automatically identify issues from IRS transcripts so that I don't have to manually review every transaction code.

**Why this priority**: This is the primary entry point for the workflow system. Transcript analysis drives everything else. Without issue detection, there's nothing to evaluate or resolve.

**Independent Test**: Can be fully tested by uploading a transcript with known issues (balance due, unfiled returns, penalties) and verifying the system correctly identifies and categorizes each issue.

**Acceptance Scenarios**:

1. **Given** a Client has uploaded IRS account transcripts, **When** the issue detection workflow runs, **Then** all balance due amounts are identified with tax years and types.

2. **Given** a transcript contains penalty transactions, **When** analyzed, **Then** penalty types, amounts, and potential abatement opportunities are flagged.

3. **Given** a transcript shows missing returns, **When** analyzed, **Then** unfiled tax years are identified with filing requirement status.

4. **Given** transcript data includes dates, **When** analyzed, **Then** statute dates (CSED, ASED, RSED) are calculated and displayed.

---

### User Story 9 - Publish Workflows to Subscribers (Priority: P1)

As a **Platform Admin**, I want to publish workflows so that all subscribers can use them to serve their clients.

**Why this priority**: The core value proposition of the platform is delivering professionally-designed workflows to subscribers. Without publishing, the white-label model doesn't work.

**Independent Test**: Can be fully tested by creating a workflow, publishing it, and verifying it appears in a subscriber's available workflows.

**Acceptance Scenarios**:

1. **Given** a Platform Admin has created a workflow, **When** they publish it, **Then** all active subscribers can see and execute the workflow.

2. **Given** a workflow is published, **When** a Platform Admin publishes an update, **Then** new cases use the updated version while existing cases continue with their bound version.

3. **Given** a workflow has issues, **When** a Platform Admin unpublishes it, **Then** it is no longer available for new cases but existing cases can complete.

4. **Given** multiple workflows exist, **When** a Subscriber User views available workflows, **Then** they see all published workflows.

---

### User Story 10 - Subscriber Onboarding (Priority: P2)

As a **Subscriber Admin** (tax professional firm owner), I want to set up my organization on the platform so that my team can use the workflows to serve our clients.

**Why this priority**: Subscribers must be able to self-onboard and configure their organization for the platform to scale.

**Independent Test**: Can be fully tested by a new subscriber completing registration, inviting team members, and executing their first workflow.

**Acceptance Scenarios**:

1. **Given** a tax professional firm wants to subscribe, **When** they complete registration, **Then** their organization is created and they become the Subscriber Admin.

2. **Given** a Subscriber Admin is logged in, **When** they invite team members, **Then** those users can join as Subscriber Users with appropriate permissions.

3. **Given** a new subscriber organization, **When** they add their first client, **Then** they can immediately run available workflows on that client's case.

4. **Given** a Subscriber Admin is managing their organization, **When** they view usage, **Then** they can see workflow executions and case statistics.

---

### Edge Cases

- What happens when a workflow version is deleted while cases are still using it? (Versions should be retired, not deleted)
- How does the system handle circular formula dependencies? (Validate and reject at save time)
- What if a decision table has no matching rules for an input? (Return "no match" result, allow workflow to handle)
- What happens when a human review step assignee is unavailable? (Allow reassignment or escalation)
- How are concurrent edits to a workflow handled? (Optimistic locking with version conflict detection)
- What if transcript data is incomplete or corrupted? (Workflow should handle gracefully with partial results and warnings)

## Requirements *(mandatory)*

### Functional Requirements

#### Workflow Definition (Platform Level)
- **FR-001**: System MUST allow Platform Admins to create workflow definitions with named steps
- **FR-002**: System MUST support step types: decision table evaluation, calculation, human task, client approval, document generation
- **FR-003**: System MUST allow conditional branching based on step outputs
- **FR-004**: System MUST validate workflow definitions for completeness before publishing
- **FR-005**: System MUST store workflow definitions in the database, not in code
- **FR-006**: System MUST support publishing workflows to make them available to subscribers
- **FR-007**: System MUST support unpublishing workflows to prevent new usage while allowing existing cases to complete

#### Decision Tables
- **FR-010**: System MUST allow creation of decision tables with input columns and output columns
- **FR-011**: System MUST support condition operators: equals, not equals, less than, greater than, between, contains, is empty
- **FR-012**: System MUST evaluate rules in priority order (top to bottom)
- **FR-013**: System MUST return the first matching rule's outputs
- **FR-014**: System MUST support referencing client data fields as input values
- **FR-015**: System MUST support referencing calculation outputs as input values

#### Calculations
- **FR-020**: System MUST allow definition of calculation formulas using mathematical expressions
- **FR-021**: System MUST support operators: add, subtract, multiply, divide, min, max, round, if/then/else
- **FR-022**: System MUST allow formulas to reference client data fields
- **FR-023**: System MUST allow formulas to reference other formula outputs (with dependency resolution)
- **FR-024**: System MUST support IRS standard values (allowable expenses, etc.) as referenced constants

#### Versioning
- **FR-030**: System MUST create a new version when a published workflow is modified
- **FR-031**: System MUST allow draft versions that are not yet active
- **FR-032**: System MUST bind each case to a specific workflow version at case start
- **FR-033**: System MUST retain all versions indefinitely (no hard delete)
- **FR-034**: System MUST support version comparison showing differences
- **FR-035**: System MUST support migrating a case to a newer workflow version with audit trail

#### Execution
- **FR-040**: System MUST execute workflow steps in defined sequence
- **FR-041**: System MUST pause execution at human task steps until completed
- **FR-042**: System MUST record all step inputs, outputs, and timing
- **FR-043**: System MUST support workflow retry from failed step
- **FR-044**: System MUST support workflow cancellation with reason
- **FR-045**: System MUST support parallel step execution where dependencies allow
- **FR-046**: System MUST persist workflow execution state to database after each step completion
- **FR-047**: System MUST resume paused or interrupted workflows from last completed step after server restart

#### Human Tasks
- **FR-050**: System MUST create task queue entries for human review steps
- **FR-051**: System MUST support task assignment to specific users or roles
- **FR-052**: System MUST support task reassignment
- **FR-053**: System MUST track task due dates and send reminders
- **FR-054**: System MUST record task completion with user, timestamp, and decision

#### Transcript Analysis
- **FR-060**: System MUST parse IRS account transcript transaction codes
- **FR-061**: System MUST identify balance due amounts by tax year and type
- **FR-062**: System MUST identify assessed penalties by type
- **FR-063**: System MUST identify unfiled return indicators
- **FR-064**: System MUST calculate Collection Statute Expiration Date (CSED) from assessment dates
- **FR-065**: System MUST identify payment history and payment plan status

#### Subscriber Management
- **FR-070**: System MUST allow new subscribers to self-register their organization
- **FR-071**: System MUST support Subscriber Admin inviting team members as Subscriber Users
- **FR-072**: System MUST isolate subscriber data (clients, cases, executions) from other subscribers
- **FR-073**: System MUST allow Subscriber Admins to view all cases within their organization
- **FR-074**: System MUST allow Subscriber Users to view only their assigned clients/cases
- **FR-075**: System MUST provide subscribers access to all published workflows

#### Platform Administration
- **FR-080**: System MUST allow Platform Admins to manage workflow definitions
- **FR-081**: System MUST allow Platform Admins to publish/unpublish workflows
- **FR-082**: System MUST allow Platform Admins to update IRS standard values (allowable expenses, etc.)
- **FR-083**: System MUST allow Platform Admins to view aggregate platform metrics (without subscriber PII)
- **FR-084**: System MUST support Platform Admin impersonation of subscriber for support purposes (with audit)

### Key Entities

#### Platform Level (Managed by Platform Admins)
- **WorkflowDefinition**: Named workflow with steps, owned by platform, has versions
- **WorkflowVersion**: Immutable snapshot of workflow definition with effective date range and publish status
- **WorkflowStep**: Individual step within a workflow (type, configuration, transitions)
- **DecisionTable**: Named table with input columns, output columns, and priority-ordered rules
- **DecisionRule**: Single row in decision table with conditions and outputs
- **CalculationFormula**: Named formula with expression and input references
- **StandardValue**: IRS standard values (allowable expenses, mileage rates, etc.) with effective dates

#### Subscriber Level (Tax Professional Organizations)
- **Subscriber**: Organization subscribed to the platform (tax professional firm)
- **SubscriberUser**: User belonging to a subscriber organization with role (Admin or User)
- **SubscriberClient**: Client (taxpayer) managed by a subscriber organization
- **CaseWorkflow**: Binding of subscriber's client case to workflow version with execution state
- **WorkflowExecution**: Record of workflow run with step-by-step results
- **HumanTask**: Task created for human review with assignee, status, and due date
- **Issue**: Problem identified from transcript analysis (type, severity, tax year, amount)

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Platform Admins can create a complete resolution workflow using the visual builder in under 30 minutes
- **SC-002**: System correctly identifies 95% of issues from test transcripts compared to manual professional review
- **SC-003**: Workflow execution for transcript analysis completes in under 10 seconds for typical transcript size
- **SC-004**: Decision table evaluation returns results in under 1 second for tables with up to 100 rules
- **SC-005**: New subscribers can complete onboarding and run their first workflow within 15 minutes
- **SC-006**: System supports 100+ subscriber organizations with 1000+ combined active cases without performance degradation
- **SC-007**: Version migration preserves all case data and provides clear audit trail of changes
- **SC-008**: Human task queue displays pending items within 2 seconds of page load
- **SC-009**: Zero subscriber cases are affected by workflow updates unless explicitly migrated
- **SC-010**: Calculation formulas produce identical results to manual spreadsheet calculations for RCP, payment plans, and statute dates
- **SC-011**: 90% of Subscriber Users can execute a workflow on a client case without training documentation

## Assumptions

- Platform Admins (your team) have domain expertise in IRS tax resolution from the Collection Solutions Handbook
- Subscriber Users (tax professionals) have basic familiarity with tax resolution but may not know all IRS rules
- IRS transcript formats are stable enough to build reliable parsing rules
- The existing client management system can be extended to support the subscriber/client hierarchy
- Initial workflows will be defined by Platform Admins based on the IRS Collection Solutions Handbook
- The visual workflow builder can be delivered in a later phase; decision tables and execution engine are higher priority
- Human tasks will initially be assigned to individual Subscriber Users; role-based assignment can be added later
- Subscribers will pay for platform access (single tier - all subscribers get full access to all published workflows)

## Constraints

- Must integrate with existing PostgreSQL database and .NET backend
- Must support multi-tier tenancy: platform level + subscriber organizations + subscriber clients
- Must not depend on external workflow engines (maintain IP ownership)
- Execution layer must be abstracted to allow future swapping if needed
- All workflow definitions must be exportable/importable for backup and migration
- Subscriber data must be isolated; subscribers cannot see other subscribers' clients or cases
- Platform Admins can view aggregate metrics but not individual subscriber client data (privacy)
- Subscribers cannot customize or modify workflows; they execute platform-provided workflows as-is

## Security Requirements

- All data encrypted at rest (database-level encryption)
- All data encrypted in transit (TLS 1.2+)
- Audit logging for all data access and modifications
- Secure token generation for client email links (time-limited, single-use where appropriate)
- No formal compliance certification required for MVP (SOC 2 can be pursued later if needed)

## Dependencies

- **004-client-management**: Client data model needs extension for subscriber/client hierarchy
- **Transcript Parsing**: Existing or new capability to extract structured data from IRS transcripts
- **User Authentication**: Auth0 integration needs role hierarchy (Platform Admin, Subscriber Admin, Subscriber User); Clients access via secure email tokens, not Auth0
- **Subscription Management**: Future feature for managing subscriber billing and access tiers

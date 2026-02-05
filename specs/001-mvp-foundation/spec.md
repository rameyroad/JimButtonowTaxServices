# Feature Specification: IRS Transcript Analysis System - MVP Foundation

**Feature Branch**: `001-mvp-foundation`
**Created**: 2026-02-05
**Status**: Draft
**Input**: User description: "Foundational scaffolding for IRS transcript analysis system focused on securely gathering and managing client data and transcript information, with UI based on CargoMax dashboard theme. Analysis modules will be added incrementally in future phases."

## Clarifications

### Session 2026-02-05

- Q: What e-signature approach should the system use for 8821 forms? → A: Support both built-in and third-party e-signature options (organization choice)
- Q: How long should 8821 authorization signature links remain valid? → A: Configurable per organization (admin sets expiration policy)
- Q: Should Phase 1 (MVP) include full multi-tenant organization isolation? → A: Yes, multi-tenant architecture from day one with organization isolation built in
- Q: Should the system treat individual and business clients as distinct types? → A: Unified client model with type flag (Individual/Business) and conditional fields
- Q: What notification channels should the system support? → A: Email + in-app for MVP; architecture supports SMS for future integration

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Tax Professional Onboards a New Client (Priority: P1)

A tax professional (service provider) needs to register a new taxpayer client in the system, capturing their identifying information securely to begin the authorization process.

**Why this priority**: Without clients in the system, no other functionality is possible. This is the entry point for all value the system provides.

**Independent Test**: Can be fully tested by creating a new client record and verifying it appears in the client list with all required fields properly stored and encrypted.

**Acceptance Scenarios**:

1. **Given** a logged-in tax professional, **When** they navigate to "Add Client" and enter taxpayer name, SSN/EIN, address, and contact information, **Then** the client is created and appears in their client list with sensitive data masked.
2. **Given** a tax professional adding a client, **When** they enter an invalid SSN/EIN format, **Then** the system displays a validation error and prevents submission.
3. **Given** a client record exists, **When** the tax professional views the client list, **Then** the SSN/EIN is partially masked (e.g., XXX-XX-1234) for security.
4. **Given** a tax professional, **When** they attempt to add a client with a duplicate SSN/EIN within their organization, **Then** the system warns them and prevents duplicate entry.

---

### User Story 2 - Tax Professional Initiates 8821 Authorization (Priority: P2)

A tax professional needs to generate and send an IRS Form 8821 authorization request to their client for e-signature, which will authorize access to the client's tax transcripts.

**Why this priority**: The 8821 authorization is the legal gateway to accessing IRS transcripts. Without this, transcript retrieval cannot proceed.

**Independent Test**: Can be fully tested by generating an 8821 form for an existing client and verifying the form is correctly populated and sent for signature.

**Acceptance Scenarios**:

1. **Given** an existing client record, **When** the tax professional selects "Request Authorization" and specifies the tax years needed, **Then** an 8821 form is generated with client and provider information pre-filled.
2. **Given** a generated 8821 form, **When** the system sends the authorization request, **Then** the client receives a notification (email) with a secure link to review and sign.
3. **Given** an 8821 request has been sent, **When** the tax professional views the client record, **Then** they see the authorization status as "Pending Signature" with the date sent.
4. **Given** an 8821 request, **When** the client has not responded within 7 days, **Then** the system displays an "Awaiting Response" indicator and allows resending.

---

### User Story 3 - Client Signs 8821 Authorization (Priority: P3)

A taxpayer client needs to review and electronically sign the IRS Form 8821 to authorize their tax professional to access their transcript information.

**Why this priority**: The client signature completes the authorization chain. This is a compliance requirement.

**Independent Test**: Can be fully tested by accessing a signature link as a client, completing the e-signature flow, and verifying the signed document is stored.

**Acceptance Scenarios**:

1. **Given** a client with a pending 8821 request, **When** they access the secure signature link, **Then** they see the pre-filled 8821 form with their information for review.
2. **Given** a client reviewing the 8821, **When** they provide their e-signature and submit, **Then** the form is digitally signed, timestamped, and stored securely.
3. **Given** a successfully signed 8821, **When** the signature is completed, **Then** the tax professional receives notification that the authorization was signed.
4. **Given** a signed 8821, **When** viewing the authorization record, **Then** both parties can download a copy of the signed form.

---

### User Story 4 - Tax Professional Views Authorization Status Dashboard (Priority: P4)

A tax professional needs to see the status of all their client authorizations at a glance to manage their workflow and identify clients requiring follow-up.

**Why this priority**: Operational visibility enables efficient workflow management across many clients.

**Independent Test**: Can be fully tested by creating multiple clients with different authorization statuses and verifying the dashboard correctly displays and filters them.

**Acceptance Scenarios**:

1. **Given** a tax professional with multiple clients, **When** they view the dashboard, **Then** they see summary counts by status: Pending, Signed, Submitted to IRS, Active, Expired.
2. **Given** the authorization dashboard, **When** they filter by status (e.g., "Pending Signature"), **Then** only clients matching that status are displayed.
3. **Given** clients with authorizations expiring within 30 days, **When** viewing the dashboard, **Then** these are highlighted with an "Expiring Soon" indicator.
4. **Given** the dashboard, **When** they search by client name or SSN/EIN (last 4 digits), **Then** matching clients are displayed regardless of status.

---

### User Story 5 - Administrator Manages Service Provider Organization (Priority: P5)

An administrator needs to set up and manage their organization (service provider firm), including inviting team members and configuring organization-level settings.

**Why this priority**: Multi-user organizations need proper setup before teams can collaborate on client management.

**Independent Test**: Can be fully tested by creating an organization, inviting a user, and verifying the invited user can log in with appropriate permissions.

**Acceptance Scenarios**:

1. **Given** a new organization signup, **When** the admin completes registration, **Then** the organization is created with the admin as the owner.
2. **Given** an organization admin, **When** they invite a team member by email and role, **Then** the invitee receives an invitation to join the organization.
3. **Given** an invited user, **When** they accept the invitation and create their account, **Then** they can access the organization's clients based on their assigned role.
4. **Given** an organization with multiple users, **When** the admin views the team, **Then** they see all members with their roles and can modify or remove access.

---

### User Story 6 - System Securely Stores Uploaded Transcripts (Priority: P6)

The system needs to securely receive and store IRS transcripts that have been manually uploaded (Phase 1 uses manual upload before full IRS API integration).

**Why this priority**: Transcript storage is the foundation for future analysis modules. Manual upload enables demo functionality before IRS integration.

**Independent Test**: Can be fully tested by uploading a transcript file for a client and verifying it is stored, encrypted, and retrievable.

**Acceptance Scenarios**:

1. **Given** a client with a signed 8821, **When** the tax professional uploads a transcript file (PDF or XML), **Then** the file is encrypted and stored with metadata linking it to the client and tax year.
2. **Given** an uploaded transcript, **When** viewing the client record, **Then** the transcript appears in the document list with upload date, tax year, and transcript type.
3. **Given** multiple transcripts for a client, **When** the tax professional selects a transcript, **Then** they can view or download the document.
4. **Given** a transcript upload, **When** the file format is invalid or corrupt, **Then** the system rejects the upload with a clear error message.

---

### Edge Cases

- What happens when a client's SSN/EIN is changed due to data entry error? System MUST allow correction with audit trail.
- How does the system handle an 8821 authorization that expires mid-process? System MUST notify relevant parties and block transcript access.
- What happens when a tax professional leaves an organization? Their clients MUST remain accessible to other authorized team members.
- How does the system handle concurrent edits to the same client record? System MUST use optimistic locking and notify the second user.
- What happens when a client requests deletion of their data? System MUST comply within 30 days per compliance requirements.
- How does the system handle duplicate 8821 requests for the same client/year? System MUST warn and allow override if intentional.

## Requirements *(mandatory)*

### Functional Requirements

**Client Management**
- **FR-001**: System MUST allow tax professionals to create client records with: legal name, SSN (individuals) or EIN (businesses), mailing address, email, and phone.
- **FR-002**: System MUST validate SSN format (XXX-XX-XXXX) and EIN format (XX-XXXXXXX) on entry.
- **FR-003**: System MUST encrypt all PII (SSN, EIN, financial data) at rest using industry-standard encryption.
- **FR-004**: System MUST mask sensitive identifiers in the UI, showing only the last 4 digits.
- **FR-005**: System MUST prevent duplicate clients within an organization based on SSN/EIN.
- **FR-006**: System MUST maintain a complete audit log of all client record changes.

**Authorization (8821) Management**
- **FR-007**: System MUST generate IRS Form 8821 with pre-populated client and provider information.
- **FR-008**: System MUST support specifying multiple tax years (up to 4 years per IRS rules) on a single 8821.
- **FR-009**: System MUST send authorization requests to clients via email with secure, time-limited links. Link expiration period MUST be configurable per organization (e.g., 24 hours, 7 days, 30 days) with a default of 7 days.
- **FR-010**: System MUST capture legally compliant e-signatures with timestamp and IP address, supporting both: (a) built-in simple e-signature (typed name, checkbox, timestamp/IP), and (b) third-party e-signature service integration (DocuSign, Adobe Sign). Organizations can configure their preferred method.
- **FR-011**: System MUST store signed 8821 forms in encrypted storage with 7-year retention.
- **FR-012**: System MUST track authorization status: Draft, Pending Signature, Signed, Submitted to CAF, Active, Expired, Revoked.
- **FR-013**: System MUST notify tax professionals when authorization status changes via email and in-app notifications (dashboard alerts, badge counts). Architecture MUST support SMS integration for future implementation.
- **FR-014**: System MUST calculate and display 8821 expiration dates (typically 3 years from signature).

**Transcript Management**
- **FR-015**: System MUST accept manual transcript uploads in PDF and XML formats.
- **FR-016**: System MUST validate that uploaded transcripts are associated with clients who have active authorizations.
- **FR-017**: System MUST store transcripts in encrypted cloud storage with metadata: client ID, tax year, transcript type, upload date.
- **FR-018**: System MUST allow authorized users to view and download stored transcripts.
- **FR-019**: System MUST support the following transcript types: Account Transcript, Return Transcript, Record of Account, Wage & Income.

**Organization & User Management**
- **FR-020**: System MUST support multi-tenant organizations (service provider firms).
- **FR-021**: System MUST implement role-based access control with at minimum: Admin, Tax Professional, Read-Only roles.
- **FR-022**: System MUST allow organization admins to invite, modify, and remove team members.
- **FR-023**: System MUST authenticate users via secure identity provider with multi-factor authentication support.
- **FR-024**: System MUST log all authentication events and data access for compliance.

**Dashboard & Navigation**
- **FR-025**: System MUST provide a dashboard showing client and authorization summary statistics.
- **FR-026**: System MUST allow filtering and searching clients by name, identifier, and status.
- **FR-027**: System MUST display navigation for future analysis modules as disabled/placeholder items until enabled.
- **FR-028**: System MUST support responsive design for desktop and tablet devices.

### Key Entities

- **Organization**: A service provider firm (CPA firm, tax resolution company, lender). Has name, contact info, subscription status. Contains multiple Users and Clients.

- **User**: A person who uses the system. Belongs to one Organization. Has email, name, role (Admin/TaxProfessional/ReadOnly), authentication credentials. Can manage multiple Clients.

- **Client (Taxpayer)**: An individual or business whose tax information is being managed. Uses a unified model with a type flag (Individual/Business). Has: legal name, type (Individual/Business), SSN (if individual) or EIN (if business), address, contact info. Business clients have additional conditional fields: business name, entity type, responsible party. Belongs to one Organization. Has multiple Authorizations and Transcripts.

- **Authorization**: An IRS Form 8821 authorization record. Links a Client to a tax professional. Has status, tax years covered, signature data, expiration date. Enables access to Transcripts.

- **Transcript**: A stored IRS transcript document. Belongs to a Client. Has tax year, transcript type, storage reference, upload/download timestamps.

- **AuditLog**: A record of system events for compliance. Captures user, action, timestamp, affected entity, before/after values.

## Assumptions

- Users will authenticate via Auth0/OCTA as specified in the architecture (README.md).
- Multi-tenant architecture with full organization isolation will be built from day one (Phase 1), even if initially deployed with a single customer.
- Manual transcript upload is acceptable for Phase 1; IRS TDS API integration will come in Phase 2.
- E-signature capture will meet IRS requirements for electronic signatures on Form 8821.
- The CargoMax dashboard theme provides sufficient UI components for the required functionality.
- Email delivery for authorization requests will use a reliable transactional email service.
- 7-year data retention is the default based on IRS requirements unless client requests deletion.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Tax professionals can add a new client and complete data entry in under 2 minutes.
- **SC-002**: Clients can review and sign an 8821 authorization in under 5 minutes from receiving the email.
- **SC-003**: System handles 100 concurrent users without degradation in response times.
- **SC-004**: 99.9% of authorization status changes trigger notifications within 1 minute.
- **SC-005**: All sensitive data (SSN, EIN, transcripts) is encrypted at rest and verified via security audit.
- **SC-006**: 95% of users successfully complete client onboarding on first attempt without support.
- **SC-007**: Dashboard loads and displays client summary within 2 seconds for organizations with up to 1,000 clients.
- **SC-008**: System maintains complete audit trail for 100% of data access and modification events.
- **SC-009**: Tax professionals can locate any client via search within 3 seconds.
- **SC-010**: Zero unauthorized access incidents during pilot period (measured via security monitoring).

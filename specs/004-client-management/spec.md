# Feature Specification: Client Management

**Feature Branch**: `004-client-management`
**Created**: 2026-02-06
**Status**: Draft
**Input**: User description: "Implement basic client management in the frontend and backend with individual and business taxpayer types, PII encryption/masking, and role-based access control"

## Clarifications

### Session 2026-02-06

- Q: Should we rename the existing `TaxProfessional` enum value to `Manager`, or keep `TaxProfessional` and update the spec? → A: Keep `TaxProfessional` - use this term consistently (domain-appropriate, no migration needed)

---

## User Scenarios & Testing *(mandatory)*

### User Story 1 - View Client List (Priority: P1) 🎯 MVP

Tax professionals need to see all their organization's clients in a searchable, sortable list to quickly find and access client information.

**Why this priority**: Viewing existing clients is the most fundamental operation. Without this, users cannot interact with any client data. This establishes the navigation patterns and list views that all other features build upon.

**Independent Test**: Navigate to the Clients section, verify the list displays all clients for the organization with proper sorting, search, and pagination working correctly.

**Acceptance Scenarios**:

1. **Given** a user with ReadOnly, TaxProfessional, or Admin role, **When** they navigate to the Clients section, **Then** they see a list of all clients belonging to their organization
2. **Given** a client list with 50+ clients, **When** the user views the list, **Then** pagination controls appear and work correctly
3. **Given** a client list, **When** the user types in the search box, **Then** results filter by client name, email, or last 4 digits of tax identifier
4. **Given** a client list, **When** the user clicks column headers, **Then** the list sorts by that column (name, type, created date)
5. **Given** a user viewing the client list, **When** displayed, **Then** only the last 4 digits of tax identifiers are visible (SSN: ***-**-1234, EIN: **-***1234)

---

### User Story 2 - Create Individual Client (Priority: P2)

Tax professionals need to add new individual taxpayer clients with their personal information and Social Security Number.

**Why this priority**: Creating clients is essential for onboarding new taxpayers. Individual clients are the most common client type for tax professionals.

**Independent Test**: Click "Add Client", select Individual type, fill in all required fields including SSN, save, and verify the client appears in the list with SSN properly masked.

**Acceptance Scenarios**:

1. **Given** a user with TaxProfessional or Admin role, **When** they click "Add Client" and select "Individual", **Then** they see a form with fields for first name, last name, SSN, email, phone, and address
2. **Given** an individual client form, **When** the user enters an SSN, **Then** it is masked as typed (showing ***-**-XXXX after entry) and only the last 4 digits are stored unencrypted
3. **Given** all required fields are completed with valid data, **When** the user saves, **Then** the client is created and appears in the client list
4. **Given** a user with ReadOnly role, **When** they view the Clients page, **Then** they do not see the "Add Client" button
5. **Given** an individual client form, **When** the user enters a duplicate SSN for the organization, **Then** a warning is displayed before saving

---

### User Story 3 - Create Business Client (Priority: P2)

Tax professionals need to add new business entity clients with their business information and Employer Identification Number (EIN).

**Why this priority**: Business clients are equally important as individual clients. Many tax professionals serve both types.

**Independent Test**: Click "Add Client", select Business type, fill in all required fields including EIN, save, and verify the client appears in the list.

**Acceptance Scenarios**:

1. **Given** a user with TaxProfessional or Admin role, **When** they click "Add Client" and select "Business", **Then** they see a form with fields for business name, entity type, responsible party, EIN, email, phone, and address
2. **Given** a business client form, **When** the user enters an EIN, **Then** it is masked appropriately and only the last 4 digits are stored unencrypted
3. **Given** a business client form, **When** the user selects entity type, **Then** options include: Sole Proprietorship, Partnership, S-Corporation, C-Corporation, LLC, Non-Profit, Trust, Estate
4. **Given** all required fields are completed with valid data, **When** the user saves, **Then** the business client is created successfully

---

### User Story 4 - View and Edit Client Details (Priority: P3)

Tax professionals need to view complete client information and update it when contact details or addresses change.

**Why this priority**: After creating clients, users need to maintain accurate information. Viewing and editing are core CRUD operations.

**Independent Test**: Click on a client in the list, view all details with PII properly masked, edit contact information, save changes, and verify updates persist.

**Acceptance Scenarios**:

1. **Given** any authenticated user, **When** they click on a client row, **Then** they navigate to the client detail page showing all client information
2. **Given** a client detail page, **When** displayed, **Then** the full tax identifier is never shown - only last 4 digits with masking (SSN: ***-**-1234)
3. **Given** a user with TaxProfessional or Admin role viewing client details, **When** they click "Edit", **Then** they can modify contact information, address, and notes
4. **Given** a user with ReadOnly role viewing client details, **When** they view the page, **Then** they cannot see Edit buttons or modify any data
5. **Given** a user editing an individual client, **When** they save changes, **Then** an audit log entry records who made the change and when

---

### User Story 5 - Role-Based Access Control (Priority: P3)

Organization administrators need to control what actions different team members can perform on client data.

**Why this priority**: Security and data governance require proper access controls. This protects sensitive PII from unauthorized access or modification.

**Independent Test**: Create users with different roles, verify each role can only perform authorized actions on client data.

**Acceptance Scenarios**:

1. **Given** a user with Admin role, **When** they access client management, **Then** they can view, create, edit, and delete clients
2. **Given** a user with TaxProfessional role, **When** they access client management, **Then** they can view, create, and edit clients but cannot delete
3. **Given** a user with ReadOnly role, **When** they access client management, **Then** they can only view client information (no create, edit, or delete)
4. **Given** any role, **When** they access clients, **Then** they can only see clients belonging to their organization (tenant isolation enforced)

---

### User Story 6 - Delete/Archive Client (Priority: P4)

Administrators need to remove clients that are no longer active while maintaining audit history.

**Why this priority**: Lower priority as deletion is less common. Soft-delete preserves data integrity and audit trails.

**Independent Test**: As Admin, archive a client, verify it no longer appears in the active list but can be found in archived view.

**Acceptance Scenarios**:

1. **Given** a user with Admin role, **When** they click "Archive" on a client, **Then** a confirmation dialog appears
2. **Given** an archived client, **When** viewing the client list, **Then** the archived client does not appear by default
3. **Given** a user with Admin role, **When** they toggle "Show Archived", **Then** archived clients appear with visual distinction
4. **Given** an archived client, **When** an Admin views it, **Then** they can restore it to active status

---

### Edge Cases

- What happens when a user tries to create a client with an invalid SSN format? → Form validation rejects with clear error message
- What happens when a user tries to view a client from another organization? → Access denied, returns 404 to prevent enumeration
- What happens when the encryption service is unavailable? → Client creation fails gracefully with user-friendly error, no partial data saved
- How does the system handle concurrent edits to the same client? → Last write wins with optimistic concurrency, conflicts show warning
- What happens when searching for clients with special characters? → Search sanitizes input and handles gracefully
- What if a user's role changes while they're editing a client? → Changes validated server-side on save, unauthorized actions rejected

## Requirements *(mandatory)*

### Functional Requirements

**Client Data Management**
- **FR-001**: System MUST support two client types: Individual (person with SSN) and Business (entity with EIN)
- **FR-002**: System MUST store tax identifiers (SSN/EIN) using encryption at rest
- **FR-003**: System MUST only store the last 4 digits of tax identifiers in unencrypted form for display/search
- **FR-004**: System MUST mask tax identifiers in all user interfaces (SSN: ***-**-1234, EIN: **-***1234)
- **FR-005**: System MUST validate SSN format (XXX-XX-XXXX) and EIN format (XX-XXXXXXX) before saving

**Individual Client Fields**
- **FR-006**: Individual clients MUST have: first name, last name, SSN (encrypted), email, and address
- **FR-007**: Individual clients MAY have: phone number and notes

**Business Client Fields**
- **FR-008**: Business clients MUST have: business name, entity type, EIN (encrypted), email, and address
- **FR-009**: Business clients MAY have: responsible party name, phone number, and notes
- **FR-010**: Business entity types MUST include: Sole Proprietorship, Partnership, S-Corporation, C-Corporation, LLC, Non-Profit, Trust, Estate

**Role-Based Access Control**
- **FR-011**: System MUST enforce three permission levels: Admin (full access), TaxProfessional (view/create/edit), ReadOnly (view only)
- **FR-012**: Admin role MUST be able to perform all client operations including archive/restore
- **FR-013**: TaxProfessional role MUST be able to view, create, and edit clients but NOT archive/delete
- **FR-014**: ReadOnly role MUST only be able to view client information with no modification capabilities
- **FR-015**: System MUST enforce role permissions on both frontend (UI controls) and backend (API validation)

**Data Security**
- **FR-016**: System MUST enforce tenant isolation - users can only access clients within their organization
- **FR-017**: System MUST log all client data modifications in an audit trail with user, timestamp, and changes
- **FR-018**: System MUST never transmit full SSN/EIN to the frontend after initial creation
- **FR-019**: System MUST warn users when creating a client with a tax identifier that already exists in the organization

**User Interface**
- **FR-020**: Client list MUST support pagination with configurable page sizes (10, 25, 50, 100)
- **FR-021**: Client list MUST support sorting by name, type, email, and created date
- **FR-022**: Client list MUST support text search across name, email, and last 4 of tax identifier
- **FR-023**: User interface MUST follow CargoMax mentor UI patterns for layout, navigation, forms, and tables

**Soft Delete**
- **FR-024**: System MUST implement soft delete (archive) rather than permanent deletion
- **FR-025**: Archived clients MUST be hidden from default views but recoverable by Admin users

### Key Entities

- **Client**: Represents a taxpayer (individual or business) managed by the organization. Contains identity information, contact details, and tax identifier. Belongs to one Organization and is created by a User.

- **Address**: Value object containing street address, city, state, postal code, and country for client mailing information.

- **EncryptedString**: Value object that holds encrypted PII data (tax identifiers) and handles encryption/decryption transparently.

- **User Role**: Enumeration defining access levels - Admin (full access), TaxProfessional (view/create/edit), ReadOnly (view only).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can create a new individual or business client in under 2 minutes
- **SC-002**: Users can find any client using search in under 5 seconds
- **SC-003**: 100% of tax identifiers are encrypted at rest with no plain text storage
- **SC-004**: 0% of API responses contain full (unmasked) tax identifiers after client creation
- **SC-005**: Client list loads within 2 seconds for organizations with up to 1,000 clients
- **SC-006**: Role-based access controls correctly restrict 100% of unauthorized actions
- **SC-007**: All client modifications are captured in audit log with 100% coverage
- **SC-008**: Users report the interface is intuitive with less than 5 minutes of training needed

## Assumptions

- The existing domain model for Client entity provides the foundation and will be used/extended
- The existing EncryptedString value object and encryption service are functional and will handle PII encryption
- CargoMax UI patterns are documented and accessible for reference during implementation
- The existing UserRole enum (ReadOnly, TaxProfessional, Admin) provides the required permission levels
- PostgreSQL Row-Level Security (implemented in 003-postgres-migration) will enforce tenant isolation at the database level
- Frontend will be built using the existing Next.js application structure
- Backend API follows existing patterns in the TranscriptAnalyzer solution

## Out of Scope

- Bulk import of clients from external sources (CSV, Excel)
- Client document storage and management
- Client portal for self-service access
- Integration with IRS e-Services for client verification
- Client relationship management (linking related individuals/businesses)
- Historical client data migration from other systems
- Multi-organization client sharing
- Client merge/duplicate resolution workflows

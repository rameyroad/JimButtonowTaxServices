# Feature Specification: White-Label Branding

**Feature Branch**: `006-white-label-branding`
**Created**: 2026-02-07
**Status**: Draft
**Input**: White-label branding support with custom logos, brand colors, custom domains, and full tenant data isolation for subscriber firms.

## Overview

This feature enables subscriber firms (tax professional organizations) to fully brand the platform as their own. Subscribers can customize the visual appearance with their logos and colors, and optionally host the application behind their own custom domain. Full tenant data isolation ensures each subscriber's data is completely separated from others.

### Business Value

- **Brand ownership**: Subscribers present the platform as their own product to clients
- **Professional appearance**: Clients see a consistent brand experience from their tax professional
- **Trust**: Custom domains reinforce subscriber's professional identity
- **Security**: Full data isolation prevents any cross-tenant data exposure

### Relationship to Workflow Engine (005)

This feature extends the subscriber model defined in 005-workflow-engine:
- Subscribers already exist as organizations using the platform
- This adds branding configuration and custom domain support per subscriber
- Tenant isolation is enhanced from logical separation to full isolation

## Clarifications

### Session 2026-02-07

- Q: How should authentication work when subscribers use custom domains? → A: Single Auth0 tenant with domain allowlist - one Auth0 config, allow callbacks from all custom domains

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Configure Brand Identity (Priority: P1)

As a **Subscriber Admin**, I want to upload my firm's logo and set brand colors so that the platform reflects my company's identity when my staff and clients use it.

**Why this priority**: Brand identity is the core value proposition of white-labeling. Without custom branding, there's no differentiation from the default platform appearance.

**Independent Test**: Can be fully tested by uploading a logo, setting primary/secondary colors, and verifying they appear throughout the subscriber's portal.

**Acceptance Scenarios**:

1. **Given** a Subscriber Admin is in their settings, **When** they upload a logo image, **Then** the logo replaces the default platform logo in the header and login screens.

2. **Given** a Subscriber Admin sets primary and secondary brand colors, **When** their team or clients view any page, **Then** buttons, links, and accents use the configured colors.

3. **Given** a Subscriber Admin updates their branding, **When** any user refreshes the page, **Then** they see the new branding immediately.

4. **Given** a logo is uploaded, **When** viewing on different devices, **Then** the logo scales appropriately for desktop, tablet, and mobile.

---

### User Story 2 - Client-Facing Branded Experience (Priority: P1)

As a **Client** (taxpayer), I want to see my tax professional's branding when I access the platform so that I trust I'm in the right place and have a seamless experience with their firm.

**Why this priority**: Client trust is essential. When clients receive email links and access the platform, consistent branding confirms they're interacting with their chosen tax professional.

**Independent Test**: Can be tested by having a client access approval/upload links and verifying they see the subscriber's branding, not the platform's default.

**Acceptance Scenarios**:

1. **Given** a client receives an email link from their tax professional's case, **When** they click the link, **Then** the page displays the subscriber's logo and brand colors.

2. **Given** a subscriber has custom branding configured, **When** a client uploads documents, **Then** the upload interface shows the subscriber's branding.

3. **Given** a subscriber has NOT configured custom branding, **When** a client accesses the platform, **Then** they see a neutral/generic default appearance.

---

### User Story 3 - Configure Custom Domain (Priority: P2)

As a **Subscriber Admin**, I want to use my own domain (e.g., portal.myTaxFirm.com) so that clients access the platform through my branded URL.

**Why this priority**: Custom domains reinforce brand ownership but require DNS configuration and SSL certificates. Lower priority than basic branding because it has external dependencies.

**Independent Test**: Can be tested by configuring a custom domain, updating DNS, and verifying the platform loads correctly at that domain with proper SSL.

**Acceptance Scenarios**:

1. **Given** a Subscriber Admin enters their custom domain, **When** the system validates it, **Then** they receive DNS configuration instructions.

2. **Given** DNS is properly configured, **When** the system verifies the domain, **Then** the domain is marked as active and SSL is provisioned.

3. **Given** a custom domain is active, **When** anyone accesses that domain, **Then** they see the subscriber's branded portal.

4. **Given** a subscriber has a custom domain, **When** email links are sent to clients, **Then** the links use the custom domain URL.

---

### User Story 4 - Full Tenant Data Isolation (Priority: P1)

As a **Platform Admin**, I want each subscriber's data to be completely isolated so that there is zero possibility of data leakage between subscribers.

**Why this priority**: Data isolation is a security and compliance requirement. Tax data is highly sensitive; any cross-tenant exposure would be catastrophic.

**Independent Test**: Can be tested by attempting to access another subscriber's data through various means (API, direct queries, etc.) and verifying all attempts fail.

**Acceptance Scenarios**:

1. **Given** two subscribers exist, **When** Subscriber A queries for clients, **Then** they receive only their own clients with no visibility into Subscriber B's data.

2. **Given** a Subscriber User attempts to access a client ID belonging to another subscriber, **When** the request is processed, **Then** it returns "not found" (not "access denied" to avoid confirming existence).

3. **Given** database queries are executed, **When** reviewing query logs, **Then** all queries include tenant filtering automatically.

4. **Given** a Platform Admin views aggregate metrics, **When** they drill into details, **Then** they cannot see individual subscriber's client data.

---

### User Story 5 - Subscriber Admin Branding Preview (Priority: P2)

As a **Subscriber Admin**, I want to preview my branding changes before they go live so that I can ensure they look correct.

**Why this priority**: Prevents embarrassing mistakes where incorrect branding is visible to clients. Important for user experience but not blocking core functionality.

**Independent Test**: Can be tested by making branding changes, viewing preview, then either publishing or discarding.

**Acceptance Scenarios**:

1. **Given** a Subscriber Admin is editing branding, **When** they click "Preview", **Then** they see a preview of how the branding will appear without affecting live users.

2. **Given** a Subscriber Admin is satisfied with preview, **When** they click "Publish", **Then** the new branding becomes live for all users.

3. **Given** a Subscriber Admin is not satisfied with preview, **When** they click "Discard", **Then** changes are discarded and current branding remains unchanged.

---

### Edge Cases

- What if a subscriber uploads an invalid or corrupted image file? (Validate file type and dimensions, reject with helpful error)
- What if brand colors have poor contrast? (Warn but allow; don't enforce accessibility)
- What happens if DNS for custom domain is misconfigured? (Show clear error status with troubleshooting steps)
- What if a custom domain's SSL certificate expires? (Auto-renew; alert subscriber if renewal fails)
- What if two subscribers try to claim the same custom domain? (First verified claim wins; reject duplicates)
- How is branding handled in transactional emails? (Apply subscriber's logo and colors to email templates)

## Requirements *(mandatory)*

### Functional Requirements

#### Brand Configuration
- **FR-001**: System MUST allow Subscriber Admins to upload a logo image (PNG, JPG, SVG)
- **FR-002**: System MUST allow Subscriber Admins to configure primary and secondary brand colors
- **FR-003**: System MUST validate logo file type and maximum dimensions/file size
- **FR-004**: System MUST apply branding to all subscriber-facing pages (login, dashboard, forms, etc.)
- **FR-005**: System MUST apply subscriber branding to client-facing pages accessed via secure links
- **FR-006**: System MUST provide a default/neutral appearance for subscribers without custom branding

#### Custom Domains
- **FR-010**: System MUST allow Subscriber Admins to register a custom domain
- **FR-011**: System MUST provide DNS configuration instructions (CNAME or A record)
- **FR-012**: System MUST verify domain ownership via DNS lookup
- **FR-013**: System MUST provision SSL certificates for verified custom domains
- **FR-014**: System MUST route requests from custom domains to the correct subscriber
- **FR-015**: System MUST use custom domain URLs in email links when configured
- **FR-016**: System MUST handle SSL certificate renewal automatically
- **FR-017**: System MUST add verified custom domains to Auth0 allowed callback URLs

#### Tenant Isolation
- **FR-020**: System MUST enforce tenant isolation at the data layer for all queries
- **FR-021**: System MUST prevent any API endpoint from returning another subscriber's data
- **FR-022**: System MUST log all data access with tenant context for audit purposes
- **FR-023**: System MUST return "not found" (not "forbidden") for cross-tenant access attempts
- **FR-024**: System MUST validate tenant context on every authenticated request

#### Branding Preview
- **FR-030**: System MUST provide a preview mode for branding changes
- **FR-031**: System MUST allow publishing or discarding preview changes
- **FR-032**: System MUST not affect live users until branding is published

### Key Entities

- **SubscriberBranding**: Logo URL, primary color, secondary color, favicon, linked to Subscriber
- **CustomDomain**: Domain name, verification status, SSL status, linked to Subscriber
- **BrandingDraft**: Unpublished branding changes for preview, linked to Subscriber
- **Subscriber**: Extended with branding and domain references

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Subscriber Admins can configure branding (logo + colors) in under 5 minutes
- **SC-002**: 100% of client-facing pages display correct subscriber branding when configured
- **SC-003**: Custom domain setup completes within 24 hours of DNS configuration (SSL provisioning)
- **SC-004**: Zero cross-tenant data access incidents (verified by security audit)
- **SC-005**: 95% of branding changes are previewed before publishing
- **SC-006**: Email links use custom domain for 100% of subscribers with verified domains
- **SC-007**: Page load time increases by no more than 100ms due to branding customization

## Assumptions

- Subscribers have access to their DNS management to configure custom domains
- SSL certificates will be provisioned via automated service (e.g., Let's Encrypt approach)
- Logo files will be stored in cloud storage and served via CDN
- Default branding will be minimal/neutral to not conflict with any subscriber's brand
- Transactional emails support template customization with subscriber branding
- Current tenant isolation via database query filters is the baseline; this may require enhancement

## Constraints

- Must integrate with existing subscriber model from 005-workflow-engine
- Must not significantly impact page load performance (max 100ms increase)
- Custom domain SSL must be automated (no manual certificate management)
- Must work with existing Auth0 integration using single tenant with domain allowlist for custom domains
- Must support responsive design (branding works on mobile, tablet, desktop)

## Dependencies

- **005-workflow-engine**: Subscriber entity and multi-tier tenancy model
- **Cloud storage**: For logo and asset storage
- **DNS/SSL automation**: For custom domain verification and certificate provisioning
- **Email service**: Must support branded email templates

export interface RegisterOrganizationRequest {
  organizationName: string;
  contactEmail: string;
  street1: string;
  street2?: string;
  city: string;
  state: string;
  postalCode: string;
  adminFirstName: string;
  adminLastName: string;
  adminEmail: string;
}

export interface RegisterOrganizationResult {
  organizationId: string;
  userId: string;
  organizationName: string;
}

export interface AcceptInvitationRequest {
  firstName: string;
  lastName: string;
}

export interface AcceptInvitationResult {
  organizationId: string;
  userId: string;
  email: string;
}

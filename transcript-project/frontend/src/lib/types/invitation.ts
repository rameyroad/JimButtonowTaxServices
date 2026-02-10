export type UserRole = 'Admin' | 'Preparer' | 'Reviewer' | 'PlatformAdmin';

export type InvitationStatus = 'Pending' | 'Accepted' | 'Expired' | 'Revoked';

export interface InvitationDto {
  id: string;
  email: string;
  role: UserRole;
  status: InvitationStatus;
  expiresAt: string;
  acceptedAt?: string;
  invitedByUserId: string;
  createdAt: string;
}

export interface CreateInvitationRequest {
  email: string;
  role: UserRole;
}

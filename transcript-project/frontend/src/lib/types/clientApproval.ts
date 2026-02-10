export type ClientApprovalStatus = 'Pending' | 'Approved' | 'Declined' | 'Expired';

export interface ClientApprovalDto {
  id: string;
  title: string;
  description?: string;
  status: ClientApprovalStatus;
  tokenExpiresAt: string;
  respondedAt?: string;
  responseNotes?: string;
  createdAt: string;
}

export interface RespondToApprovalRequest {
  approved: boolean;
  notes?: string;
}

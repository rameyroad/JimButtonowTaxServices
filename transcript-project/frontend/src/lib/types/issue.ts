export type IssueType =
  | 'BalanceDue'
  | 'Penalty'
  | 'UnfiledReturn'
  | 'StatuteExpiration'
  | 'PaymentPlan'
  | 'Lien'
  | 'Levy';

export type IssueSeverity = 'Low' | 'Medium' | 'High' | 'Critical';

export interface IssueListItem {
  id: string;
  clientId: string;
  issueType: IssueType;
  severity: IssueSeverity;
  taxYear: number;
  amount?: number;
  description: string;
  transactionCode?: string;
  detectedAt: string;
  resolvedAt?: string;
  createdAt: string;
}

export interface IssueDetail {
  id: string;
  clientId: string;
  caseWorkflowId?: string;
  issueType: IssueType;
  severity: IssueSeverity;
  taxYear: number;
  amount?: number;
  description: string;
  transactionCode?: string;
  detectedAt: string;
  resolvedAt?: string;
  resolvedByUserId?: string;
  createdAt: string;
  updatedAt: string;
}

export interface UpdateIssueRequest {
  severity?: IssueSeverity;
  description?: string;
  amount?: number;
  resolve?: boolean;
}

export interface ListIssuesParams {
  clientId: string;
  page?: number;
  pageSize?: number;
  issueType?: IssueType;
  severity?: IssueSeverity;
  taxYear?: number;
  resolved?: boolean;
  sortBy?: 'severity' | 'taxYear' | 'amount' | 'issueType' | 'detectedAt';
  sortOrder?: 'asc' | 'desc';
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

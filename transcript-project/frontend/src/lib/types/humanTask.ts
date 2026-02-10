export type HumanTaskStatus =
  | 'Pending'
  | 'InProgress'
  | 'Completed'
  | 'Reassigned'
  | 'Escalated';

export interface HumanTaskListItem {
  id: string;
  caseWorkflowId: string;
  stepExecutionId: string;
  assignedToUserId?: string;
  title: string;
  dueDate?: string;
  status: HumanTaskStatus;
  completedAt?: string;
  decision?: string;
  createdAt: string;
}

export interface HumanTaskDetail {
  id: string;
  caseWorkflowId: string;
  stepExecutionId: string;
  assignedToUserId?: string;
  title: string;
  description?: string;
  dueDate?: string;
  status: HumanTaskStatus;
  completedAt?: string;
  completedByUserId?: string;
  decision?: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CompleteHumanTaskRequest {
  decision?: string;
  notes?: string;
}

export interface ReassignHumanTaskRequest {
  assignedToUserId: string;
}

export interface ListHumanTasksParams {
  page?: number;
  pageSize?: number;
  status?: HumanTaskStatus;
  assignedToUserId?: string;
  sortBy?: 'createdAt' | 'status' | 'dueDate' | 'title';
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

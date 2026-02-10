import type { WorkflowExecutionStatus, StepExecutionStatus, WorkflowStepType } from './workflow';

export interface CaseWorkflowListItem {
  id: string;
  clientId: string;
  workflowName: string;
  workflowVersion: number;
  status: WorkflowExecutionStatus;
  startedAt?: string;
  completedAt?: string;
  totalSteps: number;
  completedSteps: number;
  createdAt: string;
}

export interface StepExecution {
  id: string;
  workflowStepId: string;
  stepName: string;
  stepType: WorkflowStepType;
  status: StepExecutionStatus;
  inputData?: string;
  outputData?: string;
  startedAt?: string;
  completedAt?: string;
  errorMessage?: string;
}

export interface CaseWorkflowDetail {
  id: string;
  clientId: string;
  workflowDefinitionId: string;
  workflowName: string;
  workflowVersion: number;
  workflowVersionId?: string;
  status: WorkflowExecutionStatus;
  startedAt?: string;
  completedAt?: string;
  startedByUserId: string;
  currentStepId?: string;
  errorMessage?: string;
  stepExecutions: StepExecution[];
  createdAt: string;
  updatedAt: string;
}

export interface StartWorkflowRequest {
  workflowDefinitionId: string;
}

export interface CancelWorkflowRequest {
  reason?: string;
}

export interface ListCaseWorkflowsParams {
  clientId: string;
  page?: number;
  pageSize?: number;
  status?: WorkflowExecutionStatus;
  sortBy?: 'createdAt' | 'status' | 'startedAt' | 'completedAt';
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

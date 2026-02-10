import type { PublishStatus } from './decisionTable';

export type WorkflowStepType =
  | 'DecisionTable'
  | 'Calculation'
  | 'HumanTask'
  | 'ClientApproval'
  | 'DocumentGeneration';

export type WorkflowExecutionStatus =
  | 'NotStarted'
  | 'Running'
  | 'Paused'
  | 'Completed'
  | 'Failed'
  | 'Cancelled';

export type StepExecutionStatus =
  | 'Pending'
  | 'Running'
  | 'Completed'
  | 'Failed'
  | 'Skipped';

export interface WorkflowDefinitionListItem {
  id: string;
  name: string;
  description?: string;
  category?: string;
  status: PublishStatus;
  stepCount: number;
  currentVersion: number;
  publishedAt?: string;
  createdAt: string;
  updatedAt: string;
}

export interface WorkflowStep {
  id: string;
  name: string;
  stepType: WorkflowStepType;
  sortOrder: number;
  configuration?: string;
  nextStepOnSuccessId?: string;
  nextStepOnFailureId?: string;
  isRequired: boolean;
}

export interface WorkflowDefinitionDetail {
  id: string;
  name: string;
  description?: string;
  category?: string;
  status: PublishStatus;
  currentVersion: number;
  publishedAt?: string;
  publishedByUserId?: string;
  steps: WorkflowStep[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateWorkflowDefinitionRequest {
  name: string;
  description?: string;
  category?: string;
}

export interface UpdateWorkflowDefinitionRequest {
  name?: string;
  description?: string;
  category?: string;
}

export interface AddWorkflowStepRequest {
  name: string;
  stepType: WorkflowStepType;
  sortOrder: number;
  configuration?: string;
  nextStepOnSuccessId?: string;
  nextStepOnFailureId?: string;
  isRequired?: boolean;
}

export interface UpdateWorkflowStepRequest {
  name?: string;
  stepType?: WorkflowStepType;
  sortOrder?: number;
  configuration?: string;
  nextStepOnSuccessId?: string;
  nextStepOnFailureId?: string;
  isRequired?: boolean;
}

export interface ListWorkflowDefinitionsParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: PublishStatus;
  category?: string;
  sortBy?: 'name' | 'status' | 'category' | 'createdAt' | 'updatedAt' | 'publishedAt';
  sortOrder?: 'asc' | 'desc';
}

export interface WorkflowVersionListItem {
  id: string;
  workflowDefinitionId: string;
  versionNumber: number;
  publishedAt: string;
  publishedByUserId: string;
  isActive: boolean;
  createdAt: string;
}

export interface WorkflowVersionDetail extends WorkflowVersionListItem {
  snapshotData: string;
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

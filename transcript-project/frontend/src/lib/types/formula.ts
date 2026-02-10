import type { DataType, PublishStatus } from './decisionTable';

export interface FormulaVariable {
  name: string;
  dataType: DataType;
  description?: string;
}

export interface FormulaListItem {
  id: string;
  name: string;
  description?: string;
  outputType: DataType;
  status: PublishStatus;
  publishedAt?: string;
  version: number;
  createdAt: string;
  updatedAt: string;
}

export interface FormulaDetail {
  id: string;
  name: string;
  description?: string;
  expression: string;
  inputVariables: string;
  outputType: DataType;
  status: PublishStatus;
  publishedAt?: string;
  publishedByUserId?: string;
  version: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateFormulaRequest {
  name: string;
  description?: string;
  expression: string;
  inputVariables: string;
  outputType: DataType;
}

export interface UpdateFormulaRequest {
  name?: string;
  description?: string;
  expression?: string;
  inputVariables?: string;
  outputType?: DataType;
}

export interface ListFormulasParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: PublishStatus;
  sortBy?: 'name' | 'status' | 'outputType' | 'createdAt' | 'updatedAt';
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

export type PublishStatus = 'Draft' | 'Published' | 'Archived';
export type DataType = 'String' | 'Number' | 'Boolean' | 'Date';
export type ConditionOperator =
  | 'Equals'
  | 'NotEquals'
  | 'LessThan'
  | 'GreaterThan'
  | 'LessThanOrEqual'
  | 'GreaterThanOrEqual'
  | 'Between'
  | 'Contains'
  | 'IsEmpty'
  | 'IsNotEmpty';

export interface DecisionTableListItem {
  id: string;
  name: string;
  description?: string;
  status: PublishStatus;
  ruleCount: number;
  columnCount: number;
  publishedAt?: string;
  version: number;
  createdAt: string;
  updatedAt: string;
}

export interface DecisionTableColumn {
  id: string;
  name: string;
  key: string;
  dataType: DataType;
  isInput: boolean;
  sortOrder: number;
}

export interface RuleCondition {
  id: string;
  columnKey: string;
  operator: ConditionOperator;
  value: string;
  value2?: string;
}

export interface RuleOutput {
  id: string;
  columnKey: string;
  value: string;
}

export interface DecisionRule {
  id: string;
  priority: number;
  isEnabled: boolean;
  conditions: RuleCondition[];
  outputs: RuleOutput[];
}

export interface DecisionTableDetail {
  id: string;
  name: string;
  description?: string;
  status: PublishStatus;
  publishedAt?: string;
  publishedByUserId?: string;
  version: number;
  columns: DecisionTableColumn[];
  rules: DecisionRule[];
  createdAt: string;
  updatedAt: string;
}

export interface ColumnDefinition {
  name: string;
  key: string;
  dataType: DataType;
  isInput: boolean;
  sortOrder: number;
}

export interface CreateDecisionTableRequest {
  name: string;
  description?: string;
  columns: ColumnDefinition[];
}

export interface UpdateDecisionTableRequest {
  name?: string;
  description?: string;
  columns?: ColumnDefinition[];
}

export interface RuleConditionInput {
  columnKey: string;
  operator: ConditionOperator;
  value: string;
  value2?: string;
}

export interface RuleOutputInput {
  columnKey: string;
  value: string;
}

export interface AddDecisionRuleRequest {
  priority: number;
  isEnabled?: boolean;
  conditions: RuleConditionInput[];
  outputs: RuleOutputInput[];
}

export interface UpdateDecisionRuleRequest {
  priority?: number;
  isEnabled?: boolean;
  conditions?: RuleConditionInput[];
  outputs?: RuleOutputInput[];
}

export interface ListDecisionTablesParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: PublishStatus;
  sortBy?: 'name' | 'status' | 'createdAt' | 'updatedAt' | 'publishedAt';
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

export type ClientType = 'Individual' | 'Business';

export type BusinessEntityType =
  | 'SoleProprietor'
  | 'Partnership'
  | 'SCorp'
  | 'CCorp'
  | 'LLC'
  | 'NonProfit'
  | 'Trust'
  | 'Estate';

export interface Address {
  street1: string;
  street2?: string;
  city: string;
  state: string;
  postalCode: string;
  country: string;
}

export interface ClientListItem {
  id: string;
  clientType: ClientType;
  displayName: string;
  taxIdentifierLast4: string;
  taxIdentifierMasked: string;
  email: string;
  phone?: string;
  isArchived: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface Client extends ClientListItem {
  firstName?: string;
  lastName?: string;
  businessName?: string;
  entityType?: BusinessEntityType;
  responsibleParty?: string;
  address: Address;
  version: number;
}

export interface UserSummary {
  id: string;
  name: string;
  email: string;
}

export interface ClientDetail extends Client {
  notes?: string;
  createdBy?: UserSummary;
  activeAuthorizationCount: number;
  transcriptCount: number;
}

export interface CreateClientRequest {
  clientType: ClientType;
  firstName?: string;
  lastName?: string;
  businessName?: string;
  entityType?: BusinessEntityType;
  responsibleParty?: string;
  taxIdentifier: string;
  email: string;
  phone?: string;
  address: Address;
  notes?: string;
}

export interface UpdateClientRequest {
  firstName?: string;
  lastName?: string;
  businessName?: string;
  entityType?: BusinessEntityType;
  responsibleParty?: string;
  taxIdentifier?: string;
  email?: string;
  phone?: string;
  address?: Address;
  notes?: string;
  version: number;
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

export interface ListClientsParams {
  page?: number;
  pageSize?: number;
  search?: string;
  clientType?: ClientType;
  sortBy?: 'name' | 'email' | 'createdAt' | 'updatedAt';
  sortOrder?: 'asc' | 'desc';
  includeArchived?: boolean;
}

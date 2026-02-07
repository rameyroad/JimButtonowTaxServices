import { baseApi } from './baseApi';
import type {
  Client,
  ClientDetail,
  ClientListItem,
  CreateClientRequest,
  ListClientsParams,
  PaginatedResponse,
  UpdateClientRequest,
} from '@/lib/types/client';

export const clientsApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    // List clients with pagination, search, and filtering
    listClients: builder.query<PaginatedResponse<ClientListItem>, ListClientsParams>({
      query: (params) => ({
        url: '/clients',
        params: {
          page: params.page ?? 1,
          pageSize: params.pageSize ?? 20,
          search: params.search,
          clientType: params.clientType,
          sortBy: params.sortBy ?? 'name',
          sortOrder: params.sortOrder ?? 'asc',
          includeArchived: params.includeArchived,
        },
      }),
      providesTags: (result) =>
        result
          ? [
              ...result.items.map(({ id }) => ({ type: 'Client' as const, id })),
              { type: 'Client', id: 'LIST' },
            ]
          : [{ type: 'Client', id: 'LIST' }],
    }),

    // Get single client details
    getClient: builder.query<ClientDetail, string>({
      query: (id) => `/clients/${id}`,
      providesTags: (result, error, id) => [{ type: 'Client', id }],
    }),

    // Create new client
    createClient: builder.mutation<Client, CreateClientRequest>({
      query: (body) => ({
        url: '/clients',
        method: 'POST',
        body,
      }),
      invalidatesTags: [{ type: 'Client', id: 'LIST' }],
    }),

    // Update existing client
    updateClient: builder.mutation<Client, { id: string; data: UpdateClientRequest }>({
      query: ({ id, data }) => ({
        url: `/clients/${id}`,
        method: 'PATCH',
        body: data,
      }),
      invalidatesTags: (result, error, { id }) => [
        { type: 'Client', id },
        { type: 'Client', id: 'LIST' },
      ],
    }),

    // Archive client (soft delete)
    archiveClient: builder.mutation<void, string>({
      query: (id) => ({
        url: `/clients/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: (result, error, id) => [
        { type: 'Client', id },
        { type: 'Client', id: 'LIST' },
      ],
    }),

    // Restore archived client
    restoreClient: builder.mutation<Client, string>({
      query: (id) => ({
        url: `/clients/${id}/restore`,
        method: 'POST',
      }),
      invalidatesTags: (result, error, id) => [
        { type: 'Client', id },
        { type: 'Client', id: 'LIST' },
      ],
    }),
  }),
  overrideExisting: false,
});

export const {
  useListClientsQuery,
  useGetClientQuery,
  useCreateClientMutation,
  useUpdateClientMutation,
  useArchiveClientMutation,
  useRestoreClientMutation,
} = clientsApi;

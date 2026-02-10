import { baseApi } from './baseApi';
import type {
  AddDecisionRuleRequest,
  CreateDecisionTableRequest,
  DecisionRule,
  DecisionTableDetail,
  DecisionTableListItem,
  ListDecisionTablesParams,
  PaginatedResponse,
  UpdateDecisionRuleRequest,
  UpdateDecisionTableRequest,
} from '@/lib/types/decisionTable';

export const decisionTablesApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    listDecisionTables: builder.query<PaginatedResponse<DecisionTableListItem>, ListDecisionTablesParams>({
      query: (params) => ({
        url: '/decision-tables',
        params: {
          page: params.page ?? 1,
          pageSize: params.pageSize ?? 20,
          search: params.search,
          status: params.status,
          sortBy: params.sortBy ?? 'name',
          sortOrder: params.sortOrder ?? 'asc',
        },
      }),
      providesTags: (result) =>
        result
          ? [
              ...result.items.map(({ id }) => ({ type: 'DecisionTable' as const, id })),
              { type: 'DecisionTable', id: 'LIST' },
            ]
          : [{ type: 'DecisionTable', id: 'LIST' }],
    }),

    getDecisionTable: builder.query<DecisionTableDetail, string>({
      query: (id) => `/decision-tables/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'DecisionTable', id }],
    }),

    createDecisionTable: builder.mutation<DecisionTableDetail, CreateDecisionTableRequest>({
      query: (body) => ({
        url: '/decision-tables',
        method: 'POST',
        body,
      }),
      invalidatesTags: [{ type: 'DecisionTable', id: 'LIST' }],
    }),

    updateDecisionTable: builder.mutation<DecisionTableDetail, { id: string; data: UpdateDecisionTableRequest }>({
      query: ({ id, data }) => ({
        url: `/decision-tables/${id}`,
        method: 'PUT',
        body: data,
      }),
      invalidatesTags: (_result, _error, { id }) => [
        { type: 'DecisionTable', id },
        { type: 'DecisionTable', id: 'LIST' },
      ],
    }),

    addDecisionRule: builder.mutation<DecisionRule, { decisionTableId: string; data: AddDecisionRuleRequest }>({
      query: ({ decisionTableId, data }) => ({
        url: `/decision-tables/${decisionTableId}/rules`,
        method: 'POST',
        body: data,
      }),
      invalidatesTags: (_result, _error, { decisionTableId }) => [
        { type: 'DecisionTable', id: decisionTableId },
      ],
    }),

    updateDecisionRule: builder.mutation<DecisionRule, { decisionTableId: string; ruleId: string; data: UpdateDecisionRuleRequest }>({
      query: ({ decisionTableId, ruleId, data }) => ({
        url: `/decision-tables/${decisionTableId}/rules/${ruleId}`,
        method: 'PUT',
        body: data,
      }),
      invalidatesTags: (_result, _error, { decisionTableId }) => [
        { type: 'DecisionTable', id: decisionTableId },
      ],
    }),

    removeDecisionRule: builder.mutation<void, { decisionTableId: string; ruleId: string }>({
      query: ({ decisionTableId, ruleId }) => ({
        url: `/decision-tables/${decisionTableId}/rules/${ruleId}`,
        method: 'DELETE',
      }),
      invalidatesTags: (_result, _error, { decisionTableId }) => [
        { type: 'DecisionTable', id: decisionTableId },
      ],
    }),

    publishDecisionTable: builder.mutation<DecisionTableDetail, string>({
      query: (id) => ({
        url: `/decision-tables/${id}/publish`,
        method: 'POST',
      }),
      invalidatesTags: (_result, _error, id) => [
        { type: 'DecisionTable', id },
        { type: 'DecisionTable', id: 'LIST' },
      ],
    }),
  }),
  overrideExisting: false,
});

export const {
  useListDecisionTablesQuery,
  useGetDecisionTableQuery,
  useCreateDecisionTableMutation,
  useUpdateDecisionTableMutation,
  useAddDecisionRuleMutation,
  useUpdateDecisionRuleMutation,
  useRemoveDecisionRuleMutation,
  usePublishDecisionTableMutation,
} = decisionTablesApi;

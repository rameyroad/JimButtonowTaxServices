import { baseApi } from './baseApi';
import type {
  CancelWorkflowRequest,
  CaseWorkflowDetail,
  CaseWorkflowListItem,
  ListCaseWorkflowsParams,
  PaginatedResponse,
  StartWorkflowRequest,
} from '@/lib/types/caseWorkflow';

export const caseWorkflowsApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    listCaseWorkflows: builder.query<PaginatedResponse<CaseWorkflowListItem>, ListCaseWorkflowsParams>({
      query: ({ clientId, ...params }) => ({
        url: `/clients/${clientId}/workflows`,
        params: {
          page: params.page ?? 1,
          pageSize: params.pageSize ?? 20,
          status: params.status,
          sortBy: params.sortBy ?? 'createdAt',
          sortOrder: params.sortOrder ?? 'desc',
        },
      }),
      providesTags: (result, _error, { clientId }) =>
        result
          ? [
              ...result.items.map(({ id }) => ({ type: 'CaseWorkflow' as const, id })),
              { type: 'CaseWorkflow', id: `CLIENT_${clientId}` },
            ]
          : [{ type: 'CaseWorkflow', id: `CLIENT_${clientId}` }],
    }),

    getCaseWorkflow: builder.query<CaseWorkflowDetail, { clientId: string; caseWorkflowId: string }>({
      query: ({ clientId, caseWorkflowId }) =>
        `/clients/${clientId}/workflows/${caseWorkflowId}`,
      providesTags: (_result, _error, { caseWorkflowId }) => [
        { type: 'CaseWorkflow', id: caseWorkflowId },
      ],
    }),

    startWorkflow: builder.mutation<CaseWorkflowDetail, { clientId: string; data: StartWorkflowRequest }>({
      query: ({ clientId, data }) => ({
        url: `/clients/${clientId}/workflows`,
        method: 'POST',
        body: data,
      }),
      invalidatesTags: (_result, _error, { clientId }) => [
        { type: 'CaseWorkflow', id: `CLIENT_${clientId}` },
      ],
    }),

    cancelWorkflow: builder.mutation<CaseWorkflowDetail, { clientId: string; caseWorkflowId: string; data?: CancelWorkflowRequest }>({
      query: ({ clientId, caseWorkflowId, data }) => ({
        url: `/clients/${clientId}/workflows/${caseWorkflowId}/cancel`,
        method: 'POST',
        body: data ?? {},
      }),
      invalidatesTags: (_result, _error, { clientId, caseWorkflowId }) => [
        { type: 'CaseWorkflow', id: caseWorkflowId },
        { type: 'CaseWorkflow', id: `CLIENT_${clientId}` },
      ],
    }),
  }),
  overrideExisting: false,
});

export const {
  useListCaseWorkflowsQuery,
  useGetCaseWorkflowQuery,
  useStartWorkflowMutation,
  useCancelWorkflowMutation,
} = caseWorkflowsApi;

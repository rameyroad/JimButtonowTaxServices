import { baseApi } from './baseApi';
import type {
  IssueDetail,
  IssueListItem,
  ListIssuesParams,
  PaginatedResponse,
  UpdateIssueRequest,
} from '@/lib/types/issue';

export const issuesApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    listClientIssues: builder.query<PaginatedResponse<IssueListItem>, ListIssuesParams>({
      query: ({ clientId, ...params }) => ({
        url: `/clients/${clientId}/issues`,
        params: {
          page: params.page ?? 1,
          pageSize: params.pageSize ?? 20,
          issueType: params.issueType,
          severity: params.severity,
          taxYear: params.taxYear,
          resolved: params.resolved,
          sortBy: params.sortBy ?? 'detectedAt',
          sortOrder: params.sortOrder ?? 'desc',
        },
      }),
      providesTags: (result, _error, { clientId }) =>
        result
          ? [
              ...result.items.map(({ id }) => ({ type: 'Issue' as const, id })),
              { type: 'Issue', id: `CLIENT_${clientId}` },
            ]
          : [{ type: 'Issue', id: `CLIENT_${clientId}` }],
    }),

    getIssue: builder.query<IssueDetail, { clientId: string; issueId: string }>({
      query: ({ clientId, issueId }) =>
        `/clients/${clientId}/issues/${issueId}`,
      providesTags: (_result, _error, { issueId }) => [
        { type: 'Issue', id: issueId },
      ],
    }),

    updateIssue: builder.mutation<IssueDetail, { clientId: string; issueId: string; data: UpdateIssueRequest }>({
      query: ({ clientId, issueId, data }) => ({
        url: `/clients/${clientId}/issues/${issueId}`,
        method: 'PATCH',
        body: data,
      }),
      invalidatesTags: (_result, _error, { clientId, issueId }) => [
        { type: 'Issue', id: issueId },
        { type: 'Issue', id: `CLIENT_${clientId}` },
      ],
    }),
  }),
  overrideExisting: false,
});

export const {
  useListClientIssuesQuery,
  useGetIssueQuery,
  useUpdateIssueMutation,
} = issuesApi;

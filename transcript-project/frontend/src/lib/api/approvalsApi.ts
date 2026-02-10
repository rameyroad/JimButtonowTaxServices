import { baseApi } from './baseApi';
import type { ClientApprovalDto, RespondToApprovalRequest } from '@/lib/types/clientApproval';

export const approvalsApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    getApprovalByToken: builder.query<ClientApprovalDto, string>({
      query: (token) => `/approvals/${token}`,
    }),

    respondToApproval: builder.mutation<ClientApprovalDto, { token: string; data: RespondToApprovalRequest }>({
      query: ({ token, data }) => ({
        url: `/approvals/${token}/respond`,
        method: 'POST',
        body: data,
      }),
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetApprovalByTokenQuery,
  useRespondToApprovalMutation,
} = approvalsApi;

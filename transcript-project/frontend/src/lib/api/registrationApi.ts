import { baseApi } from './baseApi';
import type {
  RegisterOrganizationRequest,
  RegisterOrganizationResult,
  AcceptInvitationRequest,
  AcceptInvitationResult,
} from '@/lib/types/registration';

export const registrationApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    registerOrganization: builder.mutation<RegisterOrganizationResult, RegisterOrganizationRequest>({
      query: (body) => ({
        url: '/register',
        method: 'POST',
        body,
      }),
    }),

    acceptInvitation: builder.mutation<AcceptInvitationResult, { token: string; data: AcceptInvitationRequest }>({
      query: ({ token, data }) => ({
        url: `/invitations/${token}/accept`,
        method: 'POST',
        body: data,
      }),
    }),
  }),
  overrideExisting: false,
});

export const {
  useRegisterOrganizationMutation,
  useAcceptInvitationMutation,
} = registrationApi;

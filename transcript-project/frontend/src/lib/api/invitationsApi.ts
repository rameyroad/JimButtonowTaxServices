import { baseApi } from './baseApi';
import type { CreateInvitationRequest, InvitationDto } from '@/lib/types/invitation';

export const invitationsApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    listInvitations: builder.query<InvitationDto[], void>({
      query: () => '/invitations',
      providesTags: [{ type: 'User', id: 'INVITATIONS' }],
    }),

    createInvitation: builder.mutation<InvitationDto, CreateInvitationRequest>({
      query: (body) => ({
        url: '/invitations',
        method: 'POST',
        body,
      }),
      invalidatesTags: [{ type: 'User', id: 'INVITATIONS' }],
    }),
  }),
  overrideExisting: false,
});

export const {
  useListInvitationsQuery,
  useCreateInvitationMutation,
} = invitationsApi;

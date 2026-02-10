import { baseApi } from './baseApi';
import type {
  CompleteHumanTaskRequest,
  HumanTaskDetail,
  HumanTaskListItem,
  ListHumanTasksParams,
  PaginatedResponse,
  ReassignHumanTaskRequest,
} from '@/lib/types/humanTask';

export const humanTasksApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    listHumanTasks: builder.query<PaginatedResponse<HumanTaskListItem>, ListHumanTasksParams>({
      query: (params) => ({
        url: '/tasks',
        params: {
          page: params.page ?? 1,
          pageSize: params.pageSize ?? 20,
          status: params.status,
          assignedToUserId: params.assignedToUserId,
          sortBy: params.sortBy ?? 'createdAt',
          sortOrder: params.sortOrder ?? 'desc',
        },
      }),
      providesTags: (result) =>
        result
          ? [
              ...result.items.map(({ id }) => ({ type: 'HumanTask' as const, id })),
              { type: 'HumanTask', id: 'LIST' },
            ]
          : [{ type: 'HumanTask', id: 'LIST' }],
    }),

    getHumanTask: builder.query<HumanTaskDetail, string>({
      query: (id) => `/tasks/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'HumanTask', id }],
    }),

    completeHumanTask: builder.mutation<HumanTaskDetail, { id: string; data: CompleteHumanTaskRequest }>({
      query: ({ id, data }) => ({
        url: `/tasks/${id}/complete`,
        method: 'POST',
        body: data,
      }),
      invalidatesTags: (_result, _error, { id }) => [
        { type: 'HumanTask', id },
        { type: 'HumanTask', id: 'LIST' },
        { type: 'CaseWorkflow', id: 'LIST' },
      ],
    }),

    reassignHumanTask: builder.mutation<HumanTaskDetail, { id: string; data: ReassignHumanTaskRequest }>({
      query: ({ id, data }) => ({
        url: `/tasks/${id}/reassign`,
        method: 'POST',
        body: data,
      }),
      invalidatesTags: (_result, _error, { id }) => [
        { type: 'HumanTask', id },
        { type: 'HumanTask', id: 'LIST' },
      ],
    }),
  }),
  overrideExisting: false,
});

export const {
  useListHumanTasksQuery,
  useGetHumanTaskQuery,
  useCompleteHumanTaskMutation,
  useReassignHumanTaskMutation,
} = humanTasksApi;

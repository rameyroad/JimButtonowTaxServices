import { baseApi } from './baseApi';
import type {
  AddWorkflowStepRequest,
  CreateWorkflowDefinitionRequest,
  ListWorkflowDefinitionsParams,
  PaginatedResponse,
  UpdateWorkflowDefinitionRequest,
  UpdateWorkflowStepRequest,
  WorkflowDefinitionDetail,
  WorkflowDefinitionListItem,
  WorkflowStep,
} from '@/lib/types/workflow';

export const workflowsApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    listWorkflowDefinitions: builder.query<PaginatedResponse<WorkflowDefinitionListItem>, ListWorkflowDefinitionsParams>({
      query: (params) => ({
        url: '/workflows',
        params: {
          page: params.page ?? 1,
          pageSize: params.pageSize ?? 20,
          search: params.search,
          status: params.status,
          category: params.category,
          sortBy: params.sortBy ?? 'name',
          sortOrder: params.sortOrder ?? 'asc',
        },
      }),
      providesTags: (result) =>
        result
          ? [
              ...result.items.map(({ id }) => ({ type: 'Workflow' as const, id })),
              { type: 'Workflow', id: 'LIST' },
            ]
          : [{ type: 'Workflow', id: 'LIST' }],
    }),

    getWorkflowDefinition: builder.query<WorkflowDefinitionDetail, string>({
      query: (id) => `/workflows/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'Workflow', id }],
    }),

    createWorkflowDefinition: builder.mutation<WorkflowDefinitionDetail, CreateWorkflowDefinitionRequest>({
      query: (body) => ({
        url: '/workflows',
        method: 'POST',
        body,
      }),
      invalidatesTags: [{ type: 'Workflow', id: 'LIST' }],
    }),

    updateWorkflowDefinition: builder.mutation<WorkflowDefinitionDetail, { id: string; data: UpdateWorkflowDefinitionRequest }>({
      query: ({ id, data }) => ({
        url: `/workflows/${id}`,
        method: 'PUT',
        body: data,
      }),
      invalidatesTags: (_result, _error, { id }) => [
        { type: 'Workflow', id },
        { type: 'Workflow', id: 'LIST' },
      ],
    }),

    addWorkflowStep: builder.mutation<WorkflowStep, { workflowId: string; data: AddWorkflowStepRequest }>({
      query: ({ workflowId, data }) => ({
        url: `/workflows/${workflowId}/steps`,
        method: 'POST',
        body: data,
      }),
      invalidatesTags: (_result, _error, { workflowId }) => [
        { type: 'Workflow', id: workflowId },
      ],
    }),

    updateWorkflowStep: builder.mutation<WorkflowStep, { workflowId: string; stepId: string; data: UpdateWorkflowStepRequest }>({
      query: ({ workflowId, stepId, data }) => ({
        url: `/workflows/${workflowId}/steps/${stepId}`,
        method: 'PUT',
        body: data,
      }),
      invalidatesTags: (_result, _error, { workflowId }) => [
        { type: 'Workflow', id: workflowId },
      ],
    }),

    removeWorkflowStep: builder.mutation<void, { workflowId: string; stepId: string }>({
      query: ({ workflowId, stepId }) => ({
        url: `/workflows/${workflowId}/steps/${stepId}`,
        method: 'DELETE',
      }),
      invalidatesTags: (_result, _error, { workflowId }) => [
        { type: 'Workflow', id: workflowId },
      ],
    }),

    publishWorkflow: builder.mutation<WorkflowDefinitionDetail, string>({
      query: (id) => ({
        url: `/workflows/${id}/publish`,
        method: 'POST',
      }),
      invalidatesTags: (_result, _error, id) => [
        { type: 'Workflow', id },
        { type: 'Workflow', id: 'LIST' },
      ],
    }),

    unpublishWorkflow: builder.mutation<WorkflowDefinitionDetail, string>({
      query: (id) => ({
        url: `/workflows/${id}/unpublish`,
        method: 'POST',
      }),
      invalidatesTags: (_result, _error, id) => [
        { type: 'Workflow', id },
        { type: 'Workflow', id: 'LIST' },
      ],
    }),
  }),
  overrideExisting: false,
});

export const {
  useListWorkflowDefinitionsQuery,
  useGetWorkflowDefinitionQuery,
  useCreateWorkflowDefinitionMutation,
  useUpdateWorkflowDefinitionMutation,
  useAddWorkflowStepMutation,
  useUpdateWorkflowStepMutation,
  useRemoveWorkflowStepMutation,
  usePublishWorkflowMutation,
  useUnpublishWorkflowMutation,
} = workflowsApi;

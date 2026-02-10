import { baseApi } from './baseApi';
import type {
  CreateFormulaRequest,
  FormulaDetail,
  FormulaListItem,
  ListFormulasParams,
  PaginatedResponse,
  UpdateFormulaRequest,
} from '@/lib/types/formula';

export const formulasApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    listFormulas: builder.query<PaginatedResponse<FormulaListItem>, ListFormulasParams>({
      query: (params) => ({
        url: '/formulas',
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
              ...result.items.map(({ id }) => ({ type: 'Formula' as const, id })),
              { type: 'Formula', id: 'LIST' },
            ]
          : [{ type: 'Formula', id: 'LIST' }],
    }),

    getFormula: builder.query<FormulaDetail, string>({
      query: (id) => `/formulas/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'Formula', id }],
    }),

    createFormula: builder.mutation<FormulaDetail, CreateFormulaRequest>({
      query: (body) => ({
        url: '/formulas',
        method: 'POST',
        body,
      }),
      invalidatesTags: [{ type: 'Formula', id: 'LIST' }],
    }),

    updateFormula: builder.mutation<FormulaDetail, { id: string; data: UpdateFormulaRequest }>({
      query: ({ id, data }) => ({
        url: `/formulas/${id}`,
        method: 'PUT',
        body: data,
      }),
      invalidatesTags: (_result, _error, { id }) => [
        { type: 'Formula', id },
        { type: 'Formula', id: 'LIST' },
      ],
    }),

    publishFormula: builder.mutation<FormulaDetail, string>({
      query: (id) => ({
        url: `/formulas/${id}/publish`,
        method: 'POST',
      }),
      invalidatesTags: (_result, _error, id) => [
        { type: 'Formula', id },
        { type: 'Formula', id: 'LIST' },
      ],
    }),

    unpublishFormula: builder.mutation<FormulaDetail, string>({
      query: (id) => ({
        url: `/formulas/${id}/unpublish`,
        method: 'POST',
      }),
      invalidatesTags: (_result, _error, id) => [
        { type: 'Formula', id },
        { type: 'Formula', id: 'LIST' },
      ],
    }),
  }),
  overrideExisting: false,
});

export const {
  useListFormulasQuery,
  useGetFormulaQuery,
  useCreateFormulaMutation,
  useUpdateFormulaMutation,
  usePublishFormulaMutation,
  useUnpublishFormulaMutation,
} = formulasApi;

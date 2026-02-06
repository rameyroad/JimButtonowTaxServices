import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

const baseUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5100/api/v1';

export const baseApi = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
    baseUrl,
    prepareHeaders: async (headers) => {
      // In a real implementation, get the token from Auth0
      // This will be populated by the Auth0 provider
      const token = typeof window !== 'undefined'
        ? sessionStorage.getItem('accessToken')
        : null;

      if (token) {
        headers.set('Authorization', `Bearer ${token}`);
      }

      headers.set('Content-Type', 'application/json');
      return headers;
    },
  }),
  tagTypes: [
    'Organization',
    'User',
    'Client',
    'Authorization',
    'Transcript',
    'Notification',
  ],
  endpoints: () => ({}),
});

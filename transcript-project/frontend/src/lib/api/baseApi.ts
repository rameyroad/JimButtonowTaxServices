import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

const baseUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5100/api/v1';

// Well-known dev IDs — must match backend DevDataSeeder.cs
const DEV_ORGANIZATION_ID = '00000000-0000-0000-0000-000000000001';
const DEV_USER_ID = '00000000-0000-0000-0000-000000000002';

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

      // Dev mode: send tenant headers for multi-tenancy
      if (process.env.NODE_ENV === 'development') {
        headers.set('X-Organization-Id', DEV_ORGANIZATION_ID);
        headers.set('X-User-Id', DEV_USER_ID);
        // Check sessionStorage for role override (set by dev role switcher)
        const devRole = typeof window !== 'undefined'
          ? sessionStorage.getItem('devRole') || 'PlatformAdmin'
          : 'PlatformAdmin';
        headers.set('X-User-Role', devRole);
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
    'DecisionTable',
    'Workflow',
    'CaseWorkflow',
    'Issue',
    'Formula',
    'HumanTask',
  ],
  endpoints: () => ({}),
});

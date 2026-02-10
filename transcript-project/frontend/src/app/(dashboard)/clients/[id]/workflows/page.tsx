'use client';

import { use } from 'react';
import { Box, Typography, Breadcrumbs, Link } from '@mui/material';
import { NavigateNext, People } from '@mui/icons-material';
import NextLink from 'next/link';
import { CaseWorkflowsList } from '@/components/case-workflows';

interface PageProps {
  params: Promise<{ id: string }>;
}

export default function ClientWorkflowsPage({ params }: PageProps) {
  const { id } = use(params);

  return (
    <Box sx={{ p: 3 }}>
      <Breadcrumbs
        separator={<NavigateNext fontSize="small" />}
        sx={{ mb: 2 }}
      >
        <Link
          component={NextLink}
          href="/"
          underline="hover"
          color="inherit"
        >
          Dashboard
        </Link>
        <Link
          component={NextLink}
          href="/clients"
          underline="hover"
          color="inherit"
          sx={{ display: 'flex', alignItems: 'center' }}
        >
          <People sx={{ mr: 0.5, fontSize: 20 }} />
          Clients
        </Link>
        <Link
          component={NextLink}
          href={`/clients/${id}`}
          underline="hover"
          color="inherit"
        >
          Detail
        </Link>
        <Typography color="text.primary">Workflows</Typography>
      </Breadcrumbs>

      <Typography variant="h4" component="h1" fontWeight={600} sx={{ mb: 3 }}>
        Client Workflows
      </Typography>

      <CaseWorkflowsList clientId={id} />
    </Box>
  );
}

'use client';

import { use } from 'react';
import { Box, Typography, Breadcrumbs, Link } from '@mui/material';
import { NavigateNext, People } from '@mui/icons-material';
import NextLink from 'next/link';
import { CaseWorkflowDetail } from '@/components/case-workflows';

interface PageProps {
  params: Promise<{ id: string; caseId: string }>;
}

export default function CaseWorkflowDetailPage({ params }: PageProps) {
  const { id, caseId } = use(params);

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
        <Link
          component={NextLink}
          href={`/clients/${id}/workflows`}
          underline="hover"
          color="inherit"
        >
          Workflows
        </Link>
        <Typography color="text.primary">Execution</Typography>
      </Breadcrumbs>

      <CaseWorkflowDetail clientId={id} caseWorkflowId={caseId} />
    </Box>
  );
}

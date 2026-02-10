'use client';

import { use } from 'react';
import { Box, Typography, Breadcrumbs, Link } from '@mui/material';
import { NavigateNext, AccountTree } from '@mui/icons-material';
import NextLink from 'next/link';
import { WorkflowEditor } from '@/components/workflows';

interface PageProps {
  params: Promise<{ id: string }>;
}

export default function WorkflowDetailPage({ params }: PageProps) {
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
          href="/platform/workflows"
          underline="hover"
          color="inherit"
          sx={{ display: 'flex', alignItems: 'center' }}
        >
          <AccountTree sx={{ mr: 0.5, fontSize: 20 }} />
          Workflows
        </Link>
        <Typography color="text.primary">Detail</Typography>
      </Breadcrumbs>

      <WorkflowEditor id={id} />
    </Box>
  );
}

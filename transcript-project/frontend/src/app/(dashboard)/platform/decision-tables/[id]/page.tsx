'use client';

import { use } from 'react';
import { Box, Typography, Breadcrumbs, Link } from '@mui/material';
import { NavigateNext, TableChart } from '@mui/icons-material';
import NextLink from 'next/link';
import { DecisionTableEditor } from '@/components/decision-tables';

interface PageProps {
  params: Promise<{ id: string }>;
}

export default function DecisionTableDetailPage({ params }: PageProps) {
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
          href="/platform/decision-tables"
          underline="hover"
          color="inherit"
          sx={{ display: 'flex', alignItems: 'center' }}
        >
          <TableChart sx={{ mr: 0.5, fontSize: 20 }} />
          Decision Tables
        </Link>
        <Typography color="text.primary">Detail</Typography>
      </Breadcrumbs>

      <DecisionTableEditor id={id} />
    </Box>
  );
}

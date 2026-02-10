'use client';

import { Box, Typography, Breadcrumbs, Link } from '@mui/material';
import { NavigateNext, TableChart } from '@mui/icons-material';
import NextLink from 'next/link';
import { CreateDecisionTableForm } from '@/components/decision-tables';

export default function NewDecisionTablePage() {
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
        <Typography color="text.primary">New</Typography>
      </Breadcrumbs>

      <Typography variant="h4" component="h1" fontWeight={600} sx={{ mb: 3 }}>
        Create Decision Table
      </Typography>

      <CreateDecisionTableForm />
    </Box>
  );
}

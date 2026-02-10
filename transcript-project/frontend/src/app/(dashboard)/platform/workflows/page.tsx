'use client';

import { Box, Typography, Button, Breadcrumbs, Link } from '@mui/material';
import { Add, NavigateNext, AccountTree } from '@mui/icons-material';
import NextLink from 'next/link';
import { WorkflowsTable } from '@/components/workflows';

export default function WorkflowsPage() {
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
        <Typography color="text.primary" sx={{ display: 'flex', alignItems: 'center' }}>
          <AccountTree sx={{ mr: 0.5, fontSize: 20 }} />
          Workflows
        </Typography>
      </Breadcrumbs>

      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          mb: 3,
        }}
      >
        <Box>
          <Typography variant="h4" component="h1" fontWeight={600}>
            Workflow Definitions
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
            Define and manage automated workflows for IRS tax resolution
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<Add />}
          href="/platform/workflows/new"
          component={NextLink}
        >
          New Workflow
        </Button>
      </Box>

      <WorkflowsTable />
    </Box>
  );
}

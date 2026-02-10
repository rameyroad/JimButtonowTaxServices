'use client';

import { Box, Typography, Button, Breadcrumbs, Link } from '@mui/material';
import { Add, NavigateNext, TableChart } from '@mui/icons-material';
import NextLink from 'next/link';
import { DecisionTablesTable } from '@/components/decision-tables';

export default function DecisionTablesPage() {
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
          <TableChart sx={{ mr: 0.5, fontSize: 20 }} />
          Decision Tables
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
            Decision Tables
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
            Define rules for automated decision-making in workflows
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<Add />}
          href="/platform/decision-tables/new"
          component={NextLink}
        >
          New Decision Table
        </Button>
      </Box>

      <DecisionTablesTable />
    </Box>
  );
}

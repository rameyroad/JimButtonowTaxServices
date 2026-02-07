'use client';

import { Box, Typography, Button, Breadcrumbs, Link } from '@mui/material';
import { Add, People, NavigateNext } from '@mui/icons-material';
import NextLink from 'next/link';
import { ClientsTable } from '@/components/clients';

export default function ClientsPage() {
  return (
    <Box sx={{ p: 3 }}>
      {/* Breadcrumbs */}
      <Breadcrumbs
        separator={<NavigateNext fontSize="small" />}
        sx={{ mb: 2 }}
      >
        <Link
          component={NextLink}
          href="/"
          underline="hover"
          color="inherit"
          sx={{ display: 'flex', alignItems: 'center' }}
        >
          Dashboard
        </Link>
        <Typography color="text.primary" sx={{ display: 'flex', alignItems: 'center' }}>
          <People sx={{ mr: 0.5, fontSize: 20 }} />
          Clients
        </Typography>
      </Breadcrumbs>

      {/* Header */}
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
            Clients
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
            Manage your individual and business clients
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<Add />}
          href="/clients/new"
          component={NextLink}
        >
          Add Client
        </Button>
      </Box>

      {/* Client List Table */}
      <ClientsTable />
    </Box>
  );
}

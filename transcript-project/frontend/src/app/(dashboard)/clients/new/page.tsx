'use client';

import { Box, Breadcrumbs, Link as MuiLink, Typography } from '@mui/material';
import Link from 'next/link';
import { ClientForm } from '@/components/clients';

export default function NewClientPage() {
  return (
    <Box sx={{ p: 3 }}>
      {/* Breadcrumbs */}
      <Breadcrumbs sx={{ mb: 2 }}>
        <MuiLink component={Link} href="/clients" underline="hover" color="inherit">
          Clients
        </MuiLink>
        <Typography color="text.primary">New Client</Typography>
      </Breadcrumbs>

      {/* Page header */}
      <Typography variant="h4" component="h1" sx={{ mb: 3 }}>
        Add New Client
      </Typography>

      {/* Client form */}
      <Box sx={{ maxWidth: 800 }}>
        <ClientForm />
      </Box>
    </Box>
  );
}

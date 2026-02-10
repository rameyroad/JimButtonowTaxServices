'use client';

import { Box, Breadcrumbs, Typography, Link as MuiLink } from '@mui/material';
import Link from 'next/link';
import { FormulasTable } from '@/components/formulas';

export default function FormulasPage() {
  return (
    <Box>
      <Breadcrumbs sx={{ mb: 2 }}>
        <MuiLink component={Link} href="/" underline="hover" color="inherit">
          Dashboard
        </MuiLink>
        <Typography color="text.primary">Formulas</Typography>
      </Breadcrumbs>

      <Typography variant="h4" component="h1" sx={{ mb: 3 }}>
        Calculation Formulas
      </Typography>

      <FormulasTable />
    </Box>
  );
}

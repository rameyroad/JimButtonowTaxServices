'use client';

import { Box, Breadcrumbs, Typography, Link as MuiLink } from '@mui/material';
import Link from 'next/link';
import { CreateFormulaForm } from '@/components/formulas';

export default function NewFormulaPage() {
  return (
    <Box>
      <Breadcrumbs sx={{ mb: 2 }}>
        <MuiLink component={Link} href="/" underline="hover" color="inherit">
          Dashboard
        </MuiLink>
        <MuiLink component={Link} href="/platform/formulas" underline="hover" color="inherit">
          Formulas
        </MuiLink>
        <Typography color="text.primary">New</Typography>
      </Breadcrumbs>

      <Typography variant="h4" component="h1" sx={{ mb: 3 }}>
        Create Formula
      </Typography>

      <CreateFormulaForm />
    </Box>
  );
}

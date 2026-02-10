'use client';

import { use } from 'react';
import { Box, Breadcrumbs, Typography, Link as MuiLink } from '@mui/material';
import Link from 'next/link';
import { FormulaEditor } from '@/components/formulas';

export default function FormulaDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);

  return (
    <Box>
      <Breadcrumbs sx={{ mb: 2 }}>
        <MuiLink component={Link} href="/" underline="hover" color="inherit">
          Dashboard
        </MuiLink>
        <MuiLink component={Link} href="/platform/formulas" underline="hover" color="inherit">
          Formulas
        </MuiLink>
        <Typography color="text.primary">Detail</Typography>
      </Breadcrumbs>

      <FormulaEditor formulaId={id} />
    </Box>
  );
}

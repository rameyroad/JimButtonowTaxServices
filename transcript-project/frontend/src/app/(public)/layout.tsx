'use client';

import { ReactNode } from 'react';
import { Box, Container } from '@mui/material';

interface PublicLayoutProps {
  children: ReactNode;
}

export default function PublicLayout({ children }: PublicLayoutProps) {
  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'background.default', py: 4 }}>
      <Container maxWidth="sm">
        {children}
      </Container>
    </Box>
  );
}

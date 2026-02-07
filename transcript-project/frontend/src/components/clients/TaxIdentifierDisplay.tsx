'use client';

import { Box, Typography, Tooltip } from '@mui/material';
import { Lock } from '@mui/icons-material';
import type { ClientType } from '@/lib/types/client';

interface TaxIdentifierDisplayProps {
  maskedValue: string;
  last4: string;
  clientType: ClientType;
}

export function TaxIdentifierDisplay({
  maskedValue,
  last4,
  clientType,
}: TaxIdentifierDisplayProps) {
  const label = clientType === 'Individual' ? 'SSN' : 'EIN';
  const tooltipText = `${label} ending in ${last4}`;

  return (
    <Tooltip title={tooltipText} arrow>
      <Box
        sx={{
          display: 'inline-flex',
          alignItems: 'center',
          gap: 0.5,
          fontFamily: 'monospace',
          fontSize: '0.875rem',
          color: 'text.secondary',
        }}
      >
        <Lock sx={{ fontSize: 14, color: 'action.active' }} />
        <Typography
          component="span"
          sx={{
            fontFamily: 'monospace',
            fontSize: 'inherit',
            letterSpacing: '0.05em',
          }}
        >
          {maskedValue}
        </Typography>
      </Box>
    </Tooltip>
  );
}

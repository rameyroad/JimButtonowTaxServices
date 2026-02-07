'use client';

import { Chip } from '@mui/material';
import { Person, Business } from '@mui/icons-material';
import type { ClientType } from '@/lib/types/client';

interface ClientTypeBadgeProps {
  clientType: ClientType;
  size?: 'small' | 'medium';
}

export function ClientTypeBadge({ clientType, size = 'small' }: ClientTypeBadgeProps) {
  const isIndividual = clientType === 'Individual';

  return (
    <Chip
      icon={isIndividual ? <Person fontSize="small" /> : <Business fontSize="small" />}
      label={clientType}
      size={size}
      color={isIndividual ? 'primary' : 'secondary'}
      variant="outlined"
      sx={{
        fontWeight: 500,
        '& .MuiChip-icon': {
          marginLeft: '4px',
        },
      }}
    />
  );
}

'use client';

import { Box, TextField, MenuItem } from '@mui/material';
import type { Address } from '@/lib/types/client';

// US states for dropdown
const US_STATES = [
  { code: 'AL', name: 'Alabama' },
  { code: 'AK', name: 'Alaska' },
  { code: 'AZ', name: 'Arizona' },
  { code: 'AR', name: 'Arkansas' },
  { code: 'CA', name: 'California' },
  { code: 'CO', name: 'Colorado' },
  { code: 'CT', name: 'Connecticut' },
  { code: 'DE', name: 'Delaware' },
  { code: 'FL', name: 'Florida' },
  { code: 'GA', name: 'Georgia' },
  { code: 'HI', name: 'Hawaii' },
  { code: 'ID', name: 'Idaho' },
  { code: 'IL', name: 'Illinois' },
  { code: 'IN', name: 'Indiana' },
  { code: 'IA', name: 'Iowa' },
  { code: 'KS', name: 'Kansas' },
  { code: 'KY', name: 'Kentucky' },
  { code: 'LA', name: 'Louisiana' },
  { code: 'ME', name: 'Maine' },
  { code: 'MD', name: 'Maryland' },
  { code: 'MA', name: 'Massachusetts' },
  { code: 'MI', name: 'Michigan' },
  { code: 'MN', name: 'Minnesota' },
  { code: 'MS', name: 'Mississippi' },
  { code: 'MO', name: 'Missouri' },
  { code: 'MT', name: 'Montana' },
  { code: 'NE', name: 'Nebraska' },
  { code: 'NV', name: 'Nevada' },
  { code: 'NH', name: 'New Hampshire' },
  { code: 'NJ', name: 'New Jersey' },
  { code: 'NM', name: 'New Mexico' },
  { code: 'NY', name: 'New York' },
  { code: 'NC', name: 'North Carolina' },
  { code: 'ND', name: 'North Dakota' },
  { code: 'OH', name: 'Ohio' },
  { code: 'OK', name: 'Oklahoma' },
  { code: 'OR', name: 'Oregon' },
  { code: 'PA', name: 'Pennsylvania' },
  { code: 'RI', name: 'Rhode Island' },
  { code: 'SC', name: 'South Carolina' },
  { code: 'SD', name: 'South Dakota' },
  { code: 'TN', name: 'Tennessee' },
  { code: 'TX', name: 'Texas' },
  { code: 'UT', name: 'Utah' },
  { code: 'VT', name: 'Vermont' },
  { code: 'VA', name: 'Virginia' },
  { code: 'WA', name: 'Washington' },
  { code: 'WV', name: 'West Virginia' },
  { code: 'WI', name: 'Wisconsin' },
  { code: 'WY', name: 'Wyoming' },
  { code: 'DC', name: 'District of Columbia' },
];

interface AddressFormProps {
  value: Address;
  onChange: (address: Address) => void;
  errors?: Partial<Record<keyof Address, string>>;
  disabled?: boolean;
}

/**
 * Address form fields component.
 */
export function AddressForm({ value, onChange, errors = {}, disabled = false }: AddressFormProps) {
  const handleChange = (field: keyof Address) => (event: React.ChangeEvent<HTMLInputElement>) => {
    onChange({
      ...value,
      [field]: event.target.value,
    });
  };

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
      <TextField
        label="Street Address"
        value={value.street1}
        onChange={handleChange('street1')}
        error={!!errors.street1}
        helperText={errors.street1}
        disabled={disabled}
        required
        fullWidth
      />
      <TextField
        label="Street Address 2"
        value={value.street2 ?? ''}
        onChange={handleChange('street2')}
        error={!!errors.street2}
        helperText={errors.street2}
        disabled={disabled}
        fullWidth
        placeholder="Apt, Suite, Unit, etc."
      />
      <Box sx={{ display: 'flex', gap: 2 }}>
        <TextField
          label="City"
          value={value.city}
          onChange={handleChange('city')}
          error={!!errors.city}
          helperText={errors.city}
          disabled={disabled}
          required
          sx={{ flex: 2 }}
        />
        <TextField
          select
          label="State"
          value={value.state}
          onChange={handleChange('state')}
          error={!!errors.state}
          helperText={errors.state}
          disabled={disabled}
          required
          sx={{ flex: 1 }}
        >
          {US_STATES.map((state) => (
            <MenuItem key={state.code} value={state.code}>
              {state.code}
            </MenuItem>
          ))}
        </TextField>
        <TextField
          label="ZIP Code"
          value={value.postalCode}
          onChange={handleChange('postalCode')}
          error={!!errors.postalCode}
          helperText={errors.postalCode}
          disabled={disabled}
          required
          sx={{ flex: 1 }}
          slotProps={{
            htmlInput: {
              maxLength: 10,
            },
          }}
        />
      </Box>
      <TextField
        select
        label="Country"
        value={value.country}
        onChange={handleChange('country')}
        error={!!errors.country}
        helperText={errors.country}
        disabled={disabled}
        required
        fullWidth
      >
        <MenuItem value="US">United States</MenuItem>
      </TextField>
    </Box>
  );
}

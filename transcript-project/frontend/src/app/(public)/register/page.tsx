'use client';

import { useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  TextField,
  Button,
  Alert,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from '@mui/material';
import { useRegisterOrganizationMutation } from '@/lib/api/registrationApi';

const US_STATES = [
  'AL','AK','AZ','AR','CA','CO','CT','DE','FL','GA',
  'HI','ID','IL','IN','IA','KS','KY','LA','ME','MD',
  'MA','MI','MN','MS','MO','MT','NE','NV','NH','NJ',
  'NM','NY','NC','ND','OH','OK','OR','PA','RI','SC',
  'SD','TN','TX','UT','VT','VA','WA','WV','WI','WY','DC',
];

export default function RegisterPage() {
  const [register, { isLoading, isSuccess, data, error }] = useRegisterOrganizationMutation();

  const [form, setForm] = useState({
    organizationName: '',
    contactEmail: '',
    street1: '',
    street2: '',
    city: '',
    state: '',
    postalCode: '',
    adminFirstName: '',
    adminLastName: '',
    adminEmail: '',
  });

  const handleChange = (field: string) => (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [field]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await register({
      ...form,
      street2: form.street2 || undefined,
    });
  };

  if (isSuccess && data) {
    return (
      <Paper sx={{ p: 4 }}>
        <Typography variant="h5" fontWeight={600} sx={{ mb: 2 }}>
          Registration Successful
        </Typography>
        <Alert severity="success" sx={{ mb: 2 }}>
          Your organization &quot;{data.organizationName}&quot; has been created.
        </Alert>
        <Typography variant="body2" color="text.secondary">
          You can now log in and start using the platform.
        </Typography>
      </Paper>
    );
  }

  return (
    <Paper sx={{ p: 4 }}>
      <Typography variant="h5" fontWeight={600} sx={{ mb: 1 }}>
        Register Your Firm
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Create your organization to start managing IRS transcripts and workflows.
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          Registration failed. Please check your information and try again.
        </Alert>
      )}

      <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
        <Typography variant="subtitle2" color="text.secondary">Organization</Typography>
        <TextField
          label="Firm Name"
          required
          value={form.organizationName}
          onChange={handleChange('organizationName')}
          inputProps={{ maxLength: 200 }}
        />
        <TextField
          label="Contact Email"
          required
          type="email"
          value={form.contactEmail}
          onChange={handleChange('contactEmail')}
        />
        <TextField
          label="Street Address"
          required
          value={form.street1}
          onChange={handleChange('street1')}
        />
        <TextField
          label="Street Address 2"
          value={form.street2}
          onChange={handleChange('street2')}
        />
        <Box sx={{ display: 'flex', gap: 2 }}>
          <TextField
            label="City"
            required
            value={form.city}
            onChange={handleChange('city')}
            sx={{ flex: 1 }}
          />
          <FormControl required sx={{ minWidth: 80 }}>
            <InputLabel>State</InputLabel>
            <Select
              value={form.state}
              label="State"
              onChange={(e) => setForm({ ...form, state: e.target.value })}
            >
              {US_STATES.map((s) => (
                <MenuItem key={s} value={s}>{s}</MenuItem>
              ))}
            </Select>
          </FormControl>
          <TextField
            label="ZIP"
            required
            value={form.postalCode}
            onChange={handleChange('postalCode')}
            sx={{ width: 100 }}
          />
        </Box>

        <Typography variant="subtitle2" color="text.secondary" sx={{ mt: 1 }}>Admin Account</Typography>
        <Box sx={{ display: 'flex', gap: 2 }}>
          <TextField
            label="First Name"
            required
            value={form.adminFirstName}
            onChange={handleChange('adminFirstName')}
            sx={{ flex: 1 }}
          />
          <TextField
            label="Last Name"
            required
            value={form.adminLastName}
            onChange={handleChange('adminLastName')}
            sx={{ flex: 1 }}
          />
        </Box>
        <TextField
          label="Admin Email"
          required
          type="email"
          value={form.adminEmail}
          onChange={handleChange('adminEmail')}
        />

        <Button
          type="submit"
          variant="contained"
          size="large"
          disabled={isLoading}
          sx={{ mt: 1 }}
        >
          {isLoading ? 'Registering...' : 'Register'}
        </Button>
      </Box>
    </Paper>
  );
}

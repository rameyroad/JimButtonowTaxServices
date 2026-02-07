'use client';

import {
  Alert,
  Box,
  Breadcrumbs,
  Button,
  Link as MuiLink,
  MenuItem,
  Paper,
  TextField,
  Typography,
  CircularProgress,
} from '@mui/material';
import Link from 'next/link';
import { useParams, useRouter } from 'next/navigation';
import { useCallback, useEffect, useState } from 'react';
import { useGetClientQuery, useUpdateClientMutation } from '@/lib/api/clientsApi';
import { AddressForm } from '@/components/clients';
import type { Address, BusinessEntityType, UpdateClientRequest } from '@/lib/types/client';

// Business entity types for dropdown
const ENTITY_TYPES: { value: BusinessEntityType; label: string }[] = [
  { value: 'SoleProprietor', label: 'Sole Proprietor' },
  { value: 'LLC', label: 'LLC' },
  { value: 'Partnership', label: 'Partnership' },
  { value: 'SCorp', label: 'S Corporation' },
  { value: 'CCorp', label: 'C Corporation' },
  { value: 'NonProfit', label: 'Non-Profit' },
  { value: 'Trust', label: 'Trust' },
  { value: 'Estate', label: 'Estate' },
];

interface FormErrors {
  firstName?: string;
  lastName?: string;
  businessName?: string;
  entityType?: string;
  email?: string;
  phone?: string;
  address?: Partial<Record<keyof Address, string>>;
  general?: string;
}

export default function EditClientPage() {
  const params = useParams();
  const router = useRouter();
  const clientId = params.id as string;

  const { data: client, isLoading: isLoadingClient, error: loadError } = useGetClientQuery(clientId);
  const [updateClient, { isLoading: isUpdating }] = useUpdateClientMutation();

  // Form state
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [businessName, setBusinessName] = useState('');
  const [entityType, setEntityType] = useState<BusinessEntityType | ''>('');
  const [responsibleParty, setResponsibleParty] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState<Address>({
    street1: '',
    street2: '',
    city: '',
    state: '',
    postalCode: '',
    country: 'US',
  });
  const [notes, setNotes] = useState('');
  const [errors, setErrors] = useState<FormErrors>({});

  // Populate form when client data loads
  useEffect(() => {
    if (client) {
      setFirstName(client.firstName ?? '');
      setLastName(client.lastName ?? '');
      setBusinessName(client.businessName ?? '');
      setEntityType(client.entityType ?? '');
      setResponsibleParty(client.responsibleParty ?? '');
      setEmail(client.email);
      setPhone(client.phone ?? '');
      setAddress(client.address);
      setNotes(client.notes ?? '');
    }
  }, [client]);

  const validate = useCallback((): boolean => {
    const newErrors: FormErrors = {};

    if (client?.clientType === 'Individual') {
      if (!firstName.trim()) {
        newErrors.firstName = 'First name is required';
      } else if (firstName.length > 100) {
        newErrors.firstName = 'First name must not exceed 100 characters';
      }

      if (!lastName.trim()) {
        newErrors.lastName = 'Last name is required';
      } else if (lastName.length > 100) {
        newErrors.lastName = 'Last name must not exceed 100 characters';
      }
    } else if (client?.clientType === 'Business') {
      if (!businessName.trim()) {
        newErrors.businessName = 'Business name is required';
      } else if (businessName.length > 200) {
        newErrors.businessName = 'Business name must not exceed 200 characters';
      }

      if (!entityType) {
        newErrors.entityType = 'Entity type is required';
      }
    }

    if (!email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      newErrors.email = 'Invalid email address';
    }

    if (phone && !/^[\d\s\-\(\)\+\.]+$/.test(phone)) {
      newErrors.phone = 'Invalid phone number format';
    }

    // Address validation
    const addressErrors: Partial<Record<keyof Address, string>> = {};
    if (!address.street1.trim()) {
      addressErrors.street1 = 'Street address is required';
    }
    if (!address.city.trim()) {
      addressErrors.city = 'City is required';
    }
    if (!address.state.trim()) {
      addressErrors.state = 'State is required';
    }
    if (!address.postalCode.trim()) {
      addressErrors.postalCode = 'ZIP code is required';
    }
    if (!address.country.trim()) {
      addressErrors.country = 'Country is required';
    }

    if (Object.keys(addressErrors).length > 0) {
      newErrors.address = addressErrors;
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  }, [client?.clientType, firstName, lastName, businessName, entityType, email, phone, address]);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (!client || !validate()) {
      return;
    }

    const request: UpdateClientRequest = {
      version: client.version,
    };

    // Only include changed fields
    if (client.clientType === 'Individual') {
      if (firstName.trim() !== client.firstName) {
        request.firstName = firstName.trim();
      }
      if (lastName.trim() !== client.lastName) {
        request.lastName = lastName.trim();
      }
    } else {
      if (businessName.trim() !== client.businessName) {
        request.businessName = businessName.trim();
      }
      if (entityType !== client.entityType) {
        request.entityType = entityType as BusinessEntityType;
      }
      if (responsibleParty.trim() !== (client.responsibleParty ?? '')) {
        request.responsibleParty = responsibleParty.trim() || undefined;
      }
    }

    if (email.trim().toLowerCase() !== client.email) {
      request.email = email.trim().toLowerCase();
    }
    if (phone.trim() !== (client.phone ?? '')) {
      request.phone = phone.trim() || undefined;
    }

    // Check if address changed
    const addressChanged =
      address.street1.trim() !== client.address.street1 ||
      (address.street2?.trim() ?? '') !== (client.address.street2 ?? '') ||
      address.city.trim() !== client.address.city ||
      address.state.trim() !== client.address.state ||
      address.postalCode.trim() !== client.address.postalCode ||
      address.country.trim() !== client.address.country;

    if (addressChanged) {
      request.address = {
        street1: address.street1.trim(),
        street2: address.street2?.trim() || undefined,
        city: address.city.trim(),
        state: address.state.trim(),
        postalCode: address.postalCode.trim(),
        country: address.country.trim(),
      };
    }

    if (notes.trim() !== (client.notes ?? '')) {
      request.notes = notes.trim() || undefined;
    }

    try {
      await updateClient({ id: clientId, data: request }).unwrap();
      router.push(`/clients/${clientId}`);
    } catch (error: unknown) {
      const apiError = error as { status?: number; data?: { detail?: string } };
      if (apiError.status === 409) {
        setErrors({
          general: 'This client has been modified by another user. Please refresh and try again.',
        });
      } else if (apiError.data?.detail) {
        setErrors({ general: apiError.data.detail });
      } else {
        setErrors({ general: 'An unexpected error occurred. Please try again.' });
      }
    }
  };

  const handleCancel = () => {
    router.back();
  };

  if (isLoadingClient) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (loadError) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="error">Failed to load client details. Please try again.</Alert>
      </Box>
    );
  }

  if (!client) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="warning">Client not found.</Alert>
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      {/* Breadcrumbs */}
      <Breadcrumbs sx={{ mb: 2 }}>
        <MuiLink component={Link} href="/clients" underline="hover" color="inherit">
          Clients
        </MuiLink>
        <MuiLink component={Link} href={`/clients/${clientId}`} underline="hover" color="inherit">
          {client.displayName}
        </MuiLink>
        <Typography color="text.primary">Edit</Typography>
      </Breadcrumbs>

      {/* Page header */}
      <Typography variant="h4" component="h1" sx={{ mb: 3 }}>
        Edit {client.displayName}
      </Typography>

      {/* Edit form */}
      <Paper sx={{ p: 3, maxWidth: 800 }}>
        <form onSubmit={handleSubmit}>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
            {/* General error alert */}
            {errors.general && (
              <Alert severity="error" onClose={() => setErrors((e) => ({ ...e, general: undefined }))}>
                {errors.general}
              </Alert>
            )}

            {/* Individual fields */}
            {client.clientType === 'Individual' && (
              <>
                <Typography variant="subtitle1" sx={{ fontWeight: 500 }}>
                  Personal Information
                </Typography>
                <Box sx={{ display: 'flex', gap: 2 }}>
                  <TextField
                    label="First Name"
                    value={firstName}
                    onChange={(e) => setFirstName(e.target.value)}
                    error={!!errors.firstName}
                    helperText={errors.firstName}
                    disabled={isUpdating}
                    required
                    fullWidth
                  />
                  <TextField
                    label="Last Name"
                    value={lastName}
                    onChange={(e) => setLastName(e.target.value)}
                    error={!!errors.lastName}
                    helperText={errors.lastName}
                    disabled={isUpdating}
                    required
                    fullWidth
                  />
                </Box>
              </>
            )}

            {/* Business fields */}
            {client.clientType === 'Business' && (
              <>
                <Typography variant="subtitle1" sx={{ fontWeight: 500 }}>
                  Business Information
                </Typography>
                <TextField
                  label="Business Name"
                  value={businessName}
                  onChange={(e) => setBusinessName(e.target.value)}
                  error={!!errors.businessName}
                  helperText={errors.businessName}
                  disabled={isUpdating}
                  required
                  fullWidth
                />
                <Box sx={{ display: 'flex', gap: 2 }}>
                  <TextField
                    select
                    label="Entity Type"
                    value={entityType}
                    onChange={(e) => setEntityType(e.target.value as BusinessEntityType)}
                    error={!!errors.entityType}
                    helperText={errors.entityType}
                    disabled={isUpdating}
                    required
                    fullWidth
                  >
                    {ENTITY_TYPES.map((type) => (
                      <MenuItem key={type.value} value={type.value}>
                        {type.label}
                      </MenuItem>
                    ))}
                  </TextField>
                  <TextField
                    label="Responsible Party"
                    value={responsibleParty}
                    onChange={(e) => setResponsibleParty(e.target.value)}
                    disabled={isUpdating}
                    fullWidth
                  />
                </Box>
              </>
            )}

            {/* Contact information */}
            <Typography variant="subtitle1" sx={{ fontWeight: 500, mt: 1 }}>
              Contact Information
            </Typography>
            <Box sx={{ display: 'flex', gap: 2 }}>
              <TextField
                label="Email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                error={!!errors.email}
                helperText={errors.email}
                disabled={isUpdating}
                required
                fullWidth
              />
              <TextField
                label="Phone"
                value={phone}
                onChange={(e) => setPhone(e.target.value)}
                error={!!errors.phone}
                helperText={errors.phone}
                disabled={isUpdating}
                fullWidth
                placeholder="(555) 123-4567"
              />
            </Box>

            {/* Address */}
            <Typography variant="subtitle1" sx={{ fontWeight: 500, mt: 1 }}>
              Mailing Address
            </Typography>
            <AddressForm
              value={address}
              onChange={setAddress}
              errors={errors.address}
              disabled={isUpdating}
            />

            {/* Notes */}
            <TextField
              label="Notes"
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              disabled={isUpdating}
              multiline
              rows={3}
              fullWidth
              placeholder="Optional notes about this client..."
            />

            {/* Action buttons */}
            <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end', mt: 2 }}>
              <Button variant="outlined" onClick={handleCancel} disabled={isUpdating}>
                Cancel
              </Button>
              <Button type="submit" variant="contained" disabled={isUpdating}>
                {isUpdating ? 'Saving...' : 'Save Changes'}
              </Button>
            </Box>
          </Box>
        </form>
      </Paper>
    </Box>
  );
}

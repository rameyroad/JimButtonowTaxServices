'use client';

import {
  Alert,
  Box,
  Button,
  FormControl,
  FormControlLabel,
  FormLabel,
  MenuItem,
  Paper,
  Radio,
  RadioGroup,
  TextField,
  Typography,
} from '@mui/material';
import { useRouter } from 'next/navigation';
import { useCallback, useState } from 'react';
import { useCreateClientMutation } from '@/lib/api/clientsApi';
import type { Address, BusinessEntityType, ClientType, CreateClientRequest } from '@/lib/types/client';
import { AddressForm } from './AddressForm';
import { TaxIdentifierInput } from './TaxIdentifierInput';

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

const EMPTY_ADDRESS: Address = {
  street1: '',
  street2: '',
  city: '',
  state: '',
  postalCode: '',
  country: 'US',
};

interface FormErrors {
  firstName?: string;
  lastName?: string;
  businessName?: string;
  entityType?: string;
  responsibleParty?: string;
  taxIdentifier?: string;
  email?: string;
  phone?: string;
  address?: Partial<Record<keyof Address, string>>;
  general?: string;
}

/**
 * Form for creating a new client (individual or business).
 */
export function ClientForm() {
  const router = useRouter();
  const [createClient, { isLoading }] = useCreateClientMutation();

  // Form state
  const [clientType, setClientType] = useState<ClientType>('Individual');
  // Individual fields
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  // Business fields
  const [businessName, setBusinessName] = useState('');
  const [entityType, setEntityType] = useState<BusinessEntityType | ''>('');
  const [responsibleParty, setResponsibleParty] = useState('');
  // Common fields
  const [taxIdentifier, setTaxIdentifier] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState<Address>(EMPTY_ADDRESS);
  const [notes, setNotes] = useState('');
  const [errors, setErrors] = useState<FormErrors>({});

  const handleClientTypeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const newType = event.target.value as ClientType;
    setClientType(newType);
    // Clear the tax identifier when switching types (SSN vs EIN format)
    setTaxIdentifier('');
    // Clear type-specific errors
    setErrors({});
  };

  // Validate the form
  const validate = useCallback((): boolean => {
    const newErrors: FormErrors = {};

    if (clientType === 'Individual') {
      // First name validation
      if (!firstName.trim()) {
        newErrors.firstName = 'First name is required';
      } else if (firstName.length > 100) {
        newErrors.firstName = 'First name must not exceed 100 characters';
      }

      // Last name validation
      if (!lastName.trim()) {
        newErrors.lastName = 'Last name is required';
      } else if (lastName.length > 100) {
        newErrors.lastName = 'Last name must not exceed 100 characters';
      }

      // SSN validation
      const ssnDigits = taxIdentifier.replace(/\D/g, '');
      if (!ssnDigits) {
        newErrors.taxIdentifier = 'Social Security Number is required';
      } else if (ssnDigits.length !== 9) {
        newErrors.taxIdentifier = 'SSN must be 9 digits';
      } else if (/^(000|666|9\d{2})/.test(ssnDigits)) {
        newErrors.taxIdentifier = 'Invalid SSN format';
      }
    } else {
      // Business name validation
      if (!businessName.trim()) {
        newErrors.businessName = 'Business name is required';
      } else if (businessName.length > 200) {
        newErrors.businessName = 'Business name must not exceed 200 characters';
      }

      // Entity type validation
      if (!entityType) {
        newErrors.entityType = 'Entity type is required';
      }

      // EIN validation (XX-XXXXXXX or XXXXXXXXX)
      const einDigits = taxIdentifier.replace(/\D/g, '');
      if (!einDigits) {
        newErrors.taxIdentifier = 'Employer Identification Number is required';
      } else if (einDigits.length !== 9) {
        newErrors.taxIdentifier = 'EIN must be 9 digits';
      }

      // Responsible party validation (optional but has max length)
      if (responsibleParty && responsibleParty.length > 200) {
        newErrors.responsibleParty = 'Responsible party must not exceed 200 characters';
      }
    }

    // Email validation
    if (!email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      newErrors.email = 'Invalid email address';
    }

    // Phone validation (optional but must be valid if provided)
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
  }, [clientType, firstName, lastName, businessName, entityType, responsibleParty, taxIdentifier, email, phone, address]);

  // Handle form submission
  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (!validate()) {
      return;
    }

    const taxDigits = taxIdentifier.replace(/\D/g, '');
    let formattedTaxId: string;

    if (clientType === 'Individual') {
      // Format SSN with dashes: XXX-XX-XXXX
      formattedTaxId = `${taxDigits.slice(0, 3)}-${taxDigits.slice(3, 5)}-${taxDigits.slice(5)}`;
    } else {
      // Format EIN with dash: XX-XXXXXXX
      formattedTaxId = `${taxDigits.slice(0, 2)}-${taxDigits.slice(2)}`;
    }

    const request: CreateClientRequest = {
      clientType,
      ...(clientType === 'Individual'
        ? {
            firstName: firstName.trim(),
            lastName: lastName.trim(),
          }
        : {
            businessName: businessName.trim(),
            entityType: entityType as BusinessEntityType,
            responsibleParty: responsibleParty.trim() || undefined,
          }),
      taxIdentifier: formattedTaxId,
      email: email.trim().toLowerCase(),
      phone: phone.trim() || undefined,
      address: {
        street1: address.street1.trim(),
        street2: address.street2?.trim() || undefined,
        city: address.city.trim(),
        state: address.state.trim(),
        postalCode: address.postalCode.trim(),
        country: address.country.trim(),
      },
      notes: notes.trim() || undefined,
    };

    try {
      const result = await createClient(request).unwrap();
      router.push(`/clients/${result.id}`);
    } catch (error: unknown) {
      // Handle API errors
      const apiError = error as { status?: number; data?: { detail?: string } };
      if (apiError.status === 409) {
        setErrors({
          taxIdentifier: clientType === 'Individual'
            ? 'A client with this SSN already exists'
            : 'A client with this EIN already exists',
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

  return (
    <Paper sx={{ p: 3 }}>
      <form onSubmit={handleSubmit}>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
          {/* General error alert */}
          {errors.general && (
            <Alert severity="error" onClose={() => setErrors((e) => ({ ...e, general: undefined }))}>
              {errors.general}
            </Alert>
          )}

          {/* Client Type */}
          <FormControl component="fieldset">
            <FormLabel component="legend">Client Type</FormLabel>
            <RadioGroup row value={clientType} onChange={handleClientTypeChange}>
              <FormControlLabel value="Individual" control={<Radio />} label="Individual" disabled={isLoading} />
              <FormControlLabel value="Business" control={<Radio />} label="Business" disabled={isLoading} />
            </RadioGroup>
          </FormControl>

          {/* Individual fields */}
          {clientType === 'Individual' && (
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
                  disabled={isLoading}
                  required
                  fullWidth
                />
                <TextField
                  label="Last Name"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                  error={!!errors.lastName}
                  helperText={errors.lastName}
                  disabled={isLoading}
                  required
                  fullWidth
                />
              </Box>
            </>
          )}

          {/* Business fields */}
          {clientType === 'Business' && (
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
                disabled={isLoading}
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
                  disabled={isLoading}
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
                  error={!!errors.responsibleParty}
                  helperText={errors.responsibleParty || 'Name of person responsible for the business'}
                  disabled={isLoading}
                  fullWidth
                />
              </Box>
            </>
          )}

          {/* Tax identifier */}
          <TaxIdentifierInput
            clientType={clientType}
            value={taxIdentifier}
            onChange={setTaxIdentifier}
            error={!!errors.taxIdentifier}
            helperText={errors.taxIdentifier}
            disabled={isLoading}
            required
            fullWidth
          />

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
              disabled={isLoading}
              required
              fullWidth
            />
            <TextField
              label="Phone"
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              error={!!errors.phone}
              helperText={errors.phone}
              disabled={isLoading}
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
            disabled={isLoading}
          />

          {/* Notes */}
          <TextField
            label="Notes"
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            disabled={isLoading}
            multiline
            rows={3}
            fullWidth
            placeholder="Optional notes about this client..."
          />

          {/* Action buttons */}
          <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end', mt: 2 }}>
            <Button variant="outlined" onClick={handleCancel} disabled={isLoading}>
              Cancel
            </Button>
            <Button type="submit" variant="contained" disabled={isLoading}>
              {isLoading ? 'Creating...' : 'Create Client'}
            </Button>
          </Box>
        </Box>
      </form>
    </Paper>
  );
}

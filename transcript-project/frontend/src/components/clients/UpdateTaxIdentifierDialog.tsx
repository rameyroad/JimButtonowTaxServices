'use client';

import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Typography,
} from '@mui/material';
import { useState } from 'react';
import { useUpdateTaxIdentifierMutation } from '@/lib/api/clientsApi';
import type { ClientType } from '@/lib/types/client';
import { TaxIdentifierInput } from './TaxIdentifierInput';

interface UpdateTaxIdentifierDialogProps {
  open: boolean;
  onClose: () => void;
  clientId: string;
  clientType: ClientType;
  currentLast4: string;
  version: number;
}

export function UpdateTaxIdentifierDialog({
  open,
  onClose,
  clientId,
  clientType,
  currentLast4,
  version,
}: UpdateTaxIdentifierDialogProps) {
  const [taxIdentifier, setTaxIdentifier] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [updateTaxIdentifier, { isLoading }] = useUpdateTaxIdentifierMutation();

  const isSSN = clientType === 'Individual';
  const currentMasked = isSSN ? `***-**-${currentLast4}` : `**-***${currentLast4}`;
  const digits = taxIdentifier.replace(/\D/g, '');
  const isValid = digits.length === 9;

  const handleSubmit = async () => {
    setError(null);

    try {
      await updateTaxIdentifier({
        id: clientId,
        data: { taxIdentifier, clientType, version },
      }).unwrap();
      handleClose();
    } catch (err: unknown) {
      const apiError = err as { status?: number; data?: { detail?: string } };
      if (apiError.status === 409) {
        const detail = apiError.data?.detail ?? '';
        if (detail.toLowerCase().includes('modified by another user')) {
          setError('This client was modified by another user. Please refresh the page and try again.');
        } else {
          setError('A client with this tax identifier already exists.');
        }
      } else if (apiError.status === 400) {
        setError(apiError.data?.detail ?? 'Invalid tax identifier format.');
      } else {
        setError('Failed to update tax identifier. Please try again.');
      }
    }
  };

  const handleClose = () => {
    setTaxIdentifier('');
    setError(null);
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Update {isSSN ? 'SSN' : 'EIN'}</DialogTitle>
      <DialogContent>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          Current: {currentMasked}
        </Typography>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}
        <TaxIdentifierInput
          clientType={clientType}
          value={taxIdentifier}
          onChange={setTaxIdentifier}
          fullWidth
          autoFocus
          sx={{ mt: 1 }}
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} disabled={isLoading}>
          Cancel
        </Button>
        <Button onClick={handleSubmit} variant="contained" disabled={!isValid || isLoading}>
          {isLoading ? 'Updating...' : 'Update'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}

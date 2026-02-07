'use client';

import { TextField, type TextFieldProps } from '@mui/material';
import { forwardRef, useCallback, useState } from 'react';
import type { ClientType } from '@/lib/types/client';

interface TaxIdentifierInputProps extends Omit<TextFieldProps, 'onChange' | 'value'> {
  clientType: ClientType;
  value: string;
  onChange: (value: string) => void;
}

/**
 * Tax identifier input with formatting for SSN (Individual) or EIN (Business).
 * - SSN format: XXX-XX-XXXX
 * - EIN format: XX-XXXXXXX
 */
export const TaxIdentifierInput = forwardRef<HTMLInputElement, TaxIdentifierInputProps>(
  function TaxIdentifierInput({ clientType, value, onChange, ...props }, ref) {
    const [isFocused, setIsFocused] = useState(false);

    const isSSN = clientType === 'Individual';
    const placeholder = isSSN ? 'XXX-XX-XXXX' : 'XX-XXXXXXX';
    const label = isSSN ? 'Social Security Number' : 'Employer Identification Number';
    const maxLength = isSSN ? 11 : 10; // With dashes

    // Format the value for display
    const formatValue = useCallback(
      (raw: string): string => {
        // Remove all non-digits
        const digits = raw.replace(/\D/g, '');

        if (isSSN) {
          // SSN: XXX-XX-XXXX
          if (digits.length <= 3) return digits;
          if (digits.length <= 5) return `${digits.slice(0, 3)}-${digits.slice(3)}`;
          return `${digits.slice(0, 3)}-${digits.slice(3, 5)}-${digits.slice(5, 9)}`;
        } else {
          // EIN: XX-XXXXXXX
          if (digits.length <= 2) return digits;
          return `${digits.slice(0, 2)}-${digits.slice(2, 9)}`;
        }
      },
      [isSSN]
    );

    // Mask the value when not focused (show last 4 digits only)
    const getMaskedValue = useCallback(
      (raw: string): string => {
        const digits = raw.replace(/\D/g, '');
        if (digits.length === 0) return '';

        const last4 = digits.slice(-4);
        if (isSSN) {
          return `***-**-${last4}`;
        } else {
          return `**-***${last4}`;
        }
      },
      [isSSN]
    );

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
      const input = event.target.value;
      // Only allow digits and dashes
      const cleaned = input.replace(/[^\d-]/g, '');
      // Store only digits internally
      const digitsOnly = cleaned.replace(/\D/g, '');
      onChange(digitsOnly);
    };

    const handleFocus = () => {
      setIsFocused(true);
    };

    const handleBlur = () => {
      setIsFocused(false);
    };

    // Determine what to display
    const displayValue = isFocused ? formatValue(value) : getMaskedValue(value);

    // Validation helper text
    const getHelperText = () => {
      const digits = value.replace(/\D/g, '');
      if (digits.length > 0 && digits.length < 9) {
        return `${isSSN ? 'SSN' : 'EIN'} must be 9 digits`;
      }
      return props.helperText;
    };

    const isError = value.length > 0 && value.replace(/\D/g, '').length < 9;

    return (
      <TextField
        {...props}
        ref={ref}
        label={label}
        placeholder={placeholder}
        value={displayValue}
        onChange={handleChange}
        onFocus={handleFocus}
        onBlur={handleBlur}
        error={isError || props.error}
        helperText={getHelperText()}
        slotProps={{
          htmlInput: {
            maxLength,
            inputMode: 'numeric',
            autoComplete: 'off',
          },
        }}
      />
    );
  }
);

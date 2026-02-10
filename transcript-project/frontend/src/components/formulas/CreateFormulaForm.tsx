'use client';

import { useState } from 'react';
import {
  Box,
  TextField,
  Button,
  Paper,
  Alert,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Typography,
} from '@mui/material';
import { useRouter } from 'next/navigation';
import { useCreateFormulaMutation } from '@/lib/api/formulasApi';
import type { DataType } from '@/lib/types/decisionTable';

const OUTPUT_TYPES: DataType[] = ['Number', 'String', 'Boolean', 'Date'];

export default function CreateFormulaForm() {
  const router = useRouter();
  const [createFormula, { isLoading, error }] = useCreateFormulaMutation();

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [expression, setExpression] = useState('');
  const [outputType, setOutputType] = useState<DataType>('Number');
  const [inputVariables, setInputVariables] = useState('[]');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const result = await createFormula({
        name,
        description: description || undefined,
        expression,
        inputVariables,
        outputType,
      }).unwrap();

      router.push(`/platform/formulas/${result.id}`);
    } catch {
      // Error handled by RTK Query state
    }
  };

  return (
    <Paper sx={{ p: 3, maxWidth: 700 }}>
      <form onSubmit={handleSubmit}>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2.5 }}>
          {error && (
            <Alert severity="error">
              {'data' in error
                ? JSON.stringify((error as { data: unknown }).data)
                : 'Failed to create formula'}
            </Alert>
          )}

          <TextField
            label="Formula Name"
            required
            value={name}
            onChange={(e) => setName(e.target.value)}
            inputProps={{ maxLength: 200 }}
            helperText="A descriptive name (e.g., RCP Calculation, Payment Amount)"
          />

          <TextField
            label="Description"
            multiline
            rows={2}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            inputProps={{ maxLength: 2000 }}
          />

          <TextField
            label="Expression"
            required
            multiline
            rows={3}
            value={expression}
            onChange={(e) => setExpression(e.target.value)}
            inputProps={{ maxLength: 4000 }}
            helperText="e.g., assets + future_income - allowable_expenses. Supports: +, -, *, /, min(), max(), round(), abs(), if/then/else"
            sx={{ fontFamily: 'monospace' }}
          />

          <TextField
            label="Input Variables (JSON)"
            required
            multiline
            rows={4}
            value={inputVariables}
            onChange={(e) => setInputVariables(e.target.value)}
            inputProps={{ maxLength: 4000 }}
            helperText='JSON array: [{"name": "assets", "dataType": "Number", "description": "Total assets"}]'
            sx={{ fontFamily: 'monospace' }}
          />

          <FormControl>
            <InputLabel>Output Type</InputLabel>
            <Select
              value={outputType}
              label="Output Type"
              onChange={(e) => setOutputType(e.target.value as DataType)}
            >
              {OUTPUT_TYPES.map((type) => (
                <MenuItem key={type} value={type}>{type}</MenuItem>
              ))}
            </Select>
            <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, ml: 1.75 }}>
              The data type of the formula result
            </Typography>
          </FormControl>

          <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end', mt: 1 }}>
            <Button
              variant="outlined"
              onClick={() => router.push('/platform/formulas')}
            >
              Cancel
            </Button>
            <Button
              type="submit"
              variant="contained"
              disabled={!name.trim() || !expression.trim() || isLoading}
            >
              {isLoading ? 'Creating...' : 'Create Formula'}
            </Button>
          </Box>
        </Box>
      </form>
    </Paper>
  );
}

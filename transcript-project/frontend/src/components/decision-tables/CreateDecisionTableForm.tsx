'use client';

import { useState } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
  Divider,
  FormControl,
  IconButton,
  InputLabel,
  MenuItem,
  Select,
  Switch,
  FormControlLabel,
  TextField,
  Typography,
  Alert,
} from '@mui/material';
import { Add, Delete } from '@mui/icons-material';
import { useRouter } from 'next/navigation';
import { useCreateDecisionTableMutation } from '@/lib/api/decisionTablesApi';
import type { ColumnDefinition, DataType } from '@/lib/types/decisionTable';

export default function CreateDecisionTableForm() {
  const router = useRouter();
  const [createTable, { isLoading }] = useCreateDecisionTableMutation();
  const [error, setError] = useState<string | null>(null);

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [columns, setColumns] = useState<ColumnDefinition[]>([
    { name: '', key: '', dataType: 'String', isInput: true, sortOrder: 1 },
    { name: '', key: '', dataType: 'String', isInput: false, sortOrder: 2 },
  ]);

  const handleAddColumn = () => {
    setColumns([
      ...columns,
      { name: '', key: '', dataType: 'String', isInput: true, sortOrder: columns.length + 1 },
    ]);
  };

  const handleRemoveColumn = (index: number) => {
    setColumns(columns.filter((_, i) => i !== index));
  };

  const handleColumnChange = (index: number, field: keyof ColumnDefinition, value: string | boolean) => {
    const updated = [...columns];
    updated[index] = { ...updated[index], [field]: value };
    // Auto-generate key from name
    if (field === 'name' && typeof value === 'string') {
      updated[index].key = value
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, '_')
        .replace(/^_|_$/g, '');
    }
    setColumns(updated);
  };

  const handleSubmit = async () => {
    try {
      setError(null);
      const result = await createTable({
        name,
        description: description || undefined,
        columns,
      }).unwrap();
      router.push(`/platform/decision-tables/${result.id}`);
    } catch (err: unknown) {
      const apiError = err as { data?: { detail?: string; errors?: Record<string, string[]> } };
      if (apiError.data?.errors) {
        const messages = Object.values(apiError.data.errors).flat();
        setError(messages.join(', '));
      } else {
        setError(apiError.data?.detail || 'Failed to create decision table');
      }
    }
  };

  const hasInput = columns.some((c) => c.isInput);
  const hasOutput = columns.some((c) => !c.isInput);
  const allColumnsNamed = columns.every((c) => c.name.trim() !== '');
  const canSubmit = name.trim() !== '' && hasInput && hasOutput && allColumnsNamed && !isLoading;

  return (
    <Box>
      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" sx={{ mb: 2 }}>Basic Information</Typography>
          <TextField
            label="Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            fullWidth
            required
            sx={{ mb: 2 }}
          />
          <TextField
            label="Description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            fullWidth
            multiline
            rows={3}
          />
        </CardContent>
      </Card>

      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
            <Typography variant="h6">Columns</Typography>
            <Button startIcon={<Add />} onClick={handleAddColumn} size="small">
              Add Column
            </Button>
          </Box>

          {columns.map((col, index) => (
            <Box key={index}>
              {index > 0 && <Divider sx={{ my: 1 }} />}
              <Box sx={{ display: 'flex', gap: 2, alignItems: 'center' }}>
                <TextField
                  size="small"
                  label="Name"
                  value={col.name}
                  onChange={(e) => handleColumnChange(index, 'name', e.target.value)}
                  sx={{ flex: 1 }}
                  required
                />
                <TextField
                  size="small"
                  label="Key"
                  value={col.key}
                  onChange={(e) => handleColumnChange(index, 'key', e.target.value)}
                  sx={{ flex: 1 }}
                  required
                  helperText="Auto-generated from name"
                />
                <FormControl size="small" sx={{ width: 130 }}>
                  <InputLabel>Data Type</InputLabel>
                  <Select
                    value={col.dataType}
                    label="Data Type"
                    onChange={(e) => handleColumnChange(index, 'dataType', e.target.value as DataType)}
                  >
                    <MenuItem value="String">String</MenuItem>
                    <MenuItem value="Number">Number</MenuItem>
                    <MenuItem value="Boolean">Boolean</MenuItem>
                    <MenuItem value="Date">Date</MenuItem>
                  </Select>
                </FormControl>
                <FormControlLabel
                  control={
                    <Switch
                      checked={col.isInput}
                      onChange={(e) => handleColumnChange(index, 'isInput', e.target.checked)}
                    />
                  }
                  label={col.isInput ? 'Input' : 'Output'}
                  sx={{ width: 100 }}
                />
                <IconButton
                  size="small"
                  color="error"
                  onClick={() => handleRemoveColumn(index)}
                  disabled={columns.length <= 2}
                >
                  <Delete fontSize="small" />
                </IconButton>
              </Box>
            </Box>
          ))}

          {(!hasInput || !hasOutput) && (
            <Alert severity="info" sx={{ mt: 2 }}>
              You need at least one input and one output column.
            </Alert>
          )}
        </CardContent>
      </Card>

      <Box sx={{ display: 'flex', gap: 2 }}>
        <Button
          variant="contained"
          onClick={handleSubmit}
          disabled={!canSubmit}
        >
          Create Decision Table
        </Button>
        <Button onClick={() => router.push('/platform/decision-tables')}>
          Cancel
        </Button>
      </Box>
    </Box>
  );
}

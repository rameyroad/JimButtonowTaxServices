'use client';

import { useState } from 'react';
import {
  Box,
  TextField,
  Button,
  Paper,
  Typography,
  Alert,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from '@mui/material';
import { useRouter } from 'next/navigation';
import { useCreateWorkflowDefinitionMutation } from '@/lib/api/workflowsApi';

const WORKFLOW_CATEGORIES = ['Analysis', 'Decision', 'Execution', 'Document'];

export default function CreateWorkflowForm() {
  const router = useRouter();
  const [createWorkflow, { isLoading, error }] = useCreateWorkflowDefinitionMutation();

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [category, setCategory] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const result = await createWorkflow({
        name,
        description: description || undefined,
        category: category || undefined,
      }).unwrap();

      router.push(`/platform/workflows/${result.id}`);
    } catch {
      // Error handled by RTK Query state
    }
  };

  return (
    <Paper sx={{ p: 3, maxWidth: 600 }}>
      <form onSubmit={handleSubmit}>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2.5 }}>
          {error && (
            <Alert severity="error">
              {'data' in error
                ? JSON.stringify((error as { data: unknown }).data)
                : 'Failed to create workflow'}
            </Alert>
          )}

          <TextField
            label="Workflow Name"
            required
            value={name}
            onChange={(e) => setName(e.target.value)}
            inputProps={{ maxLength: 200 }}
            helperText="A descriptive name for this workflow"
          />

          <TextField
            label="Description"
            multiline
            rows={3}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            inputProps={{ maxLength: 2000 }}
            helperText="Optional description of what this workflow does"
          />

          <FormControl>
            <InputLabel>Category</InputLabel>
            <Select
              value={category}
              label="Category"
              onChange={(e) => setCategory(e.target.value)}
            >
              <MenuItem value="">
                <em>None</em>
              </MenuItem>
              {WORKFLOW_CATEGORIES.map((cat) => (
                <MenuItem key={cat} value={cat}>
                  {cat}
                </MenuItem>
              ))}
            </Select>
            <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, ml: 1.75 }}>
              Optional category for organizing workflows
            </Typography>
          </FormControl>

          <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end', mt: 1 }}>
            <Button
              variant="outlined"
              onClick={() => router.push('/platform/workflows')}
            >
              Cancel
            </Button>
            <Button
              type="submit"
              variant="contained"
              disabled={!name.trim() || isLoading}
            >
              {isLoading ? 'Creating...' : 'Create Workflow'}
            </Button>
          </Box>
        </Box>
      </form>
    </Paper>
  );
}

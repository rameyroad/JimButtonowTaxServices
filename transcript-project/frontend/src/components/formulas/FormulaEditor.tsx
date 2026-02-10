'use client';

import { useState } from 'react';
import {
  Box,
  Button,
  Chip,
  Paper,
  TextField,
  Typography,
  Alert,
  CircularProgress,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Divider,
} from '@mui/material';
import PublishIcon from '@mui/icons-material/Publish';
import UnpublishedIcon from '@mui/icons-material/Unpublished';
import SaveIcon from '@mui/icons-material/Save';
import {
  useGetFormulaQuery,
  useUpdateFormulaMutation,
  usePublishFormulaMutation,
  useUnpublishFormulaMutation,
} from '@/lib/api/formulasApi';
import type { DataType, PublishStatus } from '@/lib/types/decisionTable';

const statusColors: Record<PublishStatus, 'default' | 'success' | 'warning'> = {
  Draft: 'default',
  Published: 'success',
  Archived: 'warning',
};

const OUTPUT_TYPES: DataType[] = ['Number', 'String', 'Boolean', 'Date'];

interface FormulaEditorProps {
  formulaId: string;
}

export default function FormulaEditor({ formulaId }: FormulaEditorProps) {
  const { data: formula, isLoading, error } = useGetFormulaQuery(formulaId);
  const [updateFormula, { isLoading: isUpdating }] = useUpdateFormulaMutation();
  const [publishFormula, { isLoading: isPublishing }] = usePublishFormulaMutation();
  const [unpublishFormula, { isLoading: isUnpublishing }] = useUnpublishFormulaMutation();

  const [isEditing, setIsEditing] = useState(false);
  const [editName, setEditName] = useState('');
  const [editDescription, setEditDescription] = useState('');
  const [editExpression, setEditExpression] = useState('');
  const [editInputVariables, setEditInputVariables] = useState('');
  const [editOutputType, setEditOutputType] = useState<DataType>('Number');
  const [saveError, setSaveError] = useState<string | null>(null);

  const startEditing = () => {
    if (!formula) return;
    setEditName(formula.name);
    setEditDescription(formula.description ?? '');
    setEditExpression(formula.expression);
    setEditInputVariables(formula.inputVariables);
    setEditOutputType(formula.outputType);
    setIsEditing(true);
    setSaveError(null);
  };

  const handleSave = async () => {
    if (!formula) return;
    try {
      await updateFormula({
        id: formulaId,
        data: {
          name: editName !== formula.name ? editName : undefined,
          description: editDescription !== (formula.description ?? '') ? editDescription : undefined,
          expression: editExpression !== formula.expression ? editExpression : undefined,
          inputVariables: editInputVariables !== formula.inputVariables ? editInputVariables : undefined,
          outputType: editOutputType !== formula.outputType ? editOutputType : undefined,
        },
      }).unwrap();
      setIsEditing(false);
      setSaveError(null);
    } catch {
      setSaveError('Failed to save formula.');
    }
  };

  const handlePublish = async () => {
    try {
      await publishFormula(formulaId).unwrap();
    } catch {
      setSaveError('Failed to publish formula.');
    }
  };

  const handleUnpublish = async () => {
    try {
      await unpublishFormula(formulaId).unwrap();
    } catch {
      setSaveError('Failed to unpublish formula.');
    }
  };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !formula) {
    return <Alert severity="error">Failed to load formula.</Alert>;
  }

  return (
    <Box>
      {saveError && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setSaveError(null)}>
          {saveError}
        </Alert>
      )}

      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
        <Typography variant="h5" sx={{ flexGrow: 1 }}>
          {formula.name}
        </Typography>
        <Chip label={formula.status} color={statusColors[formula.status]} />
        <Typography variant="body2" color="text.secondary">v{formula.version}</Typography>

        {formula.status === 'Draft' && (
          <Button
            variant="contained"
            color="success"
            startIcon={<PublishIcon />}
            onClick={handlePublish}
            disabled={isPublishing}
          >
            {isPublishing ? 'Publishing...' : 'Publish'}
          </Button>
        )}
        {formula.status === 'Published' && (
          <Button
            variant="outlined"
            startIcon={<UnpublishedIcon />}
            onClick={handleUnpublish}
            disabled={isUnpublishing}
          >
            {isUnpublishing ? 'Unpublishing...' : 'Unpublish'}
          </Button>
        )}
      </Box>

      {/* Formula details */}
      <Paper sx={{ p: 3 }}>
        {isEditing ? (
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2.5 }}>
            <TextField
              label="Name"
              value={editName}
              onChange={(e) => setEditName(e.target.value)}
              inputProps={{ maxLength: 200 }}
            />
            <TextField
              label="Description"
              multiline
              rows={2}
              value={editDescription}
              onChange={(e) => setEditDescription(e.target.value)}
              inputProps={{ maxLength: 2000 }}
            />
            <TextField
              label="Expression"
              multiline
              rows={3}
              value={editExpression}
              onChange={(e) => setEditExpression(e.target.value)}
              inputProps={{ maxLength: 4000 }}
              sx={{ '& textarea': { fontFamily: 'monospace' } }}
            />
            <TextField
              label="Input Variables (JSON)"
              multiline
              rows={4}
              value={editInputVariables}
              onChange={(e) => setEditInputVariables(e.target.value)}
              inputProps={{ maxLength: 4000 }}
              sx={{ '& textarea': { fontFamily: 'monospace' } }}
            />
            <FormControl>
              <InputLabel>Output Type</InputLabel>
              <Select
                value={editOutputType}
                label="Output Type"
                onChange={(e) => setEditOutputType(e.target.value as DataType)}
              >
                {OUTPUT_TYPES.map((type) => (
                  <MenuItem key={type} value={type}>{type}</MenuItem>
                ))}
              </Select>
            </FormControl>
            <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
              <Button variant="outlined" onClick={() => setIsEditing(false)}>
                Cancel
              </Button>
              <Button
                variant="contained"
                startIcon={<SaveIcon />}
                onClick={handleSave}
                disabled={isUpdating || !editName.trim() || !editExpression.trim()}
              >
                {isUpdating ? 'Saving...' : 'Save'}
              </Button>
            </Box>
          </Box>
        ) : (
          <Box>
            <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
              <Button variant="outlined" onClick={startEditing}>
                Edit
              </Button>
            </Box>

            {formula.description && (
              <Box sx={{ mb: 2 }}>
                <Typography variant="caption" color="text.secondary">Description</Typography>
                <Typography>{formula.description}</Typography>
              </Box>
            )}

            <Divider sx={{ my: 2 }} />

            <Box sx={{ mb: 2 }}>
              <Typography variant="caption" color="text.secondary">Expression</Typography>
              <Paper variant="outlined" sx={{ p: 2, mt: 0.5, bgcolor: 'grey.50' }}>
                <Typography sx={{ fontFamily: 'monospace', whiteSpace: 'pre-wrap' }}>
                  {formula.expression}
                </Typography>
              </Paper>
            </Box>

            <Box sx={{ mb: 2 }}>
              <Typography variant="caption" color="text.secondary">Output Type</Typography>
              <Box sx={{ mt: 0.5 }}>
                <Chip label={formula.outputType} size="small" variant="outlined" />
              </Box>
            </Box>

            <Box sx={{ mb: 2 }}>
              <Typography variant="caption" color="text.secondary">Input Variables</Typography>
              <Paper variant="outlined" sx={{ p: 2, mt: 0.5, bgcolor: 'grey.50' }}>
                <Typography sx={{ fontFamily: 'monospace', whiteSpace: 'pre-wrap', fontSize: '0.875rem' }}>
                  {(() => {
                    try {
                      return JSON.stringify(JSON.parse(formula.inputVariables), null, 2);
                    } catch {
                      return formula.inputVariables;
                    }
                  })()}
                </Typography>
              </Paper>
            </Box>

            <Divider sx={{ my: 2 }} />

            <Box sx={{ display: 'flex', gap: 4 }}>
              <Box>
                <Typography variant="caption" color="text.secondary">Created</Typography>
                <Typography variant="body2">
                  {new Date(formula.createdAt).toLocaleString()}
                </Typography>
              </Box>
              <Box>
                <Typography variant="caption" color="text.secondary">Updated</Typography>
                <Typography variant="body2">
                  {new Date(formula.updatedAt).toLocaleString()}
                </Typography>
              </Box>
              {formula.publishedAt && (
                <Box>
                  <Typography variant="caption" color="text.secondary">Published</Typography>
                  <Typography variant="body2">
                    {new Date(formula.publishedAt).toLocaleString()}
                  </Typography>
                </Box>
              )}
            </Box>
          </Box>
        )}
      </Paper>
    </Box>
  );
}

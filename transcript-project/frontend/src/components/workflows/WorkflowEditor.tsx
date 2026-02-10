'use client';

import { useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  Button,
  Chip,
  CircularProgress,
  Alert,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Switch,
  FormControlLabel,
} from '@mui/material';
import {
  Add,
  Delete,
  Edit,
  Publish,
  Unpublished,
} from '@mui/icons-material';
import {
  useGetWorkflowDefinitionQuery,
  useUpdateWorkflowDefinitionMutation,
  useAddWorkflowStepMutation,
  useUpdateWorkflowStepMutation,
  useRemoveWorkflowStepMutation,
  usePublishWorkflowMutation,
  useUnpublishWorkflowMutation,
} from '@/lib/api/workflowsApi';
import type { PublishStatus } from '@/lib/types/decisionTable';
import type { WorkflowStepType, WorkflowStep } from '@/lib/types/workflow';

const statusColors: Record<PublishStatus, 'default' | 'success' | 'warning'> = {
  Draft: 'default',
  Published: 'success',
  Archived: 'warning',
};

const STEP_TYPES: WorkflowStepType[] = [
  'DecisionTable',
  'Calculation',
  'HumanTask',
  'ClientApproval',
  'DocumentGeneration',
];

interface WorkflowEditorProps {
  id: string;
}

interface StepFormData {
  name: string;
  stepType: WorkflowStepType;
  sortOrder: number;
  configuration: string;
  isRequired: boolean;
}

const defaultStepForm: StepFormData = {
  name: '',
  stepType: 'DecisionTable',
  sortOrder: 0,
  configuration: '',
  isRequired: true,
};

export default function WorkflowEditor({ id }: WorkflowEditorProps) {
  const { data: workflow, isLoading, error } = useGetWorkflowDefinitionQuery(id);
  const [updateWorkflow] = useUpdateWorkflowDefinitionMutation();
  const [addStep] = useAddWorkflowStepMutation();
  const [updateStep] = useUpdateWorkflowStepMutation();
  const [removeStep] = useRemoveWorkflowStepMutation();
  const [publishWorkflow] = usePublishWorkflowMutation();
  const [unpublishWorkflow] = useUnpublishWorkflowMutation();

  const [editingName, setEditingName] = useState(false);
  const [nameValue, setNameValue] = useState('');
  const [stepDialogOpen, setStepDialogOpen] = useState(false);
  const [editingStep, setEditingStep] = useState<WorkflowStep | null>(null);
  const [stepForm, setStepForm] = useState<StepFormData>(defaultStepForm);

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !workflow) {
    return <Alert severity="error">Failed to load workflow</Alert>;
  }

  const handleNameSave = async () => {
    if (nameValue.trim() && nameValue !== workflow.name) {
      await updateWorkflow({ id, data: { name: nameValue.trim() } });
    }
    setEditingName(false);
  };

  const handlePublish = async () => {
    await publishWorkflow(id);
  };

  const handleUnpublish = async () => {
    await unpublishWorkflow(id);
  };

  const openAddStep = () => {
    setEditingStep(null);
    setStepForm({
      ...defaultStepForm,
      sortOrder: workflow.steps.length,
    });
    setStepDialogOpen(true);
  };

  const openEditStep = (step: WorkflowStep) => {
    setEditingStep(step);
    setStepForm({
      name: step.name,
      stepType: step.stepType,
      sortOrder: step.sortOrder,
      configuration: step.configuration || '',
      isRequired: step.isRequired,
    });
    setStepDialogOpen(true);
  };

  const handleStepSave = async () => {
    if (editingStep) {
      await updateStep({
        workflowId: id,
        stepId: editingStep.id,
        data: {
          name: stepForm.name,
          stepType: stepForm.stepType,
          sortOrder: stepForm.sortOrder,
          configuration: stepForm.configuration || undefined,
          isRequired: stepForm.isRequired,
        },
      });
    } else {
      await addStep({
        workflowId: id,
        data: {
          name: stepForm.name,
          stepType: stepForm.stepType,
          sortOrder: stepForm.sortOrder,
          configuration: stepForm.configuration || undefined,
          isRequired: stepForm.isRequired,
        },
      });
    }
    setStepDialogOpen(false);
  };

  const handleRemoveStep = async (stepId: string) => {
    await removeStep({ workflowId: id, stepId });
  };

  const sortedSteps = [...workflow.steps].sort((a, b) => a.sortOrder - b.sortOrder);

  return (
    <Box>
      {/* Header */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <Box sx={{ flex: 1 }}>
            {editingName ? (
              <TextField
                value={nameValue}
                onChange={(e) => setNameValue(e.target.value)}
                onBlur={handleNameSave}
                onKeyDown={(e) => { if (e.key === 'Enter') handleNameSave(); }}
                size="small"
                autoFocus
                sx={{ mb: 1 }}
              />
            ) : (
              <Typography
                variant="h5"
                fontWeight={600}
                onClick={() => { setNameValue(workflow.name); setEditingName(true); }}
                sx={{ cursor: 'pointer', '&:hover': { color: 'primary.main' }, mb: 1 }}
              >
                {workflow.name}
              </Typography>
            )}
            {workflow.description && (
              <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                {workflow.description}
              </Typography>
            )}
            <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
              <Chip label={workflow.status} size="small" color={statusColors[workflow.status]} />
              {workflow.category && (
                <Chip label={workflow.category} size="small" variant="outlined" />
              )}
              <Typography variant="caption" color="text.secondary">
                v{workflow.currentVersion}
              </Typography>
            </Box>
          </Box>
          <Box sx={{ display: 'flex', gap: 1 }}>
            {workflow.status === 'Draft' ? (
              <Button
                variant="contained"
                startIcon={<Publish />}
                onClick={handlePublish}
                disabled={workflow.steps.length === 0}
              >
                Publish
              </Button>
            ) : workflow.status === 'Published' ? (
              <Button
                variant="outlined"
                startIcon={<Unpublished />}
                onClick={handleUnpublish}
              >
                Unpublish
              </Button>
            ) : null}
          </Box>
        </Box>
      </Paper>

      {/* Steps */}
      <Paper sx={{ p: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h6">
            Steps ({sortedSteps.length})
          </Typography>
          <Button
            variant="outlined"
            startIcon={<Add />}
            onClick={openAddStep}
            size="small"
          >
            Add Step
          </Button>
        </Box>

        {sortedSteps.length === 0 ? (
          <Typography color="text.secondary" sx={{ py: 2, textAlign: 'center' }}>
            No steps defined. Add steps to build the workflow.
          </Typography>
        ) : (
          <TableContainer>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Order</TableCell>
                  <TableCell>Name</TableCell>
                  <TableCell>Type</TableCell>
                  <TableCell>Required</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {sortedSteps.map((step) => (
                  <TableRow key={step.id}>
                    <TableCell>{step.sortOrder + 1}</TableCell>
                    <TableCell>
                      <Typography variant="body2" fontWeight={500}>
                        {step.name}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Chip label={step.stepType} size="small" variant="outlined" />
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={step.isRequired ? 'Required' : 'Optional'}
                        size="small"
                        color={step.isRequired ? 'primary' : 'default'}
                        variant="outlined"
                      />
                    </TableCell>
                    <TableCell align="right">
                      <IconButton size="small" onClick={() => openEditStep(step)}>
                        <Edit fontSize="small" />
                      </IconButton>
                      <IconButton
                        size="small"
                        color="error"
                        onClick={() => handleRemoveStep(step.id)}
                      >
                        <Delete fontSize="small" />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </Paper>

      {/* Add/Edit Step Dialog */}
      <Dialog
        open={stepDialogOpen}
        onClose={() => setStepDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>
          {editingStep ? 'Edit Step' : 'Add Step'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <TextField
              label="Step Name"
              required
              value={stepForm.name}
              onChange={(e) => setStepForm({ ...stepForm, name: e.target.value })}
              inputProps={{ maxLength: 200 }}
            />

            <FormControl>
              <InputLabel>Step Type</InputLabel>
              <Select
                value={stepForm.stepType}
                label="Step Type"
                onChange={(e) => setStepForm({ ...stepForm, stepType: e.target.value as WorkflowStepType })}
              >
                {STEP_TYPES.map((type) => (
                  <MenuItem key={type} value={type}>
                    {type}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            <TextField
              label="Sort Order"
              type="number"
              value={stepForm.sortOrder}
              onChange={(e) => setStepForm({ ...stepForm, sortOrder: parseInt(e.target.value, 10) || 0 })}
              helperText="Execution order (0-based)"
            />

            <TextField
              label="Configuration (JSON)"
              multiline
              rows={3}
              value={stepForm.configuration}
              onChange={(e) => setStepForm({ ...stepForm, configuration: e.target.value })}
              helperText='Step-specific config, e.g. {"decisionTableId": "..."}'
            />

            <FormControlLabel
              control={
                <Switch
                  checked={stepForm.isRequired}
                  onChange={(e) => setStepForm({ ...stepForm, isRequired: e.target.checked })}
                />
              }
              label="Required step"
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setStepDialogOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleStepSave}
            disabled={!stepForm.name.trim()}
          >
            {editingStep ? 'Update' : 'Add'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

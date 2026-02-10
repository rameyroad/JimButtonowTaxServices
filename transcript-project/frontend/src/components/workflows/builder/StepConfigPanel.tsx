'use client';

import {
  Box,
  Typography,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Switch,
  FormControlLabel,
  Button,
  Divider,
  Paper,
  IconButton,
} from '@mui/material';
import { Close, Save } from '@mui/icons-material';
import type { WorkflowStepType } from '@/lib/types/workflow';

const STEP_TYPES: WorkflowStepType[] = [
  'DecisionTable',
  'Calculation',
  'HumanTask',
  'ClientApproval',
  'DocumentGeneration',
];

export interface StepConfigData {
  stepId: string;
  name: string;
  stepType: WorkflowStepType;
  sortOrder: number;
  configuration: string;
  isRequired: boolean;
}

interface StepConfigPanelProps {
  step: StepConfigData;
  onChange: (updated: StepConfigData) => void;
  onSave: () => void;
  onClose: () => void;
}

export default function StepConfigPanel({ step, onChange, onSave, onClose }: StepConfigPanelProps) {
  return (
    <Paper
      elevation={3}
      sx={{
        width: 320,
        height: '100%',
        overflow: 'auto',
        borderLeft: 1,
        borderColor: 'divider',
        flexShrink: 0,
      }}
    >
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', px: 2, py: 1.5, borderBottom: 1, borderColor: 'divider' }}>
        <Typography variant="subtitle1" fontWeight={600}>
          Step Configuration
        </Typography>
        <IconButton size="small" onClick={onClose}>
          <Close fontSize="small" />
        </IconButton>
      </Box>

      <Box sx={{ p: 2, display: 'flex', flexDirection: 'column', gap: 2 }}>
        <TextField
          label="Step Name"
          required
          fullWidth
          size="small"
          value={step.name}
          onChange={(e) => onChange({ ...step, name: e.target.value })}
          inputProps={{ maxLength: 200 }}
        />

        <FormControl size="small" fullWidth>
          <InputLabel>Step Type</InputLabel>
          <Select
            value={step.stepType}
            label="Step Type"
            onChange={(e) => onChange({ ...step, stepType: e.target.value as WorkflowStepType })}
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
          size="small"
          fullWidth
          value={step.sortOrder}
          onChange={(e) => onChange({ ...step, sortOrder: parseInt(e.target.value, 10) || 0 })}
          helperText="Execution order (0-based)"
        />

        <FormControlLabel
          control={
            <Switch
              checked={step.isRequired}
              onChange={(e) => onChange({ ...step, isRequired: e.target.checked })}
              size="small"
            />
          }
          label="Required step"
        />

        <Divider />

        <TextField
          label="Configuration (JSON)"
          multiline
          rows={6}
          fullWidth
          size="small"
          value={step.configuration}
          onChange={(e) => onChange({ ...step, configuration: e.target.value })}
          helperText='e.g. {"decisionTableId": "..."}'
          slotProps={{
            input: { sx: { fontFamily: 'monospace', fontSize: '0.8rem' } },
          }}
        />

        <Button
          variant="contained"
          startIcon={<Save />}
          onClick={onSave}
          disabled={!step.name.trim()}
          fullWidth
        >
          Save Changes
        </Button>
      </Box>
    </Paper>
  );
}

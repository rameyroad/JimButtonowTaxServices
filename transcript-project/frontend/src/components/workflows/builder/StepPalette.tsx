'use client';

import { Box, Typography, Paper } from '@mui/material';
import {
  TableChart,
  Calculate,
  PersonSearch,
  ThumbUpAlt,
  Description,
} from '@mui/icons-material';
import type { WorkflowStepType } from '@/lib/types/workflow';

const paletteItems: { type: WorkflowStepType; label: string; icon: React.ReactNode }[] = [
  { type: 'DecisionTable', label: 'Decision Table', icon: <TableChart fontSize="small" /> },
  { type: 'Calculation', label: 'Calculation', icon: <Calculate fontSize="small" /> },
  { type: 'HumanTask', label: 'Human Task', icon: <PersonSearch fontSize="small" /> },
  { type: 'ClientApproval', label: 'Client Approval', icon: <ThumbUpAlt fontSize="small" /> },
  { type: 'DocumentGeneration', label: 'Document Gen', icon: <Description fontSize="small" /> },
];

interface StepPaletteProps {
  onAddStep: (stepType: WorkflowStepType) => void;
}

export default function StepPalette({ onAddStep }: StepPaletteProps) {
  return (
    <Paper
      elevation={2}
      sx={{
        width: 180,
        flexShrink: 0,
        borderRight: 1,
        borderColor: 'divider',
        overflow: 'auto',
      }}
    >
      <Box sx={{ px: 1.5, py: 1.5, borderBottom: 1, borderColor: 'divider' }}>
        <Typography variant="subtitle2" fontWeight={600}>
          Step Types
        </Typography>
        <Typography variant="caption" color="text.secondary">
          Click to add a step
        </Typography>
      </Box>

      <Box sx={{ p: 1, display: 'flex', flexDirection: 'column', gap: 0.5 }}>
        {paletteItems.map((item) => (
          <Box
            key={item.type}
            onClick={() => onAddStep(item.type)}
            sx={{
              display: 'flex',
              alignItems: 'center',
              gap: 1,
              px: 1.5,
              py: 1,
              borderRadius: 1,
              cursor: 'pointer',
              border: 1,
              borderColor: 'divider',
              bgcolor: 'background.paper',
              '&:hover': {
                bgcolor: 'action.hover',
                borderColor: 'primary.main',
              },
              transition: 'all 0.15s',
            }}
          >
            {item.icon}
            <Typography variant="caption" fontWeight={500}>
              {item.label}
            </Typography>
          </Box>
        ))}
      </Box>
    </Paper>
  );
}

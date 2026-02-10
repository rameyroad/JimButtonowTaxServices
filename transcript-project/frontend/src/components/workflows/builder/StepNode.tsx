'use client';

import { memo } from 'react';
import { Handle, Position } from '@xyflow/react';
import type { NodeProps, Node } from '@xyflow/react';
import { Box, Typography, Chip, IconButton } from '@mui/material';
import {
  TableChart,
  Calculate,
  PersonSearch,
  ThumbUpAlt,
  Description,
  Delete,
} from '@mui/icons-material';
import type { WorkflowStepType } from '@/lib/types/workflow';

export type StepNodeData = {
  stepId: string;
  label: string;
  stepType: WorkflowStepType;
  isRequired: boolean;
  configuration?: string;
  sortOrder: number;
  onDelete?: (stepId: string) => void;
  [key: string]: unknown;
};

export type StepNodeType = Node<StepNodeData, 'step'>;

const stepTypeConfig: Record<WorkflowStepType, { icon: React.ReactNode; color: string; bgColor: string }> = {
  DecisionTable: { icon: <TableChart fontSize="small" />, color: '#1565c0', bgColor: '#e3f2fd' },
  Calculation: { icon: <Calculate fontSize="small" />, color: '#6a1b9a', bgColor: '#f3e5f5' },
  HumanTask: { icon: <PersonSearch fontSize="small" />, color: '#e65100', bgColor: '#fff3e0' },
  ClientApproval: { icon: <ThumbUpAlt fontSize="small" />, color: '#2e7d32', bgColor: '#e8f5e9' },
  DocumentGeneration: { icon: <Description fontSize="small" />, color: '#37474f', bgColor: '#eceff1' },
};

function StepNode({ data, selected }: NodeProps<StepNodeType>) {
  const config = stepTypeConfig[data.stepType];

  return (
    <Box
      sx={{
        minWidth: 200,
        maxWidth: 260,
        border: 2,
        borderColor: selected ? 'primary.main' : 'divider',
        borderRadius: 2,
        bgcolor: 'background.paper',
        boxShadow: selected ? 4 : 1,
        transition: 'box-shadow 0.2s, border-color 0.2s',
        overflow: 'hidden',
      }}
    >
      <Handle
        type="target"
        position={Position.Top}
        style={{ background: '#555', width: 10, height: 10 }}
      />

      {/* Header */}
      <Box
        sx={{
          display: 'flex',
          alignItems: 'center',
          gap: 1,
          px: 1.5,
          py: 1,
          bgcolor: config.bgColor,
          borderBottom: 1,
          borderColor: 'divider',
        }}
      >
        <Box sx={{ color: config.color, display: 'flex', alignItems: 'center' }}>
          {config.icon}
        </Box>
        <Typography variant="caption" fontWeight={600} sx={{ color: config.color, flex: 1 }}>
          {data.stepType}
        </Typography>
        {data.onDelete && (
          <IconButton
            size="small"
            onClick={(e) => {
              e.stopPropagation();
              data.onDelete!(data.stepId);
            }}
            sx={{ p: 0.25 }}
          >
            <Delete sx={{ fontSize: 14 }} />
          </IconButton>
        )}
      </Box>

      {/* Body */}
      <Box sx={{ px: 1.5, py: 1.5 }}>
        <Typography variant="body2" fontWeight={500} noWrap>
          {data.label}
        </Typography>
        <Box sx={{ display: 'flex', gap: 0.5, mt: 0.5 }}>
          <Chip
            label={data.isRequired ? 'Required' : 'Optional'}
            size="small"
            variant="outlined"
            sx={{ fontSize: '0.65rem', height: 20 }}
          />
          <Chip
            label={`#${data.sortOrder + 1}`}
            size="small"
            variant="outlined"
            sx={{ fontSize: '0.65rem', height: 20 }}
          />
        </Box>
      </Box>

      <Handle
        type="source"
        position={Position.Bottom}
        id="success"
        style={{ background: '#2e7d32', width: 10, height: 10, left: '35%' }}
      />
      <Handle
        type="source"
        position={Position.Bottom}
        id="failure"
        style={{ background: '#c62828', width: 10, height: 10, left: '65%' }}
      />
    </Box>
  );
}

export default memo(StepNode);

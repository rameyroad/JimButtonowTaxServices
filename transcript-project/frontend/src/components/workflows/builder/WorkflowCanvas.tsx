'use client';

import { useState, useCallback, useMemo, useEffect, useRef } from 'react';
import {
  ReactFlow,
  Background,
  Controls,
  MiniMap,
  useNodesState,
  useEdgesState,
  addEdge,
  type OnConnect,
  type Edge,
  type Connection,
  BackgroundVariant,
  Panel,
  MarkerType,
} from '@xyflow/react';
import '@xyflow/react/dist/style.css';
import { Box, Typography, Chip, Button, Alert, Snackbar } from '@mui/material';
import { Publish, Unpublished, ArrowBack } from '@mui/icons-material';
import NextLink from 'next/link';
import StepNode from './StepNode';
import StepPalette from './StepPalette';
import StepConfigPanel from './StepConfigPanel';
import type { StepConfigData } from './StepConfigPanel';
import type { StepNodeData, StepNodeType } from './StepNode';
import type { WorkflowDefinitionDetail, WorkflowStepType, WorkflowStep } from '@/lib/types/workflow';
import type { PublishStatus } from '@/lib/types/decisionTable';
import {
  useAddWorkflowStepMutation,
  useUpdateWorkflowStepMutation,
  useRemoveWorkflowStepMutation,
  usePublishWorkflowMutation,
  useUnpublishWorkflowMutation,
} from '@/lib/api/workflowsApi';

const NODE_TYPES = { step: StepNode };

const statusColors: Record<PublishStatus, 'default' | 'success' | 'warning'> = {
  Draft: 'default',
  Published: 'success',
  Archived: 'warning',
};

/**
 * Auto-layout steps in a vertical arrangement.
 * Steps are positioned by their sortOrder, centered horizontally.
 */
function layoutSteps(steps: WorkflowStep[], onDelete: (stepId: string) => void): StepNodeType[] {
  const sorted = [...steps].sort((a, b) => a.sortOrder - b.sortOrder);
  const startX = 300;
  const startY = 50;
  const yGap = 140;

  return sorted.map((step, idx) => ({
    id: step.id,
    type: 'step' as const,
    position: { x: startX, y: startY + idx * yGap },
    data: {
      stepId: step.id,
      label: step.name,
      stepType: step.stepType,
      isRequired: step.isRequired,
      configuration: step.configuration,
      sortOrder: step.sortOrder,
      onDelete,
    },
  }));
}

function buildEdges(steps: WorkflowStep[]): Edge[] {
  const edges: Edge[] = [];
  for (const step of steps) {
    if (step.nextStepOnSuccessId) {
      edges.push({
        id: `${step.id}-success-${step.nextStepOnSuccessId}`,
        source: step.id,
        target: step.nextStepOnSuccessId,
        sourceHandle: 'success',
        type: 'smoothstep',
        animated: true,
        style: { stroke: '#2e7d32', strokeWidth: 2 },
        markerEnd: { type: MarkerType.ArrowClosed, color: '#2e7d32' },
        label: 'success',
        labelStyle: { fontSize: 10, fill: '#2e7d32' },
      });
    }
    if (step.nextStepOnFailureId) {
      edges.push({
        id: `${step.id}-failure-${step.nextStepOnFailureId}`,
        source: step.id,
        target: step.nextStepOnFailureId,
        sourceHandle: 'failure',
        type: 'smoothstep',
        style: { stroke: '#c62828', strokeWidth: 2, strokeDasharray: '5,5' },
        markerEnd: { type: MarkerType.ArrowClosed, color: '#c62828' },
        label: 'failure',
        labelStyle: { fontSize: 10, fill: '#c62828' },
      });
    }
  }
  return edges;
}

interface WorkflowCanvasProps {
  workflow: WorkflowDefinitionDetail;
}

export default function WorkflowCanvas({ workflow }: WorkflowCanvasProps) {
  const [addStep] = useAddWorkflowStepMutation();
  const [updateStep] = useUpdateWorkflowStepMutation();
  const [removeStep] = useRemoveWorkflowStepMutation();
  const [publishWorkflow] = usePublishWorkflowMutation();
  const [unpublishWorkflow] = useUnpublishWorkflowMutation();

  const [selectedStepConfig, setSelectedStepConfig] = useState<StepConfigData | null>(null);
  const [snackbar, setSnackbar] = useState<{ open: boolean; message: string; severity: 'success' | 'error' }>({
    open: false, message: '', severity: 'success',
  });

  const handleDeleteStep = useCallback(
    async (stepId: string) => {
      await removeStep({ workflowId: workflow.id, stepId });
      if (selectedStepConfig?.stepId === stepId) {
        setSelectedStepConfig(null);
      }
      setSnackbar({ open: true, message: 'Step removed', severity: 'success' });
    },
    [removeStep, workflow.id, selectedStepConfig]
  );

  const initialNodes = useMemo(() => layoutSteps(workflow.steps, handleDeleteStep), [workflow.steps, handleDeleteStep]);
  const initialEdges = useMemo(() => buildEdges(workflow.steps), [workflow.steps]);

  const [nodes, setNodes, onNodesChange] = useNodesState(initialNodes);
  const [edges, setEdges, onEdgesChange] = useEdgesState(initialEdges);

  // Keep a ref of the latest workflow steps for comparison
  const prevStepsRef = useRef(workflow.steps);
  useEffect(() => {
    if (prevStepsRef.current !== workflow.steps) {
      prevStepsRef.current = workflow.steps;
      setNodes(layoutSteps(workflow.steps, handleDeleteStep));
      setEdges(buildEdges(workflow.steps));
    }
  }, [workflow.steps, handleDeleteStep, setNodes, setEdges]);

  const onConnect: OnConnect = useCallback(
    async (connection: Connection) => {
      if (!connection.source || !connection.target) return;

      const sourceHandle = connection.sourceHandle;
      const isSuccess = sourceHandle === 'success';

      // Update the step's next pointer via API
      await updateStep({
        workflowId: workflow.id,
        stepId: connection.source,
        data: isSuccess
          ? { nextStepOnSuccessId: connection.target }
          : { nextStepOnFailureId: connection.target },
      });

      // Optimistically add edge
      const newEdge: Edge = {
        id: `${connection.source}-${sourceHandle}-${connection.target}`,
        source: connection.source,
        target: connection.target,
        sourceHandle: sourceHandle ?? undefined,
        type: 'smoothstep',
        animated: isSuccess,
        style: {
          stroke: isSuccess ? '#2e7d32' : '#c62828',
          strokeWidth: 2,
          ...(isSuccess ? {} : { strokeDasharray: '5,5' }),
        },
        markerEnd: {
          type: MarkerType.ArrowClosed,
          color: isSuccess ? '#2e7d32' : '#c62828',
        },
        label: isSuccess ? 'success' : 'failure',
        labelStyle: { fontSize: 10, fill: isSuccess ? '#2e7d32' : '#c62828' },
      };
      setEdges((eds) => addEdge(newEdge, eds));
    },
    [workflow.id, updateStep, setEdges]
  );

  const onNodeClick = useCallback(
    (_event: React.MouseEvent, node: StepNodeType) => {
      const nodeData = node.data as StepNodeData;
      setSelectedStepConfig({
        stepId: nodeData.stepId,
        name: nodeData.label,
        stepType: nodeData.stepType,
        sortOrder: nodeData.sortOrder,
        isRequired: nodeData.isRequired,
        configuration: nodeData.configuration ?? '',
      });
    },
    []
  );

  const handleAddStep = useCallback(
    async (stepType: WorkflowStepType) => {
      const nextOrder = workflow.steps.length > 0
        ? Math.max(...workflow.steps.map((s) => s.sortOrder)) + 1
        : 0;

      await addStep({
        workflowId: workflow.id,
        data: {
          name: `New ${stepType}`,
          stepType,
          sortOrder: nextOrder,
          isRequired: true,
        },
      });
      setSnackbar({ open: true, message: `${stepType} step added`, severity: 'success' });
    },
    [addStep, workflow.id, workflow.steps]
  );

  const handleConfigSave = useCallback(
    async () => {
      if (!selectedStepConfig) return;
      await updateStep({
        workflowId: workflow.id,
        stepId: selectedStepConfig.stepId,
        data: {
          name: selectedStepConfig.name,
          stepType: selectedStepConfig.stepType,
          sortOrder: selectedStepConfig.sortOrder,
          configuration: selectedStepConfig.configuration || undefined,
          isRequired: selectedStepConfig.isRequired,
        },
      });
      setSnackbar({ open: true, message: 'Step updated', severity: 'success' });
    },
    [selectedStepConfig, updateStep, workflow.id]
  );

  const handlePublish = async () => {
    await publishWorkflow(workflow.id);
    setSnackbar({ open: true, message: 'Workflow published', severity: 'success' });
  };

  const handleUnpublish = async () => {
    await unpublishWorkflow(workflow.id);
    setSnackbar({ open: true, message: 'Workflow unpublished', severity: 'success' });
  };

  return (
    <Box sx={{ display: 'flex', height: 'calc(100vh - 64px)', overflow: 'hidden' }}>
      {/* Step palette */}
      <StepPalette onAddStep={handleAddStep} />

      {/* Canvas area */}
      <Box sx={{ flex: 1, position: 'relative' }}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          onConnect={onConnect}
          onNodeClick={onNodeClick}
          nodeTypes={NODE_TYPES}
          fitView
          fitViewOptions={{ padding: 0.3 }}
          deleteKeyCode="Delete"
          proOptions={{ hideAttribution: true }}
        >
          <Background variant={BackgroundVariant.Dots} gap={20} size={1} />
          <Controls />
          <MiniMap
            nodeStrokeWidth={3}
            pannable
            zoomable
            style={{ height: 100, width: 150 }}
          />

          {/* Top toolbar */}
          <Panel position="top-left">
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, bgcolor: 'background.paper', px: 2, py: 1, borderRadius: 1, boxShadow: 1 }}>
              <Button
                component={NextLink}
                href={`/platform/workflows/${workflow.id}`}
                size="small"
                startIcon={<ArrowBack />}
              >
                Back
              </Button>
              <Typography variant="subtitle1" fontWeight={600} noWrap sx={{ maxWidth: 300 }}>
                {workflow.name}
              </Typography>
              <Chip
                label={workflow.status}
                size="small"
                color={statusColors[workflow.status]}
              />
              <Typography variant="caption" color="text.secondary">
                v{workflow.currentVersion}
              </Typography>
            </Box>
          </Panel>

          <Panel position="top-right">
            <Box sx={{ display: 'flex', gap: 1 }}>
              {workflow.status === 'Draft' ? (
                <Button
                  variant="contained"
                  size="small"
                  startIcon={<Publish />}
                  onClick={handlePublish}
                  disabled={workflow.steps.length === 0}
                  sx={{ boxShadow: 2 }}
                >
                  Publish
                </Button>
              ) : workflow.status === 'Published' ? (
                <Button
                  variant="outlined"
                  size="small"
                  startIcon={<Unpublished />}
                  onClick={handleUnpublish}
                  sx={{ bgcolor: 'background.paper', boxShadow: 2 }}
                >
                  Unpublish
                </Button>
              ) : null}
            </Box>
          </Panel>

          {/* Handles legend */}
          <Panel position="bottom-left">
            <Box sx={{ bgcolor: 'background.paper', px: 2, py: 1, borderRadius: 1, boxShadow: 1, display: 'flex', gap: 2 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                <Box sx={{ width: 10, height: 10, borderRadius: '50%', bgcolor: '#2e7d32' }} />
                <Typography variant="caption">Success path</Typography>
              </Box>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                <Box sx={{ width: 10, height: 10, borderRadius: '50%', bgcolor: '#c62828' }} />
                <Typography variant="caption">Failure path</Typography>
              </Box>
            </Box>
          </Panel>
        </ReactFlow>
      </Box>

      {/* Config panel */}
      {selectedStepConfig && (
        <StepConfigPanel
          step={selectedStepConfig}
          onChange={setSelectedStepConfig}
          onSave={handleConfigSave}
          onClose={() => setSelectedStepConfig(null)}
        />
      )}

      <Snackbar
        open={snackbar.open}
        autoHideDuration={3000}
        onClose={() => setSnackbar((s) => ({ ...s, open: false }))}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert
          severity={snackbar.severity}
          variant="filled"
          onClose={() => setSnackbar((s) => ({ ...s, open: false }))}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}

'use client';

import {
  Box,
  Typography,
  Paper,
  Chip,
  CircularProgress,
  Alert,
  Button,
  Stepper,
  Step,
  StepLabel,
  StepContent,
  Accordion,
  AccordionSummary,
  AccordionDetails,
} from '@mui/material';
import { Cancel, ExpandMore } from '@mui/icons-material';
import { useGetCaseWorkflowQuery, useCancelWorkflowMutation } from '@/lib/api/caseWorkflowsApi';
import type { WorkflowExecutionStatus, StepExecutionStatus } from '@/lib/types/workflow';

const executionStatusColors: Record<WorkflowExecutionStatus, 'default' | 'info' | 'warning' | 'success' | 'error'> = {
  NotStarted: 'default',
  Running: 'info',
  Paused: 'warning',
  Completed: 'success',
  Failed: 'error',
  Cancelled: 'default',
};

const stepStatusColors: Record<StepExecutionStatus, 'default' | 'info' | 'warning' | 'success' | 'error'> = {
  Pending: 'default',
  Running: 'info',
  Completed: 'success',
  Failed: 'error',
  Skipped: 'warning',
};

interface CaseWorkflowDetailProps {
  clientId: string;
  caseWorkflowId: string;
}

export default function CaseWorkflowDetailView({ clientId, caseWorkflowId }: CaseWorkflowDetailProps) {
  const { data: caseWorkflow, isLoading, error } = useGetCaseWorkflowQuery({
    clientId,
    caseWorkflowId,
  });
  const [cancelWorkflow, { isLoading: isCancelling }] = useCancelWorkflowMutation();

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !caseWorkflow) {
    return <Alert severity="error">Failed to load case workflow</Alert>;
  }

  const handleCancel = async () => {
    await cancelWorkflow({
      clientId,
      caseWorkflowId,
      data: { reason: 'Cancelled by user' },
    });
  };

  const canCancel = caseWorkflow.status === 'Running' ||
    caseWorkflow.status === 'Paused' ||
    caseWorkflow.status === 'NotStarted';

  const sortedSteps = [...caseWorkflow.stepExecutions].sort(
    (a, b) => new Date(a.startedAt || a.completedAt || '').getTime() -
              new Date(b.startedAt || b.completedAt || '').getTime()
  );

  return (
    <Box>
      {/* Header */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <Box>
            <Typography variant="h5" fontWeight={600} sx={{ mb: 1 }}>
              {caseWorkflow.workflowName}
            </Typography>
            <Box sx={{ display: 'flex', gap: 1, alignItems: 'center', mb: 1 }}>
              <Chip
                label={caseWorkflow.status}
                size="small"
                color={executionStatusColors[caseWorkflow.status]}
              />
              <Typography variant="caption" color="text.secondary">
                v{caseWorkflow.workflowVersion}
              </Typography>
            </Box>
            <Box sx={{ display: 'flex', gap: 3, mt: 1 }}>
              {caseWorkflow.startedAt && (
                <Typography variant="body2" color="text.secondary">
                  Started: {new Date(caseWorkflow.startedAt).toLocaleString()}
                </Typography>
              )}
              {caseWorkflow.completedAt && (
                <Typography variant="body2" color="text.secondary">
                  Completed: {new Date(caseWorkflow.completedAt).toLocaleString()}
                </Typography>
              )}
            </Box>
          </Box>
          {canCancel && (
            <Button
              variant="outlined"
              color="error"
              startIcon={<Cancel />}
              onClick={handleCancel}
              disabled={isCancelling}
            >
              {isCancelling ? 'Cancelling...' : 'Cancel Workflow'}
            </Button>
          )}
        </Box>
        {caseWorkflow.errorMessage && (
          <Alert severity="error" sx={{ mt: 2 }}>
            {caseWorkflow.errorMessage}
          </Alert>
        )}
      </Paper>

      {/* Step Executions */}
      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" sx={{ mb: 2 }}>
          Execution Steps ({sortedSteps.length})
        </Typography>

        {sortedSteps.length === 0 ? (
          <Typography color="text.secondary" sx={{ textAlign: 'center', py: 2 }}>
            No steps have been executed yet.
          </Typography>
        ) : (
          <Stepper orientation="vertical" activeStep={-1}>
            {sortedSteps.map((step) => (
              <Step key={step.id} completed={step.status === 'Completed'} active={step.status === 'Running'}>
                <StepLabel
                  error={step.status === 'Failed'}
                  optional={
                    <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
                      <Chip
                        label={step.status}
                        size="small"
                        color={stepStatusColors[step.status]}
                        variant="outlined"
                      />
                      <Chip label={step.stepType} size="small" variant="outlined" />
                    </Box>
                  }
                >
                  {step.stepName}
                </StepLabel>
                <StepContent>
                  <Box sx={{ ml: 1 }}>
                    {step.startedAt && (
                      <Typography variant="caption" color="text.secondary" display="block">
                        Started: {new Date(step.startedAt).toLocaleString()}
                      </Typography>
                    )}
                    {step.completedAt && (
                      <Typography variant="caption" color="text.secondary" display="block">
                        Completed: {new Date(step.completedAt).toLocaleString()}
                      </Typography>
                    )}
                    {step.errorMessage && (
                      <Alert severity="error" sx={{ mt: 1 }}>
                        {step.errorMessage}
                      </Alert>
                    )}
                    {(step.inputData || step.outputData) && (
                      <Accordion sx={{ mt: 1 }} disableGutters>
                        <AccordionSummary expandIcon={<ExpandMore />}>
                          <Typography variant="body2">Data</Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                          {step.inputData && (
                            <Box sx={{ mb: 1 }}>
                              <Typography variant="caption" fontWeight={600}>Input:</Typography>
                              <Typography
                                variant="body2"
                                component="pre"
                                sx={{ fontSize: '0.75rem', overflow: 'auto', maxHeight: 200 }}
                              >
                                {JSON.stringify(JSON.parse(step.inputData), null, 2)}
                              </Typography>
                            </Box>
                          )}
                          {step.outputData && (
                            <Box>
                              <Typography variant="caption" fontWeight={600}>Output:</Typography>
                              <Typography
                                variant="body2"
                                component="pre"
                                sx={{ fontSize: '0.75rem', overflow: 'auto', maxHeight: 200 }}
                              >
                                {JSON.stringify(JSON.parse(step.outputData), null, 2)}
                              </Typography>
                            </Box>
                          )}
                        </AccordionDetails>
                      </Accordion>
                    )}
                  </Box>
                </StepContent>
              </Step>
            ))}
          </Stepper>
        )}
      </Paper>
    </Box>
  );
}

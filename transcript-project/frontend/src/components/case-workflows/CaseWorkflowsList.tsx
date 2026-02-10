'use client';

import { useState } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  TablePagination,
  Box,
  Chip,
  Typography,
  CircularProgress,
  Button,
  LinearProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from '@mui/material';
import { Add, PlayArrow } from '@mui/icons-material';
import { useRouter } from 'next/navigation';
import { useListCaseWorkflowsQuery, useStartWorkflowMutation } from '@/lib/api/caseWorkflowsApi';
import { useListWorkflowDefinitionsQuery } from '@/lib/api/workflowsApi';
import type { WorkflowExecutionStatus } from '@/lib/types/workflow';

const statusColors: Record<WorkflowExecutionStatus, 'default' | 'info' | 'warning' | 'success' | 'error'> = {
  NotStarted: 'default',
  Running: 'info',
  Paused: 'warning',
  Completed: 'success',
  Failed: 'error',
  Cancelled: 'default',
};

interface CaseWorkflowsListProps {
  clientId: string;
}

export default function CaseWorkflowsList({ clientId }: CaseWorkflowsListProps) {
  const router = useRouter();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [startDialogOpen, setStartDialogOpen] = useState(false);
  const [selectedWorkflowId, setSelectedWorkflowId] = useState('');

  const { data, isLoading } = useListCaseWorkflowsQuery({
    clientId,
    page: page + 1,
    pageSize,
  });

  const { data: workflowDefs } = useListWorkflowDefinitionsQuery({
    status: 'Published',
    pageSize: 100,
  });

  const [startWorkflow, { isLoading: isStarting }] = useStartWorkflowMutation();

  const handleStartWorkflow = async () => {
    if (!selectedWorkflowId) return;
    try {
      const result = await startWorkflow({
        clientId,
        data: { workflowDefinitionId: selectedWorkflowId },
      }).unwrap();
      setStartDialogOpen(false);
      setSelectedWorkflowId('');
      router.push(`/clients/${clientId}/workflows/${result.id}`);
    } catch {
      // Error handled by RTK Query
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h6">Workflows</Typography>
        <Button
          variant="outlined"
          startIcon={<Add />}
          onClick={() => setStartDialogOpen(true)}
          size="small"
        >
          Start Workflow
        </Button>
      </Box>

      <Paper>
        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Workflow</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Progress</TableCell>
                <TableCell>Started</TableCell>
                <TableCell>Completed</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {isLoading ? (
                <TableRow>
                  <TableCell colSpan={5} align="center" sx={{ py: 4 }}>
                    <CircularProgress size={32} />
                  </TableCell>
                </TableRow>
              ) : data?.items.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={5} align="center" sx={{ py: 4 }}>
                    <Typography color="text.secondary">
                      No workflows have been run for this client
                    </Typography>
                  </TableCell>
                </TableRow>
              ) : (
                data?.items.map((cw) => {
                  const progress = cw.totalSteps > 0
                    ? (cw.completedSteps / cw.totalSteps) * 100
                    : 0;

                  return (
                    <TableRow
                      key={cw.id}
                      hover
                      sx={{ cursor: 'pointer' }}
                      onClick={() => router.push(`/clients/${clientId}/workflows/${cw.id}`)}
                    >
                      <TableCell>
                        <Typography variant="body2" fontWeight={500}>
                          {cw.workflowName}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          v{cw.workflowVersion}
                        </Typography>
                      </TableCell>
                      <TableCell>
                        <Chip
                          label={cw.status}
                          size="small"
                          color={statusColors[cw.status]}
                        />
                      </TableCell>
                      <TableCell sx={{ minWidth: 150 }}>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <LinearProgress
                            variant="determinate"
                            value={progress}
                            sx={{ flex: 1, height: 6, borderRadius: 3 }}
                          />
                          <Typography variant="caption" color="text.secondary">
                            {cw.completedSteps}/{cw.totalSteps}
                          </Typography>
                        </Box>
                      </TableCell>
                      <TableCell>
                        <Typography variant="body2" color="text.secondary">
                          {cw.startedAt
                            ? new Date(cw.startedAt).toLocaleDateString()
                            : '-'}
                        </Typography>
                      </TableCell>
                      <TableCell>
                        <Typography variant="body2" color="text.secondary">
                          {cw.completedAt
                            ? new Date(cw.completedAt).toLocaleDateString()
                            : '-'}
                        </Typography>
                      </TableCell>
                    </TableRow>
                  );
                })
              )}
            </TableBody>
          </Table>
        </TableContainer>

        {data && data.totalCount > pageSize && (
          <TablePagination
            component="div"
            count={data.totalCount}
            page={page}
            onPageChange={(_, newPage) => setPage(newPage)}
            rowsPerPage={pageSize}
            onRowsPerPageChange={(e) => {
              setPageSize(parseInt(e.target.value, 10));
              setPage(0);
            }}
            rowsPerPageOptions={[10, 20, 50]}
          />
        )}
      </Paper>

      {/* Start Workflow Dialog */}
      <Dialog
        open={startDialogOpen}
        onClose={() => setStartDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Start Workflow</DialogTitle>
        <DialogContent>
          <FormControl fullWidth sx={{ mt: 1 }}>
            <InputLabel>Select Workflow</InputLabel>
            <Select
              value={selectedWorkflowId}
              label="Select Workflow"
              onChange={(e) => setSelectedWorkflowId(e.target.value)}
            >
              {workflowDefs?.items.map((wf) => (
                <MenuItem key={wf.id} value={wf.id}>
                  <Box>
                    <Typography variant="body2">{wf.name}</Typography>
                    {wf.category && (
                      <Typography variant="caption" color="text.secondary">
                        {wf.category}
                      </Typography>
                    )}
                  </Box>
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setStartDialogOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            startIcon={<PlayArrow />}
            onClick={handleStartWorkflow}
            disabled={!selectedWorkflowId || isStarting}
          >
            {isStarting ? 'Starting...' : 'Start'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

'use client';

import { useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  TableSortLabel,
  Chip,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  CircularProgress,
  Alert,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
} from '@mui/material';
import { CheckCircle, SwapHoriz } from '@mui/icons-material';
import {
  useListHumanTasksQuery,
  useCompleteHumanTaskMutation,
  useReassignHumanTaskMutation,
} from '@/lib/api/humanTasksApi';
import type { HumanTaskStatus } from '@/lib/types/humanTask';

const statusColors: Record<HumanTaskStatus, 'default' | 'info' | 'success' | 'warning' | 'error'> = {
  Pending: 'warning',
  InProgress: 'info',
  Completed: 'success',
  Reassigned: 'default',
  Escalated: 'error',
};

type SortField = 'createdAt' | 'status' | 'dueDate' | 'title';

export default function TasksTable() {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [statusFilter, setStatusFilter] = useState<string>('');
  const [sortBy, setSortBy] = useState<SortField>('createdAt');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');
  const [completeDialogOpen, setCompleteDialogOpen] = useState(false);
  const [selectedTaskId, setSelectedTaskId] = useState<string | null>(null);
  const [decision, setDecision] = useState('');
  const [notes, setNotes] = useState('');

  const { data, isLoading, error } = useListHumanTasksQuery({
    page: page + 1,
    pageSize,
    status: statusFilter as HumanTaskStatus || undefined,
    sortBy,
    sortOrder,
  });

  const [completeTask] = useCompleteHumanTaskMutation();
  const [reassignTask] = useReassignHumanTaskMutation();

  const handleSort = (field: SortField) => {
    if (sortBy === field) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(field);
      setSortOrder('asc');
    }
  };

  const openCompleteDialog = (taskId: string) => {
    setSelectedTaskId(taskId);
    setDecision('');
    setNotes('');
    setCompleteDialogOpen(true);
  };

  const handleComplete = async () => {
    if (!selectedTaskId) return;
    await completeTask({
      id: selectedTaskId,
      data: {
        decision: decision || undefined,
        notes: notes || undefined,
      },
    });
    setCompleteDialogOpen(false);
  };

  const handleReassign = async (taskId: string) => {
    // For now, reassign to a placeholder user ID
    // In a real implementation, this would open a user picker dialog
    await reassignTask({
      id: taskId,
      data: { assignedToUserId: '00000000-0000-0000-0000-000000000002' },
    });
  };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return <Alert severity="error">Failed to load tasks</Alert>;
  }

  return (
    <Box>
      {/* Filters */}
      <Paper sx={{ p: 2, mb: 2 }}>
        <Box sx={{ display: 'flex', gap: 2 }}>
          <FormControl size="small" sx={{ minWidth: 150 }}>
            <InputLabel>Status</InputLabel>
            <Select
              value={statusFilter}
              label="Status"
              onChange={(e) => { setStatusFilter(e.target.value); setPage(0); }}
            >
              <MenuItem value="">All</MenuItem>
              <MenuItem value="Pending">Pending</MenuItem>
              <MenuItem value="InProgress">In Progress</MenuItem>
              <MenuItem value="Completed">Completed</MenuItem>
              <MenuItem value="Reassigned">Reassigned</MenuItem>
              <MenuItem value="Escalated">Escalated</MenuItem>
            </Select>
          </FormControl>
        </Box>
      </Paper>

      {/* Table */}
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'title'}
                  direction={sortBy === 'title' ? sortOrder : 'asc'}
                  onClick={() => handleSort('title')}
                >
                  Title
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'status'}
                  direction={sortBy === 'status' ? sortOrder : 'asc'}
                  onClick={() => handleSort('status')}
                >
                  Status
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'dueDate'}
                  direction={sortBy === 'dueDate' ? sortOrder : 'asc'}
                  onClick={() => handleSort('dueDate')}
                >
                  Due Date
                </TableSortLabel>
              </TableCell>
              <TableCell>Decision</TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'createdAt'}
                  direction={sortBy === 'createdAt' ? sortOrder : 'asc'}
                  onClick={() => handleSort('createdAt')}
                >
                  Created
                </TableSortLabel>
              </TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data?.items.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} align="center">
                  <Typography color="text.secondary" sx={{ py: 2 }}>
                    No tasks found
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              data?.items.map((task) => (
                <TableRow key={task.id}>
                  <TableCell>
                    <Typography variant="body2" fontWeight={500}>
                      {task.title}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={task.status}
                      size="small"
                      color={statusColors[task.status]}
                    />
                  </TableCell>
                  <TableCell>
                    {task.dueDate
                      ? new Date(task.dueDate).toLocaleDateString()
                      : '-'}
                  </TableCell>
                  <TableCell>
                    {task.decision || '-'}
                  </TableCell>
                  <TableCell>
                    {new Date(task.createdAt).toLocaleDateString()}
                  </TableCell>
                  <TableCell align="right">
                    {task.status !== 'Completed' && (
                      <>
                        <IconButton
                          size="small"
                          color="success"
                          title="Complete"
                          onClick={() => openCompleteDialog(task.id)}
                        >
                          <CheckCircle fontSize="small" />
                        </IconButton>
                        <IconButton
                          size="small"
                          title="Reassign"
                          onClick={() => handleReassign(task.id)}
                        >
                          <SwapHoriz fontSize="small" />
                        </IconButton>
                      </>
                    )}
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
        {data && (
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
      </TableContainer>

      {/* Complete Task Dialog */}
      <Dialog
        open={completeDialogOpen}
        onClose={() => setCompleteDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Complete Task</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <FormControl size="small">
              <InputLabel>Decision</InputLabel>
              <Select
                value={decision}
                label="Decision"
                onChange={(e) => setDecision(e.target.value)}
              >
                <MenuItem value="">None</MenuItem>
                <MenuItem value="Approved">Approved</MenuItem>
                <MenuItem value="Rejected">Rejected</MenuItem>
                <MenuItem value="Modified">Modified</MenuItem>
              </Select>
            </FormControl>
            <TextField
              label="Notes"
              multiline
              rows={3}
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCompleteDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" color="success" onClick={handleComplete}>
            Complete
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

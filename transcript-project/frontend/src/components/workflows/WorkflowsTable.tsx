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
  TextField,
  InputAdornment,
  Box,
  Chip,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Typography,
  CircularProgress,
  TableSortLabel,
} from '@mui/material';
import { Search } from '@mui/icons-material';
import { useRouter } from 'next/navigation';
import { useListWorkflowDefinitionsQuery } from '@/lib/api/workflowsApi';
import type { PublishStatus } from '@/lib/types/decisionTable';

const statusColors: Record<PublishStatus, 'default' | 'success' | 'warning'> = {
  Draft: 'default',
  Published: 'success',
  Archived: 'warning',
};

type SortField = 'name' | 'status' | 'category' | 'createdAt' | 'updatedAt' | 'publishedAt';

export default function WorkflowsTable() {
  const router = useRouter();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<PublishStatus | ''>('');
  const [sortBy, setSortBy] = useState<SortField>('name');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('asc');

  const { data, isLoading } = useListWorkflowDefinitionsQuery({
    page: page + 1,
    pageSize,
    search: search || undefined,
    status: statusFilter || undefined,
    sortBy,
    sortOrder,
  });

  const handleSort = (field: SortField) => {
    if (sortBy === field) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(field);
      setSortOrder('asc');
    }
  };

  return (
    <Paper sx={{ width: '100%' }}>
      {/* Filters */}
      <Box sx={{ p: 2, display: 'flex', gap: 2, alignItems: 'center' }}>
        <TextField
          size="small"
          placeholder="Search workflows..."
          value={search}
          onChange={(e) => { setSearch(e.target.value); setPage(0); }}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <Search />
                </InputAdornment>
              ),
            },
          }}
          sx={{ minWidth: 300 }}
        />
        <FormControl size="small" sx={{ minWidth: 150 }}>
          <InputLabel>Status</InputLabel>
          <Select
            value={statusFilter}
            label="Status"
            onChange={(e) => { setStatusFilter(e.target.value as PublishStatus | ''); setPage(0); }}
          >
            <MenuItem value="">All</MenuItem>
            <MenuItem value="Draft">Draft</MenuItem>
            <MenuItem value="Published">Published</MenuItem>
            <MenuItem value="Archived">Archived</MenuItem>
          </Select>
        </FormControl>
      </Box>

      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'name'}
                  direction={sortBy === 'name' ? sortOrder : 'asc'}
                  onClick={() => handleSort('name')}
                >
                  Name
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'category'}
                  direction={sortBy === 'category' ? sortOrder : 'asc'}
                  onClick={() => handleSort('category')}
                >
                  Category
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
              <TableCell align="right">Steps</TableCell>
              <TableCell align="right">Version</TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'updatedAt'}
                  direction={sortBy === 'updatedAt' ? sortOrder : 'asc'}
                  onClick={() => handleSort('updatedAt')}
                >
                  Updated
                </TableSortLabel>
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {isLoading ? (
              <TableRow>
                <TableCell colSpan={6} align="center" sx={{ py: 4 }}>
                  <CircularProgress size={32} />
                </TableCell>
              </TableRow>
            ) : data?.items.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} align="center" sx={{ py: 4 }}>
                  <Typography color="text.secondary">
                    No workflows found
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              data?.items.map((workflow) => (
                <TableRow
                  key={workflow.id}
                  hover
                  sx={{ cursor: 'pointer' }}
                  onClick={() => router.push(`/platform/workflows/${workflow.id}`)}
                >
                  <TableCell>
                    <Typography variant="body2" fontWeight={500}>
                      {workflow.name}
                    </Typography>
                    {workflow.description && (
                      <Typography variant="caption" color="text.secondary">
                        {workflow.description.length > 80
                          ? `${workflow.description.substring(0, 80)}...`
                          : workflow.description}
                      </Typography>
                    )}
                  </TableCell>
                  <TableCell>
                    {workflow.category && (
                      <Chip label={workflow.category} size="small" variant="outlined" />
                    )}
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={workflow.status}
                      size="small"
                      color={statusColors[workflow.status]}
                    />
                  </TableCell>
                  <TableCell align="right">{workflow.stepCount}</TableCell>
                  <TableCell align="right">v{workflow.currentVersion}</TableCell>
                  <TableCell>
                    <Typography variant="body2" color="text.secondary">
                      {new Date(workflow.updatedAt).toLocaleDateString()}
                    </Typography>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

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
    </Paper>
  );
}

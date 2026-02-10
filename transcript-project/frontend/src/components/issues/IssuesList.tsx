'use client';

import {
  Box,
  Chip,
  FormControl,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TableSortLabel,
  Typography,
  IconButton,
  Tooltip,
  CircularProgress,
  Alert,
} from '@mui/material';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import UndoIcon from '@mui/icons-material/Undo';
import { useState } from 'react';
import { useListClientIssuesQuery, useUpdateIssueMutation } from '@/lib/api/issuesApi';
import type { IssueType, IssueSeverity } from '@/lib/types/issue';

const severityColors: Record<IssueSeverity, 'default' | 'info' | 'warning' | 'error'> = {
  Low: 'default',
  Medium: 'info',
  High: 'warning',
  Critical: 'error',
};

const issueTypeLabels: Record<IssueType, string> = {
  BalanceDue: 'Balance Due',
  Penalty: 'Penalty',
  UnfiledReturn: 'Unfiled Return',
  StatuteExpiration: 'Statute Expiration',
  PaymentPlan: 'Payment Plan',
  Lien: 'Lien',
  Levy: 'Levy',
};

type SortField = 'severity' | 'taxYear' | 'amount' | 'issueType' | 'detectedAt';

interface IssuesListProps {
  clientId: string;
}

export function IssuesList({ clientId }: IssuesListProps) {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [sortBy, setSortBy] = useState<SortField>('detectedAt');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');
  const [issueTypeFilter, setIssueTypeFilter] = useState<IssueType | ''>('');
  const [severityFilter, setSeverityFilter] = useState<IssueSeverity | ''>('');
  const [resolvedFilter, setResolvedFilter] = useState<'all' | 'resolved' | 'unresolved'>('unresolved');

  const { data, isLoading, error } = useListClientIssuesQuery({
    clientId,
    page: page + 1,
    pageSize,
    sortBy,
    sortOrder,
    issueType: issueTypeFilter || undefined,
    severity: severityFilter || undefined,
    resolved: resolvedFilter === 'all' ? undefined : resolvedFilter === 'resolved',
  });

  const [updateIssue] = useUpdateIssueMutation();

  const handleSort = (field: SortField) => {
    if (sortBy === field) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(field);
      setSortOrder('desc');
    }
    setPage(0);
  };

  const handleResolve = async (issueId: string) => {
    await updateIssue({ clientId, issueId, data: { resolve: true } });
  };

  const handleUnresolve = async (issueId: string) => {
    await updateIssue({ clientId, issueId, data: { resolve: false } });
  };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return <Alert severity="error">Failed to load issues.</Alert>;
  }

  return (
    <Box>
      {/* Filters */}
      <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
        <FormControl size="small" sx={{ minWidth: 150 }}>
          <InputLabel>Issue Type</InputLabel>
          <Select
            value={issueTypeFilter}
            label="Issue Type"
            onChange={(e) => {
              setIssueTypeFilter(e.target.value as IssueType | '');
              setPage(0);
            }}
          >
            <MenuItem value="">All Types</MenuItem>
            {Object.entries(issueTypeLabels).map(([value, label]) => (
              <MenuItem key={value} value={value}>{label}</MenuItem>
            ))}
          </Select>
        </FormControl>

        <FormControl size="small" sx={{ minWidth: 130 }}>
          <InputLabel>Severity</InputLabel>
          <Select
            value={severityFilter}
            label="Severity"
            onChange={(e) => {
              setSeverityFilter(e.target.value as IssueSeverity | '');
              setPage(0);
            }}
          >
            <MenuItem value="">All</MenuItem>
            <MenuItem value="Critical">Critical</MenuItem>
            <MenuItem value="High">High</MenuItem>
            <MenuItem value="Medium">Medium</MenuItem>
            <MenuItem value="Low">Low</MenuItem>
          </Select>
        </FormControl>

        <FormControl size="small" sx={{ minWidth: 130 }}>
          <InputLabel>Status</InputLabel>
          <Select
            value={resolvedFilter}
            label="Status"
            onChange={(e) => {
              setResolvedFilter(e.target.value as 'all' | 'resolved' | 'unresolved');
              setPage(0);
            }}
          >
            <MenuItem value="unresolved">Unresolved</MenuItem>
            <MenuItem value="resolved">Resolved</MenuItem>
            <MenuItem value="all">All</MenuItem>
          </Select>
        </FormControl>
      </Box>

      {/* Table */}
      <TableContainer component={Paper}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'issueType'}
                  direction={sortBy === 'issueType' ? sortOrder : 'asc'}
                  onClick={() => handleSort('issueType')}
                >
                  Type
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'severity'}
                  direction={sortBy === 'severity' ? sortOrder : 'asc'}
                  onClick={() => handleSort('severity')}
                >
                  Severity
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'taxYear'}
                  direction={sortBy === 'taxYear' ? sortOrder : 'asc'}
                  onClick={() => handleSort('taxYear')}
                >
                  Tax Year
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'amount'}
                  direction={sortBy === 'amount' ? sortOrder : 'asc'}
                  onClick={() => handleSort('amount')}
                >
                  Amount
                </TableSortLabel>
              </TableCell>
              <TableCell>Description</TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'detectedAt'}
                  direction={sortBy === 'detectedAt' ? sortOrder : 'asc'}
                  onClick={() => handleSort('detectedAt')}
                >
                  Detected
                </TableSortLabel>
              </TableCell>
              <TableCell>Status</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data?.items.length === 0 && (
              <TableRow>
                <TableCell colSpan={8} align="center">
                  <Typography color="text.secondary" sx={{ py: 2 }}>
                    No issues found.
                  </Typography>
                </TableCell>
              </TableRow>
            )}
            {data?.items.map((issue) => (
              <TableRow
                key={issue.id}
                sx={{ opacity: issue.resolvedAt ? 0.6 : 1 }}
              >
                <TableCell>
                  <Chip
                    label={issueTypeLabels[issue.issueType]}
                    size="small"
                    variant="outlined"
                  />
                </TableCell>
                <TableCell>
                  <Chip
                    label={issue.severity}
                    size="small"
                    color={severityColors[issue.severity]}
                  />
                </TableCell>
                <TableCell>{issue.taxYear}</TableCell>
                <TableCell>
                  {issue.amount != null
                    ? `$${issue.amount.toLocaleString('en-US', { minimumFractionDigits: 2 })}`
                    : '—'}
                </TableCell>
                <TableCell sx={{ maxWidth: 300 }}>
                  <Typography variant="body2" noWrap>
                    {issue.description}
                  </Typography>
                  {issue.transactionCode && (
                    <Typography variant="caption" color="text.secondary">
                      TC {issue.transactionCode}
                    </Typography>
                  )}
                </TableCell>
                <TableCell>
                  {new Date(issue.detectedAt).toLocaleDateString()}
                </TableCell>
                <TableCell>
                  {issue.resolvedAt ? (
                    <Chip label="Resolved" size="small" color="success" variant="outlined" />
                  ) : (
                    <Chip label="Open" size="small" color="warning" variant="outlined" />
                  )}
                </TableCell>
                <TableCell align="right">
                  {issue.resolvedAt ? (
                    <Tooltip title="Unresolve">
                      <IconButton size="small" onClick={() => handleUnresolve(issue.id)}>
                        <UndoIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  ) : (
                    <Tooltip title="Mark Resolved">
                      <IconButton size="small" color="success" onClick={() => handleResolve(issue.id)}>
                        <CheckCircleIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  )}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
        {data && data.totalCount > 0 && (
          <TablePagination
            component="div"
            count={data.totalCount}
            page={page}
            onPageChange={(_e, newPage) => setPage(newPage)}
            rowsPerPage={pageSize}
            onRowsPerPageChange={(e) => {
              setPageSize(parseInt(e.target.value, 10));
              setPage(0);
            }}
            rowsPerPageOptions={[10, 20, 50]}
          />
        )}
      </TableContainer>
    </Box>
  );
}

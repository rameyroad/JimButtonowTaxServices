'use client';

import { useState, useCallback } from 'react';
import { useRouter } from 'next/navigation';
import {
  Box,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TableSortLabel,
  TablePagination,
  Paper,
  TextField,
  InputAdornment,
  IconButton,
  Chip,
  Typography,
  Skeleton,
  Alert,
  FormControlLabel,
  Switch,
} from '@mui/material';
import {
  Search,
  Clear,
  Email,
  Phone,
  Archive,
} from '@mui/icons-material';
import { useListClientsQuery } from '@/lib/api/clientsApi';
import { ClientTypeBadge } from './ClientTypeBadge';
import { TaxIdentifierDisplay } from './TaxIdentifierDisplay';
import type { ClientListItem, ListClientsParams } from '@/lib/types/client';

type SortField = 'name' | 'email' | 'createdAt' | 'updatedAt';
type SortOrder = 'asc' | 'desc';

interface HeadCell {
  id: SortField | 'type' | 'taxId' | 'contact';
  label: string;
  sortable: boolean;
  width?: string | number;
}

const headCells: HeadCell[] = [
  { id: 'type', label: 'Type', sortable: false, width: 120 },
  { id: 'name', label: 'Name', sortable: true },
  { id: 'taxId', label: 'Tax ID', sortable: false, width: 150 },
  { id: 'contact', label: 'Contact', sortable: false },
  { id: 'createdAt', label: 'Created', sortable: true, width: 120 },
];

export function ClientsTable() {
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(20);
  const [searchQuery, setSearchQuery] = useState('');
  const [debouncedSearch, setDebouncedSearch] = useState('');
  const [sortBy, setSortBy] = useState<SortField>('name');
  const [sortOrder, setSortOrder] = useState<SortOrder>('asc');
  const [includeArchived, setIncludeArchived] = useState(false);

  // Debounce search input
  const handleSearchChange = useCallback((value: string) => {
    setSearchQuery(value);
    const timeoutId = setTimeout(() => {
      setDebouncedSearch(value);
      setPage(0); // Reset to first page on search
    }, 300);
    return () => clearTimeout(timeoutId);
  }, []);

  const queryParams: ListClientsParams = {
    page: page + 1, // API uses 1-based pagination
    pageSize: rowsPerPage,
    search: debouncedSearch || undefined,
    sortBy,
    sortOrder,
    includeArchived,
  };

  const { data, isLoading, error, isFetching } = useListClientsQuery(queryParams);

  const handleSort = (field: SortField) => {
    if (sortBy === field) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(field);
      setSortOrder('asc');
    }
  };

  const handleChangePage = (_: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const clearSearch = () => {
    setSearchQuery('');
    setDebouncedSearch('');
    setPage(0);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });
  };

  if (error) {
    return (
      <Alert severity="error" sx={{ mb: 2 }}>
        Failed to load clients. Please try again later.
      </Alert>
    );
  }

  return (
    <Box>
      {/* Search and Filters */}
      <Box
        sx={{
          display: 'flex',
          gap: 2,
          mb: 3,
          alignItems: 'center',
          flexWrap: 'wrap',
        }}
      >
        <TextField
          placeholder="Search by name, email, or last 4 of SSN/EIN..."
          value={searchQuery}
          onChange={(e) => handleSearchChange(e.target.value)}
          size="small"
          sx={{ minWidth: 350, flexGrow: 1, maxWidth: 500 }}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <Search color="action" />
              </InputAdornment>
            ),
            endAdornment: searchQuery && (
              <InputAdornment position="end">
                <IconButton size="small" onClick={clearSearch}>
                  <Clear fontSize="small" />
                </IconButton>
              </InputAdornment>
            ),
          }}
        />
        <FormControlLabel
          control={
            <Switch
              checked={includeArchived}
              onChange={(e) => {
                setIncludeArchived(e.target.checked);
                setPage(0);
              }}
              size="small"
            />
          }
          label="Show archived"
          sx={{ ml: 'auto' }}
        />
      </Box>

      {/* Table */}
      <TableContainer component={Paper} variant="outlined">
        <Table size="medium">
          <TableHead>
            <TableRow>
              {headCells.map((headCell) => (
                <TableCell
                  key={headCell.id}
                  sx={{
                    fontWeight: 600,
                    backgroundColor: 'grey.50',
                    width: headCell.width,
                  }}
                >
                  {headCell.sortable ? (
                    <TableSortLabel
                      active={sortBy === headCell.id}
                      direction={sortBy === headCell.id ? sortOrder : 'asc'}
                      onClick={() => handleSort(headCell.id as SortField)}
                    >
                      {headCell.label}
                    </TableSortLabel>
                  ) : (
                    headCell.label
                  )}
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
            {isLoading ? (
              // Loading skeletons
              [...Array(5)].map((_, index) => (
                <TableRow key={index}>
                  {headCells.map((cell) => (
                    <TableCell key={cell.id}>
                      <Skeleton variant="text" width={cell.width || '100%'} />
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : data?.items.length === 0 ? (
              <TableRow>
                <TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}>
                  <Typography color="text.secondary">
                    {debouncedSearch
                      ? `No clients found matching "${debouncedSearch}"`
                      : 'No clients yet. Add your first client to get started.'}
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              data?.items.map((client) => (
                <ClientRow
                  key={client.id}
                  client={client}
                  formatDate={formatDate}
                  isFetching={isFetching}
                />
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Pagination */}
      <TablePagination
        component="div"
        count={data?.totalCount ?? 0}
        page={page}
        onPageChange={handleChangePage}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={handleChangeRowsPerPage}
        rowsPerPageOptions={[10, 20, 50, 100]}
        showFirstButton
        showLastButton
      />
    </Box>
  );
}

interface ClientRowProps {
  client: ClientListItem;
  formatDate: (date: string) => string;
  isFetching: boolean;
}

function ClientRow({ client, formatDate, isFetching }: ClientRowProps) {
  const router = useRouter();

  const handleRowClick = () => {
    router.push(`/clients/${client.id}`);
  };

  return (
    <TableRow
      hover
      onClick={handleRowClick}
      sx={{
        opacity: isFetching ? 0.6 : 1,
        cursor: 'pointer',
        '&:last-child td, &:last-child th': { border: 0 },
      }}
    >
      <TableCell>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <ClientTypeBadge clientType={client.clientType} />
          {client.isArchived && (
            <Chip
              icon={<Archive fontSize="small" />}
              label="Archived"
              size="small"
              color="default"
              variant="outlined"
              sx={{ opacity: 0.7 }}
            />
          )}
        </Box>
      </TableCell>
      <TableCell>
        <Typography
          variant="body2"
          fontWeight={500}
          sx={{ color: client.isArchived ? 'text.disabled' : 'text.primary' }}
        >
          {client.displayName}
        </Typography>
      </TableCell>
      <TableCell>
        <TaxIdentifierDisplay
          maskedValue={client.taxIdentifierMasked}
          last4={client.taxIdentifierLast4}
          clientType={client.clientType}
        />
      </TableCell>
      <TableCell>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0.5 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
            <Email sx={{ fontSize: 14, color: 'action.active' }} />
            <Typography variant="body2" color="text.secondary">
              {client.email}
            </Typography>
          </Box>
          {client.phone && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
              <Phone sx={{ fontSize: 14, color: 'action.active' }} />
              <Typography variant="body2" color="text.secondary">
                {client.phone}
              </Typography>
            </Box>
          )}
        </Box>
      </TableCell>
      <TableCell>
        <Typography variant="body2" color="text.secondary">
          {formatDate(client.createdAt)}
        </Typography>
      </TableCell>
    </TableRow>
  );
}

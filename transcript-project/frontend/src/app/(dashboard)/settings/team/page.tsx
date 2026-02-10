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
  Chip,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  CircularProgress,
  Alert,
} from '@mui/material';
import { PersonAdd } from '@mui/icons-material';
import { useListInvitationsQuery, useCreateInvitationMutation } from '@/lib/api/invitationsApi';
import type { InvitationStatus, UserRole } from '@/lib/types/invitation';

const statusColors: Record<InvitationStatus, 'default' | 'success' | 'warning' | 'error'> = {
  Pending: 'warning',
  Accepted: 'success',
  Expired: 'error',
  Revoked: 'default',
};

const ROLES: UserRole[] = ['Admin', 'Preparer', 'Reviewer'];

export default function TeamSettingsPage() {
  const { data: invitations, isLoading, error } = useListInvitationsQuery();
  const [createInvitation, { isLoading: isCreating }] = useCreateInvitationMutation();
  const [dialogOpen, setDialogOpen] = useState(false);
  const [email, setEmail] = useState('');
  const [role, setRole] = useState<UserRole>('Preparer');
  const [inviteError, setInviteError] = useState('');

  const handleInvite = async () => {
    setInviteError('');
    try {
      await createInvitation({ email, role }).unwrap();
      setDialogOpen(false);
      setEmail('');
      setRole('Preparer');
    } catch {
      setInviteError('Failed to send invitation. The email may already have a pending invite.');
    }
  };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" fontWeight={600}>
          Team Management
        </Typography>
        <Button
          variant="contained"
          startIcon={<PersonAdd />}
          onClick={() => setDialogOpen(true)}
        >
          Invite Member
        </Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>Failed to load invitations</Alert>}

      <Paper>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Email</TableCell>
                <TableCell>Role</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Expires</TableCell>
                <TableCell>Invited</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {(!invitations || invitations.length === 0) ? (
                <TableRow>
                  <TableCell colSpan={5} align="center">
                    <Typography color="text.secondary" sx={{ py: 2 }}>
                      No invitations yet. Invite team members to get started.
                    </Typography>
                  </TableCell>
                </TableRow>
              ) : (
                invitations.map((inv) => (
                  <TableRow key={inv.id}>
                    <TableCell>{inv.email}</TableCell>
                    <TableCell>
                      <Chip label={inv.role} size="small" variant="outlined" />
                    </TableCell>
                    <TableCell>
                      <Chip label={inv.status} size="small" color={statusColors[inv.status]} />
                    </TableCell>
                    <TableCell>
                      {new Date(inv.expiresAt).toLocaleDateString()}
                    </TableCell>
                    <TableCell>
                      {new Date(inv.createdAt).toLocaleDateString()}
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      {/* Invite Dialog */}
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Invite Team Member</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            {inviteError && <Alert severity="error">{inviteError}</Alert>}
            <TextField
              label="Email Address"
              required
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
            <FormControl>
              <InputLabel>Role</InputLabel>
              <Select
                value={role}
                label="Role"
                onChange={(e) => setRole(e.target.value as UserRole)}
              >
                {ROLES.map((r) => (
                  <MenuItem key={r} value={r}>{r}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleInvite}
            disabled={!email.trim() || isCreating}
          >
            {isCreating ? 'Sending...' : 'Send Invitation'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

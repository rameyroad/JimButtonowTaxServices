'use client';

import { use, useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  Button,
  Alert,
  Chip,
  CircularProgress,
  TextField,
  Divider,
} from '@mui/material';
import { CheckCircle, Cancel, AccessTime } from '@mui/icons-material';
import { useGetApprovalByTokenQuery, useRespondToApprovalMutation } from '@/lib/api/approvalsApi';
import type { ClientApprovalStatus } from '@/lib/types/clientApproval';

const statusConfig: Record<ClientApprovalStatus, { color: 'warning' | 'success' | 'error' | 'default'; label: string }> = {
  Pending: { color: 'warning', label: 'Awaiting Your Response' },
  Approved: { color: 'success', label: 'Approved' },
  Declined: { color: 'error', label: 'Declined' },
  Expired: { color: 'default', label: 'Expired' },
};

export default function ApprovePage({ params }: { params: Promise<{ token: string }> }) {
  const { token } = use(params);
  const { data: approval, isLoading, error } = useGetApprovalByTokenQuery(token);
  const [respond, { isLoading: isResponding }] = useRespondToApprovalMutation();
  const [notes, setNotes] = useState('');
  const [responded, setResponded] = useState(false);

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !approval) {
    return (
      <Paper sx={{ p: 4, textAlign: 'center' }}>
        <Typography variant="h5" fontWeight={600} sx={{ mb: 2 }}>
          Approval Not Found
        </Typography>
        <Typography color="text.secondary">
          This approval link may be invalid or has expired.
        </Typography>
      </Paper>
    );
  }

  const config = statusConfig[approval.status];
  const canRespond = approval.status === 'Pending' && new Date(approval.tokenExpiresAt) > new Date();

  const handleRespond = async (approved: boolean) => {
    await respond({
      token,
      data: { approved, notes: notes || undefined },
    });
    setResponded(true);
  };

  if (responded) {
    return (
      <Paper sx={{ p: 4, textAlign: 'center' }}>
        <CheckCircle color="success" sx={{ fontSize: 48, mb: 2 }} />
        <Typography variant="h5" fontWeight={600} sx={{ mb: 1 }}>
          Response Submitted
        </Typography>
        <Typography color="text.secondary">
          Thank you for your response. Your tax professional has been notified.
        </Typography>
      </Paper>
    );
  }

  return (
    <Paper sx={{ p: 4 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
        <AccessTime color="action" />
        <Typography variant="caption" color="text.secondary">
          Expires: {new Date(approval.tokenExpiresAt).toLocaleDateString()}
        </Typography>
      </Box>

      <Typography variant="h5" fontWeight={600} sx={{ mb: 1 }}>
        {approval.title}
      </Typography>

      <Chip
        label={config.label}
        color={config.color}
        size="small"
        sx={{ mb: 2 }}
      />

      {approval.description && (
        <Typography variant="body1" sx={{ mb: 3, whiteSpace: 'pre-wrap' }}>
          {approval.description}
        </Typography>
      )}

      {!canRespond && approval.status !== 'Pending' && (
        <Alert severity="info" sx={{ mb: 2 }}>
          This approval has already been {approval.status.toLowerCase()}.
          {approval.respondedAt && (
            <> Response received on {new Date(approval.respondedAt).toLocaleDateString()}.</>
          )}
        </Alert>
      )}

      {!canRespond && approval.status === 'Pending' && (
        <Alert severity="warning" sx={{ mb: 2 }}>
          This approval link has expired. Please contact your tax professional.
        </Alert>
      )}

      {canRespond && (
        <>
          <Divider sx={{ my: 3 }} />
          <TextField
            label="Notes (optional)"
            multiline
            rows={3}
            fullWidth
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            sx={{ mb: 3 }}
          />
          <Box sx={{ display: 'flex', gap: 2 }}>
            <Button
              variant="contained"
              color="success"
              size="large"
              startIcon={<CheckCircle />}
              onClick={() => handleRespond(true)}
              disabled={isResponding}
              sx={{ flex: 1 }}
            >
              Approve
            </Button>
            <Button
              variant="outlined"
              color="error"
              size="large"
              startIcon={<Cancel />}
              onClick={() => handleRespond(false)}
              disabled={isResponding}
              sx={{ flex: 1 }}
            >
              Decline
            </Button>
          </Box>
        </>
      )}
    </Paper>
  );
}

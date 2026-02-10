'use client';

import {
  Box,
  Breadcrumbs,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Link as MuiLink,
  Typography,
  CircularProgress,
  Alert,
  Paper,
} from '@mui/material';
import AccountTreeIcon from '@mui/icons-material/AccountTree';
import ArchiveIcon from '@mui/icons-material/Archive';
import EditIcon from '@mui/icons-material/Edit';
import RestoreIcon from '@mui/icons-material/Restore';
import Link from 'next/link';
import { useParams, useRouter } from 'next/navigation';
import { useState } from 'react';
import { useGetClientQuery, useArchiveClientMutation, useRestoreClientMutation } from '@/lib/api/clientsApi';
import BugReportIcon from '@mui/icons-material/BugReport';
import { ClientTypeBadge, TaxIdentifierDisplay, UpdateTaxIdentifierDialog } from '@/components/clients';
import { IssuesList } from '@/components/issues';

export default function ClientDetailPage() {
  const params = useParams();
  const router = useRouter();
  const clientId = params.id as string;

  const { data: client, isLoading, error } = useGetClientQuery(clientId);
  const [archiveClient, { isLoading: isArchiving }] = useArchiveClientMutation();
  const [restoreClient, { isLoading: isRestoring }] = useRestoreClientMutation();

  const [showArchiveDialog, setShowArchiveDialog] = useState(false);
  const [showTaxIdDialog, setShowTaxIdDialog] = useState(false);
  const [archiveError, setArchiveError] = useState<string | null>(null);

  const handleArchive = async () => {
    try {
      await archiveClient(clientId).unwrap();
      setShowArchiveDialog(false);
      router.push('/clients');
    } catch {
      setArchiveError('Failed to archive client. Please try again.');
    }
  };

  const handleRestore = async () => {
    try {
      await restoreClient(clientId).unwrap();
      setArchiveError(null);
    } catch {
      setArchiveError('Failed to restore client. Please try again.');
    }
  };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="error">Failed to load client details. Please try again.</Alert>
      </Box>
    );
  }

  if (!client) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="warning">Client not found.</Alert>
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      {/* Breadcrumbs */}
      <Breadcrumbs sx={{ mb: 2 }}>
        <MuiLink component={Link} href="/clients" underline="hover" color="inherit">
          Clients
        </MuiLink>
        <Typography color="text.primary">{client.displayName}</Typography>
      </Breadcrumbs>

      {/* Archive error */}
      {archiveError && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setArchiveError(null)}>
          {archiveError}
        </Alert>
      )}

      {/* Archived banner */}
      {client.isArchived && (
        <Alert severity="warning" sx={{ mb: 2 }}>
          This client is archived and hidden from the main client list.
        </Alert>
      )}

      {/* Page header */}
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
        <Typography variant="h4" component="h1">
          {client.displayName}
        </Typography>
        <ClientTypeBadge clientType={client.clientType} />
        {client.isArchived && <Chip label="Archived" color="warning" size="small" />}
        <Box sx={{ flexGrow: 1 }} />
        {!client.isArchived && (
          <>
            <Button
              component={Link}
              href={`/clients/${clientId}/workflows`}
              variant="outlined"
              startIcon={<AccountTreeIcon />}
            >
              Workflows
            </Button>
            <Button
              component={Link}
              href={`/clients/${clientId}/edit`}
              variant="outlined"
              startIcon={<EditIcon />}
            >
              Edit
            </Button>
            <Button
              variant="outlined"
              color="warning"
              startIcon={<ArchiveIcon />}
              onClick={() => setShowArchiveDialog(true)}
            >
              Archive
            </Button>
          </>
        )}
        {client.isArchived && (
          <Button
            variant="contained"
            color="primary"
            startIcon={<RestoreIcon />}
            onClick={handleRestore}
            disabled={isRestoring}
          >
            {isRestoring ? 'Restoring...' : 'Restore'}
          </Button>
        )}
      </Box>

      {/* Archive confirmation dialog */}
      <Dialog open={showArchiveDialog} onClose={() => setShowArchiveDialog(false)}>
        <DialogTitle>Archive Client</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to archive &quot;{client.displayName}&quot;? The client will be
            hidden from the main client list but can be restored later.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setShowArchiveDialog(false)} disabled={isArchiving}>
            Cancel
          </Button>
          <Button onClick={handleArchive} color="warning" disabled={isArchiving}>
            {isArchiving ? 'Archiving...' : 'Archive'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Client details */}
      <Paper sx={{ p: 3, maxWidth: 800 }}>
        <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 3 }}>
          {client.clientType === 'Individual' && (
            <>
              <Box>
                <Typography variant="caption" color="text.secondary">
                  First Name
                </Typography>
                <Typography>{client.firstName}</Typography>
              </Box>
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Last Name
                </Typography>
                <Typography>{client.lastName}</Typography>
              </Box>
            </>
          )}

          {client.clientType === 'Business' && (
            <>
              <Box sx={{ gridColumn: '1 / -1' }}>
                <Typography variant="caption" color="text.secondary">
                  Business Name
                </Typography>
                <Typography>{client.businessName}</Typography>
              </Box>
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Entity Type
                </Typography>
                <Typography>{client.entityType}</Typography>
              </Box>
              {client.responsibleParty && (
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Responsible Party
                  </Typography>
                  <Typography>{client.responsibleParty}</Typography>
                </Box>
              )}
            </>
          )}

          <Box>
            <Typography variant="caption" color="text.secondary">
              Tax Identifier
            </Typography>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <TaxIdentifierDisplay
                clientType={client.clientType}
                maskedValue={client.taxIdentifierMasked}
                last4={client.taxIdentifierLast4}
              />
              {!client.isArchived && (
                <Button size="small" onClick={() => setShowTaxIdDialog(true)}>
                  Update
                </Button>
              )}
            </Box>
          </Box>

          <Box>
            <Typography variant="caption" color="text.secondary">
              Email
            </Typography>
            <Typography>{client.email}</Typography>
          </Box>

          {client.phone && (
            <Box>
              <Typography variant="caption" color="text.secondary">
                Phone
              </Typography>
              <Typography>{client.phone}</Typography>
            </Box>
          )}

          <Box sx={{ gridColumn: '1 / -1' }}>
            <Typography variant="caption" color="text.secondary">
              Address
            </Typography>
            <Typography>
              {client.address.street1}
              {client.address.street2 && <>, {client.address.street2}</>}
              <br />
              {client.address.city}, {client.address.state} {client.address.postalCode}
            </Typography>
          </Box>
        </Box>
      </Paper>

      {/* Issues section */}
      {!client.isArchived && (
        <Box sx={{ mt: 3 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
            <BugReportIcon color="action" />
            <Typography variant="h6">Issues</Typography>
          </Box>
          <IssuesList clientId={clientId} />
        </Box>
      )}

      {/* Update tax identifier dialog */}
      <UpdateTaxIdentifierDialog
        open={showTaxIdDialog}
        onClose={() => setShowTaxIdDialog(false)}
        clientId={clientId}
        clientType={client.clientType}
        currentLast4={client.taxIdentifierLast4}
        version={client.version}
      />
    </Box>
  );
}

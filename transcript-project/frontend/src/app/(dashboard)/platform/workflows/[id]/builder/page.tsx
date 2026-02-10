'use client';

import { use } from 'react';
import { Box, CircularProgress, Alert } from '@mui/material';
import { useGetWorkflowDefinitionQuery } from '@/lib/api/workflowsApi';
import { WorkflowCanvas } from '@/components/workflows/builder';

interface PageProps {
  params: Promise<{ id: string }>;
}

export default function WorkflowBuilderPage({ params }: PageProps) {
  const { id } = use(params);
  const { data: workflow, isLoading, error } = useGetWorkflowDefinitionQuery(id);

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !workflow) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="error">Failed to load workflow definition.</Alert>
      </Box>
    );
  }

  return <WorkflowCanvas workflow={workflow} />;
}

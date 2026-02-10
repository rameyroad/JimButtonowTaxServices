'use client';

import { Box, Typography } from '@mui/material';
import { TasksTable } from '@/components/human-tasks';

export default function TasksPage() {
  return (
    <Box>
      <Typography variant="h4" fontWeight={600} sx={{ mb: 3 }}>
        Task Queue
      </Typography>
      <TasksTable />
    </Box>
  );
}

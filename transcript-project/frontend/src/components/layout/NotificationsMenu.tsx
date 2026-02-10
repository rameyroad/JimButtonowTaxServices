'use client';

import { useState, MouseEvent } from 'react';
import { IconButton, Badge, Menu, MenuItem, Typography, Divider, Tooltip, Box } from '@mui/material';
import { Notifications } from '@mui/icons-material';

const sampleNotifications = [
  { id: 1, text: 'Transcript processing complete', time: '5 min ago' },
  { id: 2, text: 'Authorization expiring soon', time: '1 hour ago' },
  { id: 3, text: 'System update available', time: '2 hours ago' },
];

export default function NotificationsMenu() {
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const open = Boolean(anchorEl);

  const handleOpen = (event: MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  return (
    <>
      <Tooltip title="Notifications">
        <IconButton color="inherit" onClick={handleOpen}>
          <Badge badgeContent={sampleNotifications.length} color="error">
            <Notifications />
          </Badge>
        </IconButton>
      </Tooltip>
      <Menu
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        slotProps={{
          paper: {
            sx: { width: 280, maxHeight: 360 },
          },
        }}
        transformOrigin={{ horizontal: 'right', vertical: 'top' }}
        anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
      >
        <Box sx={{ px: 2, py: 1 }}>
          <Typography variant="subtitle1" fontWeight="bold">
            Notifications
          </Typography>
        </Box>
        <Divider />
        {sampleNotifications.map((n) => (
          <MenuItem key={n.id} onClick={handleClose} sx={{ whiteSpace: 'normal' }}>
            <Box>
              <Typography variant="body2">{n.text}</Typography>
              <Typography variant="caption" color="text.secondary">
                {n.time}
              </Typography>
            </Box>
          </MenuItem>
        ))}
        <Divider />
        <MenuItem onClick={handleClose} sx={{ justifyContent: 'center' }}>
          <Typography variant="body2" color="primary">
            View all notifications
          </Typography>
        </MenuItem>
      </Menu>
    </>
  );
}

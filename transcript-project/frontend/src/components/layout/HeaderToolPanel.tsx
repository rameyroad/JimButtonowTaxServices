'use client';

import { Box } from '@mui/material';
import LanguageSwitcher from './LanguageSwitcher';
import ThemeToggle from './ThemeToggle';
import NotificationsMenu from './NotificationsMenu';
import UserAvatarMenu from './UserAvatarMenu';

export default function HeaderToolPanel() {
  return (
    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
      <LanguageSwitcher />
      <ThemeToggle />
      <NotificationsMenu />
      <UserAvatarMenu />
    </Box>
  );
}

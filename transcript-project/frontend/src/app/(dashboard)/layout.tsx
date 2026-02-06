'use client';

import { ReactNode } from 'react';
import { Box, Drawer, AppBar, Toolbar, Typography, List, ListItem, ListItemButton, ListItemIcon, ListItemText, Divider } from '@mui/material';
import { Dashboard, People, Description, Assignment, Settings, Group } from '@mui/icons-material';
import Link from 'next/link';
import { usePathname } from 'next/navigation';

const drawerWidth = 240;

const navItems = [
  { text: 'Dashboard', href: '/', icon: <Dashboard /> },
  { text: 'Clients', href: '/clients', icon: <People /> },
  { text: 'Authorizations', href: '/authorizations', icon: <Assignment /> },
  { text: 'Transcripts', href: '/transcripts', icon: <Description /> },
  { text: 'Team', href: '/team', icon: <Group /> },
  { text: 'Settings', href: '/settings', icon: <Settings /> },
];

interface DashboardLayoutProps {
  children: ReactNode;
}

export default function DashboardLayout({ children }: DashboardLayoutProps) {
  const pathname = usePathname();

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar
        position="fixed"
        sx={{
          width: `calc(100% - ${drawerWidth}px)`,
          ml: `${drawerWidth}px`,
        }}
      >
        <Toolbar>
          <Typography variant="h6" noWrap component="div">
            IRS Transcript Analyzer
          </Typography>
        </Toolbar>
      </AppBar>

      <Drawer
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          '& .MuiDrawer-paper': {
            width: drawerWidth,
            boxSizing: 'border-box',
          },
        }}
        variant="permanent"
        anchor="left"
      >
        <Toolbar>
          <Typography variant="h6" noWrap component="div">
            Menu
          </Typography>
        </Toolbar>
        <Divider />
        <List>
          {navItems.map((item) => (
            <ListItem key={item.text} disablePadding>
              <ListItemButton
                component={Link}
                href={item.href}
                selected={pathname === item.href}
              >
                <ListItemIcon>{item.icon}</ListItemIcon>
                <ListItemText primary={item.text} />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      </Drawer>

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          bgcolor: 'background.default',
          p: 3,
          mt: 8,
          minHeight: '100vh',
        }}
      >
        {children}
      </Box>
    </Box>
  );
}

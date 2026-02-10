'use client';

import { useState, MouseEvent } from 'react';
import { IconButton, Menu, MenuItem, ListItemIcon, ListItemText, Tooltip } from '@mui/material';
import { Language, Check } from '@mui/icons-material';

const languages = [
  { code: 'en', label: 'English' },
  { code: 'es', label: 'Español' },
  { code: 'fr', label: 'Français' },
  { code: 'de', label: 'Deutsch' },
];

export default function LanguageSwitcher() {
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const [selected, setSelected] = useState('en');
  const open = Boolean(anchorEl);

  const handleOpen = (event: MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleSelect = (code: string) => {
    setSelected(code);
    handleClose();
  };

  return (
    <>
      <Tooltip title="Language">
        <IconButton color="inherit" onClick={handleOpen}>
          <Language />
        </IconButton>
      </Tooltip>
      <Menu
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        transformOrigin={{ horizontal: 'right', vertical: 'top' }}
        anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
      >
        {languages.map((lang) => (
          <MenuItem key={lang.code} onClick={() => handleSelect(lang.code)} selected={lang.code === selected}>
            {lang.code === selected && (
              <ListItemIcon>
                <Check fontSize="small" />
              </ListItemIcon>
            )}
            <ListItemText inset={lang.code !== selected}>{lang.label}</ListItemText>
          </MenuItem>
        ))}
      </Menu>
    </>
  );
}

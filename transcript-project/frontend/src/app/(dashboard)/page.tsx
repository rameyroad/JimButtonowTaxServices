'use client';

import {
  Box,
  Grid,
  Paper,
  Typography,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Chip,
  LinearProgress,
} from '@mui/material';
import {
  People,
  Description,
  Assignment,
  AttachMoney,
  TrendingUp,
  TrendingDown,
  Schedule,
  Warning,
  CheckCircle,
  HourglassEmpty,
} from '@mui/icons-material';

interface MetricCardProps {
  title: string;
  value: string | number;
  change: number;
  changeType: 'increase' | 'decrease';
  icon: React.ReactNode;
  description: string;
}

function MetricCard({ title, value, change, changeType, icon, description }: MetricCardProps) {
  const isPositive = changeType === 'increase';
  return (
    <Paper sx={{ p: 3 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
        <Typography variant="body2" color="text.secondary">
          {title}
        </Typography>
        <Box sx={{ color: 'action.active' }}>{icon}</Box>
      </Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 0.5 }}>
        {value}
      </Typography>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
        {isPositive ? (
          <TrendingUp fontSize="small" color="success" />
        ) : (
          <TrendingDown fontSize="small" color={title === 'Delayed Returns' ? 'success' : 'error'} />
        )}
        <Typography variant="caption" color={isPositive ? 'success.main' : (title === 'Delayed Returns' ? 'success.main' : 'error.main')}>
          {isPositive ? '+' : ''}{change}%
        </Typography>
        <Typography variant="caption" color="text.secondary" sx={{ ml: 0.5 }}>
          {description}
        </Typography>
      </Box>
    </Paper>
  );
}

interface ActivityItem {
  icon: React.ReactNode;
  primary: string;
  secondary: string;
  time: string;
}

const recentActivity: ActivityItem[] = [
  {
    icon: <People color="primary" fontSize="small" />,
    primary: 'New client added: Johnson, Michael',
    secondary: 'Individual — SSN ***-**-4532',
    time: '10 min ago',
  },
  {
    icon: <Description color="info" fontSize="small" />,
    primary: 'Transcript processed: Smith, Sarah',
    secondary: 'Form 1040 — Tax Year 2024',
    time: '25 min ago',
  },
  {
    icon: <Assignment color="warning" fontSize="small" />,
    primary: 'Authorization expiring: Acme Corp',
    secondary: 'Form 8821 — Expires in 3 days',
    time: '1 hr ago',
  },
  {
    icon: <CheckCircle color="success" fontSize="small" />,
    primary: 'Return filed: Davis, Robert',
    secondary: 'Form 1040 — E-filed successfully',
    time: '2 hrs ago',
  },
  {
    icon: <People color="primary" fontSize="small" />,
    primary: 'Client updated: Martinez LLC',
    secondary: 'Address and EIN updated',
    time: '3 hrs ago',
  },
  {
    icon: <Description color="info" fontSize="small" />,
    primary: 'Transcript requested: Chen, Wei',
    secondary: 'Account transcript — Tax Year 2023',
    time: '4 hrs ago',
  },
];

export default function DashboardPage() {
  return (
    <Box>
      <Box sx={{ mb: 3 }}>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 700 }}>
          Dashboard Overview
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Welcome back! Here&apos;s a snapshot of your tax practice.
        </Typography>
      </Box>

      {/* Key Metrics */}
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid size={{ xs: 12, sm: 6, xl: 3 }}>
          <MetricCard
            title="Active Clients"
            value={247}
            change={12}
            changeType="increase"
            icon={<People />}
            description="vs. last month"
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, xl: 3 }}>
          <MetricCard
            title="Returns Filed (MTD)"
            value={38}
            change={8}
            changeType="increase"
            icon={<Description />}
            description="vs. last month"
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, xl: 3 }}>
          <MetricCard
            title="Pending Authorizations"
            value={15}
            change={-5}
            changeType="decrease"
            icon={<Assignment />}
            description="vs. last week"
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, xl: 3 }}>
          <MetricCard
            title="Revenue (MTD)"
            value="$84,590"
            change={15}
            changeType="increase"
            icon={<AttachMoney />}
            description="vs. last month"
          />
        </Grid>
      </Grid>

      {/* Charts Row */}
      <Grid container spacing={3} sx={{ mb: 3 }}>
        {/* Filing Season Progress */}
        <Grid size={{ xs: 12, md: 6 }}>
          <Paper sx={{ p: 3, height: '100%' }}>
            <Typography variant="h6" sx={{ mb: 2 }}>
              Filing Season Progress
            </Typography>
            <Box sx={{ mb: 3 }}>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                <Typography variant="body2">Individual Returns</Typography>
                <Typography variant="body2" fontWeight={600}>142 / 200</Typography>
              </Box>
              <LinearProgress variant="determinate" value={71} sx={{ height: 8, borderRadius: 4 }} />
            </Box>
            <Box sx={{ mb: 3 }}>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                <Typography variant="body2">Business Returns</Typography>
                <Typography variant="body2" fontWeight={600}>58 / 85</Typography>
              </Box>
              <LinearProgress variant="determinate" value={68} color="secondary" sx={{ height: 8, borderRadius: 4 }} />
            </Box>
            <Box sx={{ mb: 3 }}>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                <Typography variant="body2">Extensions Filed</Typography>
                <Typography variant="body2" fontWeight={600}>12 / 30</Typography>
              </Box>
              <LinearProgress variant="determinate" value={40} color="warning" sx={{ height: 8, borderRadius: 4 }} />
            </Box>
            <Box>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                <Typography variant="body2">Amended Returns</Typography>
                <Typography variant="body2" fontWeight={600}>5 / 8</Typography>
              </Box>
              <LinearProgress variant="determinate" value={62.5} color="info" sx={{ height: 8, borderRadius: 4 }} />
            </Box>
          </Paper>
        </Grid>

        {/* Return Status Breakdown */}
        <Grid size={{ xs: 12, md: 6 }}>
          <Paper sx={{ p: 3, height: '100%' }}>
            <Typography variant="h6" sx={{ mb: 2 }}>
              Return Status Breakdown
            </Typography>
            {[
              { label: 'E-Filed & Accepted', value: 156, color: 'success' as const },
              { label: 'Awaiting Client Info', value: 34, color: 'warning' as const },
              { label: 'In Preparation', value: 22, color: 'info' as const },
              { label: 'Under Review', value: 18, color: 'secondary' as const },
              { label: 'Rejected / Needs Fix', value: 4, color: 'error' as const },
            ].map((item) => (
              <Box key={item.label} sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', py: 1.5, borderBottom: '1px solid', borderColor: 'divider', '&:last-child': { borderBottom: 'none' } }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <Box sx={{ width: 8, height: 8, borderRadius: '50%', bgcolor: `${item.color}.main` }} />
                  <Typography variant="body2">{item.label}</Typography>
                </Box>
                <Chip label={item.value} size="small" color={item.color} variant="outlined" />
              </Box>
            ))}
          </Paper>
        </Grid>
      </Grid>

      {/* Activity and Alerts Row */}
      <Grid container spacing={3} sx={{ mb: 3 }}>
        {/* Recent Activity */}
        <Grid size={{ xs: 12, md: 8 }}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" sx={{ mb: 1 }}>
              Recent Activity
            </Typography>
            <List disablePadding>
              {recentActivity.map((item, index) => (
                <ListItem key={index} divider={index < recentActivity.length - 1} sx={{ px: 0 }}>
                  <ListItemIcon sx={{ minWidth: 36 }}>
                    {item.icon}
                  </ListItemIcon>
                  <ListItemText
                    primary={item.primary}
                    secondary={item.secondary}
                    primaryTypographyProps={{ variant: 'body2' }}
                    secondaryTypographyProps={{ variant: 'caption' }}
                  />
                  <Typography variant="caption" color="text.secondary" sx={{ whiteSpace: 'nowrap', ml: 2 }}>
                    {item.time}
                  </Typography>
                </ListItem>
              ))}
            </List>
          </Paper>
        </Grid>

        {/* Alerts & Reminders */}
        <Grid size={{ xs: 12, md: 4 }}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" sx={{ mb: 2 }}>
              Alerts &amp; Reminders
            </Typography>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
              <Box sx={{ display: 'flex', gap: 1.5 }}>
                <Warning color="warning" fontSize="small" sx={{ mt: 0.25 }} />
                <Box>
                  <Typography variant="body2" fontWeight={600}>Expiring Authorizations</Typography>
                  <Typography variant="caption" color="text.secondary">
                    5 Form 8821s expire within 30 days
                  </Typography>
                </Box>
              </Box>
              <Box sx={{ display: 'flex', gap: 1.5 }}>
                <Schedule color="error" fontSize="small" sx={{ mt: 0.25 }} />
                <Box>
                  <Typography variant="body2" fontWeight={600}>Deadline Approaching</Typography>
                  <Typography variant="caption" color="text.secondary">
                    April 15 filing deadline in 65 days
                  </Typography>
                </Box>
              </Box>
              <Box sx={{ display: 'flex', gap: 1.5 }}>
                <HourglassEmpty color="info" fontSize="small" sx={{ mt: 0.25 }} />
                <Box>
                  <Typography variant="body2" fontWeight={600}>Pending Client Responses</Typography>
                  <Typography variant="caption" color="text.secondary">
                    12 clients haven&apos;t sent documents
                  </Typography>
                </Box>
              </Box>
              <Box sx={{ display: 'flex', gap: 1.5 }}>
                <Description color="primary" fontSize="small" sx={{ mt: 0.25 }} />
                <Box>
                  <Typography variant="body2" fontWeight={600}>Transcript Updates</Typography>
                  <Typography variant="caption" color="text.secondary">
                    3 new transcripts ready for review
                  </Typography>
                </Box>
              </Box>
            </Box>
          </Paper>

          {/* Performance Highlights */}
          <Paper sx={{ p: 3, mt: 3 }}>
            <Typography variant="h6" sx={{ mb: 2 }}>
              Performance Highlights
            </Typography>
            {[
              { label: 'On-time Filing Rate', value: '96.4%' },
              { label: 'Avg. Turnaround Time', value: '4.2 days' },
              { label: 'Client Retention', value: '92%' },
              { label: 'Avg. Refund Amount', value: '$3,842' },
            ].map((item) => (
              <Box key={item.label} sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', py: 1 }}>
                <Typography variant="body2" color="text.secondary">{item.label}</Typography>
                <Typography variant="body2" fontWeight={600}>{item.value}</Typography>
              </Box>
            ))}
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}

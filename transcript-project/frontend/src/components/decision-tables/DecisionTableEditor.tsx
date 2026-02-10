'use client';

import { useState } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  IconButton,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Switch,
  FormControlLabel,
  Alert,
  Tooltip,
} from '@mui/material';
import {
  Add,
  Delete,
  Edit,
  Publish,
} from '@mui/icons-material';
import {
  useGetDecisionTableQuery,
  useUpdateDecisionTableMutation,
  useAddDecisionRuleMutation,
  useUpdateDecisionRuleMutation,
  useRemoveDecisionRuleMutation,
  usePublishDecisionTableMutation,
} from '@/lib/api/decisionTablesApi';
import type {
  ConditionOperator,
  RuleConditionInput,
  RuleOutputInput,
} from '@/lib/types/decisionTable';

const operatorLabels: Record<ConditionOperator, string> = {
  Equals: '=',
  NotEquals: '!=',
  LessThan: '<',
  GreaterThan: '>',
  LessThanOrEqual: '<=',
  GreaterThanOrEqual: '>=',
  Between: 'between',
  Contains: 'contains',
  IsEmpty: 'is empty',
  IsNotEmpty: 'is not empty',
};

interface DecisionTableEditorProps {
  id: string;
}

export default function DecisionTableEditor({ id }: DecisionTableEditorProps) {
  const { data: table, isLoading } = useGetDecisionTableQuery(id);
  const [updateTable] = useUpdateDecisionTableMutation();
  const [addRule] = useAddDecisionRuleMutation();
  const [updateRule] = useUpdateDecisionRuleMutation();
  const [removeRule] = useRemoveDecisionRuleMutation();
  const [publishTable] = usePublishDecisionTableMutation();

  const [editingName, setEditingName] = useState(false);
  const [nameValue, setNameValue] = useState('');
  const [descValue, setDescValue] = useState('');
  const [ruleDialogOpen, setRuleDialogOpen] = useState(false);
  const [editingRuleId, setEditingRuleId] = useState<string | null>(null);
  const [publishError, setPublishError] = useState<string | null>(null);

  // Rule form state
  const [rulePriority, setRulePriority] = useState(0);
  const [ruleEnabled, setRuleEnabled] = useState(true);
  const [ruleConditions, setRuleConditions] = useState<RuleConditionInput[]>([]);
  const [ruleOutputs, setRuleOutputs] = useState<RuleOutputInput[]>([]);

  if (isLoading || !table) {
    return <Typography>Loading...</Typography>;
  }

  const inputColumns = table.columns.filter((c) => c.isInput);
  const outputColumns = table.columns.filter((c) => !c.isInput);

  const handleSaveName = async () => {
    await updateTable({ id, data: { name: nameValue, description: descValue } });
    setEditingName(false);
  };

  const handleStartEditName = () => {
    setNameValue(table.name);
    setDescValue(table.description || '');
    setEditingName(true);
  };

  const handleOpenAddRule = () => {
    setEditingRuleId(null);
    setRulePriority(table.rules.length);
    setRuleEnabled(true);
    setRuleConditions(inputColumns.map((c) => ({ columnKey: c.key, operator: 'Equals' as ConditionOperator, value: '' })));
    setRuleOutputs(outputColumns.map((c) => ({ columnKey: c.key, value: '' })));
    setRuleDialogOpen(true);
  };

  const handleOpenEditRule = (ruleId: string) => {
    const rule = table.rules.find((r) => r.id === ruleId);
    if (!rule) return;
    setEditingRuleId(ruleId);
    setRulePriority(rule.priority);
    setRuleEnabled(rule.isEnabled);
    setRuleConditions(
      rule.conditions.map((c) => ({
        columnKey: c.columnKey,
        operator: c.operator,
        value: c.value,
        value2: c.value2,
      }))
    );
    setRuleOutputs(
      rule.outputs.map((o) => ({
        columnKey: o.columnKey,
        value: o.value,
      }))
    );
    setRuleDialogOpen(true);
  };

  const handleSaveRule = async () => {
    if (editingRuleId) {
      await updateRule({
        decisionTableId: id,
        ruleId: editingRuleId,
        data: { priority: rulePriority, isEnabled: ruleEnabled, conditions: ruleConditions, outputs: ruleOutputs },
      });
    } else {
      await addRule({
        decisionTableId: id,
        data: { priority: rulePriority, isEnabled: ruleEnabled, conditions: ruleConditions, outputs: ruleOutputs },
      });
    }
    setRuleDialogOpen(false);
  };

  const handleRemoveRule = async (ruleId: string) => {
    await removeRule({ decisionTableId: id, ruleId });
  };

  const handlePublish = async () => {
    try {
      setPublishError(null);
      await publishTable(id).unwrap();
    } catch (err: unknown) {
      const error = err as { data?: { detail?: string } };
      setPublishError(error.data?.detail || 'Failed to publish');
    }
  };

  return (
    <Box>
      {/* Header */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 3 }}>
        <Box sx={{ flex: 1 }}>
          {editingName ? (
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
              <TextField
                value={nameValue}
                onChange={(e) => setNameValue(e.target.value)}
                size="small"
                label="Name"
                fullWidth
              />
              <TextField
                value={descValue}
                onChange={(e) => setDescValue(e.target.value)}
                size="small"
                label="Description"
                multiline
                rows={2}
                fullWidth
              />
              <Box sx={{ display: 'flex', gap: 1 }}>
                <Button size="small" variant="contained" onClick={handleSaveName}>Save</Button>
                <Button size="small" onClick={() => setEditingName(false)}>Cancel</Button>
              </Box>
            </Box>
          ) : (
            <Box>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <Typography variant="h5" fontWeight={600}>{table.name}</Typography>
                <Chip
                  label={table.status}
                  size="small"
                  color={table.status === 'Published' ? 'success' : table.status === 'Archived' ? 'warning' : 'default'}
                />
                <Chip label={`v${table.version}`} size="small" variant="outlined" />
                <IconButton size="small" onClick={handleStartEditName}><Edit fontSize="small" /></IconButton>
              </Box>
              {table.description && (
                <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
                  {table.description}
                </Typography>
              )}
            </Box>
          )}
        </Box>
        {table.status !== 'Published' && (
          <Button
            variant="contained"
            color="success"
            startIcon={<Publish />}
            onClick={handlePublish}
            disabled={table.rules.length === 0}
          >
            Publish
          </Button>
        )}
      </Box>

      {publishError && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setPublishError(null)}>
          {publishError}
        </Alert>
      )}

      {/* Columns */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" sx={{ mb: 2 }}>Columns</Typography>
          <Box sx={{ display: 'flex', gap: 4 }}>
            <Box sx={{ flex: 1 }}>
              <Typography variant="subtitle2" color="text.secondary" sx={{ mb: 1 }}>Input Columns</Typography>
              {inputColumns.map((col) => (
                <Chip
                  key={col.id}
                  label={`${col.name} (${col.dataType})`}
                  variant="outlined"
                  color="primary"
                  size="small"
                  sx={{ mr: 0.5, mb: 0.5 }}
                />
              ))}
            </Box>
            <Divider orientation="vertical" flexItem />
            <Box sx={{ flex: 1 }}>
              <Typography variant="subtitle2" color="text.secondary" sx={{ mb: 1 }}>Output Columns</Typography>
              {outputColumns.map((col) => (
                <Chip
                  key={col.id}
                  label={`${col.name} (${col.dataType})`}
                  variant="outlined"
                  color="secondary"
                  size="small"
                  sx={{ mr: 0.5, mb: 0.5 }}
                />
              ))}
            </Box>
          </Box>
        </CardContent>
      </Card>

      {/* Rules */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h6">Rules ({table.rules.length})</Typography>
        <Button variant="outlined" startIcon={<Add />} onClick={handleOpenAddRule}>
          Add Rule
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell sx={{ width: 60 }}>Priority</TableCell>
              <TableCell>Conditions</TableCell>
              <TableCell>Outputs</TableCell>
              <TableCell sx={{ width: 80 }}>Enabled</TableCell>
              <TableCell sx={{ width: 100 }}>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {table.rules.length === 0 ? (
              <TableRow>
                <TableCell colSpan={5} align="center" sx={{ py: 3 }}>
                  <Typography color="text.secondary">No rules defined yet</Typography>
                </TableCell>
              </TableRow>
            ) : (
              table.rules.map((rule) => (
                <TableRow key={rule.id} sx={{ opacity: rule.isEnabled ? 1 : 0.5 }}>
                  <TableCell>{rule.priority}</TableCell>
                  <TableCell>
                    {rule.conditions.map((c, i) => (
                      <Typography key={i} variant="body2">
                        {c.columnKey} {operatorLabels[c.operator]} {c.value}
                        {c.value2 ? ` and ${c.value2}` : ''}
                      </Typography>
                    ))}
                  </TableCell>
                  <TableCell>
                    {rule.outputs.map((o, i) => (
                      <Typography key={i} variant="body2">
                        {o.columnKey} = {o.value}
                      </Typography>
                    ))}
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={rule.isEnabled ? 'Yes' : 'No'}
                      size="small"
                      color={rule.isEnabled ? 'success' : 'default'}
                    />
                  </TableCell>
                  <TableCell>
                    <Tooltip title="Edit">
                      <IconButton size="small" onClick={() => handleOpenEditRule(rule.id)}>
                        <Edit fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Delete">
                      <IconButton size="small" color="error" onClick={() => handleRemoveRule(rule.id)}>
                        <Delete fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Rule Dialog */}
      <Dialog open={ruleDialogOpen} onClose={() => setRuleDialogOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>{editingRuleId ? 'Edit Rule' : 'Add Rule'}</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', gap: 2, mt: 1, mb: 2 }}>
            <TextField
              label="Priority"
              type="number"
              size="small"
              value={rulePriority}
              onChange={(e) => setRulePriority(parseInt(e.target.value, 10))}
              sx={{ width: 120 }}
            />
            <FormControlLabel
              control={<Switch checked={ruleEnabled} onChange={(e) => setRuleEnabled(e.target.checked)} />}
              label="Enabled"
            />
          </Box>

          <Typography variant="subtitle2" sx={{ mb: 1 }}>Conditions</Typography>
          {ruleConditions.map((condition, index) => (
            <Box key={index} sx={{ display: 'flex', gap: 1, mb: 1, alignItems: 'center' }}>
              <TextField
                size="small"
                label="Column"
                value={condition.columnKey}
                onChange={(e) => {
                  const updated = [...ruleConditions];
                  updated[index] = { ...updated[index], columnKey: e.target.value };
                  setRuleConditions(updated);
                }}
                sx={{ width: 150 }}
              />
              <FormControl size="small" sx={{ width: 160 }}>
                <InputLabel>Operator</InputLabel>
                <Select
                  value={condition.operator}
                  label="Operator"
                  onChange={(e) => {
                    const updated = [...ruleConditions];
                    updated[index] = { ...updated[index], operator: e.target.value as ConditionOperator };
                    setRuleConditions(updated);
                  }}
                >
                  {Object.entries(operatorLabels).map(([key, label]) => (
                    <MenuItem key={key} value={key}>{label}</MenuItem>
                  ))}
                </Select>
              </FormControl>
              <TextField
                size="small"
                label="Value"
                value={condition.value}
                onChange={(e) => {
                  const updated = [...ruleConditions];
                  updated[index] = { ...updated[index], value: e.target.value };
                  setRuleConditions(updated);
                }}
                sx={{ flex: 1 }}
              />
              {condition.operator === 'Between' && (
                <TextField
                  size="small"
                  label="Value 2"
                  value={condition.value2 || ''}
                  onChange={(e) => {
                    const updated = [...ruleConditions];
                    updated[index] = { ...updated[index], value2: e.target.value };
                    setRuleConditions(updated);
                  }}
                  sx={{ width: 150 }}
                />
              )}
              <IconButton
                size="small"
                color="error"
                onClick={() => setRuleConditions(ruleConditions.filter((_, i) => i !== index))}
              >
                <Delete fontSize="small" />
              </IconButton>
            </Box>
          ))}
          <Button
            size="small"
            onClick={() => setRuleConditions([...ruleConditions, { columnKey: '', operator: 'Equals', value: '' }])}
          >
            Add Condition
          </Button>

          <Divider sx={{ my: 2 }} />

          <Typography variant="subtitle2" sx={{ mb: 1 }}>Outputs</Typography>
          {ruleOutputs.map((output, index) => (
            <Box key={index} sx={{ display: 'flex', gap: 1, mb: 1, alignItems: 'center' }}>
              <TextField
                size="small"
                label="Column"
                value={output.columnKey}
                onChange={(e) => {
                  const updated = [...ruleOutputs];
                  updated[index] = { ...updated[index], columnKey: e.target.value };
                  setRuleOutputs(updated);
                }}
                sx={{ width: 200 }}
              />
              <TextField
                size="small"
                label="Value"
                value={output.value}
                onChange={(e) => {
                  const updated = [...ruleOutputs];
                  updated[index] = { ...updated[index], value: e.target.value };
                  setRuleOutputs(updated);
                }}
                sx={{ flex: 1 }}
              />
              <IconButton
                size="small"
                color="error"
                onClick={() => setRuleOutputs(ruleOutputs.filter((_, i) => i !== index))}
              >
                <Delete fontSize="small" />
              </IconButton>
            </Box>
          ))}
          <Button
            size="small"
            onClick={() => setRuleOutputs([...ruleOutputs, { columnKey: '', value: '' }])}
          >
            Add Output
          </Button>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRuleDialogOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleSaveRule}
            disabled={ruleConditions.length === 0 || ruleOutputs.length === 0}
          >
            {editingRuleId ? 'Update' : 'Add'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

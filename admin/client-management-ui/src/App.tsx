import React, { useState, useEffect } from 'react';
import {
  Container,
  Paper,
  Typography,
  Box,
  TextField,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Chip,
  Alert,
  Snackbar,
  Tabs,
  Tab,
} from '@mui/material';
import { Delete as DeleteIcon, Edit as EditIcon, Add as AddIcon, Refresh as RefreshIcon } from '@mui/icons-material';
import { clientService, Client } from './services/clientService';
import { topicService, Topic } from './services/topicService';
import { TopicManagement } from './components/TopicManagement';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      {...other}
    >
      {value === index && (
        <Box sx={{ p: 3 }}>
          {children}
        </Box>
      )}
    </div>
  );
}

function App() {
  const [clients, setClients] = useState<Client[]>([]);
  const [topics, setTopics] = useState<Topic[]>([]);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingClient, setEditingClient] = useState<Client | null>(null);
  const [newClient, setNewClient] = useState<Omit<Client, 'id'>>({
    clientId: '',
    clientSecret: '',
    topics: [],
  });
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [tabValue, setTabValue] = useState(0);

  useEffect(() => {
    loadClients();
    loadTopics();
  }, []);

  const loadClients = async () => {
    try {
      setLoading(true);
      const data = await clientService.getAllClients();
      setClients(data);
    } catch (err) {
      setError('Failed to load clients');
    } finally {
      setLoading(false);
    }
  };

  const loadTopics = async () => {
    try {
      const data = await topicService.getAllTopics();
      setTopics(data);
    } catch (err) {
      setError('Failed to load topics');
    }
  };

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
    if (newValue === 0) {
      loadTopics(); // Refresh topics when switching to client tab
    }
  };

  const handleOpenDialog = async (client?: Client) => {
    await loadTopics(); // Refresh topics before opening dialog
    if (client) {
      setEditingClient(client);
      setNewClient({
        clientId: client.clientId,
        clientSecret: client.clientSecret,
        topics: client.topics,
      });
    } else {
      setEditingClient(null);
      setNewClient({
        clientId: '',
        clientSecret: '',
        topics: [],
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingClient(null);
  };

  const handleGenerateCredentials = async () => {
    try {
      setLoading(true);
      const credentials = await clientService.generateCredentials();
      setNewClient(prev => ({
        ...prev,
        clientId: credentials.clientId,
        clientSecret: credentials.clientSecret,
      }));
    } catch (err) {
      setError('Failed to generate credentials');
    } finally {
      setLoading(false);
    }
  };

  const handleSaveClient = async () => {
    try {
      setLoading(true);
      if (editingClient) {
        const updated = await clientService.updateClient(editingClient.id, newClient);
        setClients(clients.map(c => c.id === editingClient.id ? updated : c));
      } else {
        const created = await clientService.createClient(newClient);
        setClients([...clients, created]);
      }
      handleCloseDialog();
    } catch (err) {
      setError('Failed to save client');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteClient = async (id: string) => {
    try {
      setLoading(true);
      await clientService.deleteClient(id);
      setClients(clients.filter(c => c.id !== id));
    } catch (err) {
      setError('Failed to delete client');
    } finally {
      setLoading(false);
    }
  };

  // Callback for when topics are modified in the TopicManagement component
  const handleTopicsModified = () => {
    loadTopics();
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4 }}>
      <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
        <Tabs value={tabValue} onChange={handleTabChange}>
          <Tab label="Clients" />
          <Tab label="Topics" />
        </Tabs>
      </Box>

      <TabPanel value={tabValue} index={0}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 4 }}>
          <Typography variant="h4" component="h1">
            Client Management
          </Typography>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => handleOpenDialog()}
            disabled={loading}
          >
            Add Client
          </Button>
        </Box>

        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Client ID</TableCell>
                <TableCell>Client Secret</TableCell>
                <TableCell>Topics</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {clients.map((client) => (
                <TableRow key={client.id}>
                  <TableCell>{client.clientId}</TableCell>
                  <TableCell>{client.clientSecret}</TableCell>
                  <TableCell>
                    {client.topics.map((topic) => (
                      <Chip key={topic} label={topic} size="small" sx={{ mr: 1 }} />
                    ))}
                  </TableCell>
                  <TableCell>
                    <IconButton onClick={() => handleOpenDialog(client)} disabled={loading}>
                      <EditIcon />
                    </IconButton>
                    <IconButton onClick={() => handleDeleteClient(client.id)} disabled={loading}>
                      <DeleteIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>

        <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
          <DialogTitle>
            {editingClient ? 'Edit Client' : 'Add New Client'}
          </DialogTitle>
          <DialogContent>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 2 }}>
              <Box sx={{ display: 'flex', gap: 1 }}>
                <TextField
                  label="Client ID"
                  value={newClient.clientId}
                  onChange={(e) => setNewClient({ ...newClient, clientId: e.target.value })}
                  fullWidth
                  disabled={loading}
                />
                <Button
                  variant="outlined"
                  startIcon={<RefreshIcon />}
                  onClick={handleGenerateCredentials}
                  disabled={loading}
                  sx={{ minWidth: '120px' }}
                >
                  Generate
                </Button>
              </Box>
              <TextField
                label="Client Secret"
                value={newClient.clientSecret}
                onChange={(e) => setNewClient({ ...newClient, clientSecret: e.target.value })}
                fullWidth
                disabled={loading}
              />
              <FormControl fullWidth>
                <InputLabel>Topics</InputLabel>
                <Select
                  multiple
                  value={newClient.topics}
                  onChange={(e) => setNewClient({ ...newClient, topics: e.target.value as string[] })}
                  renderValue={(selected) => (
                    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                      {selected.map((value) => (
                        <Chip key={value} label={value} />
                      ))}
                    </Box>
                  )}
                  disabled={loading}
                >
                  {topics.map((topic) => (
                    <MenuItem key={topic.id} value={topic.name}>
                      {topic.name}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Box>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDialog} disabled={loading}>Cancel</Button>
            <Button onClick={handleSaveClient} variant="contained" disabled={loading}>
              Save
            </Button>
          </DialogActions>
        </Dialog>
      </TabPanel>

      <TabPanel value={tabValue} index={1}>
        <TopicManagement onTopicsModified={handleTopicsModified} />
      </TabPanel>

      <Snackbar
        open={!!error}
        autoHideDuration={6000}
        onClose={() => setError(null)}
      >
        <Alert onClose={() => setError(null)} severity="error">
          {error}
        </Alert>
      </Snackbar>
    </Container>
  );
}

export default App; 
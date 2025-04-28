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
  Alert,
  Snackbar,
} from '@mui/material';
import { Delete as DeleteIcon, Edit as EditIcon, Add as AddIcon } from '@mui/icons-material';
import { topicService, Topic } from '../services/topicService';

interface TopicManagementProps {
  onTopicsModified?: () => void;
}

export const TopicManagement: React.FC<TopicManagementProps> = ({ onTopicsModified }) => {
  const [topics, setTopics] = useState<Topic[]>([]);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingTopic, setEditingTopic] = useState<Topic | null>(null);
  const [newTopic, setNewTopic] = useState<Omit<Topic, 'id'>>({
    name: '',
    description: '',
  });
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadTopics();
  }, []);

  const loadTopics = async () => {
    try {
      setLoading(true);
      const data = await topicService.getAllTopics();
      setTopics(data);
    } catch (err) {
      setError('Failed to load topics');
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (topic?: Topic) => {
    if (topic) {
      setEditingTopic(topic);
      setNewTopic({
        name: topic.name,
        description: topic.description,
      });
    } else {
      setEditingTopic(null);
      setNewTopic({
        name: '',
        description: '',
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingTopic(null);
  };

  const handleSaveTopic = async () => {
    try {
      setLoading(true);
      if (editingTopic) {
        const updated = await topicService.updateTopic(editingTopic.id, newTopic);
        setTopics(topics.map(t => t.id === editingTopic.id ? updated : t));
      } else {
        const created = await topicService.createTopic(newTopic);
        setTopics([...topics, created]);
      }
      handleCloseDialog();
      onTopicsModified?.();
    } catch (err) {
      setError('Failed to save topic');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteTopic = async (id: string) => {
    try {
      setLoading(true);
      await topicService.deleteTopic(id);
      setTopics(topics.filter(t => t.id !== id));
      onTopicsModified?.();
    } catch (err) {
      setError('Failed to delete topic');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 4 }}>
        <Typography variant="h4" component="h1">
          Topic Management
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => handleOpenDialog()}
          disabled={loading}
        >
          Add Topic
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Description</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {topics.map((topic) => (
              <TableRow key={topic.id}>
                <TableCell>{topic.name}</TableCell>
                <TableCell>{topic.description}</TableCell>
                <TableCell>
                  <IconButton onClick={() => handleOpenDialog(topic)} disabled={loading}>
                    <EditIcon />
                  </IconButton>
                  <IconButton onClick={() => handleDeleteTopic(topic.id)} disabled={loading}>
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
          {editingTopic ? 'Edit Topic' : 'Add New Topic'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 2 }}>
            <TextField
              label="Name"
              value={newTopic.name}
              onChange={(e) => setNewTopic({ ...newTopic, name: e.target.value })}
              fullWidth
              disabled={loading}
            />
            <TextField
              label="Description"
              value={newTopic.description}
              onChange={(e) => setNewTopic({ ...newTopic, description: e.target.value })}
              fullWidth
              multiline
              rows={3}
              disabled={loading}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog} disabled={loading}>Cancel</Button>
          <Button onClick={handleSaveTopic} variant="contained" disabled={loading}>
            Save
          </Button>
        </DialogActions>
      </Dialog>

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
}; 
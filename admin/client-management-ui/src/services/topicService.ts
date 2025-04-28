import axios from 'axios';

const API_BASE_URL = 'http://localhost:51820/api';

export interface Topic {
  id: string;
  name: string;
  description: string;
}

export const topicService = {
  async getAllTopics(): Promise<Topic[]> {
    const response = await axios.get(`${API_BASE_URL}/topics`);
    return response.data;
  },

  async createTopic(topic: Omit<Topic, 'id'>): Promise<Topic> {
    const response = await axios.post(`${API_BASE_URL}/topics`, topic);
    return response.data;
  },

  async updateTopic(id: string, topic: Omit<Topic, 'id'>): Promise<Topic> {
    const response = await axios.put(`${API_BASE_URL}/topics/${id}`, topic);
    return response.data;
  },

  async deleteTopic(id: string): Promise<void> {
    await axios.delete(`${API_BASE_URL}/topics/${id}`);
  }
}; 
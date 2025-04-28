import axios from 'axios';

const API_BASE_URL = 'http://localhost:51820/api';

export interface Client {
  id: string;
  clientId: string;
  clientSecret: string;
  topics: string[];
}

export interface ClientCredentials {
  clientId: string;
  clientSecret: string;
}

export const clientService = {
  async getAllClients(): Promise<Client[]> {
    const response = await axios.get(`${API_BASE_URL}/clients`);
    return response.data;
  },

  async generateCredentials(): Promise<ClientCredentials> {
    const response = await axios.get(`${API_BASE_URL}/clients/generate-credentials`);
    return response.data;
  },

  async createClient(client: Omit<Client, 'id'>): Promise<Client> {
    const response = await axios.post(`${API_BASE_URL}/clients`, client);
    return response.data;
  },

  async updateClient(id: string, client: Omit<Client, 'id'>): Promise<Client> {
    const response = await axios.put(`${API_BASE_URL}/clients/${id}`, client);
    return response.data;
  },

  async deleteClient(id: string): Promise<void> {
    await axios.delete(`${API_BASE_URL}/clients/${id}`);
  }
}; 
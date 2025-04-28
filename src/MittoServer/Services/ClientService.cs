using Microsoft.EntityFrameworkCore;
using MittoServer.Data;
using MittoServer.Models;
using System.Security.Cryptography;
using System.Text;

namespace MittoServer.Services;

public interface IClientService
{
    Task<List<Client>> GetAllClientsAsync();
    Task<ClientCredentials> GenerateCredentialsAsync();
    Task<Client> CreateClientAsync(Client client);
    Task<Client> UpdateClientAsync(string id, Client client);
    Task DeleteClientAsync(string id);
}

public class ClientService : IClientService
{
    private readonly AppDbContext _dbContext;

    public ClientService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Client>> GetAllClientsAsync()
    {
        return await _dbContext.Clients.ToListAsync();
    }

    public async Task<ClientCredentials> GenerateCredentialsAsync()
    {
        var clientId = GenerateRandomString(32);
        var clientSecret = GenerateRandomString(64);

        return new ClientCredentials
        {
            ClientId = clientId,
            ClientSecret = clientSecret
        };
    }

    public async Task<Client> CreateClientAsync(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Id))
        {
			client.Id = Guid.CreateVersion7().ToString();
		}

        _dbContext.Clients.Add(client);
        await _dbContext.SaveChangesAsync();
        return client;
    }

    public async Task<Client> UpdateClientAsync(string id, Client client)
    {
        var existingClient = await _dbContext.Clients.FindAsync(id);
        if (existingClient == null)
            throw new Exception("Client not found");

        existingClient.ClientId = client.ClientId;
        existingClient.ClientSecret = client.ClientSecret;
        existingClient.Topics = client.Topics;

        await _dbContext.SaveChangesAsync();
        return existingClient;
    }

    public async Task DeleteClientAsync(string id)
    {
        var client = await _dbContext.Clients.FindAsync(id);
        if (client == null)
            throw new Exception("Client not found");

        _dbContext.Clients.Remove(client);
        await _dbContext.SaveChangesAsync();
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);
        }
        var result = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random[i] % chars.Length]);
        }
        return result.ToString();
    }
} 
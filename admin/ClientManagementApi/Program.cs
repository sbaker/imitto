using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Enable CORS
app.UseCors();

// Handle OPTIONS requests
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
        context.Response.StatusCode = 200;
        return;
    }
    await next();
});

// In-memory storage (replace with proper storage later)
var clients = new List<Client>();
var topics = new List<Topic>();

// Encryption key (in production, use proper key management)
var key = Encoding.UTF8.GetBytes("YourSecretKey123!YourSecretKey12"); // 32 bytes for AES-256

string GenerateClientId()
{
    return Guid.NewGuid().ToString("N").Substring(0, 16);
}

string GenerateClientSecret()
{
    var randomBytes = new byte[32];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(randomBytes);
    }
    return Convert.ToBase64String(randomBytes);
}

string Encrypt(string plainText)
{
    using var aes = Aes.Create();
    aes.Key = key;
    aes.GenerateIV();

    using var encryptor = aes.CreateEncryptor();
    using var msEncrypt = new MemoryStream();
    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
    using (var swEncrypt = new StreamWriter(csEncrypt))
    {
        swEncrypt.Write(plainText);
    }

    var iv = aes.IV;
    var encrypted = msEncrypt.ToArray();
    var result = new byte[iv.Length + encrypted.Length];
    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
    Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

    return Convert.ToBase64String(result);
}

string Decrypt(string cipherText)
{
    var fullCipher = Convert.FromBase64String(cipherText);
    var iv = new byte[16];
    var cipher = new byte[fullCipher.Length - 16];

    Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
    Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

    using var aes = Aes.Create();
    aes.Key = key;
    aes.IV = iv;

    using var decryptor = aes.CreateDecryptor();
    using var msDecrypt = new MemoryStream(cipher);
    using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
    using var srDecrypt = new StreamReader(csDecrypt);
    return srDecrypt.ReadToEnd();
}

// API endpoints
app.MapGet("/api/clients", () => clients);

app.MapGet("/api/clients/generate-credentials", () =>
{
    return new
    {
        ClientId = GenerateClientId(),
        ClientSecret = GenerateClientSecret()
    };
});

app.MapPost("/api/clients", (Client client) =>
{
    var newClient = new Client
    {
        Id = Guid.NewGuid().ToString(),
        ClientId = client.ClientId,
        ClientSecret = Encrypt(client.ClientSecret),
        Topics = client.Topics
    };
    clients.Add(newClient);
    return newClient;
});

app.MapPut("/api/clients/{id}", (HttpContext context) =>
{
    var id = context.Request.RouteValues["id"]?.ToString();
    if (id == null) return Results.BadRequest("Client ID is required.");

    var client = context.Request.ReadFromJsonAsync<Client>().Result;
    if (client == null) return Results.BadRequest("Invalid client data.");

    var existingClient = clients.FirstOrDefault(c => c.Id == id);
    if (existingClient == null) return Results.NotFound();

    existingClient.ClientId = client.ClientId;
    existingClient.ClientSecret = Encrypt(client.ClientSecret);
    existingClient.Topics = client.Topics;

    return Results.Ok(existingClient);
});

app.MapDelete("/api/clients/{id}", (string id) =>
{
    var client = clients.FirstOrDefault(c => c.Id == id);
    if (client == null) return Results.NotFound();

    clients.Remove(client);
    return Results.Ok();
});

// Topic management endpoints
app.MapGet("/api/topics", () => topics);

app.MapPost("/api/topics", (Topic topic) =>
{
    var newTopic = new Topic
    {
        Id = Guid.NewGuid().ToString(),
        Name = topic.Name,
        Description = topic.Description
    };
    topics.Add(newTopic);
    return newTopic;
});

app.MapPut("/api/topics/{id}", (HttpContext context) =>
{
    var id = context.Request.RouteValues["id"]?.ToString();
    if (id == null) return Results.BadRequest("Topic ID is required.");

    var topic = context.Request.ReadFromJsonAsync<Topic>().Result;
    if (topic == null) return Results.BadRequest("Invalid topic data.");

    var existingTopic = topics.FirstOrDefault(t => t.Id == id);
    if (existingTopic == null) return Results.NotFound();

    existingTopic.Name = topic.Name;
    existingTopic.Description = topic.Description;

    return Results.Ok(existingTopic);
});

app.MapDelete("/api/topics/{id}", (string id) =>
{
    var topic = topics.FirstOrDefault(t => t.Id == id);
    if (topic == null) return Results.NotFound();

    // Remove topic from all clients
    foreach (var client in clients)
    {
        client.Topics.Remove(topic.Name);
    }

    topics.Remove(topic);
    return Results.Ok();
});

app.Run();

public class Client
{
    public string Id { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public List<string> Topics { get; set; } = new();
}

public class Topic
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
} 
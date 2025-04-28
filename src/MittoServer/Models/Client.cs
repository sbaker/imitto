using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace MittoServer.Models;

public class Client
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

	public string ClientId { get; set; } = string.Empty;
    
    public string ClientSecret { get; set; } = string.Empty;

    public ICollection<Topic> Topics { get; set; } = [];
}

public class ClientCredentials
{
    public string ClientId { get; set; } = string.Empty;
    
    public string ClientSecret { get; set; } = string.Empty;
} 
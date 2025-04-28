using System.ComponentModel;
using System.Globalization;

namespace MittoServer.Models;


public class Topic
{
    public string Id { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<Client> Clients { get; set; } = [];
} 
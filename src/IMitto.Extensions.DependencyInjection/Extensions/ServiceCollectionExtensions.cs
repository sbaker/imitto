using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Channels;
using IMitto;
using IMitto.Channels;
using IMitto.Extensions.DependencyInjection;
using IMitto.Net;
using IMitto.Net.Clients;
using IMitto.Net.Models;
using IMitto.Net.Server;
using IMitto.Local;
using IMitto.Storage;
using IMitto.Settings;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static partial class ServiceCollectionExtensions { }
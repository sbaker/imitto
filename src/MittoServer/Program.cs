using Microsoft.EntityFrameworkCore;
using MittoServer.Components;
using MittoServer.Data;
using MittoServer.Models;
using MittoServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options => {
	options.UseSqlite("Data Source=./_db/mitto.db");
	
	if (builder.Environment.IsDevelopment())
	{
		options.EnableDetailedErrors();
	}
});

builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddIMittoServer();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

	if (dbContext.Database.EnsureCreated())
	{
		var topic1 = new Topic { Name = "/sensor/*/humidity", Id = Guid.CreateVersion7().ToString(), Description = "Demo topic" };
		var topic2 = new Topic { Name = "/sensor/*/temperature", Id = Guid.CreateVersion7().ToString(), Description = "Demo topic" };
		var client = new Client
		{
			Id = Guid.CreateVersion7().ToString(),
			ClientId = "client-id",
			ClientSecret = "client-secret",
		};
		client.Topics.Add(topic1);
		client.Topics.Add(topic2);
		dbContext.Add(client);
		dbContext.SaveChanges();
	}
}

app.Run();

using Microsoft.EntityFrameworkCore;
using MittoServer.Components;
using MittoServer.Data;
using MittoServer.Models;
using MittoServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

// Register AppDbContext with SQLite
builder.Services.AddDbContext<AppDbContext>(options => {
	options.UseSqlite("Data Source=./_db/mitto.db");
});

builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IClientService, ClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

	if (dbContext.Database.EnsureCreated())
	{
		var topic1 = new Topic { Name = "/sensor/*/humidity", Id = Guid.CreateVersion7().ToString(), Description = "Demo topic" };
		var topic2 = new Topic { Name = "/sensor/*/temperature", Id = Guid.CreateVersion7().ToString(), Description = "Demo topic" };
		dbContext.Add(topic1);
		dbContext.Add(topic2);
		dbContext.Add(new Client { Id = Guid.CreateVersion7().ToString(), ClientId = "client-id", ClientSecret = "client-secret", Topics = [topic2] });
		dbContext.SaveChanges();
	}
}

app.Run();

using Microsoft.EntityFrameworkCore;
using MittoServer.Data;
using MittoServer.Models;

namespace MittoServer.Services;

public interface ITopicService
{
    Task<List<Topic>> GetAllTopicsAsync();
    Task<Topic> CreateTopicAsync(Topic topic);
    Task<Topic> UpdateTopicAsync(string id, Topic topic);
    Task DeleteTopicAsync(string id);
}

public class TopicService : ITopicService
{
    private readonly AppDbContext _dbContext;

    public TopicService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Topic>> GetAllTopicsAsync()
    {
        return await _dbContext.Topics.ToListAsync();
    }

    public async Task<Topic> CreateTopicAsync(Topic topic)
    {
		if (string.IsNullOrWhiteSpace(topic.Id))
		{
			topic.Id = Guid.CreateVersion7().ToString();
		}

		_dbContext.Topics.Add(topic);
        await _dbContext.SaveChangesAsync();
        return topic;
    }

    public async Task<Topic> UpdateTopicAsync(string id, Topic topic)
    {
        var existingTopic = await _dbContext.Topics.FindAsync(id);
        if (existingTopic == null)
            throw new Exception("Topic not found");

        existingTopic.Name = topic.Name;
        existingTopic.Description = topic.Description;

        await _dbContext.SaveChangesAsync();
        return existingTopic;
    }

    public async Task DeleteTopicAsync(string id)
    {
        var topic = await _dbContext.Topics.FindAsync(id);
        if (topic == null)
            throw new Exception("Topic not found");

        _dbContext.Topics.Remove(topic);
        await _dbContext.SaveChangesAsync();
    }
} 
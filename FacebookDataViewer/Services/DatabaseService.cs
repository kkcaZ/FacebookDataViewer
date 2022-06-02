using System.Text.Json;
using FacebookDataViewer.Data.Dto;
using FacebookDataViewer.Services.Interfaces;
using FacebookDataViewer.Settings;
using MongoDB.Driver;
using MongoDB.Bson;

namespace FacebookDataViewer.Services;

public class DatabaseService : IDatabaseService
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly ILogger<DatabaseService> _logger;
    private readonly IConfiguration _configuration;
    
    JsonSerializerOptions jsonCaseOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    
    public DatabaseService(ILogger<DatabaseService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        var section = _configuration.GetSection(nameof(MongoSettings));
        var mongoConfig = section.Get<MongoSettings>();

        _logger.LogInformation($"Connecting to database - {mongoConfig.ConnectionString}");
        _client = new MongoClient(
            mongoConfig.ConnectionString
        );

        _database = _client.GetDatabase("facebook-data");
    }
    
    /// <inheritdoc />
    public void UploadImage(string uri, string base64)
    {
        var collection = _database.GetCollection<BsonDocument>("Images");

        var document = new BsonDocument()
        {
            { "uri", uri },
            { "content", base64 }
        };
        
        collection.InsertOne(document);
    }
    
    /// <inheritdoc />
    public void UploadMessages(string title, ConversationDto conversation) {
        var collection = _database.GetCollection<BsonDocument>("Messages");

        collection.InsertOne(conversation.ToBsonDocument());
    }

    /// <inheritdoc />
    public bool TryGetImage(string uri, out string base64)
    {
        var collection = _database.GetCollection<BsonDocument>("Images");
        var filter = Builders<BsonDocument>.Filter.Eq("uri", uri);
        var documents = collection.Find(filter).ToList();

        if (documents.Count > 0)
        {
            base64 = documents.First().GetValue("content").ToString();
            return true;
        }
        else
        {
            base64 = null;
            return false;
        }
    }
    
    /// <inheritdoc />
    public bool TryGetMessages(string title, out ConversationDto? messages)
    {
        var collection = _database.GetCollection<BsonDocument>("Messages");
        var filter = Builders<BsonDocument>.Filter.Eq("Title", title);
        var documents = collection.Find(filter).ToList();

        if (documents.Count > 0)
        {
            messages = JsonSerializer.Deserialize<ConversationDto>(JsonSerializer.Serialize(BsonTypeMapper.MapToDotNetValue(documents.First()), jsonCaseOptions));
            return true;
        }
        else
        {
            messages = null;
            return false;
        }
    }

    /// <inheritdoc />
    public List<string> GetAllConversations()
    {
        var collection = _database.GetCollection<BsonDocument>("Messages");
        var documents = collection.Find(_ => true).ToList();

        List<string> chats = new List<string>();
        
        foreach (var document in documents)
        {
            var chat = JsonSerializer.Deserialize<ConversationDto>(JsonSerializer.Serialize(BsonTypeMapper.MapToDotNetValue(document), jsonCaseOptions));
            chats.Add(chat.Title);
        }

        return chats;
    }
}
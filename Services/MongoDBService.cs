using ApiWikiTech.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Data;

namespace ApiWikiTech.Services;

public class MongoDBService
{

    private readonly IMongoCollection<User> _userCollection;

    public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _userCollection = database.GetCollection<User>(mongoDBSettings.Value.CollectionName);
    }

    public async Task<List<User>> GetAsyncUsers()
    {
        return await _userCollection.Find(_ => true).ToListAsync();
    }
    public async Task AddUserAsync(User user, string role)
    {
        user.Role = role;
        await _userCollection.InsertOneAsync(user);
        return;
    }

    public async Task<User> GetAsyncOneUser(string email)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Email, email);
        User user = _userCollection.Find(filter).FirstOrDefault();
        return user;
    }
    public async Task<User> GetUserByIdAsync(string id)
    {
        var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(id));
        return await _userCollection.Find(filter).FirstOrDefaultAsync();
    }

    /*
        
        public async Task CreateAsync(User playlist) { }
        public async Task AddToPlaylistAsync(string id, string movieId) { }
        public async Task DeleteAsync(string id) { }
    */

}
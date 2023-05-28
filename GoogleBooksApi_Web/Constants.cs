using MongoDB.Bson;
using MongoDB.Driver;

namespace GoogleBooksApi_Web
{
    public class Constants
    {
        public static string host = "xxxxx";
        public static MongoClient mongoClient;
        public static IMongoDatabase database;
        public static IMongoCollection<BsonDocument> collection;
        
    }
}

namespace LucaHome.Models
{
    public class MongoSettings
    {
        public string ConnectionStringMongo { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string CommentCollectionName { get; set; } = null!;
        public string UserCollectionName { get; set; } = null!;
        public string ProjectCollectionName { get; set; } = null!;
        public string SkillCollectionName { get; set; } = null!;
        public string DbProvider { get; set; } = "MongoDB";
    }
}

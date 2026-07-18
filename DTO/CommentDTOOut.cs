namespace LucaHome.DTO
{
    public class CommentDTOOut
    {
        public required string Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime Datetime { get; set; }
    }
}

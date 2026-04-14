using System.ComponentModel.DataAnnotations;

namespace LucaHome.DTO
{
    public class CommentDTOIn
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Content { get; set; } = null!;
        public string? Middlename { get; set; }
    }
}

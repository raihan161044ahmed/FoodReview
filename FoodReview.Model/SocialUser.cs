namespace FoodReview.Model
{
    public class SocialUser
    {
        public int Id { get; set; }
        public string Provider { get; set; } 
        public string ProviderUserId { get; set; } 
        public string Email { get; set; }
        public string? Name { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key to your main User table
        public int UserId { get; set; }
        public User User { get; set; }
    }

}

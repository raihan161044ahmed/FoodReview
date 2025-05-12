using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FoodReview.Model
{
    public class Review
    {
        public int Id { get; set; }
        [JsonIgnore] 
        public int UserId { get; set; }
        public int LocationId { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } 
        public User User { get; set; }  
        public Location Location { get; set; }
    }
}

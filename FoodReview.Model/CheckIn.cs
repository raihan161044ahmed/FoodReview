using System.Text.Json.Serialization;

namespace FoodReview.Model
{
    public class CheckIn
    {
        public int Id { get; set; }
        [JsonIgnore] 
        public int UserId { get; set; }
        public int LocationId { get; set; }
        public DateTime CheckInTime { get; set; }
        public User User { get; set; }
        public Location Location { get; set; }
    }
}

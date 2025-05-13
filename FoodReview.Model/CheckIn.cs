namespace FoodReview.Model
{
    public class CheckIn
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LocationId { get; set; }
        public DateTime CheckInTime { get; set; }
    }
}

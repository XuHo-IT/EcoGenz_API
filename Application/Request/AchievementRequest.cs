namespace Application.Request
{
    public class AchievementRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string Description { get; set; }
        public DateTime? AchievedAt { get; set; }

        public Guid? UserId { get; set; }
    }
}

namespace backend.Core.Models
{
    public abstract class BaseModel
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAd { get; set; } = DateTime.Now;
    }
}

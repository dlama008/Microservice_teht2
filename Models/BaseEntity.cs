namespace Microservice_teht2.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}

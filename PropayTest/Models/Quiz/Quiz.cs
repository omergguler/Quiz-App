namespace PropayTest.Models.Quiz
{
    public class Quiz
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }

        public int CreatorId { get; set; }
    }
}

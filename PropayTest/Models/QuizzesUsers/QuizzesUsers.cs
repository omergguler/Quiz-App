namespace PropayTest.Models.QuizzesUsers
{
    public class QuizUser
    {
        public int QuizId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatorId { get; set; }

        public string FullName { get; set; }

        public int UserId { get; set; }
    }
}

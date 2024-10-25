namespace PropayTest.Models.PopularQuiz
{
    public class PopularQuiz
    {
        public int QuizId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Occurence { get; set; }

        public string CreatorUserName { get; set; }
    }
}

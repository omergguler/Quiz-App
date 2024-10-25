namespace PropayTest.Models.SolvedQuiz
{
    public class SolvedQuiz
    {
        public int ResultId { get; set; }

        public int QuizId { get; set; }

        public int UserId { get; set; }

        public string CreatorUserName { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int CreatorId { get; set; }

        public string Score { get; set; }

        public DateTime CompletedDate { get; set; }

        public int HowManyCorrect { get; set; }
        public int HowManyWrong { get; set; }

        public string Solved { get; set; }

    }
}

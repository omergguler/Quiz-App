using System.ComponentModel.DataAnnotations;

namespace PropayTest.Models.Question
{
    public class Question
    {
        public int QuestionId { get; set; }

        public string QuestionText { get; set; }

        public string Choice1 { get; set; }
   
        public string Choice2 { get; set; }

        public string Choice3 { get; set; }

  
        public string Choice4 { get; set; }

        public string CorrectChoice { get; set; }

        public int CreatorId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int HowManyAnsweredWrong { get; set; }

        public int HowManyAnsweredCorrect { get; set; }

        public string Category { get; set; }
    }
}

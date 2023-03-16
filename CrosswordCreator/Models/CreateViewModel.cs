using CrosswordCreator.Data.Entities;

namespace CrosswordCreator.Models
{
    public class CreateViewModel
    {
        public char[][] Crossword { get; set; }

        public List<KeyValuePair<Answer, Question>> FittedWords { get; set; }

        public List<QuestionModel> NotFittedWords { get; set; }
    }
}

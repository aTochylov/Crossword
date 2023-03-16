using CrosswordCreator.Data.Entities;

namespace CrosswordCreator.Models
{
    public class CheckViewModel
    {
        public List<KeyValuePair<Answer, Question>> Words { get; set; }
        public char[][] SolvedCrossword { get; set; }
        public char[][] CorrectCrossword { get; set; }
    }
}

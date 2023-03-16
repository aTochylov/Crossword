using CrosswordCreator.Data.Entities;

namespace CrosswordCreator.Models
{
    public class SolveCrosswordViewModel
    {
        public string Code { get; set; }
        public char[][] Crossword { get; set; }

        public List<KeyValuePair<Answer, Question>> Words { get; set; }
    }
}

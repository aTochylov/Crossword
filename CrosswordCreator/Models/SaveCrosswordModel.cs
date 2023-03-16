using System.ComponentModel.DataAnnotations;

namespace CrosswordCreator.Models
{
    public class SaveCrosswordModel
    {
        [Required]
        public List<QuestionModel> Questions { get; set; }
        [Required]
        public char[][] Crossword { get; set; }
    }
}

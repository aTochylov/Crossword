using System.ComponentModel.DataAnnotations;

namespace CrosswordCreator.Models
{
    public class QuestionModel
    {
        [Required]
        public string Question { get; set; } = string.Empty;
        [Required]
        public string Answer { get; set; }
    }
}

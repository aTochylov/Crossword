using CrosswordCreator.Data.Entities;
using CrosswordCreator.Models;

namespace CrosswordCreator.Services
{
    public interface ICreateCrosswordService
    {
        public Dictionary<Answer, Question> FittedWords{ get; set; }

        public List<QuestionModel> NotFittedWords { get; }

        public void AddList(IEnumerable<QuestionModel> list);

        public char[][] CrosswordArray { get; set; }
    }
}

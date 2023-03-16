using CrosswordCreator.Data.Entities;
using CrosswordCreator.Models;

namespace CrosswordCreator.Services
{
    public interface IDbCrosswordService
    {
        public Task AddCrossword(Dictionary<Answer, Question> answersQuestions, char[][] crossword, string code);
        public Crossword GetCrosswordModel(string code);
    }
}

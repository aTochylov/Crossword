using CrosswordBuilder;
using CrosswordCreator.Data.Entities;
using CrosswordCreator.Models;

namespace CrosswordCreator.Services
{
    public class CreateCrosswordService : ICreateCrosswordService
    {
        private readonly Builder builder;

        private Dictionary<Answer, Question> _fitted { get; set; } = new();

        public Dictionary<Answer, Question> FittedWords
        {
            get {
                SetStartIdxs();
                return _fitted;
            }
            set
            {
                _fitted = value;
            }

        }
        public List<QuestionModel> NotFittedWords { get; private set; } = new();
        public char[][] CrosswordArray {get => builder.Crossword; set => builder.Crossword = value; }

        public CreateCrosswordService()
        {
            builder = new();
        }

        public void AddList(IEnumerable<QuestionModel> answersQuestions)
        {
            Random r = new();
            for(byte i = 0; i < 4; i++)
                foreach (var item in answersQuestions.OrderBy(x=>r.Next()))  
                    TryAddWord(item.Answer, item.Question);            
            
            NotFittedWords = answersQuestions.Where(i => !_fitted.Keys.ToList()
            .Any(a => a.Word == i.Answer.Trim().ToUpper()))
                .Select(p => new QuestionModel { Question = p.Question, Answer = p.Answer }).ToList();
        }

        private void SetStartIdxs()
        {
            _fitted.ToList().ForEach(p =>
            {
                var wordStart = builder.GetStartIdxs(p.Key.Word);
                if(wordStart == (-1, -1))
                {
                    NotFittedWords.Add(new()
                    {
                        Answer = p.Key.Word,
                        Question = _fitted[p.Key].Text
                    });
                    _fitted.Remove(p.Key);
                }
                p.Key.WordStartCol = wordStart.Item1;
                p.Key.WordStartRow = wordStart.Item2;
            });
        }

        private bool TryAddWord(string word, string question)
        {
            word = word.Trim().ToUpper();
            if (_fitted.Keys.Any(k => k.Word.Equals(word)))
                return false;

            if (!builder.AddNewWord(word))
                return false;

            _fitted.TryAdd(new Answer { Word = word },
                new Question { Text = question.Trim() });
            return true;
        }
    }
}

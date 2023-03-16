using CrosswordCreator.Data;
using CrosswordCreator.Data.Entities;
using CrosswordCreator.Models;
using Microsoft.EntityFrameworkCore;

namespace CrosswordCreator.Services
{
    public class DbCrosswordService : IDbCrosswordService
    {
        private readonly CrosswordDbContext context;

        public DbCrosswordService(CrosswordDbContext context)
        {
            this.context = context;
        }

        public async Task AddCrossword(Dictionary<Answer, Question> answersQuestions, char[][] crossword, string code)
        {
            List<Row> rows = new List<Row>();
            //for (int i = 0; i < crossword.GetLength(0); i++)
            //    rows.Add(new string(crossword[i]));
            foreach (var r in crossword)
                rows.Add(new() { Chars = new string(r) });

            Crossword c = new()
            {
                Code = code,
                Rows = rows,
                Questions = new List<Question>(answersQuestions.Select(aq =>
                    new Question
                    {
                        Text = aq.Value.Text,
                        Answer = aq.Key
                    }))
            };

            await context.Crosswords.AddAsync(c);
            await context.SaveChangesAsync();
        }

        public Crossword GetCrosswordModel(string code)
        {
            return context.Crosswords.Include(c => c.Questions)
                .ThenInclude(q => q.Answer)
                .Include(c => c.Rows)
                .FirstOrDefault(c => c.Code == code);
        }
    }
}

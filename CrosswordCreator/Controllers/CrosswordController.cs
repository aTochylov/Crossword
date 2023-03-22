using CrosswordCreator.Data.Entities;
using CrosswordCreator.Models;
using CrosswordCreator.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CrosswordCreator.Controllers
{
    public class CrosswordController : Controller
    {
        private readonly ILogger<CrosswordController> _logger;
        private readonly ICreateCrosswordService crosswordService;
        private readonly IDbCrosswordService dbService;

        public CrosswordController(ILogger<CrosswordController> logger, ICreateCrosswordService crossword, IDbCrosswordService dbService)
        {
            _logger = logger;
            crosswordService = crossword;
            this.dbService = dbService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create(List<QuestionModel> questions)             
        {
            if (!ModelState.IsValid) return BadRequest("Incorrect input");

            crosswordService.AddList(questions);
            CreateViewModel vm = new()
            {
                NotFittedWords = crosswordService.NotFittedWords,
                FittedWords = crosswordService.FittedWords.ToList(),
                Crossword = crosswordService.CrosswordArray
            };
            return PartialView(vm);
        }

        public async Task<IActionResult> Save(SaveCrosswordModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Incorrect input");

            crosswordService.CrosswordArray = model.Crossword;
            crosswordService.FittedWords = model.Questions.Select(aq =>
                new KeyValuePair<Answer, Question>(new() { Word = aq.Answer }, new() { Text = aq.Question }))
                .ToDictionary(x => x.Key, x => x.Value);

            string code = Guid.NewGuid().ToString("N");
            await dbService.AddCrossword(crosswordService.FittedWords, crosswordService.CrosswordArray, code);

            return Ok(code);
        }

        [Route("crossword/solve/{code:length(32)}")]
        public IActionResult Solve(string code)
        {
            var crossword = dbService.GetCrosswordModel(code);
            if(crossword is null) 
                return NotFound($"No crossword with {code} code");

            SolveCrosswordViewModel vm = new()
            {
                Code = code,
                Crossword = crossword.Rows.Select(r => r.Chars.ToCharArray()).ToArray(),
                Words = crossword.Questions.Select(q => new KeyValuePair<Answer, Question>(q.Answer, q)).ToList()
            };
            return View(vm);
        }

        [Route("crossword/check/{code:length(32)}")]
        public IActionResult Check(string code, char[][] crossword)
        {
            var savedCrossw = dbService.GetCrosswordModel(code);
            if (savedCrossw is null)
                return BadRequest("Incorrect crossword code");

            CheckViewModel vm = new()
            {
                CorrectCrossword = savedCrossw.Rows.Select(x => x.Chars.ToUpper().ToArray()).ToArray(),
                SolvedCrossword = crossword.Select(x => new string(x).ToUpper().ToCharArray()).ToArray(),
                Words = savedCrossw.Questions.Select(x => new KeyValuePair<Answer, Question>(x.Answer, x)).ToList()
            };
            return PartialView(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
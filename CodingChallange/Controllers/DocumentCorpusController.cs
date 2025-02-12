using CodingChallange.Helpers;
using CodingChallenge.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodingChallenge.Controllers;

public class DocumentCorpusController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> ExecuteProcess(InputViewModel input)
    {
        try
        {
            await FilesProcessor.ProcessWordsAsync(input.Path);
        
            return View("Index", input);    
        }
        catch (DirectoryNotFoundException)
        {
            ModelState.AddModelError("Path", "The directory does not exist.");
            return View("Index", input);
        }
    }
    
    [HttpPost]
    public IActionResult ClearDatabase()
    {
        FilesProcessor.ClearDatabase();
        
        return View("Index", new InputViewModel());
    }
    
    [HttpGet("general/{word}")]
    public IActionResult GetGeneralWordAppearancesJson(string word)
    {
        var result = new GeneralCounterResultViewModel
        {
            WordCount = FilesProcessor.GetGeneralWordCounter(word.ToUpper())
        };
        return Json(result);
    }

    [HttpGet("specific/{word}")]
    public IActionResult GetSpecificWordAppearancesJson(string word)
    {
        var results = FilesProcessor.GetSpecificWordCounter(word.ToUpper());
        return Json(results);
    }
}
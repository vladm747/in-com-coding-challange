using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using CodingChallenge.Models;

namespace CodingChallange.Helpers;

public static class FilesProcessor
{
    static readonly HashSet<string> StopWords = new()
    {
        "or", "in", "a", "the", "an", "and", "of", "to", "is", "it", "on", "at", "for", "with", "as", "by", "this", "that"
    };
    
    static Hashtable _database = new Hashtable();
    
    public static async Task ProcessWordsAsync(string path)
    {
        var filePaths =  GetFilesPathsFromFolder(path);
        
        foreach (var filePath in filePaths)
        {
            var words = new List<string>();
            
            words = await ParseFile(filePath);
            
            if(!_database.ContainsKey(filePath))
            {
                _database.Add(filePath, new Hashtable());
            }
            
            if(_database.ContainsKey(filePath))
            {
                foreach (var word in words)
                {
                    var normalisedWord = word.ToUpper();

                    if (_database[filePath] is Hashtable innerTable)
                    {
                        if (innerTable.ContainsKey(normalisedWord))
                        {
                            innerTable[normalisedWord] = (int)innerTable[normalisedWord] + 1;
                        }
                        else
                        {
                            innerTable.Add(normalisedWord, 1);
                        }
                    }
                }
            }
        }
    }
    
    public static int GetGeneralWordCounter(string word)
    {
        int count = 0;
        
        foreach (var innerTable in _database.Values)
        {
            if (innerTable is Hashtable table)
            {
                if (table.ContainsKey(word))
                {
                    count += (int)table[word];
                }
                else
                {
                    continue;
                }
            }
        }
        
        return count;
    }
    
    public static List<SpecificCounterResultViewModel> GetSpecificWordCounter(string word)
    {
        var result = new List<SpecificCounterResultViewModel>();
        
        var keys = _database.Keys.Cast<string>().ToArray();

        for (int index = 0; index < _database.Count; index++)
        {
            var keyFilePath = keys[index];
            var innerTable = _database[keyFilePath];
            
            if (innerTable is Hashtable table)
            {
                if (table.ContainsKey(word))
                {
                    result.Add(new SpecificCounterResultViewModel
                    {
                        FileName = keyFilePath.Split('\\').Last(),
                        WordCount = (int)table[word]
                    });
                }
                else
                {
                    continue;
                }
            }
        }
        
        return result;
    }

    public static void ClearDatabase()
    {
        _database.Clear();
    }
    
    static async Task<List<string>> ParseFile(string filePath)
    {   
        List<string> words;
        
        if (filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            words = ParseTxtFile(filePath);
        }
        else
        {
            words = await ParseHtmlFile(filePath);
        }
        
        return words;
    }
    
    static List<string> GetFilesPathsFromFolder(string path)
    {
        if (Directory.Exists(path))
        {
            return Directory.GetFiles(path).Where(file => 
                file.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                file.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        else
        {
            throw new DirectoryNotFoundException("The directory does not exist.");
        }
    }
    
    static List<string> ParseTxtFile(string filePath)
    {
        var wordsList = new List<string>();

        foreach (var line in File.ReadLines(filePath))
        {
            string cleanedLine = Regex.Replace(line, "[^a-zA-Z ]", "");

            var words = cleanedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            wordsList.AddRange(words.Where(word => word.Length > 2 && !StopWords.Contains(word.ToLower())));
        }

        return wordsList;
    }
    
    static async Task<List<string>> ParseHtmlFile(string htmlContent)
    {
        var parser = new HtmlParser();
        var document = await parser.ParseDocumentAsync(htmlContent);

        string text = ExtractTextFromHtml(document);

        string cleanedText = Regex.Replace(text, "[^a-zA-Z ]", "");

        var words = cleanedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        return words.Where(word => word.Length > 2 && !StopWords.Contains(word.ToLower())).ToList();
    }

    static string ExtractTextFromHtml(AngleSharp.Dom.IDocument document)
    {
        foreach (var element in document.QuerySelectorAll("script, style"))
        {
            element.Remove();
        }

        return document.Body.TextContent;
    }
}
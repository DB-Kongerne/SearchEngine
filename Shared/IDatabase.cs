using System.Collections.Generic;
using Shared.Model;

namespace Shared
{
    public interface IDatabase
    {
        // Search
        List<int> GetWordIds(string[] query, out List<string> outIgnored);
        BEDocument GetDocDetails(int docId);
        List<KeyValuePair<int, int>> GetDocuments(List<int> wordIds);
        List<int> getMissing(int docId, List<int> wordIds);
        List<string> WordsFromIds(List<int> wordIds);

        // Indexing
        Dictionary<string, int> GetAllWords();
        int DocumentCounts { get; }
        void InsertDocument(BEDocument doc);
        void InsertWord(int id, string value);
        void InsertAllWords(Dictionary<string, int> words);
        void InsertAllOcc(int docId, ISet<int> wordIds);
    }
}

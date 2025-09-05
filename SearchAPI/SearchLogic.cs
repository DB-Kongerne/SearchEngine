using System;
using System.Collections.Generic;
using Shared;
using Shared.Model;

namespace SearchAPI
{
    public class SearchLogic
    {
        private readonly Shared.IDatabase mDatabase;
        private bool mCaseSensitive = true;

        public SearchLogic(Shared.IDatabase database)
        {
            mDatabase = database;
        }

        public void SetCaseSensitivity(bool enabled)
        {
            mCaseSensitive = enabled;
            Console.WriteLine("Case sensitivity is now " + (enabled ? "ON" : "OFF"));
        }

        public SearchResult Search(string[] query, int maxAmount)
        {
            List<string> ignored;
            DateTime start = DateTime.Now;

            if (!mCaseSensitive)
            {
                for (int i = 0; i < query.Length; i++)
                {
                    query[i] = query[i].ToLowerInvariant();
                }
            }

            var wordIds = mDatabase.GetWordIds(query, out ignored);

            if (wordIds.Count == 0)
            {
                return new SearchResult(query, 0, new List<DocumentHit>(), ignored, DateTime.Now - start);
            }

            var docIds = mDatabase.GetDocuments(wordIds);

            var top = new List<int>();
            foreach (var p in docIds.GetRange(0, Math.Min(maxAmount, docIds.Count)))
                top.Add(p.Key);

            List<DocumentHit> docresult = new List<DocumentHit>();
            int idx = 0;
            foreach (var docId in top)
            {
                BEDocument doc = mDatabase.GetDocDetails(docId);
                var missing = mDatabase.WordsFromIds(mDatabase.getMissing(doc.mId, wordIds));
                missing.AddRange(ignored);
                docresult.Add(new DocumentHit(doc, docIds[idx++].Value, missing));
            }

            return new SearchResult(query, docIds.Count, docresult, ignored, DateTime.Now - start);
        }
    }
}

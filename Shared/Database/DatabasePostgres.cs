using System;
using System.Collections.Generic;
using Shared.Model;
using Npgsql;

namespace Shared.Database
{
    public class DatabasePostgres : IDatabase
    {
        protected NpgsqlConnection _connection;
        protected Dictionary<string, int> mWords = null;

        public DatabasePostgres()
        {
            _connection = new NpgsqlConnection(Paths.POSTGRES_DATABASE);
            _connection.Open();
        }

        protected void Execute(string sql)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        // key is the id of the document, the value is number of search words in the document
        public List<KeyValuePair<int, int>> GetDocuments(List<int> wordIds)
        {
            var res = new List<KeyValuePair<int, int>>();

            var sql = "SELECT docId, COUNT(wordId) as count FROM Occ where ";
            sql += "wordId in " + AsString(wordIds) + " GROUP BY docId ";
            sql += "ORDER BY count DESC;";

            var selectCmd = _connection.CreateCommand();
            selectCmd.CommandText = sql;

            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var docId = reader.GetInt32(0);
                    var count = reader.GetInt32(1);
                    res.Add(new KeyValuePair<int, int>(docId, count));
                }
            }

            return res;
        }

        protected string AsString(List<int> x) => $"({string.Join(',', x)})";

        public Dictionary<string, int> GetAllWords()
        {
            Dictionary<string, int> res = new Dictionary<string, int>();

            var selectCmd = _connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM word";

            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var w = reader.GetString(1);
                    res.Add(w, id);
                }
            }
            return res;
        }

        public BEDocument GetDocDetails(int docId)
        {
            var selectCmd = _connection.CreateCommand();
            selectCmd.CommandText = $"SELECT * FROM document where id = {docId}";

            using (var reader = selectCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var url = reader.GetString(1);
                    var idxTime = reader.GetString(2);
                    var creationTime = reader.GetString(3);

                    return new BEDocument { mId = id, mUrl = url, mIdxTime = idxTime, mCreationTime = creationTime };
                }
            }
            return null;
        }

        public List<int> getMissing(int docId, List<int> wordIds)
        {
            var sql = "SELECT wordId FROM Occ where ";
            sql += "wordId in " + AsString(wordIds) + " AND docId = " + docId;
            sql += " ORDER BY wordId;";

            var selectCmd = _connection.CreateCommand();
            selectCmd.CommandText = sql;

            List<int> present = new List<int>();

            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var wordId = reader.GetInt32(0);
                    present.Add(wordId);
                }
            }
            var result = new List<int>(wordIds);
            foreach (var w in present)
                result.Remove(w);

            return result;
        }

        public List<string> WordsFromIds(List<int> wordIds)
        {
            List<string> result = new List<string>();

            if (wordIds.Count == 0)
                return result;

            var sql = "SELECT name FROM Word where ";
            sql += "id in " + AsString(wordIds);

            var selectCmd = _connection.CreateCommand();
            selectCmd.CommandText = sql;

            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var wordName = reader.GetString(0);
                    result.Add(wordName);
                }
            }
            return result;
        }

        public List<int> GetWordIds(string[] query, out List<string> outIgnored)
        {
            if (mWords == null)
                mWords = GetAllWords();
            var res = new List<int>();
            var ignored = new List<string>();

            foreach (var aWord in query)
            {
                if (mWords.ContainsKey(aWord))
                    res.Add(mWords[aWord]);
                else
                    ignored.Add(aWord);
            }
            outIgnored = ignored;
            return res;
        }

        // Default implementations for indexing methods
        public virtual int DocumentCounts => throw new NotImplementedException();
        public virtual void InsertDocument(BEDocument doc) { throw new NotImplementedException(); }
        public virtual void InsertWord(int id, string value) { throw new NotImplementedException(); }
        public virtual void InsertAllWords(Dictionary<string, int> words) { throw new NotImplementedException(); }
        public virtual void InsertAllOcc(int docId, ISet<int> wordIds) { throw new NotImplementedException(); }
    }
}

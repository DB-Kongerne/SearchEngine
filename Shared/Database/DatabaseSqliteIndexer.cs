using System;
using System.Collections.Generic;
using Shared.Model;
using Microsoft.Data.Sqlite;

namespace Shared.Database
{
    public class DatabaseSqliteIndexer : DatabaseSqlite
    {
        public DatabaseSqliteIndexer() : base()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.Mode = SqliteOpenMode.ReadWriteCreate;
            connectionStringBuilder.DataSource = Paths.SQLITE_DATABASE;

            _connection = new SqliteConnection(connectionStringBuilder.ConnectionString);
            _connection.Open();

            // Create tables for indexing
            Execute("DROP TABLE IF EXISTS Occ");
            Execute("DROP TABLE IF EXISTS document");
            Execute("CREATE TABLE document(id INTEGER PRIMARY KEY, url TEXT, idxTime TEXT, creationTime TEXT)");
            Execute("DROP TABLE IF EXISTS word");
            Execute("CREATE TABLE word(id INTEGER PRIMARY KEY, name VARCHAR(50))");
            Execute("CREATE TABLE Occ(wordId INTEGER, docId INTEGER, "
                    + "FOREIGN KEY (wordId) REFERENCES word(id), "
                    + "FOREIGN KEY (docId) REFERENCES document(id))");
            Execute("CREATE INDEX word_index ON Occ (wordId)");
        }

        public override void InsertAllWords(Dictionary<string, int> res)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var command = _connection.CreateCommand();
                command.CommandText =
                    @"INSERT INTO word(id, name) VALUES(@id,@name)";

                var paramName = command.CreateParameter();
                paramName.ParameterName = "name";
                command.Parameters.Add(paramName);

                var paramId = command.CreateParameter();
                paramId.ParameterName = "id";
                command.Parameters.Add(paramId);

                // Insert all entries in the res
                foreach (var p in res)
                {
                    paramName.Value = p.Key;
                    paramId.Value = p.Value;
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        public override void InsertAllOcc(int docId, ISet<int> wordIds)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var command = _connection.CreateCommand();
                command.CommandText =
                    @"INSERT INTO occ(wordId, docId) VALUES(@wordId,@docId)";

                var paramwordId = command.CreateParameter();
                paramwordId.ParameterName = "wordId";
                command.Parameters.Add(paramwordId);

                var paramDocId = command.CreateParameter();
                paramDocId.ParameterName = "docId";
                paramDocId.Value = docId;
                command.Parameters.Add(paramDocId);

                foreach (var p in wordIds)
                {
                    paramwordId.Value = p;
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        public override void InsertWord(int id, string value)
        {
            var insertCmd = new SqliteCommand("INSERT INTO word(id, name) VALUES(@id,@name)");
            insertCmd.Connection = _connection;

            var pName = new SqliteParameter("name", value);
            insertCmd.Parameters.Add(pName);

            var pCount = new SqliteParameter("id", id);
            insertCmd.Parameters.Add(pCount);

            insertCmd.ExecuteNonQuery();
        }

        public override void InsertDocument(BEDocument doc)
        {
            var insertCmd =
                new SqliteCommand(
                    "INSERT INTO document(id, url, idxTime, creationTime) VALUES(@id,@url, @idxTime, @creationTime)");
            insertCmd.Connection = _connection;

            var pId = new SqliteParameter("id", doc.mId);
            insertCmd.Parameters.Add(pId);

            var pUrl = new SqliteParameter("url", doc.mUrl);
            insertCmd.Parameters.Add(pUrl);

            var pIdxTime = new SqliteParameter("idxTime", doc.mIdxTime);
            insertCmd.Parameters.Add(pIdxTime);

            var pCreationTime = new SqliteParameter("creationTime", doc.mCreationTime);
            insertCmd.Parameters.Add(pCreationTime);

            insertCmd.ExecuteNonQuery();
        }

        public override int DocumentCounts
        {
            get
            {
                var selectCmd = _connection.CreateCommand();
                selectCmd.CommandText = "SELECT count(*) FROM document";

                using (var reader = selectCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var count = reader.GetInt32(0);
                        return count;
                    }
                }

                return -1;
            }
        }
    }
}

using System.IO;
using Microsoft.Data.Sqlite;

namespace Sample.LogicMine.Common
{
  /// <summary>
  /// This is just a utility class which sets up the SQLite database we'll be using
  /// </summary>
  public class DbUtils
  {
    private static readonly string DbFile = $@"{Path.GetTempPath()}\frog.db";
    public static readonly string ConnectionString = $"Data Source={DbFile}";
    
    public static void CreateDb()
    {
      const string sql = @"
CREATE TABLE Pond (
  Id            INTEGER     NOT NULL  PRIMARY KEY,
  Name          TEXT        NOT NULL
);

CREATE TABLE Frog (
  Id            INTEGER     NOT NULL  PRIMARY KEY,
  LivesInPondId INTEGER     NOT NULL  REFERENCES Pond(Id),
  MotherId      INTEGER     NULL      REFERENCES Frog(Id),
  FatherId      INTEGER     NULL      REFERENCES Frog(Id),
  IsMale        BOOLEAN     NOT NULL,
  Name          TEXT        NOT NULL,
  DateOfBirth   TEXT        NOT NULL,
  DateLastMated TEXT        NOT NULL
);

CREATE TABLE Tadpole (
  Id            INTEGER     NOT NULL  PRIMARY KEY,
  LivesInPondId INTEGER     NOT NULL  REFERENCES Pond(Id),
  MotherId      INTEGER     NULL      REFERENCES Frog(Id),
  FatherId      INTEGER     NULL      REFERENCES Frog(Id),
  IsMale        BOOLEAN     NOT NULL,
  Name          TEXT        NOT NULL,
  DateOfBirth   TEXT        NOT NULL
);";

      DeleteDb();
      using (var conn = new SqliteConnection(ConnectionString))
      {
        conn.Open();
        using (var cmd = new SqliteCommand(sql, conn))
        {
          cmd.ExecuteNonQuery();
        }
      }
    }

    public static void DeleteDb()
    {
      if (File.Exists(DbFile))
        File.Delete(DbFile);
    }
  }
}

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Dnx.Runtime;

namespace SocksDrawer.Tests
{
    public class LocalDb : IDisposable
    {
        // with thanks https://github.com/khalidabuhakmeh/DatabaseHelpersExample

        public static string DatabaseDirectory = "Data";
        public static string LocalDbConnection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";

        public string ConnectionStringName { get; private set; }
        public string DatabaseName { get; private set; }
        public string OutputFolder { get; private set; }
        public string DatabaseMdfPath { get; private set; }
        public string DatabaseLogPath { get; private set; }

        public LocalDb(string databaseName = null)
        {
            DatabaseName = string.IsNullOrWhiteSpace(databaseName) ? Guid.NewGuid().ToString("N") : databaseName;
            // if numbers at start of name, trim those out
            DatabaseName = Regex.Replace(DatabaseName, @"^\d+", string.Empty);
            CreateDatabase();
        }

        public IDbConnection OpenConnection()
        {
            return new SqlConnection(ConnectionStringName);
        }

        private void CreateDatabase()
        {
            //var localPath = Assembly.GetExecutingAssembly().Location;
            var localPath = ProjectRootResolver.ResolveRootDirectory(Directory.GetCurrentDirectory());
            OutputFolder = Path.Combine(Path.GetDirectoryName(localPath), DatabaseDirectory);
            var mdfFilename = string.Format("{0}.mdf", DatabaseName);
            DatabaseMdfPath = Path.Combine(OutputFolder, mdfFilename);
            DatabaseLogPath = Path.Combine(OutputFolder, String.Format("{0}_log.ldf", DatabaseName));

            // Create Data Directory If It Doesn't Already Exist.
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }

            // If the database does not already exist, create it.
            using (var connection = new SqlConnection(LocalDbConnection))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                //DetachDatabase();
                cmd.CommandText = String.Format("CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", DatabaseName, DatabaseMdfPath);
                cmd.ExecuteNonQuery();
            }

            // Open newly created, or old database.
            // todo: use the template LocalDbConnection so this doesn't get screwed when someone changes one but not the other
            ConnectionStringName = String.Format(@"Data Source=(localdb)\MSSQLLocalDB;AttachDBFileName={1};Initial Catalog={0};Integrated Security=True;", DatabaseName, DatabaseMdfPath);
        }

        void DetachDatabase()
        {
            using (var connection = new SqlConnection(LocalDbConnection))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = string.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; exec sp_detach_db '{0}'", DatabaseName);
                cmd.ExecuteNonQuery();
            }
            if (File.Exists(DatabaseMdfPath)) File.Delete(DatabaseMdfPath);
            if (File.Exists(DatabaseLogPath)) File.Delete(DatabaseLogPath);
        }

        // todo: use proper IDisposable impl
        public void Dispose()
        {
            DetachDatabase();
        }
    }
}

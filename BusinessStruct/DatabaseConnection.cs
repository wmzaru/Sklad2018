using System;
using System.IO;

namespace BusinessStruct
{
    public class DatabaseConnection
    {
        public static string Directory
        {
            get { return DataStruct.DatabaseConnection.Directory; }
        }
        public static string DbName
        {
            get { return DataStruct.DatabaseConnection.DbName; }
        }

        private static string _baseDir
        {
            get
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                if (!baseDir.EndsWith("\\"))
                    baseDir += "\\";
                return baseDir;
            }
        }
        private static string _dbSettingFile
        {
            get { return _baseDir + "DatabaseSetting.txt"; }
        }

        private static void ReadDbSettings(ref string directory, ref string dbname)
        {
            using (StreamReader sr = new StreamReader(_dbSettingFile))
            {
                string line = "";
                if ((line = sr.ReadLine()) != null)
                {
                    directory = line;
                }

                if ((line = sr.ReadLine()) != null)
                {
                    dbname = line;
                }
            }
        }

        private static void WriteDbSettings(string directory, string dbname)
        {
            if (File.Exists(_dbSettingFile))
            {
                File.Delete(_dbSettingFile);
            }

            using (StreamWriter sw = new StreamWriter(_dbSettingFile))
            {
                sw.WriteLine(directory);
                sw.WriteLine(dbname);
            }
        }


        public static bool TestConnecting()
        {
            if (string.IsNullOrWhiteSpace(Directory) || string.IsNullOrWhiteSpace(DbName))
            {
                string dirName = _baseDir;
                string dbName = "Database";
                if(File.Exists(_dbSettingFile))
                    ReadDbSettings(ref dirName, ref dbName);
                ChangeDatabase(dirName, dbName);
            }

            return DataStruct.DatabaseConnection.Test();
        }

        public static bool ChangeDatabase(string directory, string database)
        {
            if (DataStruct.DatabaseConnection.InitializeConnection(directory, database))
            {
                WriteDbSettings(directory, database);
                return true;
            }

            return false;
        }

        public static bool CreateDatabase(string directory, string database)
        {
            if (DataStruct.DatabaseConnection.CreateDatabase(directory, database))
            {
                WriteDbSettings(directory, database);
                return true;
            }

            return false;
        }

    }
}
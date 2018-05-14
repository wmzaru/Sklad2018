using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using DataStruct.Migrations;
using EntityStruct;

namespace DataStruct
{
    /// <summary>
    ///     Создает и держит соединение с mdf файлом.
    ///     Uses Entity Framework to connect to the MS SQL LocalDB.
    ///     Can create a database file if it does not exists.
    ///     Автоматическая миграция базы данных в последнюю версию после подклюяения к ней.
    ///     Contains template "linq to sql" database manipulation functions, that can be used in the data provider classes
    /// </summary>
    public class DatabaseConnection
    {
        private static readonly string _server;
        private static string _connectionString;
        public static string ConnectionString { get; private set; }
        public static string Directory { get; private set; }
        public static string DbName { get; private set; }

        /// <summary>
        ///     Статический конструктор устанавливает значения по умолчанию для частых переменных
        /// </summary>
        static DatabaseConnection()
        {
            _server = "(localdb)\\MSSQLLocalDB";
            Directory = "";
            DbName = "";
            _connectionString = "Data Source=" + _server + ";AttachDbFilename=\"" + Directory + DbName +
                               ".mdf\";Integrated Security=True;";
        }

        public static bool InitializeConnection(string directory, string dbName)
        {
            var result = File.Exists(directory + dbName + ".mdf");
            if (result)
            {
                Directory = directory;
                DbName = dbName;
                _connectionString = "Data Source=" + _server + ";AttachDbFilename=\"" + Directory + DbName +
                                   ".mdf\";Integrated Security=True;";
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<InventoryContext, Configuration>(true));
                using (var db = new InventoryContext(_connectionString))
                {
                    result = db.Database.Exists();
                }
            }

            return result;
        }

        public static bool Test()
        {
            var result = false;
            using (var db = new InventoryContext(_connectionString))
            {
                result = db.Database.Exists();
            }

            return result;
        }

        //public static bool CreateDatabase(string directory, string dbName)
        //{
        //    bool result = false;
        //    string cconnectionString = "Data Source=" + _server + ";AttachDbFilename=\"" + Directory + DbName + ".mdf\";Integrated Security=True;";
        //    using (var db = new InventoryContext(connectionString))
        //    {
        //        try
        //        {
        //            db.Database.CreateIfNotExists();
        //            result = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw;
        //        }
        //        if (result)
        //            InitializeConnection(directory, dbName);
        //        return result;
        //    }
        //}

        /// <summary>
        ///     Создание базы данных если она не существует
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static bool CreateDatabase(string directory, string dbName)
        {
            var result = false;
            var connectionString = "Data Source=" + _server + ";AttachDbFilename=\"" + directory + dbName +
                                   ".mdf\";Integrated Security=True;";
            using (var db = new InventoryContext(connectionString))
            {
                try
                {
                    db.Database.CreateIfNotExists();
                    result = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("create database error\n Folder: {0}\n Database: {1}", directory, dbName);
                }
            }

            if (result)
                InitializeConnection(directory, dbName);
            return result;
        }

        /// <summary>
        ///     Перечисляет каждую запись из таблицы, где условие возвращает true
        /// </summary>
        /// <typeparam name="Entity">Тип табицы в базе данных</typeparam>
        /// <param name="condition">Условие по записям в таблице</param>
        /// <returns></returns>
        public static List<Entity> ListTable<Entity>(Expression<Func<Entity, bool>> condition) where Entity : class
        {
            var list = new List<Entity>();
            using (var db = new InventoryContext(ConnectionString))
            {
                var EntityTable = db.Set<Entity>();
                var query = EntityTable.Where(condition);
                list.AddRange(query);
            }

            return list;
        }

        /// <summary>
        ///     Проверяет, есть ли хотя бы одна запись в таблице базы данных, где условие возвращает true.
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static bool IsExist<Entity>(Expression<Func<Entity, bool>> condition) where Entity : class
        {
            var exists = false;
            using (var db = new InventoryContext(_connectionString))
            {
                var EntityTable = db.Set<Entity>();
                var query = EntityTable.Where(condition);
                exists = query.Count() > 0;
            }

            return exists;
        }

        /// <summary>
        /// Добавляем запись в базу данных
        /// </summary>
        /// <typeparam name="Entity">Тип таблицы в базе данных</typeparam>
        /// <param name="record"></param>
        /// <returns></returns>
        public static bool Add<Entity>(Entity record) where Entity : class
        {
            var result = false;
            using (var db = new InventoryContext(_connectionString))
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var EntityTable = db.Set<Entity>();
                        EntityTable.Add(record);

                        db.SaveChanges();
                        dbTransaction.Commit();
                        result = true;
                    }
                    catch (Exception e)
                    {
                        dbTransaction.Rollback();

                        Console.WriteLine("Невозможно записать в базу данных");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Изменение записи в базе данных
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="record">Запись с обновленными значениями</param>
        /// <param name="condition">Услобие в базе данных которое выбирает запись</param>
        /// <returns></returns>
        public static bool Modify<Entity>(Entity record, Expression<Func<Entity, bool>> condition) where Entity : class
        {
            var result = false;
            var exists = false;
            using (var db = new InventoryContext(_connectionString))
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var EntityTable = db.Set<Entity>();
                        var query = EntityTable.Where(condition);
                        foreach (var rec in query)
                        {
                            exists = true;
                            var propertyes = EntityClone.GetProperties(typeof(Entity));
                            foreach (var property in propertyes) property.SetValue(rec, property.GetValue(record));
                        }

                        if (exists)
                        {
                            db.SaveChanges();
                            dbTransaction.Commit();
                            result = true;
                        }
                    }
                    catch (Exception e)
                    {
                        dbTransaction.Rollback();

                        Console.WriteLine("Невозможно изменить запись в базе данных");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Удаляет запись из базы данных где втсречается условие true
        /// </summary>
        /// <typeparam name="Entity"> Тип таблицы в базе</typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static bool Remove<Entity>(Expression<Func<Entity, bool>> condition) where Entity : class
        {
            var result = false;
            var exists = false;
            using (var db = new InventoryContext(ConnectionString))
            {
                using (var dbTeansaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var EntityTable = db.Set<Entity>();
                        var query = EntityTable.Where(condition);
                        foreach (var rec in query)
                        {
                            exists = true;
                            EntityTable.Remove(rec);
                        }

                        if (exists)
                        {
                            db.SaveChanges();
                            dbTeansaction.Commit();
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        dbTeansaction.Rollback();
                        Console.WriteLine("Cannot remove a record from the database", ex);
                        
                    }
                }
            }

            return result;
        }
    }
}
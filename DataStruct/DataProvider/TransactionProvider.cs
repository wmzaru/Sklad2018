using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using EntityStruct;
using EntityStruct.EntityList;
using EntityStruct.EntityTable;

namespace DataStruct
{
    public class TransactionProvider
    {

        /// <summary>
        /// Перечисляет запись заголовка каждой транзакции из таблицы TransactionTop, где условие возвращает true.
        /// </summary>
        /// <param name="condition">Условие записи в таблицу. Напр. (p => p.Id == record.Id)</param>
        /// <returns></returns>
        public static List<TransactionListTopEntity> ListTop(Expression<Func<TransactionTopEntity, bool>> condition)
        {
            List<TransactionListTopEntity> list = new List<TransactionListTopEntity>();
            using (var db = new InventoryContext(DatabaseConnection.ConnectionString))
            {
                var query = (from top in db.TransactionTop.Where(condition)
                             join partn in db.Providers on top.PartnerId equals partn.Id into PartJoin
                             from subp in PartJoin.DefaultIfEmpty()
                             select new { Head = top, Partner = subp });
                foreach (var record in query)
                    list.Add(new TransactionListTopEntity(record.Head, record.Partner));
                list.Sort();

            }

            return list;
        }
        /// <summary>
        /// Перечисляет запись тела каждой такзации в таблице TransactionBody где условие возвращает true.
        /// </summary>
        /// <param name="condition">Условие записи в таблицу. Напр. (p => p.Id == record.Id)</param>
        /// <returns></returns>
        public static List<TransactionListBodyEntity> ListBody(Expression<Func<TransactionBodyEntity, bool>> condition)
        {
            List<TransactionListBodyEntity> list = new List<TransactionListBodyEntity>();
            using (var db = new InventoryContext(DatabaseConnection.ConnectionString))
            {
                var query = (from b in db.TransactionBody.Where(condition)
                             join p in db.Products on b.ProductId equals p.Id into ProdJoin
                             from subp in ProdJoin.DefaultIfEmpty()
                             select new { Body = b, Product = subp });
                foreach (var record in query)
                    list.Add(new TransactionListBodyEntity(record.Body, record.Product));
                list.Sort();

            }

            return list;
        }
        /// <summary>
        /// Перечисляет количество запасов каждого продукта в таблице TransactionBody, где условие возвращает true.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static List<TransactionListBodyEntity> ListInvenntory(Expression<Func<TransactionBodyEntity, bool>> condition)
        {
            List<TransactionListBodyEntity> list = new List<TransactionListBodyEntity>();
            using (var db = new InventoryContext(DatabaseConnection.ConnectionString))
            {
                var query1 = (from body in db.TransactionBody.Where(condition)
                              join top in db.TransactionTop on body.TransactionId equals top.Id
                              select new
                              {
                                  ProductId = body.ProductId,
                                  Quantity = body.Quantity * (top.Arrival ? 1 : -1)
                              });
                var query2 = (from q1 in query1
                              group q1 by q1.ProductId into g
                              join product in db.Products on g.FirstOrDefault().ProductId equals product.Id
                              select new
                              {
                                  Quantity = g.Sum(p => p.Quantity),
                                  Prodict = product
                              });
                foreach (var record in query2)
                {
                    list.Add(
                        new TransactionListBodyEntity(
                            new TransactionBodyEntity()
                            {
                                Quantity = record.Quantity
                            }, record.Prodict));
                }
            }

            return list;
        }
        /// <summary>
        /// Перечисляет каждую транзакцию, которая изменяет количество запаса продукта в параметре.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static List<TransactionListTopEntity> ListInventoryDetails(int productId)
        {
            List<TransactionListTopEntity> list = new List<TransactionListTopEntity>();
            using (var db = new InventoryContext(DatabaseConnection.ConnectionString))
            {
                var query = (from body in db.TransactionBody.Where(p => p.ProductId == productId)
                             group body by body.TransactionId into g
                             join top in db.TransactionTop on g.FirstOrDefault().TransactionId equals top.Id
                             join provider in db.Providers on top.PartnerId equals provider.Id
                             select new { Top = top, Provider = provider, Sum = g.Sum(p => p.Quantity) * (top.Arrival ? 1 : -1) });
                foreach (var record in query)
                    list.Add(new TransactionListTopEntity(record.Top, record.Provider, record.Sum));

            }
            return list;
        }

        /// <summary>
        /// Перечисляет каждого партнера, который имеет транзакции, и суммирует общую стоимость своих транзакций.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static List<TransactionListTopEntity> ListPartnerTransactions(Expression<Func<TransactionTopEntity, bool>> condition)
        {
            List<TransactionListTopEntity> list = new List<TransactionListTopEntity>();
            using (var db = new InventoryContext(DatabaseConnection.ConnectionString))
            {
                var query = (from top in db.TransactionTop.Where(condition)
                             group top by top.PartnerId into g
                             join partner in db.Providers on g.FirstOrDefault().PartnerId equals partner.Id
                             select new { Partner = partner, Sum = g.Sum(p => p.TotalPrice * (p.Arrival ? -1 : 1)) });
                foreach (var record in query)
                    list.Add(new TransactionListTopEntity(null, record.Partner, record.Sum));
            }

            return list;
        }

        /// <summary>
        /// Проверяет, есть ли хотя бы одна запись в таблице TransactionTop, где условие возвращает true.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static bool IsExistTop(Expression<Func<TransactionTopEntity, bool>> condition)
        {
            return DatabaseConnection.IsExist<TransactionTopEntity>(condition);
        }

        /// <summary>
        /// Проверяет, есть ли хотя бы одна запись в таблице TransactionBody, где условие возвращает true.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static bool IsExistBody(Expression<Func<TransactionBodyEntity, bool>> condition)
        {
            return DatabaseConnection.IsExist<TransactionBodyEntity>(condition);
        }

        /// <summary>
        /// Добавляем или меняем транзакции в базе
        /// </summary>
        /// <param name="top">Если свойство Id = 0, функция добавляет новую транзакцию. Если в базе уже есть транзакция с тем же Id, вункция изменяет ее.</param>
        /// <param name="body"></param>
        /// <returns>Если прошло успешно возвращает true</returns>
        public static bool AddOrModifyTransaction(TransactionTopEntity top, List<TransactionListBodyEntity> body)
        {
            if (top == null || body == null)
                return false;
            bool result = false;
            bool newTop = (top.Id == 0);

            using (var db = new InventoryContext(DatabaseConnection.ConnectionString))
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        int transactionId = top.Id;

                        //Top записи
                        if (!newTop)    //Изменение
                        {
                            var topQuery = db.TransactionTop.Where(p => p.Id == top.Id);
                            foreach (var record in topQuery)
                            {
                                PropertyInfo[] properties = EntityClone.GetProperties(typeof(TransactionTopEntity));
                                foreach (PropertyInfo property in properties)
                                {
                                    property.SetValue(record, property.GetValue(top));
                                }
                            }

                            db.SaveChanges();
                            result = true;
                        }
                        else    //вставить
                        {
                            db.TransactionTop.Add(top);
                            db.SaveChanges();
                            result = true;
                            transactionId = top.Id;
                        }

                        if (result)
                        {
                            result = false;
                            foreach (var rec in body)
                            {
                                rec.Body.TransactionId = transactionId;
                            }

                            var query = db.TransactionBody.Where(p => p.TransactionId == transactionId);
                            foreach (var rec in query)
                            {
                                var newRec = body.Where(p => p.Body.Id == rec.Id).FirstOrDefault();
                                if (newRec == null)
                                {
                                    db.TransactionBody.Remove(rec);
                                }
                                else
                                {
                                    PropertyInfo[] properties =
                                        EntityClone.GetProperties(typeof(TransactionBodyEntity));
                                    foreach (PropertyInfo property in properties)
                                    {
                                        property.SetValue(rec, property.GetValue(newRec.Body));
                                    }

                                    body.Remove(newRec);
                                }
                            }

                            // извлекаем новые записи
                            foreach (var rec in body)
                            {
                                db.TransactionBody.Add(rec.Body);
                            }

                            db.SaveChanges();
                            dbTransaction.Commit();
                            result = true;
                        }
                    }
                    catch (Exception e)
                    {
                        dbTransaction.Rollback();
                        Console.WriteLine("Нет изменений в транзакциях");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Удалчет каждую транзакуцию из таблиц с отметками true;
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static bool RemoveTransaction(Expression<Func<TransactionTopEntity, bool>> condition)
        {
            bool result = false;
            List<int> DeletedTransactions = new List<int>();
            using (var db = new InventoryContext(DatabaseConnection.ConnectionString))
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //delete top
                        var query = db.TransactionTop.Where(condition);
                        foreach (var rec in query)
                        {
                            DeletedTransactions.Add(rec.Id);
                            db.TransactionTop.Remove(rec);
                        }

                        //deleted body
                        foreach (var rec in db.TransactionBody)
                        {
                            foreach (var DeletedId in DeletedTransactions)
                            {
                                if (DeletedId == rec.TransactionId)
                                {
                                    db.TransactionBody.Remove(rec);
                                }
                            }
                        }

                        db.SaveChanges();
                        dbTransaction.Commit();
                        result = true;
                    }
                    catch (Exception e)
                    {
                        dbTransaction.Rollback();
                        Console.WriteLine("Невозможно удалить транзакцию ", e);
                    }
                }
                return result;
            }
        }
    }
}
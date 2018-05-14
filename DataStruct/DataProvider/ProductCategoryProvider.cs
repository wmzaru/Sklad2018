using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Castle.DynamicProxy.Generators.Emitters;
using EntityStruct;
using EntityStruct.EntityTable;

namespace DataStruct
{
    public class ProductCategoryProvider
    {
        public static int GetId(string category)
        {
            int id = 0;
            if (!string.IsNullOrWhiteSpace(category))
            {
                using (var db = new InventoryContext(DatabaseConnection.ConnectionString))
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var query = db.ProductCategories.Where(p => p.Category == category);
                            var record = query.FirstOrDefault();

                            if (record == null)
                            {
                                db.ProductCategories.Add(new ProductListEntity() {Category = category});
                                db.SaveChanges();
                                record = query.FirstOrDefault();
                            }

                            id = record.Id;
                            dbTransaction.Commit();
                        }
                        catch (Exception e)
                        {
                            dbTransaction.Rollback();
                            Console.WriteLine("Невозможно добавить категорию в базу данных. \n Название категории: " + category, e);
                            throw e;
                        }
                    }
                }
            }

            return id;
        }

        public static bool Add(ProductCategoryEntity category)
        {
            if ((category == null) || 
                (string.IsNullOrWhiteSpace(category.Category)) || 
                (DatabaseConnection.IsExist<ProductCategoryEntity>(p=>p.Category == category.Category)))
            {
                return false;
            }

            return DatabaseConnection.Add<ProductCategoryEntity>(category);
        }

        public static bool Modify(ProductCategoryEntity category)
        {
            if ((category == null) ||
                (string.IsNullOrWhiteSpace(category.Category)) ||
                (DatabaseConnection.IsExist<ProductCategoryEntity>(p => p.Id != category.Id && p.Category == category.Category)))
            {
                return false;
            }

            return DatabaseConnection.Modify<ProductCategoryEntity>(category, p=>p.Id == category.Id);
        }

        public static bool Remove(ProductCategoryEntity category)
        {
            if ((category == null) || 
                (ProductProvider.IsExist(p=>p.CategoryId == category.Id)))
            {
                return false;
            }

            return DatabaseConnection.Add<ProductCategoryEntity>(category);
        }

        public static List<ProductCategoryEntity> List(Expression<Func<ProductCategoryEntity, bool>> condition)
        {
            return DatabaseConnection.ListTable<ProductCategoryEntity>(condition);
        }
    }
}
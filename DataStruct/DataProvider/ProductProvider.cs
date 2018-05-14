using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityStruct;
using EntityStruct.EntityTable;
using EntityStruct.EntityList;

namespace DataStruct
{
    public class ProductProvider
    {
        private static ProductEntity ConvertToProduct(ProductListEntity categoryElement)
        {
            ProductEntity record = null;
            if (categoryElement != null)
            {
                try
                {
                    int categoryId = ProductCategoryProvider.GetId(categoryElement.Category);
                    record = new ProductEntity()
                    {
                        CategoryId = categoryId,
                        Code = categoryElement.Code,
                        CostPrice = categoryElement.CostPrice,
                        Id = categoryElement.Id,
                        Name = categoryElement.Name,
                        SellPrice = categoryElement.SellPrice
                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: ", e);
                }
            }

            return record;
        }

        public static bool Add(ProductListEntity product)
        {
            ProductEntity record = ConvertToProduct(product);
            if (record == null)
                return false;
            return DatabaseConnection.Add<ProductEntity>(record);
        }

        public static bool Modify(ProductListEntity product)
        {
            ProductEntity record = ConvertToProduct(product);
            if (record == null)
                return false;
            return DatabaseConnection.Modify<ProductEntity>(record, p=>p.Id == record.Id);
        }

        public static bool Remove(ProductListEntity product)
        {
            ProductEntity record = ConvertToProduct(product);
            if ((record == null) ||
                (TransactionProvider.IsExistBody(p => p.ProductId == product.Id)))
                return false;
            return DatabaseConnection.Remove<ProductEntity>(p => p.Id == product.Id);
        }

        public static List<ProductListEntity> List(Expression<Func<ProductEntity, bool>> condition)
        {
            List<ProductListEntity> list = new List<ProductListEntity>();
            using (var db = new InventoryContext(DatabaseConnection.ConnectionString))
            {
                var query1 = (from p in db.Products.Where(condition)
                              join c in db.ProductCategories on p.CategoryId equals c.Id into CatJoin
                              from subc in CatJoin.DefaultIfEmpty()
                              select new {Product = p, Category = subc});
                foreach (var rec in query1)
                {
                    if(rec.Category == null)
                        list.Add(new ProductListEntity(rec.Product));
                    else
                        list.Add(new ProductListEntity(rec.Product, rec.Category));
                    
                }
                list.Sort();
            }
            return list;
        }


        public static bool IsExist(Expression<Func<ProductEntity, bool>> condition)
        {
            return DatabaseConnection.IsExist<ProductEntity>(condition);
        }


    }
}
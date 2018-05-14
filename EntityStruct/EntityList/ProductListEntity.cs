using EntityStruct.EntityTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStruct
{
    public class ProductListEntity : IComparable<ProductListEntity>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }


        public ProductListEntity() { }

        public ProductListEntity(ProductEntity product) : this()
        {
            Code = product.Code;
            CostPrice = product.CostPrice;
            Id = product.Id;
            Name = product.Name;
            SellPrice = product.SellPrice;
        }

        public ProductListEntity(ProductEntity product, ProductListEntity category) 
            : this(product)
        {
            Category = category.Category;
        }


        public int CompareTo(ProductListEntity other)
        {
            var compare1 = Name ?? string.Empty;
            var compare2 = other.Name ?? string.Empty;
            return compare1.CompareTo(compare2);
        }
    }
}

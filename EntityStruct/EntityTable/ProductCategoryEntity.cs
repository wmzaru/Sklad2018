using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStruct
{
    [Table("ProductCat")]
    public class ProductCategoryEntity
    {
        [Key]
        public int Id { get; set; }

        public string Category { get; set; }        // Категория товаров
    }
}

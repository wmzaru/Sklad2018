using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EntityStruct.EntityTable
{
    [Table("Products")]
    public class ProductEntity
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }            // Код товара
        public string Name { get; set; }            // Название товара
        public int CategoryId { get; set; }         // № категории
        public decimal CostPrice { get; set; }      // Цена покупки
        public decimal SellPrice { get; set; }      // Цена продажи
    }
}

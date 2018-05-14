using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStruct
{
    [Table("TransactionTop")]
    public class TransactionTopEntity
    {
        [Key]
        
        
        public int Id { get; set; }
        /// <summary>
        /// Поступление
        /// </summary>
        public bool Arrival { get; set; } 
        /// <summary>
        /// ID партнера
        /// </summary>
        public int PartnerId { get; set; }
        /// <summary>
        /// Дата поступления
        /// </summary>
        [Column(TypeName = "DateTime2")]
        public DateTime Date { get; set; }
        /// <summary>
        /// Общая цена
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}

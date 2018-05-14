using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStruct.EntityTable
{
    [Table("Providers")]
    public class ProviderEntity : IComparable<ProviderEntity>
    {
        [Key]
        public int Id { get; set; }             // Id

        public string Name { get; set; }        // Имя
        public string Code { get; set; }        // Код
        public bool Customer { get; set; }      // Клиент
        public bool Trader { get; set; }        // Торговец
        public string Address { get; set; }     // Адресс
        public string AccNumber { get; set; }   // № Аккаунта
        public string Phone { get; set; }       // Телефон
        public string Email { get; set; }       // Почта
        


        public int CompareTo(ProviderEntity other)
        {
            var compare1 = Name ?? string.Empty;
            var compare2 = other.Name ?? string.Empty;
            return compare1.CompareTo(compare2);
        }
    }
}

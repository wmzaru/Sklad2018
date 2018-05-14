using EntityStruct.EntityTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStruct.EntityList
{
    public class TransactionListBodyEntity
    {
        public TransactionListBodyEntity(){ }
        public TransactionListBodyEntity(TransactionBodyEntity body, ProductEntity prodict)
            :this()
        {
            Body = body;
            Product = prodict;
        }

        public TransactionBodyEntity Body { get; set; }
        public ProductEntity Product { get; set; }
    }
}

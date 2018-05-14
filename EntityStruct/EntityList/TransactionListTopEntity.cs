using EntityStruct.EntityTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStruct.EntityList
{
    public class TransactionListTopEntity : IComparable<TransactionListTopEntity>
    {
        public TransactionListTopEntity() { }
        public TransactionListTopEntity(TransactionTopEntity top, ProviderEntity provider)
            : this()
        {
            Top = top;
            Provider = provider;
        }

        public TransactionListTopEntity(TransactionTopEntity top, ProviderEntity provider, decimal listVariable)
            : this(top, provider)
        {
            ListVariable = listVariable;
        }

        public TransactionTopEntity Top { get; set; }
        public ProviderEntity Provider { get; set; }
        public decimal ListVariable { get; set; }

        public int CompareTo(TransactionListTopEntity other)
        {
            if (Top == null)
            {
                return Provider.CompareTo(other.Provider);
            }
            return Top.Date.CompareTo(other.Top.Date);
        }
    }
}

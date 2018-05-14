using System.Collections.Generic;
using DataStruct;
using EntityStruct;
using EntityStruct.EntityList;
using EntityStruct.EntityTable;

namespace BusinessStruct
{
    public class ManageTransactions
    {
        public static List<TransactionListTopEntity> ListTop(bool Arrival)
        {
            var list = TransactionProvider.ListTop(p => p.Arrival == Arrival);
            list.Sort();
            return list;
        }
        public static List<TransactionListTopEntity> ListTop(int partnerId)
        {
            var list = TransactionProvider.ListTop(p => p.PartnerId == partnerId);
            list.Sort();
            foreach (var record in list)
            {
                record.ListVariable = record.Top.TotalPrice * (record.Top.Arrival ? -1 : 1);
            }
            return list;
        }
        public static List<TransactionListBodyEntity> ListBody(int transactionId)
        {
            return TransactionProvider.ListBody(p => p.TransactionId == transactionId);
        }
        public static bool AddOrModifyTransaction(TransactionTopEntity top, List<TransactionListBodyEntity> body)
        {
            return TransactionProvider.AddOrModifyTransaction(top, body);
        }

        public static bool RemoveTransaction(TransactionTopEntity top)
        {
            return TransactionProvider.RemoveTransaction(p => p.Id == top.Id);
        }

        public static List<TransactionListBodyEntity> ListInventory()
        {
            return TransactionProvider.ListInvenntory(p => true);
        }

        public static List<TransactionListTopEntity> ListInventoryDetails(int productId)
        {
            var list = TransactionProvider.ListInventoryDetails(productId);
            list.Sort();
            return list;
        }

        public static List<TransactionListTopEntity> ListPartnerTransactions()
        {
            var list = TransactionProvider.ListPartnerTransactions(p=>true);
            list.Sort();
            return list;
        }


    }
}
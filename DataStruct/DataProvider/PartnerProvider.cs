using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EntityStruct.EntityTable;

namespace DataStruct
{
    public class PartnerProvider
    {
        public static bool Add(ProviderEntity provider)
        {
            if (provider == null)
                return false;
            return DatabaseConnection.Add<ProviderEntity>(provider);
        }

        public static bool Modify(ProviderEntity provider)
        {
            if (provider == null)
                return false;
            return DatabaseConnection.Modify<ProviderEntity>(provider, p => p.Id == provider.Id);
        }

        public static bool Remove(ProviderEntity provider)
        {
            if ((provider == null) || (TransactionProvider.IsExistTop(p => p.Id == provider.Id)))
                return false;
            return DatabaseConnection.Remove<ProviderEntity>(p => p.Id == provider.Id);
        }

        public static List<ProviderEntity> List(Expression<Func<ProviderEntity, bool>> condition)
        {
            var list = DatabaseConnection.ListTable<ProviderEntity>(condition);
            list.Sort();
            return list;
        }
    }
}
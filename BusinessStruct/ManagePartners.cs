using System.Collections.Generic;
using DataStruct;
using EntityStruct.EntityTable;

namespace BusinessStruct
{
    public class ManagePartners
    {
        public static List<ProviderEntity> ListPartners()
        {
            return PartnerProvider.List(p => true);
        }

        public static List<ProviderEntity> ListCustomers()
        {
            return PartnerProvider.List(p => p.Customer);
        }

        public static List<ProviderEntity> ListTraders()
        {
            return PartnerProvider.List(p => p.Trader);
        }

        public static bool NewPartner(ProviderEntity partner)
        {
            return PartnerProvider.Add(partner);
        }

        public static bool DeletePartner(ProviderEntity partner)
        {
            return PartnerProvider.Remove(partner);
        }

        public static bool ModifyPartner(ProviderEntity partner)
        {
            return PartnerProvider.Modify(partner);
        }
    }
}
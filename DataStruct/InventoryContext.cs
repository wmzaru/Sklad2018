using EntityStruct;
using EntityStruct.EntityTable;
using System.Data.Entity;
using System.Data.Linq.Mapping;

namespace DataStruct
{
    /// <summary>
    /// Контекст базы данных для базы данных инвентаризации.
    /// </summary>
    public class InventoryContext : DbContext
    {
        private static MappingSource _mappingSource = new AttributeMappingSource();

        public InventoryContext() : base() { }

        public InventoryContext(string connectionString) : base(connectionString) { }

        public InventoryContext(System.Data.Common.DbConnection dbConnection, bool contextOwnConnection) : base(dbConnection, contextOwnConnection) { }

        public DbSet<UsersEntity> Users { get; set; }
        public DbSet<ProviderEntity> Providers { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductListEntity> ProductCategories { get; set; }
        public DbSet<TransactionBodyEntity> TransactionBody { get; set; }
        public DbSet<TransactionTopEntity> TransactionTop { get; set; }

    }
}

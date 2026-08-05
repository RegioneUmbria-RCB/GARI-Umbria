using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AgronicaNetCoreApi
{
    public class AgronicaDataProtectionContext : DbContext, IDataProtectionKeyContext
    {
        public AgronicaDataProtectionContext(){ }

        public AgronicaDataProtectionContext(DbContextOptions<AgronicaDataProtectionContext> options)
            : base(options){ }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }

}

using Microsoft.EntityFrameworkCore;

namespace Jinkong.Payment.EfStore
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<PrepayOrders> PrepayOrders { get; set; }
        public DbSet<PrepayOrderPaymentLogs> PrepayOrderPaymentLogs { get; set; }
        public DbSet<RefundOrders> RefundOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //TODO: ...entity config
        }
    }
}
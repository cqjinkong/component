using Jinkong.NLogger.Loggers;
using Jinkong.NLogger.Mysql.EntityConfigs;
using Microsoft.EntityFrameworkCore;
// ReSharper disable CheckNamespace

namespace Jinkong.NLogger
{

    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {

        }

        public DbSet<RequestLogs> RequestLogs { get; set; }
        public DbSet<ErrorLogs> ErrorLogs { get; set; }
        public DbSet<LoginLogs> LoginLogs { get; set; }
        public DbSet<OperationLogs> OperationLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new RequestLogsConfig());
            builder.ApplyConfiguration(new ErrorLogsConfig());
            builder.ApplyConfiguration(new LoginLogsConfig());
            builder.ApplyConfiguration(new OperationLogsConfig());
        }
    }
}

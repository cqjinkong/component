﻿using Jinkong.NLogger.Loggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jinkong.NLogger.Postgresql.EntityConfigs
{
    public class RequestLogsConfig : IEntityTypeConfiguration<RequestLogs>
    {
        public void Configure(EntityTypeBuilder<RequestLogs> builder)
        {
            builder.Property(r => r.User).HasMaxLength(255);
            builder.Property(r => r.ClientId).HasMaxLength(255);
            builder.Property(r => r.Logger).HasMaxLength(255);
            builder.Property(r => r.Level).HasMaxLength(255);
            builder.Property(r => r.ClientIp).HasMaxLength(255);
            builder.Property(r => r.Method).HasMaxLength(255);
        }
    }
}

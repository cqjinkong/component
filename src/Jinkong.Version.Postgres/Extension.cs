using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.Extensions.Logging;
using Shashlik.Redis;
using Shashlik.Utils.Extensions;

// ReSharper disable CheckNamespace

namespace Jinkong.Version
{
    public static class Extension
    {
        private const string LockKey = "VERSION_UPDATING";

        /// <summary>
        /// 执行版本更新,使用ef core上下文自动启用事务执行更新
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static IServiceProvider UseVersionManagement<TDbContext>(this IServiceProvider serviceProvider)
            where TDbContext : DbContext
        {
            using var locking = RedisHelper.Instance.Locking(LockKey, 60);
            using (var scope = serviceProvider.CreateScope())
            using (var initDbContext = scope.ServiceProvider.GetService<TDbContext>())
                // 初始化表
                InitDb(initDbContext.Database.GetDbConnection());

            using (var scope = serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetService<TDbContext>())
                {
                    var versions = scope.ServiceProvider.GetServices<IVersion>()
                        ?.OrderBy(r => r.Priority)
                        .ThenBy(r => r.VersionId)
                        .ToList();
                    if (versions.IsNullOrEmpty())
                        return serviceProvider;

                    var conn = dbContext.Database.GetDbConnection();
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var versionIds = GetUpdatedVersions(conn);
                    if (versions.HasRepeat(r => r.VersionId))
                        throw new Exception("存在重复的VersionId");
                    var notUpdates = versions!.Where(r => !versionIds.Contains(r.VersionId)).ToList();
                    if (notUpdates.IsNullOrEmpty()) return serviceProvider;

                    var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                        .CreateLogger("Jinkong.Version.Postgres");
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in notUpdates)
                            {
                                logger.LogInformation($"开始更新版本:{item.VersionId}");
                                item.Update().Wait();
                                logger.LogInformation($"版本更新完成:{item.VersionId}");
                            }

                            InsertUpdateRecord(conn, transaction.GetDbTransaction(),
                                notUpdates.ToDictionary(r => r.VersionId, r => r.Desc));

                            transaction.Commit();

                            return serviceProvider;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行版本更新,使用ef core 上下文事务
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="transaction"></param>
        public static IServiceProvider UseVersionManagement<TDbContext>(this IServiceProvider serviceProvider,
            IDbContextTransaction transaction)
            where TDbContext : DbContext
        {
            using var locking = RedisHelper.Instance.Locking(LockKey, 60);
            using (var scope = serviceProvider.CreateScope())
            using (var initDbContext = scope.ServiceProvider.GetService<TDbContext>())
                // 初始化表
                InitDb(initDbContext.Database.GetDbConnection());

            using (var scope = serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetService<TDbContext>())
                {
                    var versions = scope.ServiceProvider.GetServices<IVersion>()
                        ?.OrderBy(r => r.Priority)
                        .ThenBy(r => r.VersionId)
                        .ToList();
                    if (versions.IsNullOrEmpty())
                        return serviceProvider;

                    var conn = dbContext.Database.GetDbConnection();
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var versionIds = GetUpdatedVersions(conn);
                    if (versions.HasRepeat(r => r.VersionId))
                        throw new Exception("存在重复的VersionId");
                    var notUpdates = versions!.Where(r => !versionIds.Contains(r.VersionId)).ToList();
                    if (notUpdates.IsNullOrEmpty()) return serviceProvider;

                    var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                        .CreateLogger("Jinkong.Version.Postgres");
                    using (transaction)
                    {
                        try
                        {
                            foreach (var item in notUpdates)
                            {
                                logger.LogInformation($"开始更新版本:{item.VersionId}");
                                item.Update().Wait();
                                logger.LogInformation($"版本更新完成:{item.VersionId}");
                            }

                            InsertUpdateRecord(conn, dbContext.Database.CurrentTransaction.GetDbTransaction(),
                                notUpdates.ToDictionary(r => r.VersionId, r => r.Desc));

                            transaction.Commit();

                            return serviceProvider;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="tranFunc"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IServiceProvider UseVersionManagement<TDbContext>(this IServiceProvider serviceProvider,
            Func<IServiceProvider, IDbContextTransaction> tranFunc)
            where TDbContext : DbContext
        {
            using var locking = RedisHelper.Instance.Locking(LockKey, 60);
            using (var scope = serviceProvider.CreateScope())
            using (var initDbContext = scope.ServiceProvider.GetService<TDbContext>())
                // 初始化表
                InitDb(initDbContext.Database.GetDbConnection());

            using (var scope = serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetService<TDbContext>())
                {
                    var versions = scope.ServiceProvider.GetServices<IVersion>()
                        ?.OrderBy(r => r.Priority)
                        .ThenBy(r => r.VersionId)
                        .ToList();
                    if (versions.IsNullOrEmpty())
                        return serviceProvider;
                    var conn = dbContext.Database.GetDbConnection();
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var versionIds = GetUpdatedVersions(conn);
                    if (versions.HasRepeat(r => r.VersionId))
                        throw new Exception("存在重复的VersionId");

                    var notUpdates = versions!.Where(r => !versionIds.Contains(r.VersionId)).ToList();
                    if (notUpdates.IsNullOrEmpty()) return serviceProvider;

                    var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                        .CreateLogger("Jinkong.Version.Postgres");
                    using (var transaction = tranFunc(scope.ServiceProvider))
                    {
                        try
                        {
                            foreach (var item in notUpdates)
                            {
                                logger.LogInformation($"开始更新版本:{item.VersionId}");
                                item.Update().Wait();
                                logger.LogInformation($"版本更新完成:{item.VersionId}");
                            }

                            InsertUpdateRecord(conn, dbContext.Database.CurrentTransaction.GetDbTransaction(),
                                notUpdates.ToDictionary(r => r.VersionId, r => r.Desc));

                            transaction.Commit();

                            return serviceProvider;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        static string schema = "guc.version";
        static string tableName = "updates";

        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="conn"></param>
        internal static void InitDb(IDbConnection conn)
        {
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    // 创建架构和数据表
                    var batchSql = $@"
CREATE SCHEMA IF NOT EXISTS ""{schema}"";

CREATE TABLE IF NOT EXISTS ""{schema}"".""{tableName}""(
    ""VersionId"" VARCHAR(32) PRIMARY KEY NOT NULL,
    ""UpdateTime"" timestamp NOT NULL,
    ""Desc"" VARCHAR(4000) NULL
);
";
                    cmd.CommandText = batchSql;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 获取已更新的版本id
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        internal static List<string> GetUpdatedVersions(IDbConnection conn)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                string sql = $@"SELECT ""VersionId"" FROM ""{schema}"".""{tableName}"";";
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                List<string> versions = new List<string>();
                foreach (DataRow row in table.Rows)
                    versions.Add(row[0].ToString());

                return versions;
            }
        }

        static void InsertUpdateRecord(IDbConnection conn, DbTransaction dbTransaction,
            Dictionary<string, string> versions)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            foreach (var item in versions)
            {
                using (var cmd = conn.CreateCommand())
                {
                    var sql = $@"insert into ""{schema}"".""{tableName}"" values(@id,now(),@desc);";
                    cmd.CommandText = sql;
                    cmd.Transaction = dbTransaction;
                    cmd.Parameters.Add(new NpgsqlParameter("@id", DbType.String, 32) {Value = item.Key});
                    cmd.Parameters.Add(new NpgsqlParameter("@desc", DbType.String, 4000)
                        {Value = item.Value ?? (object) DBNull.Value});
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
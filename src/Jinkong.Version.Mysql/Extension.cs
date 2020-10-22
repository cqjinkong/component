using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Data.Common;
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
            using var locker = RedisHelper.Instance.Locking(LockKey, 60);

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
            using var locker = RedisHelper.Instance.Locking(LockKey, 60);
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

        public static IServiceProvider UseVersionManagement<TDbContext>(this IServiceProvider serviceProvider,
            Func<IServiceProvider, IDbContextTransaction> tranFunc)
            where TDbContext : DbContext
        {
            using var locker = RedisHelper.Instance.Locking(LockKey, 60);
            using (var scope = serviceProvider.CreateScope())
            using (var initDbContext = scope.ServiceProvider.GetService<TDbContext>())
                // 初始化表
                InitDb(initDbContext.Database.GetDbConnection());

            using (var scope = serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetService<TDbContext>())
                {
                    var versions = scope.ServiceProvider
                        .GetServices<IVersion>()
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

        private const string Schema = "jinkong_version";
        private const string TableName = "updates";

        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="conn"></param>
        internal static void InitDb(DbConnection conn)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                // 创建架构和数据表
                var batchSql = $@"
CREATE TABLE IF NOT EXISTS `{Schema}_{TableName}`(
	`VersionId` VARCHAR(32) PRIMARY KEY NOT NULL,
    `UpdateTime` timestamp NOT NULL,
	`Desc` VARCHAR(4000) NULL
);
";
                cmd.CommandText = batchSql;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取已更新的版本id
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static List<string> GetUpdatedVersions(DbConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                string sql = $@"SELECT `VersionId` FROM `{Schema}_{TableName}`;";
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

        private static void InsertUpdateRecord(DbConnection connection, DbTransaction dbTransaction,
            Dictionary<string, string> versions)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            foreach (var item in versions)
            {
                using (var cmd = connection.CreateCommand())
                {
                    var sql = $@"insert into `{Schema}_{TableName}` values(@id,now(),@desc);";
                    cmd.CommandText = sql;
                    cmd.Transaction = dbTransaction;
                    cmd.Parameters.Add(new MySqlParameter("@id", MySqlDbType.String, 32) {Value = item.Key});
                    cmd.Parameters.Add(
                        new MySqlParameter("@desc", MySqlDbType.String, 4000)
                        {
                            Value = item.Value ?? (object) DBNull.Value
                        });
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Data.Common;
using Shashlik.Redis;
using Shashlik.Utils.Extensions;

namespace Jinkong.Version
{
    public static class Extension
    {
        /// <summary>
        /// 执行版本更新,使用ef core上下文自动启用事务执行更新
        /// </summary>
        /// <param name="service"></param>
        public static IServiceProvider UseVersionManagement<TDbContext>(this IServiceProvider ServiceProvider)
            where TDbContext : DbContext
        {
            using var locker = RedisHelper.Instance.Locking("GUC_VERSION_UPDATE_LOCKING", 60);

            using (var scope = ServiceProvider.CreateScope())
            using (var initDbContext = scope.ServiceProvider.GetService<TDbContext>())
                // 初始化表
                InitDb(initDbContext.Database.GetDbConnection());

            using (var scope = ServiceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetService<TDbContext>())
                {
                    var versions = scope.ServiceProvider.GetServices<IVersion>()?.OrderBy(r => r.Priority)
                        ?.ThenBy(r => r.VersionId)?.ToList();
                    if (versions.IsNullOrEmpty())
                        return ServiceProvider;

                    var conn = dbContext.Database.GetDbConnection();
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var versionIds = GetUpdatedVersions(conn);
                    if (versions.HasRepeat(r => r.VersionId))
                        throw new Exception("存在重复的VersionId");
                    var notUpdates = versions.Where(r => !versionIds.Contains(r.VersionId));
                    if (notUpdates.IsNullOrEmpty()) return ServiceProvider;

                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in notUpdates)
                            {
                                Console.WriteLine($"开始更新版本:{item.VersionId}");
                                item.Update().Wait();
                                Console.WriteLine($"版本更新完成:{item.VersionId}");
                            }

                            InsertUpdateRecord(conn, transaction.GetDbTransaction(),
                                notUpdates.ToDictionary(r => r.VersionId, r => r.Desc));

                            transaction.Commit();

                            return ServiceProvider;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行版本更新,使用ef core 上下文事务
        /// </summary>
        /// <param name="service"></param>
        public static IServiceProvider UseVersionManagement<TDbContext>(this IServiceProvider ServiceProvider,
            IDbContextTransaction transaction)
            where TDbContext : DbContext
        {
            using var locker = RedisHelper.Instance.Locking("GUC_VERSION_UPDATE_LOCKING", 60);
            using (var scope = ServiceProvider.CreateScope())
            using (var initDbContext = scope.ServiceProvider.GetService<TDbContext>())
                // 初始化表
                InitDb(initDbContext.Database.GetDbConnection());

            using (var scope = ServiceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetService<TDbContext>())
                {
                    var versions = scope.ServiceProvider.GetServices<IVersion>()?.OrderBy(r => r.Priority)
                        ?.ThenBy(r => r.VersionId)?.ToList();
                    if (versions.IsNullOrEmpty())
                        return ServiceProvider;

                    var conn = dbContext.Database.GetDbConnection();
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var versionIds = GetUpdatedVersions(conn);
                    if (versions.HasRepeat(r => r.VersionId))
                        throw new Exception("存在重复的VersionId");
                    var notUpdates = versions.Where(r => !versionIds.Contains(r.VersionId));
                    if (notUpdates.IsNullOrEmpty()) return ServiceProvider;

                    using (transaction)
                    {
                        try
                        {
                            foreach (var item in notUpdates)
                            {
                                Console.WriteLine($"开始更新版本:{item.VersionId}");
                                item.Update().Wait();
                                Console.WriteLine($"版本更新完成:{item.VersionId}");
                            }

                            InsertUpdateRecord(conn, dbContext.Database.CurrentTransaction.GetDbTransaction(),
                                notUpdates.ToDictionary(r => r.VersionId, r => r.Desc));

                            transaction.Commit();

                            return ServiceProvider;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
        }

        public static IServiceProvider UseVersionManagement<TDbContext>(this IServiceProvider ServiceProvider,
            Func<IServiceProvider, IDbContextTransaction> tranFunc)
            where TDbContext : DbContext
        {
            using var locker = RedisHelper.Instance.Locking("GUC_VERSION_UPDATE_LOCKING", 60);
            using (var scope = ServiceProvider.CreateScope())
            using (var initDbContext = scope.ServiceProvider.GetService<TDbContext>())
                // 初始化表
                InitDb(initDbContext.Database.GetDbConnection());

            using (var scope = ServiceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetService<TDbContext>())
                {
                    var versions = scope.ServiceProvider.GetServices<IVersion>()?.OrderBy(r => r.Priority)
                        ?.ThenBy(r => r.VersionId)?.ToList();
                    if (versions.IsNullOrEmpty())
                        return ServiceProvider;
                    var conn = dbContext.Database.GetDbConnection();
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var versionIds = GetUpdatedVersions(conn);
                    if (versions.HasRepeat(r => r.VersionId))
                        throw new Exception("存在重复的VersionId");

                    var notUpdates = versions.Where(r => !versionIds.Contains(r.VersionId));
                    if (notUpdates.IsNullOrEmpty()) return ServiceProvider;

                    using (var transaction = tranFunc(scope.ServiceProvider))
                    {
                        try
                        {
                            foreach (var item in notUpdates)
                            {
                                Console.WriteLine($"开始更新版本:{item.VersionId}");
                                item.Update().Wait();
                                Console.WriteLine($"版本更新完成:{item.VersionId}");
                            }

                            InsertUpdateRecord(conn, dbContext.Database.CurrentTransaction.GetDbTransaction(),
                                notUpdates.ToDictionary(r => r.VersionId, r => r.Desc));

                            transaction.Commit();

                            return ServiceProvider;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
        }

        const string schema = "guc_version";
        const string tableName = "updates";

        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="connString"></param>
        internal static void InitDb(DbConnection conn)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                // 创建架构和数据表
                var batchSql = $@"
CREATE TABLE IF NOT EXISTS `{schema}_{tableName}`(
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
        /// <param name="connString"></param>
        /// <returns></returns>
        internal static List<string> GetUpdatedVersions(DbConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                string sql = $@"SELECT `VersionId` FROM `{schema}_{tableName}`;";
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

        static void InsertUpdateRecord(DbConnection connection, DbTransaction dbTransaction,
            Dictionary<string, string> versions)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            foreach (var item in versions)
            {
                using (var cmd = connection.CreateCommand())
                {
                    var sql = $@"insert into `{schema}_{tableName}` values(@id,now(),@desc);";
                    cmd.CommandText = sql;
                    cmd.Transaction = dbTransaction;
                    cmd.Parameters.Add(new MySqlParameter("@id", MySqlDbType.String, 32) {Value = item.Key});
                    cmd.Parameters.Add(
                        new MySqlParameter("@desc", MySqlDbType.String, 4000)
                        {
                            Value = item.Value ?? (object)DBNull.Value
                        });
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
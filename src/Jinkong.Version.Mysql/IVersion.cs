using System.Threading.Tasks;
using Shashlik.Kernel.Dependency;

namespace Jinkong.Version
{
    [Transient]
    public interface IVersion
    {
        /// <summary>
        /// 优先级
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 版本id
        /// </summary>
        string VersionId { get; }

        /// <summary>
        /// 版本备注
        /// </summary>
        string Desc { get; }

        /// <summary>
        /// 执行更新操作
        /// </summary>
        Task Update();
    }
}
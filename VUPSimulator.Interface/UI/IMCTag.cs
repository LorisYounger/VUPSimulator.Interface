using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 消息窗口
    /// </summary>
    public interface IMCTag
    {
        /// <summary>
        /// 类型
        /// </summary>
        EventBase.VisibleType Type { get; }
        /// <summary>
        /// 主窗口
        /// </summary>
        IMainWindow MW { get; }
        /// <summary>
        /// 当被事件激活时事件
        /// </summary>
        void MW_TimeUIHandle(TimeSpan span, IMainWindow mw);


    }

}

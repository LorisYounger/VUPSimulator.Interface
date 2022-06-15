using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 软件类, 添加至软件列表 以在软件中心显示
    /// </summary>
    public interface ISoftWare
    {
        /// <summary>
        /// 软件名
        /// </summary>
        string SoftwareName { get; }
        /// <summary>
        /// 软件介绍
        /// </summary>
        string SoftwareInfo { get; }

        /// <summary>
        /// 窗体内控件, 由开发者设计和提供
        /// </summary>
        WindowsPageHandle NewSoftWare(IMainWindow mw);
    
    }
}

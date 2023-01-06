using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static VUPSimulator.Interface.Function;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 桌面控件接口
    /// </summary>
    public interface WidgetHandle
    {
        /// <summary>
        /// 宽度
        /// </summary>
        double Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        double Height { get; set; }
        //这些不需要手动继承, Grid/UC原本就有
        double MaxWidth { get; set; }
        double MaxHeight { get; set; }
        double MinWidth { get; set; }
        double MinHeight { get; set; }
        /// <summary>
        /// 允许修改大小
        /// </summary>
        WindowsSizeChange AllowSizeChange { get; }
        
        /// <summary>
        /// 执行关闭程序
        /// </summary>
        /// <returns>反馈是否关闭</returns>
        bool Closeing();

        /// <summary>
        /// 该窗口的host
        /// </summary>
        IWidget Host { get; }
        /// <summary>
        /// 这个Gird/桌面控件
        /// </summary>
        FrameworkElement This { get; }

        /// <summary>
        /// 桌面控件名 用于显示与重复性检查
        /// </summary>
        string ID { get; }
        /// <summary>
        /// 右键菜单,如需自定义请修改设置 MenuItems
        /// </summary>
        ContextMenu ContextMenu { set; }
        /// <summary>
        /// 右键菜单详细设置 如需自定义请添加
        /// </summary>
        List<MenuItem> MenuItems { get; }
        /// <summary>
        /// 是否为等比缩放
        /// </summary>
        bool IsUniformSizeChanged { get; }
    }
}

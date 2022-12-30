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
    /// 窗体内控件 请同时继承与Grid/UC以便进行窗体设计
    /// </summary>
    public interface WindowsPageHandle
    {
        //public WindowsPageHandle(MainWindow mainw)
        //{
        //    Host = new Windows(mainw,this);
        //}
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
        /// 程序ID 用于重复性检查
        /// </summary>
        string ID { get; }
        /// <summary>
        /// 执行关闭程序
        /// </summary>
        /// <returns>反馈是否关闭</returns>
        bool Closeing();
        /// <summary>
        /// 最大化处理
        /// </summary>
        void Max();
        /// <summary>
        /// 正常化处理
        /// </summary>
        void Min();
        /// <summary>
        /// 隐藏处理
        /// </summary>
        void Hide();
        /// <summary>
        /// 显示处理(一般为第二次显示)
        /// </summary>
        void Show();
        /// <summary>
        /// 允许修改大小
        /// </summary>
        WindowsSizeChange AllowSizeChange { get; }
        /// <summary>
        /// 允许隐藏
        /// </summary>
        bool AllowHide { get; }
        /// <summary>
        /// 该窗口的host
        /// </summary>
        IWindows Host { get; }
        /// <summary>
        /// 这个Gird/MW窗口
        /// </summary>
        FrameworkElement This { get; }

        /// <summary>
        /// 电脑使用性能与配置
        /// </summary>
        ComputerUsage Usage { get; set; }
    }
}

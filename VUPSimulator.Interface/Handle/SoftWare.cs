﻿using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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
        /// <param name="mw">主窗体</param>
        /// <param name="args">软件启动参数</param>
        WindowsPageHandle NewSoftWare(IMainWindow mw, string args = null);

    }
    /// <summary>
    /// 桌面组件类, 添加至控件列表 以在桌面组件中心显示
    /// </summary>
    public interface IDesktopWidget
    {
        /// <summary>
        /// 桌面组件名
        /// </summary>
        string WidgetName { get; }
        /// <summary>
        /// 软件介绍
        /// </summary>
        string WidgetInfo { get; }
        /// <summary>
        /// ID,用于防止重复打开
        /// </summary>
        string ID { get; }
        /// <summary>
        /// 软件截图
        /// </summary>
        ImageSource WidgetScreenShort { get; }

        /// <summary>
        /// 桌面组件, 由开发者设计和提供
        /// </summary>
        /// <param name="mw">主窗体</param>
        /// <param name="data">桌面组件设置相关参数</param>
        WidgetHandle NewWidget(IMainWindow mw, ILine data);

    }
}

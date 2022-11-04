using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using static VUPSimulator.Interface.Function;
namespace VUPSimulator.Interface
{
    /// <summary>
    /// 主窗体接口 开发者可通过此接口获取游戏信息和进行高级代码MOD开发
    /// </summary>
    public interface IMainWindow
    {
        /// <summary>
        /// 调用UI界面时,必须使用 Dispatcher 进行调度
        /// </summary>
        Dispatcher Dispatcher { get; }

        /// <summary>
        /// 游戏存档
        /// </summary>
        ISave Save { get; }
        /// <summary>
        /// 游戏数据
        /// </summary>
        ICore Core { get; }
        /// <summary>
        /// 是否为steam用户
        /// </summary>
        bool IsSteamUser { get; }

        /// <summary>
        /// 版本号
        /// </summary>
        int verison { get; }
        /// <summary>
        /// 版本号
        /// </summary>
        string Verison { get; }

        string ModPath { get; }
        string GameSavePath { get; }
        /// <summary>
        /// 所有使用中的Windows
        /// </summary>
        List<WindowsPageHandle> AllWindows { get; }

        /// <summary>
        /// 刷新时间时会调用该方法
        /// </summary>
        event TimeRels TimeHandle;
        /// <summary>
        /// 刷新时间时会调用该方法,在所有任务处理完之后
        /// </summary>
        event TimeRels TimeUIHandle;

        void MoveTime(TimeSpan span);

        /// <summary>
        /// 游戏暂停
        /// </summary>
        void TimeStop();
        /// <summary>
        /// 游戏继续1倍数
        /// </summary>
        void TimeStart();
        /// <summary>
        /// 游戏继续5倍数
        /// </summary>
        void TimeSpeed();

        /// <summary>
        /// 设置置顶窗口
        /// </summary>
        /// <param name="win">窗口</param>
        void Toppext(IWindows win);

        /// <summary>
        /// 移除侧边标签
        /// </summary>
        /// <param name="tag">标签</param>
        void RemoveIMCTag(IMCTag tag);
        /// <summary>
        /// 弹出消息窗口 请注意该窗口不会阻塞任何线程, 如需结束后调用,请使用ENDAction
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="text">消息文本</param>
        /// <param name="type">消息类型: 显示的图标和声音提示将会不同</param>
        /// <param name="YesNo">是否使用 YesNo窗体</param>
        /// <param name="ENDAction">结束事件</param>
        /// <param name="TextCenter">是否字体居中显示</param>
        /// <param name="CanHide">是否允许隐藏消息弹窗</param>
        void winMessageBoxShow(string title, string text, MSGType type = Function.MSGType.notify, bool YesNo = false, Action<bool?> ENDAction = null, bool TextCenter = true, bool CanHide = false);
        /// <summary>
        /// 弹出图片显示窗口 请注意该窗口不会阻塞任何线程, 如需结束后调用,请使用ENDAction
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="image">消息图片</param>
        /// <param name="type">消息类型: 显示的图标和声音提示将会不同</param>
        /// <param name="ENDAction">结束事件</param>
        /// <param name="AllowMax">是否允许最大化</param>
        /// <param name="CanHide">是否允许隐藏消息弹窗</param>
        void winImageBoxShow(string title, ImageSource image, Function.MSGType type = Function.MSGType.notify, Action ENDAction = null, bool CanHide = true, bool AllowMax = true);
        /// <summary>
        /// 弹出图片显示窗口 请注意该窗口不会阻塞任何线程, 如需结束后调用,请使用ENDAction
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="image">消息图片</param>
        /// <param name="type">消息类型: 显示的图标和声音提示将会不同</param>
        /// <param name="ENDAction">结束事件</param>
        /// <param name="AllowMax">是否允许最大化</param>
        /// <param name="CanHide">是否允许隐藏消息弹窗</param>
        void winImageBoxShow(string title, UIElement image, Function.MSGType type = Function.MSGType.notify, Action ENDAction = null, bool CanHide = true, bool AllowMax = true);

        /// <summary>
        /// 创建一个侧边日历栏组件
        /// </summary>
        /// <param name="ev">事件</param>
        /// <returns>侧边栏组件</returns>
        IMCTag CreateCALTag(Event ev);

        /// <summary>
        /// 创建一个侧边消息栏组件
        /// </summary>
        /// <param name="ev">事件</param>
        /// <returns>侧边栏组件</returns>
        IMCTag CreateMSGTag(Event ev);

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="Handle">窗体内控件</param>
        /// <param name="title">窗口标题</param>
        /// <param name="icon">窗口图标</param>
        /// <param name="StoreSize">在设置中储存用户修改界面的大小</param>
        /// <returns>窗体信息</returns>
        IWindows ShowWindows(WindowsPageHandle Handle, string title, Uri icon,bool StoreSize);
        /// <summary>
        /// 显示软件 (可用于游戏内置软件/MOD软件)
        /// </summary>
        /// <param name="SoftWare">软件名称</param>
        /// <param name="args">参数</param>
        /// <returns>该软件的窗体控件</returns>
        WindowsPageHandle ShowSoftWare(string SoftWare, string args);
    }
}

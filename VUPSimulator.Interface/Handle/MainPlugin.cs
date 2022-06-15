﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUPSimulator.Interface.Handle
{
    /// <summary>
    /// 这是插件的主体内容 请继承这个类
    /// </summary>
    public abstract class MainPlugin
    {
        /// <summary>
        /// 主窗体, 主程序提供的各种功能和设置等 大部分参数和调用均在这里
        /// </summary>
        public IMainWindow MW;
        /// <summary>
        /// MOD插件初始化
        /// </summary>
        /// <param name="mainwin">主窗体</param>
        /// 请不要加载游戏和玩家数据,仅用作初始化
        /// 加载游戏,请使用 Start
        public MainPlugin(IMainWindow mainwin)
        {
            //此处主窗体玩家等信息为空,请不要加载游戏和玩家数据
            MW = mainwin;
            //例如, 添加窗体至主程序
            //MW.Core.SoftWares.Add(ISoftWare);
        }
        /// <summary>
        /// 游戏开始 (可以读取Save存档)
        /// </summary>
        public abstract Task StartGame();

        /// <summary>
        /// 游戏结束 (可以保存或清空等,不过保存有专门的Save())
        /// </summary>
        public abstract Task EndGame();

        /// <summary>
        /// 储存游戏 (可以写Save.other储存设置和数据等)
        /// </summary>
        public abstract Task Save();
    }
}

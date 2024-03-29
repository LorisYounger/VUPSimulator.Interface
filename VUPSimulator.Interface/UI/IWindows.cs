﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// UI显示窗体 会自动生成外边框和底部栏
    /// </summary>
    public interface IWindows
    {
        Visibility Visibility { get; set; }
        IMainWindow IMW { get; }

        /// <summary>
        /// 执行关闭窗口
        /// </summary>
        void Close();
        /// <summary>
        /// 关闭窗口 无确认
        /// </summary>
        void CloseForce();

        /// <summary>
        /// 居中窗口
        /// </summary>
        void CenterScreen();

        /// <summary>
        /// 软件标题
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 设置图标
        /// </summary>
        /// new Uri($"pack://application:,,,/images/my.jpg")
        /// application->应用内
        /// siteoforigin->应用外
        Uri Icon { set; }

        /// <summary>
        /// 当关闭文件后进行操作
        /// </summary>
        event Action DeActive;

        /// <summary>
        /// 修改窗口大小状态
        /// </summary>
        void ChangeMax();

        /// <summary>
        /// 当前窗体是否为置顶窗体
        /// </summary>
        public bool NowTop { get; set; }
        /// <summary>
        /// 更新 修改窗口大小状态
        /// </summary>
        public void SetThumb();
    }
}

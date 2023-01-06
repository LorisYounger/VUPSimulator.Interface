using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// UI显示桌面控件 会自动生成可修改大小的控件和倍率调整器
    /// </summary>
    public interface IWidget
    {
        /// <summary>
        /// 缩放倍率比率
        /// </summary>
        double ZoomSize { get; set; }
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
        /// 更新 修改控件大小状态
        /// </summary>
        void SetThumb();
        /// <summary>
        /// 设置当前widget为顶层
        /// </summary>
        void SetTop();
        /// <summary>
        /// 设置当前widget为底层
        /// </summary>
        void SetBotton();
        /// <summary>
        /// 控件透明度
        /// </summary>
        double Opacity { get; set; }
        /// <summary>
        /// 允许移动设置
        /// </summary>
        bool AllowMove { get; set; }
        /// <summary>
        /// 允许修改大小
        /// </summary>
        bool AllowChangeSize { get; set; }
    }
}

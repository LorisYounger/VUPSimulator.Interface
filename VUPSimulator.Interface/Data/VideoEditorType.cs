using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;
namespace VUPSimulator.Interface
{
    /// <summary>
    /// 视频编辑选项类
    /// </summary>
    public class VideoEditorType : Line, IComparable, IComparable<VideoEditorType>
    {
        public VideoEditorType(ILine line) : base(line) { }

        /// <summary>
        /// 显示的文本
        /// </summary>
        public string Content => Info;
        /// <summary>
        /// 显示的注释
        /// </summary>
        public string Tips => GetString("tips");
        /// <summary>
        /// 全称注释
        /// </summary>
        /// <param name="mw">主窗口</param>
        public string FullTips(IMainWindow mw)
        {
            string wp = WhyPass(mw);
            return Tips + (wp == null ? "" : ('\n' + wp));
        }
        /// <summary>
        /// 判断该Type前置条件是否满足,是Enabled的一部分
        /// </summary>
        /// <param name="mw">主窗口</param>
        /// <returns>True:可以运行该事件</returns>
        public bool StartDecied(IMainWindow mw) => Function.Cal.DataEnable(mw, this);
        /// <summary>
        /// 判断该Type前置条件是否满足,是Enabled的一部分
        /// </summary>
        /// <param name="mw">主窗口</param>
        public string WhyPass(IMainWindow mw) => Function.Cal.DataEnableString(mw, this);

        int sort = int.MinValue;
        public new int CompareTo(object obj)
        {
            return Sort.CompareTo(((VideoEditorType)obj).Sort);
        }
        public int CompareTo(VideoEditorType other)
        {
            return Sort.CompareTo(other.Sort);
        }

        /// <summary>
        /// 所需要的时间 (剪辑前x(倍率))
        /// </summary>
        public double TimeUseBefore => GetDouble("timeusebefore");
        /// <summary>
        /// 所需要的时间 (剪辑后x(倍率)) 
        /// </summary>
        public double TimeUseAfter => GetDouble("timeuseafter");
        /// <summary>
        /// 所需要的时间 (固定值,单位:分钟) 
        /// </summary>
        public double TimeUse => GetDouble("timeuse");
        /// <summary>
        /// 排序顺序
        /// </summary>
        public int Sort
        {
            get
            {
                if (sort == int.MinValue)
                    sort = GetInt("sort", 500);
                return sort;
            }
        }
        /// <summary>
        /// 所属分类 long/short/effect/other
        /// </summary>
        public string Type => GetString("type", "other");

        /// <summary>
        /// 质量乘数:其他 0.01-0.99/1.00+
        /// </summary>
        public double Quality
        {
            get => this[(gdbe)"quality"];
            set => this[(gdbe)"quality"] = value;
        }
        /// <summary>
        /// 质量乘数:视频录制 0.01-0.99/1.00+
        /// </summary>
        public double QualityVideo
        {
            get => this[(gdbe)"qualityvideo"];
            set => this[(gdbe)"qualityvideo"] = value;
        }
        /// <summary>
        /// 质量乘数:声音 0.01-0.99/1.00+
        /// </summary>
        public double QualityVoice
        {
            get => this[(gdbe)"qualityvoice"];
            set => this[(gdbe)"qualityvoice"] = value;
        }
        /// <summary>
        /// 质量乘数:有趣 0.01-0.99/1.00+
        /// </summary>
        public double QualityFun
        {
            get => this[(gdbe)"qualityfun"];
            set => this[(gdbe)"qualityfun"] = value;
        }
        /// <summary>
        /// 时长乘数:有趣 0.01-0.99/1.00+
        /// </summary>
        public double Length
        {
            get => this[(gdbe)"length"];
            set => this[(gdbe)"length"] = value;
        }
        /// <summary>
        /// 视频渲染时长Buff 用于计算所需渲染时间
        /// </summary>
        public double RenderBuff
        {
            get => GetDouble("renderbuff", 1);
            set => this[(gdbe)"renderbuff"] = value;
        }
    }
}

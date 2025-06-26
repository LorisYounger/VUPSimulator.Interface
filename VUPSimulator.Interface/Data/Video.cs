using LinePutScript;
using LinePutScript.Localization.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 玩家录制的视频文件
    /// </summary>
    public class Video : Line
    {
        public Video(ILine line) : base(line) { }
        public Video() { Name = "video"; }

        /// <summary>
        /// 录制时间
        /// </summary>
        public DateTime RecordDate
        {
            get => this[(gdat)"recorddate"];
            set => this[(gdat)"recorddate"] = value;
        }
        /// <summary>
        /// 录制时长
        /// </summary>
        public TimeSpan TimeLength
        {
            get => new TimeSpan(this[(gi64)"timelength"]);
            set => this[(gi64)"timelength"] = value.Ticks;
        }
        /// <summary>
        /// 录制码率 单位kbps
        /// </summary>
        public int BitRate
        {
            get => this[(gint)"bitrate"];
            set => this[(gint)"bitrate"] = value;
        }

        /// <summary>
        /// 视频分辨率
        /// </summary>
        public enum VideoResolution
        {
            r640x360 = 4,
            r768x432 = 6,
            r1096x616 = 7,
            r1280x720 = 8,
            r1440x810 = 9,
            r1920x1080 = 10,
            r2560x1440 = 12,
            r3620x2036 = 14
        }

        /// <summary>
        /// 视频分辨率
        /// </summary>
        public VideoResolution Resolution
        {
            get => (VideoResolution)this[(gint)"resolution"];
            set => this[(gint)"resolution"] = (int)value;
        }

        public enum Type
        {
            /// <summary>
            /// 杂谈视频 (一般为口才说话自我介绍等)
            /// </summary>
            Base,
            /// <summary>
            /// 游戏视频 (录制游戏实况)
            /// </summary>
            Game,
            /// <summary>
            /// 绘画视频
            /// </summary>
            Draw,
            /// <summary>
            /// 编程视频
            /// </summary>
            Program,
            /// <summary>
            /// 唱歌视频
            /// </summary>
            Song,
        }
        /// <summary>
        /// 视频类型
        /// </summary>
        public Type VideoType
        {
            get => (Type)this[(gint)"type"];
            set => this[(gint)"type"] = (int)value;
        }

        /// <summary>
        /// 视频大小,未来会做硬件硬盘功能
        /// </summary>
        public int Size => (int)(BitRate * TimeLength.TotalSeconds / 10000);
        /// <summary>
        /// 是否在剪辑状态
        /// </summary>
        public bool IsEdit
        {
            get => this[(gbol)"isedit"];
            set => this[(gbol)"isedit"] = value;
        }
        /// <summary>
        /// 是否在渲染状态
        /// </summary>
        public bool IsRender
        {
            get => this[(gbol)"isrender"];
            set => this[(gbol)"isrender"] = value;
        }
        /// <summary>
        /// 是否随时可以提交视频
        /// </summary>
        public bool IsFinish
        {
            get => this[(gbol)"isfinish"];
            set => this[(gbol)"isfinish"] = value;
        }
        ///// <summary>
        ///// 是否随时可以提交视频//上传视频完成后会自动删除旧视频,无需使用此属性
        ///// </summary>
        //public bool IsPublish
        //{
        //    get => this[(gbol)"ispublish"];
        //    set => this[(gbol)"ispublish"] = value;
        //}

        //总质量计算: 质量/时长
        //误区:视频质量是可以大于1
        //视频质量 1点约等于 10k播放量
        /// <summary>
        /// 视频质量:其他
        /// </summary>
        /// 如果是游戏,就是游戏的游玩评分
        /// 占比50%
        public double Quality
        {
            get => this[(gflt)"quality"];
            set => this[(gflt)"quality"] = value;
        }
        /// <summary>
        /// 视频质量:视频录制
        /// </summary>
        public double QualityVideo
        {
            get => this[(gflt)"qualityvideo"];
            set => this[(gflt)"qualityvideo"] = value;
        }
        /// <summary>
        /// 视频质量:声音
        /// </summary>
        public double QualityVoice
        {
            get => this[(gflt)"qualityvoice"];
            set => this[(gflt)"qualityvoice"] = value;
        }
        /// <summary>
        /// 视频质量:有趣
        /// </summary>
        public double QualityFun
        {
            get => this[(gflt)"qualityfun"];
            set => this[(gflt)"qualityfun"] = value;
        }
        /// <summary>
        /// 总质量
        /// </summary>
        public double TotalQuality => (Quality * .5 + QualityVideo * .3 + QualityVoice * .2 + QualityFun * .3) / TimeLength.TotalHours;

        /// <summary>
        /// 预计时长
        /// </summary>
        public TimeSpan ETimeLength
        {
            get => new TimeSpan(GetInt64("etimelength", GetInt64("timelength")));
            set => this[(gi64)"etimelength"] = value.Ticks;
        }
        /// <summary>
        /// 预计视频质量:其他
        /// </summary>
        public double EQuality
        {
            get => GetFloat("equality", Quality);
            set => this[(gflt)"equality"] = value;
        }
        /// <summary>
        /// 预计视频质量:视频录制
        /// </summary>
        public double EQualityVideo
        {
            get => GetFloat("equalityvideo", QualityVideo);
            set => this[(gflt)"equalityvideo"] = value;
        }
        /// <summary>
        /// 预计视频质量:声音
        /// </summary>
        public double EQualityVoice
        {
            get => this[(gflt)"equalityvoice"];
            set => this[(gflt)"equalityvoice"] = value;
        }
        /// <summary>
        /// 预计视频质量:有趣
        /// </summary>
        public double EQualityFun
        {
            get => this[(gflt)"equalityfun"];
            set => this[(gflt)"equalityfun"] = value;
        }
        /// <summary>
        /// 预计总质量
        /// </summary>
        public double ETotalQuality => (EQuality * .5 + EQualityVideo * .3 + EQualityVoice * .2 + EQualityFun * .3) / ETimeLength.TotalHours;
        /// <summary>
        /// 截图图片名字
        /// </summary>
        public string ImageName
        {
            get
            {
                string v = this[(gstr)"image"];
                if (v != null)
                    return v;
                v = Function.Rnd.Next().ToString("x");
                this[(gstr)"image"] = v;
                return v;
            }
            set => this[(gstr)"image"] = value;
        }
        public string ImagePATH(IMainWindow mw)
        {
            var Path = mw.GameSavePath + '\\' + mw.Save.Base.UserName + "\\video_" + ImageName + ".png";
            if (File.Exists(Path))
                return Path;
            return mw.Core.ImageSources.FindSource("bg_base", "pack://application:,,,/Res/Image/system/error.png");
        }
        public BitmapImage Image(IMainWindow mw) => new BitmapImage(new Uri(ImagePATH(mw)));
        /// <summary>
        /// 保存图像
        /// </summary>
        public void SaveImage(IMainWindow mw, RenderTargetBitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(ms);
                File.WriteAllBytes(ImagePATH(mw), ms.ToArray());
            }
        }
        /// <summary>
        /// 视频名称 (默认为日期)
        /// </summary>
        public string VideoName
        {
            get => GetString("name", RecordDate.ToShortDateString());
            set => this[(gstr)"name"] = value;
        }
        /// <summary>
        /// 视频编辑总时长 小时数
        /// </summary>
        public double EditTotalHour
        {
            get => this[(gflt)"edittotalhour"];
            set => this[(gflt)"edittotalhour"] = value;
        }
        /// <summary>
        /// 视频编辑进度 小时数
        /// </summary>
        public double EditProgress
        {
            get => this[(gflt)"editprocess"];
            set => this[(gflt)"editprocess"] = value;
        }
        /// <summary>
        /// 视频渲染进度 单位GHZ
        /// </summary>
        public double RenderingProgress
        {
            get => this[(gflt)"renderprocess"];
            set => this[(gflt)"renderprocess"] = value;
        }
        ///// <summary>
        ///// 视频渲染Buff 用于计算所需渲染时间
        ///// </summary>
        //public double RenderBuff
        //{
        //    get => GetFloat("renderbuff", 1);
        //    set => this[(gflt)"renderbuff"] = value;
        //}
        /// <summary>
        /// 视频渲染所需进度
        /// </summary>
        public double RenderingTask
        {
            get => this[(gflt)"rendertask"];
            set => this[(gflt)"rendertask"] = value;
        }
        /// <summary>
        /// 视频渲染进度百分比
        /// </summary>
        public double RenderingProcessP
        {
            get => RenderingProgress / RenderingTask;
        }
        /// <summary>
        /// 视频渲染顺序
        /// </summary>
        public double RenderID
        {
            get => this[(gflt)"renderid"];
            set => this[(gflt)"renderid"] = value;
        }
        /// <summary>
        /// 视频渲染状态
        /// </summary>
        public string RenderState
        {
            get => this[(gstr)"renderstate"];
            set => this[(gstr)"renderstate"] = value;
        }
        /// <summary>
        /// 视频内容
        /// </summary>
        public string Content
        {
            get
            {
                switch (VideoType)
                {
                    case Type.Game:
                        return "游戏".Translate() + ' ' + GetString("game", "未知".Translate());
                    default:
                        return "未知".Translate();
                }
            }
        }
        /// <summary>
        /// 视频标签,一般是OBS自动生成
        /// </summary>
        public string[] Tag
        {
            get => this["tag"].GetInfos();
            set => this["tag"].info = string.Join(",", value);
        }
        /// <summary>
        /// 视频增益,由OBS自动生成
        /// </summary>
        public double Buff
        {
            get => GetFloat("buff", 1);
            set => this[(gflt)"buff"] = value;
        }
    }
}

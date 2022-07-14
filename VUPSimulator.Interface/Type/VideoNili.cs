using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LinePutScript;
using static VUPSimulator.Interface.Video;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// NiliNili视频
    /// </summary>
    public class VideoNili : Line
    {
        public VideoNili(Line line) : base(line) { Name = "nilivideo"; }
        public VideoNili(Video video)
        {
            Name = "nilivideo";
            VideoName = video.VideoName;
            TimeLength = video.TimeLength;
            BitRate = video.BitRate;
            Resolution = video.Resolution;
            VideoType = video.VideoType;
            Quality = video.Quality;
            QualityVideo = video.QualityVideo;
            QualityVoice = video.QualityVoice;
            QualityFun = video.QualityFun;
            ImageName = video.ImageName;
            Content = video.Content;
            
        }
        /// <summary>
        /// 视频名称
        /// </summary>
        public string VideoName
        {
            get => GetString("name", "");
            set => this[(gstr)"name"] = value;
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
        public VideoResolution Resolution
        {
            get => (VideoResolution)this[(gint)"resolution"];
            set => this[(gint)"resolution"] = (int)value;
        }
        /// <summary>
        /// 视频类型
        /// </summary>
        public Video.Type VideoType
        {
            get => (Video.Type)this[(gint)"type"];
            set => this[(gint)"type"] = (int)value;
        }
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
        /// 图片名
        /// </summary>
        public string ImageName
        {
            get
            {
                return this[(gstr)"image"];
            }
            set => this[(gstr)"image"] = value;
        }
        public string ImagePATH(IMainWindow mw)
        {
            string img = ImageName;
            if (img == null)
                return mw.Core.ImageSources.FindSource("software_NiliVideo", "pack://application:,,,/Res/Image/system/error.png");
            if (img.StartsWith("nilivideo_"))
                return mw.Core.ImageSources.FindSource(img, "pack://application:,,,/Res/Image/system/error.png");
            return mw.GameSavePath + '\\' + mw.Save.UserName + "\\video_" + ImageName + ".png";
        }
        public BitmapImage Image(IMainWindow mw) => new BitmapImage(new Uri(ImagePATH(mw)));

        /// <summary>
        /// 视频内容
        /// </summary>
        public string Content
        {
            get
            {
                string v = this[(gstr)"content"];
                if (v != null)
                    return v;
                return "无内容描述";
            }
            set => this[(gstr)"content"] = value;
        }

        // 视频评论相关








        /// <summary>
        /// 播放数量
        /// </summary>
        public int PlayCount
        {
            get => this[(gint)"playcount"];
            set => this[(gint)"playcount"] = value;
        }
        /// <summary>
        /// 播放表 用于图标展示 1=1天
        /// </summary>
        public List<int> PlayGraph
        {
            get
            {
                List<int> ints = new List<int>();
                foreach (var str in FindorAdd("playgraph").GetInfos())
                    ints.Add(Convert.ToInt32(str));
                return ints;
            }
            set => FindorAdd("playgraph").info = string.Join(",", value);
        }
        /// <summary>
        /// 计算今天可能获得的播放数量
        /// </summary>//TODO
        public int PlayCalDay() => 0;
        /// <summary>
        /// 点赞数量(基本) + likeuser.count
        /// </summary>
        public int LikeCount
        {
            get => this[(gint)"likecount"];
            set => this[(gint)"likecount"] = value;
        }
        /// <summary>
        /// 点赞人员 将来可以作为伪联机和好友互动?
        /// </summary>
        public List<string> LikeUser
        {
            get => FindorAdd("likeuser").GetInfos().ToList();
            set => FindorAdd("likeuser").info = string.Join(",", value);
        }
        /// <summary>
        /// 点赞表 用于图标展示 1=1天
        /// </summary>
        public List<int> LikeGraph
        {
            get
            {
                List<int> ints = new List<int>();
                foreach (var str in FindorAdd("likegraph").GetInfos())
                    ints.Add(Convert.ToInt32(str));
                return ints;
            }
            set => FindorAdd("likegraph").info = string.Join(",", value);
        }
        /// <summary>
        /// 总点赞数量
        /// </summary>
        public int LikeTotal => LikeUser.Count + LikeCount;
        /// <summary>
        /// 计算今天可能获得的播放量
        /// </summary>//TODO
        public int LikeCalDay() => 0;
        /// <summary>
        /// 收藏数量(基本) + likeuser.count
        /// </summary>
        public int StartCount
        {
            get => this[(gint)"startcount"];
            set => this[(gint)"startcount"] = value;
        }
        /// <summary>
        /// 收藏人员 将来可以作为伪联机和好友互动?
        /// </summary>
        public List<string> StartUser
        {
            get => FindorAdd("startuser").GetInfos().ToList();
            set => FindorAdd("startuser").info = string.Join(",", value);
        }
        /// <summary>
        /// 收藏总数
        /// </summary>
        public int StartTotal => StartTotal + StartUser.Count;

        /// <summary>
        /// 收藏表 用于图标展示 1=1天
        /// </summary>
        public List<int> StartGraph
        {
            get
            {
                List<int> ints = new List<int>();
                foreach (var str in FindorAdd("startgraph").GetInfos())
                    ints.Add(Convert.ToInt32(str));
                return ints;
            }
            set => FindorAdd("startgraph").info = string.Join(",", value);
        }
        /// <summary>
        /// 计算今天可能获得的收藏
        /// </summary>//TODO
        public int StartCalDay() => 0;
        
        /// <summary>
        /// 计算今天可能获得的粉丝数量
        /// </summary>//TODO
        public int FansCalDay() => 0;

        /// <summary>
        /// 收入表 用于图标展示 1=1天
        /// </summary>
        public List<int> IncomeGraph
        {
            get
            {
                List<int> ints = new List<int>();
                foreach (var str in FindorAdd("incomegraph").GetInfos())
                    ints.Add(Convert.ToInt32(str));
                return ints;
            }
            set => FindorAdd("incomegraph").info = string.Join(",", value);
        }
        /// <summary>
        /// 计算今天可能获得的收入
        /// </summary>//TODO
        public int IncomeCalDay() => 0;
    }
}

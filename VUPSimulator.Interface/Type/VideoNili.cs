using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LinePutScript;
using static VUPSimulator.Interface.Comment_base;
using static VUPSimulator.Interface.Video;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// NiliNili视频
    /// </summary>
    public class VideoNili : Line
    {
        public static VideoNili Create(Line line, IMainWindow mw, int nowtime)
        {
            var video = new VideoNili(line);

            if (!video.Have("author")) //如果没有作者则随机拉一个
            {
                var tag = video.Tags_str();
                //给tag添加类型元素
                tag.Add(video.VideoType.ToString());
                int chs = mw.Core.Users.Count / 10;
                var v = mw.Core.Users.FindAll(x =>
                {
                    if (!x.UsersType.Contains("up"))
                        return false;
                    string[] xt = x.VideoTag;
                    if (xt != null)
                    {
                        if (xt.Contains("rnd") && Function.Rnd.Next(chs) == 0)
                            return true;
                        foreach (var t in tag)
                        {
                            if (xt.Contains(t))//包含选定条件,通过
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                });
                if (v.Count == 0)
                    video.Author = mw.Core.Users[mw.Core.Users.Count / 10].UserName;
                else
                    video.Author = v[Function.Rnd.Next(v.Count)].UserName;
            }
            video.PublishDate = nowtime;// Function.Rnd.Next(-365, mw.Save.DayTimePass);


            //如果没有封面就自动生成一个


            return video;
        }
        public VideoNili(Line line) : base(line) { Name = "nilivideo"; }
        /// <summary>
        /// 发布视频到nilinili
        /// </summary>
        /// <param name="video">视频</param>
        /// <param name="nowtime">发布时间</param>
        public VideoNili(Video video, int nowtime)
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
            PublishDate = nowtime;
            Author = "_";
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
        /// 视频发布时间: 相对于游玩时间
        /// </summary>
        public int PublishDate
        {
            get => GetInt("publishdate", 0);
            set => this[(gint)"publishdate"] = value;
        }
        public DateTime PublishDateTime(IMainWindow mw) => mw.Save.StartGameTime.AddDays(PublishDate);
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
        public double TotalQuality
        {
            get
            {
                if (totalquality == null)
                    totalquality = (Quality * .5 + QualityVideo * .3 + QualityVoice * .2 + QualityFun * .3) / TimeLength.TotalHours;
                return totalquality.Value;
            }
        }
        private double? totalquality;
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
        public UIElement ToViewUI(IMainWindow mw)
        {
            string img = ImageName;
            if (img == null)
            {
                //根据自动配置开始自动生成一个

                
            }
            if (img.StartsWith("gui_"))
            {
                var b = mw.Save.GenBases.Find(x => x.Name == img);
                if (b == null)
                    return new Image()
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Res/Image/system/error.png")),
                        Stretch = Stretch.UniformToFill,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                else
                    return b.Create(mw);
            }
            else
            {
                string path;
                if (img.StartsWith("nilivideo_"))
                    path = mw.Core.ImageSources.FindSource(img, "pack://application:,,,/Res/Image/system/error.png");
                else
                    path = mw.GameSavePath + '\\' + mw.Save.UserName + "\\video_" + ImageName + ".png";
                return new Image()
                {
                    Source = new BitmapImage(new Uri(path)),
                    Stretch = Stretch.UniformToFill,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
        }

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
        /// <summary>
        /// 视频作者
        /// </summary>
        public string Author
        {
            get
            {
                string v = this[(gstr)"author"];
                if (v != null)
                    return v;
                return "匿名";
            }
            set => this[(gstr)"author"] = value;
        }
        public UserNili AuthorNili(IMainWindow mw)
        {
            if (Author == "_")
                return mw.Save.UserNili;
            else
            {
                var auth = Author;
                return new UserNili(mw.Core.Users.Find(x => x.UserName == auth));
            }
        }
        /// <summary>
        /// 视频标签
        /// </summary>
        public List<string> Tags_str()
        {
            var ls = new List<string>();
            foreach (CommentTag tag in Tags)
                ls.Add(tag.ToString().ToLower());
            return ls;
        }
        /// <summary>
        /// 评论标签
        /// </summary>
        public List<CommentTag> Tags
        {
            get
            {
                Sub subtag = Find("tag");
                if (subtag == null)
                    return new List<CommentTag>();
                List<CommentTag> tags = new List<CommentTag>();
                foreach (string tag in subtag.GetInfos())
                    tags.Add((CommentTag)Enum.Parse(typeof(CommentTag), tag, true));
                if (Quality < 30)
                    tags.Add(CommentTag.LowQuality);
                else if (Quality > 70)
                    tags.Add(CommentTag.HighQuality);
                return tags;
            }
        }
        // TODO:视频评论相关








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
        /// 计算今天可能获得的播放数量(固定,需要自己手动加个随机)
        /// =INT(($B$3+$B$11)*(POWER(E3,0.8)/10+100)*((10+$B$11*10)/(A3+10))*(2/SQRT($B$13+1)))
        /// </summary>
        public double PlayCalDay(IMainWindow mw)
        {
            //开始计算
            double qf = QualityFun / TimeLength.TotalHours;
            return (TotalQuality + qf) * (Math.Pow(AuthorNili(mw).TotalFans, 0.8) / 10 + 100) * ((10 + qf * 10) / (mw.Save.DayTimePass - PublishDate + 10)) * (2 / Math.Sqrt(TimeLength.TotalHours + 1));
        }

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
        /// 计算今天可能获得的点赞
        /// =INT(($B$3+$B$5+$B$7)*(POWER(E3,0.8)/100+10)*((5+($B$5+$B$7)*5)/(A3+10)))
        /// </summary>
        public double LikeCalDay(IMainWindow mw)
        {
            //开始计算
            double qf = (Quality + QualityVideo) / TimeLength.TotalHours;
            return (TotalQuality + qf) * (Math.Pow(AuthorNili(mw).TotalFans, 0.8) / 100 + 10) * ((5 + qf * 5) / (mw.Save.DayTimePass - PublishDate + 10));
        }
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
        /// =IF(INT(($B$3+$B$9+$B$11)*(POWER(E3,0.8)/100+10)*((5+($B$9+$B$11)*5)/(A3+20))*(SQRT($B$13+0.5)))>C3,C3,INT(($B$3+$B$9+$B$11)*(POWER(E3,0.8)/100+10)*((5+($B$9+$B$11)*5)/(A3+20))*(SQRT($B$13+0.5))))
        /// </summary>
        public double StartCalDay(IMainWindow mw, double playcalday)
        {
            //开始计算
            double qf = (QualityVoice + QualityFun) / TimeLength.TotalHours;
            double ans = (TotalQuality + qf) * (Math.Pow(AuthorNili(mw).TotalFans, 0.8) / 100 + 10) * ((5 + qf * 5) / (mw.Save.DayTimePass - PublishDate + 20)) * (Math.Sqrt(TimeLength.TotalHours + 0.5));
            return Math.Min(ans, playcalday);
        }

        /// <summary>
        /// 计算今天可能获得的粉丝数量
        /// =INT(($B$3)*(10+SQRT(E3)/10)*((5+$B$3*5)/(A3+10)))
        /// </summary>
        public double FansCalDay(IMainWindow mw)
        {
            //开始计算
            return TotalQuality * (10 + Math.Sqrt(AuthorNili(mw).TotalFans) / 10) * ((5 + TotalQuality * 5) / (mw.Save.DayTimePass - PublishDate + 10));
        }

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
        /// =C3*0.01+G3*0.1+I3*0.1
        /// </summary>
        public double IncomeCalDay(int playcal, int likecal, int starcal) => playcal * 0.01 + likecal * 0.1 + starcal * 0.1;
    }
}

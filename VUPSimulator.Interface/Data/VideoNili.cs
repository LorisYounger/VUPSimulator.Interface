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
using static VUPSimulator.Interface.Comment;
using static VUPSimulator.Interface.Video;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// NiliNili视频
    /// </summary>
    public class VideoNili : Line
    {
        /// <summary>
        /// 创建全新的随机NILI视频
        /// </summary>
        public static VideoNili Create(IMainWindow mw, int nowtime, Video.Type type, string name, string content, string bg, string[] tags, double buff = 1, double quality = -1, UserNili author = null)
        {
            var video = new VideoNili();
            video.VideoName = name;
            video.BackgroundImage = bg;
            video.Content = content;
            video.Buff = buff;
            video.FindorAdd("tag").info = string.Join(",", tags);
            if (author == null) //如果没有作者则随机拉一个
            {
                int chs = mw.Save.UsersNili.Count / 10;
                var v = mw.Save.UsersNili.FindAll(x =>
                {
                    string[] xt = x.VideoTag;
                    if (xt != null)
                    {
                        if (xt.Contains("rnd") && Function.Rnd.Next(chs) == 0)
                            return true;
                        foreach (var t in video.Tags)
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
                    author = mw.Save.UsersNili[mw.Core.Users.Count / 10];
                else
                    author = v[Function.Rnd.Next(v.Count)];
            }
            video.authnili = author;
            video.Author = author.UserName;

            //根据作者生成质量 =POWER(P3+100,0.15)*0.3-0.2
            if (quality <= 0)
                quality = Math.Pow(video.AuthorNili(mw).TotalFans + 100, 0.15) * 0.3 - 0.2;

            //根据用户标签随机视频质量
            var tag = video.AuthorNili(mw).Tags;
            if (tag.Contains(UserNili.NiliTag.QualityHigh))
                quality *= 1.2;
            else if (tag.Contains(UserNili.NiliTag.QualityLow))
                quality *= 0.8;
            else if (tag.Contains(UserNili.NiliTag.QualityFix))
                quality = 1.2;
            //录制质量
            var vq = quality * Function.Rnd.NextDouble() * 0.2 + quality * 0.8;
            if (tag.Contains(UserNili.NiliTag.ComputerGood))
                vq *= 1.2;
            else if (tag.Contains(UserNili.NiliTag.ComputerBad))
                vq *= 0.8;
            //声音质量
            var vv = quality * Function.Rnd.NextDouble() * 0.2 + quality * 0.8;
            if (tag.Contains(UserNili.NiliTag.VoiceHigh))
                vv *= 1.2;
            else if (tag.Contains(UserNili.NiliTag.VoiceLow))
                vv *= 0.8;
            //有趣
            var vf = quality * Function.Rnd.NextDouble() * 0.2 + quality * 0.8;
            if (tag.Contains(UserNili.NiliTag.FunHigh))
                vf *= 1.2;
            else if (tag.Contains(UserNili.NiliTag.FunLow))
                vf *= 0.8;
            //其他质量
            double vt;
            switch (type)
            {
                case Video.Type.Game:
                    //游戏视频以有趣和录制为主
                    vt = vf * .5 + vq * .5;
                    break;
                case Video.Type.Draw:
                    //绘画视频以声音质量和有趣为主
                    vt = vv * .5 + vf * .5;
                    break;
                case Video.Type.Program:
                    //编程视频以录制质量和声音为主
                    vt = vv * .5 + vq * .5;
                    break;
                case Video.Type.Song:
                    //唱歌视频以声音质量和录制质量为主
                    vt = vv * 0.8 + vq * .2;
                    break;
                default:
                    vt = vq * .3 + vv * .3 + vf * .4;
                    break;
            }
            //vt *= qualitybuff;
            //计算时长
            double tl;
            if (tag.Contains(UserNili.NiliTag.LengthLong))
                tl = Function.Rnd.NextDouble() + 1;
            else if (tag.Contains(UserNili.NiliTag.LengthShort))
                tl = Function.Rnd.NextDouble() * .3 + 0.2;
            else
                tl = Function.Rnd.NextDouble() * .5 + .5;
            video.TimeLength = TimeSpan.FromHours(tl);

            //应用质量设置
            video.Quality = vt * tl;
            video.QualityFun = vf * tl;
            video.QualityVideo = vq * tl;
            video.QualityVoice = vv * tl;

            video.PublishDate = nowtime;// Function.Rnd.Next(-365, mw.Save.DayTimePass);

            //高质量视频再添加创作收益
            video.JoinProfit = false;

            ////刷新时间
            //video.RelsDate(mw);

            return video;
        }

        public VideoNili()
        {
            Name = "nilivideo";
        }
        public VideoNili(Line line) : base(line) { Name = "nilivideo"; }
        /// <summary>
        /// 发布视频到nilinili
        /// </summary>
        /// <param name="video">视频</param>
        /// <param name="nowtime">发布时间</param>
        public VideoNili(Video video, string name, string content, int nowtime, string imgname, string[] tags, bool joinprofit)
        {
            Name = "nilivideo";
            VideoName = name;
            TimeLength = video.TimeLength;
            BitRate = video.BitRate;
            Resolution = video.Resolution;
            Buff = video.Buff;
            VideoType = video.VideoType;
            Quality = video.Quality;
            QualityVideo = video.QualityVideo;
            QualityVoice = video.QualityVoice;
            QualityFun = video.QualityFun;
            ImageName = imgname;
            Content = content;
            PublishDate = nowtime;
            Author = "_";
            JoinProfit = joinprofit;
            FindorAdd("tag").info = string.Join(",", tags);

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
        /// <summary>
        /// 视频结算日期: 是否已结算播放数据
        /// </summary>
        public int SettleDate
        {
            get => GetInt("settledate", PublishDate);
            set => this[(gint)"settledate"] = value;
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
            {//根据自动配置开始自动生成一个
                int gub = Function.Rnd.Next(mw.Core.GenImageTemplates.Count);
                //储存gub
                ImageName = "gub_" + gub;
                var gi = mw.Core.GenImageTemplates[gub];
                var gn = gi.genImageNili(VideoName, "niliusr_" + AuthorNili(mw).UsrImage, BackgroundImage);
                return gn.Create(mw);
            }
            if (img.StartsWith("gui_"))
            {//根据存档的gi图片进行生成,一般用于玩家自定义封面
                var b = mw.Save.GenBases.Find(x => x.info == img);
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
            else if (img.StartsWith("gub_"))
            {//根据核心模板顺序进行生成,一般用于随机生成                
                var gi = mw.Core.GenImageTemplates[Convert.ToInt32(img.Substring(4))];
                var gn = gi.genImageNili(VideoName, "niliusr_" + AuthorNili(mw).UsrImage, BackgroundImage);
                return gn.Create(mw);
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
        /// 视频背景图片
        /// </summary>
        public string BackgroundImage
        {
            get
            {
                string v = this[(gstr)"bg"];
                if (v != null)
                    return v;
                return "bg_base";
            }
            set => this[(gstr)"bg"] = value;
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
                return null;
            }
            set => this[(gstr)"author"] = value;
        }
        public UserNili AuthorNili(IMainWindow mw)
        {
            if (authnili == null)
                if (Author == "_")
                    authnili = mw.Save.UserNili;
                else
                {
                    var auth = Author;
                    authnili = mw.Save.UsersNili.Find(x => x.UserName == auth);
                }
            return authnili;
        }
        private UserNili authnili = null;
        /// <summary>
        /// 视频增益
        /// </summary>
        public double Buff
        {
            get => GetFloat("buff", 1) * (JoinProfit ? 0.5 : 1);
            set => this[(gflt)"buff"] = value;
        }
        /// <summary>
        /// 是否参与了创作收入计划
        /// </summary>
        public bool JoinProfit
        {
            get => GetBool("joinprofit");
            set => SetBool("joinprofit", value);
        }
        /// <summary>
        /// 评论标签
        /// </summary>
        public List<string> Tags
        {
            get
            {
                if (tags == null)
                {
                    Sub subtag = Find("tag");
                    tags = new List<string>();

                    if (TotalQuality < 1)
                        tags.Add("lowquality");
                    else if (TotalQuality > 3)
                        tags.Add("highquality");

                    //给tag添加类型元素
                    tags.Add(VideoType.ToString());

                    if (subtag == null)
                        return tags;
                    foreach (string tag in subtag.GetInfos())
                        tags.Add(tag.ToLower());
                }
                return tags;
            }
            set
            {
                tags = value;
                var t = value.ToList();
                t.Remove("lowquality");
                t.Remove("highquality");
                Sub subtag = FindorAdd("tag");
                subtag.info = string.Join(",", t);
            }
        }
        private List<string> tags;
        // TODO:视频评论相关


        /// <summary>
        /// 刷新播放量和提供增益
        /// </summary>
        /// <param name="mw"></param>
        public void RelsDate(IMainWindow mw)
        {
            bool isplayer = Author.Equals("_");
            var pt = mw.Save.DayTimePass - SettleDate;
            for (int i = 1; i <= pt; i++)
            {
                //播放量
                var pcd = PlayCalDay(mw, i);
                //点赞
                var lcd = LikeCalDay(mw, i);
                //关注
                var fcd = FansCalDay(mw, i);
                //收藏
                var scd = StartCalDay(mw, pcd, i);
                PlayCount += pcd;
                LikeCount += lcd;
                AuthorNili(mw).Fans += fcd;
                FansCount += fcd;
                StartCount += scd;

                //部分统计
                AuthorNili(mw).CountLike += lcd;
                AuthorNili(mw).CountPlay += pcd;
                AuthorNili(mw).CountStar += scd;

                if (isplayer)
                {//如果是玩家,还需要统计图表和收益

                    //收益: 未参加创作收益计划则为0收益
                    var ind = JoinProfit ? IncomeCalDay(pcd, lcd, scd) : 0;

                    ////收益存在Nili里,需要手动提取//修改:放到总收入里
                    //var line = mw.Save.UIData.FindLineInfo("nili");
                    //if (line == null)
                    //{
                    //    line = new Line("uidata", "nili");
                    //    mw.Save.UIData.AddLine(line);
                    //}
                    //line[(gflt)"income"] += ind;
                    mw.Save.UserNili.IncomeNoOut += ind;
                    mw.Save.UserNili.IncomeMonth += ind;
                    mw.Save.UserNili.IncomeTotal += ind;
                    IncomeCount += (long)ind;
                    IncomeYesterday = (int)ind;

                    //图标
                    var pf = mw.Save.DayTimePass - pt;
                    gint pg = (gint)(pf + i).ToString();
                    PlayGraph[pg] = pcd;
                    LikeGraph[pg] = lcd;
                    StartGraph[pg] = scd;
                    IncomeGraph[(gflt)(pf + i).ToString()] = ind;
                    mw.Save.UserNili.LikeGraph[pg] += lcd;
                    mw.Save.UserNili.PlayGraph[pg] += pcd;
                    mw.Save.UserNili.FansGraph[pg] += fcd;
                    mw.Save.UserNili.StartGraph[pg] += scd;
                    mw.Save.UserNili.IncomeGraph[(gflt)(pf + i).ToString()] += ind;
                }
            }
            SettleDate = mw.Save.DayTimePass;
        }





        /// <summary>
        /// 播放数量
        /// </summary>
        public int PlayCount
        {
            get => this[(gint)"playcount"];
            set => this[(gint)"playcount"] = value;
        }
        /// <summary>
        /// 播放表 用于图标展示 int日期:int值
        /// </summary>
        public StringStructure PlayGraph => FindorAdd("playgraph").Infos;
        /// <summary>
        /// 计算今天可能获得的播放数量(固定,需要自己手动加个随机)
        /// =INT(($B$3+$B$11)*(POWER(E3,0.8)/10+100)*((10+$B$11*10)/(A3+10))*(2/SQRT($B$13+1)))
        /// </summary>
        public int PlayCalDay(IMainWindow mw, int time)
        {
            //开始计算
            double qf = QualityFun / TimeLength.TotalHours;
            return (int)(((TotalQuality + qf) * (Math.Pow(AuthorNili(mw).TotalFans, 0.8) / 10 + 100) * ((10 + qf * 10) / (time + 10)) * (2 / Math.Sqrt(TimeLength.TotalHours + 1))) * Buff);
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
        /// 点赞表 用于图标展示 int日期:int值
        /// </summary>
        public StringStructure LikeGraph => FindorAdd("likegraph").Infos;
        /// <summary>
        /// 总点赞数量
        /// </summary>
        public int LikeTotal => LikeUser.Count + LikeCount;
        /// <summary>
        /// 计算今天可能获得的点赞
        /// =INT(($B$3+$B$5+$B$7)*(POWER(E3,0.8)/100+10)*((5+($B$5+$B$7)*5)/(A3+10)))
        /// </summary>
        public int LikeCalDay(IMainWindow mw, int time)
        {
            //开始计算
            double qf = (Quality + QualityVideo) / TimeLength.TotalHours;
            return (int)(((TotalQuality + qf) * (Math.Pow(AuthorNili(mw).TotalFans, 0.8) / 100 + 10) * ((5 + qf * 5) / (time + 10))) * Buff);
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
        public int StartTotal => StartCount + StartUser.Count;

        /// <summary>
        /// 收藏表 用于图标展示 int日期:int值
        /// </summary>
        public StringStructure StartGraph => FindorAdd("startgraph").Infos;
        /// <summary>
        /// 计算今天可能获得的收藏
        /// =IF(INT(($B$3+$B$9+$B$11)*(POWER(E3,0.8)/100+10)*((5+($B$9+$B$11)*5)/(A3+20))*(SQRT($B$13+0.5)))>C3,C3,INT(($B$3+$B$9+$B$11)*(POWER(E3,0.8)/100+10)*((5+($B$9+$B$11)*5)/(A3+20))*(SQRT($B$13+0.5))))
        /// </summary>
        public int StartCalDay(IMainWindow mw, int playcalday, int time)
        {
            //开始计算
            double qf = (QualityVoice + QualityFun) / TimeLength.TotalHours;
            double ans = (TotalQuality + qf) * (Math.Pow(AuthorNili(mw).TotalFans, 0.8) / 100 + 10) * ((5 + qf * 5) / (time + 20)) * (Math.Sqrt(TimeLength.TotalHours + 0.5)) * Buff;
            return Math.Min((int)ans, playcalday);
        }

        /// <summary>
        /// 计算今天可能获得的粉丝数量
        /// =INT(($B$3+1)*((20+$B$3*20)/(A3+3))*$B$15)
        /// </summary>
        public int FansCalDay(IMainWindow mw, int time)
        {
            //开始计算
            //=INT(($B$3)*(10+SQRT(E3)/10)*((5+$B$3*5)/(A3+10))) 旧版本 涨粉太快了
            //return (int)(TotalQuality * (10 + Math.Sqrt(AuthorNili(mw).TotalFans) / 10) * ((5 + TotalQuality * 5) / (time + 10)) * Buff);
            return (int)((TotalQuality + 1) * ((20 + TotalQuality * 20) / (time + 3)) * Buff);
        }
        /// <summary>
        /// 涨粉粉丝数量
        /// </summary>
        public int FansCount
        {
            get => this[(gint)"fancount"];
            set => this[(gint)"fancount"] = value;
        }
        /// <summary>
        /// 收入表 用于图标展示 int日期:int值
        /// </summary>
        public StringStructure IncomeGraph => FindorAdd("incomegraph").Infos;
        /// <summary>
        /// 总收入
        /// </summary>
        public long IncomeCount
        {
            get => this[(gi64)"incomecount"];
            set => this[(gi64)"incomecount"] = value;
        }
        /// <summary>
        /// 昨日收入
        /// </summary>
        public int IncomeYesterday
        {
            get => this[(gint)"incomeyesterday"];
            set => this[(gint)"incomeyesterday"] = value;
        }
        /// <summary>
        /// 计算今天可能获得的收入
        /// =C3*0.01+G3*0.1+I3*0.1
        /// </summary>
        public double IncomeCalDay(int playcal, int likecal, int starcal) => playcal * 0.01 + likecal * 0.1 + starcal * 0.1;
    }
}

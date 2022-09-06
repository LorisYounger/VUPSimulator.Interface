using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;
using TextToDocument;
using System.Windows.Documents;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 游戏类
    /// </summary>
    public class Item_Game_base : LpsDocument
    {
        /// <summary>
        /// 游戏数据
        /// </summary>
        public Item_Game Data;
        /// <summary>
        /// 创建新游戏类
        /// </summary>       
        public Item_Game_base(string lps, List<Comment_Game> comms, Line igame = null) : base(lps)
        {
            if (igame == null)
            {
                Data = new Item_Game(new Line("game", Name));
                if (Function.Rnd.Next(2) == 0)
                {
                    Data.Discount = Discount;
                }
                else
                {
                    Data.Discount = 100;
                }

                Data.Popularity = Popularity;
                Data.Rating = Rating;
            }
            else
                Data = new Item_Game(igame);
            Comments = comms;
        }
        /// <summary>
        /// 游戏名称 不必是全英文
        /// </summary>
        public string Game
        {
            get
            {
                var line = First()["name"];
                if (line == null)
                {
                    return First().info;
                }
                return line.Info;
            }
        }
        /// <summary>
        /// 获得游戏全名
        /// </summary>
        public string FullName
        {
            get
            {
                if (Name == Game)
                    return Name;
                else
                    return Game + ' ' + Name;
            }
        }
        /// <summary>
        /// 游戏名称
        /// </summary>
        public string Name => First().Info;
        /// <summary>
        /// 游戏内图片 无Logo
        /// </summary>
        public string Image_Blank
        {
            get
            {
                var line = FindLine("image").Find("blank");
                if (line == null)
                {
                    return FindLine("image").info + "_blank";
                }
                return line.info;
            }
        }
        /// <summary>
        /// 库标图
        /// </summary>
        public string Image_Head
        {
            get
            {
                var line = FindLine("image").Find("head");
                if (line == null)
                {
                    return FindLine("image").info + "_head";
                }
                return line.info;
            }
        }
        /// <summary>
        /// 软件图标
        /// </summary>
        public string Image_Icon
        {
            get
            {
                var line = FindLine("image").Find("icon");
                if (line == null)
                {
                    return FindLine("image").info + "_icon";
                }
                return line.info;
            }
        }
        public string[] Image_Screenshot
        {
            get
            {
                List<string> list = new List<string>();
                Sub sub = FindLine("image")["screenshot"];
                if (sub != null && !string.IsNullOrWhiteSpace(sub.info))
                    list.AddRange(sub.GetInfos());
                else
                    list.Add(Image_Blank);
                return list.ToArray();
            }
        }

        public ImageSource[] ImageSource_Screenshot(ICore core)
        {
            List<ImageSource> list = new List<ImageSource>();
            foreach (string str in Image_Screenshot)
                list.Add(core.ImageSources.FindImage("game_" + str));
            return list.ToArray();
        }
        /// <summary>
        /// 游戏图标 包含logo
        /// </summary>
        public string Image_Game => FindLine("image").info;

        public ImageSource ImageSource_Games(ICore core) => core.ImageSources.FindImage("game_" + Image_Game);
        public ImageSource ImageSource_Blank(ICore core) => core.ImageSources.FindImage("game_" + Image_Blank);
        public ImageSource ImageSource_Head(ICore core) => core.ImageSources.FindImage("game_" + Image_Head);
        public ImageSource ImageSource_Name(ICore core) => core.ImageSources.FindImage("game_" + this["image"]["name"].info);

        /// <summary>
        /// 游戏介绍 短
        /// </summary>
        public string Intor
        {
            get => FindLine("text")["intor"].Info;
            set => FindLine("text")["intor"].Info = value;
        }
        /// <summary>
        /// 游戏介绍文档
        /// </summary>
        /// <param name="tf">格式</param>
        /// <returns>文档</returns>
        public FlowDocument Document(TextFormat tf) => TtD.TextToDocument(FindLine("text").Text, tf);
        /// <summary>
        /// 游戏介绍文档
        /// </summary>
        /// <returns>文档</returns>
        public FlowDocument Document() => TtD.TextToDocument(FindLine("text").Text, SteamTextFormat);
        /// <summary>
        /// 针对Steam设计的游戏界面主题设计
        /// </summary>
        public static TextFormat SteamTextFormat = new TextFormat()
        {
            H1 = new TextFormat.Format()
            {
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.White),
                FontWeight = System.Windows.FontWeights.Bold,
                TextDecorations = System.Windows.TextDecorations.Underline
            },
            H2 = new TextFormat.Format()
            {
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(47, 137, 188)),
                FontWeight = System.Windows.FontWeights.Bold,
            },
            H3 = new TextFormat.Format()
            {
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(47, 137, 188)),
                FontWeight = System.Windows.FontWeights.Bold,
            },
            H4 = new TextFormat.Format()
            {
                FontWeight = System.Windows.FontWeights.Bold,
            },
            H5 = new TextFormat.Format()
            {
                FontSize = 10.5
            },
            H6 = new TextFormat.Format()
            {
                FontSize = 9
            },
            P = new TextFormat.Format()
            {
                Foreground = new SolidColorBrush(Color.FromRgb(172, 178, 184)),
                FontSize = 12,
            }
        };

        /// <summary>
        /// 发行日期 会根据值近似随机
        /// </summary>
        public int PublishDate
        {
            get => FindLine("parameter")[(gint)"publishdate"];
            set => FindLine("parameter")[(gint)"publishdate"] = value;
        }
        /// <summary>
        /// 平均折扣/初始折扣
        /// </summary>
        public int Discount
        {
            get => FindLine("discount").InfoToInt;
            set => FindLine("discount").InfoToInt = value;
        }
        /// <summary>
        /// 最大折扣
        /// </summary>
        public int DiscountMAX
        {
            get => FindLine("discount")[(gint)"max"];
            set => FindLine("discount")[(gint)"max"] = value;
        }
        /// <summary>
        /// 到达最大折扣日期 (日)
        /// </summary>
        public int DiscountMAXDate
        {
            get => FindLine("discount")[(gint)"maxdate"];
            set => FindLine("discount")[(gint)"maxdate"] = value;
        }
        /// <summary>
        /// 折扣周期最长
        /// </summary>
        public int DiscountMAXPeriod
        {
            get => FindLine("discount")[(gint)"maxperiod"];
            set => FindLine("discount")[(gint)"maxperiod"] = value;
        }
        /// <summary>
        /// 折扣周期最短
        /// </summary>
        public int DiscountMINPeriod
        {
            get => FindLine("discount")[(gint)"minperiod"];
            set => FindLine("discount")[(gint)"minperiod"] = value;
        }
        /// <summary>
        /// 好评率 官方推荐好评率
        /// </summary>
        public int Rating => FindLine("parameter")[(gint)"rating"];
        /// <summary>
        /// 实时好评率根据 评论表:游戏 计算
        /// </summary>
        public int RatingRealTime(int gamedate)
        {
            if (gamedate < 0)
                return 0;
            int r = 0, t = 0;
            var ca = Comments.FindAll(x => x.Publish <= gamedate);
            foreach (Comment_Game c in ca)
            {
                r += c.Score * c.Quality;
                t += c.Quality;
            }
            if (t == 0)
                return 0;
            return (Data.Rating + r / t) / 2;
        }

        /// <summary>
        /// 平均人气 1-??% 加成视频观看数
        /// </summary>
        public int Popularity
        {
            get => FindLine("popularity").InfoToInt;
            set => FindLine("popularity").InfoToInt = value;
        }
        /// <summary>
        /// 最高可能人气
        /// </summary>
        public int PopularityMAX
        {
            get => FindLine("popularity")[(gint)"max"];
            set => FindLine("popularity")[(gint)"max"] = value;
        }
        /// <summary>
        /// 到达最大人气日期 (日)
        /// </summary>
        public int PopularityMAXDate
        {
            get => FindLine("popularity")[(gint)"maxdate"];
            set => FindLine("popularity")[(gint)"maxdate"] = value;
        }

        /// <summary>
        /// 平均折扣
        /// </summary>
        public double Price
        {
            get => FindLine("parameter")[(gdbe)"price"];
            set => FindLine("parameter")[(gdbe)"price"] = value;
        }
        /// <summary>
        /// 开发商
        /// </summary>
        public string Developers
        {
            get => FindLine("text")[(gstr)"developers"];
            set => FindLine("text")[(gstr)"developers"] = value;
        }
        /// <summary>
        /// 标签
        /// </summary>
        public string[] Tag => FindLine("text")["tag"].GetInfos();

        /// <summary>
        /// 完成度 这个游戏的可玩内容 一般单位为小时
        /// </summary>
        public int Completion
        {
            get => FindLine("parameter")[(gint)"completion"];
            set => FindLine("parameter")[(gint)"completion"] = value;
        }
        /// <summary>
        /// 游戏评论表
        /// </summary>
        public List<Comment_Game> Comments;

        /// <summary>
        /// 游戏熟练度,对视频有加成 等于1时继续游玩不会累计经验值
        /// </summary>
        public double SkillLevel()
        {
#pragma warning disable CS0612 // 类型或成员已过时
            double value = Data.SkillLevel() / Completion;
#pragma warning restore CS0612 // 类型或成员已过时
            if (value >= 1)
                value = 1;
            return value;
        }
        /// <summary>
        /// 获得评价文字和颜色
        /// </summary>
        /// <param name="rate">游戏评价</param>
        /// <param name="brush">评价颜色笔刷</param>
        /// <param name="popularity">游戏人气</param>
        /// <param name="commentscount">评论计数</param>
        /// <param name="ratestr">tooltips文本</param>
        /// <returns>评价文字</returns>
        public static string GetReviewString(int rate, int popularity, int commentscount, out SolidColorBrush brush, out string ratestr)
        {
            brush = new SolidColorBrush(Color.FromRgb(102, 192, 244));
            ratestr = $"{commentscount} 篇用户的游戏评测中有 {rate}% 为好评";
            if (rate == 0)
            {
                brush = new SolidColorBrush(Color.FromRgb(85, 103, 114));
                ratestr = "无用户评测";
                return "无用户评测";
            }
            else if (rate > 80)
                if (popularity >= 80 && rate > 90)
                    return $"好评如潮";
                else
                    return "特别好评";
            else if (rate > 70)
                return "好评";
            else if (rate > 60)
                return "多半好评";
            else if (rate > 40)
            {
                brush = new SolidColorBrush(Color.FromRgb(185, 160, 116));
                return "褒贬不一";
            }
            else if (rate < 20 && popularity >= 70)
            {
                brush = new SolidColorBrush(Color.FromRgb(163, 76, 37));
                return "差评如潮";
            }
            else
            {
                brush = new SolidColorBrush(Color.FromRgb(195, 92, 44));
                return "多半差评";
            }
        }
        /// <summary>
        /// 获得评价文字和颜色
        /// </summary>
        /// <param name="gamedate">游戏时间</param>
        /// <param name="brush">评价颜色笔刷</param>
        /// <param name="ratestr">tooltips文本</param>
        /// <returns>评价文字</returns>
        public string GetReviewString(int gamedate, out SolidColorBrush brush, out string ratestr)
        {
            int r = Data.ReviewNum;
            int p = Data.Popularity;
            return GetReviewString(RatingRealTime(gamedate), p, r + p, out brush, out ratestr);
        }
        /// <summary>
        /// 提供所有可显示评论
        /// </summary>
        /// <param name="mw">主窗体</param>
        /// <returns>可显示的所有评论</returns>
        public List<Comment_Game> ShowComments(IMainWindow mw)
        {
            if (!Data.Publish)
                return null;
            int time = new TimeSpan(mw.Save.Now.Ticks - Data.PublishDate.Ticks).Days;
            List<Comment_Game> ablecom = Comments.FindAll(x => x.Publish <= time);
            foreach (Comment_Game cg in ablecom)
            {
                if (cg.PublishDate == DateTime.MinValue)
                    cg.PublishDate = mw.Save.Now.AddDays(mw.Save.DayTimePass - cg.Publish).Date;
            }
            return ablecom;
        }

        /// <summary>
        /// 推荐语/推荐类型 临时数据
        /// </summary>
        public string Recommendation;

        /// <summary>
        /// Nili视频自动生成
        /// name:视频名称
        /// info:视频简介
        /// </summary>
        public List<Sub> Nili => FindorAddLine("nili").Subs;

        public ComputerUsage ToComputerUsage()
        {
            Line cu = this["computerusage"];
            return new ComputerUsage(cu.Info)
            {
                CPUUsage = cu[(gdbe)"cpu"],
                GPUUsage = cu[(gdbe)"gpu"],
                MemoryUsage = cu[(gint)"memory"],
                ImportCPU = cu[(gint)"importcpu"],
                ImportGPU = cu[(gint)"importgpu"],
                ImportMemory = cu[(gint)"importmemory"],
            };
        }
    }
    /// <summary>
    /// 游戏数据类,包括游戏信息和各种参数 标头:game 这些数据可能需要作为应用数据储存起来
    /// </summary>
    public class Item_Game : Line
    {
        public Item_Game(Line line) : base(line)
        {

        }
        /// <summary>
        /// 发行日期
        /// </summary>
        public DateTime PublishDate
        {
            get => this[(gdat)"publishdate"];
            set => this[(gdat)"publishdate"] = value;
        }
        /// <summary>
        /// 是否发售
        /// </summary>
        public bool Publish
        {
            get => this[(gbol)"publish"];
            set => this[(gbol)"publish"] = value;
        }
        /// <summary>
        /// 是否拥有
        /// </summary>
        public new bool Have
        {
            get => this[(gbol)"have"];
            set => this[(gbol)"have"] = value;
        }
        /// <summary>
        /// 购买游戏花费的钱,用于退款
        /// </summary>
        public double BuyPrice
        {
            get => this[(gflt)"buyprice"];
            set => this[(gflt)"buyprice"] = value;
        }
        /// <summary>
        /// 当前折扣
        /// </summary>
        public int Discount
        {
            get => this[(gint)"discount"];
            set => this[(gint)"discount"] = value;
        }
        /// <summary>
        /// 折扣持续至/下次折扣日期
        /// </summary>
        public DateTime DiscountDate
        {
            get => this[(gdat)"discountdate"];
            set => this[(gdat)"discountdate"] = value;
        }
        /// <summary>
        /// 最后运行日期
        /// </summary>
        public DateTime LastRun
        {
            get => this[(gdat)"lastrun"];
            set => this[(gdat)"lastrun"] = value;
        }

        /// <summary>
        /// 当前人气
        /// </summary>
        public int Popularity
        {
            get => this[(gint)"popularity"];
            set => this[(gint)"popularity"] = value;
        }
        /// <summary>
        /// 评价人数
        /// </summary>
        public int ReviewNum
        {
            get
            {
                int r = this[(gint)"reviewnum"];
                if (r != 0)
                    return r;
                int p = Popularity;
                r = Function.Rnd.Next(p * p / 2, p * p);
                this[(gint)"reviewnum"] = r;
                return r;
            }
        }
        /// <summary>
        /// 好评率
        /// </summary>
        public int Rating
        {
            get => this[(gint)"rating"];
            set => this[(gint)"rating"] = value;
        }

        /// <summary>
        /// 游戏时间 代表对游戏的理解
        /// </summary>
        public double PlayTime
        {
            get => this[(gdbe)"playtime"];
            set => this[(gdbe)"playtime"] = value;
        }
        /// <summary>
        /// 实况/直播 完成度 完成后会大幅度降低观众吸引力
        /// </summary>
        public double LiveCompletion
        {
            get => this[(gflt)"livecompletion"];
            set => this[(gflt)"livecompletion"] = value;
        }
        /// <summary>
        /// 普通 完成度 代表对游戏的理解
        /// </summary>
        public double Completion
        {
            get => this[(gflt)"completion"];
            set => this[(gflt)"completion"] = value;
        }

        /// <summary>
        /// 游戏熟练程度,请不要调用这个
        /// </summary>
        /// <returns></returns>
        /// 
        [Obsolete]
        public double SkillLevel()
        {
            double lc = LiveCompletion;
            double c = Completion;
            double pt = PlayTime;
            double mc = lc > c ? lc + c * 0.1 : c + lc * 0.1;
            return mc / 2 + pt / 2;
        }
    }
}

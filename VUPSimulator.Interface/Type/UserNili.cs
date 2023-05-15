using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static VUPSimulator.Interface.Comment;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// NiliNili 用户, 支持粉丝数等参数
    /// </summary>
    public class UserNili : Users
    {
        public readonly ISub Data;
        public UserNili(ILine line, ILine globaluserset = null) : base(line)
        {
            Data = globaluserset?[UserName];
        }
        /// <summary>
        /// 粉丝数量
        /// </summary>
        public long Fans
        {
            get
            {
                if (Data == null)
                {
                    return this[(gi64)"fans"];
                }
                else
                {
                    return Data.Infos.GetInt64("fans", this[(gi64)"fans"]);
                }
            }
            set
            {
                if (Data == null)
                    this[(gi64)"fans"] = value;
                else
                    Data.Infos[(gi64)"fans"] = value;
            }
        }
        /// <summary>
        /// 具体粉丝 将来可以作为伪联机和好友互动?
        /// </summary>
        public List<string> FansUser
        {
            get
            {
                if (Data == null)
                {
                    return FindorAdd("fansuser").GetInfos().ToList();
                }
                else
                {
                    string[] array = Data.Infos.GetString("fansUser", FindorAdd("fansuser").Info).Split(',');
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = Sub.TextDeReplace(array[i]);
                    }
                    return array.ToList();
                }
            }
            set
            {
                if (Data == null)
                    FindorAdd("fansuser").info = string.Join(",", value);
                else
                    Data.Infos["fansuser"] = string.Join(",", value);
            }
        }
        /// <summary>
        /// 总共粉丝数量
        /// </summary>
        public long TotalFans => Fans + FansUser.Count;
        /// <summary>
        /// 签名
        /// </summary>
        public string Bio
        {
            get => this[(gstr)"bio"];
            set => this[(gstr)"bio"] = value;
        }
        /// <summary>
        /// 用户立绘, 用于随机生成时使用,默认开头 niliusr_
        /// </summary>
        public string UsrImage
        {
            get => this[(gstr)"usrimage"];
            set => this[(gstr)"usrimage"] = value;
        }

        /// <summary>
        /// Nili视频标签: 用于决定生成视频的质量等
        /// </summary>
        public enum NiliTag
        {
            /// <summary>
            /// 高质量视频制作者: 制作的视频比正常质量高 (约1.2)
            /// </summary>
            QualityHigh,
            /// <summary>
            /// 低质量视频制作者: 制作的视频比正常质量低 (约0.8)
            /// </summary>
            QualityLow,
            /// <summary>
            /// 固定质量视频制作者: 制作的视频质量固定为1.2 不会根据任何条件浮动
            /// </summary>
            QualityFix,
            /// <summary>
            /// 高有趣性: 制作的视频更有趣 (获得更多收藏数和播放量)
            /// </summary>
            FunHigh,
            /// <summary>
            /// 低有趣性: 有趣性x0.8 获得更少收藏
            /// </summary>
            FunLow,
            /// <summary>
            /// 高声音: 音频质量x1.2 获得更多点赞数, 更擅长于音乐视频 (以及音乐视频高权重)
            /// </summary>
            VoiceHigh,
            /// <summary>
            /// 低声音: 音频质量x0.8 获得更少点赞数, 不擅长于音乐视频
            /// </summary>
            VoiceLow,
            /// <summary>
            /// 更好的电脑/水平 录制质量x1.2 获得更多点赞数
            /// </summary>
            ComputerGood,
            /// <summary>
            /// 更差的电脑/水平 录制质量x0.8 获得更少点赞数
            /// </summary>
            ComputerBad,
            /// <summary>
            /// 长视频制作者: 一般制作的视频时长比较长(1-2小时) 较少的播放量+较多的收藏
            /// </summary>
            LengthLong,
            /// <summary>
            /// 短视频制作者: 一般制作的视频时长比较短(0.2-0.5小时) 较多的播放量+较少的收藏
            /// </summary>
            LengthShort,


            /// <summary>
            /// 倾向于制作 杂谈视频 (一般为口才说话自我介绍等)
            /// </summary>
            Base,
            /// <summary>
            /// 倾向于制作 游戏视频 (录制游戏实况)
            /// </summary>
            Game,
            /// <summary>
            /// 倾向于制作 绘画视频
            /// </summary>
            Draw,
            /// <summary>
            /// 倾向于制作 编程视频
            /// </summary>
            Program,
            /// <summary>
            /// 倾向于制作 唱歌视频
            /// </summary>
            Song,
        }

        /// <summary>
        /// Nili视频标签: 用于决定生成视频的质量等
        /// </summary>
        public List<NiliTag> Tags
        {
            get
            {
                ISub subtag = Find("nilitag");
                if (subtag == null)
                    return new List<NiliTag>();
                List<NiliTag> tags = new List<NiliTag>();
                foreach (string tag in subtag.GetInfos())
                    tags.Add((NiliTag)Enum.Parse(typeof(NiliTag), tag, true));
                return tags;
            }
        }

        /// <summary>
        /// 总计播放次数
        /// </summary>
        public long CountPlay
        {
            get
            {
                if (Data == null)
                {
                    return this[(gi64)"pc"];
                }
                else
                {
                    return Data.Infos.GetInt64("pc", this[(gi64)"pc"]);
                }
            }
            set
            {
                if (Data == null)
                    this[(gi64)"pc"] = value;
                else
                    Data.Infos[(gi64)"pc"] = value;
            }
        }
        /// <summary>
        /// 总计喜欢次数
        /// </summary>
        public long CountLike
        {
            get
            {
                if (Data == null)
                {
                    return this[(gi64)"lc"];
                }
                else
                {
                    return Data.Infos.GetInt64("lc", this[(gi64)"lc"]);
                }
            }
            set
            {
                if (Data == null)
                    this[(gi64)"lc"] = value;
                else
                    Data.Infos[(gi64)"lc"] = value;
            }
        }
        /// <summary>
        /// 总计收藏次数
        /// </summary>
        public long CountStar
        {
            get
            {
                if (Data == null)
                {
                    return this[(gi64)"sc"];
                }
                else
                {
                    return Data.Infos.GetInt64("sc", this[(gi64)"sc"]);
                }
            }
            set
            {
                if (Data == null)
                    this[(gi64)"sc"] = value;
                else
                    Data.Infos[(gi64)"sc"] = value;
            }
        }

        ///// <summary>
        ///// 获取用户立绘图片
        ///// </summary>
        //public ImageSource UsrImageSource(IMainWindow mw)
        //{
        //    return mw.Core.ImageSources.FindImage("niliusr_" + UsrImage);
        //}
    }
    /// <summary>
    /// NiliNili视频发布者会使用的类 目前只有玩家会使用该类 主要用于显示玩家每月盈利等
    /// </summary>
    public class UserNiliSuper : UserNili
    {
        public UserNiliSuper(ILine line) : base(line)
        {

        }

        /// <summary>
        /// 总粉丝表 用于图标展示 1=1天
        /// </summary>
        public StringStructure FansGraph => FindorAdd("fansgraph").Infos;
        /// <summary>
        /// 总收入表 用于图标展示 1=1天
        /// </summary>
        public StringStructure IncomeGraph => FindorAdd("incomegraph").Infos;
        /// <summary>
        /// 总收藏表 用于图标展示 1=1天
        /// </summary>
        public StringStructure StartGraph => FindorAdd("startgraph").Infos;

        /// <summary>
        /// 总点赞表 用于图标展示 1=1天
        /// </summary>
        public StringStructure LikeGraph => FindorAdd("likegraph").Infos;

        /// <summary>
        /// 总播放表 用于图标展示 1=1天
        /// </summary>
        public StringStructure PlayGraph => FindorAdd("playgraph").Infos;
        /// <summary>
        /// 所有关注的用户
        /// </summary>
        public List<string> Follow
        {
            get => FindorAdd("follow").GetInfos().ToList();
            set => FindorAdd("follow").info = string.Join(",", value);
        }
        /// <summary>
        /// 总收入
        /// </summary>
        public double IncomeTotal
        {
            get => FindorAdd("incometotal").GetFloat();
            set => FindorAdd("incometotal").SetFloat(value);
        }
        /// <summary>
        /// 月度收入
        /// </summary>
        public double IncomeMonth
        {
            get => FindorAdd("incomemonth").GetFloat();
            set => FindorAdd("incomemonth").SetFloat(value);
        }
        /// <summary>
        /// 未提取收入
        /// </summary>
        public double IncomeNoOut
        {
            get => FindorAdd("incomenoout").GetFloat();
            set => FindorAdd("incomenoout").SetFloat(value);
        }
    }
}

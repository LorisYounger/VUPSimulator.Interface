using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;
namespace VUPSimulator.Interface
{
    /// <summary>
    /// 评论表: 包含评论的基础信息 表头:comment
    /// </summary>
    public class Comment : Line
    {
        public Comment(Line line) : base(line) { }
        /// <summary>
        /// 评分, 从0到100, 偏极端的偏极端
        /// </summary>
        public int Score => this[(gint)"score"];
        /// <summary>
        /// 评论质量 从0-100
        /// </summary>
        public int Quality
        {
            get
            {
                int q = this[(gint)"quality"];
                if (Find("likes") == null)
                {
                    int i = Function.Rnd.Next(q / 2, q);
                    this[(gint)"likes"] = i;
                }
                int tq = (q * 3 + this[(gint)"likes"] * 2) / 4 + IsLike;
                if (tq <= 0)
                    return 0;
                return tq;
            }
            set => this[(gint)"quality"] = value;
        }

        /// <summary>
        /// 评论内容 为Hash形式
        /// </summary>
        public new string Comments => Text;

        /// <summary>
        /// 评论类型
        /// </summary>
        public enum CommentType
        {
            /// <summary>
            /// 基本评论
            /// </summary>
            Base,
            /// <summary>
            /// 游戏评价
            /// </summary>
            Game,
            /// <summary>
            /// 视频评论 会有回复的各种选项
            /// </summary>
            Video,
            /// <summary>
            /// 直播评论 将会额外制作,会有各种选项
            /// </summary>
            Stream
        }

        /// <summary>
        /// 评论标签,方便用于从评论表拉出来
        /// </summary>
        public enum CommentTag
        {
            /// <summary>
            /// 无感情,中立,都可以用的评价 例如: 第一,我来啦
            /// 标准好评 例如: 支持支持, 催更
            /// 标准差评 例如:好无聊,没意思
            /// </summary>
            Nomal,
            /// <summary>
            /// 有趣的评价 一般点赞数会比较高
            /// </summary>
            Fun,
            /// <summary>
            /// 主意
            /// 好评: 这点子真棒, 只有|author|才能想出来吧, 真有趣
            /// 差评: 真无聊,没意思
            /// </summary>
            Idear,
            /// <summary>
            /// 口才
            /// 好评: 说话真好听,说话真有意思
            /// 差评: 声音好难听,听不清楚
            /// </summary>
            Speak,
            /// <summary>
            /// 运营
            /// 好评: 走过路过不要错过,给|author|点波关注
            /// 差评: 大家千万要小心|author|,有黑历史
            /// </summary>
            Operate,
            /// <summary>
            /// 高质量评论
            /// 会根据评论质量自动添加此tag,无需手动添加
            /// </summary>
            HighQuality,
            /// <summary>
            /// 低质量评论
            /// 会根据评论质量自动添加此tag,无需手动添加
            /// </summary>
            LowQuality,
            /// <summary>
            /// 允许随机拉取
            /// 允许在条件不符合的情况下以一定概率被选中
            /// </summary>
            Rnd,
        }

        /// <summary>
        /// 评论类型
        /// </summary>
        public CommentType Type => (CommentType)Enum.Parse(typeof(CommentType), info, true);
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
        /// <summary>
        /// 评论标签
        /// </summary>
        public List<string> Tags_str()
        {
            var ls = new List<string>();
            foreach (CommentTag tag in Tags)
                ls.Add(tag.ToString().ToLower());
            return ls;
        }
        /// <summary>
        /// 评论用户 为null则为随便拉个人
        /// </summary>
        public string User
        {
            get => GetString("user", null);
            set => this[(gstr)"user"] = value;
        }
        /// <summary>
        /// 获取游戏评论用户 为null则为随便拉个人
        /// </summary>
        /// <returns></returns>
        public Users GetUser(ICore core, Item_Game_base game)
        {
            Users usr;
            if (User == null)
            {//开始拉人
                var tag = Tags_str();
                //给tag添加游戏元素
                tag.AddRange(game.Tag);
                int chs = core.Users.Count / 10;
                var v = core.Users.FindAll(x =>
                {
                    string[] xt = x.GameTag;
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
                    usr = core.Users[core.Users.Count / 10];
                else
                    usr = v[Function.Rnd.Next(v.Count)];

                User = usr.UserName;
                return usr;
            }
            //返回用户信息
            usr = core.Users.Find(x => x.UserName == User);
            if (usr == null)
            {
                //如果是null 则尝试重新拉人
                Remove("user");
                return GetUser(core, game);
            }
            return usr;
        }

        /// <summary>
        /// 点赞数量
        /// </summary>
        public int Likes
        {
            get
            {
                Sub l = Find("likes");
                if (l == null)
                {
                    int i = Function.Rnd.Next(Quality / 2, Quality);
                    this[(gint)"likes"] = i;
                    return i;
                }
                else
                    return l.InfoToInt;
            }
            set => this[(gint)"likes"] = value;
        }


        /// <summary>
        /// 玩家是否点过赞 0未 1=点赞 -1=点踩
        /// </summary>
        public int IsLike
        {
            get => this[(gint)"islike"];
            set => this[(gint)"islike"] = value;
        }
        /// <summary>
        /// 发布日期
        /// </summary>
        public int Publish
        {
            get => this[(gint)"publish"];
            set => this[(gint)"publish"] = value;
        }

        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime PublishDate
        {
            get => this[(gdat)"date"];
            set => this[(gdat)"date"] = value;
        }
    }
    /// <summary>
    /// 游戏评论表
    /// </summary>
    public class Comment_Game : Comment
    {//游戏评论和别的不一样, 游戏评论不是自动生成的
        public Comment_Game(Line line) : base(line) { }

        /// <summary>
        /// 绑定的游戏名称
        /// </summary>
        public string Game
        {
            get => this[(gstr)"game"];
            set => this[(gstr)"game"] = value;
        }


    }
}
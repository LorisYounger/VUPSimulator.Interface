using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 玩家存档数据 若未开始游戏则为空
    /// </summary>
    public abstract class ISave
    {
        protected abstract IMainWindow imw { get; }

        /// <summary>
        /// 游戏存档名字 (一般是人名+日期组合)
        /// </summary>//干脆直接就存用户名, 这样方便登陆,就不保存了
        public string FileName => UserName + '-' + Now.ToString("yyyy-MM-dd-HH-mm") + ".vup.lps"; //注:文件名不允许出现:/符号

        public string UserName;
        /// <summary>
        /// 当前日期,游戏开始默认为2200年1月1日上午8点(实际为以开局日期为准)
        /// </summary>
        public DateTime Now;

        /// <summary>
        /// 游戏经过的天数
        /// </summary>
        public int DayTimePass = 0;
        /// <summary>
        /// 难度 增益除以 减益乘以
        /// </summary>
        public double Difficulty;

        /// <summary>
        /// 该存档开始游戏的日期
        /// </summary>
        public DateTime StartTime;
        /// <summary>
        /// 游戏开局开始日期(日期,不包括时间)
        /// </summary>
        public DateTime StartGameTime;
        /// <summary>
        /// 上次睡觉时间
        /// </summary>
        public DateTime LastSleepTime;
        /// <summary>
        /// 当前正在处理的事件
        /// 请注意,虽然说部分软件可以挂后台,但是处理事件只能处理一件
        /// </summary>
        public string Working = null;
        /// <summary>
        /// 当前正在处理的物品,主要用于核对
        /// </summary>
        public object WorkingOBJ = null;
        /// <summary>
        /// 玩家的Nili账户
        /// </summary>
        public UserNiliSuper UserNili = null;

        #region 玩家数据

        /// <summary>
        /// 全部事件
        /// </summary>
        public List<Event> ALLEvent = new List<Event>();
        /// <summary>
        /// 全部数据
        /// </summary>
        public Line EventData = new Line();
        /// <summary>
        /// 全部物品
        /// </summary>
        public List<Item> Items = new List<Item>();
        /// <summary>
        /// 添加物品(如果已经存在则数量+1)
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="many">数量</param>
        public void AddItem(Item item, int many = 1)
        {
            if (item is Food f)
                f.QualityGuaranteeStartTime = Now;
            if (item.AllowMultiple)
            {
                var i = Items.FirstOrDefault(x => x.ItemName == item.ItemName);
                if (i != null)
                {
                    i.Many += item.Many + many - 1;
                    return;
                }
                else
                {
                    if (item.Many == 1)
                        item.Many = many;
                    else
                        item.Many += many;
                    Items.Add(item);
                    return;
                }
            }
            if (many == 1)
                Items.Add(item);
            else
                for (int i = 0; i < many; i++)
                    Items.Add(Item.New(item));
        }
        /// <summary>
        /// 全部游戏视频
        /// </summary>
        public List<Video> Video = new List<Video>();

        /// <summary>
        /// 用户电脑
        /// </summary>
        public Computer Computer;

        protected double health;
        protected double strengthsleep;
        protected double strengthfood;
        /// <summary>
        /// 资金
        /// </summary>
        public double Money;
        /// <summary>
        /// 健康
        /// </summary>
        public virtual double Health
        {
            get => health;
            set => health = value;
        }
        /// <summary>
        /// 精力
        /// </summary>
        public double Strength
        {
            get => Math.Min(strengthsleep, strengthfood);
        }
        /// <summary>
        /// 精力:睡眠程度
        /// </summary>
        public double StrengthSleep { get => strengthsleep; set => strengthsleep = Math.Min(Health, value); }
        /// <summary>
        /// 精力:食物
        /// </summary>
        public double StrengthFood { get => strengthfood; set => strengthfood = Math.Min(Health, value); }

        /// <summary>
        /// 扣除精力
        /// </summary>
        /// <param name="strength">精力:0-100</param>
        /// <param name="duration">持续时间</param>
        /// <param name="reason">原因</param>

        public abstract void StrengthRemove(double strength, double duration, string reason);
        /// <summary>
        /// 玩家状态系统
        /// </summary>
        public PlayerStateSystem PStateSystem;
        /// <summary>
        /// 属性: 思维
        /// </summary>
        public double Pidear;
        /// <summary>
        /// 属性: 口才
        /// </summary>
        public double Pspeak;
        /// <summary>
        /// 属性: 运营
        /// </summary>
        public double Poperate;
        /// <summary>
        /// 属性: 修图
        /// </summary>
        public double Pimage;
        /// <summary>
        /// 属性: 剪辑
        /// </summary>
        public double Pclip;
        /// <summary>
        /// 属性: 绘画
        /// </summary>
        public double Pdraw;
        /// <summary>
        /// 属性: 程序
        /// </summary>
        public double Pprogram;
        /// <summary>
        /// 属性: 游戏
        /// </summary>
        public double Pgame;
        /// <summary>
        /// 属性: 声乐
        /// </summary>
        public double Psong;

        #endregion

        #region UI数据
        //例如桌面背景啥的

        /// <summary>
        /// 简单UI数据储存位置,包括 收藏夹 等
        /// </summary>
        public LpsDocument UIData = new LpsDocument();
        /// <summary>
        /// 桌面控件储存数据
        /// </summary>
        public LpsDocument WidgetData = new LpsDocument();
        /// <summary>
        /// 所有'游戏'数据
        /// </summary>
        public List<Item_Game_base> Games = new List<Item_Game_base>();
        ///// <summary>
        ///// 所有DIY评论 //修改:评论现在储存在各种子类里,例如Games
        ///// </summary>
        //public List<Comment_base> Comments = new List<Comment_base>();

        #endregion

        #region 游戏数据
        /// <summary>
        /// 其他数据 多用于代码插件
        /// Name均为'other' 请使用 FindInfo 查找相关数据
        /// </summary>
        public LpsDocument Other = new LpsDocument();
        /// <summary>
        /// 评论数据 多用于评论
        /// Name均为'comm' 请使用 FindInfo 查找相关数据
        /// </summary>
        public LpsDocument CommentData = new LpsDocument();
        /// <summary>
        /// 游戏统计 用于任务/成就等
        /// </summary>
        public Line Statistic;
        /// <summary>
        /// 游戏统计 用于日常/月度统计
        /// Name均为'stat'
        /// </summary>
        public LpsDocument Statistics = new LpsDocument();
        /// <summary>
        /// 图片数据
        /// </summary>
        public List<GenBase> GenBases = new List<GenBase>();
        /// <summary>
        /// 用户数据
        /// </summary>
        public Line UsersData;
        /// <summary>
        /// Nili所有的视频(包括玩家发的视频)
        /// </summary>
        public List<VideoNili> VideoNilis = new List<VideoNili>();
        /// <summary>
        /// 老画师作者数据
        /// </summary>
        public List<Line> AuthorData = new List<Line>();
        /// <summary>
        /// 所有Nili用户(加速检索用)
        /// </summary>
        public List<UserNili> UsersNili = new List<UserNili>();
        /// <summary>
        /// 讲述人类型
        /// </summary>
        public enum TellerType
        {
            /// <summary>
            /// 经典: 卡拉斯特
            /// </summary>
            Classic,
            /// <summary>
            /// 挑战: 玛哈萝
            /// </summary>
            Challenge,
            /// <summary>
            /// 随机: 兰迪
            /// </summary>
            Random,
        }
        /// <summary>
        /// 讲述人数据: 包括模型等所有所需数据
        /// </summary>
        public Line TellerData;
        /// <summary>
        /// 讲述人类型
        /// </summary>
        public TellerType Teller
        {
            get => (TellerType)TellerData[(gint)"TellerType"];
            set => TellerData[(gint)"TellerType"] = (int)value;
        }

        #endregion
    }
}

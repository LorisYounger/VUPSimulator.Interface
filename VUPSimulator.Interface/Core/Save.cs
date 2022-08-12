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
        /// 该存档开始游戏的日期
        /// </summary>
        public DateTime StartTime;
        /// <summary>
        /// 游戏开局开始日期(日期,不包括时间)
        /// </summary>
        public DateTime StartGameTime;
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

        #region 游戏性数据

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
        /// 全部游戏视频
        /// </summary>
        public List<Video> Video = new List<Video>();

        /// <summary>
        /// 用户电脑
        /// </summary>
        public Computer Computer;

        protected int money;
        protected int health;
        protected int strength;
        /// <summary>
        /// 资金
        /// </summary>
        public double Money { get => money / 1000.0; set => money = (int)(value * 1000); }
        /// <summary>
        /// 健康
        /// </summary>
        public double Health
        {
            get => health / 100.0;
            set
            {
                //TODO:低健康 触发住院事件
                //TODO:健康=0 结束游戏
                if (value <= 30)
                {
                    //if (ALLEvent.Find(x => x.EventName == "ill") == null) TODO
                    //    imw.Core.EventBases.Find(x => x.EventName == "ill").Create(imw, Now).TimeHandle(TimeSpan.Zero, imw);
                }

                health = (int)(value * 100);

            }
        }
        /// <summary>
        /// 精力
        /// </summary>
        public double Strength
        {
            get => strength / 100.0; set
            {
                if (value <= 0)
                {
                    //TODO如果精力为0,
                    //strength = 0; 昏过去事件一般都会呼出strength事件, 无需重新呼出
                    if (ALLEvent.Find(x => x.EventName == "faint") == null)
                        imw.Core.EventBases.Find(x => x.EventName == "faint").Create(imw, Now).TimeHandle(TimeSpan.Zero, imw);
                }
                else if (value <= Health / 3)
                {
                    //呼出疲惫的DEBUFF
                    strength = (int)(value * 100);
                    if (ALLEvent.Find(x => x.EventName == "tired") == null)
                        imw.Core.EventBases.Find(x => x.EventName == "tired").Create(imw, Now).TimeHandle(TimeSpan.Zero, imw);
                }
                else
                {
                    //清除负面buff 以tired开头的负面buff都可以清
                    ALLEvent.Find(x => x.EventName.StartsWith("tired"))?.Disposed(imw);

                    if (value > Health)
                    {//精力上限为健康
                        strength = (int)(Health * 100);
                    }
                    else
                    {
                        strength = (int)(value * 100);
                    }
                }
            }
        }

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

        #region UI性数据
        //例如桌面背景啥的

        /// <summary>
        /// 简单UI数据储存位置,包括 收藏夹 等
        /// </summary>
        public LpsDocument UIData = new LpsDocument();
        /// <summary>
        /// 所有'游戏'数据
        /// </summary>
        public List<Item_Game_base> Games = new List<Item_Game_base>();
        ///// <summary>
        ///// 所有DIY评论 //修改:评论现在储存在各种子类里,例如Games
        ///// </summary>
        //public List<Comment_base> Comments = new List<Comment_base>();
        #endregion

        /// <summary>
        /// 其他数据 多用于代码插件
        /// Name均为'other' 请使用 FindInfo 查找相关数据
        /// </summary>
        public LpsDocument Other = new LpsDocument();

        /// <summary>
        /// 游戏统计 用于任务/成就等
        /// Name均为'statis' 请使用 FindInfo 查找相关数据
        /// </summary>
        public LpsDocument Statistics = new LpsDocument();

    }
}

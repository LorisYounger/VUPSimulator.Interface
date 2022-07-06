using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 所有事件的基本类 事件
    /// </summary>
    public class EventBase : Line
    {
        /// <summary>
        /// 从该基本类创建新事件 并自动加进事件链条
        /// </summary>
        /// <param name="mw">主窗体</param>
        /// <param name="startdate">事件开始日期</param>
        /// <param name="setting">事件相关设置</param>
        /// <returns>新的事件 注意手动添加Handle</returns>
        public Event Create(IMainWindow mw, DateTime startdate, params Sub[] setting)
        {
            Line line = (Line)Clone();

            //自动登记时间
            //是否随机触发
            if (PeriodRandom)
            {
                double per = Period;
                startdate = startdate.AddHours(per / 2 + per * Function.Rnd.NextDouble());
            }
            line[(gstr)"startdate"] = startdate.ToString("yyyy/MM/dd HH:mm");
            //看看要不要添加结束时间
            if (EndTime != 0)
                line[(gstr)"enddate"] = startdate.AddHours(EndTime).ToString("yyyy/MM/dd HH:mm");

            foreach (Sub sub in setting)
                line.AddorReplaceSub(sub);

            //创建事件并添加进事件链
            var ev = Create(mw, line, Type);
            mw?.Save?.ALLEvent.Add(ev);
            return ev;
        }
        /// <summary>
        /// 从line中创建新事件
        /// </summary>
        /// <param name="mw">主窗体</param>
        /// <param name="line">数据</param>
        /// <param name="type">事件类型</param>
        /// <returns></returns>
        public static Event Create(IMainWindow mw, Line line, EventType type)
        {
            switch (type)
            {
                case EventType.money: return new EventMoney(mw, line);
                case EventType.strength: return new EventStrength(mw, line);
                case EventType.health: return new EventHealth(mw, line);
                case EventType.timepass: return new EventTimePass(mw, line);
                case EventType.xnamedisenable: return new EventXNameDisenable(mw, line);
                case EventType.xnamedisposed: return new EventXNameDisposed(mw, line);
                case EventType.xnameenable: return new EventXNameEnable(mw, line);
                case EventType.xnamefource: return new EventXNameFource(mw, line);
                default:
                    return new Event(mw, line);
            }
        }
        /// <summary>
        /// 从line中创建新事件
        /// </summary>
        /// <param name="mw">主窗体</param>
        /// <param name="line">数据</param>
        public static Event Create(IMainWindow mw, Line line) => Create(mw, line, (EventType)Enum.Parse(typeof(EventType), line.info, true));
        public enum EventType
        {
            none,//无 看上去啥也没有其实可以用setget进行很多操作

            money,//钱相关操作
            //bank,//银行(储蓄内金额)相关操作
            //debt,//负债 属于银行系统
            //这个以后会有专门的房子类型house,//房租(同时也是房子,加成)

            //数值操作相关类型
            health,//健康相关操作
            strength,//体力相关操作

            //这个以后会有专门的工作类型work,//工作
            //这个以后会有专门的工作房子类型endwork,//结束工作

            //xname,//xname相关基本类,用于操作别的事件 //启动事件之后大改,现在有多重方法启动后续事件,比如说设置相关值和通过计算
            xnamedisposed,//结束并销毁 xname事件
            xnamedisenable,//关闭 xname事件
            xnameenable,//启动 xname事件
            xnamefource,//强行启动xname事件,无视startdate,
            xnamestart,//从event库拉取相关事件

            time,//时间相关操作,主要是跳过相应时间
            timepass,//跳过相应时间

            cg,//激活时显示一张图片

            endevent,//结束游戏事件基本类 带有CG, 停止 Save 运行,关闭界面,提供加载或新游戏选项
            endevent_money,//当钱小于某个值的时候,结束游戏
            endevent_fans,//当粉丝小于某个值的时候,结束游戏

        }
        public EventType Type
        {
            get => (EventType)Enum.Parse(typeof(EventType), info, true);
            set => info = value.ToString();
        }
        public EventBase(Line line) : base(line)
        {

        }
        /// <summary>
        /// Event的名字,方便从Event库呼出
        /// </summary>
        public string EventName => FindorAdd("name").Info;
        /// <summary>
        /// 是否对玩家可见 (在侧边栏)
        /// </summary>
        public VisibleType Visible
        {
            get
            {
                Sub vissub = Find("visible");
                if (vissub == null)
                    return VisibleType.None;
                if (int.TryParse(vissub.info, out int p))
                    return (VisibleType)p;
                return (VisibleType)Enum.Parse(typeof(VisibleType), vissub.info, true);
            }
            set => FindorAdd("period").info = value.ToString();
        }

        public enum VisibleType
        {
            /// <summary>
            /// 不显示在侧边栏
            /// </summary>
            None,
            /// <summary>
            /// 显示为日历 - 包含下次和结束日期
            /// </summary>
            Calendar,
            /// <summary>
            /// 显示为一次性消息 - 仅仅包含开始日期
            /// </summary>
            Message
        }

        /// <summary>
        /// 伴生对象,在事件发起后会召唤其他事件
        /// </summary>
        public string[] Associated
        {
            get
            {
                Sub ass = Find("associated");
                if (ass == null)
                    return new string[0];
                return ass.GetInfos();
            }
            set
            {
                FindorAdd("associated").Info = string.Join(",", value);
            }
        }
        /// <summary>
        /// 伴生对象,在事件销毁后会召唤其他事件
        /// </summary>
        public string[] EndAssociated
        {
            get
            {
                Sub ass = Find("endassociated");
                if (ass == null)
                    return new string[0];
                return ass.GetInfos();
            }
            set
            {
                FindorAdd("endassociated").Info = string.Join(",", value);
            }
        }
        /// <summary>
        /// 当事件发生时是否弹出消息给玩家
        /// </summary>
        public bool VisibleMessage
        {
            get => this[(gbol)"visiblemsg"];
            set => this[(gbol)"visiblemsg"] = value;
        }
        /// <summary>
        /// 如果弹出消息,是否只弹出一次?
        /// </summary>
        public bool VisibleMessageIsSingle
        {
            get => this[(gbol)"visiblemsgsingle"];
            set => this[(gbol)"visiblemsgsingle"] = value;
        }
        /// <summary>
        /// 事件信息(标题)
        /// </summary>
        public string EventInfo
        {
            get
            {
                Sub sub = Find("info");
                if (sub == null)
                    return info;
                else
                    return sub.Info;
            }
            set => FindorAdd("info").Info = value;
        }
        /// <summary>
        /// 显示消息(当事件触发后弹窗消息)
        /// 注:如果为空则尝试引导标题
        /// </summary>
        public string Message
        {
            get
            {
                Sub sub = Find("message");
                if (sub == null)
                    return EventInfo;
                else
                    return sub.Info;
            }
            set => FindorAdd("message").Info = value;
        }
        /// <summary>
        /// 事件注释(当事件前显示的消息)
        /// 注:如果为空则尝试引导标题
        /// TODO: 将所有转义值转义为储存值
        /// </summary>
        public string Intor
        {
            get
            {
                Sub sub = Find("intor");
                if (sub == null)
                    return EventInfo;
                else
                    return sub.Info;
            }
            set => FindorAdd("intor").Info = value;
        }

        public enum PeriodType
        {
            Single = 0,//仅触发一次,在startdate
            Hourly = 1,//触发每小时一次
            Daily = 24,//触发每天一次
            Weekly = 168,//触发每周一次
            Monthly = 720,//触发每月一次
            Yearly = 8760,//触发每年一次
            H_1 = 24,//
            H_2 = 2,//每2小时触发一次
            H_3 = 3,//每3小时触发一次
            H_4 = 4,
            H_5 = 5,
            H_6 = 6,
            H_7 = 7,
            H_8 = 8,
            H_9 = 9,
            H_10 = 10,
            H_11 = 11,
            H_12 = 12,//每12小时触发一次
            D_1 = 24,//每天触发一次
            D_2 = 48,
            D_3 = 72,
            D_4 = 96,
            D_5 = 120,
            D_6 = 144,
            W_1 = 168,
            W_2 = 336,//每2周触发一次
            W_3 = 504,
            W_4 = 672,
            M_2 = 1440,//每2月触发一次
            M_3 = 2160,
            M_4 = 2880,
            M_5 = 3600,
            M_6 = 4320,
            Y_1 = 8760,//每年一次
            Y_2 = 17520,
            Y_3 = 26280,
            Y_4 = 35040,
            Y_5 = 43800,
            Y_10 = 87600,
            Y_20 = 175200,
        }

        /// <summary>
        /// 是否随机触发 频率根据PeriodType决定
        /// </summary>
        public bool PeriodRandom
        {
            get => this[(gbol)"periodrandom"];
            set => this[(gbol)"periodrandom"] = value;
        }
        /// <summary>
        /// 周期触发频率 单位小时(可以参考PeriodType)
        /// </summary>
        public int Period
        {
            get
            {
                Sub sub = Find("period");
                if (sub == null)
                    return 0;
                if (int.TryParse(sub.info, out int p))
                    return p;
                return (int)Enum.Parse(typeof(PeriodType), sub.info, true);
            }
            set => Find("period").info = value.ToString();
        }
        /// <summary>
        /// 周期触发频率 PeriodType 版本
        /// </summary>
        public string PeriodtoString
        {
            get
            {
                Sub sub = Find("period");
                if (sub == null)
                    return "Single";
                if (int.TryParse(sub.info, out int p))
                    return ((PeriodType)p).ToString();
                if (Enum.TryParse(sub.info, true, out PeriodType pt))
                    return pt.ToString();
                return sub.info;
            }
            set => Find("period").info = value.ToString();
        }
        /// <summary>
        /// 事件结束时间,单位小时 0为null,会在新建的时候自动添加
        /// </summary>
        public int EndTime
        {
            get
            {
                if (Find("endtime") == null)
                    return 0;
                if (int.TryParse(Find("endtime").info, out int p))
                    return p;
                return (int)Enum.Parse(typeof(PeriodType), Find("endtime").info, true);
            }
            set => FindorAdd("endtime").info = value.ToString();
        }
        /// <summary>
        /// 事件结束时间 PeriodType 版本
        /// </summary>
        public PeriodType EndTimetoType
        {
            get
            {
                if (Find("endtime") == null)
                    return PeriodType.Single;
                if (int.TryParse(Find("endtime").info, out int p))
                    return (PeriodType)p;
                return (PeriodType)Enum.Parse(typeof(PeriodType), Find("endtime").info, true);
            }
            set => FindorAdd("endtime").info = value.ToString();
        }
        /// <summary>
        /// 是否能够从事件库中触发该事件
        /// </summary>
        public bool AutoTrigger
        {
            get => this[(gbol)"autotrigger"];
            set => this[(gbol)"autotrigger"] = value;
        }

        public Function.MSGType MSGType
        {
            get
            {
                if (Find("msgtype") == null)
                    return 0;
                if (int.TryParse(Find("msgtype").info, out int r))
                {
                    return (Function.MSGType)r;
                }
                if (Enum.TryParse(Find("msgtype").info, true, out Function.MSGType pt))
                    return pt;
                return 0;
            }
            set => this[(gint)"periodrandom"] = (int)value;
        }

        /// <summary>
        /// 执行该Event的计算
        /// </summary>
        public void EventCalculat(IMainWindow mw) => Function.DataCalculat(mw, this);
        /// <summary>
        /// 判断该Event能否自动激活从而触发事件链
        /// </summary>
        /// <param name="mw">主窗口</param>
        /// <returns>True:立即激活立即执行,除非PeriodRandom为True,则设置为下次激活时间</returns>
        public bool StartTrigger(IMainWindow mw)
        {
            //能否被自动激活
            if (!AutoTrigger)
                return false;
            //如果有同类指令,不自动触发
            if (mw.Save.ALLEvent.Exists(x => x.EventName == EventName))
                return false;
            return StartDecied(mw);
        }
        /// <summary>
        /// 判断该Event前置条件是否满足,是Enabled的一部分
        /// </summary>
        /// <param name="mw">主窗口</param>
        /// <returns>True:可以运行该事件</returns>
        public bool StartDecied(IMainWindow mw) => Function.DataEnable(mw, this);
        /// <summary>
        /// 告诉用户为啥可以通过这个Event/激活条件是什么
        /// </summary>
        public string WhyPass(IMainWindow mw) => Function.DataEnableString(mw, this);
    }





    public class Event : EventBase
    {
        public Event(IMainWindow mw, Line line) : base(line)
        {
            if (mw == null)
                return;
            mw.Dispatcher.Invoke(new Action(() =>
            {
                //如果是日历,则一开始就进侧边栏
                if (Visible == VisibleType.Calendar)
                    VisibleMCTag = mw.CreateCALTag(this);
                else if (Visible == VisibleType.Message && Name == "gevent")//如果是游戏存档,也可以进侧边栏
                    VisibleMCTag = mw.CreateMSGTag(this);
            }));
            mw.TimeHandle += TimeHandle;
        }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                Sub sub = Find("startdate");
                if (sub == null)
                    return Function.DateMaxValue;
                else
                    return Convert.ToDateTime(sub.Info);
            }
            set => FindorAdd("startdate").info = value.ToString("yyyy/MM/dd HH:mm");
        }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                Sub sub = Find("enddate");
                if (sub == null)
                    return Function.DateMaxValue;
                else
                    return Convert.ToDateTime(sub.Info);
            }
            set => FindorAdd("enddate").info = value.ToString("yyyy/MM/dd HH:mm");
        }

        /// <summary>
        /// 转换成介绍
        /// </summary>
        public virtual string ToIntor => $"开始日期:{StartDate.ToShortDateString()}\n{(EndDate == Function.DateMaxValue ? "" : "结束日期:" + EndDate.ToShortDateString() + '\n')}" +
            $"循环:{PeriodtoString}";
        /// <summary>
        /// 是否生效 失效则不会起用,尽管在时间范围内
        /// </summary>
        public bool Enabled
        {
            get => !this[(gbol)"disenabled"];
            set => this[(gbol)"disenabled"] = !value;
        }
        /// <summary>
        /// 判断这个事件是否生效
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>是否生效</returns>
        public bool IsEnable(DateTime time) => Enabled && time.Ticks >= StartDate.Ticks;
        /// <summary>
        /// 销毁这个Event 消息在消息查看后销毁, 事件在最后一次激发销毁
        /// </summary>
        public async Task Disposed(IMainWindow mw)
        {
            //在销毁前执行ENDASS
            //拉相关事件 如果已经存在就不拉
            foreach (var evs in EndAssociated)
                if (mw.Save.ALLEvent.Find(x => x.EventName == evs) == null)
                    mw.Core.EventBases.Find(x => x.EventName == evs).Create(mw, mw.Save.Now);

            if (VisibleMCTag != null)
            {
                await mw.Dispatcher.InvokeAsync(() => mw.RemoveIMCTag(VisibleMCTag));
                mw.TimeUIHandle -= VisibleMCTag.MW_TimeUIHandle;
                VisibleMCTag = null;
            }
            Enabled = false;
            mw.Save.ALLEvent.Remove(this);
        }
        /// <summary>
        /// 下次激活事件的日期
        /// </summary>
        public DateTime NextDate
        {
            get
            {
                Sub sub = Find("nextdate");
                if (sub == null)
                    return StartDate;
                else
                    return Convert.ToDateTime(sub.info);
            }
            set => FindorAdd("nextdate").info = value.ToString("yyyy/MM/dd HH:mm");
        }

        public IMCTag VisibleMCTag;
        /// <summary>
        /// 判断是否能够执行 如果能则运行Handle
        /// </summary>
        public async void TimeHandle(TimeSpan ts, IMainWindow mw)
        {
        TimeAgain:
            if (IsEnable(mw.Save.Now))
            {
                //触发条件
                if (NextDate.Ticks <= mw.Save.Now.Ticks)
                {
                    //其他条件判断(例如金钱等) 成功则执行 失败就跳过并计算下一次激活周期
                    if (StartDecied(mw))
                    {
                        //先计算
                        EventCalculat(mw);
                        //再执行th
                        HandleAction(ts, mw);
                        if (Visible == VisibleType.Message && VisibleMCTag == null)
                        {//如果是消息并且是第一次激活, 添加到侧边栏,需要弹窗,会有自动的弹窗小助手
                         //记得等一下,免得弹窗炸了
                            await mw.Dispatcher.InvokeAsync(new Action(() =>
                             {
                                 VisibleMCTag = mw.CreateMSGTag(this);
                             }));
                        }
                        if (VisibleMessage)
                        {   //如果是可见消息则弹窗
                            mw.Dispatcher.Invoke(new Action(() => mw.winMessageBoxShow(EventInfo, Message)));
                            if (VisibleMessageIsSingle)
                                VisibleMessage = false;
                        }

                        //拉相关事件 如果已经存在就不拉
                        foreach (var evs in Associated)
                            if (mw.Save.ALLEvent.Find(x => x.EventName == evs) == null)
                                mw.Core.EventBases.Find(x => x.EventName == evs).Create(mw, mw.Save.Now);
                    }

                    //计算下一次激活周期
                    switch (Period)
                    {
                        case 0:
                            NextDate = DateTime.MaxValue;
                            Enabled = false;
                            break;
                        default:
                            double per = Period;
                            if (PeriodRandom)
                                NextDate = NextDate.AddHours(per / 2 + per * Function.Rnd.NextDouble());
                            else
                                NextDate = NextDate.AddHours(per);
                            break;
                    }
                    //如果是最后一次运行
                    if (NextDate.Ticks > EndDate.Ticks && Visible != VisibleType.Message)
                        await Disposed(mw);
                    else
                        goto TimeAgain;//再运行次,如果时间间隔长但是激发次数短,则多来几下
                }
            }
        }

        /// <summary>
        /// 当该事件被点击时显示的 返回为true关闭点击部件
        /// </summary>
        public virtual bool HandleClick(IMainWindow mw)
        {
            //一般来讲点击的时候还没触发,所以不显示
            mw.winMessageBoxShow(EventInfo, Intor);
            return Visible == VisibleType.Message;
        }
        /// <summary>
        /// 当事件被激活后的处理
        /// </summary>
        public virtual void HandleAction(TimeSpan ts, IMainWindow mw) { }

    }
    /// <summary>
    /// 事件操作浮点值寄存类
    /// </summary>
    public class EventDoubleValue : Event
    {
        public EventDoubleValue(IMainWindow mw, Line line) : base(mw, line)
        {

        }
        /// <summary>
        /// 值
        /// </summary>
        public double Value
        {
            get => Find("value").InfoToDouble;
            set => Find("value").InfoToDouble = value;
        }
    }
    /// <summary>
    /// 事件操作int值寄存类
    /// </summary>
    public class EventIntValue : Event
    {
        public EventIntValue(IMainWindow mw, Line line) : base(mw, line)
        {

        }
        /// <summary>
        /// 值
        /// </summary>
        public int Value
        {
            get => Find("value").InfoToInt;
            set => Find("value").InfoToInt = value;
        }
    }
    /// <summary>
    /// 添加或者减少钱 事件
    /// </summary>
    public class EventMoney : EventDoubleValue
    {
        public EventMoney(IMainWindow mw, Line line) : base(mw, line)
        {

        }
        public override string ToIntor => base.ToIntor + "\n金钱:" + (Value >= 0 ? "+" : "") + Value;

        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            mw.Save.Money += Value;
        }
    }
    /// <summary>
    /// 操作健康 事件
    /// </summary>
    public class EventHealth : EventDoubleValue
    {
        public EventHealth(IMainWindow mw, Line line) : base(mw, line)
        {

        }
        public override string ToIntor => base.ToIntor + "\n健康:" + (Value >= 0 ? "+" : "") + Value;
        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            mw.Save.Health += Value;
        }
    }
    public class EventStrength : EventDoubleValue
    {
        public EventStrength(IMainWindow mw, Line line) : base(mw, line) { }
        public override string ToIntor => base.ToIntor + "\n体力:" + (Value >= 0 ? "+" : "") + Value;

        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            mw.Save.Strength += Value;
        }
    }

    public class EventTime : Event
    {
        public EventTime(IMainWindow mw, Line line) : base(mw, line) { }

        public TimeSpan Time
        {
            get => new TimeSpan(this[(gint)"d"], this[(gint)"h"], this[(gint)"m"], 0);
            set
            {
                this[(gint)"d"] = value.Days;
                this[(gint)"h"] = value.Hours;
                this[(gint)"m"] = value.Minutes;
            }
        }
    }
    public class EventTimePass : EventTime
    {
        public EventTimePass(IMainWindow mw, Line line) : base(mw, line) { }
        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            mw.Save.Now.Add(Time);
        }
    }

    public class EventXName : Event
    {
        public EventXName(IMainWindow mw, Line line) : base(mw, line)
        {

        }
        /// <summary>
        /// 事件操作名称, 指定要操作的事件
        /// </summary>
        public string XName => Find("xname").Info;
    }
    public class EventXNameDisposed : EventXName
    {
        public EventXNameDisposed(IMainWindow mw, Line line) : base(mw, line) { }
        public override async void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            string xname = XName;
            foreach (Event ev in mw.Save.ALLEvent.FindAll(x => x.EventName == xname))
                await ev.Disposed(mw);
        }
    }
    public class EventXNameDisenable : EventXName
    {
        public EventXNameDisenable(IMainWindow mw, Line line) : base(mw, line) { }
        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            string xname = XName;
            foreach (Event ev in mw.Save.ALLEvent.FindAll(x => x.EventName == xname))
                ev.Enabled = false;
        }
    }
    public class EventXNameEnable : EventXName
    {
        public EventXNameEnable(IMainWindow mw, Line line) : base(mw, line) { }
        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            string xname = XName;
            foreach (Event ev in mw.Save.ALLEvent.FindAll(x => x.EventName == xname))
                ev.Enabled = true;
        }
    }
    public class EventXNameFource : EventXName
    {
        public EventXNameFource(IMainWindow mw, Line line) : base(mw, line) { }
        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            string xname = XName;
            foreach (Event ev in mw.Save.ALLEvent.FindAll(x => x.EventName == xname))
                ev.TimeHandle(ts, mw);
        }
    }
    public class EventXNameStart : EventXName
    {
        public EventXNameStart(IMainWindow mw, Line line) : base(mw, line) { }
        public bool Fource
        {
            get => this[(gbol)"fource"];
            set => this[(gbol)"fource"] = value;
        }
        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            string xname = XName;
            if (!Fource)
            {
                Event ev = mw.Save.ALLEvent.Find(x => x.EventName == xname);
                if (ev != null)
                    return;
            }
            mw.Core.EventBases.Find(x => x.EventName == xname)?.Create(mw, mw.Save.Now);
        }
    }
    /// <summary>
    /// 显示CG的event
    /// </summary>
    public class EventCG : Event
    {
        public EventCG(IMainWindow mw, Line line) : base(mw, line)
        {

        }
        /// <summary>
        /// CG 名称 (通过imgres获取)
        /// </summary>
        public string CG
        {
            get => Find("cg").Info;
            set => Find("cg").Info = value;
        }
        /// <summary>
        /// CG 显示大小 如果为0 则为AUTO (尽量最大化)
        /// </summary>
        public int Width
        {
            get => this[(gint)"width"];
            set => this[(gint)"width"] = value;
        }
        /// <summary>
        /// CG 显示大小 如果为0 则为AUTO (尽量最大化)
        /// </summary>
        public int Height
        {
            get => this[(gint)"height"];
            set => this[(gint)"height"] = value;
        }
        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            //TODO:显示CG
        }
    }

}

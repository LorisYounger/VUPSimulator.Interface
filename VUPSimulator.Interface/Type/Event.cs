using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;
using LinePutScript.Localization.WPF;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        /// <returns>新的事件</returns>
        public Event Create(IMainWindow mw, DateTime startdate, params ISub[] setting)
        {
            ILine line = new Line(this);

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

            foreach (ISub sub in setting)
                line.AddorReplaceSub(sub);

            //创建事件并添加进事件链
            return Create(mw, line, Type);
        }
        /// <summary>
        /// 从line中创建新事件
        /// </summary>
        /// <param name="mw">主窗体</param>
        /// <param name="line">数据</param>
        /// <param name="type">事件类型</param>
        /// <returns></returns>
        public static Event Create(IMainWindow mw, ILine line, EventType type)
        {
            switch (type)
            {
                case EventType.property: return new EventProperty(mw, line);
                case EventType.timepass: return new EventTimePass(mw, line);
                case EventType.xnamedisenable: return new EventXNameDisenable(mw, line);
                case EventType.xnamedisposed: return new EventXNameDisposed(mw, line);
                case EventType.xnameenable: return new EventXNameEnable(mw, line);
                case EventType.xnamefource: return new EventXNameFource(mw, line);
                case EventType.none:
                default:
                    return new Event(mw, line);
            }
        }
        /// <summary>
        /// 从line中创建新事件
        /// </summary>
        /// <param name="mw">主窗体</param>
        /// <param name="line">数据</param>
        public static Event Create(IMainWindow mw, ILine line)
        {
            if (EventType.TryParse(line.info, true, out EventType eventtype))
            {
                return Create(mw, line, eventtype);
            }
            return Create(mw, line, EventType.none);
        }
        public enum EventType
        {
            none,//无 看上去啥也没有其实可以用setget进行很多操作

            property,//钱相关操作
            //bank,//银行(储蓄内金额)相关操作
            //debt,//负债 属于银行系统
            //这个以后会有专门的房子类型house,//房租(同时也是房子,加成)

            //数值操作相关类型
            health,//健康相关操作
            strength,//饱腹相关操作

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
        /// <summary>
        /// 从line中创建新事件
        /// </summary>
        /// <param name="line">数据</param>
        public EventBase(ILine line) : base(line)
        {

        }
        /// <summary>
        /// 初始化一个新的事件基础类实例
        /// </summary>
        /// <param name="name">事件的名称</param>
        /// <param name="type">事件类型，默认为none</param>
        /// <param name="visible">事件的可见性类型，默认为None</param>
        /// <param name="associated">伴生对象数组，默认为null</param>
        /// <param name="endAssociated">事件结束后伴生对象数组，默认为null</param>
        /// <param name="visibleMessage">是否在事件发生时显示消息，默认为false</param>
        /// <param name="visibleMessageIsSingle">如果显示消息，是否只显示一次，默认为false</param>
        /// <param name="eventInfo">事件信息（标题），默认为""</param>
        /// <param name="message">显示消息（事件触发后弹窗消息），默认为""</param>
        /// <param name="intor">事件注释（事件前显示的消息），默认为""</param>
        /// <param name="period">周期触发频率(单位小时)，默认为0</param>
        /// <param name="periodRandom">是否随机触发，默认为false</param>
        /// <param name="endTime">事件结束时间，单位小时，默认为0</param>
        /// <param name="autoTrigger">是否能够从事件库中触发该事件，默认为false</param>
        /// <param name="mSGType">消息类型，默认为notify</param>
        public EventBase(string name, string eventInfo = "", EventType type = EventType.none, string message = "", string intor = "",
            VisibleType visible = VisibleType.None, Function.MSGType mSGType = Function.MSGType.notify, string[] associated = null, string[] endAssociated = null, bool visibleMessage = false,
            bool visibleMessageIsSingle = false, int period = 0, bool periodRandom = false, int endTime = 0, bool autoTrigger = false)
            : base(name, "")
        {
            Type = type;
            Visible = visible;
            if (associated != null)
                Associated = associated;
            if (endAssociated != null)
                EndAssociated = endAssociated;
            VisibleMessage = visibleMessage;
            VisibleMessageIsSingle = visibleMessageIsSingle;
            EventInfo = eventInfo;
            Message = message;
            Intor = intor;
            Period = period;
            EndTime = endTime;
            AutoTrigger = autoTrigger;
            MSGType = mSGType;
        }

        /// <summary>
        /// Event的名字,方便从Event库呼出
        /// </summary>
        public string EventName
        {
            get => FindorAdd("name").Info;
            set => FindorAdd("name").Info = value;
        }
        /// <summary>
        /// 是否对玩家可见 (在侧边栏)
        /// </summary>
        public VisibleType Visible
        {
            get
            {
                ISub vissub = Find("visible");
                if (vissub == null)
                    return VisibleType.None;
                if (int.TryParse(vissub.info, out int p))
                    return (VisibleType)p;
                return (VisibleType)Enum.Parse(typeof(VisibleType), vissub.info, true);
            }
            set => FindorAdd("visible").info = value.ToString();
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
                ISub ass = Find("associated");
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
                ISub ass = Find("endassociated");
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
                ISub sub = Find("info");
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
                ISub sub = Find("message");
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
                ISub sub = Find("intor");
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
                ISub sub = Find("period");
                if (sub == null)
                    return 0;
                if (int.TryParse(sub.info, out int p))
                    return p;
                return (int)Enum.Parse(typeof(PeriodType), sub.info, true);
            }
            set => FindorAdd("period").info = value.ToString();
        }
        public PeriodType PeriodtoType
        {
            get
            {
                if (Find("period") == null)
                    return PeriodType.Single;
                if (int.TryParse(Find("period").info, out int p))
                    return (PeriodType)p;
                return (PeriodType)Enum.Parse(typeof(PeriodType), Find("period").info, true);
            }
            set => FindorAdd("period").info = ((int)value).ToString();
        }
        /// <summary>
        /// 周期触发频率 PeriodType 版本
        /// </summary>
        public string PeriodtoString
        {
            get
            {
                ISub sub = Find("period");
                if (sub == null)
                    return "Single";
                if (int.TryParse(sub.info, out int p))
                    return ((PeriodType)p).ToString();
                if (Enum.TryParse(sub.info, true, out PeriodType pt))
                    return pt.ToString();
                return sub.info;
            }
            set => FindorAdd("period").info = value.ToString();
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
        /// <summary>
        /// 消息类型,用于指定事件弹出的消息类型
        /// </summary>
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
            set => this[(gint)"msgtype"] = (int)value;
        }

        /// <summary>
        /// 执行该Event的计算
        /// </summary>
        public void EventCalculat(IMainWindow mw) => Function.Cal.DataCalculat(mw, this);
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
        public bool StartDecied(IMainWindow mw) => Function.Cal.DataEnable(mw, this);
        /// <summary>
        /// 告诉用户为啥可以通过这个Event/激活条件是什么
        /// </summary>
        public string WhyPass(IMainWindow mw) => Function.Cal.DataEnableString(mw, this);
    }


    public class Event : EventBase
    {

        public Event(IMainWindow mw, ILine line) : base(line)
        {
            if (mw == null)
                return;
            mw.Dispatcher.Invoke(() =>
            {
                //如果是日历,则一开始就进侧边栏
                if (Visible == VisibleType.Calendar)
                    VisibleMCTag = mw.CreateCALTag(this);
                else if (Visible == VisibleType.Message && Name == "gevent")//如果是游戏存档,也可以进侧边栏
                    VisibleMCTag = mw.CreateMSGTag(this);
            }, System.Windows.Threading.DispatcherPriority.Normal);
            //mw.TimeHandle += TimeHandle;
            mw.Save?.ALLEvent.Add(this);
        }
        public Event(IMainWindow mw, string name, params ISub[] setting) : base(new Line(name, "", "", setting))
        {
            if (mw == null)
                return;
            mw.Dispatcher.Invoke(() =>
            {
                //如果是日历,则一开始就进侧边栏
                if (Visible == VisibleType.Calendar)
                    VisibleMCTag = mw.CreateCALTag(this);
                else if (Visible == VisibleType.Message && Name == "gevent")//如果是游戏存档,也可以进侧边栏
                    VisibleMCTag = mw.CreateMSGTag(this);
            }, System.Windows.Threading.DispatcherPriority.Normal);
            //mw.TimeHandle += TimeHandle;
            mw.Save?.ALLEvent.Add(this);
        }
        public Event(IMainWindow mw, string name) : base(new Line(name, ""))
        {
            if (mw == null)
                return;
            mw.Dispatcher.Invoke(() =>
            {
                //如果是日历,则一开始就进侧边栏
                if (Visible == VisibleType.Calendar)
                    VisibleMCTag = mw.CreateCALTag(this);
                else if (Visible == VisibleType.Message && Name == "gevent")//如果是游戏存档,也可以进侧边栏
                    VisibleMCTag = mw.CreateMSGTag(this);
            }, System.Windows.Threading.DispatcherPriority.Normal);
            //mw.TimeHandle += TimeHandle;
            mw.Save?.ALLEvent.Add(this);
        }
        /// <summary>
        /// 初始化一个新的事件基础类实例
        /// </summary>
        /// <param name="name">事件的名称</param>
        /// <param name="type">事件类型，默认为none</param>
        /// <param name="visible">事件的可见性类型，默认为None</param>
        /// <param name="associated">伴生对象数组，默认为null</param>
        /// <param name="endAssociated">事件结束后伴生对象数组，默认为null</param>
        /// <param name="visibleMessage">是否在事件发生时显示消息，默认为false</param>
        /// <param name="visibleMessageIsSingle">如果显示消息，是否只显示一次，默认为false</param>
        /// <param name="eventInfo">事件信息（标题），默认为""</param>
        /// <param name="message">显示消息（事件触发后弹窗消息），默认为""</param>
        /// <param name="intor">事件注释（事件前显示的消息），默认为""</param>
        /// <param name="periodRandom">是否随机触发，默认为false</param>
        /// <param name="endTime">事件结束时间，单位小时，默认为0</param>
        /// <param name="autoTrigger">是否能够从事件库中触发该事件，默认为false</param>
        /// <param name="mSGType">消息类型，默认为notify</param>
        /// <param name="mw">主窗口</param>
        /// <param name="enabled">是否生效</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="period">周期触发频率(单位小时)，默认为0</param>
        /// <param name="startDate">开始日期</param>
        public Event(IMainWindow mw, string name,
            string eventInfo = "", EventType type = EventType.none, string message = "", string intor = "",
            DateTime startDate = default, DateTime endDate = default, bool enabled = true,
            VisibleType visible = VisibleType.None, Function.MSGType mSGType = Function.MSGType.notify, string[] associated = null, string[] endAssociated = null, bool visibleMessage = false,
            bool visibleMessageIsSingle = false, int period = 0, bool periodRandom = false, int endTime = 0, bool autoTrigger = false
            ) : base(name, eventInfo, type, message, intor, visible, mSGType, associated, endAssociated, visibleMessage, visibleMessageIsSingle, period, periodRandom, endTime, autoTrigger)
        {
            StartDate = startDate == default ? Function.DateMaxValue : startDate;
            EndDate = endDate == default ? Function.DateMaxValue : endDate;
            Enabled = enabled;
            if (mw == null)
                return;
            mw.Dispatcher.Invoke(() =>
            {
                //如果是日历,则一开始就进侧边栏
                if (Visible == VisibleType.Calendar)
                    VisibleMCTag = mw.CreateCALTag(this);
                else if (Visible == VisibleType.Message && Name == "gevent")//如果是游戏存档,也可以进侧边栏
                    VisibleMCTag = mw.CreateMSGTag(this);
            }, System.Windows.Threading.DispatcherPriority.Normal);
            //mw.TimeHandle += TimeHandle;
            mw.Save?.ALLEvent.Add(this);
        }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                ISub sub = Find("startdate");
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
                ISub sub = Find("enddate");
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
        public virtual string ToIntor => $"{"开始日期".Translate()}:{StartDate.ToShortDateString()}\n{(EndDate == Function.DateMaxValue ? "" : "结束日期".Translate() + ':' + EndDate.ToShortDateString() + '\n')}" +
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
        public void Disposed(IMainWindow mw)
        {
            //在销毁前执行ENDASS
            //拉相关事件 如果已经存在就不拉
            foreach (var evs in EndAssociated)
                if (mw.Save.ALLEvent.Find(x => x.EventName == evs) == null)
                    mw.Core.EventBases.Find(x => x.EventName == evs).Create(mw, mw.Save.Base.Now);

            if (VisibleMCTag != null)
            {
                mw.Dispatcher.Invoke(() => mw.RemoveIMCTag(VisibleMCTag), System.Windows.Threading.DispatcherPriority.Background);
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
                ISub sub = Find("nextdate");
                if (sub == null || !DateTime.TryParse(sub.info, out var date))
                    return StartDate;
                else
                    return date;
            }
            set => FindorAdd("nextdate").info = value.ToString("yyyy/MM/dd HH:mm");
        }

        public IMCTag VisibleMCTag;
        /// <summary>
        /// 判断是否能够执行 如果能则运行Handle 请不要添加改Handle到mw.TimeHandle中 mw.Save.ALLEvent 会自动运行
        /// </summary>
        public async void TimeHandle(TimeSpan ts, IMainWindow mw)
        {
        TimeAgain:
            if (IsEnable(mw.Save.Base.Now))
            {
                //触发条件
                if (NextDate.Ticks <= mw.Save.Base.Now.Ticks)
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
                            await mw.Dispatcher.InvokeAsync(() =>
                             {
                                 VisibleMCTag = mw.CreateMSGTag(this);
                             }, System.Windows.Threading.DispatcherPriority.Normal);
                        }
                        if (VisibleMessage)
                        {   //如果是可见消息则弹窗
                            mw.Dispatcher.Invoke(() => mw.winMessageBoxShow(EventInfo, Message), System.Windows.Threading.DispatcherPriority.Normal);
                            if (VisibleMessageIsSingle)
                                VisibleMessage = false;
                        }

                        //拉相关事件 如果已经存在就不拉
                        foreach (var evs in Associated)
                            if (mw.Save.ALLEvent.Find(x => x.EventName == evs) == null)
                                mw.Core.EventBases.Find(x => x.EventName == evs).Create(mw, mw.Save.Base.Now);
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
                        Disposed(mw);
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
    /// 添加或者减少属性 事件
    /// </summary>
    public class EventProperty : Event
    {
        public EventProperty(IMainWindow mw, ILine line) : base(mw, line)
        {

        }
        /// <summary>
        /// 初始化一个新的 添加或者减少属性 事件
        /// </summary>
        /// <param name="name">事件的名称</param>
        /// <param name="type">事件类型，默认为none</param>
        /// <param name="visible">事件的可见性类型，默认为None</param>
        /// <param name="associated">伴生对象数组，默认为null</param>
        /// <param name="endAssociated">事件结束后伴生对象数组，默认为null</param>
        /// <param name="visibleMessage">是否在事件发生时显示消息，默认为false</param>
        /// <param name="visibleMessageIsSingle">如果显示消息，是否只显示一次，默认为false</param>
        /// <param name="eventInfo">事件信息（标题），默认为""</param>
        /// <param name="message">显示消息（事件触发后弹窗消息），默认为""</param>
        /// <param name="intor">事件注释（事件前显示的消息），默认为""</param>
        /// <param name="periodRandom">是否随机触发，默认为false</param>
        /// <param name="endTime">事件结束时间，单位小时，默认为0</param>
        /// <param name="autoTrigger">是否能够从事件库中触发该事件，默认为false</param>
        /// <param name="mSGType">消息类型，默认为notify</param>
        /// <param name="mw">主窗口</param>
        /// <param name="enabled">是否生效</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="health">健康</param>
        /// <param name="money">金钱</param>
        /// <param name="notifyText">通知文本</param>
        /// <param name="pclip">剪辑</param>
        /// <param name="pdraw">绘画</param>
        /// <param name="pgame">游戏</param>
        /// <param name="pidear">思维</param>
        /// <param name="pimage">修图</param>
        /// <param name="poperate">运营</param>
        /// <param name="pprogram">程序</param>
        /// <param name="psong">声乐</param>
        /// <param name="pspeak">口才</param>
        /// <param name="strength">精力</param>
        /// <param name="strengthFood">食物</param>
        /// <param name="period">周期触发频率(单位小时)，默认为0</param>
        /// <param name="strengthSleep">睡眠</param>
        public EventProperty(IMainWindow mw, string name, string eventInfo = "", EventType type = EventType.none, string message = "", string intor = "",
            double money = 0, double health = 0, double strength = 0, double strengthSleep = 0, double strengthFood = 0,
            string notifyText = null, double pidear = 0, double pspeak = 0, double poperate = 0, double pimage = 0, double pclip = 0, double pdraw = 0,
            double pprogram = 0, double pgame = 0, double psong = 0, bool visibleMessageIsSingle = false, int period = 0, bool periodRandom = false, int endTime = 0, bool autoTrigger = false,
            DateTime startDate = default, DateTime endDate = default, bool enabled = true,
            VisibleType visible = VisibleType.None, Function.MSGType mSGType = Function.MSGType.notify, string[] associated = null, string[] endAssociated = null, bool visibleMessage = false
           ) : base(mw, name, eventInfo, type, message, intor, startDate, endDate, enabled, visible, mSGType, associated, endAssociated, visibleMessage, visibleMessageIsSingle, period, periodRandom, endTime, autoTrigger)
        {
            Money = money;
            Health = health;
            Strength = strength;
            StrengthSleep = strengthSleep;
            StrengthFood = strengthFood;
            if (notifyText != null)
                NotifyText = notifyText;
            Pidear = pidear;
            Pspeak = pspeak;
            Poperate = poperate;
            Pimage = pimage;
            Pclip = pclip;
            Pdraw = pdraw;
            Pprogram = pprogram;
            Pgame = pgame;
            Psong = psong;
        }

        public override string ToIntor
        {
            get
            {
                StringBuilder sb = new StringBuilder(base.ToIntor);
                if (Money != 0)
                    sb.Append('\n').Append("金钱".Translate()).Append(':').Append(Money >= 0 ? "+" : "").Append(Money);
                if (Health != 0)
                    sb.Append('\n').Append("健康".Translate()).Append(':').Append(Health >= 0 ? "+" : "").Append(Health);
                if (Strength != 0)
                    sb.Append('\n').Append("饱腹".Translate()).Append(':').Append(Strength >= 0 ? "+" : "").Append(Strength);
                if (StrengthSleep != 0)
                    sb.Append('\n').Append("睡眠".Translate()).Append(':').Append(StrengthSleep >= 0 ? "+" : "").Append(StrengthSleep);
                if (StrengthFood != 0)
                    sb.Append('\n').Append("食物".Translate()).Append(':').Append(StrengthFood >= 0 ? "+" : "").Append(StrengthFood);
                if (Pidear != 0)
                    sb.Append('\n').Append("思维".Translate()).Append(':').Append(Pidear >= 0 ? "+" : "").Append(Pidear);
                if (Pspeak != 0)
                    sb.Append('\n').Append("口才".Translate()).Append(':').Append(Pspeak >= 0 ? "+" : "").Append(Pspeak);
                if (Poperate != 0)
                    sb.Append('\n').Append("运营".Translate()).Append(':').Append(Poperate >= 0 ? "+" : "").Append(Poperate);
                if (Pimage != 0)
                    sb.Append('\n').Append("修图".Translate()).Append(':').Append(Pimage >= 0 ? "+" : "").Append(Pimage);
                if (Pclip != 0)
                    sb.Append('\n').Append("剪辑".Translate()).Append(':').Append(Pclip >= 0 ? "+" : "").Append(Pclip);
                if (Pdraw != 0)
                    sb.Append('\n').Append("绘画".Translate()).Append(':').Append(Pdraw >= 0 ? "+" : "").Append(Pdraw);
                if (Pprogram != 0)
                    sb.Append('\n').Append("程序".Translate()).Append(':').Append(Pprogram >= 0 ? "+" : "").Append(Pprogram);
                if (Pgame != 0)
                    sb.Append('\n').Append("游戏".Translate()).Append(':').Append(Pgame >= 0 ? "+" : "").Append(Pgame);
                if (Psong != 0)
                    sb.Append('\n').Append("声乐".Translate()).Append(':').Append(Psong >= 0 ? "+" : "").Append(Psong);

                return sb.ToString();
            }
        }
        /// <summary>
        /// 资金
        /// </summary>
        public double Money
        {
            get => this[(gdbe)"money"];
            set => this[(gdbe)"money"] = value;
        }
        /// <summary>
        /// 健康
        /// </summary>
        public double Health { get => this[(gdbe)"health"]; set => this[(gdbe)"health"] = value; }
        /// <summary>
        /// 精力
        /// </summary>
        public double Strength { get => this[(gdbe)"strength"]; set => this[(gdbe)"strength"] = value; }
        /// <summary>
        /// 精力:睡眠程度
        /// </summary>
        public double StrengthSleep { get => this[(gdbe)"strengthsleep"]; set => this[(gdbe)"strengthsleep"] = value; }
        /// <summary>
        /// 精力:食物
        /// </summary>
        public double StrengthFood { get => this[(gdbe)"strengthfood"]; set => this[(gdbe)"strengthfood"] = value; }
        /// <summary>
        /// 属性: 思维
        /// </summary>
        public double Pidear { get => this[(gdbe)"pidear"]; set => this[(gdbe)"pidear"] = value; }
        /// <summary>
        /// 属性: 口才
        /// </summary>
        public double Pspeak { get => this[(gdbe)"pspeak"]; set => this[(gdbe)"pspeak"] = value; }
        /// <summary>
        /// 属性: 运营
        /// </summary>
        public double Poperate { get => this[(gdbe)"poperate"]; set => this[(gdbe)"poperate"] = value; }
        /// <summary>
        /// 属性: 修图
        /// </summary>
        public double Pimage { get => this[(gdbe)"pimage"]; set => this[(gdbe)"pimage"] = value; }
        /// <summary>
        /// 属性: 剪辑
        /// </summary>
        public double Pclip { get => this[(gdbe)"pclip"]; set => this[(gdbe)"pclip"] = value; }
        /// <summary>
        /// 属性: 绘画
        /// </summary>
        public double Pdraw { get => this[(gdbe)"pdraw"]; set => this[(gdbe)"pdraw"] = value; }
        /// <summary>
        /// 属性: 程序
        /// </summary>
        public double Pprogram { get => this[(gdbe)"pprogram"]; set => this[(gdbe)"pprogram"] = value; }
        /// <summary>
        /// 属性: 游戏
        /// </summary>
        public double Pgame { get => this[(gdbe)"pgame"]; set => this[(gdbe)"pgame"] = value; }
        /// <summary>
        /// 属性: 声乐
        /// </summary>
        public double Psong { get => this[(gdbe)"psong"]; set => this[(gdbe)"psong"] = value; }

        public string NotifyText
        {
            get => this.GetString("notifytext", "由事件{0}发起".Translate(EventName));
            set => this[(gstr)"notifytext"] = value;
        }

        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            mw.Save.Base.Money += Money;
            mw.Save.Base.Health += Health;
            if (Strength < 0)
            {
                mw.Save.StrengthRemove(Strength, 1, NotifyText);
            }
            else
            {
                mw.Save.Base.StrengthSleep += Strength;
                mw.Save.Base.StrengthFood += Strength;
            }
            mw.Save.Base.StrengthSleep += StrengthSleep;
            mw.Save.Base.StrengthFood += StrengthFood;

            mw.Save.Base.Pidear += Pidear;
            mw.Save.Base.Pspeak += Pspeak;
            mw.Save.Base.Poperate += Poperate;
            mw.Save.Base.Pimage += Pimage;
            mw.Save.Base.Pclip += Pclip;
            mw.Save.Base.Pdraw += Pdraw;
            mw.Save.Base.Pprogram += Pprogram;
            mw.Save.Base.Pgame += Pgame;
            mw.Save.Base.Psong += Psong;
        }
    }

    /// <summary>
    /// 时间跳过时间
    /// </summary>
    public class EventTimePass : Event
    {
        public EventTimePass(IMainWindow mw, ILine line) : base(mw, line) { }
        /// <summary>
        /// 初始化一个新的 时间跳过 事件
        /// </summary>
        /// <param name="name">事件的名称</param>
        /// <param name="type">事件类型，默认为none</param>
        /// <param name="visible">事件的可见性类型，默认为None</param>
        /// <param name="associated">伴生对象数组，默认为null</param>
        /// <param name="endAssociated">事件结束后伴生对象数组，默认为null</param>
        /// <param name="visibleMessage">是否在事件发生时显示消息，默认为false</param>
        /// <param name="visibleMessageIsSingle">如果显示消息，是否只显示一次，默认为false</param>
        /// <param name="eventInfo">事件信息（标题），默认为""</param>
        /// <param name="message">显示消息（事件触发后弹窗消息），默认为""</param>
        /// <param name="intor">事件注释（事件前显示的消息），默认为""</param>
        /// <param name="periodRandom">是否随机触发，默认为false</param>
        /// <param name="endTime">事件结束时间，单位小时，默认为0</param>
        /// <param name="autoTrigger">是否能够从事件库中触发该事件，默认为false</param>
        /// <param name="mSGType">消息类型，默认为notify</param>
        /// <param name="mw">主窗口</param>
        /// <param name="enabled">是否生效</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="time">时间</param>
        public EventTimePass(IMainWindow mw, string name, TimeSpan time, string eventInfo = "", EventType type = EventType.none, string message = "", string intor = "",
           bool visibleMessageIsSingle = false, int period = 0, bool periodRandom = false, int endTime = 0, bool autoTrigger = false,
           DateTime startDate = default, DateTime endDate = default, bool enabled = true,
           VisibleType visible = VisibleType.None, Function.MSGType mSGType = Function.MSGType.notify, string[] associated = null, string[] endAssociated = null, bool visibleMessage = false
            ) : base(mw, name, eventInfo, type, message, intor, startDate, endDate, enabled, visible, mSGType, associated, endAssociated, visibleMessage, visibleMessageIsSingle, period, periodRandom, endTime, autoTrigger)
        {
            Time = time;
        }
        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            mw.Save.Base.Now.Add(Time);
        }
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

    public class EventXName : Event
    {
        public EventXName(IMainWindow mw, ILine line) : base(mw, line)
        {

        }
        /// <summary>
        /// 初始化一个新的 时间跳过 事件
        /// </summary>
        /// <param name="name">事件的名称</param>
        /// <param name="type">事件类型，默认为none</param>
        /// <param name="visible">事件的可见性类型，默认为None</param>
        /// <param name="associated">伴生对象数组，默认为null</param>
        /// <param name="endAssociated">事件结束后伴生对象数组，默认为null</param>
        /// <param name="visibleMessage">是否在事件发生时显示消息，默认为false</param>
        /// <param name="visibleMessageIsSingle">如果显示消息，是否只显示一次，默认为false</param>
        /// <param name="eventInfo">事件信息（标题），默认为""</param>
        /// <param name="message">显示消息（事件触发后弹窗消息），默认为""</param>
        /// <param name="intor">事件注释（事件前显示的消息），默认为""</param>
        /// <param name="periodRandom">是否随机触发，默认为false</param>
        /// <param name="endTime">事件结束时间，单位小时，默认为0</param>
        /// <param name="autoTrigger">是否能够从事件库中触发该事件，默认为false</param>
        /// <param name="mSGType">消息类型，默认为notify</param>
        /// <param name="mw">主窗口</param>
        /// <param name="enabled">是否生效</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="xname">事件操作名称, 指定要操作的事件</param>
        public EventXName(IMainWindow mw, string name, string xname, string eventInfo = "", EventType type = EventType.none, string message = "", string intor = "",
            DateTime startDate = default, DateTime endDate = default, bool enabled = true,
            VisibleType visible = VisibleType.None, Function.MSGType mSGType = Function.MSGType.notify, string[] associated = null, string[] endAssociated = null, bool visibleMessage = false,
            bool visibleMessageIsSingle = false, int period = 0, bool periodRandom = false, int endTime = 0, bool autoTrigger = false
            ) : base(mw, name, eventInfo, type, message, intor, startDate, endDate, enabled, visible, mSGType, associated, endAssociated, visibleMessage, visibleMessageIsSingle, period, periodRandom, endTime, autoTrigger)
        {
            XName = xname;
        }
        /// <summary>
        /// 事件操作名称, 指定要操作的事件
        /// </summary>
        public string XName
        {
            get => this.GetString("xname", "");
            set => this[(gstr)"xname"] = value;
        }
    }
    /// <summary>
    /// 销毁指定的事件
    /// </summary>
    public class EventXNameDisposed : EventXName
    {
        public EventXNameDisposed(IMainWindow mw, ILine line) : base(mw, line) { }
        /// <summary>
        /// 初始化一个新的 销毁指定的事件
        /// </summary>
        /// <param name="name">事件的名称</param>
        /// <param name="type">事件类型，默认为none</param>
        /// <param name="visible">事件的可见性类型，默认为None</param>
        /// <param name="associated">伴生对象数组，默认为null</param>
        /// <param name="endAssociated">事件结束后伴生对象数组，默认为null</param>
        /// <param name="visibleMessage">是否在事件发生时显示消息，默认为false</param>
        /// <param name="visibleMessageIsSingle">如果显示消息，是否只显示一次，默认为false</param>
        /// <param name="eventInfo">事件信息（标题），默认为""</param>
        /// <param name="message">显示消息（事件触发后弹窗消息），默认为""</param>
        /// <param name="intor">事件注释（事件前显示的消息），默认为""</param>
        /// <param name="periodRandom">是否随机触发，默认为false</param>
        /// <param name="endTime">事件结束时间，单位小时，默认为0</param>
        /// <param name="autoTrigger">是否能够从事件库中触发该事件，默认为false</param>
        /// <param name="mSGType">消息类型，默认为notify</param>
        /// <param name="mw">主窗口</param>
        /// <param name="enabled">是否生效</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="xname">事件操作名称, 指定要操作的事件</param>
        public EventXNameDisposed(IMainWindow mw, string name, string xname, string eventInfo = "", EventType type = EventType.none, string message = "", string intor = "",
             bool visibleMessageIsSingle = false, int period = 0, bool periodRandom = false, int endTime = 0, bool autoTrigger = false,
           DateTime startDate = default, DateTime endDate = default, bool enabled = true,
           VisibleType visible = VisibleType.None, Function.MSGType mSGType = Function.MSGType.notify, string[] associated = null, string[] endAssociated = null, bool visibleMessage = false
            ) : base(mw, name, xname, eventInfo, type, message, intor, startDate, endDate, enabled, visible, mSGType, associated, endAssociated, visibleMessage, visibleMessageIsSingle, period, periodRandom, endTime, autoTrigger)
        { }
        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            string xname = XName;
            foreach (Event ev in mw.Save.ALLEvent.FindAll(x => x.EventName == xname))
                ev.Disposed(mw);
        }
    }
    /// <summary>
    /// 关闭指定的事件
    /// </summary>
    public class EventXNameDisenable : EventXName
    {
        /// <summary>
        /// 初始化一个新的 关闭指定的事件
        /// </summary>
        /// <param name="name">事件的名称</param>
        /// <param name="type">事件类型，默认为none</param>
        /// <param name="visible">事件的可见性类型，默认为None</param>
        /// <param name="associated">伴生对象数组，默认为null</param>
        /// <param name="endAssociated">事件结束后伴生对象数组，默认为null</param>
        /// <param name="visibleMessage">是否在事件发生时显示消息，默认为false</param>
        /// <param name="visibleMessageIsSingle">如果显示消息，是否只显示一次，默认为false</param>
        /// <param name="eventInfo">事件信息（标题），默认为""</param>
        /// <param name="message">显示消息（事件触发后弹窗消息），默认为""</param>
        /// <param name="intor">事件注释（事件前显示的消息），默认为""</param>
        /// <param name="periodRandom">是否随机触发，默认为false</param>
        /// <param name="endTime">事件结束时间，单位小时，默认为0</param>
        /// <param name="autoTrigger">是否能够从事件库中触发该事件，默认为false</param>
        /// <param name="mSGType">消息类型，默认为notify</param>
        /// <param name="mw">主窗口</param>
        /// <param name="enabled">是否生效</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="xname">事件操作名称, 指定要操作的事件</param>
        public EventXNameDisenable(IMainWindow mw, string name, string xname, string eventInfo = "", EventType type = EventType.none, string message = "", string intor = "",
             bool visibleMessageIsSingle = false, int period = 0, bool periodRandom = false, int endTime = 0, bool autoTrigger = false,
           DateTime startDate = default, DateTime endDate = default, bool enabled = true,
           VisibleType visible = VisibleType.None, Function.MSGType mSGType = Function.MSGType.notify, string[] associated = null, string[] endAssociated = null, bool visibleMessage = false
            ) : base(mw, name, xname, eventInfo, type, message, intor, startDate, endDate, enabled, visible, mSGType, associated, endAssociated, visibleMessage, visibleMessageIsSingle, period, periodRandom, endTime, autoTrigger)
        { }
        public EventXNameDisenable(IMainWindow mw, ILine line) : base(mw, line) { }
        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            string xname = XName;
            foreach (Event ev in mw.Save.ALLEvent.FindAll(x => x.EventName == xname))
                ev.Enabled = false;
        }
    }
    /// <summary>
    /// 启用指定的事件
    /// </summary>
    public class EventXNameEnable : EventXName
    {
        /// <summary>
        /// 初始化一个新的 启用指定的事件
        /// </summary>
        /// <param name="name">事件的名称</param>
        /// <param name="type">事件类型，默认为none</param>
        /// <param name="visible">事件的可见性类型，默认为None</param>
        /// <param name="associated">伴生对象数组，默认为null</param>
        /// <param name="endAssociated">事件结束后伴生对象数组，默认为null</param>
        /// <param name="visibleMessage">是否在事件发生时显示消息，默认为false</param>
        /// <param name="visibleMessageIsSingle">如果显示消息，是否只显示一次，默认为false</param>
        /// <param name="eventInfo">事件信息（标题），默认为""</param>
        /// <param name="message">显示消息（事件触发后弹窗消息），默认为""</param>
        /// <param name="intor">事件注释（事件前显示的消息），默认为""</param>
        /// <param name="periodRandom">是否随机触发，默认为false</param>
        /// <param name="endTime">事件结束时间，单位小时，默认为0</param>
        /// <param name="autoTrigger">是否能够从事件库中触发该事件，默认为false</param>
        /// <param name="mSGType">消息类型，默认为notify</param>
        /// <param name="mw">主窗口</param>
        /// <param name="enabled">是否生效</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="xname">事件操作名称, 指定要操作的事件</param>
        public EventXNameEnable(IMainWindow mw, string name, string xname, string eventInfo = "", EventType type = EventType.none, string message = "", string intor = "",
             bool visibleMessageIsSingle = false, int period = 0, bool periodRandom = false, int endTime = 0, bool autoTrigger = false,
           DateTime startDate = default, DateTime endDate = default, bool enabled = true,
           VisibleType visible = VisibleType.None, Function.MSGType mSGType = Function.MSGType.notify, string[] associated = null, string[] endAssociated = null, bool visibleMessage = false
            ) : base(mw, name, xname, eventInfo, type, message, intor, startDate, endDate, enabled, visible, mSGType, associated, endAssociated, visibleMessage, visibleMessageIsSingle, period, periodRandom, endTime, autoTrigger)
        { }
        public EventXNameEnable(IMainWindow mw, ILine line) : base(mw, line) { }
        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            string xname = XName;
            foreach (Event ev in mw.Save.ALLEvent.FindAll(x => x.EventName == xname))
                ev.Enabled = true;
        }
    }
    /// <summary>
    /// 强制执行指定的事件
    /// </summary>
    public class EventXNameFource : EventXName
    {
        /// <summary>
        /// 初始化一个新的 强制执行指定的事件
        /// </summary>
        /// <param name="name">事件的名称</param>
        /// <param name="type">事件类型，默认为none</param>
        /// <param name="visible">事件的可见性类型，默认为None</param>
        /// <param name="associated">伴生对象数组，默认为null</param>
        /// <param name="endAssociated">事件结束后伴生对象数组，默认为null</param>
        /// <param name="visibleMessage">是否在事件发生时显示消息，默认为false</param>
        /// <param name="visibleMessageIsSingle">如果显示消息，是否只显示一次，默认为false</param>
        /// <param name="eventInfo">事件信息（标题），默认为""</param>
        /// <param name="message">显示消息（事件触发后弹窗消息），默认为""</param>
        /// <param name="intor">事件注释（事件前显示的消息），默认为""</param>
        /// <param name="periodRandom">是否随机触发，默认为false</param>
        /// <param name="endTime">事件结束时间，单位小时，默认为0</param>
        /// <param name="autoTrigger">是否能够从事件库中触发该事件，默认为false</param>
        /// <param name="mSGType">消息类型，默认为notify</param>
        /// <param name="mw">主窗口</param>
        /// <param name="enabled">是否生效</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="xname">事件操作名称, 指定要操作的事件</param>
        public EventXNameFource(IMainWindow mw, string name, string xname, string eventInfo = "", EventType type = EventType.none, string message = "", string intor = "",
             bool visibleMessageIsSingle = false, int period = 0, bool periodRandom = false, int endTime = 0, bool autoTrigger = false,
           DateTime startDate = default, DateTime endDate = default, bool enabled = true,
           VisibleType visible = VisibleType.None, Function.MSGType mSGType = Function.MSGType.notify, string[] associated = null, string[] endAssociated = null, bool visibleMessage = false
            ) : base(mw, name, xname, eventInfo, type, message, intor, startDate, endDate, enabled, visible, mSGType, associated, endAssociated, visibleMessage, visibleMessageIsSingle, period, periodRandom, endTime, autoTrigger)
        { }
        public EventXNameFource(IMainWindow mw, ILine line) : base(mw, line) { }
        public override void HandleAction(TimeSpan ts, IMainWindow mw)
        {
            string xname = XName;
            foreach (Event ev in mw.Save.ALLEvent.FindAll(x => x.EventName == xname))
                ev.TimeHandle(ts, mw);
        }
    }
    /// <summary>
    /// 开始指定的事件
    /// </summary>
    public class EventXNameStart : EventXName
    {
        /// <summary>
        /// 初始化一个新的 开始指定的事件
        /// </summary>
        /// <param name="name">事件的名称</param>
        /// <param name="type">事件类型，默认为none</param>
        /// <param name="visible">事件的可见性类型，默认为None</param>
        /// <param name="associated">伴生对象数组，默认为null</param>
        /// <param name="endAssociated">事件结束后伴生对象数组，默认为null</param>
        /// <param name="visibleMessage">是否在事件发生时显示消息，默认为false</param>
        /// <param name="visibleMessageIsSingle">如果显示消息，是否只显示一次，默认为false</param>
        /// <param name="eventInfo">事件信息（标题），默认为""</param>
        /// <param name="message">显示消息（事件触发后弹窗消息），默认为""</param>
        /// <param name="intor">事件注释（事件前显示的消息），默认为""</param>
        /// <param name="periodRandom">是否随机触发，默认为false</param>
        /// <param name="endTime">事件结束时间，单位小时，默认为0</param>
        /// <param name="autoTrigger">是否能够从事件库中触发该事件，默认为false</param>
        /// <param name="mSGType">消息类型，默认为notify</param>
        /// <param name="mw">主窗口</param>
        /// <param name="enabled">是否生效</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="xname">事件操作名称, 指定要操作的事件</param>
        public EventXNameStart(IMainWindow mw, string name, string xname, string eventInfo = "", EventType type = EventType.none, string message = "", string intor = "",
             bool visibleMessageIsSingle = false, int period = 0, bool periodRandom = false, int endTime = 0, bool autoTrigger = false,
           DateTime startDate = default, DateTime endDate = default, bool enabled = true,
           VisibleType visible = VisibleType.None, Function.MSGType mSGType = Function.MSGType.notify, string[] associated = null, string[] endAssociated = null, bool visibleMessage = false
            ) : base(mw, name, xname, eventInfo, type, message, intor, startDate, endDate, enabled, visible, mSGType, associated, endAssociated, visibleMessage, visibleMessageIsSingle, period, periodRandom, endTime, autoTrigger)
        { }
        public EventXNameStart(IMainWindow mw, ILine line) : base(mw, line) { }
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
            mw.Core.EventBases.Find(x => x.EventName == xname)?.Create(mw, mw.Save.Base.Now);
        }
    }
    /// <summary>
    /// 显示CG的事件
    /// </summary>
    public class EventCG : Event
    {
        public EventCG(IMainWindow mw, ILine line) : base(mw, line)
        {

        }
        /// <summary>
        /// CG 名称 (通过imgres获取)
        /// </summary>
        public string CG
        {
            get => Find("cg").Info;
            set => FindorAdd("cg").Info = value;
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

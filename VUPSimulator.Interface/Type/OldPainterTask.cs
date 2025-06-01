using LinePutScript;
using LinePutScript.Converter;
using LinePutScript.Localization.WPF;
using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static VUPSimulator.Interface.Item;
using static VUPSimulator.Interface.OldPainterAuthor;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 老画师绘画任务
    /// </summary>
    public class OldPainterTask : NotifyPropertyChangedBase
    {
        /// <summary>
        /// 进度
        /// </summary>
        public enum PainterProgress
        {
            Begin,
            Progressing,
            Completed,
        }

        /// <summary>
        /// 稿件类型。
        /// </summary>
        public enum PainterCategory
        {
            /// <summary>
            /// 立绘
            /// </summary>
            L2DPaint,
            /// <summary>
            /// 建模
            /// </summary>
            L2DModel,
            /// <summary>
            /// 立绘优化
            /// </summary>
            L2DPaintUpdate,
            /// <summary>
            /// 建模优化
            /// </summary>
            L2DModelUpdate,
            /// <summary>
            /// 插图
            /// </summary>
            Illustration,
            /// <summary>
            /// 头像
            /// </summary>
            Profile,
            /// <summary>
            /// 表情包
            /// </summary>
            Expression,
        }
        public IMainWindow mw;
        public ILine SaveData;
        /// <summary>
        /// 任务ID,用于定位用的,是随机数
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 加载任务数据
        /// </summary>
        /// <param name="mw"></param>
        /// <param name="savedata">任务数据和子数据(用于保存)</param>
        /// <param name="save">游戏存档,考虑到这个项目需要在初始化的时候调用,那时候MW还没有SAVE</param>
        public OldPainterTask(IMainWindow mw, ILine savedata, ISave save = null)
        {
            this.mw = mw;
            SaveData = savedata;
            Category = (PainterCategory)savedata[(gint)"category"];
            Title = savedata[(gstr)"title"];
            Id = savedata.InfoToInt;
            ImageShotPath = savedata[(gstr)"imagepath"];
            ImageShot = mw.Core.ImageSources.FindImage(ImageShotPath, "oldpainter_wait_" + Category);
            CreateTime = savedata[(gdat)"createtime"];
            StartTime = savedata[(gdat)"starttime"];
            EndTime = savedata[(gdat)"endtime"];
            ExpectedAmountFrom = savedata[(gdbe)"expectedamountfrom"];
            ExpectedAmountTo = savedata[(gdbe)"expectedamountto"];
            Content = savedata[(gstr)"content"];
            IsArchived = savedata[(gbol)"isarch"];
            save ??= mw.Save;
            //添加Content
            switch (Category)
            {
                case PainterCategory.L2DPaintUpdate:
                case PainterCategory.L2DModel:
                case PainterCategory.L2DModelUpdate:
                    //绑定的Content是用户拥有的L2D文件
                    ContentItem = (Item_Paint)save.Items.Find(x => x.Type == ItemType.l2d && x.ItemIdentifier == Content);
                    break;
            }
            //加载应征和邀请的画师
            foreach (string AuthID in savedata["ALLAuthorID"].GetInfos())
            {
                OldPainterAuthor author = mw.Core.Authors.Find(x => x.Identy == AuthID);
                if (author != null)
                {
                    AuthorInTask ait = new AuthorInTask(this, author, save: save);
                    switch (ait.AuthorType)
                    {
                        //case AuthorInTask.AuthorInTaskType.Recommended:
                        //    RecommendedPainters.Add(ait);//虽然说推荐的画师不会被保存,这里是容错
                        //    break;
                        case AuthorInTask.AuthorInTaskType.Recruit:
                            RecruitPainters.Add(ait);
                            break;
                        case AuthorInTask.AuthorInTaskType.Invite:
                            InvitePainters.Add(ait);
                            break;
                        case AuthorInTask.AuthorInTaskType.Parted:
                            PartedPainters.Add(ait);
                            break;
                    }
                }
            }

        }
        /// <summary>
        /// 生成推荐的画师列表
        /// </summary>
        public void GenRecommend()
        {
            RecommendedPainters.Clear();
            SkillType sk;
            switch (Category)
            {
                case PainterCategory.L2DPaint:
                case PainterCategory.L2DPaintUpdate:
                    sk = OldPainterAuthor.SkillType.L2DPaint;
                    break;
                case PainterCategory.L2DModel:
                case PainterCategory.L2DModelUpdate:
                    sk = OldPainterAuthor.SkillType.L2DModel;
                    break;
                case PainterCategory.Illustration:
                    sk = OldPainterAuthor.SkillType.Illustration;
                    break;
                case PainterCategory.Profile:
                    sk = OldPainterAuthor.SkillType.Profile;
                    break;
                case PainterCategory.Expression:
                    sk = OldPainterAuthor.SkillType.Expression;
                    break;
                default:
                    return;
            }
            foreach (var author in mw.Core.Authors.OrderBy(x => Function.Rnd.Next()))
            {
                var skill = author.Skills.Find(x => x.Type == sk);
                if (skill == null)
                    continue;
                if (skill.PriceMin > ExpectedAmountTo || skill.PriceMax < ExpectedAmountFrom)
                    continue;
                if (SaveData.GetInt(author.Identy + "_atpye") != 0)
                    continue;
                RecommendedPainters.Add(new AuthorInTask(this, author, AuthorInTask.AuthorInTaskType.Recommended));
            }
        }

        /// <summary>
        /// 将任务数据转换为用于保存的 SaveData
        /// </summary>
        /// <returns></returns>
        public ILine ToLine()
        {
            SaveData[(gint)"category"] = (int)Category;
            SaveData[(gstr)"title"] = Title;
            SaveData[(gstr)"imagepath"] = ImageShotPath;
            SaveData[(gdat)"createtime"] = CreateTime;
            SaveData[(gdat)"starttime"] = StartTime;
            SaveData[(gdat)"endtime"] = EndTime;
            SaveData[(gdbe)"expectedamountfrom"] = ExpectedAmountFrom;
            SaveData[(gdbe)"expectedamountto"] = ExpectedAmountTo;
            SaveData[(gstr)"content"] = Content;
            SaveData[(gbol)"isarch"] = IsArchived;
            SaveData.InfoToInt = Id;
            //保存应征和邀请的画师
            var ALLAuthorID = new List<string>();
            ALLAuthorID.AddRange(RecruitPainters.Select(x => x.AuthorId));
            ALLAuthorID.AddRange(InvitePainters.Select(x => x.AuthorId));
            ALLAuthorID.AddRange(PartedPainters.Select(x => x.AuthorId));
            SaveData["ALLAuthorID"].info = string.Join(',', ALLAuthorID);
            return SaveData;
        }

        /// <summary>
        /// 创建一个新的画师任务
        /// </summary>
        /// <param name="mw"></param>
        public OldPainterTask(IMainWindow mw)
        {
            this.mw = mw;
            SaveData = new Line("oldpainttask", Id.ToString());
            Id = Function.Rnd.Next();
        }

        /// <summary>
        /// 是否已归档
        /// </summary>
        public bool IsArchived = false;

        /// <summary>
        /// 画师任务
        /// </summary>
        public PainterCategory Category { get => _category; set => Set(ref _category, value); }
        private PainterCategory _category;

        /// <summary>
        /// 当前进度
        /// </summary>
        public PainterProgress CurrentProgress { get => _currentProgress; set => Set(ref _currentProgress, value); }
        private PainterProgress _currentProgress;
        /// <summary>
        /// 当前进度值
        /// </summary>
        public double CurrentProgressValue
        {
            get
            {
                if (PartedPainters.Count == 0) return 0;
                return PartedPainters.Average(x => x.CurrentProgressValue);
            }
            set
            {

            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get => _title; set => Set(ref _title, value); }
        private string _title;

        /// <summary>
        /// 企划图片所在路径
        /// </summary>
        public string ImageShotPath;

        /// <summary>
        /// 企划图片
        /// </summary>
        public ImageSource ImageShot { get => _imageShot; set => Set(ref _imageShot, value); }
        private ImageSource _imageShot;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get => _createTime; set => Set(ref _createTime, value); }
        private DateTime _createTime;

        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime StartTime { get => _startTime; set => Set(ref _startTime, value); }
        private DateTime _startTime;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get => _endTime; set => Set(ref _endTime, value); }
        private DateTime _endTime;

        /// <summary>
        /// 预算范围-起始
        /// </summary>
        public double ExpectedAmountFrom { get => _expectedAmountFrom; set => Set(ref _expectedAmountFrom, value); }
        private double _expectedAmountFrom;

        /// <summary>
        /// 预算范围-截止
        /// </summary>
        public double ExpectedAmountTo { get => _expectedAmountTo; set => Set(ref _expectedAmountTo, value); }
        private double _expectedAmountTo;


        /// <summary>
        /// 参与的画师 (已给定金,正在干活)
        /// </summary>
        public ObservableCollection<AuthorInTask> PartedPainters { get => _partedPainters; set => Set(ref _partedPainters, value); }
        private ObservableCollection<AuthorInTask> _partedPainters = new ObservableCollection<AuthorInTask>();

        /// <summary>
        /// 应征的画师 (找上门的画师)
        /// </summary>
        public ObservableCollection<AuthorInTask> RecruitPainters { get => _recruitPainters; set => Set(ref _recruitPainters, value); }
        private ObservableCollection<AuthorInTask> _recruitPainters = new ObservableCollection<AuthorInTask>();

        /// <summary>
        /// (想要)招募的画师 (主动找别人的画师)
        /// </summary>
        public ObservableCollection<AuthorInTask> InvitePainters { get => _invitePainters; set => Set(ref _invitePainters, value); }
        private ObservableCollection<AuthorInTask> _invitePainters = new ObservableCollection<AuthorInTask>();

        /// <summary>
        /// 推荐的画师 (随机抽取)
        /// </summary>
        public ObservableCollection<AuthorInTask> RecommendedPainters { get => _recommendedPainters; set => Set(ref _recommendedPainters, value); }
        private ObservableCollection<AuthorInTask> _recommendedPainters = new ObservableCollection<AuthorInTask>();
        /// <summary>
        /// 任务内容 (例如绑定的L2D文件名称等)
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 任务内容 (实际绑定的内容)
        /// </summary>
        public Item_Paint ContentItem;
        /// <summary>
        /// 应征作者参数
        /// </summary>
        public class AuthorInTask : NotifyPropertyChangedBase
        {
            /// <summary>
            /// 作者在项目中的类型
            /// </summary>
            public enum AuthorInTaskType
            {
                /// <summary>
                ///  推荐的画师 (随机抽取)
                /// </summary>
                Recommended,
                /// <summary>
                /// 应征的画师 (找上门的画师)
                /// </summary>
                Recruit,
                /// <summary>
                /// 招募的画师 (找别人的画师)
                /// </summary>
                Invite,
                /// <summary>
                /// 参与的画师 (已给定金,正在干活)
                /// </summary>
                Parted,
            }
            public AuthorInTaskType AuthorType
            {
                get => (AuthorInTaskType)OPT.SaveData[(gint)(AuthorId + "_atpye")];
                set => OPT.SaveData[(gint)(AuthorId + "_atpye")] = (int)value;
            }
            /// <summary>
            /// 作者在任务中的信息。
            /// </summary>
            /// <param name="opt">任务</param>
            /// <param name="author">老画师作者</param>
            /// <param name="authorType">作者在项目中的类型(为空则加载默认设置)</param>
            /// <param name="save">游戏存档,考虑到这个项目需要在初始化的时候调用,那时候MW还没有SAVE</param>
            public AuthorInTask(OldPainterTask opt, OldPainterAuthor author, AuthorInTaskType? authorType = null, ISave save = null)
            {
                this.OPT = opt;
                Author = author;
                if (authorType.HasValue)
                {
                    AuthorType = authorType.Value;
                }
                save ??= opt.mw.Save;
                switch (opt.Category)
                {
                    case PainterCategory.L2DPaint:
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.L2DPaint);
                        if (!string.IsNullOrEmpty(Content))
                            ContentItem = (Item_Paint)save.Items.Find(x => x.Type == ItemType.l2d && x.ItemIdentifier == Content);
                        str_MiddleTask = "线稿";
                        str_EndTask = "成稿";
                        break;
                    case PainterCategory.L2DPaintUpdate://L2D更新啥的以项目Content为准
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.L2DPaint);
                        str_MiddleTask = "完善";
                        str_EndTask = "改进";
                        ContentItem = opt.ContentItem;//绑定的L2D文件
                        break;
                    case PainterCategory.L2DModel:
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.L2DModel);
                        str_MiddleTask = "拆分";
                        str_EndTask = "动作";
                        ContentItem = opt.ContentItem;//绑定的L2D文件
                        break;
                    case PainterCategory.L2DModelUpdate://同理
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.L2DModel);
                        str_MiddleTask = "改绑";
                        str_EndTask = "调参";
                        ContentItem = opt.ContentItem;//绑定的L2D文件
                        break;
                    case PainterCategory.Illustration://这些及以下还没做,以后整
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.Illustration);
                        break;
                    case PainterCategory.Profile:
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.Profile);
                        break;
                    case PainterCategory.Expression:
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.Expression);
                        break;
                }
                if (CurrentProgress == PainterProgress.Completed)
                    str_CanNext = "已完成";//TODO:以后出评价系统,现在先不整
                else
                    str_CanNext = "进行下一步";

                WorkPreview = opt.mw.Core.ImageSources.FindImage(WorkPreviewPath, "oldpainter_wait_" + Category);
            }
            public Skill Skill { get; set; }

            public OldPainterTask OPT;

            public OldPainterAuthor Author;

            /// <summary>
            /// 画师头像
            /// </summary>
            public ImageSource HeadImage => Author.HeadImage;

            /// <summary>
            /// 画师任务
            /// </summary>
            public PainterCategory Category => OPT.Category;
            /// <summary>
            /// 画师ID
            /// </summary>
            public string AuthorId => Author.Identy;

            /// <summary>
            /// 画师名字
            /// </summary>
            public string Name => Author.Name;

            /// <summary>
            /// 评分
            /// </summary>
            public double Rate => Skill?.Rate ?? 0;
            /// <summary>
            /// 当前进度
            /// </summary>
            public PainterProgress CurrentProgress
            {
                get
                {
                    if (CurrentProgressValue >= 100)
                        return PainterProgress.Completed;
                    else if (CurrentProgressValue > 0)
                        return PainterProgress.Progressing;
                    else
                        return PainterProgress.Begin;
                }
            }
            /// <summary>
            /// 当前进度值
            /// </summary>
            public double CurrentProgressValue
            {
                get => OPT.SaveData[(gdbe)(AuthorId + "_cpv")]; set
                {
                    OPT.SaveData[(gdbe)(AuthorId + "_cpv")] = value;
                    NotifyOfPropertyChange(nameof(CurrentProgressValue));
                }
            }

            /// <summary>
            /// 报价 稿酬
            /// </summary>
            public double Amount
            {
                get => OPT.SaveData[(gdbe)(AuthorId + "_amount")]; set
                {
                    OPT.SaveData[(gdbe)(AuthorId + "_amount")] = value;
                    NotifyOfPropertyChange(nameof(Amount));
                }
            }
            /// <summary>
            /// 报价 稿酬(已花费)(用于生成质量)
            /// </summary>
            public double AmountUsed
            {
                get => OPT.SaveData[(gdbe)(AuthorId + "_amount_used")]; set
                {
                    OPT.SaveData[(gdbe)(AuthorId + "_amount_used")] = value;
                    NotifyOfPropertyChange(nameof(Amount));
                }
            }
            /// <summary>
            /// 预计用时
            /// </summary>
            public double ExpectedDays
            {
                get => OPT.SaveData[(gdbe)(AuthorId + "_expecteddays")]; set
                {
                    OPT.SaveData[(gdbe)(AuthorId + "_expecteddays")] = value;
                    NotifyOfPropertyChange(nameof(ExpectedDays));
                }
            }
            /// <summary>
            /// 最后更新时间 (给程序用来更新计时用的)
            /// </summary>
            public DateTime LastUpdateTime
            {
                get => OPT.SaveData[(gdat)(AuthorId + "_lastupdatetime")]; set
                {
                    OPT.SaveData[(gdat)(AuthorId + "_lastupdatetime")] = value;
                }
            }
            /// <summary>
            /// 能否继续下一步 (当进度到达50%和100%时会自动设置为true,否则为false)
            /// </summary>
            public bool CanNext
            {
                get => OPT.SaveData.GetBool(AuthorId + "_cannext"); set
                {
                    OPT.SaveData.SetBool(AuthorId + "_cannext", value);
                    NotifyOfPropertyChange(nameof(CanNext));
                }
            }

            /// <summary>
            /// 稿件标题。
            /// </summary>
            public string WorkPreviewTitle
            {
                get => OPT.SaveData[(gstr)(AuthorId + "_title")]; set
                {
                    OPT.SaveData[(gstr)(AuthorId + "_title")] = value;
                    NotifyOfPropertyChange(nameof(WorkPreviewTitle));
                }
            }
            /// <summary>
            /// 任务内容 (例如绑定的L2D文件名称等)
            /// </summary>
            public string Content
            {
                get => OPT.SaveData[(gstr)(AuthorId + "_content")]; set
                {
                    OPT.SaveData[(gstr)(AuthorId + "_content")] = value;
                    NotifyOfPropertyChange(nameof(Content));
                }
            }
            /// <summary>
            /// 任务内容 (实际绑定的内容)
            /// </summary>
            public Item_Paint ContentItem;

            public string ContentInfoDisplay
            {
                get => ContentItem?.PaintIntroduce ?? "任务内容: " + Content;
                set { }
            }
            /// <summary>
            /// 稿件预览路径。
            /// </summary>
            public string WorkPreviewPath
            {
                get => OPT.SaveData[(gstr)(AuthorId + "_path")]; set
                {
                    OPT.SaveData[(gstr)(AuthorId + "_path")] = value;
                    NotifyOfPropertyChange(nameof(WorkPreviewPath));
                }
            }

            /// <summary>
            /// 稿件。
            /// </summary>

            public ImageSource WorkPreview { get => _workPreview; set => Set(ref _workPreview, value); }
            private ImageSource _workPreview;

            public double WorkPreviewOpacity => Math.Min(1, 0.5 + CurrentProgressValue / 200);

            /// <summary>
            /// 这个是画师作品的预览图,和稿件不一样
            /// </summary>
            public ImageSource[] WorkPreviews { get => Author.WorkPreviews; }

            /// <summary>
            /// 评论数量
            /// </summary>
            public int ReviewCount => Author.TaskNumber;

            /// <summary>
            /// 完成率
            /// </summary>
            public double CompleteRate => Author.Finish;

            /// <summary>
            /// 准时率
            /// </summary>
            public double OnTimeRate => Author.OnTime;

            /// <summary>
            /// 是否已经加入
            /// </summary>
            public bool IsJoined { get => AuthorType == AuthorInTaskType.Parted; }

            /// <summary>
            /// 是否是带价邀请
            /// </summary>
            public bool InviteWithPrice { get => OPT.SaveData.GetBool(AuthorId + "_invitewithprice"); set => OPT.SaveData.SetBool(AuthorId + "_invitewithprice", value); }

            /// <summary>
            /// 是否可以邀请。
            /// </summary>
            public bool CanInvite { get => !OPT.SaveData.GetBool(AuthorId + "_noinvite"); set => OPT.SaveData.SetBool(AuthorId + "_noinvite", !value); }

            public enum RefusalReasonType
            {
                /// <summary>
                /// 未知原因 (默认,未指定)
                /// </summary>
                Unknown,
                /// <summary>
                /// 价格过低
                /// </summary>
                LowPrice,
                /// <summary>
                /// 价格过高
                /// </summary>
                HighPrice,
                /// <summary>
                /// 江郎才尽 (一般是没有可以抽的作品了)
                /// </summary>
                WriterBlock,
                /// <summary>
                /// 该项目已经有应聘者了 (对于改进和建模方面的任务,只能有一个画师参与)
                /// </summary>
                HaveAuthor,
                /// <summary>
                /// 忙碌中 (画师正在忙于其他任务)(其实是有其他原因导致的拒绝,但是懒得和玩家说)
                /// </summary>
                Busy,
            }
            public RefusalReasonType RefusalReason
            {
                get => (RefusalReasonType)OPT.SaveData[(gint)(AuthorId + "_refusalreason")];
                set => OPT.SaveData[(gint)(AuthorId + "_refusalreason")] = (int)value;
            }
            /// <summary>
            /// 该任务的中间进度名称
            /// </summary>
            public string str_MiddleTask { get; set; }
            /// <summary>
            /// 该任务的结束进度名称
            /// </summary>
            public string str_EndTask { get; set; }
            /// <summary>
            /// 按钮显示字体
            /// </summary>
            public string str_CanNext { get; set; }

            /// <summary>
            /// 时间流逝时调用,用于更新任务状态等。
            /// </summary>
            public void Handle_PassTime(IMainWindow mw)
            {
                if (CurrentProgress == PainterProgress.Completed || CurrentProgressValue == 50)
                {//中间节点和已经完成的任务不需要更新
                    //如果已经完成,则不需要更新
                    return;
                }
                //经过时间(天)
                double timepass = (mw.Save.Base.Now - LastUpdateTime).TotalDays;
                LastUpdateTime = mw.Save.Base.Now;
                //增加进度指示
                double ps = timepass / ExpectedDays;
                ExpectedDays = Math.Max(1, ExpectedDays - timepass);
                double befprocess = CurrentProgressValue;
                if (ps > 1)
                {
                    ps = 1;
                }
                else if (ps < 0)
                {
                    return;
                }
                ps *= 100;
                bool isPause50 = false;
                bool isPause100 = false;
                //避免卡UI线程,还是先Dispatch一下
                mw.Dispatcher.Invoke(() =>
                {
                    //先看看进度增加了后会不会超过50%或100%
                    if (CurrentProgressValue < 50)
                    {
                        if (ps + CurrentProgressValue > 50)
                        {//确保PS不会超过50%
                            ps = 50 - CurrentProgressValue;
                            CurrentProgressValue = 50;//直接设置为50%
                            CanNext = true;//可以进行下一步
                                           //50%时显示线稿
                            isPause50 = true;

                            //弹消息提示用户去老画师看看
                            new Event(mw, "none",
                            new Sub("period", "Single"),
                            new Sub("intor", "企划 {0} 已达到 {1} 节点\n请前往老画师进行确认".Translate(OPT.Title, str_MiddleTask)),
                            new Sub("visible", "Message"),
                            new Sub("info", "老画师: 企划已达到新节点"),
                            new Sub("startdate", mw.Save.Base.Now.ToString("yyyy/MM/dd HH:mm"))
                            );
                        }
                        else
                        {
                            CurrentProgressValue += ps;//增加进度
                        }
                    }
                    else
                    {
                        if (ps + CurrentProgressValue > 100)
                        {//确保PS不会超过100%
                            ps = 100 - CurrentProgressValue;
                            CurrentProgressValue = 100;//直接设置为100%
                            str_CanNext = "已完成";//可以进行下一步
                                                //CanNext = true;//可以进行下一步TODO:去评价
                            isPause100 = true;
                            //弹消息提示用户去老画师看看
                            new Event(mw, "none",
                            new Sub("period", "Single"),
                            new Sub("intor", "企划 {0} 已达到 {1} 节点\n请前往老画师进行签收".Translate(OPT.Title, str_EndTask)),
                            new Sub("visible", "Message"),
                            new Sub("info", "老画师: 企划已完成"),
                            new Sub("startdate", mw.Save.Base.Now.ToString("yyyy/MM/dd HH:mm"))
                            );
                        }
                        else
                        {
                            CurrentProgressValue += ps;//增加进度
                        }
                    }
                    //增加进度(普通)
                    switch (Category)
                    {
                        case PainterCategory.L2DPaint:
                            var l2d = ContentItem as Item_L2D;
                            double buffprice = Math.Max(10, Math.Min(175, (Amount - AmountUsed) / l2d.PriceBase * (100 - befprocess)));//根据价格基数计算buff(10-175)
                            l2d.Process = (int)(CurrentProgressValue / 2);
                            l2d.ImageRank += Function.RndNext(l2d.Min * buffprice, l2d.Max * buffprice) / 10000 * ps;

                            AmountUsed += ps * l2d.PriceBase / 10000 * buffprice;//增加已花费的金额
                                                                                 //抽表情
                            if (buffprice > 110 && Function.Rnd.Next(l2d.ExpressionHave.Count * 2 + 2) == 0)
                            {
                                l2d.ExpressionADD(Function.Rnd.Next(l2d.ExpressionList.Count));
                                AmountUsed += l2d.PriceExp;
                            }

                            if (isPause50)
                            {
                                //更新预览图
                                WorkPreviewPath = l2d.Image;
                                WorkPreview = mw.Dispatcher.Invoke(() => mw.Core.ImageSources.FindImage(WorkPreviewPath, "oldpainter_wait_" + Category));
                            }
                            else if (isPause100)
                            {//对齐:避免超过100%
                                l2d.Process = 50;
                            }

                            break;
                        case PainterCategory.L2DModel:
                            l2d = ContentItem as Item_L2D;
                            buffprice = 50 + Math.Max(-20, Math.Min(100, (Amount - AmountUsed) / (l2d.PriceBase + l2d.PriceExp + Skill.PriceMax + Skill.PriceMin) * (100 - befprocess) * 2));//根据价格基数计算buff(50-150)
                            l2d.Process = 50 + (int)(CurrentProgressValue / 2);
                            l2d.ModelRank += Function.RndNext(Skill.LevelMin * buffprice, Skill.LevelMax * buffprice) / 10000 * ps;
                            AmountUsed += ps * Amount / 10000 * buffprice;//增加已花费的金额
                            if (isPause50)
                            {//更新预览图
                                WorkPreviewPath = l2d.Image;
                                WorkPreview = mw.Dispatcher.Invoke(() => mw.Core.ImageSources.FindImage(WorkPreviewPath, "oldpainter_wait_" + Category));
                            }
                            else if (isPause100)
                            {//对齐:避免超过100%
                                l2d.Process = 100;
                            }
                            break;
                        case PainterCategory.L2DPaintUpdate:
                            //升级立绘
                            l2d = ContentItem as Item_L2D;
                            buffprice = 25 + Math.Max(-10, Math.Min(100, (Amount - AmountUsed) / (l2d.PriceBase + l2d.PriceExp + Skill.PriceMax + Skill.PriceMin) * (100 - befprocess) / 2));//根据价格基数计算buff(25-150)
                            l2d.ImageRank += Math.Sqrt(l2d.ImageRank + Function.RndNext(Skill.LevelMin * buffprice, Skill.LevelMax * buffprice) / 10000 * ps) - Math.Sqrt(l2d.ImageRank);//增加立绘等级
                            AmountUsed += ps * Amount / 10000 * buffprice;//增加已花费的金额
                                                                          //抽表情
                            if (buffprice > 70 && Function.Rnd.Next(l2d.ExpressionHave.Count * 2 + 2) == 0)
                            {
                                l2d.ExpressionADD(Function.Rnd.Next(l2d.ExpressionList.Count));
                                AmountUsed += l2d.PriceExp;
                            }
                            if (isPause100)
                            {
                                l2d.Process = 75;
                            }
                            break;
                        case PainterCategory.L2DModelUpdate:
                            l2d = ContentItem as Item_L2D;
                            buffprice = 25 + Math.Max(0, Math.Min(100, (Amount - AmountUsed) / (l2d.PriceBase + l2d.PriceExp + Skill.PriceMax + Skill.PriceMin) * (100 - befprocess) / 2));//根据价格基数计算buff(25-150)
                            l2d.ModelRank += Math.Sqrt(l2d.ModelRank + Function.RndNext(Skill.LevelMin * buffprice, Skill.LevelMax * buffprice) / 10000 * ps) - Math.Sqrt(l2d.ModelRank);//增加立绘等级
                            AmountUsed += ps * Amount / 10000 * buffprice;//增加已花费的金额
                            if (isPause100)
                            {
                                l2d.Process = 100;
                            }
                            break;
                        default:
                            CurrentProgressValue += ps;
                            break;
                    }

                });
            }
            ///// <summary>
            ///// 是否显示。
            ///// </summary>
            //public bool IsVisible { get => _isVisible; set => Set(ref _isVisible, value); }
            //private bool _isVisible = true;
        }

    }
}

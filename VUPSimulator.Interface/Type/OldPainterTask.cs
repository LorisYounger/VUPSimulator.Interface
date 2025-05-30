using LinePutScript;
using LinePutScript.Converter;
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
        public OldPainterTask(IMainWindow mw, ILine savedata)
        {
            this.mw = mw;
            SaveData = savedata;
            Category = (PainterCategory)savedata[(gint)"category"];
            Title = savedata[(gstr)"title"];
            Id = savedata.InfoToInt;
            ImageShotPath = savedata[(gstr)"imagepath"];
            ImageShot = mw.Core.ImageSources.FindImage(ImageShotPath, "oldpainter_" + Category);
            CreateTime = savedata[(gdat)"createtime"];
            StartTime = savedata[(gdat)"starttime"];
            EndTime = savedata[(gdat)"endtime"];
            ExpectedAmountFrom = savedata[(gdbe)"expectedamountfrom"];
            ExpectedAmountTo = savedata[(gdbe)"expectedamountto"];
            Content = savedata[(gstr)"content"];

            //加载应征和邀请的画师
            foreach (string AuthID in savedata["ALLAuthorID"].GetInfos())
            {
                OldPainterAuthor author = mw.Core.Authors.Find(x => x.Identy == AuthID);
                if (author != null)
                {
                    AuthorInTask ait = new AuthorInTask(this, author);
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
            switch (Category)
            {
                case PainterCategory.L2DPaintUpdate:
                case PainterCategory.L2DModel:
                case PainterCategory.L2DModelUpdate:
                    //绑定的Content是用户拥有的L2D文件
                    ContentItem = mw.Save.Items.Find(x => x.Type == ItemType.l2d && x.ItemIdentifier == Content);
                    break;
            }

            //推荐的画师不会被保存,由系统生成,不和应征邀请相同
            GenRecommend();
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
                if (SaveData.GetInt(author + "_atpye") != 0)
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
        public double CurrentProgressValue { get => _currentProgressValue; set => Set(ref _currentProgressValue, value); }
        private double _currentProgressValue = 0.0;

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
        public Item ContentItem;
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
            public AuthorInTask(OldPainterTask opt, OldPainterAuthor author, AuthorInTaskType? authorType = null)
            {
                this.OPT = opt;
                Author = author;
                if (authorType.HasValue)
                {
                    AuthorType = authorType.Value;
                }
                switch (opt.Category)
                {
                    case PainterCategory.L2DPaint:
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.L2DPaint);
                        if (!string.IsNullOrEmpty(Content))
                            ContentItem = opt.mw.Save.Items.Find(x => x.Type == ItemType.l2d && x.ItemIdentifier == Content);
                        break;
                    case PainterCategory.L2DPaintUpdate://L2D更新啥的以项目Content为准
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.L2DPaint);                       
                        break;
                    case PainterCategory.L2DModel:
                    case PainterCategory.L2DModelUpdate:
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.L2DModel);                       
                        break;
                    case PainterCategory.Illustration:
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.Illustration);
                        break;
                    case PainterCategory.Profile:
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.Profile);
                        break;
                    case PainterCategory.Expression:
                        Skill = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.Expression);
                        break;
                }
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
            /// 预计用时
            /// </summary>
            public int ExpectedDays
            {
                get => OPT.SaveData[(gint)(AuthorId + "_expecteddays")]; set
                {
                    OPT.SaveData[(gint)(AuthorId + "_expecteddays")] = value;
                    NotifyOfPropertyChange(nameof(ExpectedDays));
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
            public Item ContentItem;
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
            }
            public RefusalReasonType RefusalReason
            {
                get => (RefusalReasonType)OPT.SaveData[(gint)(AuthorId + "_refusalreason")];
                set => OPT.SaveData[(gint)(AuthorId + "_refusalreason")] = (int)value;
            }

            ///// <summary>
            ///// 是否显示。
            ///// </summary>
            //public bool IsVisible { get => _isVisible; set => Set(ref _isVisible, value); }
            //private bool _isVisible = true;
        }

    }
}

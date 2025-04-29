using LinePutScript;
using LinePutScript.Converter;
using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 老画师绘画任务
    /// </summary>
    public class OldPainterTask : NotifyPropertyChangedBase
    {
        /// <summary>
        /// 进度。
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
        public Line SaveData;
        /// <summary>
        /// 加载任务数据
        /// </summary>
        /// <param name="mw"></param>
        /// <param name="taskdata">任务数据(用于加载)</param>
        /// <param name="savedata">任务子数据(用于保存)</param>
        public OldPainterTask(IMainWindow mw, Line taskdata, Line savedata)
        {
            this.mw = mw;
            SaveData = savedata;
            Category = (PainterCategory)taskdata[(gint)"category"];
            Title = taskdata[(gstr)"title"];
            ImageShotPath = taskdata[(gstr)"imagepath"];
            ImageShot = mw.Core.ImageSources.FindImage(ImageShotPath, "oldpainter_" + Category);
            CreateTime = taskdata[(gdat)"createtime"];
            StartTime = taskdata[(gdat)"starttime"];
            EndTime = taskdata[(gdat)"endtime"];
            ExpectedAmountFrom = taskdata[(gdbe)"expectedamountfrom"];
            ExpectedAmountTo = taskdata[(gdbe)"expectedamountto"];
            _partedPainters = new ObservableCollection<AuthorInTask>();
            _recruitPainters = new ObservableCollection<AuthorInTask>();
            _invitePainters = new ObservableCollection<AuthorInTask>();
            _recommendedPainters = new ObservableCollection<AuthorInTask>();
            //TODO


            //推荐的画师不会被保存,由系统生成
        }

        public OldPainterTask(IMainWindow mw) { this.mw = mw; }

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
        public DateTime? StartTime { get => _startTime; set => Set(ref _startTime, value); }
        private DateTime? _startTime;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get => _endTime; set => Set(ref _endTime, value); }
        private DateTime? _endTime;

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
        /// 参与的画师
        /// </summary>
        public ObservableCollection<AuthorInTask> PartedPainters { get => _partedPainters; set => Set(ref _partedPainters, value); }
        private ObservableCollection<AuthorInTask> _partedPainters;

        /// <summary>
        /// 应征的画师
        /// </summary>
        public ObservableCollection<AuthorInTask> RecruitPainters { get => _recruitPainters; set => Set(ref _recruitPainters, value); }
        private ObservableCollection<AuthorInTask> _recruitPainters;

        /// <summary>
        /// 招募的画师
        /// </summary>
        public ObservableCollection<AuthorInTask> InvitePainters { get => _invitePainters; set => Set(ref _invitePainters, value); }
        private ObservableCollection<AuthorInTask> _invitePainters;

        /// <summary>
        /// 推荐的画师
        /// </summary>
        public ObservableCollection<AuthorInTask> RecommendedPainters { get => _recommendedPainters; set => Set(ref _recommendedPainters, value); }
        private ObservableCollection<AuthorInTask> _recommendedPainters;

        /// <summary>
        /// 应征作者参数
        /// </summary>
        public class AuthorInTask : NotifyPropertyChangedBase
        {
            public AuthorInTask(OldPainterTask opt, OldPainterAuthor author)
            {
                this.opt = opt;
                Author = author;
                switch (opt.Category)
                {
                    case PainterCategory.L2DPaint:
                    case PainterCategory.L2DPaintUpdate:
                        Rate = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.L2DPaint).Rate;
                        break;
                    case PainterCategory.L2DModel:
                    case PainterCategory.L2DModelUpdate:
                        Rate = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.L2DModel).Rate;
                        break;
                    case PainterCategory.Illustration:
                        Rate = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.Illustration).Rate;
                        break;
                    case PainterCategory.Profile:
                        Rate = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.Profile).Rate;
                        break;
                    case PainterCategory.Expression:
                        Rate = author.Skills.Find(x => x.Type == OldPainterAuthor.SkillType.Expression).Rate;
                        break;
                }
                WorkPreview = opt.mw.Core.ImageSources.FindImage(WorkPreviewPath, "oldpainter_wait_" + Category);
            }
            OldPainterTask opt;

            OldPainterAuthor Author;

            public ImageSource HeadImage => Author.ProfileImageSource(opt.mw);


            public PainterCategory Category => opt.Category;

            /// <summary>
            /// 作者名字
            /// </summary>
            public string Name => Author.Name;

            /// <summary>
            /// 评分
            /// </summary>
            public double Rate { get => _rate; set => Set(ref _rate, value); }
            private double _rate;
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
            /// 当前进度值
            /// </summary>
            public double CurrentProgressValue
            {
                get => opt.SaveData[(gdbe)(Author + "_cpv")]; set
                {
                    opt.SaveData[(gdbe)(Author + "_cpv")] = value;
                    NotifyOfPropertyChange(nameof(CurrentProgressValue));
                }
            }

            /// <summary>
            /// 报价 稿酬
            /// </summary>
            public double Amount
            {
                get => opt.SaveData[(gdbe)(Author + "_amount")]; set
                {
                    opt.SaveData[(gdbe)(Author + "_amount")] = value;
                    NotifyOfPropertyChange(nameof(Amount));
                }
            }

            /// <summary>
            /// 预计用时
            /// </summary>
            public int ExpectedDays
            {
                get => opt.SaveData[(gint)(Author + "_expecteddays")]; set
                {
                    opt.SaveData[(gint)(Author + "_expecteddays")] = value;
                    NotifyOfPropertyChange(nameof(ExpectedDays));
                }
            }

            /// <summary>
            /// 稿件标题。
            /// </summary>
            public string WorkPreviewTitle
            {
                get => opt.SaveData[(gstr)(Author + "_title")]; set
                {
                    opt.SaveData[(gstr)(Author + "_title")] = value;
                    NotifyOfPropertyChange(nameof(WorkPreviewTitle));
                }
            }
            public string WorkPreviewPath
            {
                get => opt.SaveData[(gstr)(Author + "_path")]; set
                {
                    opt.SaveData[(gstr)(Author + "_path")] = value;
                    NotifyOfPropertyChange(nameof(WorkPreviewPath));
                }
            }
            /// <summary>
            /// 稿件。
            /// </summary>

            public ImageSource WorkPreview { get => _workPreview; set => Set(ref _workPreview, value); }
            private ImageSource _workPreview;

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
            public bool IsJoined { get => _isJoined; set => Set(ref _isJoined, value); }
            private bool _isJoined;

            /// <summary>
            /// 是否是带价邀请
            /// </summary>
            public bool InviteWithPrice { get => _inviteWithPrice; set => Set(ref _inviteWithPrice, value); }
            private bool _inviteWithPrice;

            /// <summary>
            /// 是否显示。
            /// </summary>
            public bool IsVisible { get => _isVisible; set => Set(ref _isVisible, value); }
            private bool _isVisible = true;
        }



    }
}

using LinePutScript;
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
            /// 立绘。
            /// </summary>
            Paint,
            /// <summary>
            /// 建模。
            /// </summary>
            Model,
        }
        /// <summary>
        /// 读档 老画师绘画任务
        /// </summary>
        public OldPainterTask(ILine line)
        {

        }
        public OldPainterTask() { }

        /// <summary>
        /// 画师任务
        /// </summary>
        public UserOldPainter.Type Category { get => _category; set => Set(ref _category, value); }
        private UserOldPainter.Type _category;

        /// <summary>
        /// 当前进度
        /// </summary>
        public PainterProgress CurrentProgress { get => _currentProgress; set => Set(ref _currentProgress, value); }
        private PainterProgress _currentProgress;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get => _title; set => Set(ref _title, value); }
        private string _title;

        /// <summary>
        /// 企划图片
        /// </summary>
        public ImageSource ImageShot { get => _imageShot; set => Set(ref _imageShot, value); }
        private ImageSource _imageShot;


        /// <summary>
        /// 评分。
        /// </summary>
        public double Rate { get => _rate; set => Set(ref _rate, value); }
        private double _rate;

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
        public decimal ExpectedAmountFrom { get => _expectedAmountFrom; set => Set(ref _expectedAmountFrom, value); }
        private decimal _expectedAmountFrom;

        /// <summary>
        /// 预算范围-截止
        /// </summary>
        public decimal ExpectedAmountTo { get => _expectedAmountTo; set => Set(ref _expectedAmountTo, value); }
        private decimal _expectedAmountTo;


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
            public ImageSource HeadImage { get => _headImage; set => Set(ref _headImage, value); }
            private ImageSource _headImage;

            public string Name { get => _name; set => Set(ref _name, value); }
            private string _name;

            /// <summary>
            /// 评分。
            /// </summary>
            public double Rate { get => _rate; set => Set(ref _rate, value); }
            private double _rate;

            /// <summary>
            /// 项目类型。
            /// </summary>
            public PainterCategory Category { get => _category; set => Set(ref _category, value); }
            private PainterCategory _category;

            /// <summary>
            /// 当前进度
            /// </summary>
            public PainterProgress CurrentProgress { get => _currentProgress; set => Set(ref _currentProgress, value); }
            private PainterProgress _currentProgress;

            /// <summary>
            /// 报价 稿酬
            /// </summary>
            public decimal Amount { get => _amount; set => Set(ref _amount, value); }
            private decimal _amount;

            /// <summary>
            /// 预计用时
            /// </summary>
            public int ExpectedDays { get => _expectedDays; set => Set(ref _expectedDays, value); }
            private int _expectedDays;

            /// <summary>
            /// 稿件标题。
            /// </summary>
            public string ManuscriptTitle { get => _manuscriptTitle; set => Set(ref _manuscriptTitle, value); }
            private string _manuscriptTitle;

            /// <summary>
            /// 稿件。
            /// </summary>
            public ImageSource Manuscript { get => _manuscript; set => Set(ref _manuscript, value); }
            private ImageSource _manuscript;

            public ImageSource[] WorkPreviews { get => _workPreviews; set => Set(ref _workPreviews, value); }
            private ImageSource[] _workPreviews;

            /// <summary>
            /// 评论数量
            /// </summary>
            public int ReviewCount { get => _reviewCount; set => Set(ref _reviewCount, value); }
            private int _reviewCount;

            /// <summary>
            /// 完成率
            /// </summary>
            public double CompleteRate { get => _completeRate; set => Set(ref _completeRate, value); }
            private double _completeRate;

            /// <summary>
            /// 准时率
            /// </summary>
            public double OnTimeRate { get => _onTimeRate; set => Set(ref _onTimeRate, value); }
            private double _onTimeRate;

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

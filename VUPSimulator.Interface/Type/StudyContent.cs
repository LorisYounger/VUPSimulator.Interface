using LinePutScript;
using LinePutScript.Localization.WPF;
using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Effects;
using static VUPSimulator.Interface.ISave;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 学习使用的内容类
    /// </summary>
    public class StudyContent
    {
        /// <summary>
        /// 课程
        /// </summary>
        public class Lession
        {
            public Lession() { }

            public Lession(Line data)
            {
                Name = data.Name;
                Description = data[(gstr)"desc"];
                EffectType = (ValueBaseType)Enum.Parse(typeof(ValueBaseType), data[(gstr)"effecttype"]);
                Quantity = data[(gdbe)"quantity"];
                Type = (LessionType)Enum.Parse(typeof(LessionType), data[(gstr)"type"]);
                UseTime = data[(gdbe)"usetime"];
                UseTimeTimes = data[(gint)"usetimetimes"];
                Price = data[(gdbe)"price"];
                TimeLimited = data[(gbol)"timelimited"];
                LevelRequirement = data[(gint)"levelrequirement"];
                LevelMaximum = data[(gint)"levelmaximum"];
            }
            public void LoadImage(IMainWindow mw)
            {
                Image = mw.Core.ImageSources.FindImage($"study_lession_{Name}");
            }

            /// <summary>
            /// 课程名字
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 课程名字(翻译)
            /// </summary>
            public string TransName => Name.Translate();

            /// <summary>
            /// 该课程针对的属性
            /// </summary>
            public ValueBaseType EffectType { get; set; }
            public string EffecttoString
            {
                get => EffectType.ToString().Translate() + (Quantity > 0 ? " +" : " ") + Quantity.ToString("0.##");
            }
            /// <summary>
            /// 该课程对属性的影响 (质量)
            /// </summary>
            public double Quantity { get; set; }

            /// <summary>
            /// 封面图片
            /// </summary>
            public ImageSource Image { get; set; }

            public string Description { get; set; }

            public string TransDescription => Description.Translate();
            /// <summary>
            /// 课程类型
            /// </summary>
            public enum LessionType
            {
                Book,
                OnlineSchool,
                Class,
                OneToOne,
            }
            public LessionType Type { get; set; }

            /// <summary>
            /// 需要时间消耗 (小时) (单次)
            /// </summary>
            public double UseTime { get; set; }
            /// <summary>
            /// 需要学习轮次
            /// </summary>
            public double UseTimeTimes { get; set; } = 1;
            /// <summary>
            /// 总计需要时间 (小时)
            /// </summary>
            public double TotalUseTime => UseTime * UseTimeTimes;
            /// <summary>
            /// 价格
            /// </summary>
            public double Price { get; set; }
            /// <summary>
            /// 是否是限制指定时间范围可用的课程
            /// </summary>
            public bool TimeLimited { get; set; } = false;
            /// <summary>
            /// 等级需求
            /// </summary>
            public int LevelRequirement { get; set; } = 0;
            /// <summary>
            /// 最大有效等级
            /// </summary>
            public int LevelMaximum { get; set; } = 20;
        }



        /// <summary>
        /// 学习进度
        /// </summary>
        public class StudyProgress
            : NotifyPropertyChangedBase
        {
            public string Name
            {
                get { return _name; }
                set { _name = value; NotifyOfPropertyChange(() => Name); }
            }
            private string _name;

            public string Description
            {
                get { return _description; }
                set { _description = value; NotifyOfPropertyChange(() => Description); }
            }
            private string _description;

            public ImageSource Image
            {
                get { return _image; }
                set { _image = value; NotifyOfPropertyChange(() => Image); }
            }
            private ImageSource _image;

            public string RemainingTime
            {
                get { return _remainingTime; }
                set { _remainingTime = value; NotifyOfPropertyChange(() => RemainingTime); }
            }
            private string _remainingTime;

            public double Progress
            {
                get { return _progress; }
                set { _progress = value; NotifyOfPropertyChange(() => Progress); }
            }
            private double _progress;
        }
    }
}

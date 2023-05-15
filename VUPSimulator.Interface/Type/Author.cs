using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static VUPSimulator.Interface.Author;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 老画师作者
    /// </summary>
    public class Author
    {
        public Author(LpsDocument lps)
        {
            var line = lps.FindLine("author");
            Name = line.Info;
            basescore = line.GetDouble("score");
            basescorenumber = line.GetInt("scorenumber");
            basefinish = line.GetInt("finish");
            baseisontime = line.GetInt("isontime");
            ProfileImage = line.GetString("profileimage");
            Intor = line.GetString("intor");
            foreach (var lin in lps.FindAllLine("skill"))
                Skills.Add(new Skill(lin));
            foreach (var lin in lps.FindAllLine("work"))
                Works.Add(new Work(lin));
        }
        /// <summary>
        /// 作者存档数据
        /// </summary>
        public Line AuthorData;
        /// <summary>
        /// 加载Save中的作者存档数据
        /// </summary>
        /// <param name="AuthorDataSet">存档数据 AuthorData</param>
        public void LoadSaveData(List<ILine> AuthorDataSet)
        {
            var line = AuthorDataSet.Find(x => x.info == Name);
            if (line == null)
            {
                AuthorData = new Line("author", Name);
                AuthorDataSet.Add(AuthorData);
            }
            else
                AuthorData = line;
        }
        /// <summary>
        /// 作者头像图
        /// </summary>
        public string ProfileImage;
        /// <summary>
        /// 获得作者头像
        /// </summary>
        public ImageSource ProfileImageSource(IMainWindow mw) => mw.Core.ImageSources.FindImage("profile_" + ProfileImage);

        /// <summary>
        /// 名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 作者介绍
        /// </summary>
        public string Intor;
        /// <summary>
        /// 评分
        /// </summary>
        public double Score
        {
            get
            {
                var sub = AuthorData.Find("score");
                if (sub != null)
                    return sub.InfoToDouble;
                else
                    return basescore;
            }
        }
        /// <summary>
        /// 添加新评价 (评分,是否完成,是否准时)
        /// </summary>
        public void AddNewFinish(double score, bool isfinish, bool isontime)
        {
            var s = Score;
            var nb = TaskNumber;
            var ns = (s * nb + score) / (nb + 1);
            //新分数
            AuthorData[(gdbe)"score"] = ns;
            AuthorData[(gint)"number"]++;
            if (isontime)
                AuthorData[(gint)"ontime"]++;
            if (isfinish)
                AuthorData[(gint)"finish"]++;
        }
        private double basescore;
        private int basescorenumber;
        private int basefinish;
        private int baseisontime;
        /// <summary>
        /// 准时率
        /// </summary>
        public double OnTime
        {
            get
            {
                var sub = AuthorData.Find("ontime");
                if (sub != null)
                    return sub.InfoToDouble / TaskNumber;
                else
                    return (double)baseisontime / TaskNumber;
            }
        }
        /// <summary>
        /// 完成率
        /// </summary>
        public double Finish
        {
            get
            {
                var sub = AuthorData.Find("finish");
                if (sub != null)
                    return sub.InfoToDouble / TaskNumber;
                else
                    return (double)basefinish / TaskNumber;
            }
        }
        /// <summary>
        /// 总共任务次数
        /// </summary>
        public int TaskNumber
        {
            get
            {
                var sub = AuthorData.Find("number");
                if (sub != null)
                    return sub.InfoToInt;
                else
                    return basescorenumber;
            }
        }

        /// <summary>
        /// 技能类型
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Live2D立绘
            /// </summary>
            L2DPaint,
            /// <summary>
            /// Live2D建模
            /// </summary>
            L2DModel,
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
        /// <summary>
        /// 技能
        /// </summary>
        public static readonly string[] TypeToString = new string[] { "Live2D立绘", "Live2D建模", "插图", "头像", "表情包" };
        /// <summary>
        /// 当前作者技能
        /// </summary>
        public List<Skill> Skills = new List<Skill>();
        /// <summary>
        /// 当前作者画作
        /// </summary>
        public List<Work> Works = new List<Work>();
        /// <summary>
        /// 作者技能
        /// </summary>
        public class Skill
        {
            /// <summary>
            /// 技能类型
            /// </summary>
            public Type Type;
            /// <summary>
            /// 技能等级(最小值) 0-5/10
            /// </summary>
            public double LevelMin;
            /// <summary>
            /// 技能等级(最大值) 0-5/10
            /// </summary>
            public double LevelMax;
            /// <summary>
            /// 报价价格区间: 最小价格
            /// </summary>
            public double PriceMin;
            /// <summary>
            /// 报价价格区间: 最大价格
            /// </summary>
            public double PriceMax;
            public double priceplevel = 0;
            /// <summary>
            /// 根据价格计算预期等级
            /// </summary>
            public double PricepLevel(double price, double time)
            {
                if (priceplevel == 0)
                    priceplevel = (PriceMax - PriceMin) / (LevelMax - LevelMin);
                price -= PriceMin;
                double v = LevelMin + price * priceplevel;
                v *= SpendTime / time;
                return Math.Min(LevelMax, Math.Max(v, LevelMin));
            }
            /// <summary>
            /// 平均作品所需时长
            /// </summary>
            public double SpendTime;
            public Skill(Line line)
            {
                Type = (Type)Enum.Parse(typeof(Type), line.info, true);
                LevelMin = line.GetDouble("levelmin");
                LevelMax = line.GetDouble("levelmax");
                PriceMin = line.GetDouble("pricemin");
                PriceMax = line.GetDouble("pricemax");
                SpendTime = line.GetDouble("spendtime");
            }
        }
        /// <summary>
        /// 展示的画作
        /// </summary>
        public class Work
        {
            /// <summary>
            /// 画作类型
            /// </summary>
            public Type SkillType;
            /// <summary>
            /// 画作名字
            /// </summary>
            public string Name;
            /// <summary>
            /// 图片
            /// </summary>
            public string Image;
            /// <summary>
            /// 获取画作图片
            /// </summary>
            public ImageSource ImageSource(IMainWindow mw) => mw.Core.ImageSources.FindImage("auth_" + Image);
            /// <summary>
            /// 画作介绍
            /// </summary>
            public string Intor;
            public Work(Line line)
            {
                SkillType = (Type)Enum.Parse(typeof(Type), line.info);
                Name = line.GetString("name");
                Image = line.GetString("image");
                Intor = line.Text;
            }
        }
    }
}

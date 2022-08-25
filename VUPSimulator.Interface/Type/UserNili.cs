using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// NiliNili 用户, 支持粉丝数等参数
    /// </summary>
    public class UserNili : Users
    {
        public UserNili(Line line) : base(line)
        {
            
        }
        /// <summary>
        /// 粉丝数量
        /// </summary>
        public int Fans
        {
            get => this[(gint)"fans"];
            set => this[(gint)"fans"] = value;
        }
        /// <summary>
        /// 具体粉丝 将来可以作为伪联机和好友互动?
        /// </summary>
        public List<string> FansUser
        {
            get => FindorAdd("fansuser").GetInfos().ToList();
            set => FindorAdd("fansuser").info = string.Join(",", value);
        }
        /// <summary>
        /// 总共粉丝数量
        /// </summary>
        public int TotalFans => Fans + FansUser.Count;
        /// <summary>
        /// 签名
        /// </summary>
        public string Bio
        {
            get => this[(gstr)"bio"];
            set => this[(gstr)"bio"] = value;
        }
        /// <summary>
        /// 用户立绘, 用于随机生成时使用
        /// </summary>
        public string UsrImage
        {
            get => this[(gstr)"usrimage"];
            set => this[(gstr)"usrimage"] = value;
        }
    }
    /// <summary>
    /// NiliNili视频发布者会使用的类 目前只有玩家会使用该类 主要用于显示玩家每月盈利等
    /// </summary>
    public class UserNiliSuper : UserNili
    {
        public UserNiliSuper(Line line) : base(line)
        {

        }

        /// <summary>
        /// 总粉丝表 用于图标展示 1=1天
        /// </summary>
        public List<int> FansGraph
        {
            get
            {
                List<int> ints = new List<int>();
                foreach (var str in FindorAdd("fansgraph").GetInfos())
                    ints.Add(Convert.ToInt32(str));
                return ints;
            }
            set => FindorAdd("fansgraph").info = string.Join(",", value);
        }

        /// <summary>
        /// 总收入表 用于图标展示 1=1天
        /// </summary>
        public List<int> IncomeGraph
        {
            get
            {
                List<int> ints = new List<int>();
                foreach (var str in FindorAdd("incomegraph").GetInfos())
                    ints.Add(Convert.ToInt32(str));
                return ints;
            }
            set => FindorAdd("incomegraph").info = string.Join(",", value);
        }

        /// <summary>
        /// 总收藏表 用于图标展示 1=1天
        /// </summary>
        public List<int> StartGraph
        {
            get
            {
                List<int> ints = new List<int>();
                foreach (var str in FindorAdd("startgraph").GetInfos())
                    ints.Add(Convert.ToInt32(str));
                return ints;
            }
            set => FindorAdd("startgraph").info = string.Join(",", value);
        }

        /// <summary>
        /// 总点赞表 用于图标展示 1=1天
        /// </summary>
        public List<int> LikeGraph
        {
            get
            {
                List<int> ints = new List<int>();
                foreach (var str in FindorAdd("likegraph").GetInfos())
                    ints.Add(Convert.ToInt32(str));
                return ints;
            }
            set => FindorAdd("likegraph").info = string.Join(",", value);
        }

        /// <summary>
        /// 总播放表 用于图标展示 1=1天
        /// </summary>
        public List<int> PlayGraph
        {
            get
            {
                List<int> ints = new List<int>();
                foreach (var str in FindorAdd("playgraph").GetInfos())
                    ints.Add(Convert.ToInt32(str));
                return ints;
            }
            set => FindorAdd("playgraph").info = string.Join(",", value);
        }
    }

}

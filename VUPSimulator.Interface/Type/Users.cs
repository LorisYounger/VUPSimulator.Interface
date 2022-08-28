using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LinePutScript;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 所有用户信息的表, 所有人用同一套用户信息系统
    /// 表头 user
    /// </summary>
    public class Users : Line
    {
        public Users(Line line) : base(line) { }
        /// <summary>
        /// 新建标准随机普通用户
        /// </summary>
        /// <param name="userName">用户名</param>
        public Users(string userName)
        {
            UserName = userName;
            //全部为随机用户
            this["vtag"].Info = "rnd";
            this["gtag"].Info = "rnd";
            this["stag"].Info = "rnd";
            this[(gint)"friendliness"] = Function.Rnd.Next(20, 80);
        }
        ///// <summary>
        ///// 全局用户信息设置
        ///// </summary>
        //public Line ALLUserSetting = null;
        //public CoreUsers(Line line,Line set): base(line)
        //{
        //    ALLUserSetting = set;
        //}
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get => this[(gstr)"name"];
            // set => this[(gstr)"name"] = value;
        }
        /// <summary>
        /// 头像
        /// </summary>
        public string Photo
        {
            get => this[(gstr)"photo"];
            // set => this[(gstr)"photo"] = value;
        }
        /// <summary>
        /// 获得头像图片
        /// </summary>
        /// <param name="mw"></param>
        /// <returns></returns>
        public BitmapImage PhotoImage(IMainWindow mw) => string.IsNullOrEmpty(Photo) ? mw.Core.ProfileImage.GetRndImage(UserName) : mw.Core.ImageSources.FindImage("profile_" + Photo, "profile_nomal");

        /// <summary>
        /// 对主人公的好感度
        /// </summary>
        public double GetFriendliness(Line globaluserset)
        {
            Sub gs = globaluserset[UserName];
            if (gs == null)
            {
                return this[(gdbe)"friendliness"];
            }
            return gs.Infos[(gflt)"friendliness"];
        }
        /// <summary>
        /// 对主人公的好感度
        /// </summary>
        public void SetFriendliness(Line globaluserset, int value)
        {
            Sub gs = globaluserset[UserName];
            if (gs == null)
            {
                gs = new Sub(UserName,"");
            }
            gs.Infos[(gflt)"friendliness"] = value;
        }

        /// <summary>
        /// 人物视频标签,方便拉取
        /// </summary>
        public string[] VideoTag
        {
            get
            {
                Sub s = Find("vtag");
                if (s == null)
                    return null;
                s.info = s.info.ToLower();
                return s.GetInfos();
            }
        }
        /// <summary>
        /// 人物游戏标签,方便拉取
        /// </summary>
        /// 一些常用的标签:
        ///     
        public string[] GameTag
        {
            get
            {
                Sub s = Find("gtag");
                if (s == null)
                    return null;
                s.info = s.info.ToLower();
                return s.GetInfos();
            }
        }
        /// <summary>
        /// 确认类型,用户可以为多个类型
        /// 常见类型: 画师,建模师,虚拟主播等, 确认类型后可以获取些特殊的数据
        /// </summary>
        public string UsersType => Info;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 游戏核心数据
    /// </summary>
    public class ICore
    {
        /// <summary>
        /// 游戏校验码
        /// </summary>
        public long HashCode;
        /// <summary>
        /// 游戏窗口
        /// </summary>
        public IMainWindow IMW;

        //所有游戏需要的资源:

        /// <summary>
        /// 所有L2D立绘
        /// </summary>
        public List<Item_L2D_base> L2DBase = new List<Item_L2D_base>();
        /// <summary>
        /// 所有图片链接,从图片库获取l2d图片
        /// </summary>
        public ImageResources ImageSources = new ImageResources();
        /// <summary>
        /// 所有其他文件/文件夹的链接
        /// </summary>
        public Resources Resources = new Resources();
        /// <summary>
        /// 音乐列表
        /// </summary>
        public Resources Music = new Resources();
        /// <summary>
        /// 所有事件库
        /// </summary>
        public List<EventBase> EventBases = new List<EventBase>();
        /// <summary>
        /// 所有可出售物品
        /// </summary>
        public List<Item_Salability> Items = new List<Item_Salability>();
        /// <summary>
        /// 所有Steam游戏
        /// </summary>
        public List<string> Game = new List<string>();
        /// <summary>
        /// 所有用户
        /// </summary>
        public List<Users> Users = new List<Users>();
      

        /// <summary>
        /// 所有评论 TODO:根据评论区分
        /// </summary>
        [Obsolete("请不要使用该方法进行相关评论操作,请使用mw.Save")]
        public List<Comment_base> Comments = new List<Comment_base>();
        /// <summary>
        /// 所有评论 TODO:根据评论区分
        /// </summary>
        [Obsolete("请不要使用该方法进行相关评论操作,请使用mw.Save")]
        public List<Comment_Game> Comments_Game = new List<Comment_Game>();
        /// <summary>
        /// 所有长视频剪辑类型
        /// </summary>
        public List<VideoEditorType> VideoEditorsLong = new List<VideoEditorType>();
        /// <summary>
        /// 所有短视频剪辑类型
        /// </summary>
        public List<VideoEditorType> VideoEditorsShort = new List<VideoEditorType>();
        /// <summary>
        /// 所有特效视频剪辑类型
        /// </summary>
        public List<VideoEditorType> VideoEditorsEffects = new List<VideoEditorType>();
        /// <summary>
        /// 所有其他视频剪辑类型
        /// </summary>
        public List<VideoEditorType> VideoEditorsOther = new List<VideoEditorType>();
        /// <summary>
        /// 所有主题
        /// </summary>
        public List<Theme> Theme = new List<Theme>();
        /// <summary>
        /// 所有字体(位置)
        /// </summary>
        public List<IFont> Fonts = new List<IFont>();

        /// <summary>
        /// 随机生成的头像工具
        /// </summary>
        public ProfileImage ProfileImage = new ProfileImage();
        /// <summary>
        /// 图片生成模板
        /// </summary>
        public List<GenImageTemplate> GenImageTemplates = new List<GenImageTemplate>();


        /// <summary>
        /// 所有三方软件
        /// </summary>
        public List<ISoftWare> SoftWares = new List<ISoftWare>();
        /// <summary>
        /// 所有三方插件
        /// </summary>
        public List<MainPlugin> Plugins = new List<MainPlugin>();

    }


}

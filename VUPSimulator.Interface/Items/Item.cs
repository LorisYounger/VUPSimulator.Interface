using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 物品类
    /// </summary>
    public class Item : Line
    {
        /// <summary>
        /// 物品类型列表,如果需要自己DIY物品可以直接使用int 之后会添加相关接口TODO
        /// </summary>
        public enum ItemType
        {
            none,//无
            l2d_base,//L2D基础类
            l2d,//L2D立绘
            cpu,//CPU部件
            gpu,//GPU部件
            memory,//内存
            motherboard,//主板
            camera,//摄像头
            microphone,//麦克风
            game_base,//游戏基本类
            game,//游戏类
            videoraw,//未剪辑的视频,虽然说不是item类,但是反正都可以拿来用
            videoedit,//未剪辑的视频,虽然说不是item类,但是反正都可以拿来用
            videofin,//未剪辑的视频,虽然说不是item类,但是反正都可以拿来用
        }
        public ItemType Type
        {
            get => (ItemType)Enum.Parse(typeof(ItemType), info);
            set => info = value.ToString();
        }
        public Item(Line line) : base(line)
        {

        }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string ItemName
        {
            get => Find("name").Info;
            set => FindorAdd("name").Info = value;
        }
        /// <summary>
        /// 物品数量
        /// </summary>
        public int Many
        {
            get
            {
                Sub sub = Find("many");
                if (sub == null)
                    return 1;
                return sub.InfoToInt;
            }
            set => FindorAdd("many").InfoToInt = value;
        }
        /// <summary>
        /// 物品单价 为-1则是禁止出售
        /// </summary>
        public int Price
        {
            get
            {
                Sub sub = Find("price");
                if (sub == null)
                    return -1;
                return sub.InfoToInt;
            }
            set => FindorAdd("price").InfoToInt = value;
        }
        /// <summary>
        /// 物品图片
        /// </summary>
        public string Image
        {
            get
            {
                var line = Find("image");
                if (line == null)
                {
                    return info + '_' + ItemName;
                }
                return line.Info;
            }
        }
        /// <summary>
        /// 根据物品类型自动生成相应Item
        /// </summary>
        /// <param name="line">物品</param>
        /// <returns></returns>
        public static Item New(Line line)
        {
            Item item;
            switch (line.info.GetString().ToLower())
            {
                case "l2d_base":
                    item = new Item_L2D_base(line);
                    break;
                case "l2d":
                    item = new Item_L2D(line);
                    break;
                case "cpu":
                    item = new Item_CPU(line);
                    break;
                case "gpu":
                    item = new Item_GPU(line);
                    break;
                case "memory":
                    item = new Item_Memory(line);
                    break;
                case "motherboard":
                    item = new Item_MotherBoard(line);
                    break;
                case "camera":
                    item = new Item_Camera(line);
                    break;
                case "microphone":
                    item = new Item_Microphone(line);
                    break;
                default:
                    item = new Item(line);
                    break;
            }
            return item;
        }
    }
}

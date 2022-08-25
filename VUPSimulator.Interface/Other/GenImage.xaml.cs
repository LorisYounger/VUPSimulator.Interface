using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using LinePutScript;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// Nili 视频自动生成的封面
    /// </summary>
    public partial class GenImage : UserControl
    {
        public Line Data;
        IMainWindow mw;
        /// <summary>
        /// 生成封面
        /// </summary>
        /// <param name="data">数据</param>        
        public GenImage(IMainWindow mw, string data)
        {
            InitializeComponent();
            this.mw = mw;
            Data = new Line(Data);
            Rels();
        }
        /// <summary>
        /// 生成封面
        /// 生成顺序 -背景-用户图像-文本
        /// </summary>
        /// <param name="mw">主窗体</param>
        /// <param name="usrX">用户图像摆放位置x</param>
        /// <param name="usrY">用户图像摆放位置y</param>
        /// <param name="usrZ">用户图像摆放角度0-360</param>
        /// <param name="usrSize">用完图像大小(X长度,宽度自动缩放)</param>
        /// <param name="usrpath">用户图像位置(带:为系统位置,否则为游戏资源)</param>
        /// <param name="textX">文本摆放位置X</param>
        /// <param name="textY">文本摆放位置Y</param>
        /// <param name="textZ">文本摆放角度</param>
        /// <param name="textSize">文字大小</param>
        /// <param name="textfont">文字字体</param>
        /// <param name="text">文字</param>
        /// <param name="backgroundpath">背景图位置(带:为系统位置,否则为游戏资源)</param>
        public GenImage(IMainWindow mw, int usrX, int usrY, int usrZ, int usrSize, string usrpath, int textX, int textY, int textZ, int textSize, string textcolor, string text, string backgroundpath)
        {
            InitializeComponent();
            this.mw = mw;
            Data = new Line("nimg", "");
            Data[(gint)"ux"] = usrX;
            Data[(gint)"uy"] = usrY;
            Data[(gint)"uz"] = usrZ;
            Data[(gint)"usize"] = usrSize;
            Data[(gstr)"upath"] = usrpath;
            Data[(gint)"tx"] = textX;
            Data[(gint)"ty"] = textY;
            Data[(gint)"tz"] = textZ;
            Data[(gint)"tsize"] = textSize;
            Data[(gstr)"tcolor"] = textcolor;
            Data[(gstr)"text"] = text;
            Data[(gstr)"bg"] = backgroundpath;
            Rels();
        }
        public void Rels()
        {

        }
        public override string ToString()
        {
            return Data.ToString();
        }
        public abstract class GIBase
        {
            public double Opacity = 1;
            public GIBase(double opacity)
            {
                Opacity = opacity;
            }
            public GIBase()
            {
            }
            public abstract FrameworkElement GenUI(IMainWindow mw);
            public abstract Sub ToSub();

            protected private static ImageSource getImage(IMainWindow mw, string image)
            {
                if (image.StartsWith(":"))
                {
                    if (System.IO.File.Exists(image))
                    {
                        return new BitmapImage(new Uri(image));
                    }
                    else
                    {
                        return new BitmapImage(new Uri("pack://application:,,,/Res/Image/system/error.png"));
                    }
                }
                else
                {
                    return mw.Core.ImageSources.FindImage(image);
                }
            }

        }
        public abstract class GIPlaceBase : GIBase
        {
            public GIPlaceBase(double x, double y, double opacity = 1) : base(opacity)
            {
                PointX = x;
                PointY = y;
            }
            public double PointX;
            public double PointY;
        }
        public class GIBackGround : GIBase
        {

            public string BackGround;
            public GIBackGround(string backgroundpath, double opacity = 1) : base(opacity)
            {
                BackGround = backgroundpath;
            }
            public GIBackGround(Sub sub)
            {
                BackGround = sub.Infos[(gstr)"bg"];
            }
            public override FrameworkElement GenUI(IMainWindow mw)
            {
                return new Image() { Source = getImage(mw, BackGround), Opacity = Opacity };
            }
            public override Sub ToSub()
            {
                return new Sub("GIBackGround", $"{Opacity},{BackGround}");
            }
        }
    }
}

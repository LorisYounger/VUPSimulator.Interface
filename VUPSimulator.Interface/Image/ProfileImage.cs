using LinePutScript;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 随机生成profile图片
    /// 顶层->底层
    /// 0装饰: 头发脸部装饰等 为可选(20%莫得)
    /// 1前头发: 覆盖在前面的头发,和后头发绑定
    /// 2脸装饰: 覆盖在脸上的装饰 为可选(20%莫得)
    /// 3嘴巴: 覆盖在脸上的嘴巴 为可选
    /// 4衣服: 因为是头像,衣服部分不多,大约到袖口位置
    /// 5眼睛: 眼睛+表情,不包括脸
    /// 6脸+身体: 脸部+身体
    /// 7后头发: 在脸部后面的头发,和前头发绑定
    /// ---
    /// 如果rgb值如果是相等的，就会自动生成相应灰度随机的颜色，0和255除外
    /// </summary>
    public class ProfileImage
    {
        /// <summary>
        /// 图层层数
        /// </summary>
        public const int len = 8;
        /// <summary>
        /// 文件列表
        /// </summary>
        public List<string>[] Files = new List<string>[len];
        /// <summary>
        /// 图片缓存
        /// </summary>
        public Dictionary<int, BitmapImage> cache = new Dictionary<int, BitmapImage>();
        public ProfileImage()
        {
            for (int i = 0; i < len; i++)
                Files[i] = new List<string>();
        }
        public BitmapImage GetRndImage(int hash)
        {
            hash = Math.Abs(hash);
            if (cache.ContainsKey(hash))
                return cache[hash];

            string[] imgs = new string[len];
            for (int i = 0; i < len - 1; i++)
                imgs[i] = Files[i][(hash / (i + 1)) % Files.Length];
            imgs[7] = Files[7][(hash / 2) % Files.Length];

            foreach (int i in new int[] { 0, 2 })
                if ((hash / (100 + i)) % 5 == 0)
                    imgs[i] = null;


            //计算人物色系
            float H = (hash) % 360;
            float[] S = new float[len];
            for (int i = 0; i < len - 1; i++)
                S[i] = (hash / (i + 10)) % 100 / 100f;
            S[7] = S[1];

            BitmapImage bitmapImage = new BitmapImage();
            using (Bitmap bitMap = new Bitmap(64, 64))
            {
                using (Graphics g1 = Graphics.FromImage(bitMap))
                {
                    for (int i = len - 1; i >= 0; i--)
                        if (imgs[i] != null)
                            using (Bitmap img = new Bitmap(imgs[i]))
                            {
                                for (int x = 0; x < img.Width; x++)
                                    for (int y = 0; y < img.Width; y++)
                                    {
                                        var pix = img.GetPixel(x, y);
                                        if (pix.R == pix.G && pix.G == pix.B && pix.R != 0 && pix.R != 255)
                                            img.SetPixel(x, y, HSBtoRGB(H, S[i], pix.R, pix.A));
                                    }
                                g1.DrawImage(img, 0, 0, 64, 64);
                            }
                }
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }
            }
            cache.Add(hash, bitmapImage);
            return bitmapImage;
        }
        public BitmapImage GetRndImage(string name) => GetRndImage((int)Sub.GetHashCode(name));

        /// <summary>
        /// HSB颜色值转颜色
        /// </summary>
        /// <param name="H">色相 0-359</param>
        /// <param name="S">饱和度 0-100%</param>
        /// <param name="B">亮度 0-100%</param>
        /// <param name="alpha">透明度 0-255 可选</param>
        /// <returns>颜色</returns>
        public static Color HSBtoRGB(float H, float S, float B, byte alpha = 255)
        {
            float[] rgb = new float[3];
            //先令饱和度和亮度为100%，调节色相h
            for (int offset = 240, i = 0; i < 3; i++, offset -= 120)
            {
                //算出色相h的值和三个区域中心点(即0°，120°和240°)相差多少，然后根据坐标图按分段函数算出rgb。但因为色环展开后，红色区域的中心点是0°同时也是360°，不好算，索性将三个区域的中心点都向右平移到240°再计算比较方便
                float x = Math.Abs((H + offset) % 360 - 240);
                //如果相差小于60°则为255
                if (x <= 60) rgb[i] = 255;
                //如果相差在60°和120°之间，
                else if (60 < x && x < 120) rgb[i] = ((1 - (x - 60) / 60) * 255);
                //如果相差大于120°则为0
                else rgb[i] = 0;
            }
            //在调节饱和度s
            for (int i = 0; i < 3; i++)
                rgb[i] += (255 - rgb[i]) * (1 - S);
            //最后调节亮度b
            for (int i = 0; i < 3; i++)
                rgb[i] *= B;
            return Color.FromArgb(alpha, (int)rgb[0], (int)rgb[1], (int)rgb[2]);
        }
        /// <summary>
        /// HSB颜色值转颜色
        /// </summary>
        /// <param name="H">色相 0-359</param>
        /// <param name="S">饱和度 0-100%</param>
        /// <param name="B">亮度 0-255</param>
        /// <param name="alpha">透明度 0-255 可选</param>
        /// <returns>颜色</returns>
        public static Color HSBtoRGB(float H, float S, byte B, byte alpha = 255) => HSBtoRGB(H, S, B / 255f, alpha);
    }
}

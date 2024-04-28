﻿using LinePutScript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VUPSimulator.Interface
{
    /// <summary>
    /// 游戏主题
    /// </summary>
    public class Theme
    {
        public string Name;
        public string xName;
        public string Image;
        public ImageResources Images;
        public LPS ThemeColor;
        public Theme(LPS lps)
        {
            xName = lps.First().Name;
            Name = lps.First().Info;
            Image = lps.First().Find("image").info;

            lps.RemoveAt(0);
            ThemeColor = lps;

            Images = new ImageResources();
        }
    }
    /// <summary>
    /// 字体
    /// </summary>
    public class IFont
    {
        public string Name;
        public string Path;
        public IFont(FileInfo path)
        {
            Name = path.Name.Substring(0, path.Name.Length - path.Extension.Length);
            Path = path.Directory.FullName + @"\#" + Name;
        }
        public FontFamily Font
        {
            get
            {//file:///D:\Documents\Visual Studio 2022\Projects\VPet\VPet-Simulator.Windows\Res\#凤凰点阵体 12px
                return new FontFamily(@"file:///" + Path);
            }
        }
    }
}

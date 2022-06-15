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
using System.Windows.Shapes;
using VUPSimulator.Interface;

namespace CheatEngine
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class winCheatEngine : UserControl, WindowsPageHandle
    {
        IMainWindow mw;
        public winCheatEngine(IMainWindow mainwin)
        {
            InitializeComponent();
            mw = mainwin;
            host = mw.ShowWindows(this, "Cheat Engine", mw.Core.ImageSources.FindImageUri("software_CheatEngine"));
        }

        public string ID => "winCheatEngine";

        public bool AllowMax => false;

        public bool AllowHide => true;

        private IWindows host;
        public IWindows Host => host;

        public FrameworkElement This => this;

        public ComputerUsage Usage { get => usage; set => usage = value; }

        private ComputerUsage usage = new ComputerUsage("CheatEngine")
        {
            CPUUsage = 2,
            GPUUsage = 0.2,
            MemoryUsage = 40,
            Import = 10
        };

        public bool Closeing()
        {
            return true;
        }

        public void Hide() { }

        public void Max()
        {
            throw new NotImplementedException();
        }

        public void Min()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            Host.Visibility = Visibility.Visible;
            mw.Toppext(Host);
        }


        public void ADDP(double i)
        {
            mw.Save.Pidear += i;
            mw.Save.Pspeak += i;
            mw.Save.Poperate += i;
            mw.Save.Pimage += i;
            mw.Save.Pclip += i;
            mw.Save.Pdraw += i;
            mw.Save.Pgame += i;
            mw.Save.Psong += i;
        }

        private void C_ADD1(object sender, RoutedEventArgs e)
        {
            ADDP(1);
        }

        private void C_ADD_5(object sender, RoutedEventArgs e)
        {
            ADDP(5);
        }

        private void C_HealthFull(object sender, RoutedEventArgs e)
        {
            mw.Save.Health = 100;            
        }

        private void C_ADD_MONEY_10(object sender, RoutedEventArgs e)
        {
            mw.Save.Money += 100000;
        }

        private void C_ADD_MONEY_1(object sender, RoutedEventArgs e)
        {
            mw.Save.Money += 10000;
        }

        private void C_ST_FULL(object sender, RoutedEventArgs e)
        {
            mw.Save.Strength = mw.Save.Health;
        }
    }
}

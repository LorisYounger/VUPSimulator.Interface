using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;
namespace VUPSimulator.Interface
{
    /// <summary>
    /// 电脑的数据非常多,单独拿出来用
    /// </summary>
    public class Computer
    {
        public Item_CPU CPU;
        public Item_GPU GPU;
        public Item_MotherBoard MotherBoard;
        public List<Item_Memory> Memorys = new List<Item_Memory>();
        public List<Item_Camera> Cameras = new List<Item_Camera>();
        public List<Item_Microphone> Microphones = new List<Item_Microphone>();
        /// <summary>
        /// 电脑名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 重新计算 电脑信息
        /// </summary>
        public void ReCalValue()
        {
            camtotalqual = -1;
            micrototalqual = -1;
            memtotal = -1;
            cputotal = double.MinValue;
            gputotal = double.MinValue;
        }
        public Computer(Line line, IMainWindow mw, List<Item> items)
        {

            Name = line.Info;
            CPU = (Item_CPU)items.Find(x => x.ItemName == line.Find("cpu").Info);
            GPU = (Item_GPU)items.Find(x => x.ItemName == line.Find("gpu").Info);
            MotherBoard = (Item_MotherBoard)items.Find(x => x.ItemName == line.Find("motherboard").Info);
            //删掉正在使用的零件,等保存的时候再导出
            items.Remove(CPU);
            items.Remove(GPU);
            items.Remove(MotherBoard);

            foreach (string str in line.Find("memory").GetInfos())
            {
                var mem = (Item_Memory)items.Find(x => x.ItemName == str);
                items.Remove(mem);
                Memorys.Add(mem);
            }
            foreach (string str in line.Find("camera").GetInfos())
            {
                var cms = (Item_Camera)items.Find(x => x.ItemName == str);
                items.Remove(cms);
                Cameras.Add(cms);
            }
            foreach (string str in line.Find("microphone").GetInfos())
            {
                var mcf = (Item_Microphone)items.Find(x => x.ItemName == str);
                items.Remove(mcf);
                Microphones.Add(mcf);
            }
            mw.TimeHandle += CalculatUsage;

            CalculatUsage(TimeSpan.Zero, mw);
        }
        /// <summary>
        /// 用于显示的CPU使用率 (100%)
        /// </summary>
        public double CPU_Usage
        {
            get
            {
                double tmp = (int)(CPU_Real_Usage / CPUTotal * 1000) / 10;
                //if (tmp >= 100) //允许超过100
                //    tmp = 100;
                //else
                if (tmp < 0)
                    tmp = 0;
                return tmp;
            }
        }
        /// <summary>
        /// 用于显示的CPU使用率 (GHZ)
        /// </summary>
        public double CPU_Show_Usage => CPU_Real_Usage / CPUTotal * CPU.Speed;
        /// <summary>
        /// CPU实际使用的值(基于TOTAL)
        /// </summary>
        public double CPU_Real_Usage;
        /// <summary>
        /// GPU实际使用的值(基于TOTAL)
        /// </summary>
        public double GPU_Real_Usage;
        /// <summary>
        /// 用于显示的GPU使用率 (100%)
        /// </summary>
        public double GPU_Usage
        {
            get
            {
                double tmp = (int)(GPU_Real_Usage / GPUTotal * 1000) / 10;
                //if (tmp >= 100) //允许超过100
                //    tmp = 100;
                //else
                if (tmp < 0)
                    tmp = 0;
                return tmp;
            }
        }
        /// <summary>
        /// 用于显示的内存使用率 (MB)
        /// </summary>
        public int Memory_Real_Usage;
        /// <summary>
        /// 用于显示的内存使用率 (100%)
        /// </summary>
        public int Memory_Usage
        {
            get
            {
                int tmp = 100 * Memory_Real_Usage / MemorysTotal;
                //if (tmp >= 100) //允许超过100
                //    tmp = 100;
                //else
                if (tmp < 0)
                    tmp = 0;
                return tmp;
            }
        }
        /// <summary>
        /// 总摄像头质量
        /// </summary>

        public int CamerasTotalQuality
        {
            get
            {
                if (camtotalqual == -1)
                {
                    double qual = Cameras[0].Quality;
                    for (int i = 1; i < Cameras.Count; i++)
                    {
                        qual += Cameras[i].Quality * Math.Sqrt(i + 1) - Math.Sqrt(i);
                    }
                    camtotalqual = (int)qual;
                }
                return camtotalqual;
            }
        }
        private int camtotalqual = -1;
        /// <summary>
        /// 总麦克风质量
        /// </summary>
        public int MicrophonesTotalQuality
        {
            get
            {
                if (micrototalqual == -1)
                {
                    double qual = Microphones[0].Quality;
                    for (int i = 1; i < Microphones.Count; i++)
                    {
                        qual += Microphones[i].Quality * Math.Sqrt(i + 1) - Math.Sqrt(i);
                    }
                    micrototalqual = (int)qual;
                }
                return micrototalqual;
            }
        }
        private int micrototalqual = -1;
        /// <summary>
        /// 总可用内存
        /// </summary>
        public int MemorysTotal
        {
            get
            {
                if (memtotal == -1)
                {
                    int memory = 0;
                    foreach (Item_Memory mem in Memorys)
                    {
                        memory += mem.Size;
                    }
                    memtotal = memory;
                }
                return memtotal;
            }
        }
        private int memtotal = -1;
        /// <summary>
        /// 总可用CPU 单位0.1Ghz
        /// </summary>
        public double CPUTotal
        {
            get
            {
                if (cputotal == double.MinValue)
                {
                    cputotal = CPU.TotalSpeed;
                }
                return cputotal;
            }
        }
        private double cputotal = double.MinValue;
        /// <summary>
        /// 总可用GPU 单位Ghz
        /// </summary>
        public double GPUTotal
        {
            get
            {
                if (gputotal == double.MinValue)
                {
                    gputotal = GPU.TotalSpeed;
                }
                return gputotal;
            }
        }
        private double gputotal = double.MinValue;
        /// <summary>
        /// 系统使用
        /// </summary>
        public ComputerUsage SystemCU = new ComputerUsage("System")
        {
            CPUUsage = 10,
            GPUUsage = 2,
            MemoryUsage = 400,
            Import = 0
        };
        /// <summary>
        /// 计算程序分配的资源
        /// </summary>
        /// <param name="span">时间间隔</param>
        /// <param name="mw">主窗口</param>
        public void CalculatUsage(TimeSpan span, IMainWindow mw)
        {
            double cpulf = CPUTotal;
            double gpulf = GPUTotal;
            int memlf = MemorysTotal;

            List<ComputerUsage> list = new List<ComputerUsage>();
            SystemCU.Clean();

            //为系统添加额外的占用
            if (SystemCU.CPUUsage < CPUTotal * .2)
            {
                SystemCU.CPUUsage += span.TotalDays * CPUTotal / 10;
                if (SystemCU.CPUUsage > CPUTotal * .2)
                    SystemCU.CPUUsage = CPUTotal * .2 + 0.1;
            }
            if (SystemCU.GPUUsage < GPUTotal * .2)
            {
                SystemCU.GPUUsage += span.TotalDays * GPUTotal / 10;
                if (SystemCU.GPUUsage > GPUTotal * .2)
                    SystemCU.GPUUsage = GPUTotal * .2 + 0.1;
            }
            if (SystemCU.MemoryUsage < MemorysTotal / 4)
            {
                SystemCU.MemoryUsage += (int)(span.TotalDays * MemorysTotal / 8);
                if (SystemCU.MemoryUsage > MemorysTotal / 4)
                    SystemCU.MemoryUsage = MemorysTotal / 4 + 1;
            }

            list.Add(SystemCU);
            foreach (WindowsPageHandle mp in mw.AllWindows)
            {
                mp.Usage.Clean();
                list.Add(mp.Usage);
            }
            list = list.OrderBy(x => x.Import).ToList();

            double totaltmp = 0;
            foreach (ComputerUsage cu in list)
            {
                totaltmp += cu.CPUUsageReal;
            }
            if (totaltmp <= cpulf)
            {//资源充足,直接全分配
                foreach (ComputerUsage cu in list)
                {
                    cu.DistributionCPU(ref cpulf, 1);
                }
            }
            else
            {
                double realps = cpulf / totaltmp * 0.8;//先分80%
                foreach (ComputerUsage cu in list)
                {
                    cu.DistributionCPU(ref cpulf, realps);
                }

                //第二次分配
                foreach (ComputerUsage cu in list)//剩下的按优先级分配
                {
                    if (cpulf <= 0)
                        break;
                    cu.DistributionCPU(ref cpulf, 1);
                }
            }

            totaltmp = 0;
            foreach (ComputerUsage cu in list)
            {
                totaltmp += cu.GPUUsageReal;
            }
            if (totaltmp <= gpulf)
            {//资源充足,直接全分配
                foreach (ComputerUsage cu in list)
                {
                    cu.DistributionGPU(ref gpulf, 1);
                }
            }
            else
            {
                double realps = gpulf / totaltmp * 0.8;//先分80%
                foreach (ComputerUsage cu in list)
                {
                    cu.DistributionGPU(ref gpulf, realps);
                }

                //第二次分配
                foreach (ComputerUsage cu in list)//剩下的按优先级分配
                {
                    if (gpulf <= 0)
                        break;
                    cu.DistributionGPU(ref gpulf, 1);
                }
            }

            int totalmem = 0;
            foreach (ComputerUsage cu in list)
            {
                totalmem += cu.MemoryUsageReal;
            }
            if (totalmem <= memlf)
            {//资源充足,直接全分配
                foreach (ComputerUsage cu in list)
                {
                    cu.DistributionMemory(ref memlf, 1);
                }
            }
            else
            {
                double realps = memlf / totalmem * 0.8;//先分80%
                foreach (ComputerUsage cu in list)
                {
                    cu.DistributionMemory(ref memlf, realps);
                }

                //第二次分配
                foreach (ComputerUsage cu in list)//剩下的按优先级分配
                {
                    if (memlf <= 0)
                        break;
                    cu.DistributionMemory(ref memlf, 1);
                }
            }

            CPU_Real_Usage = CPUTotal - cpulf;
            GPU_Real_Usage = GPUTotal - gpulf;
            Memory_Real_Usage = MemorysTotal - memlf;
        }
        /// <summary>
        /// 导出可保存的计算机
        /// </summary>
        /// <returns>计算机和相关物品</returns>
        public List<Line> ToLine()
        {
            List<Line> lines = new List<Line>
            {
                new Line("computer", Name, "", new Sub("cpu", CPU.ItemName), new Sub("gpu", GPU.ItemName), new Sub("motherboard", MotherBoard.ItemName)),
                CPU,
                GPU,
                MotherBoard
            };

            string tmp = "memory#";
            foreach (Item_Memory mem in Memorys)
            {
                lines.Add(mem);
                tmp += mem.ItemName;
                tmp += ',';
            }
            lines.First().AddSub(new Sub(tmp.Trim(',')));
            tmp = "camera#";
            foreach (Item_Camera cam in Cameras)
            {
                lines.Add(cam);
                tmp += cam.ItemName;
                tmp += ',';
            }
            lines.First().AddSub(new Sub(tmp.Trim(',')));
            tmp = "microphone#";
            foreach (Item_Microphone mic in Microphones)
            {
                lines.Add(mic);
                tmp += mic.ItemName;
                tmp += ',';
            }
            lines.First().AddSub(new Sub(tmp.Trim(',')));
            return lines;
        }
    }
    /// <summary>
    /// 所有应用程序的系统占用
    /// </summary>
    public class ComputerUsage
    {
        /// <summary>
        /// 电脑名称,用于显示在任务管理器
        /// </summary>
        public string SoftWareName;
        public ComputerUsage()
        {
            SoftWareName = "";
        }
        public ComputerUsage(string softwarename)
        {
            SoftWareName = softwarename;
        }
        /// <summary>
        /// 这个程序申请CPU使用的值 单位0.1GHZ 其中默认程序cpu占用为0.1GHZ,系统1ghz
        /// </summary>
        public double CPUUsage = 1;
        /// <summary>
        /// 这个程序申请GPU使用的值 单位GHZ 其中默认程序Gpu占用为0.1GHZ,系统2GHZ
        /// </summary>
        public double GPUUsage = 0.1;
        /// <summary>
        /// 这个程序申请内存使用值 单位MB 其中默认程序内存占用为20mb,系统400mb
        /// </summary>
        public int MemoryUsage = 20;
        /// <summary>
        /// 这个程序实际被分配了多少CPU 单位0.1GHZ 其中默认程序cpu占用为0.1GHZ,系统0.4ghz
        /// </summary>
        public double CPUWork;
        /// <summary>
        /// 这个程序实际被分配了多少GPU 单位GHZ 单位GHZ 其中默认程序Gpu占用为1GHZ,系统2GHZ
        /// </summary>
        public double GPUWork;
        /// <summary>
        /// 这个程序被分配了多少内存 单位MB 其中默认程序内存占用为20mb,系统400mb
        /// </summary>
        public int MemoryWork;
        /// <summary>
        /// 这个程序实际申请CPU使用的值 是CPUUSG的80-120%
        /// </summary>
        public double CPUUsageReal;
        /// <summary>
        /// 这个程序申请GPU使用的值 单位GHZ 其中默认程序Gpu占用为0.1GHZ,系统2GHZ
        /// </summary>
        public double GPUUsageReal;
        /// <summary>
        /// 这个程序申请内存使用值 单位MB 其中默认程序内存占用为20mb,系统400mb
        /// </summary>
        public int MemoryUsageReal;
        /// <summary>
        /// 这个程序实际效率 0-100% 其中低于50%会卡顿 低于10%会强制关闭
        /// </summary>
        public int TotalWork
        {//同时这个也可以应用于软件渲染等
            get
            {
                if (CPUUsageReal == 0)
                    return 100;//默认值
                //计算使用
                double total = (CPUWork / CPUUsageReal * ImportCPU);
                total += (GPUWork / GPUUsageReal * ImportGPU);
                total += ((double)MemoryWork / MemoryUsageReal * ImportMemory);
                return (int)total;
            }
        }
        /// <summary>
        /// 指示CPU的重要性 1-100 和其他重要性加起来等于100
        /// </summary>
        public int ImportCPU = 50;
        /// <summary>
        /// 指示GPU的重要性 1-100 和其他重要性加起来等于100
        /// </summary>
        public int ImportGPU = 10;
        /// <summary>
        /// 指示内存的重要性 1-100 和其他重要性加起来等于100
        /// </summary>
        public int ImportMemory = 40;
        /// <summary>
        /// 程序优先级,越低越会优先分配内存
        /// </summary>
        public int Import = 5;

        /// <summary>
        /// 清理之前分配的资源 以及随机分配实际值
        /// </summary>
        public void Clean()
        {
            CPUWork = 0;
            GPUWork = 0;
            MemoryWork = 0;
            CPUUsageReal = (Function.Rnd.NextDouble() * 0.4 + 0.8) * CPUUsage;
            GPUUsageReal = (Function.Rnd.NextDouble() * 0.4 + 0.8) * GPUUsage;
            MemoryUsageReal = (int)((Function.Rnd.NextDouble() * 0.4 + 0.8) * MemoryUsage);
        }

        /// <summary>
        /// 分配CPU资源给这个程序占用
        /// </summary>
        /// <param name="cpuf">剩余CPU资源</param>
        /// <param name="prs">分配多少百分比</param>
        public void DistributionCPU(ref double cpuf, double prs)
        {
            double inneed = CPUUsageReal - CPUWork;
            if (cpuf <= 0 || prs <= 0)
                return;
            if (prs < 1)
                inneed *= prs;
            if (inneed > cpuf)
            {
                CPUWork += cpuf;
                cpuf = 0;
            }
            else
            {
                cpuf -= inneed;
                CPUWork = inneed;
            }
        }
        /// <summary>
        /// 分配GPU资源给这个程序占用
        /// </summary>
        /// <param name="gpuf">剩余GPU资源</param>
        /// <param name="prs">分配多少百分比</param>
        public void DistributionGPU(ref double gpuf, double prs)
        {
            double inneed = GPUUsageReal - GPUWork;
            if (gpuf <= 0 || prs <= 0)
                return;
            if (prs < 1)
                inneed *= prs;
            if (inneed > gpuf)
            {
                GPUWork += gpuf;
                gpuf = 0;
            }
            else
            {
                gpuf -= inneed;
                GPUWork = inneed;
            }
        }
        /// <summary>
        /// 分配内存资源给这个程序占用
        /// </summary>     
        /// <param name="memf">剩余内存资源</param>
        /// <param name="prs">分配多少百分比</param>
        public void DistributionMemory(ref int memf, double prs)
        {
            int inneed = MemoryUsageReal - MemoryWork;
            if (memf <= 0 || prs <= 0)
                return;
            if (prs < 1)
                inneed = (int)(inneed * prs);
            if (inneed > memf)
            {
                MemoryWork += memf;
                memf = 0;
            }
            else
            {
                memf -= inneed;
                MemoryWork = inneed;
            }
        }
    }

    /// <summary>
    /// CPU物品
    /// </summary>
    public class Item_CPU : Item_Salability
    {
        public Item_CPU(Line line) : base(line)
        {

        }
        /// <summary>
        /// 速度 单位GHZ 其中默认程序cpu占用为0.1GHZ,系统0.4ghz
        /// </summary>
        public double Speed => Find("speed").InfoToDouble;
        /// <summary>
        /// 核心数
        /// </summary>
        public int CoreNumber => Find("corenum").InfoToInt;
        /// <summary>
        /// 耐久度 电脑随着使用时间可能会有所损耗,默认10 (每年损耗/事件损耗)
        /// </summary>
        public int Durability
        {
            get => GetInt("dura", 10);
            set => this[(gint)"dura"] = value;
        }
        /// <summary>
        /// 总可用速度 单位0.1GHZ
        /// </summary>
        public int TotalSpeed => (int)(Speed * Math.Pow(CoreNumber, 0.8) * Durability);
        /// <summary>
        /// 所有电子产品个数只能为1
        /// </summary>
        public new int Many
        {
            get => 1;
        }
        /// <summary>
        /// 商品分类信息
        /// </summary>
        public override string[] Categories => new string[] { "配件", "CPU" };
        /// <summary>
        /// 物品描述
        /// </summary>
        public override string Description
        {
            get => GetString("desc", ItemDisplayName) + $"\n速度:{Speed}GHZ\n核心数:{CoreNumber}";
        }
        /// <summary>
        /// 核心数量
        /// </summary>
        public override double SortValue => CoreNumber;
        /// <summary>
        /// 允许多个物品数量堆叠
        /// </summary>
        public override bool AllowMultiple => false;
    }

    /// <summary>
    /// GPU物品
    /// </summary>
    public class Item_GPU : Item_Salability
    {
        public Item_GPU(Line line) : base(line)
        {

        }
        /// <summary>
        /// 频率 单位MHZ 
        /// </summary>
        public int Speed => Find("speed").InfoToInt;
        /// <summary>
        /// CUDA核心数
        /// </summary>
        public int CoreNumber => Find("corenum").InfoToInt;
        /// <summary>
        /// 耐久度 电脑随着使用时间可能会有所损耗,默认10 (每年损耗/事件损耗)
        /// </summary>
        public int Durability
        {
            get
            {
                Sub sub = Find("dura");
                if (sub == null)
                    return 10;
                return sub.InfoToInt;
            }
            set => FindorAdd("dura").InfoToInt = value;
        }
        /// <summary>
        /// 总可用速度 单位GHZ 其中默认程序Gpu占用为0GHZ,系统2GHZ,占用仅显示百分比
        /// </summary>
        public int TotalSpeed => (int)(Speed * Math.Sqrt(CoreNumber) * Durability / 10000);
        /// <summary>
        /// 商品分类信息
        /// </summary>
        public override string[] Categories => new string[] { "配件", "GPU" };
        /// <summary>
        /// 物品描述
        /// </summary>
        public override string Description
        {
            get => GetString("desc", ItemDisplayName) + $"\n速度:{Speed}MHZ\n核心数:{CoreNumber}";
        }
        /// <summary>
        /// 频率
        /// </summary>
        public override double SortValue => Speed;
        /// <summary>
        /// 允许多个物品数量堆叠
        /// </summary>
        public override bool AllowMultiple => false;
    }
    /// <summary>
    /// 内存
    /// </summary>
    public class Item_Memory : Item_Salability
    {
        public Item_Memory(Line line) : base(line)
        {

        }
        /// <summary>
        /// 频率 单位MHZ 只是好看,没啥用
        /// </summary>
        public int Speed => Find("speed").InfoToInt;

        /// <summary>
        /// 大小 决定可用内存可用数量,单位MB 其中默认程序内存占用为10mb,系统200mb
        /// </summary>
        public int Size => Find("size").InfoToInt;
        /// <summary>
        /// 所有电子产品个数只能为1
        /// </summary>
        public new int Many
        {
            get => 1;
        }
        /// <summary>
        /// 商品分类信息
        /// </summary>
        public override string[] Categories => new string[] { "配件", "内存" };
        /// <summary>
        /// 物品描述
        /// </summary>
        public override string Description
        {
            get => GetString("desc", ItemDisplayName) + $"\n频率:{Speed}MHZ\n大小:{Function.SizeConvert(Size)}";
        }
        /// <summary>
        /// 内存大小
        /// </summary>
        public override double SortValue => Size;
        /// <summary>
        /// 允许多个物品数量堆叠
        /// </summary>
        public override bool AllowMultiple => false;
    }
    /// <summary>
    /// 主板
    /// </summary>
    public class Item_MotherBoard : Item_Salability
    {
        public Item_MotherBoard(Line line) : base(line)
        {

        }
        /// <summary>
        /// 网络速度支持 单位MB 默认程序占用为0,系统占用为0
        /// </summary>
        public int Network => Find("network").InfoToInt;

        /// <summary>
        /// 支持多少张内存
        /// </summary>
        public int MemorySupport => Find("memorysupport").InfoToInt;
        /// <summary>
        /// 支持多少个外置麦克风 分为主,次,2次,3次等等 主麦克风100%,次麦克风sqrt(2)-1,次次麦克风sqrt(3)-sqrt(2)以此类推
        /// </summary>
        public int MicrophoneSupport => Find("microphonesupport").InfoToInt;
        /// <summary>
        /// 支持多少个外置摄像头 分为主,次,2次,3次等等 主摄像头100%,次摄像头sqrt(2)-1,次次摄像头sqrt(3)-sqrt(2)以此类推
        /// </summary>
        public int CameraSupport => Find("camerasupport").InfoToInt;
        /// <summary>
        /// 所有电子产品个数只能为1
        /// </summary>
        public new int Many
        {
            get => 1;
        }
        /// <summary>
        /// 商品分类信息
        /// </summary>
        public override string[] Categories => new string[] { "配件", "主板" };
        /// <summary>
        /// 物品描述
        /// </summary>
        public override string Description
        {
            get => GetString("desc", ItemDisplayName) + $"\n网络速度:{Network}MBPS\n支持内存:{MemorySupport}个\n支持麦克风:{MicrophoneSupport}个\n支持摄像头:{CameraSupport}个";
        }
        /// <summary>
        /// 支持的外设数量
        /// </summary>
        public override double SortValue => MemorySupport + MicrophoneSupport + CameraSupport;
        /// <summary>
        /// 允许多个物品数量堆叠
        /// </summary>
        public override bool AllowMultiple => false;
    }
    /// <summary>
    /// 摄像头
    /// </summary>
    public class Item_Camera : Item_Salability
    {
        public Item_Camera(Line line) : base(line)
        {

        }
        /// <summary>
        /// 分辨率 只是好看
        /// </summary>
        public string Resolution => Find("resolution").Info;

        /// <summary>
        /// 成像质量 范围 50-200 影响L2d捕获成功率
        /// </summary>
        public int Quality => Find("quality").InfoToInt;
        /// <summary>
        /// 所有电子产品个数只能为1
        /// </summary>
        public new int Many
        {
            get => 1;
        }
        /// <summary>
        /// 商品分类信息
        /// </summary>
        public override string[] Categories => new string[] { "配件", "摄像头" };
        /// <summary>
        /// 物品描述
        /// </summary>
        public override string Description
        {
            get => GetString("desc", ItemDisplayName) + $"\n分辨率:{Resolution}\n成像质量:{Quality}";
        }
        /// <summary>
        /// 成像质量
        /// </summary>
        public override double SortValue => Quality;
        /// <summary>
        /// 允许多个物品数量堆叠
        /// </summary>
        public override bool AllowMultiple => false;
    }

    /// <summary>
    /// 麦克风
    /// </summary>
    public class Item_Microphone : Item_Salability
    {
        public Item_Microphone(Line line) : base(line)
        {

        }
        /// <summary>
        /// 分辨率 只是好看 (eg:16位 32000hz)
        /// </summary>
        public string Resolution => Find("resolution").Info;

        /// <summary>
        /// 成像质量 范围 50-200 影响声音质量
        /// </summary>
        public int Quality => Find("quality").InfoToInt;
        /// <summary>
        /// 所有电子产品个数只能为1
        /// </summary>
        public new int Many
        {
            get => 1;
        }
        /// <summary>
        /// 商品分类信息
        /// </summary>
        public override string[] Categories => new string[] { "配件", "麦克风" };
        /// <summary>
        /// 物品描述
        /// </summary>
        public override string Description
        {
            get => GetString("desc", ItemDisplayName) + $"\n分辨率:{Resolution}\n成像质量:{Quality}";
        }
        /// <summary>
        /// 成像质量
        /// </summary>
        public override double SortValue => Quality;
        /// <summary>
        /// 允许多个物品数量堆叠
        /// </summary>
        public override bool AllowMultiple => false;
    }
}

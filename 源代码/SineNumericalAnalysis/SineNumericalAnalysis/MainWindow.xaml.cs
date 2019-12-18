using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SineNumericalAnalysis
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    struct NumericalAnalysis
    {
        private double aimDelta;
        private double storageAcc;
        private double x;
        private double result;
        private double truncationDelta;
        private double methodDelta;
        private double calDelta;
        private UInt64 iterationTimes;
        private double realResult;
        private TimeSpan runTime;
        private TimeSpan sysRunTime;
        private double realDelta;
        private double sqrtMaxiterationTimes; //

        public double CalDelta { get => calDelta; set => calDelta = value; }
        public double Result { get => result; set => result = value; }
        public double X { get => x; set => x = value; }
        public double AimDelta { get => aimDelta; set => aimDelta = value; }
        public UInt64 IterationTimes { get => iterationTimes; set => iterationTimes = value; }
        public double StorageAcc { get => storageAcc; set => storageAcc = value; }
        public double TruncationDelta { get => truncationDelta; set => truncationDelta = value; }
        public double MethodDelta { get => methodDelta; set => methodDelta = value; }
        public double RealResult { get => realResult; set => realResult = value; }
        public double RealDelta { get => realDelta; set => realDelta = value; }
        public double SqrtMaxiterationTimes { get => sqrtMaxiterationTimes; set => sqrtMaxiterationTimes = value; }
        public TimeSpan RunTime { get => runTime; set => runTime = value; }
        public TimeSpan SysRunTime { get => sysRunTime; set => sysRunTime = value; }
    };
    struct ScientificConstant
    {
        private readonly double pi;
        private readonly double e;
        private readonly double sqrt3;

        public ScientificConstant(double pi, double e, double sqrt3)
        {
            this.pi = pi;
            this.e = e;
            this.sqrt3 = sqrt3;
        }

        public double Pi => pi;
        public double E => e;
        public double Sqrt3 => sqrt3;
    };
    public partial class MainWindow : Window
    {
        private ComboBoxItem taylor = new ComboBoxItem();
        private ComboBoxItem euler = new ComboBoxItem();
        private NumericalAnalysis taylorData = new NumericalAnalysis();
        private NumericalAnalysis eulerData = new NumericalAnalysis();
        private ScientificConstant constantData = new ScientificConstant(3.14159265358979, 2.71828182845905, 1.73205080756888);
        public MainWindow()
        {
            InitializeComponent();
            DeltaValue.Text = "";
            XValue.Text = "";
            ResultValue.Text = "";
            ResultValue.IsReadOnly = true;
            InfoBox.Text = "";
            InfoBox.IsReadOnly = true;
            taylor.Content = "Taylor";
            euler.Content = "Euler";
            Method.Items.Add(taylor);
            Method.Items.Add(euler);
            Method.SelectedIndex = 0;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // 获取鼠标相对标题栏位置  
            Point position = e.GetPosition(this);

            // 如果鼠标位置在标题栏内，允许拖动  
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (position.X >= 0 && position.X < this.ActualWidth && position.Y >= 0 && position.Y < this.ActualHeight)
                {
                    this.DragMove();
                }
            }
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(DeltaValue.Text, out int delta))
            {
                MessageDialog messageDialog = new MessageDialog();
                messageDialog.SetInfo("Delta value error.");
                messageDialog.ShowDialog();
                DeltaValue.Text = "";
                return;
            }
            if ((delta<0)||(delta>6))
            {
                MessageDialog messageDialog = new MessageDialog();
                messageDialog.SetInfo("Delta value error.");
                messageDialog.ShowDialog();
                DeltaValue.Text = "";
                return;
            }
            if (!double.TryParse(XValue.Text, out double x))
            {
                MessageDialog messageDialog = new MessageDialog();
                messageDialog.SetInfo("x value error.");
                messageDialog.ShowDialog();
                XValue.Text = "";
                return;
            }
            if (Abs(x)>10)
            {
                MessageDialog messageDialog = new MessageDialog();
                messageDialog.SetInfo("x value error.");
                messageDialog.ShowDialog();
                XValue.Text = "";
                return;
            }
            switch (Method.Text)
            {
                case "Taylor":
                    {
                        Stopwatch TaylorSW = new Stopwatch();
                        TaylorSW.Start();
                        taylorData.AimDelta = Pow(0.1, delta);
                        taylorData.StorageAcc = Pow(0.1, (delta + 1) * 2 + 1);
                        bool flag = (x < 0);
                        double modX = (Abs(x)) % (2 * constantData.Pi);
                        if ((modX >= 0) && (modX < (constantData.Pi / 4)))
                        {
                            taylorData.X = modX;
                            TaylorProcess();
                        }
                        else if ((modX >= (constantData.Pi / 4)) && (modX < (constantData.Pi / 2)))
                        {
                            taylorData.X = (constantData.Pi / 2) - modX;
                            TaylorProcess();
                            taylorData.Result = Sqrt((1 - (taylorData.Result * taylorData.Result)), taylorData.StorageAcc);
                        }
                        else if ((modX >= (constantData.Pi / 2)) && (modX < (3 * constantData.Pi / 4)))
                        {
                            taylorData.X = modX - (constantData.Pi / 2);
                            TaylorProcess();
                            taylorData.Result = Sqrt((1 - (taylorData.Result * taylorData.Result)), taylorData.StorageAcc);
                        }
                        else if ((modX >= (3 * constantData.Pi / 4)) && (modX < (constantData.Pi)))
                        {
                            taylorData.X = constantData.Pi - modX;
                            TaylorProcess();
                        }
                        else if ((modX >= (constantData.Pi)) && (modX < (5 * constantData.Pi / 4)))
                        {
                            taylorData.X = modX - constantData.Pi;
                            TaylorProcess();
                            taylorData.Result = -taylorData.Result;
                        }
                        else if ((modX >= (5 * constantData.Pi / 4)) && (modX < (3 * constantData.Pi / 2)))
                        {
                            taylorData.X = (3 * constantData.Pi / 2) - modX;
                            TaylorProcess();
                            taylorData.Result = -Sqrt((1 - (taylorData.Result * taylorData.Result)), taylorData.StorageAcc);
                        }
                        else if ((modX >= (3 * constantData.Pi / 2)) && (modX < (7 * constantData.Pi / 4)))
                        {
                            taylorData.X = modX - (3 * constantData.Pi / 2);
                            TaylorProcess();
                            taylorData.Result = -Sqrt((1 - (taylorData.Result * taylorData.Result)), taylorData.StorageAcc);
                        }
                        else if ((modX >= (7 * constantData.Pi / 4)) && (modX < (2 * constantData.Pi)))
                        {
                            taylorData.X = (2 * constantData.Pi) - modX;
                            TaylorProcess();
                            taylorData.Result = -taylorData.Result;
                        }
                        if(flag)
                        {
                            taylorData.Result = -taylorData.Result;
                        }
                        TaylorSW.Stop();
                        taylorData.RunTime = TaylorSW.Elapsed;
                        Stopwatch SysSW = new Stopwatch();
                        SysSW.Start();
                        taylorData.RealResult = Math.Sin(x);
                        SysSW.Stop();
                        taylorData.RealDelta = Abs(taylorData.Result - taylorData.RealResult);
                        taylorData.SysRunTime = SysSW.Elapsed;
                        ResultValue.Text = taylorData.Result.ToString("G8");
                        InfoBox.Text = "";
                        InfoBox.Text += "AimDelta:\r\n" + taylorData.AimDelta.ToString("G8");
                        InfoBox.Text += "\r\n\r\nX:\r\n" + Convert.ToString(taylorData.X);
                        InfoBox.Text += "\r\n\r\nResult:\r\n" + taylorData.Result.ToString("G8");
                        InfoBox.Text += "\r\n\r\nStorageAcc:\r\n" + taylorData.StorageAcc.ToString("G8");
                        InfoBox.Text += "\r\n\r\nIterationTimes:\r\n" + Convert.ToString(taylorData.IterationTimes);
                        InfoBox.Text += "\r\n\r\nMethodDelta:\r\n" + taylorData.MethodDelta.ToString("G8");
                        InfoBox.Text += "\r\n\r\nTruncationDelta:\r\n" + taylorData.TruncationDelta.ToString("G8");
                        InfoBox.Text += "\r\n\r\nCalDelta:\r\n" + taylorData.CalDelta.ToString("G8");
                        InfoBox.Text += "\r\n\r\nTaylorRunTime(ms):\r\n" + taylorData.RunTime.TotalMilliseconds.ToString();
                        InfoBox.Text += "\r\n\r\nRealResult:\r\n" + taylorData.RealResult.ToString("G8");
                        InfoBox.Text += "\r\n\r\nRealDelta:\r\n" + taylorData.RealDelta.ToString("G8");
                        InfoBox.Text += "\r\n\r\nSysRunTime(ms):\r\n" + taylorData.SysRunTime.TotalMilliseconds.ToString();
                        break;
                    }
                case "Euler":
                    {
                        Stopwatch EulerSW = new Stopwatch();
                        EulerSW.Start();
                        eulerData.AimDelta = Pow(0.1, delta);
                        eulerData.StorageAcc = Pow(0.1, (delta + 1) * 2 + 1);
                        bool flag = (x < 0);
                        double modX = (Abs(x)) % (2 * constantData.Pi);
                        int m = (delta + 1) * 2 + 1;
                        if ((modX >= 0) && (modX < (constantData.Pi / 4)))
                        {
                            eulerData.X = modX;
                            EulerProcess(m);
                        }
                        else if ((modX >= (constantData.Pi / 4)) && (modX < (constantData.Pi / 2)))
                        {
                            eulerData.X = (constantData.Pi / 2) - modX;
                            EulerProcess(m);
                            eulerData.Result = Sqrt((1 - (eulerData.Result * eulerData.Result)), eulerData.StorageAcc);
                        }
                        else if ((modX >= (constantData.Pi / 2)) && (modX < (3 * constantData.Pi / 4)))
                        {
                            eulerData.X = modX - (constantData.Pi / 2);
                            EulerProcess(m);
                            eulerData.Result = Sqrt((1 - (eulerData.Result * eulerData.Result)), eulerData.StorageAcc);
                        }
                        else if ((modX >= (3 * constantData.Pi / 4)) && (modX < (constantData.Pi)))
                        {
                            eulerData.X = constantData.Pi - modX;
                            EulerProcess(m);
                        }
                        else if ((modX >= (constantData.Pi)) && (modX < (5 * constantData.Pi / 4)))
                        {
                            eulerData.X = modX - constantData.Pi;
                            EulerProcess(m);
                            eulerData.Result = -eulerData.Result;
                        }
                        else if ((modX >= (5 * constantData.Pi / 4)) && (modX < (3 * constantData.Pi / 2)))
                        {
                            eulerData.X = (3 * constantData.Pi / 2) - modX;
                            EulerProcess(m);
                            eulerData.Result = -Sqrt((1 - (eulerData.Result * eulerData.Result)), eulerData.StorageAcc);
                        }
                        else if ((modX >= (3 * constantData.Pi / 2)) && (modX < (7 * constantData.Pi / 4)))
                        {
                            eulerData.X = modX - (3 * constantData.Pi / 2);
                            EulerProcess(m);
                            eulerData.Result = -Sqrt((1 - (eulerData.Result * eulerData.Result)), eulerData.StorageAcc);
                        }
                        else if ((modX >= (7 * constantData.Pi / 4)) && (modX < (2 * constantData.Pi)))
                        {
                            eulerData.X = (2 * constantData.Pi) - modX;
                            EulerProcess(m);
                            eulerData.Result = -eulerData.Result;
                        }
                        if (flag)
                        {
                            eulerData.Result = -eulerData.Result;
                        }
                        EulerSW.Stop();
                        eulerData.RunTime = EulerSW.Elapsed;
                        Stopwatch SysSW = new Stopwatch();
                        SysSW.Start();
                        eulerData.RealResult = Math.Sin(x);
                        SysSW.Stop();
                        eulerData.RealDelta = Abs(eulerData.Result - eulerData.RealResult);
                        eulerData.SysRunTime = SysSW.Elapsed;
                        ResultValue.Text = eulerData.Result.ToString("G8");
                        InfoBox.Text = "";
                        InfoBox.Text += "AimDelta:\r\n" + eulerData.AimDelta.ToString("G8");
                        InfoBox.Text += "\r\n\r\nX:\r\n" + Convert.ToString(eulerData.X);
                        InfoBox.Text += "\r\n\r\nResult:\r\n" + eulerData.Result.ToString("G8");
                        InfoBox.Text += "\r\n\r\nStorageAcc:\r\n" + eulerData.StorageAcc.ToString("G8");
                        InfoBox.Text += "\r\n\r\nIterationTimes:\r\n" + Convert.ToString(eulerData.IterationTimes);
                        InfoBox.Text += "\r\n\r\nMethodDelta:\r\n" + eulerData.MethodDelta.ToString("G8");
                        InfoBox.Text += "\r\n\r\nTruncationDelta:\r\n" + eulerData.TruncationDelta.ToString("G8");
                        InfoBox.Text += "\r\n\r\nCalDelta:\r\n" + eulerData.CalDelta.ToString("G8");
                        InfoBox.Text += "\r\n\r\nEulerRunTime(ms):\r\n" + eulerData.RunTime.TotalMilliseconds.ToString();
                        InfoBox.Text += "\r\n\r\nRealResult:\r\n" + eulerData.RealResult.ToString("G8");
                        InfoBox.Text += "\r\n\r\nRealDelta:\r\n" + eulerData.RealDelta.ToString("G8");
                        InfoBox.Text += "\r\n\r\nSysRunTime(ms):\r\n" + eulerData.SysRunTime.TotalMilliseconds.ToString();
                        break;
                    }
                default:
                    {
                        MessageDialog messageDialog = new MessageDialog();
                        messageDialog.SetInfo("Method error.");
                        messageDialog.ShowDialog();
                        break;
                    }
            }
        }
        private double Factorial(int n)
        {
            if((n == 0)||(n == 1))
                return 1.0;
            else
                return (Convert.ToDouble(n) * Factorial(n - 1));
        }
        private double Pow(double x, int n)
        {
            double res = 1.0;
            for(int i=0;i<n;i++)
            {
                res *= x;
            }
            return res;
        }
        private double Abs(double x)
        {
            if(x>0)
            {
                return x;
            }
            else
            {
                return -x;
            }
        }
        private double Sqrt(double x, double delta)
        {
            if((x>1)||(x<0)||(delta<0))
            {
                MessageDialog messageDialog = new MessageDialog();
                messageDialog.SetInfo("Sqrt error.");
                messageDialog.ShowDialog();
                return -1;
            }
            else if(x==1)
            {
                return 1;
            }
            else if(x==0)
            {
                return 0;
            }
            else
            {
                double x0 = x;
                double x1 = (x0 / 2) + (x / (2 * x0));
                while (Abs(x0 - x1) > delta)
                {
                    x0 = x1;
                    x1 = (x0 / 2) + (x / (2 * x0));
                }
                return x1;
            }
        }
        private double SinTaylorItem(double x, int n)
        {
            return Pow(-1.0, n - 1) * Pow(x, 2 * n - 1) / Factorial(2 * n - 1);
        }
        private double CosTaylorItem(double x, int n)
        {
            return Pow(-1.0, n - 1) * Pow(x, 2 * (n - 1)) / Factorial(2 * (n - 1));
        }
        private double TaylorMethodError(double x, int n)
        {
            return Abs(Pow(x, 2 * n + 1) / Factorial(2 * n + 1));
        }
        private double TaylorTruncationError(double x, int m, double delta)
        {
            double error = 0.0;
            for(int i=1;i<=m;i++)
            {
                error += (delta * CosTaylorItem(x,i));
            }
            return error;
        }
        private void TaylorProcess()
        {
            double res = 0.0;
            int m = 1;
            bool flag = false;
            while(!flag)
            {
                res += SinTaylorItem(taylorData.X, m);
                if(TaylorMethodError(taylorData.X, m)<taylorData.AimDelta)
                {
                    flag = true;
                    taylorData.IterationTimes = Convert.ToUInt64(Abs(m));
                    taylorData.MethodDelta = Abs(TaylorMethodError(taylorData.X, m));
                    taylorData.TruncationDelta = Abs(TaylorTruncationError(taylorData.X, m, taylorData.StorageAcc));
                    taylorData.Result = res;
                    taylorData.CalDelta = taylorData.MethodDelta + taylorData.TruncationDelta + 0.5 * taylorData.StorageAcc;
                }
                m += 1;
            }
        }
        private double EulerF(double x,double y,double delta)
        {
            double mod = x % (2 * constantData.Pi);
            if (mod < (0.5 * constantData.Pi) || (mod > (1.5 * constantData.Pi)))
                return Sqrt(1 - (y * y), delta);
            else
                return -Sqrt(1 - (y * y), delta);
        }
        private double EulerMethodError(double x, double h)
        {
            return Abs((Pow((1 + h * 2 * constantData.Sqrt3 / 9 + 2 * h * h / 27), Convert.ToInt32(x / h)) - 1) * ((h * h) * (18 + constantData.Sqrt3) / (36 * (h + 2))));
        }
        private double EulerTruncationError(double x, double h, int m)
        {
            return Abs((Pow((1 + h * 2 * constantData.Sqrt3 / 9 + 2 * h * h / 27), Convert.ToInt32(x / h)) - 1) * ((9 * constantData.Sqrt3 * Pow(0.1, m) / (12 * h))));
        }
        private double EulerItem(double x, double y,double x_pre, double h, double delta)
        {
            double y_pre = y + h * EulerF(x, y, delta);
            y_pre = EulerCutOut(y_pre);
            double y_adj = y + h * (EulerF(x, y, delta) + EulerF(x_pre, y_pre, delta)) / 2;
            y_adj = EulerCutOut(y_adj);
            return y_adj;
        }
        private double EulerCutOut(double x)
        {
            if (x > 1.0)
            {
                return 2.0 - Abs(x);
            }
            else if (x < -1.0)
            {
                return -2.0 + Abs(x);
            }
            else
            {
                return x;
            }
        }
        private void EulerProcess(int m)
        {
            double h = eulerData.AimDelta;
            if ((((m - 1) / 2 - 1) % 2) == 1)
            {
                h = Pow(0.1, (m - 1) / 2 - 2);
            }
            else
            {
                h = Pow(0.1, (m - 1) / 2 - 1);
            }
            double delta = eulerData.StorageAcc;
            UInt64 n = Convert.ToUInt64(eulerData.X / h);
            double res = 0.0;
            for(UInt64 i=0;i<n;i++)
            {
                res = EulerItem(i * h, res, (i + 1) * h, h, delta);
            }
            eulerData.IterationTimes = n;
            eulerData.MethodDelta = Abs(EulerMethodError(eulerData.X, h));
            eulerData.TruncationDelta = Abs(EulerTruncationError(eulerData.X, h, m));
            eulerData.Result = res;
            eulerData.CalDelta = eulerData.MethodDelta + eulerData.TruncationDelta + 0.5 * eulerData.StorageAcc;
        }
    }
}

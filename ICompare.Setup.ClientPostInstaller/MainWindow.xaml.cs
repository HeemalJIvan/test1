using ICompare.Setup.PostInstallerLibrary;
using Microsoft.Data.ConnectionUI;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Configuration;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Media;
using System.Windows.Input;
using Fluid.UI.Wpf;

namespace ICompare.Tools.ConfigSetup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly string _PRIVATE_PATH = "Fluid;Lib";

        public ICommand HealthCommand { get; private set; }

        public string MasterConnectionString
        {
            get
            {
                return txtConnectionString.Text;
            }
            set
            {
                txtConnectionString.Text = value;
            }
        }

        private string SupernovaScriptLocation = "Scripts";
        public bool isDllsInSync;
        public bool areAllDllsPresent;

        public MainWindow()
        {

            //Stripped out because ID-417

//#if DEBUG
            InitializeComponent();
            DataContext = this;
            HealthCommand = new RelayCommand<object>(HealthAction);
//#else
//            if (!IsRunningAsAdministrator())
//            {
//                ProcessStartInfo processStartInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().CodeBase);
//                processStartInfo.UseShellExecute = true;
//                processStartInfo.Verb = "runas";
//                Process.Start(processStartInfo);
//                //System.Windows.Forms.Application.Exit();
//                this.Close();
//            }
//            else
//            {
//                HealthCommand = new RelayCommand<object>(HealthAction);
//                DataContext = this;
//                InitializeComponent();
//            }
//#endif
        }

        //public static bool IsRunningAsAdministrator()
        //{
        //    WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
        //    WindowsPrincipal windowsPrincipal = new WindowsPrincipal(windowsIdentity);
        //    return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        //}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string connstr = GetConnectionString();

                if (!String.IsNullOrEmpty(connstr))
                    MasterConnectionString = connstr;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        string GetConnectionString()
        {
            DataConnectionDialog dcd = new DataConnectionDialog();

            dcd.DataSources.Add(Microsoft.Data.ConnectionUI.DataSource.SqlDataSource);

            if (DataConnectionDialog.Show(dcd) == System.Windows.Forms.DialogResult.OK)
            {
                return dcd.ConnectionString;
            }

            return String.Empty;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WriteICompareFss();
                WriteFluidConfig();
                WriteDBToolConfig();
                //WriteSynapseConfigs();
                WriteTestBenchConfig();

                RunPostInstallScript();

                MessageBox.Show("Application configured successfully", "Successful");

                Application.Current.Shutdown();
            }
            catch (IOException ex)
            {
                MessageBox.Show("An error occurred while configuring your application. The application could not create the required configuration files. Please review your user rights.", ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("An error occurred while configuring your application. Please verify that the path exists, and that the database exists and is accessible\n{0}\n\n\n{1}", ex.Message, ex), ex.Message);

                //MessageBox.Show(ex.ToString(), ex.Message);
            }



        }

        private void WriteTestBenchConfig()
        {
            string testBenchconfig = Setup.ClientPostInstaller.Properties.Resources.ICompareCore_Test_TestBench_exe;
            testBenchconfig = testBenchconfig.Replace("[PRIVATEPATH]", "Lib;Lib//Fluid;Lib//icompare;Lib//fusion");
            testBenchconfig = testBenchconfig.Replace("[SCRIPTPATH]", "D:\\Test\\DataModels\\DB Tool SQL Scripts\\iCompareCore.Test.DBToolConsole");
            testBenchconfig = testBenchconfig.Replace("[ASSEMBLYPATH]", $"{Directory.GetCurrentDirectory()}\\lib\\fluid");
            testBenchconfig = testBenchconfig.Replace("[FUSIONASSEMBLYPATH]", $"{Directory.GetCurrentDirectory()}\\lib\\fusion");
            testBenchconfig = testBenchconfig.Replace("[LIBASSEMBLYPATH]", $"{Directory.GetCurrentDirectory()}\\lib");
            File.WriteAllText(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "ICompareCore.Test.TestBench.exe.config"), testBenchconfig);
        }

        private void RunPostInstallScript()
        {
            using (SqlConnection connection = new SqlConnection(MasterConnectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(Setup.ClientPostInstaller.Properties.Resources.Post_Install, connection);
                command.ExecuteNonQuery();
            }
        }

        private void WriteSynapseConfigs()
        {
            //string schedulerConsoleConfig = Properties.Resources.Fluid_Test_SchedulerConsole_exe;
            //schedulerConsoleConfig = schedulerConsoleConfig.Replace("[MASTERCONNECTIONSTRING]", MasterConnectionString);
            //schedulerConsoleConfig = schedulerConsoleConfig.Replace("[PRIVATEPATH]", _PRIVATE_PATH);
            //File.WriteAllText(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Synapse", "Fluid.Test.SchedulerConsole.exe.config"), schedulerConsoleConfig);

            //string kineticConfig = Properties.Resources.Synapse_Business_SynapseKineticAgent_exe;
            //kineticConfig = kineticConfig.Replace("[SUPERNOVACONNSTRING]", String.Format("RootPath={0};DatabaseName={1};PageSize={2}", SupernovaPath, SupernovaName, SupernovaPS)).Replace("[SUPERNOVASCRIPT]", SupernovaPath + "\\" + SupernovaScriptLocation);
            //kineticConfig = kineticConfig.Replace("[PRIVATEPATH]", _PRIVATE_PATH);

            //File.WriteAllText(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Synapse", "Synapse.Business.SynapseKineticAgent.exe.config"), kineticConfig);
        }

        public void WriteDBToolConfig()
        {
            //DBTool
            string dbToolconfig = Setup.ClientPostInstaller.Properties.Resources.ICompareCore_Test_DBToolConsole_exe;
            dbToolconfig = dbToolconfig.Replace("[PRIVATEPATH]", "Lib;Lib//Fluid;Lib//icompare;Lib//fusion");
            dbToolconfig = dbToolconfig.Replace("[SCRIPTPATH]", "D:\\Test\\DataModels\\DB Tool SQL Scripts\\iCompareCore.Test.DBToolConsole");
            dbToolconfig = dbToolconfig.Replace("[ASSEMBLYPATH]", $"{Directory.GetCurrentDirectory()}\\lib\\fluid");
            dbToolconfig = dbToolconfig.Replace("[FUSIONASSEMBLYPATH]", $"{Directory.GetCurrentDirectory()}\\lib\\fusion");
            dbToolconfig = dbToolconfig.Replace("[LIBASSEMBLYPATH]", $"{Directory.GetCurrentDirectory()}\\lib");
            File.WriteAllText(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "ICompareCore.Test.DBToolConsole.exe.config"), dbToolconfig);
        }

        public void WriteFluidConfig()
        {
            //Fluid
            //0 = Fluid Connection String, 1 = Supernova RootPath=D:\Synapse\Databases;DatabaseName=Signature;PageSize=100000, 2 = ICompareMaster, 3 = ScriptPath D:\Tests\SuperNovaX\Synapse\Scripts
            string fluidconfig = Setup.ClientPostInstaller.Properties.Resources.Fluid_UI_Shell_exe;
            //fluidconfig = fluidconfig.Replace("[SUPERNOVACONNSTRING]", String.Format("RootPath={0};DatabaseName={1};PageSize={2}", SupernovaPath, SupernovaName, SupernovaPS)).Replace("[SUPERNOVASCRIPT]", SupernovaPath + "\\" + SupernovaScriptLocation);
            fluidconfig = fluidconfig.Replace("[PRIVATEPATH]", "Lib;Lib//Fluid;Lib//icompare;Lib//fusion");
            //fluidconfig = fluidconfig.Replace("SCRIPTPATH", txtScriptpath.Text);
            fluidconfig = fluidconfig.Replace("SCRIPTPATH", String.Empty);
            File.WriteAllText(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Fluid.UI.Shell.exe.config"), fluidconfig);
        }

        public void WriteICompareFss()
        {
            //BaseSettingsManager settingsManager = new BaseSettingsManager();

            //ICompare
            //0 = Version, 1 = AppPath, 2 = ConnectionString

            string fss = Encoding.ASCII.GetString(Setup.ClientPostInstaller.Properties.Resources.settings);

            //string masterString = Encrypt(MasterConnectionString.Replace("=", "~=")); Not necessary if the string is encrypted

            string masterString = Encrypt(MasterConnectionString);

            fss = String.Format(fss, "4", Directory.GetCurrentDirectory(), masterString);

            File.WriteAllText("Settings.fss", fss);
        }

        private string Encrypt(string v)
        {
            string PasswordHash = "S!gN@tur";
            string SaltKey = "S!gnSalt";
            string VIKey = "S!gn@tur33V!K33y";


            byte[] plainTextBytes = Encoding.UTF8.GetBytes(v);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }



        private string ShowFolderBrowser()
        {
            string s = null;

            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                s = folderBrowserDialog.SelectedPath;
            }

            return s;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string s = ShowFolderBrowser();

            if (!String.IsNullOrEmpty(s))
                txtScriptpath.Text = s;
        }

        private void BtnDBTool_Click(object sender, RoutedEventArgs e)
        {
            WriteDBToolConfig();
        }

        //private void Button_Click_3(object sender, RoutedEventArgs e)
        //{


        //}

        public void HealthAction(object o)
        {
            try
            {
                HealthCheck checker = new HealthCheck();
                // Add to config for testing and set to true to view all missing assemblies <add key="ReadGAC" value="false"/>
                bool ReadGAC = ConfigurationManager.AppSettings["ReadGAC"] is null ? false : Convert.ToBoolean(ConfigurationManager.AppSettings["ReadGAC"]);
                isDllsInSync = checker.CheckDllVersions();
                areAllDllsPresent = checker.CheckDllReferences(ReadGAC);
                if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\logs"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\logs");
                }
                else
                {
                    if (File.Exists(Directory.GetCurrentDirectory() + "\\logs\\DLL_Log.txt"))
                    {
                        File.Delete(Directory.GetCurrentDirectory() + "\\logs\\DLL_Log.txt");
                    }
                }
                StringBuilder healthStatusSb = new StringBuilder();

                if (!isDllsInSync || !areAllDllsPresent)
                {
                    foreach (var v in checker.ErrorList)
                        healthStatusSb.AppendLine(v);
                }

                string healthStatus = !String.IsNullOrWhiteSpace(healthStatusSb.ToString()) ? healthStatusSb.ToString() : "All dlls present and up to date";

                using (StreamWriter sw = File.CreateText(Directory.GetCurrentDirectory() + "\\logs\\DLL_Log.txt"))
                {
                    sw.Write(healthStatus.ToString());
                }

                if (healthStatus != "All dlls present and up to date")
                {
                    btnHealthCheck.ToolTip = healthStatus;
                    rbtnStatus.IsChecked = false;
                    rbtnStatus.Background = Brushes.Red;
                    rbtnStatus.Visibility = Visibility.Visible;
                    rbtnStatus.ToolTip = "Please view the log file.";
                }
                else
                {
                    btnHealthCheck.ToolTip = "All dlls present and up to date";
                    rbtnStatus.IsChecked = false;
                    rbtnStatus.Background = Brushes.Green;
                    rbtnStatus.Visibility = Visibility.Visible;
                    rbtnStatus.ToolTip = "All dlls present and up to date.";
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error occurred on health check. {" + ex.Message + "}");
            }
        }


        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    bool DeveloperMode = ConfigurationManager.AppSettings["DeveloperMode"] is null ? false : Convert.ToBoolean(ConfigurationManager.AppSettings["DeveloperMode"]);
        //    isDllsInSync = checker.CheckDllVersions();
        //    areAllDllsPresent = checker.CheckDllReferences(DeveloperMode);
        //}
    }
}

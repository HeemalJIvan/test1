using Fluid.Business;
using Fluid.Business.Data;
using FusionXC.Data;
using Microsoft.Data.ConnectionUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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



namespace ICompare.Setup.ServicesPostInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region UI Properties

        public string MatchingEngine
        {
            get { return txtMatchingEngine.Text; }
            set
            {
                NotifyPropertyChanged("MatchingEngine");
                txtMatchingEngine.Text = value;
            }
        }

        public string ValidationEngine
        {
            get { return txtValidationEngine.Text; }
            set
            {
                NotifyPropertyChanged("ValidationEngine");
                txtValidationEngine.Text = value;
            }
        }

        public string AllocationEngine
        {
            get { return txtAllocationEngine.Text; }
            set
            {
                NotifyPropertyChanged("AllocationEngine");
                txtAllocationEngine.Text = value;
            }
        }

        private ObservableCollection<ConnectionProfile> connectionProfiles = new ObservableCollection<ConnectionProfile>();

        public IEnumerable<ConnectionProfile> ConnectionProfiles
        {
            get
            {
                return connectionProfiles;
            }
        }
        #endregion

        #region Properties
        public int ProfileCount { get; set; }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string connstr = GetConnectionString();

            if (!String.IsNullOrEmpty(connstr))
                MasterConnectionString = connstr;

            LoadConnectionProfiles();
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
                WriteConfigs();

                MessageBox.Show("Application configured successfully", "Successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message);
            }

            Application.Current.Shutdown();
        }

        private void WriteConfigs()
        {
            WriteImportConfig();

            WriteMatchingConfig();

            WriteValidationConfig();

            WriteAllocationConfig();
        }

        private void WriteValidationConfig()
        {
            string validationEngine = ((ValidationEngine is null || ValidationEngine == "") ? Guid.NewGuid().ToString("D").ToUpper() : ValidationEngine.ToString());

            #region ServiceConfig
            string config = Properties.Resources.ICompare_Business_ValidationService_exe;

            config = config.Replace("{CONNECTIONSTRING}", MasterConnectionString);
            config = config.Replace("{ENGINEID}", validationEngine.ToString());

            File.WriteAllText("ICompare.Business.ValidationService.exe.config", config);
            #endregion

            #region ConsoleConfig
            string consoleConfig = Properties.Resources.ICompare_Test_ValidationEngineConsole_exe;

            consoleConfig = consoleConfig.Replace("{CONNECTIONSTRING}", MasterConnectionString);
            consoleConfig = consoleConfig.Replace("{ENGINEID}", validationEngine.ToString());

            File.WriteAllText("ICompare.Test.ValidationEngineConsole.exe.config", consoleConfig);
            #endregion
        }

        private void WriteMatchingConfig()
        {
            string matchingEngine = ((MatchingEngine is null || MatchingEngine == "") ? Guid.NewGuid().ToString("D").ToUpper() : MatchingEngine.ToString());

            #region ServiceConfig
            string config = Properties.Resources.ICompare_Business_MatchingService_exe;

            config = config.Replace("{CONNECTIONSTRING}", MasterConnectionString);
            config = config.Replace("{ENGINEID}", matchingEngine.ToString());

            File.WriteAllText("ICompare.Business.MatchingService.exe.config", config);
            #endregion

            #region ConsoleConfig
            string consoleConfig = Properties.Resources.ICompare_Test_MatchingEngineConsole_exe;

            consoleConfig = consoleConfig.Replace("{CONNECTIONSTRING}", MasterConnectionString);
            consoleConfig = consoleConfig.Replace("{ENGINEID}", matchingEngine.ToString());

            File.WriteAllText("ICompare.Test.MatchingEngineConsole.exe.config", consoleConfig);
            #endregion
        }

        private void WriteImportConfig()
        {
            string config = Properties.Resources.ImportConfig;

            config = config.Replace("{CONNECTIONSTRING}", MasterConnectionString);

            File.WriteAllText("ICompare.Business.ImportService.exe.config", config);
        }


        private void WriteAllocationConfig()
        {
            string allocationEngine = ((AllocationEngine is null || AllocationEngine == "") ? Guid.NewGuid().ToString("D").ToUpper() : AllocationEngine.ToString());
            
            #region ServiceConfig
            string config = Properties.Resources.ICompare_Business_AllocationService_exe;

            config = config.Replace("{CONNECTIONSTRING}", MasterConnectionString);
            config = config.Replace("{ENGINEID}", allocationEngine);

            File.WriteAllText("ICompare.Business.AllocationService.exe.config", config);
            #endregion

            #region ConsoleConfig
            string consoleConfig = Properties.Resources.ICompare_Test_AllocationEngineConsole_exe;

            consoleConfig = consoleConfig.Replace("{CONNECTIONSTRING}", MasterConnectionString);
            consoleConfig = consoleConfig.Replace("{ENGINEID}", allocationEngine);

            File.WriteAllText("ICompare.Test.AllocationEngineConsole.exe.config", consoleConfig);
            #endregion
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



            UnicodeEncoding uEncode = new UnicodeEncoding();
            byte[] bytePassword = uEncode.GetBytes(string.Concat(v, "IC0mp@r3"));
            System.Security.Cryptography.SHA512Managed sha = new System.Security.Cryptography.SHA512Managed();
            byte[] hash = sha.ComputeHash(bytePassword);

            return uEncode.GetString(hash);
        }

        private void btnAdvanced_Click(object sender, RoutedEventArgs e)
        {
            if (gbAdvancedOptions.Height == btnAdvanced.Height)
            {
                gbAdvancedOptions.Height = 150;
            }
            else
            {
                gbAdvancedOptions.Height = btnAdvanced.Height;
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadConnectionProfiles()
        {
            try
            {
                FluidDataContext.Prepare<SqlDataOperations>("ServicePost", txtConnectionString.Text.ToString(), 120);
                IDataOperations ops = DataSession.GetDataOperations();
                ops.Load(connectionProfiles).Filter("ProfileName <>'Master'");
                ops.Commit();
                cbxConnectionString.ItemsSource = ConnectionProfiles;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbxConnectionString_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string SelectedValue = ((Fluid.Business.Data.ConnectionProfile)cbxConnectionString.SelectedValue).ProfileName;
            for (int i = 0; i < connectionProfiles.ToArray().Count(); i++)
            {
                string ProfileName = ConnectionProfiles.ToArray()[i].ProfileName.ToString();
                if (ProfileName == SelectedValue)
                {
                    ProfileCount = i;
                    MatchingEngine = connectionProfiles.ToArray()[i].MatchingEngineId.ToString().ToUpper();
                    ValidationEngine = connectionProfiles.ToArray()[i].ValidationEngineId.ToString().ToUpper();
                    AllocationEngine = connectionProfiles.ToArray()[i].AllocationEngineId.ToString().ToUpper();
                }
            }

        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string buttonclicked = ((System.Windows.FrameworkElement)sender).Name.ToString();
                if (ConnectionProfiles.Count() != 0)
                {
                    MessageBoxResult MBResult = MessageBoxResult.Yes;

                    #region DB Set Values 
                    string CPMatchingEnigine = connectionProfiles.ToArray()[ProfileCount].MatchingEngineId.ToString().ToUpper();
                    string CPValidationEngine = connectionProfiles.ToArray()[ProfileCount].ValidationEngineId.ToString().ToUpper();
                    string CPAllocationEngine = connectionProfiles.ToArray()[ProfileCount].AllocationEngineId.ToString().ToUpper();
                    #endregion

                    string Message = "Are you sure that you want to generate a new GUID for {0} while the current GUID is linked on {1}?";

                    if (buttonclicked == "btnGenerateME")
                    {
                        if ((MatchingEngine != "") && ((CPMatchingEnigine == MatchingEngine)))
                        {
                            MBResult = MessageBox.Show(string.Format(Message, "Matching Enigine", ((Fluid.Business.Data.ConnectionProfile)cbxConnectionString.SelectedValue).ProfileName), "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        }
                        if (MBResult == MessageBoxResult.Yes)
                        {
                            MatchingEngine = Guid.NewGuid().ToString("D").ToString().ToUpper();
                        }
                    }
                    else if (buttonclicked == "btnGenerateVE")
                    {
                        if ((ValidationEngine != "") && ((CPValidationEngine == ValidationEngine)))
                        {
                            MBResult = MessageBox.Show(string.Format(Message, "Validation Enigine", ((Fluid.Business.Data.ConnectionProfile)cbxConnectionString.SelectedValue).ProfileName), "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        }
                        if (MBResult == MessageBoxResult.Yes)
                        {
                            ValidationEngine = Guid.NewGuid().ToString("D").ToString().ToUpper();
                        }
                    }
                    else if (buttonclicked == "btnGenerateAE")
                    {
                        if ((AllocationEngine != "") && ((CPAllocationEngine == AllocationEngine)))
                        {
                            MBResult = MessageBox.Show(string.Format(Message, "Allocation Enigine", ((Fluid.Business.Data.ConnectionProfile)cbxConnectionString.SelectedValue).ProfileName), "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        }
                        if (MBResult == MessageBoxResult.Yes)
                        {
                            AllocationEngine = Guid.NewGuid().ToString("D").ToString().ToUpper();
                        }
                    }
                }
                else
                {
                    if (buttonclicked == "btnGenerateME")
                    {
                        MatchingEngine = Guid.NewGuid().ToString("D").ToString().ToUpper();
                    }
                    else if (buttonclicked == "btnGenerateVE")
                    {
                        ValidationEngine = Guid.NewGuid().ToString("D").ToString().ToUpper();
                    }
                    else if (buttonclicked == "btnGenerateAE")
                    {
                        AllocationEngine = Guid.NewGuid().ToString("D").ToString().ToUpper();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string buttonclicked = ((System.Windows.FrameworkElement)sender).Name.ToString();
                switch (buttonclicked)
                {
                    case "btnCopyME":
                        Clipboard.SetText(MatchingEngine);
                        break;
                    case "btnCopyVE":
                        Clipboard.SetText(ValidationEngine);
                        break;
                    case "btnCopyAE":
                        Clipboard.SetText(AllocationEngine);
                        break;
                    default:
                        Clipboard.SetText("");
                        break;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}

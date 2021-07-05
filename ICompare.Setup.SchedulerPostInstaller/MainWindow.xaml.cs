using Fluid.Business.DataServer;
using FusionXC.Data;
using Microsoft.Data.ConnectionUI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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

namespace Fluid.Setup.PostInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        //public string SupernovaPath
        //{
        //    get
        //    {
        //        return txtSupernovaPath.Text;
        //    }
        //    set
        //    {
        //        txtSupernovaPath.Text = value;
        //    }
        //}

        //public string SupernovaName
        //{
        //    get
        //    {
        //        return txtSupernovaName.Text;
        //    }
        //    set
        //    {
        //        txtSupernovaName.Text = value;
        //    }
        //}

        //public string SupernovaPS
        //{
        //    get
        //    {
        //        return txtSupernovaPS.Text;
        //    }
        //    set
        //    {
        //        txtSupernovaPS.Text = value;
        //    }
        //}

        public MainWindow()
        {
            InitializeComponent();

            //txtSupernovaPath.Text = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Synapse", "Supernova");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string config = ICompare.Setup.SchedulerPostInstaller.Properties.Resources.App.Replace("[MASTERCONNECTIONSTRING]", MasterConnectionString);

                File.WriteAllText("Scheduler Console/Fluid.Test.SchedulerConsole.exe.config", config);

                string service = ICompare.Setup.SchedulerPostInstaller.Properties.Resources.Fluid_Business_SchedulerService_exe.Replace("[MASTERCONNECTIONSTRING]", MasterConnectionString);
                //service = service.Replace("[PROCESSPATH]", txtSupernovaPath.Text);

                File.WriteAllText("Scheduler Service/Fluid.Business.SchedulerService.exe.config", service);

                CreateSupernovaDB();

                MessageBox.Show("Scheduler configured successfully.", "Success", MessageBoxButton.OK);

                Application.Current.Shutdown();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Scheduler Setup", MessageBoxButton.OK);
            }
        }

        private void CreateSupernovaDB()
        {
            FluidDataServer fs = new FluidDataServer();

            using (SqlConnection sql = new SqlConnection(MasterConnectionString))
            {
                SqlCommand cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT ProfileName, SuperNovaConnectionString FROM ConnectionProfile WHERE SuperNovaConnectionString IS NOT NULL AND SuperNovaConnectionString <> ''";

                sql.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    string supernovaDb = reader[1].ToString();
                    try
                    {
                        fs.CreateSuperNovaDatabase(supernovaDb);
                    }
                    catch (Exception ex)
                    {
                        ExceptionAggregator.Display(ex, "Error saving ComparableData:");

                        //todo: Display error to user and continue
                    }
                }
            }

        }

        //private void btnSupernovaPath_Click(object sender, RoutedEventArgs e)
        //{
        //    string connstr = ShowFolderBrowser();

        //    if (!String.IsNullOrEmpty(connstr))
        //        SupernovaPath = connstr;
        //}

        private void btnAdvanced_Click(object sender, RoutedEventArgs e)
        {
            //if (FluidGroupBox.Visibility == Visibility.Collapsed)
            //{
            //    FluidGroupBox.Visibility = Visibility.Visible;
            //    btnAdvanced.Content = "Hide <<";
            //}
            //else
            //{
            //    FluidGroupBox.Visibility = Visibility.Collapsed;
            //    btnAdvanced.Content = "Advanced >>";
            //}
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
    }
}

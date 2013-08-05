using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using ChangeRingtoneClass;

namespace RandomRingtone
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Properties
        PeriodicTask periodicTask = null;
        #endregion

        #region Constructor
        public MainPage()
        {
            InitializeComponent();
        }
        #endregion

        #region Load
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!WP7RootToolsSDK.Environment.HasRootAccess())
            {
                MessageBox.Show("You Don't have Root Access! Head to WP7 Root Tools and give this app Root Access.");
                PeriodicStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                if (Operations.IsAgentOn())
                {
                    StartAgent.IsEnabled = false;
                    StopAgent.IsEnabled = true;
                    PeriodicStackPanel.DataContext = periodicTask;
                }
                else
                {
                    StartAgent.IsEnabled = true;
                    StopAgent.IsEnabled = false;
                }
                Toast.IsChecked = Operations.CheckToast();
                RingTone.IsChecked = Operations.CheckRandomizeRingTone();
                SMSTone.IsChecked = Operations.CheckRandomizeSMSTone();
            }
        }
        #endregion

        #region User Actions
        private void Randomize_Click(object sender, RoutedEventArgs e)
        {
            if (Operations.CheckRandomizeRingTone())
                Operations.ChangeRingTone(true, false);
            if (Operations.CheckRandomizeSMSTone())
                Operations.ChangeSMSTone(true, false);
        }
        private void StartAgent_Click(object sender, RoutedEventArgs e)
        {
            PeriodicTask task = Operations.StartPeriodicAgent();
            if (task != null)
            {
                PeriodicStackPanel.DataContext = task;
                StartAgent.IsEnabled = false;
                StopAgent.IsEnabled = true;
            }
        }
        private void StopAgent_Click(object sender, RoutedEventArgs e)
        {
            if (Operations.RemoveAgent())
            {
                StartAgent.IsEnabled = true;
                StopAgent.IsEnabled = false;
            }
        }
        private void ToastToggle_Checked(object sender, RoutedEventArgs e)
        {
            Operations.ToastToggle(true);
        }
        private void ToastToggle_UnChecked(object sender, RoutedEventArgs e)
        {
            Operations.ToastToggle(false);
        }
        private void RingToneToggle_Checked(object sender, RoutedEventArgs e)
        {
            Operations.RandomizeRingToneToggle(true);
        }
        private void RingtoneToggle_UnChecked(object sender, RoutedEventArgs e)
        {
            Operations.RandomizeRingToneToggle(false);
        }
        private void SMSToggle_Checked(object sender, RoutedEventArgs e)
        {
            Operations.RandomizeSMSToggle(true);
        }
        private void SMSToggle_UnChecked(object sender, RoutedEventArgs e)
        {
            Operations.RandomizeSMSToggle(false);
        }
        #endregion   
    }
}
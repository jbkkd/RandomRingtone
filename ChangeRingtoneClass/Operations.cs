using System;
using System.Windows;
using WP7RootToolsSDK;
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Scheduler;

namespace ChangeRingtoneClass
{
    public class Operations
    {
        #region Properties
        public static string periodicTaskName = "RandomRingTask";
        public static PeriodicTask periodicTask;
        #endregion

        #region Agent
        /// <summary>
        /// Starts the Background Agent
        /// </summary>
        public static PeriodicTask StartPeriodicAgent()
        {
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            if (IsAgentOn())
            {
                RemoveAgent();
            }
            periodicTask = new PeriodicTask(periodicTaskName);
            periodicTask.Description = "This app will randomize your ringtone every 30 minutes.";
            try
            {
                ScheduledActionService.Add(periodicTask);
                return periodicTask;
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                }
                else
                {
                    MessageBox.Show(exception.Message);
                }
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
            }
            return null;
        }

        /// <summary>
        /// Stops the Background Agent
        /// </summary>
        public static bool RemoveAgent()
        {
            try
            {
                ScheduledActionService.Remove(periodicTaskName);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        /// <summary>
        /// Checks the current agent staus
        /// </summary>
        /// <returns>Boolean of the Agent status</returns>
        public static bool IsAgentOn()
        {
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            if (periodicTask != null)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Settings
        /// <summary>
        /// Checks if Toasts are on
        /// </summary>
        /// <returns>True if Toasts on, otherwise false</returns>
        public static bool CheckToast()
        {
            IsolatedStorageSettings ISOKey = IsolatedStorageSettings.ApplicationSettings;
            try
            {
                return (bool)ISOKey["Toast"];
            }
            catch
            {
                return false;
            }
        }
        public static bool CheckRandomizeRingTone()
        {
            IsolatedStorageSettings ISOKey = IsolatedStorageSettings.ApplicationSettings;
            try
            {
                return (bool)ISOKey["RandomizeRingTone"];
            }
            catch
            {
                return true;
            }
        }
        public static bool CheckRandomizeSMSTone()
        {
            IsolatedStorageSettings ISOKey = IsolatedStorageSettings.ApplicationSettings;
            try
            {
                return (bool)ISOKey["RandomizeSMSTone"];
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Toggles the toast to On or Off
        /// </summary>
        /// <param name="Toggle">Boolean to set the Toast status</param>
        public static void ToastToggle(bool Toggle)
        {
            IsolatedStorageSettings ISOKey = IsolatedStorageSettings.ApplicationSettings;
            ISOKey["Toast"] = Toggle;
            if (IsAgentOn())
                StartPeriodicAgent();
        }
        public static void RandomizeRingToneToggle(bool Toggle)
        {
            IsolatedStorageSettings ISOKey = IsolatedStorageSettings.ApplicationSettings;
            ISOKey["RandomizeRingTone"] = Toggle;
            if (IsAgentOn())
                StartPeriodicAgent();
        }
        public static void RandomizeSMSToggle(bool Toggle)
        {
            IsolatedStorageSettings ISOKey = IsolatedStorageSettings.ApplicationSettings;
            ISOKey["RandomizeSMSTone"] = Toggle;
            if (IsAgentOn())
                StartPeriodicAgent();
        }
        #endregion

        #region ChangeTone
        public static void ChangeRingTone(bool showAlerts, bool showToasts)
        {
            //Ringtone Registry keys - First one is current ringtone, not sure about the other ones
            RegistryKey RingTone0 = Registry.GetKey(RegistryHyve.CurrentUser, "ControlPanel\\Sounds\\RingTone0");
            RegistryKey RingTone1 = Registry.GetKey(RegistryHyve.CurrentUser, "ControlPanel\\Sounds\\RingTone1");
            RegistryKey RingTone2 = Registry.GetKey(RegistryHyve.CurrentUser, "ControlPanel\\Sounds\\RingTone2");
            RegistryKey RingTone3 = Registry.GetKey(RegistryHyve.CurrentUser, "ControlPanel\\Sounds\\RingTone3");
            
            //Gets the current ringtone
            string Sound = Registry.GetStringValue(RegistryHyve.CurrentUser, RingTone0.Path, "Sound");

            //Gets a list of all the custom ringtones
            RegistryKey CustomProtectedRingTones = Registry.GetKey(RegistryHyve.CurrentUser, "ControlPanel\\Sounds\\CustomProtectedRingTones");
            RegistryKey CustomRingTones = Registry.GetKey(RegistryHyve.CurrentUser, "ControlPanel\\Sounds\\CustomRingTones");
            List<RegistryItem> CustomProtectedRingTonesList = CustomProtectedRingTones.GetSubItems();
            int themiddle = CustomProtectedRingTonesList.Count - 1;
            CustomProtectedRingTonesList.AddRange(CustomRingTones.GetSubItems());

            //Randomizes ringtone
            Random rand = new Random();
            int therandom = rand.Next(0, CustomProtectedRingTonesList.Count - 1);
            RegistryValue randomring = (RegistryValue)CustomProtectedRingTonesList[therandom];

            //Sets the random ringtone
            if (therandom <= themiddle)
                Sound = "\\My Documents\\My Ringtones\\" + randomring.ValueName;
            else
                Sound = "\\My Documents\\Ringtones\\" + randomring.ValueName;
            Registry.SetStringValue(RegistryHyve.CurrentUser, RingTone0.Path, "Sound", Sound);
            Registry.SetStringValue(RegistryHyve.CurrentUser, RingTone1.Path, "Sound", Sound);
            Registry.SetStringValue(RegistryHyve.CurrentUser, RingTone2.Path, "Sound", Sound);
            Registry.SetStringValue(RegistryHyve.CurrentUser, RingTone3.Path, "Sound", Sound);

            if (showToasts)
            {
                ShellToast Toast = new ShellToast();
                Toast.Content = "Ringtone is now: " + randomring.Value.ToString();
                Toast.Title = "Random";
                Toast.NavigationUri = null;
                Toast.Show();
            } 
            if (showAlerts)
                MessageBox.Show("Ringtone is now: " + randomring.Value.ToString());
        }

        public static void ChangeSMSTone(bool showAlerts, bool showToasts)
        {
            string[] multi = Registry.GetMultiStringValue(RegistryHyve.CurrentUser, "ControlPanel\\Sounds", "Notifications");
            Random rand = new Random();
            int randomNumber = rand.Next(0, multi.Length - 1);
            Registry.SetStringValue(RegistryHyve.CurrentUser, "ControlPanel\\Sounds\\SMS", "Sounds", multi[randomNumber]);
            if (showToasts)
            {
                ShellToast Toast = new ShellToast();
                Toast.Content = "SMSTone is now: " + multi[randomNumber];
                Toast.Title = "Random";
                Toast.NavigationUri = null;
                Toast.Show();
            }
            if (showAlerts)
            {
                MessageBox.Show("SMSTone is now: " + multi[randomNumber]);
            }
        }
        #endregion
    }
}

﻿using System.Windows;
using Microsoft.Phone.Scheduler;
using ChangeRingtoneClass;
using System.IO.IsolatedStorage;

namespace RandomRingTask
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;
        //public bool showToasts; to be implemented
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }
        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }
        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            IsolatedStorageSettings ISOKey = IsolatedStorageSettings.ApplicationSettings;
            bool toasts = Operations.CheckToast();
            if (Operations.CheckRandomizeRingTone())
            {
                Operations.ChangeRingTone(false, toasts);
            }
            if (Operations.CheckRandomizeSMSTone())
            {
                Operations.ChangeSMSTone(false, toasts);
            }
            NotifyComplete();
        }
    }
}
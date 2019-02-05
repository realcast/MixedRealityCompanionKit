﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealcastMonitor
{
    /// <summary>
    /// The view model for the SettingsDialog object.
    /// </summary>
    public partial class SettingsDialogViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not the user
        /// wishes to automatically reconnect to the previous device session
        /// </summary>
        private bool autoReconnect = false;
        public bool AutoReconnect
        {
            get 
            {
                return this.autoReconnect;
            }

            set
            {
                if (this.autoReconnect != value)
                {
                    this.autoReconnect = value;
                    this.NotifyPropertyChanged("AutoReconnect");
                }
            }
        }

        /// <summary>
        /// Gets or sets the default network key for device connections.
        /// </summary>
        private string defaultNetworkKey = string.Empty;
        public string DefaultNetworkKey
        {
            get
            {
                return this.defaultNetworkKey;
            }

            set
            {
                if (this.defaultNetworkKey != value)
                {
                    this.defaultNetworkKey = value;
                    this.NotifyPropertyChanged("DefaultNetworkKey");
                }
            }
        }

        /// <summary>
        /// Gets or sets the default SSID for device connections.
        /// </summary>
        private string defaultSsid = string.Empty;
        public string DefaultSsid
        {
            get
            {
                return this.defaultSsid;
            }

            set
            {
                if (this.defaultSsid != value)
                {
                    this.defaultSsid = value;
                    this.NotifyPropertyChanged("DefaultSsid");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the user
        /// wishes the connection dialog to default to the credentials
        /// controls expanded.
        /// </summary>
        private bool expandCredentials = false;
        public bool ExpandCredentials
        {
            get
            {
                return this.expandCredentials;
            }

            set
            {
                if (this.expandCredentials != value)
                {
                    this.expandCredentials = value;
                    this.NotifyPropertyChanged("ExpandCredentials");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the user
        /// wishes the connection dialog to default to the network settings
        /// controls expanded.
        /// </summary>
        private bool expandNetworkSettings = false;
        public bool ExpandNetworkSettings
        {
            get
            {
                return this.expandNetworkSettings;
            }

            set
            {
                if (this.expandNetworkSettings != value)
                {
                    this.expandNetworkSettings = value;
                    this.NotifyPropertyChanged("ExpandNetworkSettings");
                }
            }
        }

        /// <summary>
        /// Gets or sets the time, in seconds, between heartbeat checks.
        /// </summary>
        private float heartbeatInterval = Settings.DefaultHeartbeatInterval;
        public string HeartbeatInterval
        {
            get 
            {
                return this.heartbeatInterval.ToString();
            }

            set 
            {
                float f = 0f;
                if (float.TryParse(value, out f))
                {
                    this.heartbeatInterval = f;
                    this.NotifyPropertyChanged("HeartbeatInterval");    
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the user
        /// wishes to use an installed certificate for connections
        /// </summary>
        private bool useInstalledCertificate = false;
        public bool UseInstalledCertificate
        {
            get
            {
                return this.useInstalledCertificate;
            }

            set
            {
                if (this.useInstalledCertificate != value)
                {
                    this.useInstalledCertificate = value;
                    this.NotifyPropertyChanged("UseInstalledCertificate");
                }
            }
        }

        private string appName = string.Empty;

        public string AppName
        {
            get
            {
                return appName;
            }

            set
            {
                if (this.appName != value)
                {
                    this.appName = value;
                    this.NotifyPropertyChanged("AppName");
                }
            }
        }
    }
}

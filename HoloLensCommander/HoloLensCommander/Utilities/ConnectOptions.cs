﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealcastMonitor
{
    /// <summary>
    /// Object encapsulating the options for connecting to a device.
    /// </summary>
    public class ConnectOptions
    {
        /// <summary>
        /// The IP address to be used for the connection.
        /// </summary>
        public string Address;

        /// <summary>
        /// Are we attempting to connect to a Desktop PC?
        /// </summary>
        public bool ConnectingToDesktopPC;

        /// <summary>
        /// Should the device be renamed at connection time?
        /// </summary>
        public bool DeployNameToDevice;

        public bool ExpandCredentials;
        public bool ExpandNetworkSettings;

        /// <summary>
        /// Optional name for the device.
        /// </summary>
        public string Name;

        /// <summary>
        /// The key used to connect to the network access point.
        /// </summary>
        public string NetworkKey;

        /// <summary>
        /// The password associated with the user account.
        /// </summary>
        public string Password;

        /// <summary>
        /// The SSID for the network access point to which the device is to be connected.
        /// </summary>
        public string Ssid;

        /// <summary>
        /// Specifies whether or not the connection is to be updated.
        /// </summary>
        public bool UpdateConnection;

        /// <summary>
        /// Specifies whether or not an installed device certificate is to be used
        /// for the connection.
        /// </summary>
        public bool UseInstalledCertificate;

        /// <summary>
        /// The name of the user.
        /// </summary>
        public string UserName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectOptions" /> class.
        /// </summary>
        public ConnectOptions() : this(
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectOptions" /> class.
        /// </summary>
        /// <param name="address">The address to be used for the connection.</param>
        /// <param name="name">Optional, local name for the device.</param>
        /// <param name="userName">The name to use when connecting to the device.</param>
        /// <param name="password">The password to use when connecting to the device.</param>
        public ConnectOptions(
            string address,
            string name,
            string userName,
            string password) : this(
            address,
            name,
            userName,
            password,
            false,
            false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectOptions" /> class.
        /// </summary>
        /// <param name="address">The address to be used for the connection.</param>
        /// <param name="name">Optional, local name for the device.</param>
        /// <param name="userName">The name to use when connecting to the device.</param>
        /// <param name="password">The password to use when connecting to the device.</param>
        /// <param name="updateConnection">Should the connection be automatically updated to the detected WiFi address?</param>
        /// <param name="useInstalledCertificate">Should an installed certificate be used to connect to the device?</param>
        public ConnectOptions(
            string address,
            string name,
            string userName,
            string password,
            bool updateConnection,
            bool useInstalledCertificate) : this(
            address,
            name,
            false,
            userName,
            password,
            false,
            string.Empty,
            string.Empty,
            updateConnection,
            useInstalledCertificate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectOptions" /> class.
        /// </summary>
        /// <param name="address">The address to be used for the connection.</param>
        /// <param name="name">Optional name for the device.</param>
        /// <param name="expandCredentials">Should the connect dialog expand the credentials controls?</param>
        /// <param name="userName">The name to use when connecting to the device.</param>
        /// <param name="password">The password to use when connecting to the device.</param>
        /// <param name="expandNetworkSettings">Should the connect dialog expand the network settings controls?</param>
        /// <param name="ssid">The SSID of the network access point to which to connect the device.</param>
        /// <param name="networkKey">The network key used when connecting to the access point.</param>
        /// <param name="updateConnection">Should the connection be automatically updated to the detected WiFi address?</param>
        /// <param name="useInstalledCertificate">Should an installed certificate be used to connect to the device?</param>
        public ConnectOptions(
            string address,
            string name,
            bool expandCredentials,
            string userName,
            string password,
            bool expandNetworkSettings,
            string ssid,
            string networkKey,
            bool updateConnection,
            bool useInstalledCertificate)
        {
            this.Address = address;
            this.Name = name;
            this.ExpandCredentials = expandCredentials;
            this.UserName = userName;
            this.Password = password;
            this.ExpandNetworkSettings = expandNetworkSettings;
            this.Ssid = ssid;
            this.NetworkKey = networkKey;
            this.UpdateConnection = updateConnection;
            this.UseInstalledCertificate = useInstalledCertificate;
        }
    }
}

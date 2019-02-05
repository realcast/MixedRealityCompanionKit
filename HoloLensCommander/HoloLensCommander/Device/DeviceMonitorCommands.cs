﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Core;
using Microsoft.Tools.WindowsDevicePortal;
using static Microsoft.Tools.WindowsDevicePortal.DevicePortal;

namespace RealcastMonitor
{
    /// <summary>
    /// Class that provides the relevant functionality of the Windows Device Portal.
    /// </summary>
    public partial class DeviceMonitor
    {
        /// <summary>
        /// Connects to a device.
        /// </summary>
        /// <param name="options">Options that specify how the connection is to be established.</param>
        /// <returns></returns>
        public async Task ConnectAsync(
            ConnectOptions options)
        {
            this.connectOptions = options;

            string address = this.connectOptions.Address.ToLower();

            if (!address.StartsWith("http"))
            {
                string scheme = "https";

                if (string.Equals(address, DefaultConnectionAddress) ||
                    string.Equals(address, DefaultConnectionAddressAsIp))
                {
                    scheme = "http";
                }

                address = string.Format(
                    "{0}://{1}",
                    scheme,
                    address);
            }

            if (this.connectOptions.ConnectingToDesktopPC)
            {
                string s = address.Substring(address.IndexOf("//"));
                if (!s.Contains(":"))
                {
                    // Append the default Windows Device Portal port for Desktop PC connections.
                    address += ":50443";
                }
            }

            this.devicePortalConnection = new DefaultDevicePortalConnection(
                    address,
                    this.connectOptions.UserName,
                    this.connectOptions.Password);
            this.devicePortal = new DevicePortal(this.devicePortalConnection);

            await this.CheckHeartbeatAsync();
        }

        /// <summary>
        /// Establish a connection to the device.
        /// </summary>
        /// <returns></returns>
        /// <returns>Task object used for tracking method completion.</returns>
        internal async Task EstablishConnection()
        {
            try
            {
                Certificate certificate = null;

                if (!this.connectOptions.UseInstalledCertificate)
                {
                    // Get the device certificate
                    certificate = await this.devicePortal.GetRootDeviceCertificateAsync(true);
                }

                // Establish the connection to the device.
                this.devicePortal.ConnectionStatus += DevicePortal_ConnectionStatus;
                Task t = this.devicePortal.ConnectAsync(
                    this.connectOptions.Ssid,
                    this.connectOptions.NetworkKey,
                    updateConnection: this.connectOptions.UpdateConnection,
                    manualCertificate: certificate);
            }
            catch
            {
                this.firstContact = false;
            }
        }

        /// <summary>
        /// Handles the ConnectionStatus event.
        /// </summary>
        /// <param name="sender">The object which sent this event.</param>
        /// <param name="args">Event arguments.</param>
        private void DevicePortal_ConnectionStatus(
            DevicePortal sender, 
            DeviceConnectionStatusEventArgs args)
        {
            if (args.Status == DeviceConnectionStatus.Connected)
            {
                // Connection successfully established.
                this.firstContact = true;
                this.devicePortal = sender;
                this.devicePortal.AppInstallStatus += DevicePortal_AppInstallStatus;
                this.devicePortal.ConnectionStatus -= DevicePortal_ConnectionStatus;

                if (this.connectOptions.DeployNameToDevice)
                {
                    Task.Run(
                        async () =>
                        {
                            if (await this.SetDeviceNameAsync(this.connectOptions.Name))
                            {
                                await this.RebootAsync();
                            }
                        });
                }
            }
            else if (args.Status == DeviceConnectionStatus.Failed)
            {
                // Connection failed.
                this.firstContact = false;
                this.devicePortal.ConnectionStatus -= DevicePortal_ConnectionStatus;

                throw new Exception(args.Message);
            }
        }

        /// <summary>
        /// Deletes a mixed reality file from the device.
        /// </summary>
        /// <param name="fileName">The name of the file to be deleted./param>
        /// <returns>Task object used for tracking method completion.</returns>
        public async Task DeleteMixedRealityFile(string fileName)
        {
            await this.devicePortal.DeleteMrcFileAsync(fileName);
        }

        /// <summary>
        /// Disconnects from the device by stopping the heartbeat.
        /// </summary>
        public void Disconnect()
        {
            // Stop the timer.
            this.heartbeatTimer.Change(
                Timeout.Infinite,
                Timeout.Infinite);
        }

        /// <summary>
        /// Find the installed application name that is associated with the specified package name.
        /// </summary>
        /// <param name="packageName">Name of the application package.</param>
        /// <returns>The application display name, or null if not found.</returns>
        private async Task<string> FindAppNameFromPackageName(string packageName)
        {
            string appName = null;

            // Get the collection of installed applications.
            AppPackages apps = await this.GetInstalledApplicationsAsync();

            // Remove the version/plaform from the package name and squash 
            string squashedPackageName = SquashPackageName(packageName);

            foreach (PackageInfo package in apps.Packages)
            {
                // Squash the name so there are no underscores or spaces
                string squashedName = SquashName(package.FamilyName);


                // Try to match with the squashed package name
                if (squashedPackageName == squashedName)
                {
                    appName = package.Name;
                    break;
                }
            }

            // Return the un-squashed name
            return appName;
        }

        /// <summary>
        /// Gets the collection of installed applications.
        /// </summary>
        /// <returns>AppPackages object describing the installed applications.</returns>
        public async Task<AppPackages> GetInstalledApplicationsAsync()
        {
            return await this.devicePortal.GetInstalledAppPackagesAsync();
        }

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        /// <returns>The device's name.</returns>
        public async Task<string> GetMachineNameAsync()
        {
            return await this.devicePortal.GetDeviceNameAsync();
        }

        /// <summary>
        /// Downloads a mixed reality file.
        /// </summary>
        /// <param name="fileName">The name of the file to download</param>
        /// <returns>Byte array containing the file data.</returns>
        public async Task<byte[]> GetMixedRealityFileAsync(string fileName)
        {
            return await this.devicePortal.GetMrcFileDataAsync(
                fileName,
                false);
        }

        /// <summary>
        /// Gets the names of the mixed reality files.
        /// </summary>
        /// <returns>MrcFileList object describing the collection of files.</returns>
        public async Task<MrcFileList> GetMixedRealityFileListAsync()
        {
            return await this.devicePortal.GetMrcFileListAsync();
        }

        /// <summary>
        /// Gets the Uri providing the live mixed reality data.
        /// </summary>
        /// <returns>Uri to the mixed reality data stream.</returns>
        public Uri GetMixedRealityViewUri()
        {
            return this.devicePortal.GetLowResolutionMrcLiveStreamUri(
                true,   // Include holograms.
                true,   // Include color camera.
                true,   // Include microphone.
                true);  // Include application audio.
        }

        /// <summary>
        /// Gets the collection of running processes.
        /// </summary>
        /// <returns>AppPackages object describing the running processes.</returns>
        public async Task<RunningProcesses> GetRunningProcessesAsync()
        {
            return await this.devicePortal.GetRunningProcessesAsync();
        }

        /// <summary>
        /// Installs an application on this device.
        /// </summary>
        /// <param name="installFiles">Object describing the file(s) required to install an application.</param>
        /// <returns>Task object used for tracking method completion.</returns>
        public async Task InstallApplicationAsync(AppInstallFiles installFiles)
        {
            await Task.Run(
                async () =>
                {
                    string appName = await this.FindAppNameFromPackageName(installFiles.AppPackageFile.Name);

                    await this.devicePortal.InstallApplicationAsync(
                        appName,
                        installFiles.AppPackageFile,
                        installFiles.AppDependencyFiles,
                        installFiles.AppCertificateFile);
                });
        }

        /// <summary>
        /// Launches an application on this device.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="packageName">The name of the application package.</param>
        /// <returns>The processes identifier of the applicaition.</returns>
        public async Task<uint> LaunchApplicationAsync(
            string appId,
            string packageName)
        {
            return await this.devicePortal.LaunchApplicationAsync(
                appId,
                packageName);
        }

        /// <summary>
        /// Reboots this device.
        /// </summary>
        /// <returns>Task object used for tracking method completion.</returns>
        public async Task RebootAsync()
        {
            await this.devicePortal.RebootAsync();

            // Force re-establishing the connection.
            // This is needed in case the device's name has been changed.
            this.firstContact = false;
        }

        /// <summary>
        /// Set the name of the device.
        /// </summary>
        /// <param name="name">The new name for the device.</param>
        /// <returns>Task object used for tracking method completion.</returns>
        /// <remarks>The name change does not go into effect until the device has been rebooted.</remarks>
        public async Task<bool> SetDeviceNameAsync(string name)
        {
            if (string.Equals(this.MachineName, name)) { return false; }

            await this.devicePortal.SetDeviceNameAsync(name);
            this.MachineName = name;

            return true;
        }

        /// <summary>
        /// Updates the interpupilary distance on the device.
        /// </summary>
        /// <param name="ipd">The value to set as the user's interpupilary distance.</param>
        /// <returns>Task object used for tracking method completion.</returns>
        public async Task SetIpd(float ipd)
        {
            await this.devicePortal.SetInterPupilaryDistanceAsync(ipd);
        }

        /// <summary>
        /// Shuts down this device.
        /// </summary>
        /// <returns>Task object used for tracking method completion.</returns>
        public async Task ShutdownAsync()
        {
            await this.devicePortal.ShutdownAsync();    
        }

        /// <summary>
        /// Returns a "compressed" (spaces and underscores removed) string.
        /// </summary>
        /// <param name="name">The name to squash.</param>
        /// <returns>The modified name.</returns>
        private string SquashName(string name)
        {
            string squashedName = name;

            squashedName = squashedName.Replace(" ", "");
            squashedName = squashedName.Replace("_", "");

            return squashedName;
        }

        /// <summary>
        /// Returns a "compressed" (spaces, underscores, version and platform info removed) version of a package name
        /// </summary>
        /// <param name="name">The package name to squash.</param>
        /// <returns>The modified package name.</returns>
        private string SquashPackageName(string name)
        {
            string squashedName = "";
            int versionIndex = -1;

            string[] nameParts = name.Split('_');
            for (int i = 0; i < nameParts.Length; i++)
            {
                Version v;
                if (Version.TryParse(nameParts[i], out v))
                {
                    versionIndex = i;
                    break;
                }

                squashedName += nameParts[i];
            }

            return squashedName;
        }

        /// <summary>
        /// Starts a mixed reality recording on this device.
        /// </summary>
        /// <returns>Task object used for tracking method completion.</returns>
        public async Task StartMixedRealityRecordingAsync()
        {
            await this.devicePortal.StartMrcRecordingAsync(
                true,       // Include holograms.
                true,       // Include color camera.
                true,       // Include microphone audio.
                true);      // Include application audio.
        }

        /// <summary>
        /// Stops the mixed reality recording on this device.
        /// </summary>
        /// <returns>Task object used for tracking method completion.</returns>
        public async Task StopMixedRealityRecordingAsync()
        {
            await this.devicePortal.StopMrcRecordingAsync();
        }

        /// <summary>
        /// Stops all running applications on this device.
        /// </summary>
        /// <returns>Task object used for tracking method completion.</returns>
        public async Task TerminateAllApplicationsAsync()
        {
            RunningProcesses runningApps = await this.GetRunningProcessesAsync();

            List<string> doNotClose = new List<string>();
            doNotClose.AddRange(DoNotCloseApps);

            foreach (DeviceProcessInfo processInfo in runningApps.Processes)
            {
                // Skip applications that should not be closed.
                if (doNotClose.Contains(
                    processInfo.Name, 
                    StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(processInfo.PackageFullName))
                {
                    await this.TerminateApplicationAsync(processInfo.PackageFullName);
                }
            }
        }

        /// <summary>
        /// Stops a single application on this device.
        /// </summary>
        /// <param name="packageName">Package name of the application to stop.</param>
        /// <returns>Task object used for tracking method completion.</returns>
        public async Task TerminateApplicationAsync(string packageName)
        {
            await this.devicePortal.TerminateApplicationAsync(packageName) ;
        }

        /// <summary>
        /// Uninstalls an application on this device.
        /// </summary>
        /// <param name="packageName">Package name of the application to uninstall.</param>
        /// <returns>Task object used for tracking method completion.</returns>
        public async Task UninstallApplicationAsync(string packageName)
        {
            await this.devicePortal.UninstallApplicationAsync(packageName);
        }

        private void DevicePortal_AppInstallStatus(
            DevicePortal sender, 
            ApplicationInstallStatusEventArgs args)
        {
            NotifyAppInstallStatus(args);
        }

        private void NotifyAppInstallStatus(
            ApplicationInstallStatusEventArgs args)
        {
            if (!this.dispatcher.HasThreadAccess)
            {
                // Assigning the return value of RunAsync to a Task object to avoid 
                // warning 4014 (call is not awaited).
                Task t = this.dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () =>
                    {
                        this.NotifyAppInstallStatus(args);
                    }).AsTask();
                return;
            }

            this.AppInstallStatus?.Invoke(
                this, 
                args);
        }
    }
}

﻿// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

#endregion

namespace MiniAVC
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class Starter : MonoBehaviour
    {
        #region Fields

        private static bool alreadyRunning;
        private FirstRunGui shownFirstRunGui;
        private IssueGui shownIssueGui;

        #endregion

        #region Initialisation

        private void Awake()
        {
            try
            {
                Logger.Log("Starter was created.");

                // Only allow one instance to run.
                if (alreadyRunning)
                {
                    Destroy(this);
                    return;
                }
                alreadyRunning = true;

                // Unload if KSP-AVC is detected.
                if (AssemblyLoader.loadedAssemblies.Any(a => a.name == "KSP-AVC"))
                {
                    Logger.Log("KSP-AVC was detected...  Unloading MiniAVC!");
                    Destroy(this);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Updating

        private void Update()
        {
            try
            {
                // Stop updating if there is already a gui being shown or the addon library has not been populated.
                if (!AddonLibrary.Populated || this.shownFirstRunGui != null || this.shownIssueGui != null)
                {
                    return;
                }

                // Do not show first start if no add-ons were found, or just destroy if all add-ons have been processed.
                if (AddonLibrary.Addons.Count == 0)
                {
                    Destroy(this);
                    return;
                }

                // Create and show first run gui if required.
                foreach (var settings in AddonLibrary.Settings.Where(s => s.FirstRun))
                {
                    this.shownFirstRunGui = this.gameObject.AddComponent<FirstRunGui>();
                    this.shownFirstRunGui.Settings = settings;
                    this.shownFirstRunGui.Addons = AddonLibrary.Addons.Where(a => a.Settings == settings).ToList();
                    return;
                }

                // Create and show issue gui if required.
                var removeAddons = new List<Addon>();
                foreach (var addon in AddonLibrary.Addons.Where(a => a.IsProcessingComplete))
                {
                    if (!addon.HasError && (addon.IsUpdateAvailable || !addon.IsCompatible))
                    {
                        this.shownIssueGui = this.gameObject.AddComponent<IssueGui>();
                        this.shownIssueGui.Addon = addon;
                        this.shownIssueGui.enabled = true;
                        removeAddons.Add(addon);
                        break;
                    }
                    removeAddons.Add(addon);
                }
                AddonLibrary.Addons.RemoveAll(removeAddons.Contains);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Destruction

        private void OnDestroy()
        {
            Logger.Log("Starter was destroyed.");
        }

        #endregion
    }
}
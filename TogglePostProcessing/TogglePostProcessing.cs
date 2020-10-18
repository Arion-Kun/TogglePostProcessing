﻿using System;
using MelonLoader;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;
using RubyButtonAPI;
using System.Diagnostics;
using UIExpansionKit.API; // If using this mark as optional melon dependency
using UnityEngine;
using UnityEngine.UI;

namespace TogglePostProcessing
{
    public static class BuildInfo
    {
        public const string Author = "arion#1223";
        public const string Company = null;
        public const string DownloadLink = "https://github.com/Arion-Kun/TogglePostProcessing/releases/";

        public const string Name = "TogglePostProcessing";

        public const string Version = "1.1.0";

    }
    public class TogglePostProcessing : MelonMod
    {
        private static bool ToggleHandler;
        private static bool QMToggle;
        private static List<OriginalVolume> OriginalVolumes;
        public struct OriginalVolume
        {
            public PostProcessVolume postProcessVolume;
            public bool defaultState;
        }
        public override void OnApplicationStart()
        {
            MelonPrefs.RegisterCategory("TogglePostProcessing", "Toggle Post Processing");
            MelonPrefs.RegisterBool("TogglePostProcessing", "DisablePostProcessing", false, "Disable Post Processing");
            //QM Stuff
            MelonPrefs.RegisterBool("TogglePostProcessing", "QMToggle", true, "QuickMenu Toggle Button");
#if TwoPointZero
            MelonPrefs.RegisterFloat("TigglePostProcessing", "NightMode1", 0, "Night Mode - 1");
            MelonPrefs.RegisterFloat("TigglePostProcessing", "NightMode2", 0, "Night Mode - 2");
            MelonPrefs.RegisterFloat("TogglePostProcessing", "NightModeCustomLevel", 0, "Night Mode - Custom: Darkness Level");
            ExpansionKitApi.RegisterSimpleMenuButton(ExpandedMenu.SettingsMenu, "Night Mode - Custom", //action);

            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomLow", false, "Bloom - Low");
            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomMedium", false, "Bloom - Medium");
            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomHigh", false, "Bloom - High");
            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomCustom", false, "Bloom - Custom");
            MelonPrefs.RegisterFloat("TogglePostProcessing", "BloomCustomLevel", 0, "Bloom - Custom: Bloom Level");
#endif
            GetPrefs();

            MelonLogger.Log("Settings can be configured in UserData/modprefs.ini or through UIExpansionKit");
            MelonLogger.Log("It is highly recommended that [UIExpansionKit] be used though.");
        }
        public readonly NightMode NightMode = new NightMode();
        public readonly Bloom Bloom = new Bloom();
        public override void OnLevelWasLoaded(int level)
        {
            try
            {
                GrabWorldVolumes();
                GetPrefs();
                ToggleMethod(ToggleHandler);
                #if TwoPointZero
                NightMode.ApplyNightMode();
                Bloom.ApplyBloom();
                #endif
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"MelonMod Error: {e}");
            }
        }
        public override void OnModSettingsApplied()
        {
            try
            {
                if (ToggleHandler != MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing") || QMToggle != MelonPrefs.GetBool("TogglePostProcessing", "QMToggle"))
                {
                    GetPrefs();
                }
                ToggleMethod(MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing"));
                #if QM
                TogglePostProcessingQM.setActive(QMToggle);
                TogglePostProcessingQM.setToggleState(!ToggleHandler);
#endif
#if TwoPointZero
                NightMode.ApplyNightMode(); 
                Bloom.ApplyBloom();
#endif
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"MelonMod Error: {e}");
            }
        }
        #if QM
        #region QuickMenu
        public static QMToggleButton TogglePostProcessingQM;
        public override void VRChat_OnUiManagerInit()
        {
            try
            {
                TogglePostProcessingQM = new QMToggleButton("ShortcutMenu", 0.23f, 1.2f, "TPP", new Action(() =>
                {
                    DisablePostProcessingBool(false);
                    if (ToggleHandler != MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing"))
                    {
                        GetPrefs();
                    }
                    ToggleMethod(ToggleHandler);
                }), "OFF", new Action(() =>
                {
                    DisablePostProcessingBool(true);
                    if (ToggleHandler != MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing"))
                    {
                        GetPrefs();
                    }
                    ToggleMethod(ToggleHandler);
                }), "Toggle Post Processing");
                Arion.SetSizeButtonfor(TogglePostProcessingQM.btnOff, 2.5f, 1.51f);
                Arion.SetSizeButtonfor(TogglePostProcessingQM.btnOn, 2.5f, 1.51f);
                Arion.SetSizeButtonfor(TogglePostProcessingQM.getGameObject(), 1.9f, 1.38f);
                TogglePostProcessingQM.setToggleState(!MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing"));
                TogglePostProcessingQM.setActive(QMToggle);
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"MelonMod Error: {e}");
            }
        }
        #endregion
        #endif
        #region Toggle
        public void GetPrefs()
        {
            try
            {
                ToggleHandler = MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing");
                QMToggle = MelonPrefs.GetBool("TogglePostProcessing", "QMToggle");
                #if TwoPointZero
                NightMode.NightMode1Bool = MelonPrefs.GetBool("TogglePostProcessing", "NightMode1");
                NightMode.NightMode2Bool = MelonPrefs.GetBool("TogglePostProcessing", "NightMode2");
                NightMode.NightMode3Bool = MelonPrefs.GetBool("TogglePostProcessing", "NightModeCustom");
                NightMode.NightMode3Float = MelonPrefs.GetFloat("TogglePostProcessing", "NightModeCustomLevel");

                Bloom.Bloom1Bool = MelonPrefs.GetBool("TogglePostProcessing", "BloomLow");
                Bloom.Bloom2Bool = MelonPrefs.GetBool("TogglePostProcessing", "BloomMedium");
                Bloom.Bloom3Bool = MelonPrefs.GetBool("TogglePostProcessing", "BloomHigh");
                Bloom.Bloom4Bool = MelonPrefs.GetBool("TogglePostProcessing", "BloomCustom");
                Bloom.Bloom4Float = MelonPrefs.GetFloat("TogglePostProcessing", "BloomCustomLevel");
                #endif
            }
            catch (Exception e)
            { 
                MelonLogger.LogError($"GetPrefs Error: {e}"); 
            }

        }
        private static void GrabWorldVolumes() //Credits to Psychloor for Method
        {
            try
            {
                OriginalVolumes = new List<OriginalVolume>();
                foreach (var volume in Resources.FindObjectsOfTypeAll<PostProcessVolume>())
                {
                    OriginalVolumes.Add(new OriginalVolume() { postProcessVolume = volume, defaultState = volume.enabled });
                }
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"GrabWorldVolumes Error: {e}"); 
            }

        }
        private static void Reset() //Credits to Psychloor for Method
        {
            try
            {
                foreach (OriginalVolume originalVolume in OriginalVolumes)
                {
                    if (originalVolume.postProcessVolume)
                    {
                        originalVolume.postProcessVolume.enabled = originalVolume.defaultState;
                    }
                }
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"Reset Error: {e}"); 
            }
        }
        private static void ToggleMethod(bool disable)
        {
            try
            {
                if (disable)
                {
                    if (OriginalVolumes != null)
                    {
                        foreach (OriginalVolume originalVolume in OriginalVolumes)
                        {
                            if (originalVolume.postProcessVolume)
                            {
                                originalVolume.postProcessVolume.enabled = false;
                            }
                        }
                    }
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception e)
            { 
                MelonLogger.LogError($"ToggleMethod Error: {e}");
            }
        }
        private static void DisablePostProcessingBool(bool value)
        {
            MelonPrefs.SetBool("TogglePostProcessing", "DisablePostProcessing", value);
        }
#endregion
    }
}
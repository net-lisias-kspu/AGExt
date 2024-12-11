/*
	This file is part of Action Groups Extended (AGExt) /L Unleashed
		© 2018-2024 Lisias T : http://lisias.net <support@lisias.net>
		© 2017-2018 LinuxGuruGamer
		© 2014-2016 Sir Diazo

	Action Groups Extended (AGExt) /L Unleashed is licensed as follows:

		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Action Groups Extended (AGExt) /L Unleashed is distributed in the hope that
	it will be useful, but WITHOUT ANY WARRANTY; without even the implied
	warranty of	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Action Groups Extended (AGExt) /L Unleashed.
	If not, see <https://www.gnu.org/licenses/>.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using KSP.UI.Screens;
using KSP.Localization;
using UnityEngine;

using GUILayout = KSPe.UI.GUILayout;
using GUI = KSPe.UI.GUI;
using Toolbar = KSPe.UI.Toolbar;

namespace ActionGroupsExtended
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AGXFlight : PartModule
    {
        public bool showMyUI = true;
        public static AGXFlight thisModule;
        private bool showDockedSubVesselIndicators = false;
        public static Dictionary<int, bool> isDirectAction = new Dictionary<int, bool>();
        bool showCareerStockAGs = false;
        bool showCareerCustomAGs = false;
        bool showAGXRightClickMenu = false;
        private static int activationCoolDown = 5;
        private static List<AGXCooldown> groupCooldowns;

        //Selected Parts Window Variables
        private bool AGXLockSet = false; //is inputlock set?
        private string LastKeyCode = "";
        public static Dictionary<int, bool> IsGroupToggle; //is group a toggle group?
        public static bool[,] ShowGroupInFlight; //Show group in flight?
        public static int ShowGroupInFlightCurrent;
        public static string[] ShowGroupInFlightNames;
        public bool ShowGroupsInFlightWindow = false;
        public static Rect GroupsInFlightWin;
        public static Rect SelPartsWin;
        public static Rect FlightWin;
        private Vector2 ScrollPosSelParts;
        public Vector2 ScrollPosSelPartsActs;
        public Vector2 ScrollGroups;
        public Vector2 CurGroupsWin;
        public Vector2 FlightWinScroll;
        public Vector2 RTWinScroll;
        private List<AGXPart> AGEditorSelectedParts;
        private bool AGEEditorSelectedPartsSame = false;

        private int SelectedPartsCount = 0;
        private static bool ShowSelectedWin = false;
        private bool SelPartsIncSym = true;
        private string BtnTxt;
        private Part AGXRoot;

        private int GroupsPage = 1;
        private string CurGroupDesc;
        static private bool AutoHideGroupsWin = false;
        private bool TempShowGroupsWin = false;
        private static Rect KeyCodeWin;
        private static Rect CurActsWin;
        private static Rect RemoteTechQueueWin;
        private bool ShowKeyCodeWin = false;
        private bool ShowJoySticks = false;
        private static bool ShowKeySetWin = false;
        private static int CurrentKeySetFlight = 1;
        private static string CurrentKeySetNameFlight;
        private static Rect KeySetWin;
        public static ConfigNode AGExtNode;
        static string[] KeySetNamesFlight = new string[5];
        private int LastPartCount = 0;

        private List<KeyCode> ActiveKeys;
        private Dictionary<int, KeyCode> DefaultTen;
        private Dictionary<int, KeyCode> ActiveKeysDirect;
        private Dictionary<int, bool> DirectKeysState;
        public static List<AGXRemoteTechQueueItem> AGXRemoteTechQueue;
        public static bool RTFound = false;
        private string RTGroup = "1";
        private static bool RTWinShow = false;

        public static Rect GroupsWin;
        public bool Trigger;

        private List<BaseAction> PartActionsList;

        public static Dictionary<int, string> AGXguiNames;
        public static Dictionary<int, KeyCode> AGXguiKeys;
        public static Dictionary<int, bool> AGXguiMod1Groups;
        public static Dictionary<int, bool> AGXguiMod2Groups;
        public static KeyCode AGXguiMod1Key = KeyCode.None;
        public static KeyCode AGXguiMod2Key = KeyCode.None;
        private bool AGXguiMod1KeySelecting = false;
        private bool AGXguiMod2KeySelecting = false;
        public int AGXCurActGroup = 1;
        private readonly List<string> KeyCodeNames = new List<string>();
        private readonly List<string> JoyStickCodes = new List<string>();
        private bool ShowCurActsWin = true;
        private static bool ShowAGXMod = true;
        private bool ShowAGXFlightWin = true;
        public static bool loadFinished = false;
        private int RightClickDelay = 0;
        private bool RightLickPartAdded = false;
        public static List<AGXActionsState> ActiveActionsState = new List<AGXActionsState>();
        public static List<AGXActionsState> ActiveActionsStateToShow = new List<AGXActionsState>();
        private int actionsCheckFrameCount = 0;

        public static bool FlightWinShowKeycodes = true;
        static ConfigNode AGXBaseNode = new ConfigNode();
        public static ConfigNode AGXFlightNode = new ConfigNode();
        static ConfigNode AGXEditorNodeFlight = new ConfigNode();
        bool highlightPartThisFrameSelWin = false;
        bool highlightPartThisFrameActsWin = false;
        Part partToHighlight = null;
        bool showAllPartsList = false; //show list of all parts in group window?
        List<string> showAllPartsListTitles; //list of all parts with actions to show in group window

        bool defaultShowingNonNumeric = false; //are we in non-numeric (abort/brakes/gear/list) mode?
        private KSPActionGroup defaultGroupToShow = KSPActionGroup.Abort; //which group is currently selected if showing non-numeric groups
        private readonly List<BaseAction> defaultActionsListThisType; //list of default actions showing in group win when non-numeric
        private readonly List<BaseAction> defaultActionsListAll; //list of all default actions on vessel, only used in non-numeric mode when going to other mode
        private Vector2 groupWinScroll = new Vector2();
        private bool highlightPartThisFrameGroupWin = false;
        private List<AGXAction> ThisGroupActions;
        private bool showGroupsIsGroups = true;
        public static bool useRT = true;
        public static Dictionary<int, bool> groupActivatedState; //group activated state, this does NOT save, provided for kOS script usage
        private static uint currentMissionId;

		public AGXFlight()
		{
			KeyCodeNames = new List<String>();
			KeyCodeNames.AddRange(Enum.GetNames(typeof(KeyCode)));
			KeyCodeNames.Remove("None");
			JoyStickCodes.AddRange(KeyCodeNames.Where(JoySticks));
			KeyCodeNames.RemoveAll(JoySticks);
		}

        public void onMyUIShow()
        {
            showMyUI = true;
        }

        public void onMyUIHide()
        {
            showMyUI = false;
        }

        IEnumerator DockedSubVesselsIconTimer()
        {
            for (int i = 1; i < 30; i++)
            {
                yield return null;
            }
            showDockedSubVesselIndicators = false;
        }

        private void OnRightButtonClick()
        {
            Log.detail("rgt btn click {0}", Time.realtimeSinceStartup);
            showAGXRightClickMenu = !showAGXRightClickMenu;
        }

        private void OnLeftButtonClick()
        {
            Log.detail("lft btn click {0}", Time.realtimeSinceStartup);
            if (showCareerStockAGs)
            {
                ShowAGXMod = !ShowAGXMod;
            }
            else
            {
                // ScreenMessages.PostScreenMessage("Action Groups Unavailable. VAB/SPH Facility Upgrade Required.");
                ScreenMessages.PostScreenMessage(Localizer.Format("#AGEXT_UI_SCREEN_MESSAGE_6"));
            }
        }

        public void RefreshDefaultActionsListType()
        {
            defaultActionsListThisType.Clear();
            foreach (BaseAction act in defaultActionsListAll)
            {
                if ((act.actionGroup & defaultGroupToShow) == defaultGroupToShow)
                {
                    defaultActionsListThisType.Add(act);
                }
            }
        }

        public void Start()
        {
            string errLine = "1";
            try
            {
                thisModule = this;
                useRT = true;
                AGXguiMod1Groups = new Dictionary<int, bool>();
                AGXguiMod2Groups = new Dictionary<int, bool>();
                for (int i = 1; i <= 250; i++)
                {
                    AGXguiMod1Groups[i] = false;
                    AGXguiMod2Groups[i] = false;
                }
                errLine = "2";
                defaultActionsListThisType.Clear();
                defaultActionsListAll.Clear();
                ThisGroupActions = new List<AGXAction>();
                ActiveKeys = new List<KeyCode>();
                ActiveKeysDirect = new Dictionary<int, KeyCode>();
                DefaultTen = new Dictionary<int, KeyCode>();
                DirectKeysState = new Dictionary<int, bool>();
                for (int i = 1; i <= 250; i++)
                {
                    DirectKeysState[i] = false;
                }

                AGEditorSelectedParts = new List<AGXPart>();
                PartActionsList = new List<BaseAction>();
                ScrollPosSelParts = Vector2.zero;
                ScrollPosSelPartsActs = Vector2.zero;
                ScrollGroups = Vector2.zero;
                CurGroupsWin = Vector2.zero;

                errLine = "3";
                AGXguiNames = new Dictionary<int, string>();
                AGXguiKeys = new Dictionary<int, KeyCode>();
                groupActivatedState = new Dictionary<int, bool>();

                errLine = "4";
                for (int i = 1; i <= 250; i = i + 1)
                {
                    AGXguiNames[i] = "";
                    AGXguiKeys[i] = KeyCode.None;
                    groupActivatedState[i] = false;
                }

                errLine = "5";
                try
                {
                    AGExtNode = AGXStaticData.LoadBaseConfigNode();
                }
                catch
                {
                    Log.err("Flight Start CRITICAL FAIL: AGExt.settings not found. Check install path.");
                }
                if (AGExtNode == null)
                {
                    Log.err("Flight Start CRITICAL FAIL: AGExt.settings not loaded. Check install path.");
                }
                errLine = "6";
                CurrentKeySetFlight = 1;
                CurrentKeySetNameFlight = (string)AGExtNode.GetValue("KeySetName1");
                KeySetNamesFlight[0] = (string)AGExtNode.GetValue("KeySetName1");
                KeySetNamesFlight[1] = (string)AGExtNode.GetValue("KeySetName2");
                KeySetNamesFlight[2] = (string)AGExtNode.GetValue("KeySetName3");
                KeySetNamesFlight[3] = (string)AGExtNode.GetValue("KeySetName4");
                KeySetNamesFlight[4] = (string)AGExtNode.GetValue("KeySetName5");
                AGXRoot = null;
                errLine = "7";
                StartLoadWindowPositions();
                errLine = "8";
                if (AGExtNode.GetValue("FlightWinShowKeys") == "1")
                {
                    FlightWinShowKeycodes = true;
                }
                else
                {
                    FlightWinShowKeycodes = false;
                }
                errLine = "9";

                //Log.dbg("input lock");
                try
                {
                    if ((string)AGExtNode.GetValue("LockOutKSPManager") == "0")
                    {
                        AGXLockSet = false;
                        //Log.dbg("zero found");
                    }
                    else
                    {
                        InputLockManager.SetControlLock(ControlTypes.CUSTOM_ACTION_GROUPS, "AGExtControlLock");
                        AGXLockSet = true;
                        //Log.dbg("one found");
                    }
                    errLine = "10";
                }
                catch
                {
                    errLine = "11";
                    InputLockManager.SetControlLock(ControlTypes.CUSTOM_ACTION_GROUPS, "AGExtControlLock");
                    AGXLockSet = true;
                    //Log.dbg("Catch");
                }
                errLine = "12";
                groupCooldowns = new List<AGXCooldown>(); //setup values for group cooldowns logic
                try
                {
                    activationCoolDown = Convert.ToInt32(AGExtNode.GetValue("ActivationCooldown"));
                }
                catch
                {
                    Log.err("Default cooldown not found, using 5 Update frames");
                }
                errLine = "13";
                AddButtons();
                errLine = "16";
                if (AGExtNode.GetValue("FltShow") == "0")
                {
                    ShowAGXMod = false;
                }
                errLine = "17";
                float facilityLevelSPH = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar);
                float facilityLevelVAB = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding);
                float facilityLevel;
                bool VABmax = true;
                if (facilityLevelSPH > facilityLevelVAB)
                {
                    facilityLevel = facilityLevelSPH;
                    VABmax = false;
                }
                else
                {
                    facilityLevel = facilityLevelVAB;
                    VABmax = true;
                }
                //Log.dbg("AGX Career check: " + facilityLevel);
                if (AGExtNode.HasValue("OverrideCareer")) //are action groups unlocked?
                {
                    //Log.dbg("b");
                    errLine = "18";
                    if ((string)AGExtNode.GetValue("OverrideCareer") == "1" || HighLogic.CurrentGame.Parameters.CustomParams<AGExt>().OverrideCareer )
                    {
                        //Log.dbg("c");
                        showCareerCustomAGs = true;
                        showCareerStockAGs = true;
                    }
                    else
                    {
                        //Log.dbg("d");
                        if (GameVariables.Instance.UnlockedActionGroupsCustom(facilityLevel, VABmax))
                        {
                            // print("g");
                            showCareerStockAGs = true;
                            showCareerCustomAGs = true;
                        }
                        else if (GameVariables.Instance.UnlockedActionGroupsStock(facilityLevel, VABmax))
                        {
                            // print("h");
                            showCareerStockAGs = true;
                            showCareerCustomAGs = false;
                        }
                        else
                        {
                            //Log.dbg("i");
                            showCareerStockAGs = false;
                            showCareerCustomAGs = false;
                        }
                    }
                }
                else
                {
                    //Log.dbg("j");
                    errLine = "19";

                    if (GameVariables.Instance.UnlockedActionGroupsCustom(facilityLevel, VABmax))
                    {
                        // print("m");
                        showCareerStockAGs = true;
                        showCareerCustomAGs = true;
                    }
                    else if (GameVariables.Instance.UnlockedActionGroupsStock(facilityLevel, VABmax))
                    {
                        // print("n");
                        showCareerStockAGs = true;
                        showCareerCustomAGs = false;
                    }
                    else
                    {
                        // print("o");
                        showCareerStockAGs = false;
                        showCareerCustomAGs = false;
                    }
                }
                // print("startd " + showCareerCustomAGs);
                errLine = "20";
                IsGroupToggle = new Dictionary<int, bool>();
                ShowGroupInFlight = new bool[6, 251];
                ShowGroupInFlightNames = new string[6];
                ShowGroupInFlightNames[1] = "Group 1";
                ShowGroupInFlightNames[2] = "Group 2";
                ShowGroupInFlightNames[3] = "Group 3";
                ShowGroupInFlightNames[4] = "Group 4";
                ShowGroupInFlightNames[5] = "Group 5";

                errLine = "21";
                for (int i = 1; i <= 250; i++)
                {
                    IsGroupToggle[i] = false;
                    for (int i2 = 1; i2 <= 5; i2++)
                    {
                        ShowGroupInFlight[i2, i] = true;
                    }
                }
                ShowGroupInFlightCurrent = 1;
                // print("AGXStart " + Planetarium.GetUniversalTime());

                errLine = "25";
                GameEvents.onPartDie.Add(partDead); //remove part from RootParts if possible

                AGXFlightNode = new ConfigNode();
                errLine = "26";

				if (KSPe.IO.SaveGameMonitor.Instance.IsValid)
				{
					string filename = KSPe.IO.Hierarchy.SAVE.Solve(HighLogic.SaveFolder, "AGExtEditor.cfg");
					if (System.IO.File.Exists(filename))
					{
						errLine = "27";
						AGXEditorNodeFlight = ConfigNode.Load(filename);
					}
					else
					{
						errLine = "28";
						AGXEditorNodeFlight = new ConfigNode("EDITOR");
						AGXEditorNodeFlight.AddValue("name", "editor");
					}
				}
				else
					AGXEditorNodeFlight = new ConfigNode("EDITOR");

                errLine = "29";
                LoadCurrentKeyBindings();
                errLine = "30";
                AGXRemoteTechQueue = new List<AGXRemoteTechQueueItem>();
                RTFound = false;
                foreach (AssemblyLoader.LoadedAssembly Asm in AssemblyLoader.loadedAssemblies)
                {
                    if (Asm.dllName == "RemoteTech")
                    {
                        Log.detail("RemoteTech found");
                        RTFound = true;
                    }
                }
                //Log.dbg("RT FOUND " + RTFound); 
                errLine = "31";
                if (RTFound && bool.Parse(AGExtNode.GetValue("RTWinShow")))
                {
                    RTWinShow = true;
                }
                else
                {
                    RTWinShow = false;
                }
                Log.trace("RemoteTech {0}", RTWinShow);
                isDirectAction = new Dictionary<int, bool>();
                for (int i = 1; i <= 250; i++)
                {
                    isDirectAction[i] = false;
                }

                // Log.Info("Flight Started Okay"); //temporary
            }
            catch (Exception e)
            {
                Log.err("Flight Start FAIL {0}", errLine);
                Log.err("AGExt node dump: {0}", AGExtNode);
                Log.ex(this, e);
            }
            GameEvents.onShowUI.Add(onMyUIShow);
            GameEvents.onHideUI.Add(onMyUIHide);
        }

        void OnDestroy()
        {
            GameEvents.onPartDie.Remove(partDead); //remove part from RootParts if possible
        }

        void AddButtons()
        {
            Log.trace("AddButton");
            Toolbar.Button button = Toolbar.Button.Create(this
                    , ApplicationLauncher.AppScenes.FLIGHT
                    , UI.icon_button_38
                    , UI.icon_button_24
                );
            button.Mouse
                .Add(Toolbar.Button.MouseEvents.Kind.Left, OnLeftButtonClick)
                .Add(Toolbar.Button.MouseEvents.Kind.Right, OnRightButtonClick)
            ;
            ToolbarController.Instance.Add(button);
        }

        public static void LoadDirectActionState(string DirectActions)
        {
            Log.trace("load state {0}", DirectActions);
            try
            {
                isDirectAction = new Dictionary<int, bool>();
                if (DirectActions.Length == 250)
                {
                    for (int i = 1; i <= 250; i++)
                    {
                        if (DirectActions[0] == '1')
                        {
                            isDirectAction[i] = true;
                        }
                        else
                        {
                            isDirectAction[i] = false;
                        }
                        DirectActions = DirectActions.Substring(1);
                    }
                }
                else
                {
                    for (int i = 1; i <= 250; i++)
                    {
                        isDirectAction[i] = false;
                    }
                }
            }
            catch (Exception e)
            {
                Log.err("LoadDirectActions Fail {0}", e);
                for (int i = 1; i <= 251; i++)
                {
                    isDirectAction[i] = false;
                }
            }
        }

        public static string SaveDirectActionState(string str)
        {
            try
            {
                string ReturnStr = "";

                for (int i = 1; i <= 250; i++)
                {
                    if (isDirectAction[i])
                    {
                        ReturnStr = ReturnStr + "1";
                    }
                    else
                    {
                        ReturnStr = ReturnStr + "0";
                    }
                }
                return ReturnStr;
            }
            catch
            {
                return str;
            }
        }

        public void StartLoadWindowPositions()
        {
            string errLine = "1";
            try
            {
                errLine = "2";
                int WinX;
                int WinY;

                if (Int32.TryParse((string)AGExtNode.GetValue("FltGroupsX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("FltGroupsY"), out WinY))
                {
                    GroupsWin = new Rect(WinX, WinY, 250, 530);
                }
                else
                {
                    GroupsWin = new Rect(100, 100, 250, 530);
                }
                errLine = "3";
                if (Int32.TryParse((string)AGExtNode.GetValue("FltSelPartsX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("FltSelPartsY"), out WinY))
                {
                    SelPartsWin = new Rect(WinX, WinY, 365, 270);
                }
                else
                {
                    SelPartsWin = new Rect(100, 100, 365, 270);
                }
                errLine = "4";
                if (Int32.TryParse((string)AGExtNode.GetValue("FltKeyCodeX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("FltKeyCodeY"), out WinY))
                {
                    KeyCodeWin = new Rect(WinX, WinY, 410, 730);
                }
                else
                {
                    KeyCodeWin = new Rect(100, 100, 410, 730);
                }
                errLine = "5";
                if (Int32.TryParse((string)AGExtNode.GetValue("FltKeySetX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("FltKeySetY"), out WinY))
                {
                    KeySetWin = new Rect(WinX, WinY, 185, 335);
                }
                else
                {
                    KeySetWin = new Rect(100, 100, 185, 335);
                }
                errLine = "6";
                if (Int32.TryParse((string)AGExtNode.GetValue("FltCurActsX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("FltCurActsY"), out WinY))
                {
                    CurActsWin = new Rect(WinX, WinY, 345, 140);
                }
                else
                {
                    CurActsWin = new Rect(100, 100, 345, 140);
                }
                errLine = "7";
                if (Int32.TryParse((string)AGExtNode.GetValue("FltMainX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("FltMainY"), out WinY))
                {
                    FlightWin = new Rect(WinX, WinY, 235, 100);
                    GroupsInFlightWin = new Rect(WinX + 235, WinY, 80, 110);
                }
                else
                {
                    FlightWin = new Rect(100, 100, 235, 100);
                    GroupsInFlightWin = new Rect(335, 100, 80, 110);
                }
                errLine = "8";
                errLine = "9";
                if (Int32.TryParse((string)AGExtNode.GetValue("RTWinX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("RTWinY"), out WinY))
                {
                    RemoteTechQueueWin = new Rect(WinX, WinY, 350, 125);
                }
                else
                {
                    RemoteTechQueueWin = new Rect(100, 100, 350, 125);
                }
                errLine = "10";
            }
            catch (Exception e)
            {
                Log.err("StartLoadWindowPostion Error, Recovered. {0}", errLine);
                Log.ex(this, e);
                GroupsWin = new Rect(100, 100, 250, 530);
                SelPartsWin = new Rect(100, 100, 365, 270);
                KeyCodeWin = new Rect(100, 100, 410, 730);
                KeySetWin = new Rect(100, 100, 185, 335);
                CurActsWin = new Rect(100, 100, 345, 140);
                FlightWin = new Rect(100, 100, 235, 100);
                GroupsInFlightWin = new Rect(335, 100, 80, 110);
                RemoteTechQueueWin = new Rect(100, 100, 350, 125);
            }
        }

#if false
        public class SettingsWindow : MonoBehaviour, IDrawable
        {
            public Rect SettingsWin = new Rect(0, 0, 150, 180);
            public Vector2 Draw(Vector2 position)
            {
                var oldSkin = GUI.skin;
                GUI.skin = HighLogic.Skin;

                SettingsWin.x = position.x;
                SettingsWin.y = position.y;

                // GUI.Window(2233452, SettingsWin, DrawSettingsWin, "AGX Settings", AGXWinStyle);
                GUI.Window(2233452, SettingsWin, DrawSettingsWin, Localizer.Format("#AGEXT_UI_agx_settings"), AGXWinStyle);
                //RCSlaWin = GUILayout.Window(42334567, RCSlaWin, DrawWin, (string)null, GUI.skin.box);
                //GUI.skin = oldSkin;

                return new Vector2(SettingsWin.width, SettingsWin.height);
            }

            public void DrawSettingsWin(int WindowID)
            {

                // if (GUI.Button(new Rect(10, 25, 130, 25), "Show KeyCodes", AGXBtnStyle))
                if (GUI.Button(new Rect(10, 25, 130, 25), Localizer.Format("#AGEXT_UI_setting_show_key_codes"), AGXBtnStyle))
                {
                    AGXFlight.FlightWinShowKeycodes = !AGXFlight.FlightWinShowKeycodes;
                    if (AGXFlight.FlightWinShowKeycodes)
                    {
                        AGXFlight.AGExtNode.SetValue("FlightWinShowKeys", "1");
                    }
                    else
                    {
                        AGXFlight.AGExtNode.SetValue("FlightWinShowKeys", "0");
                    }
                    //AGXFlight.AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
                    AGXStaticData.SaveBaseConfigNode(AGExtNode);
                }

                // if (GUI.Button(new Rect(10, 50, 130, 25), "Edit Actions", AGXBtnStyle))
                if (GUI.Button(new Rect(10, 50, 130, 25), Localizer.Format("#AGEXT_UI_setting_edit_actions"), AGXBtnStyle))
                {
                    AGXFlight.ClickEditButton();

                }
                // if (GUI.Button(new Rect(10, 75, 130, 25), "Reset Windows", AGXBtnStyle))
                if (GUI.Button(new Rect(10, 75, 130, 25), Localizer.Format("#AGEXT_UI_setting_reset_windows"), AGXBtnStyle))
                {
                    KeySetWin.x = 250;
                    KeySetWin.y = 250;
                    GroupsWin.x = 350;
                    GroupsWin.y = 350;
                    SelPartsWin.x = 200;
                    SelPartsWin.y = 200;
                    KeyCodeWin.x = 300;
                    KeyCodeWin.y = 300;
                    CurActsWin.x = 150;
                    CurActsWin.y = 150;
                    FlightWin.x = 400;
                    FlightWin.y = 400;
                }
                AGXBtnStyle.normal.background = AutoHideGroupsWin ? ButtonTextureRed : ButtonTexture;
                AGXBtnStyle.hover.background = AutoHideGroupsWin ? ButtonTextureRed : ButtonTexture;
                // if (GUI.Button(new Rect(10, 100, 130, 25), "Auto-Hide Groups", AGXBtnStyle))
                if (GUI.Button(new Rect(10, 100, 130, 25), Localizer.Format("#AGEXT_UI_setting_auto_hide_groups"), AGXBtnStyle))
                {
                    AutoHideGroupsWin = !AutoHideGroupsWin;
                }
                AGXBtnStyle.normal.background = ButtonTexture;
                AGXBtnStyle.hover.background = ButtonTexture;
                // if (GUI.Button(new Rect(10, 125, 130, 25), "Show RemoteTech", AGXBtnStyle))
                if (GUI.Button(new Rect(10, 125, 130, 25), Localizer.Format("#AGEXT_UI_setting_show_remote_tech"), AGXBtnStyle))
                {
                    if (RTFound)
                    {
                        RTWinShow = !RTWinShow;
                    }
                }
                // if (GUI.Button(new Rect(10, 150, 130, 25), "Bypass RemoteTech", AGXBtnStyle))
                if (GUI.Button(new Rect(10, 150, 130, 25), Localizer.Format("#AGEXT_UI_setting_bypass_remote_tech"), AGXBtnStyle))
                {
                    useRT = !useRT;
                }
                AGXBtnStyle.normal.background = ButtonTexture;
                AGXBtnStyle.hover.background = ButtonTexture;

                //GUI.DragWindow();

            }
            public void Update()
            {

            }
        }
#endif
        public bool IsVesselLoaded(uint flightID) //is vessel loaded, wrapper to convert from flightID to vsl
        {
            try
            {
                Vessel vslToCheck = FlightGlobals.Vessels.First(vsl3 => vsl3.rootPart.flightID == flightID);
                return IsVesselLoaded(vslToCheck);
            }
            catch //if this fails, vessel in question is not controllable, so just return false without throwing an error
            {
                return false;
            }
        }

        public bool IsVesselLoaded(Vessel vsl) //is vessel loaded
        {
            try
            {
                return (FlightGlobals.Vessels.First(vsl2 => vsl2 == vsl).loaded); //list of vessels in game, find the asked vessel
            }
            catch //if this fails, vessel is not loaded so don't throw an error, just return false
            {
                return false;
            }
        }

        public void RefreshCurrentActions()
        {
            CalculateActiveActions();
        }

        public static void LoadGroupVisibilityNames(string LoadString) //ver2 only
        {
            string errLine = "1";
            try
            {
                for (int i = 1; i <= 4; i++)
                {
                    errLine = "2";
                    int KeyLength = LoadString.IndexOf('\u2023');
                    ShowGroupInFlightNames[i] = LoadString.Substring(0, KeyLength);
                    LoadString = LoadString.Substring(KeyLength + 1);
                }
                errLine = "3";
                ShowGroupInFlightNames[5] = LoadString;
            }
            catch (Exception e)
            {
                Log.err("LoadGroupVisiblityNames Fail {0}", errLine);
                Log.ex(typeof(AGXFlight), e);
                ShowGroupInFlightNames[1] = "Group 1";
                ShowGroupInFlightNames[2] = "Group 2";
                ShowGroupInFlightNames[3] = "Group 3";
                ShowGroupInFlightNames[4] = "Group 4";
                ShowGroupInFlightNames[5] = "Group 5";
            }
        }

        public static void SaveWindowPositions()
        {
            AGExtNode.SetValue("FltGroupsX", GroupsWin.x.ToString());
            AGExtNode.SetValue("FltGroupsY", GroupsWin.y.ToString());
            AGExtNode.SetValue("FltSelPartsX", SelPartsWin.x.ToString());
            AGExtNode.SetValue("FltSelPartsY", SelPartsWin.y.ToString());
            AGExtNode.SetValue("FltKeyCodeX", KeyCodeWin.x.ToString());
            AGExtNode.SetValue("FltKeyCodeY", KeyCodeWin.y.ToString());
            AGExtNode.SetValue("FltKeySetX", KeySetWin.x.ToString());
            AGExtNode.SetValue("FltKeySetY", KeySetWin.y.ToString());
            AGExtNode.SetValue("FltCurActsX", CurActsWin.x.ToString());
            AGExtNode.SetValue("FltCurActsY", CurActsWin.y.ToString());
            AGExtNode.SetValue("FltMainX", FlightWin.x.ToString());
            AGExtNode.SetValue("FltMainY", FlightWin.y.ToString());
            AGExtNode.SetValue("RTWinX", RemoteTechQueueWin.x.ToString());
            AGExtNode.SetValue("RTWinY", RemoteTechQueueWin.y.ToString());
            AGExtNode.SetValue("RTWinShow", RTWinShow.ToString());
            if (!ShowAGXMod)
            {
                //rint("No show");
                AGExtNode.SetValue("FltShow", "0");
            }
            else
            {
                // print("show");
                AGExtNode.SetValue("FltShow", "1");
            }
            AGXStaticData.SaveBaseConfigNode(AGExtNode);
        }

        public void OnDisable()
        {
            SaveWindowPositions();

            ToolbarController.Instance.Destroy();

            loadFinished = false;
            if (AGXLockSet)
            {
                InputLockManager.RemoveControlLock("AGExtControlLock");
                AGXLockSet = false;
            }
            StaticData.CurrentVesselActions.Clear();
            GameEvents.onShowUI.Remove(onMyUIShow);
            GameEvents.onHideUI.Remove(onMyUIHide);
        }

        public static string SaveGroupVisibilityNames(string str)
        {
            try
            {
                string StringToSave = ShowGroupInFlightNames[1];
                for (int i = 2; i <= 5; i++)
                {
                    StringToSave = StringToSave + '\u2023' + ShowGroupInFlightNames[i];
                }
                // print("return okay " + StringToSave);
                return StringToSave;
            }
            catch
            {
                // print("Return fail" + str);
                return str;
            }
        }

        public static void SaveShipSpecificData(Vessel vsl) //saves ship data, called on vessel change in Update() or when AGX UI gets hidden, vsl object will always be FlightGlobals.ActiveVessel in initial implementation
        { //actions themselves are not saved via this method, just everything else
            //populate our strings to save to each partmodule
            //currentkeyset is also saved here
            Log.trace("Ship Data save {0}", vsl.id);
            string groupVisibilityToSave = SaveGroupVisibility("");
            string groupVisibiltyNames = SaveGroupVisibilityNames("");
            string directActionsToSave = SaveDirectActionState("");

            if (vsl != null && !vsl.isEVA)
            {
                foreach (Part p in vsl.Parts)
                {
                    Log.trace("Saveing date");
                    ModuleAGX pmAGX = p.Modules.OfType<ModuleAGX>().FirstOrDefault();

                    pmAGX.groupNames = SaveGroupNames(pmAGX); //check against currentMissionID done inside this method

                    if (p.missionID == currentMissionId || !pmAGX.hasData || pmAGX.focusFlightID == 0)
                    { //only save this stuff if they are the current vessel
                        pmAGX.groupVisibility = groupVisibilityToSave;
                        pmAGX.groupVisibilityNames = groupVisibiltyNames;
                        pmAGX.currentKeyset = CurrentKeySetFlight;
                        pmAGX.DirectActionState = directActionsToSave; //this may be an issue if docked vessels dock with the same action group in different directaction states.
                    }
                    pmAGX.focusFlightID = currentMissionId;
                    pmAGX.hasData = true;
                }
            }
        }

        private static bool JoySticks(String s)
        {
            return s.StartsWith("Joystick");
        }

        public void OnGUI()
        {
            if (!showCareerStockAGs)
            {
                ShowAGXMod = false;
            }

            if (ShowAGXMod && showMyUI)
            {
                if (!showCareerCustomAGs)
                {
                    defaultShowingNonNumeric = true;
                }
                if (ShowAGXFlightWin)
                {
                    GroupsInFlightWin.x = FlightWin.x + 235;
                    GroupsInFlightWin.y = FlightWin.y;
                    FlightWin = GUI.Window(673467788, FlightWin, FlightWindow, Localizer.Format("#AGEXT_UI_actions"), UI.AGXWinStyle);
                    if (RTWinShow)
                    {
                        RemoteTechQueueWin = GUI.Window(673462798, RemoteTechQueueWin, RTQueueWindow, "RT Queued Actions", UI.AGXWinStyle);
                    }
                }

                if (ShowGroupsInFlightWindow)
                {
                    GroupsInFlightWin = GUI.Window(673461788, GroupsInFlightWin, GroupsInFlightWindow, "", UI.AGXWinStyle);
                }

                if (ShowKeySetWin)
                {
                    KeySetWin = GUI.Window(673467792, KeySetWin, KeySetWindow, Localizer.Format("#AGEXT_UI_key_sets"), UI.AGXWinStyle);
                    if (!AutoHideGroupsWin)
                    {
                        GroupsWin = GUI.Window(673467795, GroupsWin, GroupsWindow, "", UI.AGXWinStyle);
                    }
                    ShowCurActsWin = false;

                }

                if (ShowSelectedWin)
                {
                    SelPartsWin = GUI.Window(673467794, SelPartsWin, SelParts, Localizer.Format("#AGEXT_UI_selected_parts_numbers_title") + AGEditorSelectedParts.Count(), UI.AGXWinStyle);
                    ShowCurActsWin = true;
                    if (AutoHideGroupsWin && !TempShowGroupsWin)
                    {
                    }
                    else
                    {
                        GroupsWin = GUI.Window(673467795, GroupsWin, GroupsWindow, "", UI.AGXWinStyle);
                    }

                    if (ShowKeyCodeWin)
                    {
                        KeyCodeWin = GUI.Window(673467793, KeyCodeWin, KeyCodeWindow, Localizer.Format("#AGEXT_UI_keycodes"), UI.AGXWinStyle);
                    }

                    string ErrLine = "1";
                    try
                    {
                        ErrLine = "2";
                        if (defaultShowingNonNumeric)
                        {
                            foreach (BaseAction Act in defaultActionsListThisType)
                            {
                                ErrLine = "8";
                                Vector3 partScreenPosD = FlightCamera.fetch.mainCamera.WorldToScreenPoint(Act.listParent.part.transform.position);
                                ErrLine = "9";
                                Rect partCenterWinD = new Rect(partScreenPosD.x - 10, (Screen.height - partScreenPosD.y) - 10, 21, 21);
                                ErrLine = "10";
                                GUI.DrawTexture(partCenterWinD, UI.PartPlus);
                            }
                        }
                        else
                        {
                            ErrLine = "7";
                            foreach (AGXAction agAct in ThisGroupActions)
                            {
                                ErrLine = "8";
                                Vector3 partScreenPosC = FlightCamera.fetch.mainCamera.WorldToScreenPoint(agAct.ba.listParent.part.transform.position);
                                ErrLine = "9";
                                Rect partCenterWinC = new Rect(partScreenPosC.x - 10, (Screen.height - partScreenPosC.y) - 10, 21, 21);
                                ErrLine = "10";
                                GUI.DrawTexture(partCenterWinC, UI.PartPlus);
                            }
                        }

                        ErrLine = "11";
                        foreach (AGXPart agPrt in AGEditorSelectedParts) if (null != agPrt.AGPart)
                        {
                            ErrLine = "3";
                            Vector3 partScreenPosB = FlightCamera.fetch.mainCamera.WorldToScreenPoint(agPrt.AGPart.transform.position);
                            ErrLine = "4";
                            Rect partCenterWinB = new Rect(partScreenPosB.x - 10, (Screen.height - partScreenPosB.y) - 10, 21, 21);
                            ErrLine = "5";
                            GUI.DrawTexture(partCenterWinB, UI.PartCross);
                            ErrLine = "6";
                        }
                    }
                    catch (Exception e)
                    {
                        Log.err("AGX Draw cross fail. {0}", ErrLine);
                        Log.ex(this, e);
                    }
                }
                if (ShowCurActsWin && ShowSelectedWin)
                {
                    CurActsWin = GUI.Window(673467790, CurActsWin, CurrentActionsWindow, Localizer.Format("#AGEXT_UI_action_this_group") + StaticData.CurrentVesselActions.FindAll(p => p.group == AGXCurActGroup).Count.ToString(), UI.AGXWinStyle);
                }
            }

            if (highlightPartThisFrameActsWin || highlightPartThisFrameSelWin || highlightPartThisFrameGroupWin)
            {
                Vector3 partScreenPos = FlightCamera.fetch.mainCamera.WorldToScreenPoint(partToHighlight.transform.position);
                Rect partCenterWin = new Rect(partScreenPos.x - 20, (Screen.height - partScreenPos.y) - 20, 41, 41);
                GUI.DrawTexture(partCenterWin, UI.PartCenter);
            }

            if (showDockedSubVesselIndicators)
            {
                foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                {
                    if (p.missionID == currentMissionId)
                    {
                        Vector3 partScreenPos = FlightCamera.fetch.mainCamera.WorldToScreenPoint(p.transform.position);
                        Rect partCenterWin = new Rect(partScreenPos.x - 20, (Screen.height - partScreenPos.y) - 20, 21, 21);
                        GUI.DrawTexture(partCenterWin, UI.PartPlus);
                    }
                }
            }

            if (showAGXRightClickMenu && showMyUI)
            {
                Rect SettingsWin = new Rect(Screen.width - 200, 40, 150, 205);
                GUI.Window(2233452, SettingsWin, DrawSettingsWin, Localizer.Format("#AGEXT_UI_agx_settings"), UI.AGXWinStyle);
            }
            //Log.dbg("guis " + HighLogic.Skin.font.name + " " + GUI.skin.font.name); 
#if DEBUG
            Font[] fonts = FindObjectsOfType<UnityEngine.Font>();
            Log.dbg("fntc {0}", fonts.Count());
#endif
        }

        public void DrawSettingsWin(int WindowID)
        {
            if (GUI.Button(new Rect(10, 25, 130, 25), Localizer.Format("#AGEXT_UI_setting_show_key_codes"), UI.AGXBtnStyle))
            {
                AGXFlight.FlightWinShowKeycodes = !AGXFlight.FlightWinShowKeycodes;
                if (AGXFlight.FlightWinShowKeycodes)
                {
                    AGXFlight.AGExtNode.SetValue("FlightWinShowKeys", "1");
                }
                else
                {
                    AGXFlight.AGExtNode.SetValue("FlightWinShowKeys", "0");
                }
                AGXStaticData.SaveBaseConfigNode(AGExtNode);
            }

            if (GUI.Button(new Rect(10, 50, 130, 25), Localizer.Format("#AGEXT_UI_setting_edit_actions"), UI.AGXBtnStyle))
            {
                AGXFlight.ClickEditButton();
            }

            if (GUI.Button(new Rect(10, 75, 130, 25), Localizer.Format("#AGEXT_UI_setting_reset_windows"), UI.AGXBtnStyle))
            {
                KeySetWin.x = 250;
                KeySetWin.y = 250;
                GroupsWin.x = 350;
                GroupsWin.y = 350;
                SelPartsWin.x = 200;
                SelPartsWin.y = 200;
                KeyCodeWin.x = 300;
                KeyCodeWin.y = 300;
                CurActsWin.x = 150;
                CurActsWin.y = 150;
                FlightWin.x = 400;
                FlightWin.y = 400;
                RemoteTechQueueWin.x = 450;
                RemoteTechQueueWin.y = 450;
            }

            GUIStyle btnStyle = AutoHideGroupsWin ? UI.AGXBtnStyleDisabled : UI.AGXBtnStyle;
            if (GUI.Button(new Rect(10, 100, 130, 25), Localizer.Format("#AGEXT_UI_setting_auto_hide_groups"), btnStyle))
            {
                AutoHideGroupsWin = !AutoHideGroupsWin;
            }

            if (GUI.Button(new Rect(10, 125, 130, 25), Localizer.Format("#AGEXT_UI_setting_show_remote_tech"), UI.AGXBtnStyle))
            {
                if (RTFound)
                {
                    RTWinShow = !RTWinShow;
                }
            }
            if (GUI.Button(new Rect(10, 150, 130, 25), Localizer.Format("#AGEXT_UI_setting_bypass_remote_tech"), UI.AGXBtnStyle))
            {
                useRT = !useRT;
            }
        }

        public static void ActivateActionGroupCheckModKeys(int group) //backwards compatibility, toggle group
        {
            //Log.dbg("AGX Key check for some reason " + group);
            if (AGXguiMod1Groups[group] == Input.GetKey(AGXguiMod1Key) && AGXguiMod2Groups[group] == Input.GetKey(AGXguiMod2Key))
            {
                //Log.dbg("AGX Key activate for some reason " + group);
                ActivateActionGroup(group, false, false);
            }
        }

        public static void ActivateActionGroupCheckModKeys(int group, bool force, bool forceDir) //backwards compatibility, toggle group
        {
            //Log.dbg("AGX Key check for some reason " + group);
            if (AGXguiMod1Groups[group] == Input.GetKey(AGXguiMod1Key) && AGXguiMod2Groups[group] == Input.GetKey(AGXguiMod2Key))
            {
                //Log.dbg("AGX Key activate for some reason " + group);
                ActivateActionGroup(group, force, forceDir);
            }
        }

        public static void ActivateActionGroup(int group) //backwards compatibility, toggle group
        {
            ActivateActionGroup(group, false, false);
        }

        public static void ActivateActionGroup(int group, bool force, bool forceDir)
        {
            if (RTFound)
            {
                // Log.Info("RemoteTech found");
                Log.detail("delay {0}", AGXRemoteTechLinks.RTTimeDelay(FlightGlobals.ActiveVessel));
                Log.detail("in local {0}", AGXRemoteTechLinks.InLocal(FlightGlobals.ActiveVessel));
                if (useRT)
                {
                    if (FlightGlobals.ActiveVessel.Parts.Any(p => p.protoModuleCrew.Any() && p.Modules.Contains("ModuleCommand"))) //are we in local control? Kerbal on board on a part with command abilities?
                    {
                        // Log.Info("RemoteTech local");
                        AGXRemoteTechQueue.Add(new AGXRemoteTechQueueItem(group, AGXguiNames[group], FlightGlobals.ActiveVessel, Planetarium.GetUniversalTime(), force, forceDir, AGXRemoteTechItemState.COUNTDOWN));
                    }
                    else if (double.IsInfinity(AGXRemoteTechLinks.RTTimeDelay(FlightGlobals.ActiveVessel))) //remotetech returns positive infinity when a vessel is in local control so no delay, note that RT also returns positive infinity when a vessel has no connection so this check has to come second.
                    {
                        // Log.Info("RemoteTech infinity");
                        AGXRemoteTechQueue.Add(new AGXRemoteTechQueueItem(group, AGXguiNames[group], FlightGlobals.ActiveVessel, Planetarium.GetUniversalTime(), force, forceDir, AGXRemoteTechItemState.NOCOMMS));
                    }
                    else
                    {
                        Log.detail("RemoteTech normal {0}", AGXRemoteTechLinks.RTTimeDelay(FlightGlobals.ActiveVessel));
                        AGXRemoteTechQueue.Add(new AGXRemoteTechQueueItem(group, AGXguiNames[group], FlightGlobals.ActiveVessel, Planetarium.GetUniversalTime() + AGXRemoteTechLinks.RTTimeDelay(FlightGlobals.ActiveVessel), force, forceDir, AGXRemoteTechItemState.COUNTDOWN));
                    }
                }
                else
                {
                    ActivateActionGroupActivation(group, force, forceDir);
                    AGXRemoteTechQueue.Add(new AGXRemoteTechQueueItem(group, AGXguiNames[group], FlightGlobals.ActiveVessel, Planetarium.GetUniversalTime(), force, forceDir, AGXRemoteTechItemState.GOOD));
                }
            }
            else
            {
                ActivateActionGroupActivation(group, force, forceDir);
            }
        }

        public static void ActivateActionGroupActivation(int group, bool force, bool forceDir)
        {
            Log.trace("activating group {0}", group);
            foreach (AGXAction agAct in StaticData.CurrentVesselActions.Where(agx => agx.group == group))
            {
                //Log.dbg("ActactA" + forceDir + " " + agAct.ba.name);
                if (groupCooldowns.Any(cd => cd.actGroup == agAct.group && cd.vslFlightID == agAct.ba.listParent.part.vessel.rootPart.flightID)) //rather then fight with double negative bools, do noting if both match, run if no match
                {
                    //if this triggers, that action/group combo is in cooldown
                    Log.detail("Action not activated, that group still in cooldown");
                }
                else //action not in cooldown
                {
                    if (force) //are we forcing a direction or toggling?
                    {
                        if (forceDir) //we are forcing a direction so set the agAct.activated to trigger the direction below correctly
                        {
                            agAct.activated = false; //we are forcing activation so activated is false
                        }
                        else
                        {
                            agAct.activated = true;
                        }
                    }

                    if (agAct.activated)
                    {
                        KSPActionParam actParam = new KSPActionParam(KSPActionGroup.None, KSPActionType.Deactivate);
                        bool saveNoneState = agAct.ba.listParent.part.vessel.ActionGroups[actParam.group];
                        agAct.ba.listParent.part.vessel.ActionGroups[actParam.group] = false;
                        Log.trace("action deactivate FIRE! {0}", agAct.ba.listParent.part.vessel.ActionGroups[actParam.group]);
                        agAct.ba.Invoke(actParam);
                        agAct.activated = false;
                        if (agAct.ba.name != "kOSVoidAction")
                        {
                            foreach (AGXAction agxAct in StaticData.CurrentVesselActions)
                            {
                                if (agxAct.ba == agAct.ba)
                                {
                                    agxAct.activated = false;
                                }
                            }
                        }
                        if (group <= 10)
                        {
                            FlightGlobals.ActiveVessel.ActionGroups[CONST.CustomActions[group]] = false;
                        }
                        agAct.ba.listParent.part.vessel.ActionGroups[actParam.group] = saveNoneState;
                    }
                    else
                    {
                        KSPActionParam actParam = new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate);
                        bool saveNoneState = agAct.ba.listParent.part.vessel.ActionGroups[actParam.group];
                        agAct.ba.listParent.part.vessel.ActionGroups[actParam.group] = true;
                        //Log.dbg("AGX action activate FIRE!" + agAct.ba.guiName);
                        agAct.ba.Invoke(actParam);
                        agAct.activated = true;
                        if (agAct.ba.name != "kOSVoidAction")
                        {
                            foreach (AGXAction agxAct in StaticData.CurrentVesselActions)
                            {
                                if (agxAct.ba == agAct.ba)
                                {
                                    agxAct.activated = true;
                                }
                            }
                        }

                        if (group <= 10)
                        {
                            FlightGlobals.ActiveVessel.ActionGroups[CONST.CustomActions[group]] = true;
                        }
                        agAct.ba.listParent.part.vessel.ActionGroups[actParam.group] = saveNoneState;
                    }

                    if (agAct.ba.listParent.module.moduleName == "ModuleEngines" && agAct.ba.name == "ActivateAction" || agAct.ba.listParent.module.moduleName == "ModuleEngines" && agAct.ba.name == "OnAction")
                    {
                        //overide to activate part when activating an engine so gimbals come on
                        agAct.ba.listParent.part.force_activate();
                        //Log.dbg("Force act");
                    }

                    if (agAct.ba.listParent.module.moduleName == "ModuleEnginesFX" && agAct.ba.name == "ActivateAction" || agAct.ba.listParent.module.moduleName == "ModuleEnginesFX" && agAct.ba.name == "OnAction")
                    {
                        //overide to activate part when activating an engine so gimbals come on
                        agAct.ba.listParent.part.force_activate();
                        //Log.dbg("Force act");
                    }
                }
            }

            if (force && forceDir)
            {
                groupActivatedState[group] = true;
            }
            else if (force && !forceDir)
            {
                groupActivatedState[group] = false;
            }
            else
            {
                groupActivatedState[group] = !groupActivatedState[group];
            }

            Log.trace("Endactivation");
            groupCooldowns.Add(new AGXCooldown(FlightGlobals.ActiveVessel.rootPart.flightID, group, 0));
            CalculateActionsState();
            Log.trace("Endactivation2");
        }

        public static List<BaseAction> GetActionsList(int grp) //return all actions in action gorup
        {
            List<BaseAction> baList = new List<BaseAction>();
            foreach (AGXAction agAct in StaticData.CurrentVesselActions)
            {
                if (agAct.group == grp)
                {
                    baList.Add(agAct.ba);
                }
            }
            return baList;
        }

        public static List<BaseAction> GetActionsList() //return all actions on vessel
        {
            List<BaseAction> baList = new List<BaseAction>();
            foreach (AGXAction agAct in StaticData.CurrentVesselActions)
            {
                baList.Add(agAct.ba);
            }
            return baList;
        }

        public void GroupsInFlightWindow(int WindowID)
        {
            if (showGroupsIsGroups)
            {
                if (GUI.Button(new Rect(5, 5, 70, 20), ShowGroupInFlightNames[1], UI.AGXBtnStyle))
                {
                    ShowGroupInFlightCurrent = 1;
                    CalculateActionsToShow();
                    ShowGroupsInFlightWindow = false;
                }
                if (GUI.Button(new Rect(5, 25, 70, 20), ShowGroupInFlightNames[2], UI.AGXBtnStyle))
                {
                    ShowGroupInFlightCurrent = 2;
                    CalculateActionsToShow();
                    ShowGroupsInFlightWindow = false;
                }
                if (GUI.Button(new Rect(5, 45, 70, 20), ShowGroupInFlightNames[3], UI.AGXBtnStyle))
                {
                    ShowGroupInFlightCurrent = 3;
                    CalculateActionsToShow();
                    ShowGroupsInFlightWindow = false;
                }
                if (GUI.Button(new Rect(5, 65, 70, 20), ShowGroupInFlightNames[4], UI.AGXBtnStyle))
                {
                    ShowGroupInFlightCurrent = 4;
                    CalculateActionsToShow();
                    ShowGroupsInFlightWindow = false;
                }
                if (GUI.Button(new Rect(5, 85, 70, 20), ShowGroupInFlightNames[5], UI.AGXBtnStyle))
                {
                    ShowGroupInFlightCurrent = 5;
                    CalculateActionsToShow();
                    ShowGroupsInFlightWindow = false;
                }
            }
            else
            {
                if (GUI.Button(new Rect(5, 5, 70, 20), KeySetNamesFlight[0], UI.AGXBtnStyle))
                {
                    SaveCurrentKeyBindings();
                    CurrentKeySetFlight = 1;
                    LoadCurrentKeyBindings();
                    ShowGroupsInFlightWindow = false;
                }

                if (GUI.Button(new Rect(5, 25, 70, 20), KeySetNamesFlight[1], UI.AGXBtnStyle))
                {
                    SaveCurrentKeyBindings();
                    CurrentKeySetFlight = 2;
                    LoadCurrentKeyBindings();
                    ShowGroupsInFlightWindow = false;
                }

                if (GUI.Button(new Rect(5, 45, 70, 20), KeySetNamesFlight[2], UI.AGXBtnStyle))
                {
                    SaveCurrentKeyBindings();
                    CurrentKeySetFlight = 3;
                    LoadCurrentKeyBindings();
                    ShowGroupsInFlightWindow = false;
                }

                if (GUI.Button(new Rect(5, 65, 70, 20), KeySetNamesFlight[3], UI.AGXBtnStyle))
                {
                    SaveCurrentKeyBindings();
                    CurrentKeySetFlight = 4;
                    LoadCurrentKeyBindings();
                    ShowGroupsInFlightWindow = false;
                }

                if (GUI.Button(new Rect(5, 85, 70, 20), KeySetNamesFlight[4], UI.AGXBtnStyle))
                {
                    SaveCurrentKeyBindings();
                    CurrentKeySetFlight = 5;
                    LoadCurrentKeyBindings();
                    ShowGroupsInFlightWindow = false;
                }
            }
        }

        public static void ClickEditButton()
        {
            if (ShowSelectedWin || ShowKeySetWin)
            {
                SaveCurrentKeyBindings();

                SaveWindowPositions();
                ShowSelectedWin = false;
                ShowKeySetWin = false;
                SaveShipSpecificData(FlightGlobals.ActiveVessel);
            }
            else if (!ShowSelectedWin)
            {
                ShowSelectedWin = true;
                ShowAGXMod = true;
            }
        }

        public void FlightWindow(int WindowID)
        {
            Log.trace("flight win");
            GUI.skin.scrollView.normal.background = null;
            if (GUI.Button(new Rect(160, 5, 70, 20), ShowGroupInFlightNames[ShowGroupInFlightCurrent], UI.AGXBtnStyle))
            {
                if (!showGroupsIsGroups)
                {
                    showGroupsIsGroups = true;
                    ShowGroupsInFlightWindow = true;
                }
                else
                {
                    ShowGroupsInFlightWindow = !ShowGroupsInFlightWindow;
                }
            }

            if (GUI.Button(new Rect(5, 5, 75, 20), KeySetNamesFlight[CurrentKeySetFlight - 1], UI.AGXBtnStyle))
            {
                if (showGroupsIsGroups)
                {
                    showGroupsIsGroups = false;
                    ShowGroupsInFlightWindow = true;
                }
                else
                {
                    ShowGroupsInFlightWindow = !ShowGroupsInFlightWindow;
                }
            }

            GUI.Box(new Rect(5, 25, 210, Math.Min(410, 10 + (20 * Math.Max(1, ActiveActionsStateToShow.Count)))), "", UI.AGXBtnStyle);
            FlightWinScroll = GUI.BeginScrollView(new Rect(10, 30, 220, Math.Min(400, 20 + (20 * (ActiveActionsStateToShow.Count - 1)))), FlightWinScroll, new Rect(0, 0, 180, (20 * (ActiveActionsStateToShow.Count))));
            Rect FlightWinOld = new Rect(FlightWin.x, FlightWin.y, FlightWin.width, FlightWin.height);
            FlightWin.height = Math.Min(Math.Max(60, 40 + (20 * ActiveActionsStateToShow.Count)), 440);

            if (FlightWin.y + FlightWin.height > Screen.height)
            {
                FlightWin.y = Screen.height - FlightWin.height;
            }

            if (FlightWin.y < 1)
            {
                FlightWin.y = 1;
            }

            if (FlightWinOld.height != FlightWin.height & FlightWinOld.y + FlightWinOld.height > Screen.height - 20)
            {
                FlightWin.y = (FlightWinOld.height + FlightWinOld.y) - FlightWin.height;
            }

            if (ActiveActionsStateToShow.Count > 0)
            {
                for (int i2 = 1; i2 <= ActiveActionsStateToShow.Count; i2 = i2 + 1)
                {
                    Color TxtClr = GUI.contentColor;
                    TextAnchor TxtAnch4 = GUI.skin.button.alignment;
                    if (IsGroupToggle[ActiveActionsStateToShow.ElementAt((i2 - 1)).group])
                    {
                        if (ActiveActionsStateToShow.ElementAt((i2 - 1)).actionOn && !ActiveActionsStateToShow.ElementAt((i2 - 1)).actionOff)
                        {
                            GUI.contentColor = Color.green;
                        }
                        else if (ActiveActionsStateToShow.ElementAt((i2 - 1)).actionOn && ActiveActionsStateToShow.ElementAt((i2 - 1)).actionOff)
                        {
                            GUI.contentColor = Color.yellow;
                        }
                        else if (!ActiveActionsStateToShow.ElementAt((i2 - 1)).actionOn && ActiveActionsStateToShow.ElementAt((i2 - 1)).actionOff)
                        {
                            GUI.contentColor = Color.red;
                        }
                        else
                        {
                            GUI.contentColor = TxtClr;
                        }
                    }

                    if (GUI.Button(new Rect(0, 0 + (20 * (i2 - 1)), FlightWinShowKeycodes ? 110 : 200, 20), ActiveActionsStateToShow.ElementAt((i2 - 1)).group + ": " + AGXguiNames[ActiveActionsStateToShow.ElementAt((i2 - 1)).group], UI.AGXBtnStyleMiddleLeft))
                    {
                        if (VesselIsControlled())
                        {
                            ActivateActionGroup(ActiveActionsStateToShow.ElementAt(i2 - 1).group);
                        }
                    }

                    if (FlightWinShowKeycodes)
                    {
                        string btnName = "";
                        if (AGXguiMod1Groups[ActiveActionsStateToShow.ElementAt((i2 - 1)).group] && AGXguiMod2Groups[ActiveActionsStateToShow.ElementAt((i2 - 1)).group])
                        {
                            btnName = '\u00bd' + AGXguiKeys[ActiveActionsStateToShow.ElementAt((i2 - 1)).group].ToString();
                        }
                        else if (AGXguiMod1Groups[ActiveActionsStateToShow.ElementAt((i2 - 1)).group])
                        {
                            btnName = '\u2474' + AGXguiKeys[ActiveActionsStateToShow.ElementAt((i2 - 1)).group].ToString();
                        }
                        else if (AGXguiMod2Groups[ActiveActionsStateToShow.ElementAt((i2 - 1)).group])
                        {
                            btnName = '\u2475' + AGXguiKeys[ActiveActionsStateToShow.ElementAt((i2 - 1)).group].ToString();
                        }
                        else
                        {
                            btnName = AGXguiKeys[ActiveActionsStateToShow.ElementAt((i2 - 1)).group].ToString();
                        }
                        if (GUI.Button(new Rect(110, 0 + (20 * (i2 - 1)), 90, 20), btnName, UI.AGXBtnStyle))
                        {
                            if (VesselIsControlled())
                            {
                                ActivateActionGroup(ActiveActionsStateToShow.ElementAt(i2 - 1).group);
                            }
                        }
                    }
                    GUI.contentColor = TxtClr;
                }
            }
            GUI.EndScrollView();

            if (ActiveActionsState.Count == 0)
            {
                GUI.Label(new Rect(40, 30, 150, 20), "No actions available", UI.AGXLblStyle);
            }
            else if (ActiveActionsStateToShow.Count == 0)
            {
                GUI.Label(new Rect(40, 30, 150, 20), "Actions in other group", UI.AGXLblStyle);
            }

            GUI.DragWindow();
        }

        public void SetDefaultAction(BaseAction ba, int group)
        {
            ba.actionGroup = ba.actionGroup | CONST.CustomActions[group];
        }

        public void RemoveDefaultAction(BaseAction ba, int group)
        {
            ba.actionGroup = ba.actionGroup & ~CONST.CustomActions[group];
        }

        public void RTQueueWindow(int WindowID)
        {
            GUI.skin.scrollView.normal.background = null;
            RTWinScroll = GUI.BeginScrollView(new Rect(5, 20, 263, Math.Min(100, 20 + (20 * (AGXRemoteTechQueue.Count - 1)))), RTWinScroll, new Rect(0, 0, 250, (20 * (AGXRemoteTechQueue.Count))));
            for (int i2 = 1; i2 <= AGXRemoteTechQueue.Count; i2 = i2 + 1)
            {
                if (GUI.Button(new Rect(0, 0 + (20 * (i2 - 1)), 100, 20), AGXRemoteTechQueue.ElementAt((i2 - 1)).vsl.vesselName, UI.AGXBtnStyleMiddleLeft))
                {
                    FlightGlobals.SetActiveVessel(AGXRemoteTechQueue.ElementAt((i2 - 1)).vsl);
                }
                if (GUI.Button(new Rect(101, 0 + (20 * (i2 - 1)), 100, 20), AGXRemoteTechQueue.ElementAt((i2 - 1)).group + ": " + AGXRemoteTechQueue.ElementAt((i2 - 1)).grpName, UI.AGXBtnStyleMiddleLeft))
                {
                    FlightGlobals.SetActiveVessel(AGXRemoteTechQueue.ElementAt((i2 - 1)).vsl);
                }

                GUIStyle btnStyle = UI.AGXBtnStyleMiddleRight;
                string timeString = "";
                if (AGXRemoteTechQueue.ElementAt((i2 - 1)).state == AGXRemoteTechItemState.GOOD)
                {
                    timeString = "GOOD";
                    btnStyle = UI.AGXBtnStyleMiddleRightActive;
                }
                else if (AGXRemoteTechQueue.ElementAt((i2 - 1)).state == AGXRemoteTechItemState.FAILED)
                {
                    timeString = "FAIL";
                    btnStyle = UI.AGXBtnStyleMiddleRightSelected;
                }
                else if (AGXRemoteTechQueue.ElementAt((i2 - 1)).state == AGXRemoteTechItemState.NOCOMMS)
                {
                    timeString = "NO GO";
                    btnStyle = UI.AGXBtnStyleMiddleRightSelected;
                }
                else if (AGXRemoteTechQueue.ElementAt((i2 - 1)).state == AGXRemoteTechItemState.COUNTDOWN)
                {
                    if (AGXRemoteTechQueue.ElementAt((i2 - 1)).timeToActivate - Planetarium.GetUniversalTime() > 3600)
                    {
                        timeString = ((AGXRemoteTechQueue.ElementAt((i2 - 1)).timeToActivate - Planetarium.GetUniversalTime()) / 3600).ToString("######0.00") + "hr";
                    }
                    else if (AGXRemoteTechQueue.ElementAt((i2 - 1)).timeToActivate - Planetarium.GetUniversalTime() > 60)
                    {
                        timeString = ((AGXRemoteTechQueue.ElementAt((i2 - 1)).timeToActivate - Planetarium.GetUniversalTime()) / 60).ToString("######0.00") + "m";
                    }
                    else
                    {
                        timeString = ((AGXRemoteTechQueue.ElementAt((i2 - 1)).timeToActivate - Planetarium.GetUniversalTime())).ToString("######0.00") + "s";
                    }
                }
                if (GUI.Button(new Rect(201, 0 + (20 * (i2 - 1)), 50, 20), timeString, btnStyle))
                {
                    FlightGlobals.SetActiveVessel(AGXRemoteTechQueue.ElementAt((i2 - 1)).vsl);
                }
            }
            GUI.EndScrollView();

            RTGroup = GUI.TextField(new Rect(270, 20, 75, 20), RTGroup, UI.AGXFldStyleMiddleRight);
            if (GUI.Button(new Rect(270, 40, 75, 20), "Toggle", UI.AGXBtnStyle))
            {
                RemoteTechExernalCall("TOGGLE");
            }

            if (GUI.Button(new Rect(270, 60, 75, 20), "Activate", UI.AGXBtnStyle))
            {
                RemoteTechExernalCall("ACTIVATE");
            }

            if (GUI.Button(new Rect(270, 80, 75, 20), "Deactivate", UI.AGXBtnStyle))
            {
                RemoteTechExernalCall("DEACTIVATE");
            }

            if (!useRT)
            {
                GUI.Label(new Rect(110, 110, 100, 15), "REMOTETECH BYPASSED", UI.AGXBtnStyleTextYellow);
            }

            GUI.DragWindow();
        }

        public void RemoteTechExernalCall(string direction)
        {
            int groupToCall = int.Parse(RTGroup);
            if (groupToCall >= 1 && groupToCall <= 250)
            {
                ConfigNode passAction = new ConfigNode("AGXAction");
                passAction.AddValue("Executor", "AGX");
                passAction.AddValue("QueueLabel", "Action Group: " + groupToCall.ToString());
                passAction.AddValue("ActiveLabel", "Action Group: " + groupToCall.ToString());
                passAction.AddValue("ShortLabel", "AGX " + groupToCall.ToString());
                passAction.AddValue("ReflectionType", "ActionGroupsExtended.AGXRemoteTechLinks, AGExt");
                passAction.AddValue("ReflectionPopMethod", "RTDataReceive");
                passAction.AddValue("GUIDString", FlightGlobals.ActiveVessel.id.ToString());
                passAction.AddValue("FlightID", FlightGlobals.ActiveVessel.rootPart.flightID.ToString());
                passAction.AddValue("Group", groupToCall.ToString());

                if (direction == "TOGGLE")
                {
                    passAction.AddValue("Force", "FALSE");
                    passAction.AddValue("ForceDir", "FALSE");
                }
                else if (direction == "ACTIVATE")
                {
                    passAction.AddValue("Force", "TRUE");
                    passAction.AddValue("ForceDir", "TRUE");
                }
                else
                {
                    passAction.AddValue("Force", "TRUE");
                    passAction.AddValue("ForceDir", "FALSE");
                }

                try
                {
                    Type calledType = Type.GetType("RemoteTech.API.API, RemoteTech");
                    calledType.InvokeMember("QueueCommandToFlightComputer", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { passAction });
                }
                catch (Exception e)
                {
                    Log.err("Reflection to RT fail {0}", e);
                }
            }
        }

        public void CurrentActionsWindow(int WindowID)
        {
            GUI.skin.scrollView.normal.background = null;
            ThisGroupActions = new List<AGXAction>();
            ThisGroupActions.AddRange(StaticData.CurrentVesselActions.Where(p => p.group == AGXCurActGroup));
            GUI.Box(new Rect(5, 25, 310, 110), "");
            CurGroupsWin = GUI.BeginScrollView(new Rect(10, 30, 330, 100), CurGroupsWin, new Rect(0, 0, 310, Math.Max(100, 0 + (20 * (ThisGroupActions.Count)))));
            int RowCnt = 1;
            highlightPartThisFrameActsWin = false;
            if (defaultShowingNonNumeric)
            {
                GUI.Label(new Rect(10, 30, 274, 30), "Abort/Brake/Gear/Lights actions\nshow in Groups window", UI.AGXLblStyle);
            }
            else
            {
                if (ThisGroupActions.Count > 0)
                {
                    while (RowCnt <= ThisGroupActions.Count)
                    {
                        if (Mouse.screenPos.y >= CurActsWin.y + 30 && Mouse.screenPos.y <= CurActsWin.y + 130 && new Rect(CurActsWin.x + 10, (CurActsWin.y + 30 + ((RowCnt - 1) * 20)) - CurGroupsWin.y, 300, 20).Contains(Mouse.screenPos))
                        {
                            highlightPartThisFrameActsWin = true;
                            partToHighlight = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part;
                        }

                        TextAnchor TxtAnch4 = new TextAnchor();
                        TxtAnch4 = GUI.skin.button.alignment;

                        if (GUI.Button(new Rect(0, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).group.ToString() + ": " + AGXguiNames[ThisGroupActions.ElementAt(RowCnt - 1).group], UI.AGXBtnStyleMiddleLeft))
                        {
                            StaticData.CurrentVesselActions.RemoveAll(ag => ag.group == ThisGroupActions.ElementAt(RowCnt - 1).group & ag.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba);
                            ModuleAGX agxMod = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.Modules.OfType<ModuleAGX>().First();
                            agxMod.agxActionsThisPart.RemoveAll(p => p == ThisGroupActions.ElementAt(RowCnt - 1));
                            RefreshCurrentActions();

                            if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                            {
                                RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                            }
                        }

                        if (GUI.Button(new Rect(100, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).prt.partInfo.title, UI.AGXBtnStyleMiddleLeft))
                        {
                            StaticData.CurrentVesselActions.RemoveAll(ag => ag.group == ThisGroupActions.ElementAt(RowCnt - 1).group & ag.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba);
                            ModuleAGX agxMod = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.Modules.OfType<ModuleAGX>().First();
                            agxMod.agxActionsThisPart.RemoveAll(p => p == ThisGroupActions.ElementAt(RowCnt - 1));
                            RefreshCurrentActions();
                            if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                            {
                                RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                            }
                        }
                        try
                        {
                            if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).ba.guiName, UI.AGXBtnStyleMiddleLeft))
                            {
                                StaticData.CurrentVesselActions.RemoveAll(ag => ag.group == ThisGroupActions.ElementAt(RowCnt - 1).group & ag.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba);
                                ModuleAGX agxMod = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.Modules.OfType<ModuleAGX>().First();
                                agxMod.agxActionsThisPart.RemoveAll(p => p == ThisGroupActions.ElementAt(RowCnt - 1));
                                RefreshCurrentActions();
                                if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                                {
                                    RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                                }
                            }
                        }
                        catch
                        {
                            if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), "Error", UI.AGXBtnStyleMiddleLeft))
                            {
                                StaticData.CurrentVesselActions.RemoveAll(ag => ag.group == ThisGroupActions.ElementAt(RowCnt - 1).group & ag.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba);
                                ModuleAGX agxMod = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.Modules.OfType<ModuleAGX>().First();
                                agxMod.agxActionsThisPart.RemoveAll(p => p == ThisGroupActions.ElementAt(RowCnt - 1));
                                RefreshCurrentActions();
                                if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                                {
                                    RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                                }
                            }
                        }

                        RowCnt = RowCnt + 1;
                    }
                }
                else
                {
                    TextAnchor TxtAnch5 = new TextAnchor();
                    TxtAnch5 = GUI.skin.label.alignment;

                    GUI.Label(new Rect(10, 30, 274, 30), Localizer.Format("#AGEXT_UI_no_actions"), UI.AGXLblStyleMiddleCenter);
                }
            }
            GUI.EndScrollView();

            GUI.DragWindow();
        }

        public void SaveNewKeyBind() //save our new keybind to the correct ModuleAGX
        {
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                if (p.Modules.Contains<ModuleAGX>())
                {
                    Log.trace("1");
                    if (p.Modules.OfType<ModuleAGX>().First().focusFlightID == currentMissionId)
                    {
                        // Log.Info("2");
                        p.Modules.OfType<ModuleAGX>().First().currentKeyset = CurrentKeySetFlight;
                        p.Modules.OfType<ModuleAGX>().First().hasData = true;
                    }
                }
            }
        }

        public void FlightSaveKeysetStuff()
        {
            AGExtNode.SetValue("KeySetName1", KeySetNamesFlight[0]);
            AGExtNode.SetValue("KeySetName2", KeySetNamesFlight[1]);
            AGExtNode.SetValue("KeySetName3", KeySetNamesFlight[2]);
            AGExtNode.SetValue("KeySetName4", KeySetNamesFlight[3]);
            AGExtNode.SetValue("KeySetName5", KeySetNamesFlight[4]);
            CurrentKeySetNameFlight = KeySetNamesFlight[CurrentKeySetFlight - 1];
            AGXStaticData.SaveBaseConfigNode(AGExtNode);
        }

        public static void FlightSaveKeysetStuffStatic()
        {
            AGExtNode.SetValue("KeySetName1", KeySetNamesFlight[0]);
            AGExtNode.SetValue("KeySetName2", KeySetNamesFlight[1]);
            AGExtNode.SetValue("KeySetName3", KeySetNamesFlight[2]);
            AGExtNode.SetValue("KeySetName4", KeySetNamesFlight[3]);
            AGExtNode.SetValue("KeySetName5", KeySetNamesFlight[4]);
            CurrentKeySetNameFlight = KeySetNamesFlight[CurrentKeySetFlight - 1];
            AGXStaticData.SaveBaseConfigNode(AGExtNode);
        }
        public void KeySetWindow(int WindowID)
        {
            GUI.DrawTexture(new Rect(6, (CurrentKeySetFlight * 25) + 1, 68, 18), UI.BtnTexGrn);

            if (GUI.Button(new Rect(5, 25, 70, 20), Localizer.Format("#AGEXT_UI_select_1"), UI.AGXBtnStyle))
            {
                FlightSaveKeysetStuff();
                SaveCurrentKeyBindings();

                CurrentKeySetFlight = 1;
                CurrentKeySetNameFlight = KeySetNamesFlight[0];
                SaveNewKeyBind();
                LoadCurrentKeyBindings();
            }
            KeySetNamesFlight[0] = GUI.TextField(new Rect(80, 25, 100, 20), KeySetNamesFlight[0]);

            if (GUI.Button(new Rect(5, 50, 70, 20), Localizer.Format("#AGEXT_UI_select_2"), UI.AGXBtnStyle))
            {
                FlightSaveKeysetStuff();
                SaveCurrentKeyBindings();
                CurrentKeySetFlight = 2;
                CurrentKeySetNameFlight = KeySetNamesFlight[1];
                SaveNewKeyBind();
                LoadCurrentKeyBindings();
            }

            KeySetNamesFlight[1] = GUI.TextField(new Rect(80, 50, 100, 20), KeySetNamesFlight[1]);
            if (GUI.Button(new Rect(5, 75, 70, 20), Localizer.Format("#AGEXT_UI_select_3"), UI.AGXBtnStyle))
            {
                FlightSaveKeysetStuff();
                SaveCurrentKeyBindings();
                CurrentKeySetFlight = 3;
                CurrentKeySetNameFlight = KeySetNamesFlight[2];
                SaveNewKeyBind();
                LoadCurrentKeyBindings();
            }

            KeySetNamesFlight[2] = GUI.TextField(new Rect(80, 75, 100, 20), KeySetNamesFlight[2]);
            if (GUI.Button(new Rect(5, 100, 70, 20), Localizer.Format("#AGEXT_UI_select_4"), UI.AGXBtnStyle))
            {
                FlightSaveKeysetStuff();
                SaveCurrentKeyBindings();
                CurrentKeySetFlight = 4;
                CurrentKeySetNameFlight = KeySetNamesFlight[3];
                SaveNewKeyBind();
                LoadCurrentKeyBindings();
            }

            KeySetNamesFlight[3] = GUI.TextField(new Rect(80, 100, 100, 20), KeySetNamesFlight[3]);
            if (GUI.Button(new Rect(5, 125, 70, 20), Localizer.Format("#AGEXT_UI_select_5"), UI.AGXBtnStyle))
            {
                FlightSaveKeysetStuff();
                SaveCurrentKeyBindings();
                CurrentKeySetFlight = 5;
                CurrentKeySetNameFlight = KeySetNamesFlight[4];
                SaveNewKeyBind();
                LoadCurrentKeyBindings();
            }

            KeySetNamesFlight[4] = GUI.TextField(new Rect(80, 125, 100, 20), KeySetNamesFlight[4]);
            Color TxtClr3 = GUI.contentColor;
            GUI.contentColor = new Color(0.5f, 1f, 0f, 1f);
            GUI.Label(new Rect(5, 145, 175, 25), Localizer.Format("#AGEXT_UI_action_group_groups"), UI.AGXLblStyleMiddleCenterBold);
            GUI.contentColor = TxtClr3;

            GUI.DrawTexture(new Rect(6, (ShowGroupInFlightCurrent * 25) + 141, 68, 18), UI.BtnTexGrn);
            if (GUI.Button(new Rect(5, 165, 70, 20), Localizer.Format("#AGEXT_UI_group_1"), UI.AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 1;
            }
            ShowGroupInFlightNames[1] = GUI.TextField(new Rect(80, 165, 100, 20), ShowGroupInFlightNames[1]);

            if (GUI.Button(new Rect(5, 190, 70, 20), Localizer.Format("#AGEXT_UI_group_2"), UI.AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 2;
            }

            ShowGroupInFlightNames[2] = GUI.TextField(new Rect(80, 190, 100, 20), ShowGroupInFlightNames[2]);
            if (GUI.Button(new Rect(5, 215, 70, 20), Localizer.Format("#AGEXT_UI_group_3"), UI.AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 3;
            }

            ShowGroupInFlightNames[3] = GUI.TextField(new Rect(80, 215, 100, 20), ShowGroupInFlightNames[3]);
            if (GUI.Button(new Rect(5, 240, 70, 20), Localizer.Format("#AGEXT_UI_group_4"), UI.AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 4;
            }

            ShowGroupInFlightNames[4] = GUI.TextField(new Rect(80, 240, 100, 20), ShowGroupInFlightNames[4]);
            if (GUI.Button(new Rect(5, 265, 70, 20), Localizer.Format("#AGEXT_UI_group_5"), UI.AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 5;
            }

            ShowGroupInFlightNames[5] = GUI.TextField(new Rect(80, 265, 100, 20), ShowGroupInFlightNames[5]);
            if (GUI.Button(new Rect(5, 300, 175, 30), Localizer.Format("#AGEXT_UI_close_window"), UI.AGXBtnStyle))
            {
                FlightSaveKeysetStuff();
                ShowKeySetWin = false;
                ShowSelectedWin = true;
            }

            GUI.DragWindow();
        }

        public static void FlightSaveGlobalInfo()
        {
            string ErrString = "1";
            try
            {
                SaveCurrentKeyBindings();
                ErrString = "1a";
                FlightSaveKeysetStuffStatic();
                ErrString = "1b";
            }
            catch (Exception e)
            {
                Log.err("AGX FlightSaveGlobalInfo fail {0}", ErrString);
                Log.ex(typeof(AGXFlight), e);
            }
        }

        public void LoadCurrentKeyBindings()
        {
            //Log.dbg("jeyset load "+ CurrentKeySetFlight);
            String LoadString = AGExtNode.GetValue("KeySet" + CurrentKeySetFlight.ToString());
            //Log.dbg("Keyset load " + CurrentKeySet + " " + LoadString);

            for (int i = 1; i <= 250; i++)
            {
                AGXguiKeys[i] = KeyCode.None;
            }

            if (LoadString.Length > 0)
            {
                while (LoadString[0] == '\u2023')
                {
                    LoadString = LoadString.Substring(1);

                    int KeyLength;
                    int KeyIndex;
                    KeyCode LoadKey;
                    KeyLength = LoadString.IndexOf('\u2023');

                    if (KeyLength == -1)
                    {
                        KeyIndex = Convert.ToInt32(LoadString.Substring(0, 3));
                        LoadString = LoadString.Substring(3);
                        LoadKey = (KeyCode)Enum.Parse(typeof(KeyCode), LoadString);
                        AGXguiKeys[KeyIndex] = LoadKey;
                    }
                    else
                    {
                        KeyIndex = Convert.ToInt32(LoadString.Substring(0, 3));
                        LoadString = LoadString.Substring(3);
                        LoadKey = (KeyCode)Enum.Parse(typeof(KeyCode), LoadString.Substring(0, KeyLength - 3));
                        LoadString = LoadString.Substring(KeyLength - 3);
                        AGXguiKeys[KeyIndex] = LoadKey;
                    }
                }
            }

            String GroupKeysMod1ToLoad = AGExtNode.GetValue("KeySetMod1Group" + CurrentKeySetFlight.ToString());
            String GroupKeysMod2ToLoad = AGExtNode.GetValue("KeySetMod2Group" + CurrentKeySetFlight.ToString());
            for (int i = 0; i <= 249; i++)
            {
                if (GroupKeysMod1ToLoad[i] == '1')
                {
                    AGXguiMod1Groups[i + 1] = true;
                }
                else
                {
                    AGXguiMod1Groups[i + 1] = false;
                }
                if (GroupKeysMod2ToLoad[i] == '1')
                {
                    AGXguiMod2Groups[i + 1] = true;
                }
                else
                {
                    AGXguiMod2Groups[i + 1] = false;
                }
            }
            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), AGExtNode.GetValue("KeySetModKey1" + CurrentKeySetFlight.ToString()));
            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), AGExtNode.GetValue("KeySetModKey2" + CurrentKeySetFlight.ToString()));
        }

        public static void SaveCurrentKeyBindings()
        {
            //Log.dbg("Saving current keybinds " + CurrentKeySetFlight);

            AGExtNode.SetValue("KeySetName" + CurrentKeySetFlight, CurrentKeySetNameFlight);
            string SaveString = "";
            int KeyID = 1;
            while (KeyID <= 250)
            {
                if (AGXguiKeys[KeyID] != KeyCode.None)
                {
                    SaveString = SaveString + '\u2023' + KeyID.ToString("000") + AGXguiKeys[KeyID].ToString();
                }
                KeyID = KeyID + 1;
            }

            AGExtNode.SetValue("KeySet" + CurrentKeySetFlight.ToString(), SaveString);

            string GroupsMod1ToSave = "";
            string GroupsMod2ToSave = "";
            for (int i = 1; i <= 250; i++)
            {
                if (AGXguiMod1Groups[i])
                {
                    GroupsMod1ToSave = GroupsMod1ToSave + "1";
                }
                else
                {
                    GroupsMod1ToSave = GroupsMod1ToSave + "0";
                }
                if (AGXguiMod2Groups[i])
                {
                    GroupsMod2ToSave = GroupsMod2ToSave + "1";
                }
                else
                {
                    GroupsMod2ToSave = GroupsMod2ToSave + "0";
                }
            }
            AGExtNode.SetValue("KeySetMod1Group" + CurrentKeySetFlight.ToString(), GroupsMod1ToSave);
            AGExtNode.SetValue("KeySetMod2Group" + CurrentKeySetFlight.ToString(), GroupsMod2ToSave);
            AGExtNode.SetValue("KeySetModKey1" + CurrentKeySetFlight.ToString(), AGXguiMod1Key.ToString());
            AGExtNode.SetValue("KeySetModKey2" + CurrentKeySetFlight.ToString(), AGXguiMod2Key.ToString());
            AGXStaticData.SaveBaseConfigNode(AGExtNode);
        }

        public void KeyCodeWindow(int WindowID)
        {
            if (GUI.Button(new Rect(5, 3, 80, 25), Localizer.Format("#AGEXT_UI_clear_key"), UI.AGXBtnStyle))
            {
                if (AGXguiMod1KeySelecting)
                {
                    AGXguiMod1Key = KeyCode.None;
                    AGXguiMod1KeySelecting = false;
                }
                else if (AGXguiMod2KeySelecting)
                {
                    AGXguiMod2Key = KeyCode.None;
                    AGXguiMod2KeySelecting = false;
                }
                else
                {
                    AGXguiKeys[AGXCurActGroup] = KeyCode.None;
                    ShowKeyCodeWin = false;
                    CalculateActiveActions();
                }
            }

            GUIStyle btnStyle = UI.AGXBtnStyle;
            if (AGXguiMod1KeySelecting)                 btnStyle = UI.AGXBtnStyleDisabled;
            else if (AGXguiMod1Groups[AGXCurActGroup])  btnStyle = UI.AGXBtnStyleEnabled;
            else                                        btnStyle = UI.AGXBtnStyle;

            if (GUI.Button(new Rect(80, 3, 60, 25), AGXguiMod1Key.ToString(), btnStyle))
            {
                if (Event.current.button == 0)
                {
                    AGXguiMod1Groups[AGXCurActGroup] = !AGXguiMod1Groups[AGXCurActGroup];
                }
                if (Event.current.button == 1)
                {
                    AGXguiMod1KeySelecting = true;
                    AGXguiMod2KeySelecting = false;
                }
            }

            if (AGXguiMod2KeySelecting)                 btnStyle = UI.AGXBtnStyleDisabled;
            else if (AGXguiMod2Groups[AGXCurActGroup])  btnStyle = UI.AGXBtnStyleEnabled;
            else                                        btnStyle = UI.AGXBtnStyle;

            if (GUI.Button(new Rect(140, 3, 60, 25), AGXguiMod2Key.ToString(), btnStyle))
            {
                if (Event.current.button == 0)
                {
                    AGXguiMod2Groups[AGXCurActGroup] = !AGXguiMod2Groups[AGXCurActGroup];
                }
                if (Event.current.button == 1)
                {
                    AGXguiMod2KeySelecting = true;
                    AGXguiMod1KeySelecting = false;
                }
            }

            if (ShowJoySticks)
            {
                GUI.DrawTexture(new Rect(281, 3, 123, 18), UI.BtnTexGrn);
            }

            if (GUI.Button(new Rect(280, 2, 125, 20), Localizer.Format("#AGEXT_UI_show_joy_Sticks"), UI.AGXBtnStyle))
            {
                ShowJoySticks = !ShowJoySticks;
            }

            if (!ShowJoySticks)
            {
                int KeyListCount = 0;
                while (KeyListCount <= 34)
                {
                    if (GUI.Button(new Rect(5, 25 + (KeyListCount * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), UI.AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            ShowKeyCodeWin = false;
                            CalculateActiveActions();
                        }
                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 69)
                {
                    if (GUI.Button(new Rect(105, 25 + ((KeyListCount - 35) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), UI.AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            ShowKeyCodeWin = false;
                            CalculateActiveActions();
                        }
                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 104)
                {
                    if (GUI.Button(new Rect(205, 25 + ((KeyListCount - 70) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), UI.AGXBtnStyle))
                    {

                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            ShowKeyCodeWin = false;
                            CalculateActiveActions();
                        }
                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 139)
                {
                    if (GUI.Button(new Rect(305, 25 + ((KeyListCount - 105) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), UI.AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            ShowKeyCodeWin = false;
                            CalculateActiveActions();
                        }
                    }
                    KeyListCount = KeyListCount + 1;
                }
            }
            else
            {
                int JoyStickCount = 0;
                while (JoyStickCount <= 34)
                {
                    if (GUI.Button(new Rect(5, 25 + (JoyStickCount * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount), UI.AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            ShowKeyCodeWin = false;
                            CalculateActiveActions();
                        }
                    }
                    JoyStickCount = JoyStickCount + 1;
                }
                while (JoyStickCount <= 69)
                {
                    if (GUI.Button(new Rect(130, 25 + ((JoyStickCount - 35) * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount), UI.AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            ShowKeyCodeWin = false;
                            CalculateActiveActions();
                        }
                    }
                    JoyStickCount = JoyStickCount + 1;
                }
                while (JoyStickCount <= 99)
                {
                    if (GUI.Button(new Rect(255, 25 + ((JoyStickCount - 70) * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount), UI.AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            ShowKeyCodeWin = false;
                            CalculateActiveActions();
                        }
                    }
                    JoyStickCount = JoyStickCount + 1;
                }

                GUI.Label(new Rect(260, 665, 120, 20), Localizer.Format("#AGEXT_UI_button_test"), UI.AGXLblStyle);
                if (Event.current.keyCode != KeyCode.None)
                {
                    LastKeyCode = Event.current.keyCode.ToString();
                }
                GUI.TextField(new Rect(260, 685, 130, 20), LastKeyCode, UI.AGXFldStyle);
            }
            GUI.DragWindow();
        }

        public void RefreshPartActions()
        {
            try
            {
                PartActionsList.Clear();
                PartActionsList.AddRange(AGEditorSelectedParts.First().AGPart.Actions.Where(ba => ba.active));
                foreach (PartModule pm in AGEditorSelectedParts.First().AGPart.Modules)
                {
                    PartActionsList.AddRange(pm.Actions.Where(ba => ba.active));
                }
                //Log.dbg("AGX Actions refresh found actions: " + PartActionsList.Count);
            }
            catch
            {
                //              print("AGX Actions refresh fail");
            }
        }

        public void WipeVesselData() //loading screwed up, reset everything
        {
            Log.warn("Error loading, resetting data this vessel to defaults. If this vessel does have saved data that should load, please file a bug report.");
            CurrentKeySetFlight = 1;
            LoadCurrentKeyBindings();
            CurrentKeySetNameFlight = KeySetNamesFlight[CurrentKeySetFlight - 1];
            StaticData.CurrentVesselActions.Clear();
            for (int i = 1; i <= 250; i++)
            {
                IsGroupToggle[i] = false;
                isDirectAction[i] = false;
                AGXguiNames[i] = "";
            }
            ShowGroupInFlightNames[1] = "Group1";
            ShowGroupInFlightNames[2] = "Group2";
            ShowGroupInFlightNames[3] = "Group3";
            ShowGroupInFlightNames[4] = "Group4";
            ShowGroupInFlightNames[5] = "Group5";
            RefreshCurrentActions();
        }

        public void LoadVesselDataFromPM(ModuleAGX rootAGX)
        {
            CurrentKeySetFlight = rootAGX.currentKeyset;
            LoadCurrentKeyBindings();
            CurrentKeySetNameFlight = KeySetNamesFlight[CurrentKeySetFlight - 1];
            LoadGroupNames(); //group names check every part now based on currentMissionID, pass no values
            LoadGroupVisibility(rootAGX.groupVisibility);
            LoadGroupVisibilityNames(rootAGX.groupVisibilityNames);
            LoadDirectActionState(rootAGX.DirectActionState);

            StaticData.CurrentVesselActions.Clear(); //refreshing list, clear old actions
            foreach (Part p in rootAGX.vessel.Parts)
            {
                foreach (AGXAction agAct in p.Modules.OfType<ModuleAGX>().FirstOrDefault().agxActionsThisPart)
                {
                    if (!StaticData.CurrentVesselActions.Contains(agAct))
                    {
                        StaticData.CurrentVesselActions.Add(agAct); //add action from part if not already present, not sure what could cause doubles but error trap it
                    }
                }
            }

            RefreshCurrentActions();
        }

        public void SelParts(int WindowID)
        {
            GUI.skin.scrollView.normal.background = null;
            SelectedPartsCount = AGEditorSelectedParts.Count;
            int SelPartsLeft = -10; //move everything left or right by tweaking this variable
            if (GUI.Button(new Rect(3, 1, 90, 20), "Prev Docked", UI.AGXBtnStyle))
            {
                SaveShipSpecificData(FlightGlobals.ActiveVessel);
                List<uint> missionIDs = new List<uint>();
                Dictionary<uint, ModuleAGX> missionIDsPM = new Dictionary<uint, ModuleAGX>();
                foreach (Part p in FlightGlobals.ActiveVessel.parts)
                {
                    if (!missionIDs.Contains(p.missionID))
                    {
                        missionIDs.Add(p.missionID);
                        missionIDsPM.Add(p.missionID, p.Modules.OfType<ModuleAGX>().First());
                    }
                }

                int tempIndex = missionIDs.IndexOf(currentMissionId);

                if (tempIndex == 0)
                {
                    currentMissionId = missionIDs.Last();
                }
                else
                {
                    currentMissionId = missionIDs[tempIndex - 1];
                }

                showDockedSubVesselIndicators = true;
                StartCoroutine(DockedSubVesselsIconTimer());
                Log.trace("dd {0}|{1}", missionIDs.Count, currentMissionId);
                LoadVesselDataFromPM(missionIDsPM[currentMissionId]);
            }

            if (GUI.Button(new Rect(272, 1, 90, 20), "Next Docked", UI.AGXBtnStyle))
            {
                SaveShipSpecificData(FlightGlobals.ActiveVessel);
                List<uint> missionIDs = new List<uint>();
                Dictionary<uint, ModuleAGX> missionIDsPM = new Dictionary<uint, ModuleAGX>();
                foreach (Part p in FlightGlobals.ActiveVessel.parts)
                {
                    if (!missionIDs.Contains(p.missionID))
                    {
                        missionIDs.Add(p.missionID);
                        missionIDsPM.Add(p.missionID, p.Modules.OfType<ModuleAGX>().First());
                    }
                }
                int tempIndex = missionIDs.IndexOf(currentMissionId);
                Log.trace("test {0} {1} {2} {3}", tempIndex, missionIDs.Count, missionIDs[tempIndex], currentMissionId);

                if (tempIndex == missionIDs.Count - 1)
                {
                    currentMissionId = missionIDs.First();
                }
                else
                {
                    currentMissionId = missionIDs[tempIndex + 1];
                }
                Log.trace("test2 {0}", currentMissionId);

                showDockedSubVesselIndicators = true;
                StartCoroutine(DockedSubVesselsIconTimer());
                LoadVesselDataFromPM(missionIDsPM[currentMissionId]);
            }

            GUI.Box(new Rect(SelPartsLeft + 20, 25, 200, 110), "");
            if (AGEditorSelectedParts != null && AGEditorSelectedParts.Count > 0)
            {
                int ButtonCount = 1;

                ScrollPosSelParts = GUI.BeginScrollView(new Rect(SelPartsLeft + 20, 30, 220, 110), ScrollPosSelParts, new Rect(0, 0, 200, (20 * Math.Max(5, SelectedPartsCount)) + 10));
                highlightPartThisFrameSelWin = false;
                while (ButtonCount <= SelectedPartsCount)
                {
                    if (Mouse.screenPos.y >= SelPartsWin.y + 30 && Mouse.screenPos.y <= SelPartsWin.y + 140 && new Rect(SelPartsWin.x + SelPartsLeft + 25, (SelPartsWin.y + 30 + ((ButtonCount - 1) * 20)) - ScrollPosSelParts.y, 190, 20).Contains(Mouse.screenPos))
                    {
                        highlightPartThisFrameSelWin = true;
                        //Log.dbg("part found to highlight " + AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart.ConstructID);
                        partToHighlight = AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart;
                    }

                    //highlight code here
                    if (GUI.Button(new Rect(5, 0 + ((ButtonCount - 1) * 20), 190, 20), AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart.partInfo.title, UI.AGXBtnStyle))
                    {
                        AGEditorSelectedParts.RemoveAt(ButtonCount - 1);
                        if (AGEditorSelectedParts.Count == 0)
                        {
                            AGEEditorSelectedPartsSame = false;
                        }
                        return;
                    }

                    ButtonCount = ButtonCount + 1;
                }
                GUI.EndScrollView();
            }
            else //no parts selected, show list all parts button
            {
                if (GUI.Button(new Rect(SelPartsLeft + 50, 45, 140, 70), Localizer.Format("#AGEXT_UI_show_list_hint"), UI.AGXBtnStyle)) //button itself
                {
                    showAllPartsListTitles = new List<string>(); //generate list of all parts 
                    showAllPartsListTitles.Clear(); //this probably isn't needed, but it works as is, not messing with it
                    foreach (Part p in FlightGlobals.ActiveVessel.parts) //cycle all parts
                    {
                        List<BaseAction> actCheck = new List<BaseAction>(); //start check to see if p has any actions
                        actCheck.AddRange(p.Actions); //add actions on part
                        foreach (PartModule pm in p.Modules) //add actions from each partModule on part
                        {
                            actCheck.AddRange(pm.Actions);
                        }
                        if (actCheck.Count > 0) //only add part to showAllPartsListTitles if part has actions on it
                        {
                            if (!showAllPartsListTitles.Contains(p.partInfo.title))
                            {
                                showAllPartsListTitles.Add(p.partInfo.title);
                            }
                        }
                    }
                    showAllPartsListTitles.Sort(); //sort alphabetically
                    showAllPartsList = true; //change groups win to all parts list
                    TempShowGroupsWin = true; // if autohide enabled, show group win
                }
            }

            GUIStyle btnStyle = UI.AGXBtnStyle;
            if (SelPartsIncSym)
            {
                btnStyle = UI.AGXBtnStyleEnabled;
                BtnTxt = Localizer.Format("#AGEXT_UI_select_mode_yes");
            }
            else
            {
                btnStyle = UI.AGXBtnStyleDisabled;
                BtnTxt = Localizer.Format("#AGEXT_UI_select_mode_no");
            }

            if (GUI.Button(new Rect(SelPartsLeft + 245, 25, 110, 25), BtnTxt, btnStyle))
            {
                SelPartsIncSym = !SelPartsIncSym;

            }

            if (GUI.Button(new Rect(SelPartsLeft + 245, 55, 110, 25), Localizer.Format("#AGEXT_UI_clear_all"), UI.AGXBtnStyle))
            {
                AGEditorSelectedParts.Clear();
                PartActionsList.Clear();
                AGEEditorSelectedPartsSame = false;
            }

            GUI.Box(new Rect(SelPartsLeft + 20, 150, 200, 110), "");

            if (AGEEditorSelectedPartsSame)
            {
                if (PartActionsList.Count > 0)
                {
                    int ActionsCount = 1;
                    int ActionsCountTotal = PartActionsList.Count;

                    ScrollPosSelPartsActs = GUI.BeginScrollView(new Rect(SelPartsLeft + 20, 155, 220, 110), ScrollPosSelPartsActs, new Rect(0, 0, 200, (20 * Math.Max(5, ActionsCountTotal)) + 10));

                    while (ActionsCount <= ActionsCountTotal)
                    {
                        if (GUI.Button(new Rect(5, 0 + ((ActionsCount - 1) * 20), 190, 20), PartActionsList.ElementAt(ActionsCount - 1).guiName, UI.AGXBtnStyle))
                        {
                            if (defaultShowingNonNumeric)
                            {
                                string baname = PartActionsList.ElementAt(ActionsCount - 1).name;
                                string moduleName = PartActionsList.ElementAt(ActionsCount - 1).listParent.module.name;
                                foreach (AGXPart agP in AGEditorSelectedParts)
                                {
                                    List<BaseAction> actsToCheck = new List<BaseAction>();
                                    if (moduleName.Length > 0)
                                    {
                                        foreach (PartModule pm in agP.AGPart.Modules)
                                        {
                                            if (pm.name == moduleName)
                                            {
                                                actsToCheck.AddRange(pm.Actions);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        actsToCheck.AddRange(agP.AGPart.Actions);
                                    }
                                    foreach (BaseAction ba in actsToCheck)
                                    {
                                        if (ba.name == baname)
                                        {
                                            ba.actionGroup = ba.actionGroup | defaultGroupToShow;
                                        }
                                    }
                                }
                                RefreshDefaultActionsListType();
                            }
                            else
                            {
                                int PrtCnt = 0;
                                string baname = PartActionsList.ElementAt(ActionsCount - 1).name;

                                foreach (AGXPart agPrt in AGEditorSelectedParts)
                                {
                                    List<BaseAction> ThisPartActionsList = new List<BaseAction>();
                                    ThisPartActionsList.AddRange(agPrt.AGPart.Actions.Where(a => a.active));
                                    foreach (PartModule pm3 in agPrt.AGPart.Modules)
                                    {
                                        ThisPartActionsList.AddRange(pm3.Actions.Where(a => a.active));
                                    }
                                    AGXAction ToAdd = new AGXAction();
                                    if (ThisPartActionsList.ElementAt(ActionsCount - 1).guiName == PartActionsList.ElementAt(ActionsCount - 1).guiName)
                                    {
                                        ToAdd = new AGXAction() { prt = agPrt.AGPart, ba = ThisPartActionsList.ElementAt(ActionsCount - 1), group = AGXCurActGroup, activated = false };
                                    }
                                    else
                                    {
                                        ToAdd = new AGXAction() { prt = agPrt.AGPart, ba = PartActionsList.ElementAt(ActionsCount - 1), group = AGXCurActGroup, activated = false };
                                    }

                                    if (!StaticData.CurrentVesselActions.Contains(ToAdd))
                                    {
                                        StaticData.CurrentVesselActions.Add(ToAdd);
                                        ToAdd.ba.listParent.part.Modules.OfType<ModuleAGX>().First().agxActionsThisPart.Add(ToAdd); //add it to list on part, this is our save
                                        ToAdd.ba.listParent.part.Modules.OfType<ModuleAGX>().First().hasData = true;
                                        RefreshCurrentActions();
                                    }
                                    PrtCnt = PrtCnt + 1;
                                    if (ToAdd.group < 11)
                                    {
                                        SetDefaultAction(ToAdd.ba, ToAdd.group);
                                    }
                                }
                            }
                        }
                        ActionsCount = ActionsCount + 1;
                    }
                    GUI.EndScrollView();
                }
                else
                {
                    if (AGEditorSelectedParts.Count >= 1)
                    {
                        if (GUI.Button(new Rect(SelPartsLeft + 30, 190, 185, 40), "No actions found.\r\nRefresh?", UI.AGXBtnStyle))
                        {
                            RefreshPartActions();
                        }
                    }
                }
            }
            else
            {
                TextAnchor TxtAnch = GUI.skin.label.alignment;

                GUI.Label(new Rect(SelPartsLeft + 20, 180, 190, 40), Localizer.Format("#AGEXT_UI_select_part_hint"), UI.AGXLblStyleMiddleCenter);
            }

            if (defaultShowingNonNumeric)
            {
                if (GUI.Button(new Rect(SelPartsLeft + 245, 85, 120, 30), defaultGroupToShow.ToString(), UI.AGXBtnStyleMiddleLeft)) //current action group button
                {
                    //TempShowGroupsWin = true;
                }
            }
            else
            {
                if (GUI.Button(new Rect(SelPartsLeft + 245, 85, 120, 30), AGXCurActGroup + ": " + AGXguiNames[AGXCurActGroup], UI.AGXBtnStyleMiddleLeft)) //current action group button
                {
                    TempShowGroupsWin = true;
                }

                if (IsGroupToggle[AGXCurActGroup])
                {
                    Color TxtClr = GUI.contentColor;
                    GUI.contentColor = Color.green;
                    if (GUI.Button(new Rect(SelPartsLeft + 235, 160, 90, 22), Localizer.Format("#AGEXT_UI_state_vis_yes"), UI.AGXBtnStyle))
                    {
                        IsGroupToggle[AGXCurActGroup] = false;
                    }
                    GUI.contentColor = TxtClr;
                }
                else
                {
                    if (GUI.Button(new Rect(SelPartsLeft + 235, 160, 90, 22), Localizer.Format("#AGEXT_UI_state_vis_no"), UI.AGXBtnStyle))
                    {
                        IsGroupToggle[AGXCurActGroup] = true;
                    }
                }

                if (isDirectAction[AGXCurActGroup])
                {
                    if (GUI.Button(new Rect(SelPartsLeft + 320, 160, 45, 22), Localizer.Format("#AGEXT_UI_hold"), UI.AGXBtnStyleDisabled))
                    {
                        isDirectAction[AGXCurActGroup] = false;
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(SelPartsLeft + 320, 160, 45, 22), Localizer.Format("#AGEXT_UI_tap"), UI.AGXBtnStyle))
                    {
                        isDirectAction[AGXCurActGroup] = true;
                    }
                }

                GUI.Label(new Rect(SelPartsLeft + 231, 183, 110, 22), Localizer.Format("#AGEXT_UI_show"), UI.AGXLblStyle);
                Color TxtClr2 = GUI.contentColor;

                if (ShowGroupInFlight[1, AGXCurActGroup])
                {
                    GUI.contentColor = Color.green;
                }
                else
                {
                    GUI.contentColor = Color.red;
                }

                if (GUI.Button(new Rect(SelPartsLeft + 271, 183, 20, 22), "1", UI.AGXBtnStyle))
                {
                    ShowGroupInFlight[1, AGXCurActGroup] = !ShowGroupInFlight[1, AGXCurActGroup];
                    CalculateActionsToShow();
                }

                if (ShowGroupInFlight[2, AGXCurActGroup])
                {
                    GUI.contentColor = Color.green;
                }
                else
                {
                    GUI.contentColor = Color.red;
                }

                if (GUI.Button(new Rect(SelPartsLeft + 291, 183, 20, 22), "2", UI.AGXBtnStyle))
                {
                    ShowGroupInFlight[2, AGXCurActGroup] = !ShowGroupInFlight[2, AGXCurActGroup];
                    CalculateActionsToShow();
                }

                if (ShowGroupInFlight[3, AGXCurActGroup])
                {
                    GUI.contentColor = Color.green;
                }
                else
                {
                    GUI.contentColor = Color.red;
                }

                if (GUI.Button(new Rect(SelPartsLeft + 311, 183, 20, 22), "3", UI.AGXBtnStyle))
                {
                    ShowGroupInFlight[3, AGXCurActGroup] = !ShowGroupInFlight[3, AGXCurActGroup];
                    CalculateActionsToShow();
                }

                if (ShowGroupInFlight[4, AGXCurActGroup])
                {
                    GUI.contentColor = Color.green;
                }
                else
                {
                    GUI.contentColor = Color.red;
                }

                if (GUI.Button(new Rect(SelPartsLeft + 331, 183, 20, 22), "4", UI.AGXBtnStyle))
                {
                    ShowGroupInFlight[4, AGXCurActGroup] = !ShowGroupInFlight[4, AGXCurActGroup];
                    CalculateActionsToShow();
                }

                if (ShowGroupInFlight[5, AGXCurActGroup])
                {
                    GUI.contentColor = Color.green;
                }
                else
                {
                    GUI.contentColor = Color.red;
                }

                if (GUI.Button(new Rect(SelPartsLeft + 351, 183, 20, 22), "5", UI.AGXBtnStyle))
                {
                    ShowGroupInFlight[5, AGXCurActGroup] = !ShowGroupInFlight[5, AGXCurActGroup];
                    CalculateActionsToShow();
                }

                GUI.contentColor = TxtClr2;
                GUI.Label(new Rect(SelPartsLeft + 245, 115, 110, 20), Localizer.Format("#AGEXT_UI_action_name"), UI.AGXLblStyle);
                CurGroupDesc = AGXguiNames[AGXCurActGroup];
                CurGroupDesc = GUI.TextField(new Rect(SelPartsLeft + 245, 135, 120, 22), CurGroupDesc, UI.AGXFldStyle);
                AGXguiNames[AGXCurActGroup] = CurGroupDesc;
                GUI.Label(new Rect(SelPartsLeft + 245, 203, 110, 25), Localizer.Format("#AGEXT_UI_keybinding"), UI.AGXLblStyle);
                string btnName = "";

                if (AGXguiMod1Groups[AGXCurActGroup] && AGXguiMod2Groups[AGXCurActGroup])
                {
                    btnName = '\u00bd' + AGXguiKeys[AGXCurActGroup].ToString();
                }
                else if (AGXguiMod1Groups[AGXCurActGroup])
                {
                    btnName = '\u2474' + AGXguiKeys[AGXCurActGroup].ToString();
                }
                else if (AGXguiMod2Groups[AGXCurActGroup])
                {
                    btnName = '\u2475' + AGXguiKeys[AGXCurActGroup].ToString();
                }
                else
                {
                    btnName = AGXguiKeys[AGXCurActGroup].ToString();
                }

                if (GUI.Button(new Rect(SelPartsLeft + 245, 222, 120, 20), btnName, UI.AGXBtnStyle))
                {
                    ShowKeyCodeWin = true;
                }
            }

            if (GUI.Button(new Rect(SelPartsLeft + 245, 244, 120, 20), CurrentKeySetNameFlight, UI.AGXBtnStyle))
            {
                SaveCurrentKeyBindings();
                ShowKeySetWin = true;
            }

            GUI.DragWindow();
        }

        public void GroupsWindow(int WindowID)
        {
            bool[] PageGrn = new bool[5];
            foreach (AGXAction AGact in StaticData.CurrentVesselActions)
            {
                if (AGact.group <= 50)
                {
                    PageGrn[0] = true;
                }
                if (AGact.group >= 51 && AGact.group <= 100)
                {
                    PageGrn[1] = true;
                }
                if (AGact.group >= 101 && AGact.group <= 150)
                {
                    PageGrn[2] = true;
                }
                if (AGact.group >= 151 && AGact.group <= 200)
                {
                    PageGrn[3] = true;
                }
                if (AGact.group >= 201 && AGact.group <= 250)
                {
                    PageGrn[4] = true;
                }
            }

            GUIStyle btnStyle = UI.AGXBtnStyle;
            if (AutoHideGroupsWin)
                btnStyle = UI.AGXBtnStyleDisabled;

            if (showCareerCustomAGs)
            {
                if (GUI.Button(new Rect(80, 3, 40, 20), Localizer.Format("#AGEXT_UI_type_other"), btnStyle))
                {
                    defaultShowingNonNumeric = !defaultShowingNonNumeric;
                    if (defaultShowingNonNumeric)
                    {
                        defaultActionsListAll.Clear();
                        {
                            foreach (Part p in FlightGlobals.ActiveVessel.parts)
                            {
                                defaultActionsListAll.AddRange(p.Actions);
                                foreach (PartModule pm in p.Modules)
                                {
                                    defaultActionsListAll.AddRange(pm.Actions);
                                }
                            }
                        }
                        RefreshDefaultActionsListType();
                    }
                }
            }

            if (PageGrn[0])         btnStyle = UI.AGXBtnStyleEnabled;
            if (GroupsPage == 1)    btnStyle = UI.AGXBtnStyleDisabled;
            if (GUI.Button(new Rect(120, 3, 25, 20), "1", btnStyle))
            {
                defaultShowingNonNumeric = false;
                GroupsPage = 1;
            }

            if (PageGrn[1])         btnStyle = UI.AGXBtnStyleEnabled;
            if (GroupsPage == 2)    btnStyle = UI.AGXBtnStyleDisabled;
            if (GUI.Button(new Rect(145, 3, 25, 20), "2", btnStyle))
            {
                defaultShowingNonNumeric = false;
                GroupsPage = 2;
            }

            if (PageGrn[2])         btnStyle = UI.AGXBtnStyleEnabled;
            if (GroupsPage == 3)    btnStyle = UI.AGXBtnStyleDisabled;
            if (GUI.Button(new Rect(170, 3, 25, 20), "3", btnStyle))
            {
                defaultShowingNonNumeric = false;
                GroupsPage = 3;
            }

            if (PageGrn[3])         btnStyle = UI.AGXBtnStyleEnabled;
            if (GroupsPage == 4)    btnStyle = UI.AGXBtnStyleDisabled;
            if (GUI.Button(new Rect(195, 3, 25, 20), "4", btnStyle))
            {
                defaultShowingNonNumeric = false;
                GroupsPage = 4;
            }

            if (PageGrn[4])         btnStyle = UI.AGXBtnStyleEnabled;
            if (GroupsPage == 5)    btnStyle = UI.AGXBtnStyleDisabled;
            if (GUI.Button(new Rect(220, 3, 25, 20), "5", btnStyle))
            {
                defaultShowingNonNumeric = false;
                GroupsPage = 5;
            }

            if (showAllPartsList) //show all parts list is clicked so change to that mode
            {
                ScrollGroups = GUI.BeginScrollView(new Rect(5, 25, 240, 500), ScrollGroups, new Rect(0, 0, 240, Mathf.Max(500, showAllPartsListTitles.Count * 20))); //scroll view just in case there are a lot of parts to list
                int listCount = 1; //track which button we are on in list
                while (listCount <= showAllPartsListTitles.Count) //procedurally generate buttons
                {
                    if (GUI.Button(new Rect(0, (listCount - 1) * 20, 240, 20), showAllPartsListTitles.ElementAt(listCount - 1), UI.AGXBtnStyle)) //button code
                    {
                        string partNameToSelect = showAllPartsListTitles.ElementAt(listCount - 1); //title of part clicked on as string, not a Part object
                        AGEditorSelectedParts.Clear(); //selected parts list should be clear if we are in this mode, but check anyways
                        foreach (Part p in FlightGlobals.ActiveVessel.parts) //add all Parts with matching title to selected parts list, converting from string to Part
                        {
                            if (p.partInfo.title == partNameToSelect)
                            {
                                AGEditorSelectedParts.Add(new AGXPart(p));
                            }
                        }
                        PartActionsList.Clear(); //populate actions list from selected parts
                        PartActionsList.AddRange(AGEditorSelectedParts.First().AGPart.Actions.Where(ba => ba.active));
                        foreach (PartModule pm in AGEditorSelectedParts.First().AGPart.Modules)
                        {
                            PartActionsList.AddRange(pm.Actions.Where(ba => ba.active));
                        }

                        showAllPartsList = false; //exit show all parts mode
                        TempShowGroupsWin = false; //hide window if auto hide enabled
                        AGEEditorSelectedPartsSame = true; //all selected parts are the same type as per the check above
                    }

                    listCount = listCount + 1; //moving to next button
                }

                GUI.EndScrollView();
            }
            else if (defaultShowingNonNumeric)
            {
                if (defaultGroupToShow == KSPActionGroup.Abort)     btnStyle = UI.AGXBtnStyleEnabled;
                else                                                btnStyle = UI.AGXBtnStyle;

                if (GUI.Button(new Rect(5, 25, 58, 20), Localizer.Format("#AGEXT_UI_type_abort"), btnStyle)) //button code
                {
                    defaultGroupToShow = KSPActionGroup.Abort;
                    RefreshDefaultActionsListType();
                }

                if (defaultGroupToShow == KSPActionGroup.Brakes)    btnStyle = UI.AGXBtnStyleEnabled;
                else                                                btnStyle = UI.AGXBtnStyle;

                if (GUI.Button(new Rect(64, 25, 58, 20), Localizer.Format("#AGEXT_UI_type_brakes"), btnStyle)) //button code
                {
                    defaultGroupToShow = KSPActionGroup.Brakes;
                    RefreshDefaultActionsListType();
                }

                if (defaultGroupToShow == KSPActionGroup.Gear)      btnStyle = UI.AGXBtnStyleEnabled;
                else                                                btnStyle = UI.AGXBtnStyle;

                if (GUI.Button(new Rect(122, 25, 59, 20), Localizer.Format("#AGEXT_UI_type_gear"), btnStyle)) //button code
                {
                    defaultGroupToShow = KSPActionGroup.Gear;
                    RefreshDefaultActionsListType();
                }

                if (defaultGroupToShow == KSPActionGroup.Light)     btnStyle = UI.AGXBtnStyleEnabled;
                else                                                btnStyle = UI.AGXBtnStyle;

                if (GUI.Button(new Rect(182, 25, 58, 20), Localizer.Format("#AGEXT_UI_type_lights"), btnStyle)) //button code
                {
                    defaultGroupToShow = KSPActionGroup.Light;
                    RefreshDefaultActionsListType();
                }

                if (defaultGroupToShow == KSPActionGroup.RCS)       btnStyle = UI.AGXBtnStyleEnabled;
                else                                                btnStyle = UI.AGXBtnStyle;

                if (GUI.Button(new Rect(5, 45, 76, 20), Localizer.Format("#AGEXT_UI_type_rcs"), btnStyle)) //button code
                {
                    defaultGroupToShow = KSPActionGroup.RCS;
                    RefreshDefaultActionsListType();
                }

                if (defaultGroupToShow == KSPActionGroup.SAS)       btnStyle = UI.AGXBtnStyleEnabled;
                else                                                btnStyle = UI.AGXBtnStyle;

                if (GUI.Button(new Rect(81, 45, 76, 20), Localizer.Format("#AGEXT_UI_type_sas"), btnStyle)) //button code
                {
                    defaultGroupToShow = KSPActionGroup.SAS;
                    RefreshDefaultActionsListType();
                }

                if (defaultGroupToShow == KSPActionGroup.Stage)     btnStyle = UI.AGXBtnStyleEnabled;
                else                                                btnStyle = UI.AGXBtnStyle;

                if (GUI.Button(new Rect(157, 45, 76, 20), Localizer.Format("#AGEXT_UI_type_stage"), btnStyle)) //button code
                {
                    defaultGroupToShow = KSPActionGroup.Stage;
                    RefreshDefaultActionsListType();
                }

                //add vector2
                groupWinScroll = GUI.BeginScrollView(new Rect(5, 70, 240, 455), groupWinScroll, new Rect(0, 0, 240, Mathf.Max(455, defaultActionsListThisType.Count * 20)));
                int listCount2 = 1;
                highlightPartThisFrameGroupWin = false;
                while (listCount2 <= defaultActionsListThisType.Count)
                {
                    if (Mouse.screenPos.y >= GroupsWin.y + 70 && Mouse.screenPos.y <= GroupsWin.y + 525 && new Rect(GroupsWin.x + 5, (GroupsWin.y + 70 + ((listCount2 - 1) * 20)) - groupWinScroll.y, 240, 20).Contains(Mouse.screenPos))
                    {
                        highlightPartThisFrameGroupWin = true;
                        //Log.dbg("part found to highlight acts " + highlightPartThisFrameActsWin + " " + ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.transform.position);
                        partToHighlight = defaultActionsListThisType.ElementAt(listCount2 - 1).listParent.part;
                    }

                    if (GUI.Button(new Rect(0, (listCount2 - 1) * 20, 240, 20), defaultActionsListThisType.ElementAt(listCount2 - 1).listParent.part.partInfo.title + " " + defaultActionsListThisType.ElementAt(listCount2 - 1).guiName, UI.AGXBtnStyleMiddleLeft)) //button code
                    {
                        defaultActionsListThisType.ElementAt(listCount2 - 1).actionGroup = defaultActionsListThisType.ElementAt(listCount2 - 1).actionGroup & ~defaultGroupToShow;
                        RefreshDefaultActionsListType();
                    }
                    listCount2 = listCount2 + 1;
                }
                GUI.EndScrollView();
            }
            else
            {
                ScrollGroups = GUI.BeginScrollView(new Rect(5, 25, 240, 500), ScrollPosSelParts, new Rect(0, 0, 240, 500));

                int ButtonID = 1 + (50 * (GroupsPage - 1));
                int ButtonPos = 1;
                while (ButtonPos <= 25)
                {
                    if (ShowKeySetWin)
                    {
                        string btnName = "";
                        if (AGXguiMod1Groups[ButtonID] && AGXguiMod2Groups[ButtonID])
                        {
                            btnName = '\u00bd' + AGXguiKeys[ButtonID].ToString();
                        }
                        else if (AGXguiMod1Groups[ButtonID])
                        {
                            btnName = '\u2474' + AGXguiKeys[ButtonID].ToString();
                        }
                        else if (AGXguiMod2Groups[ButtonID])
                        {
                            btnName = '\u2475' + AGXguiKeys[ButtonID].ToString();
                        }
                        else
                        {
                            btnName = AGXguiKeys[ButtonID].ToString();
                        }
                        if (GUI.Button(new Rect(0, (ButtonPos - 1) * 20, 120, 20), ButtonID + " Key: " + btnName, UI.AGXBtnStyleMiddleLeft))
                        {

                            AGXCurActGroup = ButtonID;
                            ShowKeyCodeWin = true;
                        }
                    }
                    else
                    {
                        if (StaticData.CurrentVesselActions.Any(pfd => pfd.group == ButtonID)) btnStyle = UI.AGXBtnStyleMiddleLeftActive;
                        else                                                                   btnStyle = UI.AGXBtnStyleMiddleLeft;

                        if (GUI.Button(new Rect(0, (ButtonPos - 1) * 20, 120, 20), ButtonID + ": " + AGXguiNames[ButtonID], btnStyle))
                        {
                            AGXCurActGroup = ButtonID;
                            TempShowGroupsWin = false;
                        }
                    }
                    ButtonPos = ButtonPos + 1;
                    ButtonID = ButtonID + 1;
                }
                while (ButtonPos <= 50)
                {
                    if (ShowKeySetWin)
                    {
                        string btnName2 = "";
                        if (AGXguiMod1Groups[ButtonID] && AGXguiMod2Groups[ButtonID])
                        {
                            btnName2 = '\u00bd' + AGXguiKeys[ButtonID].ToString();
                        }
                        else if (AGXguiMod1Groups[ButtonID])
                        {
                            btnName2 = '\u2474' + AGXguiKeys[ButtonID].ToString();
                        }
                        else if (AGXguiMod2Groups[ButtonID])
                        {
                            btnName2 = '\u2475' + AGXguiKeys[ButtonID].ToString();
                        }
                        else
                        {
                            btnName2 = AGXguiKeys[ButtonID].ToString();
                        }
                        if (GUI.Button(new Rect(120, (ButtonPos - 26) * 20, 120, 20), ButtonID + " Key: " + btnName2, UI.AGXBtnStyleMiddleLeft))
                        {
                            AGXCurActGroup = ButtonID;
                            ShowKeyCodeWin = true;
                        }
                    }
                    else
                    {
                        if (StaticData.CurrentVesselActions.Any(pfd => pfd.group == ButtonID))  btnStyle = UI.AGXBtnStyleMiddleLeftActive;
                        else                                                                    btnStyle = UI.AGXBtnStyleMiddleLeft;

                        if (GUI.Button(new Rect(120, (ButtonPos - 26) * 20, 120, 20), ButtonID + ": " + AGXguiNames[ButtonID], btnStyle))
                        {


                            AGXCurActGroup = ButtonID;
                            TempShowGroupsWin = false;

                        }
                    }
                    ButtonPos = ButtonPos + 1;
                    ButtonID = ButtonID + 1;
                }

                GUI.EndScrollView();

            }
            GUI.DragWindow();
        }

        public List<AGXPart> AGXAddSelectedPart(Part p, bool Sym)
        {
            List<Part> ToAdd = new List<Part>();
            List<AGXPart> RetLst = new List<AGXPart>();
            ToAdd.Add(p);
            if (Sym)
            {
                ToAdd.AddRange(p.symmetryCounterparts);
            }
            foreach (Part prt in ToAdd)
            {
                if (!AGEditorSelectedParts.Any(prt2 => prt2.AGPart == prt))
                {
                    RetLst.Add(new AGXPart(prt));
                }
            }
            AGEEditorSelectedPartsSame = true;
            foreach (AGXPart aprt in RetLst)
            {
                if (aprt.AGPart.partInfo.title != RetLst.First().AGPart.partInfo.title)
                {
                    AGEEditorSelectedPartsSame = false;
                }
            }
            if (AGEEditorSelectedPartsSame)
            {
                PartActionsList.Clear();
                PartActionsList.AddRange(p.Actions.Where(ba => ba.active));
                foreach (PartModule pm in p.Modules)
                {
                    PartActionsList.AddRange(pm.Actions.Where(ba => ba.active));
                }

            }
            return RetLst;

        }
        public void LoadGroupVisibility(string stringToLoad)
        {
            string errLine = "1";
            try
            {
                string LoadString = stringToLoad;// (string)pm.Fields.GetValue("AGXGroupStates");
                // print("AGXTogLoad" + pm.part.ConstructID + " " + LoadString);
                if (LoadString.Length == 1501)
                {
                    errLine = "2";
                    ShowGroupInFlightCurrent = Convert.ToInt32(LoadString.Substring(0, 1));
                    LoadString = LoadString.Substring(1);

                    for (int i = 1; i <= 250; i++)
                    {
                        errLine = "3";
                        if (LoadString[0] == '1')
                        {
                            IsGroupToggle[i] = true;
                        }
                        else
                        {
                            IsGroupToggle[i] = false;
                        }
                        errLine = "4";
                        LoadString = LoadString.Substring(1);
                        for (int i2 = 1; i2 <= 5; i2++)
                        {
                            errLine = "5";
                            if (LoadString[0] == '1')
                            {
                                errLine = "6";
                                ShowGroupInFlight[i2, i] = true;
                            }
                            else
                            {
                                errLine = "7";
                                ShowGroupInFlight[i2, i] = false;
                            }
                            LoadString = LoadString.Substring(1);
                        }
                    }
                }
                else
                {
                    errLine = "8";
                    ShowGroupInFlightCurrent = 1;
                    for (int i = 1; i <= 250; i++)
                    {
                        errLine = "9";
                        IsGroupToggle[i] = false;
                        for (int i2 = 1; i2 <= 5; i2++)
                        {
                            ShowGroupInFlight[i2, i] = true;
                        }
                    }
                }
                errLine = "10";
                CalculateActionsToShow();
                // print("AGXTogLoadFin: " + IsGroupToggle[1] + IsGroupToggle[2] + IsGroupToggle[3] + IsGroupToggle[4] + IsGroupToggle[5] + IsGroupToggle[6] + IsGroupToggle[7] + IsGroupToggle[8] + IsGroupToggle[9] + IsGroupToggle[10]);
            }
            catch (Exception e)
            {
                print("AGX LoadGroupVisibility Fail! " + errLine + " " + e);
                ShowGroupInFlightCurrent = 1;
                for (int i = 1; i <= 250; i++)
                {
                    IsGroupToggle[i] = false;
                    for (int i2 = 1; i2 <= 5; i2++)
                    {
                        ShowGroupInFlight[i2, i] = true;
                    }
                }
            }
        }

        public static string GroupNamesDictToString(Dictionary<int, string> namesDict)
        {

            string errStep = "3";
            try
            {

                string SaveStringNames = "";
                errStep = "5";
                errStep = "6";
                int GroupCnt = 1;
                errStep = "7";
                while (GroupCnt <= 250)
                {
                    errStep = "8";
                    if (namesDict[GroupCnt].Length >= 1)
                    {
                        errStep = "9";
                        SaveStringNames = SaveStringNames + '\u2023' + GroupCnt.ToString("000") + namesDict[GroupCnt];
                        errStep = "10";
                    }
                    errStep = "11";
                    GroupCnt = GroupCnt + 1;
                    errStep = "12";
                }
                errStep = "13";

                //Log.dbg(p.partName + " " + SaveStringNames);
                //Log.dbg("Savegroup return " + SaveStringNames);
                return SaveStringNames;
            }
            catch (Exception e)
            {
                Log.err("GroupsNamesToString FAIL {0}", errStep);
                Log.ex(typeof(AGXFlight), e);
                return "";
            }
        }

        public static Dictionary<int, string> GroupNamesStringToDict(string str)
        {
            //string errLine = "1";
            Dictionary<int, string> DictToReturn = new Dictionary<int, string>();
            for (int i = 1; i <= 250; i++)
            {
                DictToReturn[i] = "";
            }
            string LoadNames = str;

            if (LoadNames.Length > 0)
            {
                //errLine = "4";
                while (LoadNames[0] == '\u2023')
                {
                    //errLine = "5";
                    string groupName = "";
                    LoadNames = LoadNames.Substring(1);
                    int groupNum = Convert.ToInt32(LoadNames.Substring(0, 3));
                    LoadNames = LoadNames.Substring(3);
                    //errLine = "6";
                    if (LoadNames.IndexOf('\u2023') == -1)
                    {
                        //errLine = "7";
                        groupName = LoadNames;
                    }
                    else
                    {
                        //errLine = "8";
                        groupName = LoadNames.Substring(0, LoadNames.IndexOf('\u2023'));
                        LoadNames = LoadNames.Substring(LoadNames.IndexOf('\u2023'));
                    }
                    DictToReturn[groupNum] = groupName;
                }
            }
            return DictToReturn;

        }

        public void LoadGroupNames()
        {
            string errLine = "1";
            try
            {
                for (int i = 1; i <= 250; i = i + 1) //always sreset all goups
                {
                    AGXguiNames[i] = "";
                }
                errLine = "3";
                foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                {
                    if (p.Modules.Contains("ModuleAGX"))
                    {
                        ModuleAGX agxPM = (ModuleAGX)p.Modules["ModuleAGX"];
                        if (agxPM.focusFlightID == 0)
                        {
                            agxPM.focusFlightID = FlightGlobals.ActiveVessel.rootPart.missionID; //error trap first load in flight mode, this will still be 0 so set it to the root part of the ship. 99% of the time this will be on launchpad
                        }

                        //Log.dbg(groupName + " || " + AGXguiNames[groupNum] + " " + groupNum);
                        if (p.missionID == currentMissionId) //missionID matchs, group names on this part have priority
                        {
                            AGXguiNames = GroupNamesStringToDict(agxPM.groupNames);
                        }
                        else //if (AGXguiNames[groupNum].Length < 1) //missionID no match, only populate if groupname is blank
                        {
                            //Log.dbg("Add name in");
                            Dictionary<int, string> tempNames = GroupNamesStringToDict(agxPM.groupNames);
                            for (int i = 1; i <= 250; i++)
                            {
                                if (tempNames[i].Length > 0 && AGXguiNames[i].Length < 1)
                                {
                                    AGXguiNames[i] = tempNames[i];
                                }
                            }
                        }
                    }

                }
                // }
                // }



            }
            catch (Exception e)
            {
                Log.err("LoadGroupNamesFail {0}", errLine);
                Log.ex(this, e);
            }
        }

        public void LoadGroupNamesOld(string namesToLoad, bool doReset)
        {
            string errLine = "1";
            try
            {
                if (doReset)
                {
                    errLine = "2";
                    for (int i = 1; i <= 250; i = i + 1)
                    {
                        AGXguiNames[i] = "";
                    }
                }
                errLine = "3";

                string LoadNames = namesToLoad;

                if (LoadNames.Length > 0) //also update PartVesselCheck method
                {
                    errLine = "4";
                    while (LoadNames[0] == '\u2023')
                    {
                        errLine = "5";
                        string groupName = "";
                        LoadNames = LoadNames.Substring(1);
                        int groupNum = Convert.ToInt32(LoadNames.Substring(0, 3));
                        LoadNames = LoadNames.Substring(3);
                        errLine = "6";
                        if (LoadNames.IndexOf('\u2023') == -1)
                        {
                            errLine = "7";
                            groupName = LoadNames;
                        }
                        else
                        {
                            errLine = "8";
                            groupName = LoadNames.Substring(0, LoadNames.IndexOf('\u2023'));
                            LoadNames = LoadNames.Substring(LoadNames.IndexOf('\u2023'));
                        }
                        errLine = "9";
                        //Log.dbg(groupName + " || " + AGXguiNames[groupNum] + " " + groupNum);
                        if (AGXguiNames[groupNum].Length < 1)
                        {
                            //Log.dbg("Add name in");
                            AGXguiNames[groupNum] = groupName;
                        }

                    }
                }

            }
            catch (Exception e)
            {
                Log.err("LoadGroupNamesFail {0}", errLine);
                Log.ex(this, e);
            }
        }


        public void LoadDefaultActionGroups()
        {
            List<KSPActionGroup> CustomActions = CONST.CustomActions.Values.ToList();

            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                string AddGroup = "";
                foreach (PartModule pm in p.Modules)
                {
                    foreach (BaseAction ba in pm.Actions)
                    {
                        foreach (KSPActionGroup agrp in CustomActions)
                        {

                            if ((ba.actionGroup & agrp) == agrp)
                            {
                                AddGroup = AddGroup + '\u2023' + (CustomActions.IndexOf(agrp) + 1).ToString("000") + ba.guiName;
                            }
                        }
                    }
                }
            }
        }

        public Part SelectPartUnderMouse()
        {
            return Mouse.HoveredPart;
        }

        public bool CheckMouseOver()
        {
            Vector3 CurrentMousePosition = new Vector3(); //only add the part if not over a UI window
            CurrentMousePosition = Input.mousePosition;
            CurrentMousePosition.y = Screen.height - Input.mousePosition.y;
            if (ShowAGXFlightWin && FlightWin.Contains(CurrentMousePosition))
            {
                return true;
            }
            else if (ShowGroupsInFlightWindow && GroupsInFlightWin.Contains(CurrentMousePosition))
            {
                return true;
            }
            else if (ShowAGXFlightWin && RTWinShow && RemoteTechQueueWin.Contains(CurrentMousePosition))
            {
                return true;
            }
            else if (ShowKeySetWin && KeySetWin.Contains(CurrentMousePosition))
            {
                return true;
            }
            else if (ShowKeySetWin && !AutoHideGroupsWin && GroupsWin.Contains(CurrentMousePosition))
            {
                return true;
            }
            else if (ShowSelectedWin && SelPartsWin.Contains(CurrentMousePosition))
            {
                return true;
            }
            else if (ShowSelectedWin && AutoHideGroupsWin && TempShowGroupsWin && GroupsWin.Contains(CurrentMousePosition))
            {
                return true;
            }
            else if (ShowSelectedWin && !AutoHideGroupsWin && GroupsWin.Contains(CurrentMousePosition))
            {
                return true;
            }
            else if (ShowSelectedWin && ShowKeySetWin && KeyCodeWin.Contains(CurrentMousePosition))
            {
                return true;
            }
            else if (ShowCurActsWin && ShowSelectedWin && CurActsWin.Contains(CurrentMousePosition))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void AddSelectedPart(Part p)
        {

            //if(() &&  &&  &&  &&  &&  &&  &&  && )
            //Log.Info("Mouse " + CurrentMousePosition + " " + FlightWin);
            if (!CheckMouseOver())
            {
                if (!AGEditorSelectedParts.Any(prt => prt.AGPart == p))
                {
                    if (AGEditorSelectedParts.Count == 0)
                    {
                        AGEditorSelectedParts.AddRange(AGXAddSelectedPart(p, SelPartsIncSym));
                    }
                    else if (AGEditorSelectedParts.First().AGPart.name == p.name)
                    {
                        AGEditorSelectedParts.AddRange(AGXAddSelectedPart(p, SelPartsIncSym));
                    }
                    else
                    {
                        AGEditorSelectedParts.Clear();
                        AGEditorSelectedParts.AddRange(AGXAddSelectedPart(p, SelPartsIncSym));
                    }
                }
            }
        }

        public static bool VesselIsControlled() //check if focus vessel is controllable, use lockmask as Squad sets that when a vessel isnt/.
        {
            Log.detail("TEST {0}", InputLockManager.IsLocked(ControlTypes.GROUP_LIGHTS));
            if (InputLockManager.lockStack.ContainsKey("vessel_noControl_" + FlightGlobals.ActiveVessel.id.ToString()) && InputLockManager.IsLocked(ControlTypes.GROUP_LIGHTS))
            {
                Log.detail("Not controllable");
                return false; //if value is present in lockstack, controls are locked so not controllable
            }
            else
            {
                Log.detail("Controllable");
                return true; //vessel is controlled, activate action
            }
        }

        public void Update()
        {
            string errLine = "1";
            try
            {
                bool RootPartExists = new bool();
                errLine = "2";
                try
                {
                    errLine = "3";
                    if (FlightGlobals.ActiveVessel.parts.Count > 0) //we are actually checking null here on teh try-catch block, the if statement is a dummy
                    {
                        errLine = "4";
                    }
                    errLine = "5";
                    RootPartExists = true;
                }
                catch
                {
                    errLine = "6";
                    RootPartExists = false;
                }
                errLine = "7";

                if (RootPartExists) //we have a root part so proceed
                {
                    errLine = "7a";
                    if (AGXRoot != FlightGlobals.ActiveVessel.rootPart || LastPartCount != FlightGlobals.ActiveVessel.parts.Count) //root part change or part count change, refresh
                    {
                        errLine = "7b";
                        //try saving just in case, see note below though
                        try
                        {
                            if (AGXRoot != null)
                            {
                                errLine = "7c";
                                SaveShipSpecificData(AGXRoot.vessel);
                            }
                            else
                            {
                                // Log.Info("Update save, root null");
                            }
                        }
                        catch (Exception e)
                        {
                            Log.err("Update save fail {0}", e);
                        }
                        errLine = "7d";
                        //note we generally do not save data here, all saving of data is done by the GUI buttons to the ModuleAGX partmodule directly in flight mode
                        StartCoroutine("GUIDelayCoroutine");
                    }
                } //if(RootPartExists) closing bracket
                errLine = "8";
                Log.trace("Testd");
                if (InputLockManager.GetControlLock("kOSTerminal") == ControlTypes.None && (ControlTypes.KSC_ALL & (ControlTypes)InputLockManager.lockMask) == 0)//.KSC_ALL catches both vessel not controllable via lock Squad sets as well as if ControlLock (my other mod) is engaged
                {
                    //.Log("AGX Test");
                    foreach (KeyCode KC in ActiveKeys)
                    {

                        errLine = "37";
                        if (Input.GetKeyDown(KC))
                        {
                            //Log.dbg("AGX keydown " + KC + "|");
                            //InputLockManager.SetControlLock(2,"test");
                            for (int i = 1; i <= 250; i = i + 1)
                            {
                                if (AGXguiKeys[i] == KC)
                                {
                                    if (VesselIsControlled()) //in version 1.34c this will always be true if hit, used for mouse clicks on the GUI at this point, leave for future though
                                    {
                                        ActivateActionGroupCheckModKeys(i);
                                    }
                                }
                            }
                        }
                    }

                    foreach (KeyValuePair<int, KeyCode> kcPair in ActiveKeysDirect)
                    {
                        if (Input.GetKey(kcPair.Value) && !DirectKeysState[kcPair.Key])
                        {
                            if (VesselIsControlled())
                            {
                                ActivateActionGroupCheckModKeys(kcPair.Key, true, true);
                                DirectKeysState[kcPair.Key] = true;
                                Log.trace("turn on");
                            }
                        }
                        else if (!Input.GetKey(kcPair.Value) && DirectKeysState[kcPair.Key])
                        {
                            if (VesselIsControlled())
                            {
                                ActivateActionGroupCheckModKeys(kcPair.Key, true, false);
                                DirectKeysState[kcPair.Key] = false;
                                Log.trace("turn off");
                            }
                        }
                    }
                    foreach (KeyValuePair<int, KeyCode> kcPair2 in DefaultTen) //toggle groups if no actions are assigned
                    {
                        if (Input.GetKeyDown(kcPair2.Value))
                        {
                            if (AGXguiMod1Groups[kcPair2.Key] == Input.GetKey(AGXguiMod1Key) && AGXguiMod2Groups[kcPair2.Key] == Input.GetKey(AGXguiMod2Key))
                            {
                                if (VesselIsControlled())
                                {
                                    if (kcPair2.Key <= 10)
                                    {
                                        FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(CONST.CustomActions[kcPair2.Key]);
                                        groupActivatedState[kcPair2.Key] = FlightGlobals.ActiveVessel.ActionGroups[CONST.CustomActions[kcPair2.Key]];
                                    }
                                    else
                                    {
                                        groupActivatedState[kcPair2.Key] = !groupActivatedState[kcPair2.Key];
                                    }
                                }
                            }
                        }
                    }
                }
                errLine = "38";
                if (Input.GetKeyDown(KeyCode.Mouse0) && ShowSelectedWin)
                {
                    errLine = "39";
                    Part selPart = new Part();
                    selPart = SelectPartUnderMouse();
                    if (selPart != null)
                    {
                        AddSelectedPart(selPart);
                    }
                    errLine = "40";
                }
                errLine = "41";
                if (RightClickDelay < 3)
                {
                    errLine = "42";
                    if (RightClickDelay == 2)
                    {
                        errLine = "43";
                        UIPartActionWindow UIPartsListThing = new UIPartActionWindow();
                        UIPartsListThing = (UIPartActionWindow)FindObjectOfType(typeof(UIPartActionWindow));
                        try
                        {
                            if (UIPartsListThing != null)
                            {
                                AddSelectedPart(UIPartsListThing.part);
                            }
                            RightLickPartAdded = true;
                        }
                        catch
                        {
                            RightLickPartAdded = true;
                        }
                    }

                    RightClickDelay = RightClickDelay + 1;

                    errLine = "44";
                }
                errLine = "45";

                if (Input.GetKeyUp(KeyCode.Mouse1) && ShowSelectedWin && RightLickPartAdded)
                {
                    RightClickDelay = 0;
                    RightLickPartAdded = false;

                }
                errLine = "46";
                if (ShowAGXMod)
                {
                    if (actionsCheckFrameCount >= 15) //this increments in the FixedUpdate frame now
                    {
                        CheckActionsActive();
                        actionsCheckFrameCount = 0;
                    }
                }
                errLine = "47";
                groupCooldowns.RemoveAll(cd => cd.delayLeft > activationCoolDown); //remove actions from list that are finished cooldown, cooldown is in Update frame passes, pulled from .cfg
                foreach (AGXCooldown agCD in groupCooldowns)
                {
                    agCD.delayLeft = agCD.delayLeft + 1;

                }
                errLine = "48";
                if (RTFound)
                {
                    CheckRTQueue();
                }
                Log.detail("TEst {0}|{1}|{2}", InputLockManager.IsLocked(ControlTypes.ALL_SHIP_CONTROLS), InputLockManager.IsUnlocked(ControlTypes.ALL_SHIP_CONTROLS), InputLockManager.IsAllLocked(ControlTypes.ALL_SHIP_CONTROLS) );
            } //public void Update() try close bracket
            catch (Exception e)
            {
                Log.err("Flight Update Error {0}", errLine);
                Log.ex(this, e);
            }
        }

        // Used implicitly!!
        IEnumerator GUIDelayCoroutine()
        {
            Log.trace("Coroutine fire");
            int i = 0;
            while (i < 4)
            {
                i = i + 1;
                yield return null;
            }
            LoadGUIDataAfterDelay();
        }

        public void LoadGUIDataAfterDelay()
        {
            Log.trace("LoadUI call");
            string errLine = "1";
            try
            {
                ModuleAGX rootAGX = null;

                Log.detail("{0}", FlightGlobals.ActiveVessel.vesselType);
                if (FlightGlobals.ActiveVessel.rootPart.Modules.Contains("KerbalEVA") || FlightGlobals.ActiveVessel.vesselType == VesselType.Flag) //kerbals have no actions so...
                {
                    Log.trace("oddball");
                    foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                    {
                        if (!p.Modules.Contains("ModuleAGX"))
                        {
                            Log.detail("AGXModule being added");
                            rootAGX = (ModuleAGX)p.AddModule("ModuleAGX");
                        }
                    }
                    rootAGX = (ModuleAGX)FlightGlobals.ActiveVessel.rootPart.Modules["ModuleAGX"];
                    Log.trace("EvA");
                }
                else
                {
                    if (FlightGlobals.ActiveVessel.rootPart.Modules.Contains("ModuleAGX")) //get our root part module
                    {
                        errLine = "7e";
                        rootAGX = FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGX>().First();
                        // Log.Info("root module found");
                    }
                    else
                    {
                        errLine = "7f";
                        rootAGX = new ModuleAGX();
                        //  Log.Info("trap new root module");
                    }

                    if (!rootAGX.hasData)//make sure our moduleAGX has data, rare but can happen on docking ships launched before installing AGX
                    {
                        // Log.Info("no datat");
                        errLine = "7g";
                        // Log.Info("active vessel part count " + FlightGlobals.ActiveVessel.Parts.Count);
                        foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                        {
                            // Log.Info("active vessel part count2 " + FlightGlobals.ActiveVessel.Parts.Count);
                            errLine = "7g1";
                            if (p.Modules.OfType<ModuleAGX>().FirstOrDefault().hasData) //.hasData is false on Default so this works
                            {
                                //   Log.Info("active vessel part count3 " + FlightGlobals.ActiveVessel.Parts.Count);
                                errLine = "7g2";
                                rootAGX = p.Modules.OfType<ModuleAGX>().First();
                            }
                            if (rootAGX.hasData)
                            {
                                errLine = "7g3";
                                break; //valid data found, break the forEach
                            }
                        }
                        if (FlightGlobals.ActiveVessel.rootPart.Modules.Contains("ModuleAGX")) //no modules with data, return to root 
                        {
                            errLine = "7g4";
                            rootAGX = FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGX>().First();
                            // Log.Info("root module found");
                        }
                    }
                }
                errLine = "7h";

                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    errLine = "7h9b";
                    currentMissionId = 1; //we are a kerbal (or somethings screwy), just set this to 1 so it never matches. 0 is default so that might match something
                }
                else
                {
                    errLine = "7h9c";
                    if (rootAGX.focusFlightID == 0) //check we have a master vesel assigned, this will trigger on launching a new vessel, etc.
                    {
                        errLine = "7h10";
                        rootAGX.focusFlightID = rootAGX.vessel.rootPart.missionID;
                        currentMissionId = rootAGX.vessel.rootPart.missionID;
                    }
                    else if (rootAGX.focusFlightID == rootAGX.part.missionID)
                    {
                        errLine = "7h11";
                        //  Log.Info("agx root found");
                        //do nothing, rootAGX is currently set to the root part's ModuleAGX
                    }
                    else //check other parts for their mission id and change to that ModuleAGX if it matchs
                    {
                        errLine = "7h12";
                        //string errLine = "7h12";
                        ModuleAGX moduleReturn = null;
                        HashSet<uint> missionIDs = new HashSet<uint>();
                        errLine = "7h13";
                        foreach (Part p in rootAGX.vessel.parts)
                        {
                            errLine = "7h14";
                            if (!p.Modules.Contains("KerbalEVA"))
                            {
                                errLine = "7h14a";
                                missionIDs.Add(p.missionID);
                            }
                        }
                        //   Log.Info("agx not root " + missionIDs.Count);
                        errLine = "7h15";
                        if (missionIDs.Contains(rootAGX.focusFlightID))
                        {
                            errLine = "7h16";
                            currentMissionId = rootAGX.focusFlightID;
                            foreach (Part p in rootAGX.vessel.parts)
                            {
                                errLine = "7h17";
                                if (p.missionID == currentMissionId)
                                {
                                    errLine = "7h18";
                                    moduleReturn = p.Modules.OfType<ModuleAGX>().First();
                                    break; //stop the foreach, found what we needed
                                }
                            }
                        }
                        else //vessel does not conatain the missionID in rootAGX.focusFlightID for somereason, use root part
                        {
                            errLine = "7h19";
                            currentMissionId = rootAGX.part.missionID;
                        }
                    }
                }

                errLine = "7i";
                if (currentMissionId != 1) //if currentMissionID = 1, we are either a Kerbal on EVA, or loading has screwed up and rootAGX is not valid so don't load
                {
                    LoadVesselDataFromPM(rootAGX);
                }
                else
                {
                    WipeVesselData();
                }
                AGXRoot = FlightGlobals.ActiveVessel.rootPart;
                LastPartCount = FlightGlobals.ActiveVessel.parts.Count;
                SaveShipSpecificData(FlightGlobals.ActiveVessel);
                if (currentMissionId > 1)
                {
                    foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                    {
                        if (p.Modules.Contains("ModuleAGX"))
                        {
                            p.Modules["ModuleAGX"].Fields.SetValue("missionID", (float)currentMissionId);
                        }
                    }
                }
                errLine = "7s";
            }
            catch (Exception e)
            {
                Log.err("Flight Load Vessel GUI Delay error {0}", errLine);
                Log.ex(this, e);
            }
        }

        public void CheckRTQueue()
        {
            string errLine = "1";
            try
            {
                foreach (AGXRemoteTechQueueItem rtItem in AGXRemoteTechQueue)
                {
                    try
                    {
                        if (rtItem.state == AGXRemoteTechItemState.COUNTDOWN && rtItem.timeToActivate < Planetarium.GetUniversalTime())
                        {
                            if (rtItem.vsl == FlightGlobals.ActiveVessel)
                            {
                                ActivateActionGroupActivation(rtItem.group, rtItem.forcing, rtItem.forceDir);
                                rtItem.state = AGXRemoteTechItemState.GOOD;
                            }
                            else
                            {
                                if (rtItem.vsl.loaded)
                                {
                                    AGXOtherVessel otherVsl = new AGXOtherVessel(rtItem.vsl.rootPart.flightID);
                                    otherVsl.ActivateActionGroupActivation(rtItem.group, rtItem.forcing, rtItem.forceDir);
                                    rtItem.state = AGXRemoteTechItemState.GOOD;
                                }
                                else
                                {
                                    rtItem.state = AGXRemoteTechItemState.FAILED;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.err("RTQueue Fail&Recover {0}", errLine);
                        Log.ex(this, e);
                        rtItem.state = AGXRemoteTechItemState.FAILED;
                    }
                }
                AGXRemoteTechQueue.RemoveAll(itm => itm.timeToActivate + 3 < Planetarium.GetUniversalTime());
            }
            catch (Exception e)
            {
                Log.info("RTQueue Fail " + errLine + " " + e);
            }
        }

        public void FixedUpdate()
        {
            if (ShowAGXMod)
            {
                actionsCheckFrameCount = actionsCheckFrameCount + 1; //this affects actions on vessel, limit how often we check toggle states
            }
        }

        public void partDead(Part p)
        {
            StaticData.CurrentVesselActions.RemoveAll(act => act.ba.listParent.part == p);
            RefreshCurrentActions();

            if (AGXFlightNode.HasNode(p.flightID.ToString()))
            {
                AGXFlightNode.RemoveNode(p.flightID.ToString());
            }
        }

        public void DockingEventggg(GameEvents.HostTargetAction<Part, Part> htAct) //docking event happend, merge two vessel actions //never called, ggg voids it
        {
            //Log.dbg("undock " + htAct.host.vessel.rootPart.ConstructID + " " + htAct.target.vessel.rootPart.ConstructID);
            try
            {
                // print("Docking event!");
                if (loadFinished)
                {
                    Vessel vsl1 = htAct.host.vessel;
                    Vessel vsl2 = htAct.target.vessel;
                    //Log.dbg(vsl1.id + " " + vsl2.id);
                    if (vsl1 != vsl2) //check to make sure this is not the same vessel docking to itself somehow, both vsl1 and vsl2 would be FG.AC then.
                    {
                        Log.trace("Old Docking event!");
                        if (vsl1 == FlightGlobals.ActiveVessel || vsl2 == FlightGlobals.ActiveVessel) //check to make sure at least one vessel is FG.AC  Not sure how a docking event could happen when neither vessel is active but make sure
                        {
                            if (AGXFlightNode.HasNode(vsl1.rootPart.flightID.ToString()))
                            {
                                // print("vsl1 found");
                                ConfigNode vsl1Node = AGXFlightNode.GetNode(vsl1.rootPart.flightID.ToString());

                                foreach (ConfigNode prtNode in vsl1Node.nodes)
                                {
                                    Vector3 partLoc = new Vector3((float)Convert.ToDouble(prtNode.GetValue("relLocX")), (float)Convert.ToDouble(prtNode.GetValue("relLocY")), (float)Convert.ToDouble(prtNode.GetValue("relLocZ")));
                                    float partDist = 100f;
                                    Part gamePart = new Part();
                                    foreach (Part p in vsl1.parts) //do a distance compare check, floats do not guarantee perfect decimal accuray so use part with least distance, should be zero distance in most cases
                                    {
                                        float thisPartDist = Vector3.Distance(partLoc, vsl1.rootPart.transform.InverseTransformPoint(p.transform.position));
                                        if (thisPartDist < partDist)
                                        {
                                            gamePart = p;
                                            partDist = thisPartDist;
                                        }
                                    }
                                    bool ShowAmbiguousMessage = true;
                                    if (partDist < 0.3f) //do not show it if part found is more then 0.3meters off
                                    {
                                        ShowAmbiguousMessage = true;
                                    }
                                    else
                                    {
                                        ShowAmbiguousMessage = false;
                                    }
                                    foreach (ConfigNode actNode in prtNode.nodes)
                                    {
                                        //Log.dbg("node " + actNode + " " + gamePart.ConstructID);
                                        AGXAction actToAdd = StaticData.LoadAGXActionVer2(actNode, gamePart, ShowAmbiguousMessage);
                                        //Log.dbg("act to add " + actToAdd.ba);
                                        if (actToAdd != null)
                                        {
                                            List<AGXAction> Checking = new List<AGXAction>();
                                            Checking.AddRange(StaticData.CurrentVesselActions);
                                            Checking.RemoveAll(p => p.group != actToAdd.group);
                                            Checking.RemoveAll(p => p.prt != actToAdd.prt);
                                            Checking.RemoveAll(p => p.ba != actToAdd.ba);

                                            if (Checking.Count == 0)
                                            {
                                                StaticData.CurrentVesselActions.Add(actToAdd);
                                            }
                                        }
                                    }
                                }
                            }

                            if (AGXFlightNode.HasNode(vsl2.rootPart.flightID.ToString()))
                            {
                                //Log.dbg("vsl2 found");
                                ConfigNode vsl2Node = AGXFlightNode.GetNode(vsl2.rootPart.flightID.ToString());

                                foreach (ConfigNode prtNode in vsl2Node.nodes)
                                {
                                    Vector3 partLoc = new Vector3((float)Convert.ToDouble(prtNode.GetValue("relLocX")), (float)Convert.ToDouble(prtNode.GetValue("relLocY")), (float)Convert.ToDouble(prtNode.GetValue("relLocZ")));
                                    float partDist = 100f;
                                    Part gamePart = new Part();
                                    foreach (Part p in vsl2.parts) //do a distance compare check, floats do not guarantee perfect decimal accuray so use part with least distance, should be zero distance in most cases
                                    {
                                        float thisPartDist = Vector3.Distance(partLoc, vsl2.rootPart.transform.InverseTransformPoint(p.transform.position));
                                        if (thisPartDist < partDist)
                                        {
                                            gamePart = p;
                                            partDist = thisPartDist;
                                        }
                                    }
                                    bool ShowAmbiguousMessage = true;
                                    if (partDist < 0.3f) //do not show it if part found is more then 0.3meters off
                                    {
                                        ShowAmbiguousMessage = true;
                                    }
                                    else
                                    {
                                        ShowAmbiguousMessage = false;
                                    }
                                    foreach (ConfigNode actNode in prtNode.nodes)
                                    {
                                        //Log.dbg("node " + actNode + " " + gamePart.ConstructID);
                                        AGXAction actToAdd = StaticData.LoadAGXActionVer2(actNode, gamePart, ShowAmbiguousMessage);
                                        //Log.dbg("act to add " + actToAdd.ba);
                                        if (actToAdd != null)
                                        {
                                            List<AGXAction> Checking = new List<AGXAction>();
                                            Checking.AddRange(StaticData.CurrentVesselActions);
                                            Checking.RemoveAll(p => p.group != actToAdd.group);
                                            Checking.RemoveAll(p => p.prt != actToAdd.prt);
                                            Checking.RemoveAll(p => p.ba != actToAdd.ba);

                                            if (Checking.Count == 0)
                                            {
                                                StaticData.CurrentVesselActions.Add(actToAdd);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    RefreshCurrentActions();
                }
            }
            catch (Exception e)
            {
                Log.err("Docking fail. Ignore this error if you did not just dock.");
                Log.ex(this, e);
            }
        }

        public void CalculateActiveActions()
        {
            Log.trace("calculateActiveActions Start {0}", StaticData.CurrentVesselActions.Count);
            ActiveActionsState.Clear();
            for (int i = 1; i <= 250; i = i + 1)
            {
                if (StaticData.CurrentVesselActions.Any(a => a.group == i))
                {
                    // ActiveActions.Add(i);
                    ActiveActionsState.Add(new AGXActionsState() { group = i, actionOff = false, actionOn = false });
                }
            }
            ActiveKeys.Clear();
            ActiveKeysDirect.Clear();
            DefaultTen.Clear();
            for (int i2 = 1; i2 <= 250; i2++) //add all keycodes to key detections
            {
                if (AGXguiKeys[i2] != KeyCode.None)
                {
                    DefaultTen.Add(i2, AGXguiKeys[i2]); //only add it if keycode is assigned
                }
            }
            if (ActiveActionsState.Count > 0)
            {
                foreach (AGXActionsState i2 in ActiveActionsState)
                {
                    if (isDirectAction[i2.group])
                    {
                        ActiveKeysDirect.Add(i2.group, AGXguiKeys[i2.group]);
                    }
                    else
                    {
                        ActiveKeys.Add(AGXguiKeys[i2.group]);
                    }

                    if (DefaultTen.ContainsKey(i2.group)) //remove assigned groups from basic key detection
                    {
                        DefaultTen.Remove(i2.group);
                    }
                }
            }
            //Log.dbg("Def 10 count " + DefaultTen.Count + " " + ActiveKeys.Count + " " + ActiveKeysDirect.Count);

            Log.trace("calculateActiveActions22 end");
            CalculateActionsState();
            CalculateActionsToShow();
        }

        public static void CalculateActionsState() //flag each actiongroup as activated or not
        {
            // print("Calculate start");
            Log.trace("CalclateActionsState33 start!");
            string errLine = "1";
            try
            {
                errLine = "2";
                foreach (AGXActionsState actState in ActiveActionsState)
                {
                    errLine = "3";
                    actState.actionOn = false;
                    actState.actionOff = false;
                }
                errLine = "4";
                //Log.dbg("Calculate start2");
                try
                {
                    foreach (AGXAction agxAct in StaticData.CurrentVesselActions)
                    {
                        //Log.dbg("actions " + agxAct.ToString());
                        Log.trace("cnt {0}", ActiveActionsState.Count);
                        errLine = "5";
                        if (agxAct.activated)
                        {
                            errLine = "6";
                            //Log.dbg(agxAct.activated + agxAct.ba.name); 
                            ActiveActionsState.Find(p => p.group == agxAct.group).actionOn = true;
                        }
                        else if (!agxAct.activated)
                        {
                            errLine = "7";
                            //Log.dbg(agxAct.activated + agxAct.ba.name);
                            ActiveActionsState.Find(p => p.group == agxAct.group).actionOff = true;
                        }
                        errLine = "8";
                    }
                }
                catch
                {
                    //ActiveActionsState was count 0, therefore the .actionON line throws a null ref, repopulate it.
                }
                errLine = "9";
                foreach (AGXActionsState actState in ActiveActionsState) //update our state list, in theory this is never used but just in case, all action groups updated at this time have actions assigned and are state checked that way
                {
                    errLine = "10";
                    if (actState.group <= 10)
                    {
                        if (actState.actionOn && !actState.actionOff)
                        {
                            FlightGlobals.ActiveVessel.ActionGroups.groups[actState.group + 6] = true;
                            groupActivatedState[actState.group] = true;
                        }
                        else
                        {
                            FlightGlobals.ActiveVessel.ActionGroups.groups[actState.group + 6] = false;
                            groupActivatedState[actState.group] = false;
                        }
                    }
                    else if (actState.actionOn && !actState.actionOff)
                    {
                        groupActivatedState[actState.group] = true;
                    }
                    else
                    {
                        groupActivatedState[actState.group] = false;
                    }
                }
                Log.trace("CalclateActionsState33 end!");
            }
            catch (Exception e)
            {
                Log.err("CalculateActionsState {0}", errLine);
                Log.ex(typeof(AGXFlight), e);
            }
            //Log.dbg("Calculate start4");
        }

        public static string SaveGroupVisibility(string str)
        {
            string errLine = "1";
            try
            {
                errLine = "2";

                errLine = "4";
                string ReturnStr = ShowGroupInFlightCurrent.ToString(); //add currently show flightgroup
                errLine = "5";
                for (int i = 1; i <= 250; i++)
                {
                    errLine = "6";
                    ReturnStr = ReturnStr + Convert.ToInt16(IsGroupToggle[i]).ToString(); //add toggle state for group
                    errLine = "7";
                    for (int i2 = 1; i2 <= 5; i2++)
                    {
                        errLine = "8";
                        ReturnStr = ReturnStr + Convert.ToInt16(ShowGroupInFlight[i2, i]).ToString(); //add flight state visibility for each group
                    }
                }
                //Log.dbg("AGXTogSave: " + ReturnStr);    
                errLine = "9";
                return ReturnStr;
            }
            catch (Exception e)
            {
                Log.err("Fail: SaveGroupVisibility {0}", errLine);
                Log.ex(typeof(AGXFlight), e);
                return str;
            }
        }

        public void CalculateActionsToShow()
        {
            ActiveActionsStateToShow.Clear();
            foreach (AGXActionsState actState in ActiveActionsState)
            {
                if (ShowGroupInFlight[ShowGroupInFlightCurrent, actState.group])
                {
                    ActiveActionsStateToShow.Add(actState);
                }
            }
        }

        public static string SaveGroupNames(ModuleAGX agxPM) //pass partmodule because we need both a part and a string reference
        {
            string errStep = "1";
            try
            {
                if (agxPM.part.missionID == currentMissionId)
                {
                    errStep = "2";
                    Log.trace("test mission id match");
                    return GroupNamesDictToString(AGXguiNames);
                }
                else
                {
                    Log.trace("test mission id no match");
                    Dictionary<int, string> curPMNames = GroupNamesStringToDict(agxPM.groupNames);
                    Dictionary<int, string> tempNames = new Dictionary<int, string>();
                    for (int i = 1; i <= 250; i++)
                    {
                        if (curPMNames[i].Length > 0)
                        {
                            tempNames[i] = curPMNames[i];
                        }
                        else if (AGXguiNames[i].Length > 0)
                        {
                            tempNames[i] = AGXguiNames[i];
                        }
                        else
                        {
                            tempNames[i] = "";
                        }
                    }
                    return GroupNamesDictToString(tempNames);
                }
            }
            catch (Exception e)
            {
                Log.err("Save Group Names FAIL! (SaveGroupNames) {0}", errStep);
                Log.ex(typeof(AGXFlight), e);
                return agxPM.groupNames;
            }
        }

        public void AGXResetPartsList()
        {
            AGEEditorSelectedPartsSame = true;
            AGEditorSelectedParts.Clear();
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                AGEditorSelectedParts.Add(new AGXPart(p));
            }

            AGXPart AGPcompare = new AGXPart();
            AGPcompare = AGEditorSelectedParts.First();

            foreach (AGXPart p in AGEditorSelectedParts)
            {
                if (p.AGPart.name != AGPcompare.AGPart.name)
                {
                    AGEEditorSelectedPartsSame = false;
                }
            }
        }

        public static void CheckActionsActive()
        {
            Log.trace("CheckActionsActice Start");
            StaticData.CurrentVesselActions = CheckActionsActiveActualCode(StaticData.CurrentVesselActions);
            Log.dbg("CheckActionsActice Mid");
            CalculateActionsState();
            Log.dbg("CheckActionsActice End");
        }

        public static List<AGXAction> CheckActionsActiveActualCode(List<AGXAction> actsListToCheck) //monitor actions state, have to add them manually
        {
            //string errLine = "1";

            //start toggle checking
            foreach (AGXAction agAct in actsListToCheck)
            {
                try
                {
                    if (agAct.group <= 10 && agAct.ba.listParent.part.vessel == FlightGlobals.ActiveVessel)
                    {
                        agAct.activated = agAct.ba.listParent.part.vessel.ActionGroups[CONST.CustomActions[agAct.group]];
                    }
                }
                catch (Exception e)
                {
                    Log.err("Action State Check Fail a {0} {1}", agAct.ba.name, agAct.ba.listParent.module.moduleName);
                    Log.ex(typeof(AGXFlight), e);
                    return actsListToCheck;
                }

                try
                {
                    switch (agAct.ba.listParent.module.moduleName)
                    {
                        case "ModuleDeployableSolarPanel":
                            agAct.activated = agAct.ba.listParent.module.Fields.GetValue("deployState").ToString() == "EXTENDED";
                            break;
                        case "ModuleLandingLeg":
                            if (agAct.ba.name == "OnAction" || agAct.ba.name == "RaiseAction" || agAct.ba.name == "LowerAction")
                            {
                                agAct.activated =
                                    (int)agAct.ba.listParent.module.Fields.GetValue("savedLegState") == 3;
                            }
                            if (agAct.ba.name == "ToggleSuspensionLockAction") //only act
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("suspensionLocked");
                            }
                            break;
                        case "ModuleEngines":
                            agAct.activated =
                                ((ModuleEngines)agAct.ba.listParent.module).getIgnitionState;
                            break;
                        case "ModuleEnginesFX":
                            agAct.activated =
                                ((ModuleEnginesFX)agAct.ba.listParent.module).getIgnitionState;
                            break;
                        case "ModuleEnviroSensor":
                            agAct.activated =
                                (bool)agAct.ba.listParent.module.Fields.GetValue("sensorActive");
                            break;
                        case "ModuleGenerator":
                            agAct.activated =
                                (bool)agAct.ba.listParent.module.Fields.GetValue("generatorIsActive");
                            break;
                        case "ModuleGimbal":
                            agAct.activated =
                                !(bool)agAct.ba.listParent.module.Fields.GetValue("gimbalLock");
                            break;
                        case "ModuleCommandActions":
                            if (agAct.ba.name == "ControlFromHere")
                            {
                                agAct.activated = false;
                                if (agAct.ba.listParent.part.flightID == agAct.ba.listParent.part.vessel.referenceTransformId)
                                {
                                    agAct.activated =
                                        agAct.ba.listParent.part.flightID == agAct.ba.listParent.part.vessel.referenceTransformId;
                                }
                            }
                            break;
                        case "ModuleDockingNodeActions":
                            agAct.activated = false;
                            if (agAct.ba.listParent.part.flightID == agAct.ba.listParent.part.vessel.referenceTransformId)
                            {
                                agAct.activated = true;
                            }
                            break;
                        case "ModuleGimbalActions":
                            agAct.activated = true;
                            foreach (ModuleGimbal pm in agAct.ba.listParent.part.Modules.OfType<ModuleGimbal>())
                            {
                                if (pm.gimbalLock)
                                {
                                    agAct.activated = false;
                                }
                            }
                            break;
                        case "ModuleLandingGear":
                            if (agAct.ba.name == "OnAction")
                            {
                                agAct.activated =
                                   (string)agAct.ba.listParent.module.Fields.GetValue("storedGearState") == "DEPLOYED";
                            }
                            else if (agAct.ba.name == "BrakesAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("brakesEngaged");
                            }
                            break;
                        case "ModuleSteering":
                            if (agAct.ba.name == "InvertSteeringAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("invertSteering");
                            }
                            if (agAct.ba.name == "ToggleSteeringAction" || agAct.ba.name == "LockSteeringAction" || agAct.ba.name == "UnlockSteeringAction")
                            {
                                agAct.activated =
                                    !(bool)agAct.ba.listParent.module.Fields.GetValue("steeringLocked");
                            }
                            break;
                        case "ModuleLight":
                            if (agAct.ba.name == "ToggleLightAction" || agAct.ba.name == "LightOnAction" || agAct.ba.name == "LightOffAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("isOn");
                            }
                            break;
                        case "ModuleRCS":
                            if (agAct.ba.name == "ToggleAction")
                            {
                                agAct.activated =
                                    ((ModuleRCS)agAct.ba.listParent.module).rcsEnabled;
                            }
                            break;
                        case "ModuleReactionWheel":
                            //Log.dbg((string)agAct.ba.listParent.module.Fields.GetValue("stateString"));
                            if (agAct.ba.name == "Toggle" || agAct.ba.name == "Activate" || agAct.ba.name == "Deactivate")
                            {
                                agAct.activated =
                                    !((string)agAct.ba.listParent.module.Fields.GetValue("stateString") ==
                                      "Disabled");
                            }
                            break;
                        case "ModuleScienceExperiment":
                            if (agAct.ba.name == "DeployAction" || agAct.ba.name == "ResetAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("Deployed");
                            }
                            break;
                        case "ModuleResourceIntake":
                            if (agAct.ba.name == "ToggleAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("intakeEnabled");
                            }
                            break;
                        case "ModuleWheel":
                            switch (agAct.ba.name)
                            {
                                case "InvertSteeringAction":
                                    agAct.activated =
                                        (bool)agAct.ba.listParent.module.Fields.GetValue("invertSteering");
                                    break;
                                case "ToggleSteeringAction":
                                case "LockSteeringAction":
                                case "UnlockSteeringAction":
                                    agAct.activated =
                                        !(bool)agAct.ba.listParent.module.Fields.GetValue("steeringLocked");
                                    break;
                                case "BrakesAction":
                                    agAct.activated =
                                        (bool)agAct.ba.listParent.module.Fields.GetValue("brakesEngaged");
                                    break;
                                case "ToggleMotorAction":
                                    agAct.activated =
                                        (bool)agAct.ba.listParent.module.Fields.GetValue("motorEnabled");
                                    break;
                            }
                            break;
                        case "ModuleAnimateGeneric":
                            if (agAct.ba.name == "ToggleAction")
                            {
                                agAct.activated =
                                    ((ModuleAnimateGeneric)agAct.ba.listParent.module).animTime == 1f;
                            }
                            break;
                        case "SCANsat":
                            if (agAct.ba.name == "startScanAction" || agAct.ba.name == "stopScanAction" || agAct.ba.name == "toggleScanAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("scanning");
                            }
                            break;
                        case "KethaneConverter":
                            if (agAct.ba.name == "ActivateConverterAction" || agAct.ba.name == "DeactivateConverterAction" || agAct.ba.name == "ToggleConverterAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("IsEnabled");
                            }
                            break;
                        case "KethaneGenerator":
                            if (agAct.ba.name == "EnableAction" || agAct.ba.name == "DisableAction" || agAct.ba.name == "ToggleAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("Enabled");
                            }
                            break;
                        case "KethaneDetector":
                            if (agAct.ba.name == "EnableDetectionAction" || agAct.ba.name == "DisableDetectionAction" || agAct.ba.name == "ToggleDetectionAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("IsDetecting");
                            }
                            break;
                        case "KethaneExtractor":
                            if (agAct.ba.name == "DeployDrillAction" || agAct.ba.name == "RetractDrillAction" || agAct.ba.name == "ToggleDrillAction")
                            {
                                agAct.activated =
                                    (string)agAct.ba.listParent.module.Fields.GetValue("Status") == "Deployed";
                            }
                            break;
                        case "FSswitchEngineThrustTransform":
                            if (agAct.ba.name == "switchTTAction" || agAct.ba.name == "reverseTTAction" || agAct.ba.name == "normalTTAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("isReversed");
                            }
                            break;
                        case "FSairBrake":
                            if (agAct.ba.name == "toggleAirBrakeAction")
                            {
                                agAct.activated =
                                    !((float)agAct.ba.listParent.module.Fields.GetValue("targetAngle") == 0);
                            }
                            break;
                        case "FSwing":
                            if (agAct.ba.name == "toggleLeadingEdgeAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("leadingEdgeExtended");
                            }
                            if (agAct.ba.name == "extendFlapAction" || agAct.ba.name == "retractFlapAction")
                            {
                                agAct.activated =
                                    !((float)agAct.ba.listParent.module.Fields.GetValue("flapMin") == (float)agAct.ba.listParent.module.Fields.GetValue("flapTarget"));
                            }
                            break;
                        case "FSwingletRangeAdjustment":
                            if (agAct.ba.name == "lockRangeAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("locked");
                            }
                            break;
                        case "FSanimateGeneric":
                            if (agAct.ba.name == "toggleAction")
                            {
                                agAct.activated =
                                     (float)agAct.ba.listParent.module.Fields.GetValue("animTime") == 1f;
                            }
                            break;
                        case "FARControllableSurface":
                            if (agAct.ba.name == "IncreaseDeflect" || agAct.ba.name == "DecreaseDeflect")
                            {
                                agAct.activated =
                                    !((int)agAct.ba.listParent.module.Fields.GetValue("flapDeflectionLevel") == 0);
                            }

                            if (agAct.ba.name == "ActivateSpoiler")   //THIS CODE WORKS, just need to wait for brake to become public on the next version of FAR
                            {
                                Assembly FarAsm = null;
                                foreach (AssemblyLoader.LoadedAssembly Asm in AssemblyLoader.loadedAssemblies)
                                {
                                    if (Asm.dllName == "FerramAerospaceResearch")
                                    {
                                        Log.trace("far found");
                                        FarAsm = Asm.assembly;
                                    }
                                }
                                Log.trace("far found2");
                                if (FarAsm != null)
                                {
                                    Type FarCtrlSurf = FarAsm.GetType("FARControllableSurface");
                                    Log.trace("far found3");
                                    if ((bool)agAct.ba.listParent.module.GetType().GetField("brake").GetValue(agAct.ba.listParent.module))//.GetValue(FarCtrlSurf));
                                    {
                                        agAct.activated = true;
                                    }
                                    else
                                    {
                                        agAct.activated = false;
                                    }
                                }
                            }
                            break;
                        case "FSrotorTrim":
                            if (agAct.ba.name == "toggleSteeringAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("steeringEnabled");
                            }
                            break;

                        case "FSVTOLrotator":
                            if (agAct.ba.name == "toggleVTOLAction" || agAct.ba.name == "raiseVTOLAction" || agAct.ba.name == "lowerVTOLAction")
                            {
                                agAct.activated =
                                    (float)agAct.ba.listParent.module.Fields.GetValue("targetAngle") == Mathf.Abs((float)agAct.ba.listParent.module.Fields.GetValue("deployedAngle"));
                            }
                            if (agAct.ba.name == "toggleVTOLsteeringAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("VTOLsteeringActive");
                            }
                            break;
                        case "FSengineHover":
                            if (agAct.ba.name == "toggleHoverAction" || agAct.ba.name == "increaseVerticalSpeed" || agAct.ba.name == "decreaseVerticalSpeed")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("hoverActive");
                            }
                            break;
                        case "FShoverThrottle":
                            if (agAct.ba.name == "toggleHoverAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("hoverActive");
                            }
                            break;
                        case "FSpartTurner":
                            if (agAct.ba.name == "toggleSteeringAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("steeringEnabled");
                            }
                            if (agAct.ba.name == "toggleInvertAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("reversedInput");
                            }
                            break;
                        case "FSwheel":
                            if (agAct.ba.name == "ToggleGearAction")
                            {
                                agAct.activated =
                                    (float)agAct.ba.listParent.module.Fields.GetValue("animTime") == 1f;
                            }
                            if (agAct.ba.name == "ReverseMotorAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("reverseMotor");

                            }
                            if (agAct.ba.name == "ToggleMotorAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("motorEnabled");
                            }
                            if (agAct.ba.name == "BrakesAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("brakesEngaged");
                            }
                            break;
                        case "MuMechToggle":
                            if (agAct.ba.name == "LockToggle")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("isMotionLock");
                            }
                            break;
                        case "DTMagnetometer":
                            if (agAct.ba.name == "ActivateMagnetometerAction" || agAct.ba.name == "DeactivateMagnetometerAction" || agAct.ba.name == "ToggleMagnetometerAction")
                            {
                                agAct.activated =
                                   agAct.ba.listParent.module.isEnabled;
                            }
                            break;
                        case "FNThermalHeatExchanger":
                            if (agAct.ba.name == "ActivateHeatExchangerAction" || agAct.ba.name == "DeactivateHeatExchangerAction" || agAct.ba.name == "ToggleHeatExchangerAction")
                            {
                                agAct.activated =
                                    agAct.ba.listParent.module.isEnabled;
                            }
                            break;
                        case "ISRUScoop":
                            if (agAct.ba.name == "ActivateScoopAction" || agAct.ba.name == "DisableScoopAction" || agAct.ba.name == "ToggleScoopAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("scoopIsEnabled");
                            }
                            break;
                        case "MicrowavePowerTransmitter":
                            if (agAct.ba.name == "ActivateTransmitterAction" || agAct.ba.name == "DeactivateTransmitterAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("IsEnabled");
                            }
                            if (agAct.ba.name == "ActivateRelayAction" || agAct.ba.name == "DeactivateRelayAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("relay");
                            }
                            break;
                        case "MicrowavePowerTransmitterBackup":
                            if (agAct.ba.name == "ActivateTransmitterAction" || agAct.ba.name == "DeactivateTransmitterAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("IsEnabled");
                            }
                            break;
                        case "ModuleModableScienceGenerator":
                            if (agAct.ba.name == "DeployAction" || agAct.ba.name == "ResetAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("Deployed");
                            }
                            break;
                        case "AlcubierreDrive":
                            if (agAct.ba.name == "StartChargingAction" || agAct.ba.name == "StopChargingAction" || agAct.ba.name == "ToggleChargingAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("IsCharging");
                            }
                            break;
                        case "MicrowavePowerReceiverBackup":
                            if (agAct.ba.name == "ActivateReceiverAction" || agAct.ba.name == "DisableReceiverAction" || agAct.ba.name == "ToggleReceiverAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("IsEnabled");
                            }
                            break;

                        case "FNGenerator":
                            if (agAct.ba.name == "ActivateGeneratorAction" || agAct.ba.name == "DeactivateGeneratorAction" || agAct.ba.name == "ToggleGeneratorAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("IsEnabled");
                            }
                            break;
                        case "FNRadiator":
                            if (agAct.ba.name == "DeployRadiatorAction" || agAct.ba.name == "RetractRadiatorAction" || agAct.ba.name == "ToggleRadiatorAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("radiatorIsEnabled");
                            }
                            break;
                        case "FNReactor":
                            if (agAct.ba.name == "ActivateReactorAction" || agAct.ba.name == "DeactivateReactorAction" || agAct.ba.name == "ToggleReactorAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("IsEnabled");
                            }
                            break;
                        case "MicrowavePowerReceiver":
                            if (agAct.ba.name == "ActivateReceiverAction" || agAct.ba.name == "DisableReceiverAction" || agAct.ba.name == "ToggleReceiverAction")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("receiverIsEnabled");
                            }
                            break;
                        case "TacGenericConverter":
                            if (agAct.ba.name == "ToggleConverter")
                            {
                                agAct.activated =
                                    (bool)agAct.ba.listParent.module.Fields.GetValue("converterEnabled");
                            }
                            break;
                        case "ModuleControlSurfaceActions":
                            Log.trace("surface found");
                            agAct.activated = true;
                            foreach (PartModule pm in agAct.ba.listParent.part.Modules)
                            {
                                if (pm is ModuleControlSurface)
                                {
                                    Log.trace("surface found2");
                                    ModuleControlSurface CS = (ModuleControlSurface)pm;
                                    switch (agAct.ba.name)
                                    {
                                        case "TogglePitchAction":
                                        case "EnablePitchAction":
                                        case "DisablePitchAction":
                                            Log.trace("surface found3");
                                            if (CS.ignorePitch)
                                            {
                                                Log.trace("surface found4");
                                                agAct.activated = false;
                                            }
                                            break;
                                        case "ToggleYawAction":
                                        case "EnableYawAction":
                                        case "DisableYawAction":
                                            if (CS.ignoreYaw)
                                            {
                                                agAct.activated = false;
                                            }
                                            break;
                                        case "ToggleRollAction":
                                        case "EnableRollAction":
                                        case "DisableRollAction":
                                            if (CS.ignoreRoll)
                                            {
                                                agAct.activated = false;
                                            }
                                            break;
                                    }
                                }
                                else if (pm.moduleName == "FARControllableSurface") //FAR adds this after stock ModuleControlSurface always, so this runs second
                                {
                                    Log.trace("Start FAR Module");
                                    int i;
                                    if (int.TryParse(pm.Fields.GetValue("rollaxis").ToString(), out i)) //true if FAR installed, false if NEAR installed. Only need to check once, not three times for pitch/roll/yaw
                                    {
                                        switch (agAct.ba.name)
                                        {
                                            case "TogglePitchAction":
                                            case "EnablePitchAction":
                                            case "DisablePitchAction":
                                                if ((float)pm.Fields.GetValue("pitchaxis") == 0f)
                                                {
                                                    agAct.activated = false;
                                                }
                                                break;
                                            case "ToggleYawAction":
                                            case "EnableYawAction":
                                            case "DisableYawAction":
                                                if ((float)pm.Fields.GetValue("yawaxis") == 0f)
                                                {
                                                    agAct.activated = false;
                                                }
                                                break;
                                            case "ToggleRollAction":
                                            case "EnableRollAction":
                                            case "DisableRollAction":
                                                if ((float)pm.Fields.GetValue("rollaxis") == 0f)
                                                {
                                                    agAct.activated = false;
                                                }
                                                break;
                                        }
                                    }
                                    else //NEAR installed
                                    {
                                        switch (agAct.ba.name)
                                        {
                                            case "TogglePitchAction":
                                            case "EnablePitchAction":
                                            case "DisablePitchAction":
                                                if (!(bool)pm.Fields.GetValue("pitchaxis"))
                                                {
                                                    agAct.activated = false;
                                                }
                                                break;
                                            case "ToggleYawAction":
                                            case "EnableYawAction":
                                            case "DisableYawAction":
                                                if (!(bool)pm.Fields.GetValue("yawaxis"))
                                                {
                                                    agAct.activated = false;
                                                }
                                                break;
                                            case "ToggleRollAction":
                                            case "EnableRollAction":
                                            case "DisableRollAction":
                                                if (!(bool)pm.Fields.GetValue("rollaxis"))
                                                {
                                                    agAct.activated = false;
                                                }
                                                break;
                                        }
                                    }
                                    Log.trace("end FAR Module");
                                }
                            }
                            break;
                        case "ModuleFuelCrossfeedActions":
                            agAct.activated =
                                agAct.ba.listParent.part.fuelCrossFeed;
                            break;
                        case "ModuleResourceActions":
                            if (agAct.ba.name == "ToggleResource" || agAct.ba.name == "EnableResource" || agAct.ba.name == "DisableResource")
                            {
                                agAct.activated =
                                    !(bool)agAct.ba.listParent.module.Fields.GetValue("lockResource");
                            }

                            if (agAct.ba.name == "ToggleElec" || agAct.ba.name == "EnableElec" || agAct.ba.name == "DisableElec")
                            {
                                agAct.activated =
                                    !(bool)agAct.ba.listParent.module.Fields.GetValue("lockEC");
                            }
                            break;
                        case "USI_ModuleAsteroidDrill":
                            agAct.activated = false;

                            agAct.activated =
                                (bool)agAct.ba.listParent.module.Fields.GetValue("IsActivated");

                            break;
                        case "ModuleControlSurface":
                            agAct.activated = false;
                            ModuleControlSurface ctrlSurf = (ModuleControlSurface)agAct.ba.listParent.module;

                            agAct.activated =
                                ctrlSurf.deploy;
                            break;
                    }
                }
                catch (Exception e)
                {
                    Log.err("Action State Check Fail {0} {1}", agAct.ba.name, agAct.ba.listParent.module.moduleName);
                    Log.ex(typeof(AGXFlight), e);
                }
            }
            return actsListToCheck;
        }
    }
}
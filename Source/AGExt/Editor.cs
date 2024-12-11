/*
	This file is part of Action Groups Extended (AGExt) /L Unleashed
		© 2018-2024 LisiasT : https://lisias.net : http://lisias.net <support@lisias.net>
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

using GUILayout = KSPe.UI.GUILayout;
using GUI = KSPe.UI.GUI;
using Toolbar = KSPe.UI.Toolbar;

namespace ActionGroupsExtended
{
	using File = KSPe.IO.File<AGXEditor>;

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class AGXEditor : PartModule
    {
        public static AGXEditor thisModule;
        bool showCareerStockAGs = false; //support locking action groups in early career
        bool showCareerCustomAGs = false;
        bool showAGXRightClickMenu = false; //show stock toolbar right click menu?
        bool defaultShowingNonNumeric = false; //are we in non-numeric (abort/brakes/gear/list) mode?
        List<BaseAction> defaultActionsListThisType; //list of default actions showing in group win when non-numeric
        List<BaseAction> defaultActionsListAll; //list of all default actions on vessel, only used in non-numeric mode when going to other mode
        KSPActionGroup defaultGroupToShow = KSPActionGroup.Abort; //which group is currently selected if showing non-numeric groups
        List<AGXAction> ThisGroupActions;
        private string LastKeyCode = "";
        public static Dictionary<int, bool> IsGroupToggle; //is group a toggle group?
        public static bool[,] ShowGroupInFlight; //Show group in flight?

        public static string[] ShowGroupInFlightNames;
        public static int ShowGroupInFlightCurrent = 1;

        private List<Part> SelectedWithSym; //selected parts from last frame for Default actions monitoring
        private List<AGXDefaultCheck> SelectedWithSymActions; //monitor baseaction.actiongroups of selected parts
        public static bool NeedToLoadActions = true;
        public static bool LoadFinished = false;
        //Selected Parts Window Variables
        public static Rect SelPartsWin;
        private static Vector2 ScrollPosSelParts;
        public static Vector2 ScrollPosSelPartsActs;
        public static Vector2 ScrollGroups;
        public static Vector2 CurGroupsWin;
        private static List<AGXPart> AGEditorSelectedParts;
        private static bool AGEEditorSelectedPartsSame = false;
        private static int SelectedPartsCount = 0;
        private static bool ShowSelectedWin = true;
        private static Part PreviousSelectedPart = null;
        private static bool SelPartsIncSym = true;
        private static string BtnTxt;
        private static bool AGXDoLock = false;
        private static Part AGXRoot;
        public static List<AGXAction> DetachedPartActions;
        public static Timer DetachedPartReset;
        public static Timer DisablePartAttachingReset;

        private static int GroupsPage = 1;
        private static string CurGroupDesc;
        private static bool AutoHideGroupsWin = false;
        private static bool TempShowGroupsWin = false;
        private static Rect KeyCodeWin;
        private static Rect CurActsWin;
        private static bool ShowKeyCodeWin = false;
        private static bool ShowJoySticks = false;
        private static bool ShowKeySetWin = false;
        private static int CurrentKeySet = 1;
        private static string CurrentKeySetName;
        private static Rect KeySetWin;
        private static ConfigNode AGExtNode;
        static string[] KeySetNames = new string[5];
        private static bool TrapMouse = false;

        public static Rect GroupsWin;
        public static bool Trigger;
        public static string HexStr = "Load";

        private static List<BaseAction> PartActionsList;

        Vector2 groupWinScroll = new Vector2();
        bool highlightPartThisFrameGroupWin = false;
        public static Dictionary<int, string> AGXguiNames;
        public static Dictionary<int, KeyCode> AGXguiKeys;
        public static Dictionary<int, bool> AGXguiMod1Groups;
        public static Dictionary<int, bool> AGXguiMod2Groups;
        public static KeyCode AGXguiMod1Key = KeyCode.None;
        public static KeyCode AGXguiMod2Key = KeyCode.None;
        private bool AGXguiMod1KeySelecting = false;
        private bool AGXguiMod2KeySelecting = false;
        public static int AGXCurActGroup = 1;
        static List<string> KeyCodeNames = new List<string>();
        static List<string> JoyStickCodes = new List<string>();
        private static bool ActionsListDirty = true; //is our actions requiring update?
        private static bool ShowCurActsWin = true;
        public static Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>();
        public static Dictionary<int, bool> isDirectAction = new Dictionary<int, bool>();
        private bool AGXShow = false;
        static bool inVAB = true; //true if in VAB, flase in SPH
        bool highlightPartThisFrameSelWin = false;
        bool highlightPartThisFrameActsWin = false;
        Part partToHighlight = null;
        bool showAllPartsList = false; //show list of all parts in group window?
        List<string> showAllPartsListTitles; //list of all parts with actions to show in group window
        KSPActionGroup KSPDefaultLastActionGroup = KSPActionGroup.Custom01;
        public static bool disablePartAttaching = true; //disable part attaching feature when loading so non-symmetric actions are not made symmetric
                                                        //static Part partLastHighlight = null;
                                                        ////static Color partHighlighLastColor;
                                                        //static Part.HighlightType partHighlightLastType;
                                                        //static Material[] partHighlightLastMaterial;

#if false
        public class SettingsWindowEditor : MonoBehaviour, IDrawable
        {
            public Rect SettingsWinEditor = new Rect(0, 0, 150, 125);
            public Vector2 Draw(Vector2 position)
            {
                var oldSkin = GUI.skin;
                GUI.skin = HighLogic.Skin;

                SettingsWinEditor.x = position.x;
                SettingsWinEditor.y = position.y;

                // GUI.Window(2233452, SettingsWinEditor, DrawSettingsWinEditor, "AGX Settings", AGXEditor.AGXWinStyle);
                GUI.Window(2233452, SettingsWinEditor, DrawSettingsWinEditor, Localizer.Format("#AGEXT_UI_agx_settings"), AGXEditor.AGXWinStyle);
                //RCSlaWin = GUILayout.Window(42334567, RCSlaWin, DrawWin, (string)null, GUI.skin.box);
                //GUI.skin = oldSkin;

                return new Vector2(SettingsWinEditor.width, SettingsWinEditor.height);
            }

            public void DrawSettingsWinEditor(int WindowID)
            {

                // if (GUI.Button(new Rect(10, 25, 130, 25), "Reset Windows", AGXBtnStyle))
                if (GUI.Button(new Rect(10, 25, 130, 25), Localizer.Format("#AGEXT_UI_setting_reset_windows"), AGXBtnStyle))
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


                }
                AGXBtnStyle.normal.background = AutoHideGroupsWin ? ButtonTextureRed : ButtonTexture;
                AGXBtnStyle.hover.background = AutoHideGroupsWin ? ButtonTextureRed : ButtonTexture;
                // if (GUI.Button(new Rect(10, 50, 130, 25), "Auto-Hide Groups", AGXBtnStyle))
                if (GUI.Button(new Rect(10, 50, 130, 25), Localizer.Format("#AGEXT_UI_setting_auto_hide_groups"), AGXBtnStyle))
                {
                    AutoHideGroupsWin = !AutoHideGroupsWin;
                }
                string s;
                if (HighLogic.CurrentGame.Parameters.CustomParams<AG_Ext>().useBlizzy)
                    s = "Disable Blizzy";
                else
                    s = "Enable Blizzy";
                if (GUI.Button(new Rect(10, 75, 130, 25), Localizer.Format(s), AGXBtnStyle))
                {
                    HighLogic.CurrentGame.Parameters.CustomParams<AG_Ext>().useBlizzy = !HighLogic.CurrentGame.Parameters.CustomParams<AG_Ext>().useBlizzy;
                }
                AGXBtnStyle.normal.background = ButtonTexture;
                AGXBtnStyle.hover.background = ButtonTexture;
            }
            public void Update()
            {

            }
        }
#endif
        public void DrawSettingsWinEditor(int WindowID)
        {
            if (GUI.Button(new Rect(10, 25, 130, 25), Localizer.Format("#AGEXT_UI_setting_reset_windows"), UI.AGXBtnStyle))
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
            }

            if (GUI.Button(new Rect(10, 50, 130, 25), Localizer.Format("#AGEXT_UI_setting_auto_hide_groups"), AutoHideGroupsWin ? UI.AGXBtnStyleAutoHide : UI.AGXBtnStyle))
            {
                AutoHideGroupsWin = !AutoHideGroupsWin;
            }
        }

        public void Start()
        {
            Log.trace("AGXEditorSTartSTart");
            thisModule = this;
            ShowKeyCodeWin = false;
            ShowKeySetWin = false;
            AGXguiMod1Groups = new Dictionary<int, bool>();
            AGXguiMod2Groups = new Dictionary<int, bool>();
            for (int i = 1; i <= 250; i++)
            {
                AGXguiMod1Groups[i] = false;
                AGXguiMod2Groups[i] = false;
            }
            defaultActionsListThisType = new List<BaseAction>(); //initialize list
            defaultActionsListAll = new List<BaseAction>(); //initialize list
            string errLine = "1";
            ThisGroupActions = new List<AGXAction>();
            try
            {
                errLine = "2";
                KSPActs[1] = KSPActionGroup.Custom01;
                KSPActs[2] = KSPActionGroup.Custom02;
                KSPActs[3] = KSPActionGroup.Custom03;
                KSPActs[4] = KSPActionGroup.Custom04;
                KSPActs[5] = KSPActionGroup.Custom05;
                KSPActs[6] = KSPActionGroup.Custom06;
                KSPActs[7] = KSPActionGroup.Custom07;
                KSPActs[8] = KSPActionGroup.Custom08;
                KSPActs[9] = KSPActionGroup.Custom09;
                KSPActs[10] = KSPActionGroup.Custom10;

                AGEditorSelectedParts = new List<AGXPart>();
                PartActionsList = new List<BaseAction>();
                ScrollPosSelParts = Vector2.zero;
                ScrollPosSelPartsActs = Vector2.zero;
                ScrollGroups = Vector2.zero;
                CurGroupsWin = Vector2.zero;

                AGXguiNames = new Dictionary<int, string>();
                AGXguiKeys = new Dictionary<int, KeyCode>();

                for (int i = 1; i <= 250; i = i + 1)
                {
                    AGXguiNames[i] = "";
                    AGXguiKeys[i] = KeyCode.None;
                }
                errLine = "3";

                KeyCodeNames = new List<String>();
                KeyCodeNames.AddRange(Enum.GetNames(typeof(KeyCode)));
                KeyCodeNames.Remove("None");
                JoyStickCodes.AddRange(KeyCodeNames.Where(JoySticks));
                KeyCodeNames.RemoveAll(JoySticks);
                AGExtNode = AGXStaticData.LoadBaseConfigNode();
                errLine = "4";
                if (AGExtNode.GetValue("EditShow") == "0")
                {
                    AGXShow = false;
                }
                else
                {
                    AGXShow = true;
                }
                errLine = "5";
                CurrentKeySet = 1;
                errLine = "6";
                //LoadCurrentKeySet();
                CurrentKeySetName = (string)AGExtNode.GetValue("KeySetName" + CurrentKeySet);
                KeySetNames[0] = (string)AGExtNode.GetValue("KeySetName1");
                KeySetNames[1] = (string)AGExtNode.GetValue("KeySetName2");
                KeySetNames[2] = (string)AGExtNode.GetValue("KeySetName3");
                KeySetNames[3] = (string)AGExtNode.GetValue("KeySetName4");
                KeySetNames[4] = (string)AGExtNode.GetValue("KeySetName5");
                errLine = "7";
                //CurrentVesselActions = new List<AGXAction>();
                errLine = "8";
                AGXRoot = null;
                errLine = "9";
                StartLoadWindowPositions();
                errLine = "14";
                //Log.dbg("a");
                if (EditorDriver.editorFacility == EditorFacility.VAB)
                {
                    inVAB = true;
                }
                else
                {
                    inVAB = false;
                }
                if (AGExtNode.HasValue("OverrideCareer") || HighLogic.CurrentGame.Parameters.CustomParams<AGExt>().OverrideCareer ) //are action groups unlocked?
                {
                    //Log.dbg("b");
                    if ((string)AGExtNode.GetValue("OverrideCareer") == "1")
                    {
                        //Log.dbg("AGX 0");
                        showCareerCustomAGs = true;
                        showCareerStockAGs = true;
                    }
                    else
                    {
                        //Log.dbg("d");
                        float facilityLevel = 0f;
                        if (EditorDriver.editorFacility == EditorFacility.SPH)
                        {
                            // print("e");
                            facilityLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar);
                            //Log.dbg("AGX Career check SPH: " + facilityLevel);
                        }
                        else
                        {
                            //Log.dbg("f");
                            facilityLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding);
                            //Log.dbg("AGX Career check VAB: " + facilityLevel);
                        }

                        if (GameVariables.Instance.UnlockedActionGroupsCustom(facilityLevel, inVAB)) 
                        {
                            //Log.dbg("AGX 1");
                            showCareerStockAGs = true;
                            showCareerCustomAGs = true;
                        }
                        else if (GameVariables.Instance.UnlockedActionGroupsStock(facilityLevel, inVAB))
                        {
                            //Log.dbg("AGX 2");
                            showCareerStockAGs = true;
                            showCareerCustomAGs = false;
                        }
                        else
                        {
                            //Log.dbg("AGX 3");
                            showCareerStockAGs = false;
                            showCareerCustomAGs = false;
                        }
                    }
                }
                else
                {
                    // print("j");
                    float facilityLevel2 = 0f;
                    if (EditorDriver.editorFacility == EditorFacility.SPH)
                    {
                        // print("k");
                        facilityLevel2 = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar);
                    }
                    else
                    {
                        //Log.dbg("l");
                        facilityLevel2 = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding);
                    }

                    if (GameVariables.Instance.UnlockedActionGroupsCustom(facilityLevel2, inVAB))
                    {
                        //Log.dbg("m");
                        showCareerStockAGs = true;
                        showCareerCustomAGs = true;
                    }
                    else if (GameVariables.Instance.UnlockedActionGroupsStock(facilityLevel2,inVAB))
                    {
                        //Log.dbg("n");
                        showCareerStockAGs = true;
                        showCareerCustomAGs = false;
                    }
                    else
                    {
                        //Log.dbg("o");
                        showCareerStockAGs = false;
                        showCareerCustomAGs = false;
                    }
                }
                //Log.dbg("startd " + showCareerCustomAGs);

                //LoadCurrentKeyBindings();
                errLine = "15";
                AddButtons();
                errLine = "16";

                DetachedPartActions = new List<AGXAction>();

                DetachedPartReset = new Timer();
                DetachedPartReset.Interval = 500;

                DetachedPartReset.Stop();
                DetachedPartReset.AutoReset = true;

                DetachedPartReset.Elapsed += new ElapsedEventHandler(ResetDetachedParts);

                DisablePartAttachingReset = new Timer();
                DisablePartAttachingReset.Interval = 1000;
                DisablePartAttachingReset.Stop();
                DisablePartAttachingReset.AutoReset = true;
                DisablePartAttachingReset.Elapsed += new ElapsedEventHandler(ResetDisablePartAttaching);

                SelectedWithSym = new List<Part>();
                SelectedWithSymActions = new List<AGXDefaultCheck>();
                errLine = "17";

                errLine = "18";
                IsGroupToggle = new Dictionary<int, bool>();
                ShowGroupInFlight = new bool[6, 251];
                ShowGroupInFlightNames = new string[6];

                ShowGroupInFlightNames[1] = "Group 1";
                ShowGroupInFlightNames[2] = "Group 2";
                ShowGroupInFlightNames[3] = "Group 3";
                ShowGroupInFlightNames[4] = "Group 4";
                ShowGroupInFlightNames[5] = "Group 5";

                errLine = "19";
                Log.trace("toggle dict set");
                for (int i = 1; i <= 250; i++)
                {
                    IsGroupToggle[i] = false;
                    for (int i2 = 1; i2 <= 5; i2++)
                    {
                        ShowGroupInFlight[i2, i] = true;
                    }
                }
                errLine = "20";
                
                GameEvents.onPartAttach.Add(PartAttaching);// this game event only fires for part removed, not child parts
                GameEvents.onPartRemove.Add(PartRemove);
                GameEvents.onEditorPartEvent.Add(OnPartEvent);
                GameEvents.onEditorLoad.Add(OnShipLoad);
                isDirectAction = new Dictionary<int, bool>();
                StaticData.CurrentVesselActions.Clear();
                //EditorLoadFromFile();
                EditorLoadFromNode();
                errLine = "21";

                LoadFinished = true;
                Log.detail("Editor Start Okay {0}", StaticData.CurrentVesselActions.Count());
                Log.trace("AGXEditorSTartEnd");
            }
            catch (Exception e)
            {
                Log.err("Editor Start Fail {0}", errLine);
                Log.err("node dump: {0}", AGExtNode);
                Log.ex(this, e);
            }
        }

        public void OnShipLoad(ShipConstruct ship, CraftBrowserDialog.LoadType loadType)
        {
            Log.dbg("testing");
            DetachedPartReset.Start(); //start timer so it fires even if no parts load
            EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.partsEditor);
            if (loadType == CraftBrowserDialog.LoadType.Normal)
            {
                Log.trace("part attaching disable");
                disablePartAttaching = true; //disable symmetric action loading
                DetachedPartActions.Clear(); //onPartAttach fires before this point, need to get rid of the actions that adds to this list.
                StaticData.CurrentVesselActions.Clear();
                //EditorLoadFromFile();
                EditorLoadFromNode();
                Log.detail("Ship Load of type NORMAL detected");
            }
            else
            {
                Log.detail("Ship Load of type MERGE detected");
            }
        }

        public void StartLoadWindowPositions()
        {
            string errLine = "1";
            try
            {
                int WinX;
                int WinY;
                if (Int32.TryParse((string)AGExtNode.GetValue("EdGroupsX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("EdGroupsY"), out WinY))
                {
                    GroupsWin = new Rect(WinX, WinY, 250, 530);
                }
                else
                {
                    GroupsWin = new Rect(100, 100, 250, 530);
                }

                errLine = "10";
                if (Int32.TryParse((string)AGExtNode.GetValue("EdSelPartsX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("EdSelPartsY"), out WinY))
                {
                    SelPartsWin = new Rect(WinX, WinY, 365, 270);
                }
                else
                {
                    SelPartsWin = new Rect(100, 100, 365, 270);
                }

                errLine = "11";
                if (Int32.TryParse((string)AGExtNode.GetValue("EdKeyCodeX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("EdKeyCodeY"), out WinY))
                {
                    KeyCodeWin = new Rect(WinX, WinY, 410, 730);
                }
                else
                {
                    KeyCodeWin = new Rect(100, 100, 410, 730);
                }

                errLine = "12";
                if (Int32.TryParse((string)AGExtNode.GetValue("EdKeySetX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("EdKeySetY"), out WinY))
                {
                    KeySetWin = new Rect(WinX, WinY, 185, 335);
                }
                else
                {
                    KeySetWin = new Rect(100, 100, 185, 335);
                }

                errLine = "13";
                if (Int32.TryParse((string)AGExtNode.GetValue("EdCurActsX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("EdCurActsY"), out WinY))
                {
                    CurActsWin = new Rect(WinX, WinY, 345, 140);
                }
                else
                {
                    CurActsWin = new Rect(100, 100, 345, 140);
                }
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
            }
        }

        private void OnRightButtonClick()
        {
            Log.detail("rgt btn click {0}", Time.realtimeSinceStartup);
            showAGXRightClickMenu = !showAGXRightClickMenu;
        }

        private void OnLeftButtonClick()
        {
            Log.detail("lft btn click {0}", Time.realtimeSinceStartup);
            try
            {
                if (showCareerStockAGs)
                {
                    Log.trace("onLeftButtonClick, EditorLogic.fetch.editorScreen: {0}", EditorLogic.fetch.editorScreen);
                    if (EditorLogic.fetch.editorScreen != EditorScreen.Actions)
                    {
                        if (EditorLogic.SortedShipList.Count > 0)
                        {
                            EditorLogic.fetch.SelectPanelActions();
                        }
                        Log.trace("no editor");
                    }
                    else
                    {
                        Log.trace("iseditor");
                        if (AGXShow)
                        {
                            AGXShow = false;
                            AGExtNode.SetValue("EditShow", "0");
                        }
                        else
                        {
                            AGXShow = true;
                            AGExtNode.SetValue("EditShow", "1");
                        }
                        AGXStaticData.SaveBaseConfigNode(AGExtNode);
                    }
                }
                else
                {
                    // ScreenMessages.PostScreenMessage("Action Groups Unavailable. Facility Upgrade Required.");
                    ScreenMessages.PostScreenMessage(Localizer.Format("#AGEXT_UI_SCREEN_MESSAGE_2"));
                }
            }
            catch
            {
                //EdtiorLogic was null, do nothing so silent fail is okay
            }
        }

        public void OnPartEvent(ConstructionEventType cType, Part p)
        {
            Log.trace("Part event fire!");
            if (cType == ConstructionEventType.PartAttached || cType == ConstructionEventType.PartCreated)
            {
                StartCoroutine(CheckStockCustomActions(p)); //check stock actions on delay
            }
            if (cType == ConstructionEventType.PartDeleted) //stop timer if part is deleted to prevent nullrefs
            {
                DisablePartAttachingReset.Start();
            }
        }

        IEnumerator CheckStockCustomActions(Part p) //check a part for custom actions to add to AGX (support for Default Action Groups Mod requies the delay)
        {
            yield return new WaitForSeconds(0.1f); 
            foreach(PartModule pm in p.Modules) 
            {
                foreach(BaseAction ba2 in pm.Actions) 
                {
                    if (!(ba2.actionGroup == KSPActionGroup.None))//will cause false positives on actions with Default Groups assigned, but still worth it
                    {
                        if ((ba2.actionGroup & KSPActionGroup.Custom01) == KSPActionGroup.Custom01)
                        {
                            AGXAction ToAdd = new AGXAction();
                            ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = 1, activated = false };
                            if (!StaticData.CurrentVesselActions.Contains(ToAdd)) //add action to main list
                            {
                                StaticData.CurrentVesselActions.Add(ToAdd);
                            }
                            //do NOT break; this, action could be in multiple action groups (although unlikely)
                        }

                        if ((ba2.actionGroup & KSPActionGroup.Custom02) == KSPActionGroup.Custom02)
                        {
                            AGXAction ToAdd = new AGXAction();
                            ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = 2, activated = false };
                            if (!StaticData.CurrentVesselActions.Contains(ToAdd)) //add action to main list
                            {
                                StaticData.CurrentVesselActions.Add(ToAdd);
                            }
                        }

                        if ((ba2.actionGroup & KSPActionGroup.Custom03) == KSPActionGroup.Custom03)
                        {
                            AGXAction ToAdd = new AGXAction();
                            ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = 3, activated = false };
                            if (!StaticData.CurrentVesselActions.Contains(ToAdd)) //add action to main list
                            {
                                StaticData.CurrentVesselActions.Add(ToAdd);
                            }
                        }

                        if ((ba2.actionGroup & KSPActionGroup.Custom04) == KSPActionGroup.Custom04)
                        {
                            AGXAction ToAdd = new AGXAction();
                            ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = 4, activated = false };
                            if (!StaticData.CurrentVesselActions.Contains(ToAdd)) //add action to main list
                            {
                                StaticData.CurrentVesselActions.Add(ToAdd);
                            }
                        }

                        if ((ba2.actionGroup & KSPActionGroup.Custom05) == KSPActionGroup.Custom05)
                        {
                            AGXAction ToAdd = new AGXAction();
                            ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = 5, activated = false };
                            if (!StaticData.CurrentVesselActions.Contains(ToAdd)) //add action to main list
                            {
                                StaticData.CurrentVesselActions.Add(ToAdd);
                            }
                        }

                        if ((ba2.actionGroup & KSPActionGroup.Custom06) == KSPActionGroup.Custom06)
                        {
                            AGXAction ToAdd = new AGXAction();
                            ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = 6, activated = false };
                            if (!StaticData.CurrentVesselActions.Contains(ToAdd)) //add action to main list
                            {
                                StaticData.CurrentVesselActions.Add(ToAdd);
                            }
                        }

                        if ((ba2.actionGroup & KSPActionGroup.Custom07) == KSPActionGroup.Custom07)
                        {
                            AGXAction ToAdd = new AGXAction();
                            ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = 7, activated = false };
                            if (!StaticData.CurrentVesselActions.Contains(ToAdd)) //add action to main list
                            {
                                StaticData.CurrentVesselActions.Add(ToAdd);
                            }
                        }

                        if ((ba2.actionGroup & KSPActionGroup.Custom08) == KSPActionGroup.Custom08)
                        {
                            AGXAction ToAdd = new AGXAction();
                            ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = 8, activated = false };
                            if (!StaticData.CurrentVesselActions.Contains(ToAdd)) //add action to main list
                            {
                                StaticData.CurrentVesselActions.Add(ToAdd);
                            }
                        }

                        if ((ba2.actionGroup & KSPActionGroup.Custom09) == KSPActionGroup.Custom09)
                        {
                            AGXAction ToAdd = new AGXAction();
                            ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = 9, activated = false };
                            if (!StaticData.CurrentVesselActions.Contains(ToAdd)) //add action to main list
                            {
                                StaticData.CurrentVesselActions.Add(ToAdd);
                            }
                        }

                        if ((ba2.actionGroup & KSPActionGroup.Custom10) == KSPActionGroup.Custom10)
                        {
                            AGXAction ToAdd = new AGXAction();
                            ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = 10, activated = false };
                            if (!StaticData.CurrentVesselActions.Contains(ToAdd)) //add action to main list
                            {
                                StaticData.CurrentVesselActions.Add(ToAdd);
                            }
                        }
                    }
                }
            }
           // Log.Info("stock check end");
        }

        public void PartAttaching(GameEvents.HostTargetAction<Part, Part> host_target)
        {
            Log.detail("Part attached fire!"); //+ StaticData.CurrentVesselActions.Count() + "||" + EditorLogic.fetch.FSMStarted);
            string ErrLine = "1";
            try
            {
                ErrLine = "2";

                ErrLine = "6";
                Log.trace("1 {0}", host_target.host.name);
                Log.trace("2 {0}", host_target.host.Modules.Count);

                ErrLine = "7";
                ModuleAGX agxMod = host_target.host.Modules.OfType<ModuleAGX>().First();

                ErrLine = "8";
                Log.trace("Part attache fire2! {0}", agxMod.agxActionsThisPart.Count);
                foreach (AGXAction agAct in agxMod.agxActionsThisPart)
                {
                    ErrLine = "9";

                    if (!StaticData.CurrentVesselActions.Contains(agAct))
                    {
                        Log.trace("part attached detect"); 
                        ErrLine = "10";
                        StaticData.CurrentVesselActions.Add(agAct);

                        ErrLine = "11";

                        ErrLine = "11b";
                        if (AGXguiNames[agAct.group].Length == 0)
                        {
                            ErrLine = "12";
                            AGXguiNames[agAct.group] = agAct.grpName;
                            ErrLine = "13";
                        }

                        ErrLine = "14";
                    }
                    else
                    {
                        Log.trace("part attached not detect"); 
                    }
                    ErrLine = "15";

                    if (!disablePartAttaching)
                    {
                        DetachedPartActions.Add(agAct);
                    }
                }

                ErrLine = "16";
                AttachAGXPart(host_target.host);
                foreach (Part p in host_target.host.FindChildParts<Part>(true)) //action only fires for part clicked on, have to parse all child parts this way
                {
                    ErrLine = "17";
                    agxMod = p.Modules.OfType<ModuleAGX>().First();
                    ErrLine = "18";
                    foreach (AGXAction agAct in agxMod.agxActionsThisPart)
                    {
                        ErrLine = "19";
                        if (!disablePartAttaching)
                        {
                            DetachedPartActions.Add(agAct);
                        }
                        if (!StaticData.CurrentVesselActions.Contains(agAct))
                        {
                            //Log.dbg("adding action " + agAct.ba.guiName + agAct.group);
                            ErrLine = "20";
                            StaticData.CurrentVesselActions.Add(agAct);

                            ErrLine = "21";
                            if (AGXguiNames[agAct.group].Length == 0)
                            {
                                ErrLine = "22";
                                AGXguiNames[agAct.group] = agAct.grpName;
                                ErrLine = "23";
                            }
                        }
                    }
                    AttachAGXPart(p);
                }

                DetachedPartReset.Start();
                Log.trace("Part attache fire end! {0}", StaticData.CurrentVesselActions.Count());
            }
            catch (Exception e)
            {
                Log.err("Part Attaching Fail: {0}", ErrLine);
                Log.ex(this, e);
            }
        }

        public void PartRemove(GameEvents.HostTargetAction<Part, Part> host_target)
        {
            string errLine = "1";
            try
            {
                Log.trace("Part Remove Event Fire!");
                disablePartAttaching = false;
                //.Log("AGX Part Remove Fire " + StaticData.CurrentVesselActions.Count());
                errLine = "2";
                UpdateAGXActionGroupNames();
                errLine = "3";
                //Log.dbg("Part detached! " + host_target.target.ConstructID);
                DetachedPartActions.AddRange(StaticData.CurrentVesselActions.Where(p3 => p3.ba.listParent.part == host_target.target)); //add actiongroups on this part to List
                errLine = "4";
                foreach (Part p in host_target.target.FindChildParts<Part>(true)) //action only fires for part clicked on, have to parse all child parts this way
                {
                    errLine = "5";
                    // print("Part detached2! " + p.ConstructID);
                    DetachedPartActions.AddRange(StaticData.CurrentVesselActions.Where(p3 => p3.ba.listParent.part == p)); //add parts to list
                    errLine = "6";
                }
                errLine = "7";
                DetachedPartReset.Stop(); //stop timer so it resets

                errLine = "8";
                ModuleAGX agxMod = host_target.target.Modules.OfType<ModuleAGX>().First();
                errLine = "9";
                agxMod.agxActionsThisPart.AddRange(StaticData.CurrentVesselActions.Where(p3 => p3.ba.listParent.part == host_target.target));

                errLine = "10";
                foreach (Part p in host_target.target.FindChildParts<Part>(true)) //action only fires for part clicked on, have to parse all child parts this way
                {
                    errLine = "11";
                    agxMod = p.Modules.OfType<ModuleAGX>().First();
                    errLine = "12";
                    agxMod.agxActionsThisPart.AddRange(StaticData.CurrentVesselActions.Where(p3 => p3.ba.listParent.part == p)); //add parts to list
                    errLine = "13";
                }
                errLine = "14";
                Log.trace("Part Remove Fire {0}", StaticData.CurrentVesselActions.Count());
            }
            catch (Exception e)
            {
                Log.err("PartRemove FAIL {0}", errLine);
                Log.ex(this, e);
            }

        }

        public static void UpdateAGXActionGroupNames()
        {
            foreach (AGXAction agAct in StaticData.CurrentVesselActions)
            {
                try
                {
                    agAct.grpName = AGXguiNames[agAct.group];
                }
                catch
                {
                    //empty, silently fail
                }
            }
        }

        public static void LoadGroupVisibility(string LoadString)
        {
            string errLine = "1";
            try
            {
                if (LoadString.Length == 1501)
                {
                    errLine = "15";
                    ShowGroupInFlightCurrent = Convert.ToInt32(LoadString.Substring(0, 1));
                    errLine = "16";
                    LoadString = LoadString.Substring(1);
                    errLine = "17";
                    for (int i = 1; i <= 250; i++)
                    {
                        errLine = "18";
                        if (LoadString[0] == '1')
                        {
                            IsGroupToggle[i] = true;
                        }
                        else
                        {
                            IsGroupToggle[i] = false;
                        }
                        LoadString = LoadString.Substring(1);
                        //ReturnStr = ReturnStr + Convert.ToInt16(IsGroupToggle[i]).ToString(); //add toggle state for group
                        for (int i2 = 1; i2 <= 5; i2++)
                        {
                            if (LoadString[0] == '1')
                            {
                                ShowGroupInFlight[i2, i] = true;
                            }
                            else
                            {
                                ShowGroupInFlight[i2, i] = false;
                            }
                            LoadString = LoadString.Substring(1);
                            //ReturnStr = ReturnStr + Convert.ToInt16(ShowGroupInFlight[i2, i]).ToString(); //add flight state visibility for each group
                        }
                    }
                    errLine = "19";
                }
                else
                {
                    for (int i = 1; i <= 250; i++)
                    {
                        errLine = "20";
                        IsGroupToggle[i] = false;
                        for (int i2 = 1; i2 <= 5; i2++)
                        {
                            ShowGroupInFlight[i2, i] = true;
                        }
                    }
                }
                errLine = "21";
            }
            catch (Exception e)
            {
                Log.err("LoadGroupVisibility Fail {0}", errLine);
                Log.ex(typeof(AGXEditor), e);
            }
        }

        public void ImmediatePanelsMovement()
        {
            Log.trace("immedieate");
            if (EditorLogic.fetch.editorScreen == EditorScreen.Parts)
            {
                EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.actions);
            }
            else if (EditorLogic.fetch.editorScreen == EditorScreen.Crew)
            {
                EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.crew);
            }
            else if (EditorLogic.fetch.editorScreen == EditorScreen.Actions)
            {
                if (!AGXShow)
                {
                    EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.crew);
                }
                else
                {
                    EditorPanels.Instance.panelManager.Dismiss(EditorPanels.Instance.actions);
                }
            }
        }

        IEnumerator DelayPanelsMovement()
        {
            yield return new WaitForSeconds(0.2f);
            ActualPanelsMovement();
        }

        IEnumerator DelayPanelsMovementSecondStage() //run panels a second time so queue is used up
        {
            yield return new WaitForSeconds(0.2f);
            ImmediatePanelsMovement();
        }

        IEnumerator DelayHidePanels()
        {
            yield return new WaitForSeconds(0.2f);
            Log.detail("delay hide");
            EditorPanels.Instance.panelManager.Dismiss(EditorPanels.Instance.actions);
        }

        IEnumerator DelayShowPanels()
        {
            yield return new WaitForSeconds(0.2f);
            Log.detail("delay show");
            EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.actions);
            //ActualPanelsMovement();
        }

        public void ActualPanelsMovement()
        {
            Log.dbg("mobe pbls {0}", EditorLogic.fetch.editorScreen);
            Log.dbg("trying2 {0}",  EditorLogic.fetch.editorScreen);
            if (EditorLogic.fetch.editorScreen == EditorScreen.Parts)
            {
                Log.dbg("hit it");

                EditorLogic.fetch.SelectPanelParts();
                EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.partsEditor); //this works
                EditorLogic.fetch.Unlock("AGXLock");
                AGEditorSelectedParts.Clear();
                EditorSaveToNode();
                EditorLogic.fetch.SetBackup();
            }
            else if (EditorLogic.fetch.editorScreen == EditorScreen.Crew)
            {
                Log.dbg("hit it2");

                EditorLogic.fetch.SelectPanelCrew();
                EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.crew); //this works
                EditorLogic.fetch.Unlock("AGXLock");
                AGEditorSelectedParts.Clear();
                EditorSaveToNode();
                EditorLogic.fetch.SetBackup();
            }
            else if (EditorLogic.fetch.editorScreen == EditorScreen.Actions)
            {
                Log.dbg("hit it3");

                EditorLogic.fetch.SelectPanelActions();
                if (AGXShow)
                {
                    Log.dbg("hit it3a");
                    StartCoroutine("DelayHidePanels");
                    EditorLogic.fetch.Unlock("AGXLock");
                    AGEditorSelectedParts.Clear();
                    EditorSaveToNode();
                    EditorLogic.fetch.SetBackup();
                }
                else
                {
                    // Log.dbg("hit it3b" + EditorLogic.fetch.editorScreen);
                    StartCoroutine("DelayShowPanels");
                    EditorLogic.fetch.Unlock("AGXLock");
                    AGEditorSelectedParts.Clear();
                    EditorSaveToNode();
                    EditorLogic.fetch.SetBackup();
                }
            }

            //StartCoroutine("DelayPanelsMovementSecondStage");
        }

        public void SetDefaultAction(BaseAction ba, int group)
        {
            ba.actionGroup = ba.actionGroup | CONST.CustomActions[group];
        }

        public static void SetDefaultActionStatic(BaseAction ba, int group)
        {
            ba.actionGroup = ba.actionGroup | CONST.CustomActions[group];
        }

        public static void ResetDisablePartAttaching(object source, ElapsedEventArgs e)
        {
            Log.dbg("Part attaching reset");
            DisablePartAttachingReset.Stop();
            disablePartAttaching = true;
        }

        public static void ResetDetachedParts(object source, ElapsedEventArgs e)
        {
            Log.dbg("Detached parts start {0}", StaticData.CurrentVesselActions.Count());
            DetachedPartReset.Stop();
            //check stock custim actions here
            foreach (AGXAction agAct in DetachedPartActions)
            {
                Log.detail("DetachedPartActions {0}", DetachedPartActions.Count());
                foreach (Part p in agAct.prt.symmetryCounterparts)
                {
                    AGXAction actToAdd = StaticData.LoadAGXActionVer2(StaticData.SaveAGXActionVer2(agAct), p, false);
                    if (actToAdd != null && !StaticData.CurrentVesselActions.Contains(actToAdd))
                    {
                        StaticData.CurrentVesselActions.Add(actToAdd);
                        //Log.dbg("add act");
                    }
                    p.Modules.OfType<ModuleAGX>().First().agxActionsThisPart.Clear();
                }
            }
            DetachedPartActions.Clear();
            EditorSaveToNode();
            DisablePartAttachingReset.Start();
            Log.dbg("Detached parts end {0}", StaticData.CurrentVesselActions.Count());
        }

        public static void AttachAGXPart(Part p)
        {
            //Log.dbg("d1");
            if (DetachedPartActions.Count(a => a.ba.listParent.part == p) == 0) //part has no actions in list, may be a clone
            {
                //Log.dbg("d2");
                foreach (Part p2 in p.symmetryCounterparts) //check any symmetry counterparts
                {
                    //Log.dbg("d3");
                    if (DetachedPartActions.Count(a => a.ba.listParent.part == p2) > 0) //symmetry counterpart has at least one action
                    {
                        //Log.dbg("d4");
                        foreach (AGXAction agAct in DetachedPartActions.Where(p3 => p3.ba.listParent.part == p2))
                        {
                            //Log.dbg("d5");
                            AGXAction actToAdd = StaticData.LoadAGXActionVer2(StaticData.SaveAGXActionVer2(agAct), p, false);
                            if (actToAdd != null)
                            {
                                List<AGXAction> Checking = new List<AGXAction>();
                                Checking.AddRange(StaticData.CurrentVesselActions);
                                Checking.RemoveAll(p4 => p4.ba != actToAdd.ba);
                                Checking.RemoveAll(p5 => p5.group != actToAdd.group);
                                if (Checking.Count == 0)
                                {
                                    StaticData.CurrentVesselActions.Add(actToAdd);
                                    //Log.dbg("d6");
                                    if (actToAdd.group < 11)
                                    {
                                        SetDefaultActionStatic(actToAdd.ba, actToAdd.group);
                                    }
                                }
                                else
                                {
                                    //Log.dbg("d6 fail");
                                }
                            }
                        }
                    }
                }
            }
        }

        public void LeavingEditor(GameScenes gScn)
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                //Log.dbg("LEaving editor");
                EditorSaveToFile();
            }
        }

        public void OnDisable()
        {
            string errLine = "1";
            try
            {
                LoadFinished = false;
                errLine = "2";
                SaveCurrentKeyBindings();
                errLine = "3";
                SaveWindowPositions();
                errLine = "4";

                errLine = "7";
                GameEvents.onPartAttach.Remove(PartAttaching);
                errLine = "8";
                GameEvents.onPartRemove.Remove(PartRemove);
                errLine = "9";
                GameEvents.onEditorPartEvent.Remove(OnPartEvent);
                errLine = "10";
                GameEvents.onEditorLoad.Remove(OnShipLoad);
                errLine = "11";
                InputLockManager.RemoveControlLock("AGXLock");
                errLine = "12";
                StaticData.CurrentVesselActions.Clear();
                errLine = "13";
            }
            catch (Exception e)
            {
                Log.err("Editor.OnDestroy ", errLine);
                Log.ex(this,e);
            }
        }

        public static void SaveWindowPositions()
        {
            AGExtNode.SetValue("EdGroupsX", GroupsWin.x.ToString());
            AGExtNode.SetValue("EdGroupsY", GroupsWin.y.ToString());
            AGExtNode.SetValue("EdSelPartsX", SelPartsWin.x.ToString());
            AGExtNode.SetValue("EdSelPartsY", SelPartsWin.y.ToString());
            AGExtNode.SetValue("EdKeyCodeX", KeyCodeWin.x.ToString());
            AGExtNode.SetValue("EdKeyCodeY", KeyCodeWin.y.ToString());
            AGExtNode.SetValue("EdKeySetX", KeySetWin.x.ToString());
            AGExtNode.SetValue("EdKeySetY", KeySetWin.y.ToString());
            AGExtNode.SetValue("EdCurActsX", CurActsWin.x.ToString());
            AGExtNode.SetValue("EdCurActsY", CurActsWin.y.ToString());
            AGXStaticData.SaveBaseConfigNode(AGExtNode);
        }

        private static bool JoySticks(String s)
        {
            return s.StartsWith("Joystick");
        }

        void AddButtons()
        {
            Log.trace("AddButton");
            Toolbar.Button button = Toolbar.Button.Create(this
                    , ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH
                    , UI.icon_button_38
                    , UI.icon_button_24
                );
            button.Mouse
                .Add(Toolbar.Button.MouseEvents.Kind.Left, OnLeftButtonClick)
                .Add(Toolbar.Button.MouseEvents.Kind.Right, OnRightButtonClick)
            ;
            ToolbarController.Instance.Add(button);
        }

        public void OnGUI()
        {
            //Log.dbg("start ondraw draw");
            Vector3 RealMousePos = new Vector3();
            RealMousePos = Input.mousePosition;
            RealMousePos.y = Screen.height - Input.mousePosition.y;

            ControlTypes i = InputLockManager.GetControlLock("ExitConfirmationDialog");

            if ((i & ControlTypes.EDITOR_EXIT) == ControlTypes.EDITOR_EXIT)
                Log.err("EditorExit active");

            if (AGXShow &&
                (i & ControlTypes.EDITOR_EXIT) != ControlTypes.EDITOR_EXIT
                )
            {
                // print("Test call 3" + showCareerCustomAGs);
                if (!showCareerCustomAGs)
                {
                    //Log.dbg("Test call");
                    defaultShowingNonNumeric = true;
                }
                if (ShowKeySetWin)
                {
                    KeySetWin = GUI.Window(673467792, KeySetWin, KeySetWindow, Localizer.Format("#AGEXT_UI_key_sets"), UI.AGXWinStyle);
                    TrapMouse = KeySetWin.Contains(RealMousePos);
                    if (!AutoHideGroupsWin)
                    {
                        GroupsWin = GUI.Window(673467795, GroupsWin, GroupsWindow, "", UI.AGXWinStyle);
                    }
                    ShowCurActsWin = false;

                }

                if (ShowSelectedWin)
                {
                    //Log.dbg("start selparts draw");
                    SelPartsWin = GUI.Window(673467794, SelPartsWin, SelParts, Localizer.Format("#AGEXT_UI_selected_parts_numbers_title") + AGEditorSelectedParts.Count(), UI.AGXWinStyle);
                    // print("end selparts draw");
                    ShowCurActsWin = true;
                    TrapMouse = SelPartsWin.Contains(RealMousePos);
                    if (AutoHideGroupsWin && !TempShowGroupsWin)
                    {
                    }
                    else
                    {
                        GroupsWin = GUI.Window(673467795, GroupsWin, GroupsWindow, "", UI.AGXWinStyle);
                        TrapMouse |= GroupsWin.Contains(RealMousePos);
                    }

                    if (ShowKeyCodeWin)
                    {
                        KeyCodeWin = GUI.Window(673467793, KeyCodeWin, KeyCodeWindow, "                Keycodes", UI.AGXWinStyle);
                        TrapMouse |= KeyCodeWin.Contains(RealMousePos);
                    }

                }

                if (ShowCurActsWin && ShowSelectedWin)
                {
                    // CurActsWin = GUI.Window(673467790, CurActsWin, CurrentActionsWindow, "Actions (This group): " + StaticData.CurrentVesselActions.FindAll(p => p.group == AGXCurActGroup).Count.ToString(), AGXWinStyle);
                    CurActsWin = GUI.Window(673467790, CurActsWin, CurrentActionsWindow, Localizer.Format("#AGEXT_UI_action_this_group") + StaticData.CurrentVesselActions.FindAll(p => p.group == AGXCurActGroup).Count.ToString(), UI.AGXWinStyle);
                    TrapMouse |= CurActsWin.Contains(RealMousePos);
                }

                string ErrLine = "1";
                try
                {
                    ErrLine = "2";

                    ErrLine = "7";
                    if (ShowSelectedWin)
                    {
                        if (defaultShowingNonNumeric)
                        {
                            foreach (BaseAction Act in defaultActionsListThisType)
                            {
                                ErrLine = "8";
                                Vector3 partScreenPosD = EditorLogic.fetch.editorCamera.WorldToScreenPoint(Act.listParent.part.transform.position);
                                ErrLine = "9";
                                Rect partCenterWinD = new Rect(partScreenPosD.x - 10, (Screen.height - partScreenPosD.y) - 10, 21, 21);
                                ErrLine = "10";
                                GUI.DrawTexture(partCenterWinD, UI.PartPlus);
                            }
                        }
                        else
                        {
                            foreach (AGXAction agAct in ThisGroupActions)
                            {
                                try
                                {
                                    ErrLine = "8";
                                    Vector3 partScreenPosC = EditorLogic.fetch.editorCamera.WorldToScreenPoint(agAct.ba.listParent.part.transform.position);
                                    ErrLine = "9";
                                    Rect partCenterWinC = new Rect(partScreenPosC.x - 10, (Screen.height - partScreenPosC.y) - 10, 21, 21);
                                    ErrLine = "10";
                                    GUI.DrawTexture(partCenterWinC, UI.PartPlus);
                                }
                                catch
                                {
                                    //silent fail, this nullrefs for one update frame when hitting the "New" button in editor.
                                }
                            }
                        }
                        foreach (AGXPart agPrt in AGEditorSelectedParts)
                        {
                            try
                            {
                                ErrLine = "3";
                                Vector3 partScreenPosB = EditorLogic.fetch.editorCamera.WorldToScreenPoint(agPrt.AGPart.transform.position);
                                ErrLine = "4";
                                Rect partCenterWinB = new Rect(partScreenPosB.x - 10, (Screen.height - partScreenPosB.y) - 10, 21, 21);
                                ErrLine = "5";
                                GUI.DrawTexture(partCenterWinB, UI.PartCross);
                                ErrLine = "6";
                            }
                            catch
                            {
                                //silent fail, this nullrefs for one update frame when hitting the "New" button in editor.
                            }
                        }
                    }

                    ErrLine = "11";
                    if (showAGXRightClickMenu)
                    {
                        Rect SettingsWinEditor = new Rect(Screen.width - 200, Screen.height - 150, 150, 110);
                        // GUI.Window(2233452, SettingsWinEditor, DrawSettingsWinEditor, "AGX Settings", AGXEditor.AGXWinStyle);
                        GUI.Window(2233452, SettingsWinEditor, DrawSettingsWinEditor, Localizer.Format("#AGEXT_UI_agx_settings"), UI.AGXWinStyle);
                    }
                }
                catch (Exception e)
                {
                    Log.err("Draw cross fail. {0}", ErrLine);
                    Log.ex(this, e);
                }
            }
            // print("Truth check " + highlightPartThisFrameActsWin + " " + highlightPartThisFrameSelWin);
            if (highlightPartThisFrameActsWin || highlightPartThisFrameSelWin || highlightPartThisFrameGroupWin)//highlight mouse over cross
            {
                // print("mouse over");
                //Log.dbg("screen pos " + EditorLogic.fetch.editorCamera.WorldToScreenPoint(partToHighlight.transform.position));
                //Log.dbg("orgpos" + partToHighlight.);
                Vector3 partScreenPos = EditorLogic.fetch.editorCamera.WorldToScreenPoint(partToHighlight.transform.position);
                Rect partCenterWin = new Rect(partScreenPos.x - 20, (Screen.height - partScreenPos.y) - 20, 41, 41);
                GUI.DrawTexture(partCenterWin, UI.PartCenter);
            }
            //Log.dbg("end ondraw draw");
        }

        public void CurrentActionsWindow(int WindowID)
        {
            ThisGroupActions = new List<AGXAction>();
            ThisGroupActions.AddRange(StaticData.CurrentVesselActions.Where(p => p.group == AGXCurActGroup));
            GUI.Box(new Rect(5, 25, 310, 110), "");
            CurGroupsWin = GUI.BeginScrollView(new Rect(10, 30, 330, 100), CurGroupsWin, new Rect(0, 0, 310, Math.Max(100, 0 + (20 * (ThisGroupActions.Count)))));
            int RowCnt = 1;
            highlightPartThisFrameActsWin = false;
            if (ThisGroupActions.Count > 0)
            {
                while (RowCnt <= ThisGroupActions.Count)
                {
                    if (Mouse.screenPos.y >= CurActsWin.y + 30 && Mouse.screenPos.y <= CurActsWin.y + 130 && new Rect(CurActsWin.x + 10, (CurActsWin.y + 30 + ((RowCnt - 1) * 20)) - CurGroupsWin.y, 300, 20).Contains(Mouse.screenPos))
                    {
                        highlightPartThisFrameActsWin = true;
                        //Log.dbg("part found to highlight acts " + highlightPartThisFrameActsWin + " " + ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.transform.position);
                        partToHighlight = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part;
                    }

                    if (GUI.Button(new Rect(0, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).group.ToString() + ": " + AGXguiNames[ThisGroupActions.ElementAt(RowCnt - 1).group], UI.AGXBtnStyleMiddleLeft))
                    {
                        int ToDel = 0;
                        foreach (AGXAction AGXToRemove in StaticData.CurrentVesselActions)
                        {
                            if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                            {
                                StaticData.CurrentVesselActions.RemoveAt(ToDel);
                                goto BreakOutA;
                            }
                            ++ToDel;
                        }
                    BreakOutA:
                        if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                        {
                            RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                        }
                        ModuleAGX agxMod = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.Modules.OfType<ModuleAGX>().First();
                        agxMod.agxActionsThisPart.RemoveAll(p => p == ThisGroupActions.ElementAt(RowCnt - 1));
                    }

                    if (GUI.Button(new Rect(100, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).prt.partInfo.title, UI.AGXBtnStyleMiddleLeft))
                    {
                        int ToDel = 0;
                        foreach (AGXAction AGXToRemove in StaticData.CurrentVesselActions)
                        {
                            if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                            {
                                StaticData.CurrentVesselActions.RemoveAt(ToDel);
                                goto BreakOutB;
                            }
                            ++ToDel;
                        }
                    BreakOutB:
                        if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                        {
                            RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                        }

                        ModuleAGX agxMod2 = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.Modules.OfType<ModuleAGX>().First();
                        agxMod2.agxActionsThisPart.RemoveAll(p => p == ThisGroupActions.ElementAt(RowCnt - 1));
                    }
                    try
                    {
                        if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).ba.guiName, UI.AGXBtnStyleMiddleLeft))
                        {
                            int ToDel = 0;
                            foreach (AGXAction AGXToRemove in StaticData.CurrentVesselActions)
                            {
                                if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                                {
                                    StaticData.CurrentVesselActions.RemoveAt(ToDel);
                                    goto BreakOutC;
                                }
                                ++ToDel;
                            }
                        BreakOutC:
                            //SaveCurrentVesselActions();
                            if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                            {
                                RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                            }
                            ModuleAGX agxMod3 = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.Modules.OfType<ModuleAGX>().First();
                            agxMod3.agxActionsThisPart.RemoveAll(p => p == ThisGroupActions.ElementAt(RowCnt - 1));
                        }
                    }
                    catch
                    {
                        if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), "error", UI.AGXBtnStyleMiddleLeft))
                        {
                            int ToDel = 0;
                            foreach (AGXAction AGXToRemove in StaticData.CurrentVesselActions)
                            {
                                if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                                {
                                    StaticData.CurrentVesselActions.RemoveAt(ToDel);
                                    goto BreakOutD;
                                }
                                ++ToDel;
                            }
                        BreakOutD:
                            //SaveCurrentVesselActions();
                            if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                            {
                                RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                            }
                        }
                    }

                    ++RowCnt;
                }
            }
            else
            {
                GUI.Label(new Rect(10, 30, 274, 30), Localizer.Format("#AGEXT_UI_no_actions"), UI.AGXLblStyleMiddleCenter);
            }
            GUI.EndScrollView();

            GUI.DragWindow();
        }

        public void KeySetWindow(int WindowID)
        {
            GUIStyle btnStyle = CurrentKeySet == 1 ? UI.AGXBtnStyleEnabled : UI.AGXBtnStyle;
            if (GUI.Button(new Rect(5, 25, 70, 20), Localizer.Format("#AGEXT_UI_select_1"), btnStyle))
            {
                EditorSaveKeysetStuff();
                SaveCurrentKeyBindings();

                CurrentKeySet = 1;
                CurrentKeySetName = KeySetNames[0];
                LoadCurrentKeyBindings();
            }
            KeySetNames[0] = GUI.TextField(new Rect(80, 25, 100, 20), KeySetNames[0], btnStyle);

            btnStyle = CurrentKeySet == 2 ? UI.AGXBtnStyleEnabled : UI.AGXBtnStyle;
            if (GUI.Button(new Rect(5, 50, 70, 20), Localizer.Format("#AGEXT_UI_select_2"), btnStyle))
            {
                EditorSaveKeysetStuff();
                SaveCurrentKeyBindings();
                CurrentKeySet = 2;
                CurrentKeySetName = KeySetNames[1];
                LoadCurrentKeyBindings();
            }
            KeySetNames[1] = GUI.TextField(new Rect(80, 50, 100, 20), KeySetNames[1], btnStyle);

            btnStyle = CurrentKeySet == 3 ? UI.AGXBtnStyleEnabled : UI.AGXBtnStyle;
            if (GUI.Button(new Rect(5, 75, 70, 20), Localizer.Format("#AGEXT_UI_select_3"), btnStyle))
            {
                EditorSaveKeysetStuff();
                SaveCurrentKeyBindings();
                CurrentKeySet = 3;
                CurrentKeySetName = KeySetNames[2];
                LoadCurrentKeyBindings();
            }
            KeySetNames[2] = GUI.TextField(new Rect(80, 75, 100, 20), KeySetNames[2], btnStyle);

            btnStyle = CurrentKeySet == 4 ? UI.AGXBtnStyleEnabled : UI.AGXBtnStyle;
            if (GUI.Button(new Rect(5, 100, 70, 20), Localizer.Format("#AGEXT_UI_select_4"), btnStyle))
            {
                EditorSaveKeysetStuff();
                SaveCurrentKeyBindings();
                CurrentKeySet = 4;
                CurrentKeySetName = KeySetNames[3];
                LoadCurrentKeyBindings();
            }
            KeySetNames[3] = GUI.TextField(new Rect(80, 100, 100, 20), KeySetNames[3], btnStyle);

            btnStyle = CurrentKeySet == 5 ? UI.AGXBtnStyleEnabled : UI.AGXBtnStyle;
            if (GUI.Button(new Rect(5, 125, 70, 20), Localizer.Format("#AGEXT_UI_select_5"), btnStyle))
            {
                EditorSaveKeysetStuff();
                SaveCurrentKeyBindings();
                CurrentKeySet = 5;
                CurrentKeySetName = KeySetNames[4];
                LoadCurrentKeyBindings();
            }
            KeySetNames[4] = GUI.TextField(new Rect(80, 125, 100, 20), KeySetNames[4], btnStyle);

            GUI.Label(new Rect(5, 145, 175, 25), Localizer.Format("#AGEXT_UI_action_group_groups"), UI.AGXLblStyleMiddleCenterBold);

            btnStyle = ShowGroupInFlightCurrent == 1 ? UI.AGXBtnStyleEnabled : UI.AGXBtnStyle;
            if (GUI.Button(new Rect(5, 165, 70, 20), Localizer.Format("#AGEXT_UI_group_1"), btnStyle))
            {
                ShowGroupInFlightCurrent = 1;
            }

            ShowGroupInFlightNames[1] = GUI.TextField(new Rect(80, 165, 100, 20), ShowGroupInFlightNames[1]);
            btnStyle = ShowGroupInFlightCurrent == 2 ? UI.AGXBtnStyleEnabled : UI.AGXBtnStyle;
            if (GUI.Button(new Rect(5, 190, 70, 20), Localizer.Format("#AGEXT_UI_group_2"), btnStyle))
            {
                ShowGroupInFlightCurrent = 2;
            }

            ShowGroupInFlightNames[2] = GUI.TextField(new Rect(80, 190, 100, 20), ShowGroupInFlightNames[2]);
            btnStyle = ShowGroupInFlightCurrent == 3 ? UI.AGXBtnStyleEnabled : UI.AGXBtnStyle;
            if (GUI.Button(new Rect(5, 215, 70, 20), Localizer.Format("#AGEXT_UI_group_3"), btnStyle))
            {
                ShowGroupInFlightCurrent = 3;
            }

            ShowGroupInFlightNames[3] = GUI.TextField(new Rect(80, 215, 100, 20), ShowGroupInFlightNames[3]);
            btnStyle = ShowGroupInFlightCurrent == 4 ? UI.AGXBtnStyleEnabled : UI.AGXBtnStyle;
            if (GUI.Button(new Rect(5, 240, 70, 20), Localizer.Format("#AGEXT_UI_group_4"), btnStyle))
            {
                ShowGroupInFlightCurrent = 4;
            }

            ShowGroupInFlightNames[4] = GUI.TextField(new Rect(80, 240, 100, 20), ShowGroupInFlightNames[4]);
            btnStyle = ShowGroupInFlightCurrent == 5 ? UI.AGXBtnStyleEnabled : UI.AGXBtnStyle;
            if (GUI.Button(new Rect(5, 265, 70, 20), Localizer.Format("#AGEXT_UI_group_5"), btnStyle))
            {
                ShowGroupInFlightCurrent = 5;
            }

            ShowGroupInFlightNames[5] = GUI.TextField(new Rect(80, 265, 100, 20), ShowGroupInFlightNames[5]);
            if (GUI.Button(new Rect(5, 300, 175, 30), Localizer.Format("#AGEXT_UI_close_window"), UI.AGXBtnStyle))
            {
                EditorSaveKeysetStuff();
                ShowKeySetWin = false;
            }
            GUI.DragWindow();
        }

        public static void LoadCurrentKeyBindings()
        {
            //Log.dbg("loading key set " + CurrentKeySet.ToString());          
            string LoadString = AGExtNode.GetValue("KeySet" + CurrentKeySet.ToString());

            for (int i = 1; i <= 250; i++)
            {
                AGXguiKeys[i] = KeyCode.None;
            }

            if (LoadString.Length > 0)
            {
                while (LoadString[0] == '\u2023')
                {
                    LoadString = LoadString.Substring(1);

                    int KeyIndex;
                    KeyCode LoadKey = new KeyCode();
                    int KeyLength = LoadString.IndexOf('\u2023');

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

            string GroupKeysMod1ToLoad = AGExtNode.GetValue("KeySetMod1Group" + CurrentKeySet.ToString());
            string GroupKeysMod2ToLoad = AGExtNode.GetValue("KeySetMod2Group" + CurrentKeySet.ToString());
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
            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), AGExtNode.GetValue("KeySetModKey1" + CurrentKeySet.ToString()));
            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), AGExtNode.GetValue("KeySetModKey2" + CurrentKeySet.ToString()));
        }

        public static void SaveCurrentKeyBindings()
        {
            AGExtNode.SetValue("KeySetName" + CurrentKeySet, CurrentKeySetName);
            string SaveString = "";
            int KeyID = 1;
            while (KeyID <= 250)
            {
                if (AGXguiKeys[KeyID] != KeyCode.None)
                {
                    SaveString = SaveString + '\u2023' + KeyID.ToString("000") + AGXguiKeys[KeyID].ToString();
                }
                ++KeyID;
            }

            AGExtNode.SetValue("KeySet" + CurrentKeySet.ToString(), SaveString);

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

            AGExtNode.SetValue("KeySetMod1Group" + CurrentKeySet.ToString(), GroupsMod1ToSave);
            AGExtNode.SetValue("KeySetMod2Group" + CurrentKeySet.ToString(), GroupsMod2ToSave);
            AGExtNode.SetValue("KeySetModKey1" + CurrentKeySet.ToString(), AGXguiMod1Key.ToString());
            AGExtNode.SetValue("KeySetModKey2" + CurrentKeySet.ToString(), AGXguiMod2Key.ToString());

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

            btnStyle = ShowJoySticks ? UI.AGXBtnStyleEnabled : UI.AGXBtnStyle;
            if (GUI.Button(new Rect(280, 2, 125, 20), Localizer.Format("#AGEXT_UI_show_joy_Sticks"), btnStyle))
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
                        }
                    }
                    ++KeyListCount;
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
                        }
                    }
                    ++KeyListCount;
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
                        }
                    }
                    ++JoyStickCount;
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
                        }
                    }
                    ++JoyStickCount;
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
                        }
                    }
                    ++JoyStickCount;
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

        public void RemoveDefaultAction(BaseAction ba, int group)
        {
            ba.actionGroup = ba.actionGroup & ~CONST.CustomActions[group];
        }

        public void RefreshPartActions()
        {
            PartActionsList.Clear();
            PartActionsList.AddRange(AGEditorSelectedParts.First().AGPart.Actions.Where(ba => ba.active));
            foreach (PartModule pm in AGEditorSelectedParts.First().AGPart.Modules)
            {
                PartActionsList.AddRange(pm.Actions.Where(ba => ba.active));
            }
            //Log.dbg("AGX Actions refresh found actions: " + PartActionsList.Count);
        }

        public void SelParts(int WindowID)
        {
            SelectedPartsCount = AGEditorSelectedParts.Count;
            int SelPartsLeft = -10;

            GUI.Box(new Rect(SelPartsLeft + 20, 25, 200, 110), "");
            highlightPartThisFrameSelWin = false;
            if (AGEditorSelectedParts != null && AGEditorSelectedParts.Count > 0)
            {
                int ButtonCount = 1;

                ScrollPosSelParts = GUI.BeginScrollView(new Rect(SelPartsLeft + 20, 30, 220, 110), ScrollPosSelParts, new Rect(0, 0, 200, (20 * Math.Max(5, SelectedPartsCount)) + 10));

                while (ButtonCount <= SelectedPartsCount)
                {
                    if (Mouse.screenPos.y >= SelPartsWin.y + 30 && Mouse.screenPos.y <= SelPartsWin.y + 140 && new Rect(SelPartsWin.x + SelPartsLeft + 25, (SelPartsWin.y + 30 + ((ButtonCount - 1) * 20)) - ScrollPosSelParts.y, 190, 20).Contains(Mouse.screenPos))
                    {
                        highlightPartThisFrameSelWin = true;
                        //Log.dbg("part found to highlight " + AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart.ConstructID);
                        partToHighlight = AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart;
                    }

                    if (GUI.Button(new Rect(5, 0 + ((ButtonCount - 1) * 20), 190, 20), AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart.partInfo.title, UI.AGXLblStyleMiddleCenter))
                    {
                        AGEditorSelectedParts.RemoveAt(ButtonCount - 1);
                        if (AGEditorSelectedParts.Count == 0)
                        {
                            AGEEditorSelectedPartsSame = false;
                        }
                        return;
                    }

                    ++ButtonCount;
                }
                GUI.EndScrollView();
            }
            else //no parts selected, show list all parts button?
            {
                if (GUI.Button(new Rect(SelPartsLeft + 50, 45, 140, 70), Localizer.Format("#AGEXT_UI_show_list_hint"), UI.AGXBtnStyle)) //button itself
                {
                    showAllPartsListTitles = new List<string>(); //generate list of all parts 
                    showAllPartsListTitles.Clear(); //this probably isn't needed, but it works as is, not messing with it
                    foreach (Part p in EditorLogic.SortedShipList) //cycle all parts
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
                                            Log.dbg("adding act");
                                        }
                                    }
                                }
                                RefreshDefaultActionsListType();
                            }
                            else
                            {
                                int PrtCnt = 0;
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

                                    if (!StaticData.CurrentVesselActions.Contains(ToAdd)) //add action to main list
                                    {
                                        StaticData.CurrentVesselActions.Add(ToAdd);
                                    }

                                    ModuleAGX thisActionModule = ToAdd.ba.listParent.part.Modules.OfType<ModuleAGX>().First(); //add action to partmodules list, yes this duplicates the action but it necessary for add/remove/symmetry stuff in editor
                                    if (!thisActionModule.agxActionsThisPart.Contains(ToAdd))
                                    {
                                        thisActionModule.agxActionsThisPart.Add(ToAdd);
                                    }

                                    ++PrtCnt;
                                    if (ToAdd.group < 11)
                                    {
                                        SetDefaultAction(ToAdd.ba, ToAdd.group);
                                    }
                                }
                            }
                        }
                        ++ActionsCount;
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
                GUI.Label(new Rect(SelPartsLeft + 20, 180, 190, 40), Localizer.Format("#AGEXT_UI_select_part_hint"), UI.AGXLblStyleMiddleCenter);
            }

            if (defaultShowingNonNumeric)
            {
                if (GUI.Button(new Rect(SelPartsLeft + 245, 85, 120, 30), defaultGroupToShow.ToString(), UI.AGXBtnStyle)) //current action group button
                {
                    //TempShowGroupsWin = true;
                }
            }
            else
            {
                if (GUI.Button(new Rect(SelPartsLeft + 245, 85, 120, 30), AGXCurActGroup + ": " + AGXguiNames[AGXCurActGroup], UI.AGXBtnStyleMiddleLeft))
                {
                    TempShowGroupsWin = true;
                }

                if (IsGroupToggle[AGXCurActGroup])
                {
                    Color TxtClr = GUI.contentColor;
                    GUI.contentColor = Color.green;
                    if (GUI.Button(new Rect(SelPartsLeft + 230, 160, 90, 22), Localizer.Format("#AGEXT_UI_state_vis_yes"), UI.AGXBtnStyleEnabled))
                    {
                        IsGroupToggle[AGXCurActGroup] = false;
                    }
                    GUI.contentColor = TxtClr;
                }
                else
                {
                    if (GUI.Button(new Rect(SelPartsLeft + 230, 160, 90, 22), Localizer.Format("#AGEXT_UI_state_vis_no"), UI.AGXBtnStyle))
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
                    // CalculateActionsToShow();
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

            if (GUI.Button(new Rect(SelPartsLeft + 245, 244, 120, 20), CurrentKeySetName, UI.AGXBtnStyle))
            {
                ShowKeySetWin = true;
            }

            GUI.DragWindow();
        }

        public static void LoadDirectActionState(string DirectActions)
        {
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

        public void RefreshDefaultActionsList()
        {
            string errLine = "1";
            try
            {
                defaultActionsListAll.Clear();
                errLine = "2";
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    errLine = "3";
                    defaultActionsListAll.AddRange(p.Actions);
                    errLine = "4";
                    foreach (PartModule pm in p.Modules)
                    {
                        errLine = "5";
                        defaultActionsListAll.AddRange(pm.Actions);
                    }
                }
                Log.trace("RefDefActsComplete Okay"); //temporary
            }
            catch (Exception e)
            {
                Log.err("RefDefActsList Error, safe to ignore. {0}", errLine);
                Log.ex(this, e);
            }
        }

        public void RefreshDefaultActionsListType()
        {
            string errLine = "1";
            try
            {
                errLine = "2";
                defaultActionsListThisType.Clear();
                errLine = "3";
                foreach (BaseAction act in defaultActionsListAll)
                {
                    errLine = "4";
                    if ((act.actionGroup & defaultGroupToShow) == defaultGroupToShow)
                    {
                        errLine = "5";
                        defaultActionsListThisType.Add(act);
                    }
                }
            }
            catch (Exception e)
            {
                Log.err("RefDefActsListType Error {0}", errLine);
                Log.ex(this, e);
            }
        }

        public void GroupsWindow(int WindowID)
        {
            string ErrLine = "1";
            try
            {
                bool[] PageGrn = new bool[5];
                ErrLine = "3";
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
                ErrLine = "4";

                if (showCareerCustomAGs)
                {
                    // if (GUI.Button(new Rect(20, 3, 80, 20), "Other", AGXBtnStyle))
                    if (GUI.Button(new Rect(20, 3, 80, 20), Localizer.Format("#AGEXT_UI_type_other"), UI.AGXBtnStyle))
                    {
                        ErrLine = "4a";
                        defaultShowingNonNumeric = !defaultShowingNonNumeric;
                        ErrLine = "4b";
                        if (defaultShowingNonNumeric)
                        {
                            ErrLine = "4c";
                            RefreshDefaultActionsList();
                            ErrLine = "4k";
                            RefreshDefaultActionsListType();
                            ErrLine = "4l";
                        }
                        ErrLine = "4m";
                    }
                }

                ErrLine = "5";
                GUIStyle btnStyle = UI.AGXBtnStyle;
                if (GroupsPage == 1)            btnStyle = UI.AGXBtnStyleDisabled;
                else if (PageGrn[0])            btnStyle = UI.AGXBtnStyleEnabled;
                else                            btnStyle = UI.AGXBtnStyle;

                ErrLine = "6";
                if (GUI.Button(new Rect(120, 3, 25, 20), "1", btnStyle))
                {
                    GroupsPage = 1;
                }

                if (GroupsPage == 2)            btnStyle = UI.AGXBtnStyleDisabled;
                else if (PageGrn[1])            btnStyle = UI.AGXBtnStyleEnabled;
                else                            btnStyle = UI.AGXBtnStyle;

                if (GUI.Button(new Rect(145, 3, 25, 20), "2", btnStyle))
                {
                    GroupsPage = 2;
                }

                if (GroupsPage == 3)            btnStyle = UI.AGXBtnStyleDisabled;
                else if (PageGrn[2])            btnStyle = UI.AGXBtnStyleEnabled;
                else                            btnStyle = UI.AGXBtnStyle;

                ErrLine = "7";
                if (GUI.Button(new Rect(170, 3, 25, 20), "3", btnStyle))
                {
                    GroupsPage = 3;
                }

                if (GroupsPage == 4)            btnStyle = UI.AGXBtnStyleDisabled;
                else if (PageGrn[3])            btnStyle = UI.AGXBtnStyleEnabled;
                else                            btnStyle = UI.AGXBtnStyle;

                if (GUI.Button(new Rect(195, 3, 25, 20), "4", btnStyle))
                {
                    GroupsPage = 4;
                }

                if (GroupsPage == 5)            btnStyle = UI.AGXBtnStyleDisabled;
                else if (PageGrn[4])            btnStyle = UI.AGXBtnStyleEnabled;
                else                            btnStyle = UI.AGXBtnStyle;

                if (GUI.Button(new Rect(220, 3, 25, 20), "5", btnStyle))
                {
                    GroupsPage = 5;
                }

                ErrLine = "8";
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
                            foreach (Part p in EditorLogic.SortedShipList) //add all Parts with matching title to selected parts list, converting from string to Part
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
                        ++listCount; //moving to next button
                    }

                    GUI.EndScrollView();
                    ErrLine = "9";
                }
                else if (defaultShowingNonNumeric)
                {
                    ErrLine = "10";

                    if (defaultGroupToShow == KSPActionGroup.Abort) btnStyle = UI.AGXBtnStyleEnabled;
                    else                                            btnStyle = UI.AGXBtnStyle;

                    ErrLine = "11";
                    if (GUI.Button(new Rect(5, 25, 58, 20), Localizer.Format("#AGEXT_UI_type_abort"), btnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.Abort;
                        RefreshDefaultActionsListType();
                    }

                    if (defaultGroupToShow == KSPActionGroup.Brakes)    btnStyle = UI.AGXBtnStyleEnabled;
                    else                                                btnStyle = UI.AGXBtnStyle;

                    ErrLine = "12";
                    if (GUI.Button(new Rect(64, 25, 58, 20), Localizer.Format("#AGEXT_UI_type_brakes"), btnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.Brakes;
                        RefreshDefaultActionsListType();
                    }

                    if (defaultGroupToShow == KSPActionGroup.Gear)  btnStyle = UI.AGXBtnStyleEnabled;
                    else                                            btnStyle = UI.AGXBtnStyle;

                    if (GUI.Button(new Rect(122, 25, 59, 20), Localizer.Format("#AGEXT_UI_type_gear"), btnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.Gear;
                        RefreshDefaultActionsListType();
                    }

                    if (defaultGroupToShow == KSPActionGroup.Light) btnStyle = UI.AGXBtnStyleEnabled;
                    else                                            btnStyle = UI.AGXBtnStyle;

                    if (GUI.Button(new Rect(182, 25, 58, 20), Localizer.Format("#AGEXT_UI_type_lights"), btnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.Light;
                        RefreshDefaultActionsListType();
                    }

                    if (defaultGroupToShow == KSPActionGroup.RCS)   btnStyle = UI.AGXBtnStyleEnabled;
                    else                                            btnStyle = UI.AGXBtnStyle;

                    if (GUI.Button(new Rect(5, 45, 76, 20), Localizer.Format("#AGEXT_UI_type_rcs"), btnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.RCS;
                        RefreshDefaultActionsListType();
                    }

                    if (defaultGroupToShow == KSPActionGroup.SAS)   btnStyle = UI.AGXBtnStyleEnabled;
                    else                                            btnStyle = UI.AGXBtnStyle;

                    if (GUI.Button(new Rect(81, 45, 76, 20), Localizer.Format("#AGEXT_UI_type_sas"), btnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.SAS;
                        RefreshDefaultActionsListType();
                    }

                    if (defaultGroupToShow == KSPActionGroup.Stage) btnStyle = UI.AGXBtnStyleEnabled;
                    else                                            btnStyle = UI.AGXBtnStyle;

                    if (GUI.Button(new Rect(157, 45, 76, 20), Localizer.Format("#AGEXT_UI_type_stage"), btnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.Stage;
                        RefreshDefaultActionsListType();
                    }

                    ErrLine = "17";
                    //add vector2
                    groupWinScroll = GUI.BeginScrollView(new Rect(5, 70, 240, 455), groupWinScroll, new Rect(0, 0, 240, Mathf.Max(455, defaultActionsListThisType.Count * 20)));
                    int listCount2 = 1;
                    highlightPartThisFrameGroupWin = false;
                    ErrLine = "18";
                    while (listCount2 <= defaultActionsListThisType.Count)
                    {
                        if (Mouse.screenPos.y >= GroupsWin.y + 70 && Mouse.screenPos.y <= GroupsWin.y + 525 && new Rect(GroupsWin.x + 5, (GroupsWin.y + 70 + ((listCount2 - 1) * 20)) - groupWinScroll.y, 240, 20).Contains(Mouse.screenPos))
                        {
                            highlightPartThisFrameGroupWin = true;
                            //Log.dbg("part found to highlight acts " + highlightPartThisFrameActsWin + " " + ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.transform.position);
                            partToHighlight = defaultActionsListThisType.ElementAt(listCount2 - 1).listParent.part;
                        }

                        if (GUI.Button(new Rect(0, (listCount2 - 1) * 20, 240, 20), defaultActionsListThisType.ElementAt(listCount2 - 1).listParent.part.partInfo.title + " " + defaultActionsListThisType.ElementAt(listCount2 - 1).guiName, UI.AGXBtnStyle)) //button code
                        {
                            defaultActionsListThisType.ElementAt(listCount2 - 1).actionGroup = defaultActionsListThisType.ElementAt(listCount2 - 1).actionGroup & ~defaultGroupToShow;
                            RefreshDefaultActionsListType();
                        }
                        ++listCount2;
                    }
                    GUI.EndScrollView();
                    ErrLine = "19";
                }
                else
                {
                    ErrLine = "20";
                    ScrollGroups = GUI.BeginScrollView(new Rect(5, 25, 240, 500), ScrollGroups, new Rect(0, 0, 240, 500));

                    int ButtonID = 1 + (50 * (GroupsPage - 1));
                    int ButtonPos = ButtonPos = 1;
                    TextAnchor TxtAnch3 = GUI.skin.button.alignment;
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
                            btnStyle = StaticData.CurrentVesselActions.Any(pfd => pfd.group == ButtonID) ? UI.AGXBtnStyleMiddleLeftActive : UI.AGXBtnStyleMiddleLeft;
                            if (GUI.Button(new Rect(0, (ButtonPos - 1) * 20, 120, 20), ButtonID + ": " + AGXguiNames[ButtonID], btnStyle))
                            {
                                AGXCurActGroup = ButtonID;
                                TempShowGroupsWin = false;
                            }
                        }
                        ButtonPos = ButtonPos + 1;
                        ButtonID = ButtonID + 1;
                    }
                    ErrLine = "21";
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
                            btnStyle = StaticData.CurrentVesselActions.Any(pfd => pfd.group == ButtonID) ? UI.AGXBtnStyleMiddleLeftActive : UI.AGXBtnStyleMiddleLeft;
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
                ErrLine = "22";
            }
            catch (Exception e)
            {
                Log.err("GroupsWin Error {0}", ErrLine);
                Log.ex(this, e);
            }
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

        public static void LoadGroupNames(string LoadNames) //v2 done
        {
            string errLine = "1";
            try
            {
                for (int i = 1; i <= 250; i = i + 1)
                {
                    AGXguiNames[i] = "";
                }

                errLine = "13";
                //Log.dbg("AGX Load Name: " + EditorLogic.RootPart.partName+" "+ LoadNames);

                if (LoadNames.Length > 0)
                {
                    errLine = "14";
                    while (LoadNames[0] == '\u2023')
                    {
                        errLine = "15";
                        string groupName = "";
                        LoadNames = LoadNames.Substring(1);
                        int groupNum = Convert.ToInt32(LoadNames.Substring(0, 3));
                        LoadNames = LoadNames.Substring(3);

                        if (LoadNames.IndexOf('\u2023') == -1)
                        {
                            groupName = LoadNames;
                        }
                        else
                        {
                            groupName = LoadNames.Substring(0, LoadNames.IndexOf('\u2023'));
                            LoadNames = LoadNames.Substring(LoadNames.IndexOf('\u2023'));
                        }

                        AGXguiNames[groupNum] = groupName;
                    }
                }
            }
            catch (Exception e)
            {
                Log.err("LoadGroupNames Fail {0}", errLine);
                Log.ex(typeof(AGXEditor), e);
            }
        }

        static bool transitioning = false;
        public void Update()
        {
            string errLine = "1";
            Log.detail("update start {0}", Time.realtimeSinceStartup);
            try
            {
                Log.dbg("EditorLogic.fetch.editorScreen {0}", EditorLogic.fetch.editorScreen);

                switch (EditorLogic.fetch.editorScreen)
                {
                    case EditorScreen.Parts:
                        if (EditorPanels.Instance.panelManager == null || EditorPanels.Instance.partsEditor.State != "In")
                        {
                            Log.detail("BRING IN PARTS");

                            if (!transitioning)
                            {
                                EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.partsEditor);
                                transitioning = true;
                            }
                        }
                        else
                            transitioning = false;
                        break;

                    case EditorScreen.Crew:
                        if (EditorPanels.Instance.panelManager == null || EditorPanels.Instance.crew.State != "In")
                        {
                            Log.dbg("Detail IN CREW");

                            if (!transitioning)
                            {
                                EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.crew);
                                transitioning = true;
                            }
                        }
                        else
                            transitioning = false;
                        break;

                    case EditorScreen.Actions:
                        if (AGXShow)
                        {
                            if (EditorPanels.Instance.panelManager != null)
                            {
                                Log.detail("HIDE PANELS");
                                EditorPanels.Instance.panelManager.Dismiss(EditorPanels.Instance.actions);
                            }
                        }
                        else
                        {
                            if (EditorPanels.Instance.panelManager == null || EditorPanels.Instance.actions.State != "In")
                            {
                                Log.detail("BRING IN ACTIONS");
                                if (!transitioning)
                                {
                                    EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.actions);
                                    transitioning = true;
                                }
                            }
                            else
                                transitioning = false;
                        }

                        break;
                }

                EditorLogic ELCur; // = new EditorLogic();
                ELCur = EditorLogic.fetch;//get current editor logic instance

                errLine = "2";

                if (AGXDoLock && ELCur.editorScreen != EditorScreen.Actions)
                {
                    errLine = "3";
                    ELCur.Unlock("AGXLock");
                    AGXDoLock = false;
                }
                else if (AGXDoLock && !TrapMouse)
                {
                    errLine = "4";
                    ELCur.Unlock("AGXLock");
                    AGXDoLock = false;
                }
                else if (!AGXDoLock && TrapMouse && ELCur.editorScreen == EditorScreen.Actions)
                {
                    errLine = "5";
                    ELCur.Lock(false, false, false, "AGXLock");
                    AGXDoLock = true;
                }

                errLine = "6";

                if (ELCur.editorScreen == EditorScreen.Actions) //only show mod if on actions editor screen
                {
                    errLine = "7";
                    ShowSelectedWin = true;
                }
                else
                {
                    errLine = "8";
                    ShowSelectedWin = false;
                    errLine = "9";
                    AGEditorSelectedParts.Clear();//mod is hidden, clear list so parts don't get orphaned in it
                    errLine = "10";
                    PartActionsList.Clear();
                    errLine = "11";
                    ActionsListDirty = true;
                }
                errLine = "12";

                if (ShowSelectedWin)
                {
                   // Log.dbg("time start " + "|" + Time.realtimeSinceStartup);
                    errLine = "13";

                    if (EditorActionGroups.Instance.GetSelectedParts() != null) //on first run, list is null
                    {
                        errLine = "14";
                        if (ActionsListDirty)
                        {
                            errLine = "15";
                            UpdateActionsListCheck();
                        }

                        if (EditorActionGroups.Instance.GetSelectedParts().Count > 0) //are there parts selected?
                        {
                            errLine = "16";

                            if (PreviousSelectedPart != EditorActionGroups.Instance.GetSelectedParts().First()) //has selected part changed?
                            {
                                errLine = "17";
                                if (!AGEditorSelectedParts.Any(p => p.AGPart == EditorActionGroups.Instance.GetSelectedParts().First())) //make sure selected part is not already in AGEdSelParts
                                {
                                    errLine = "18";
                                    if (AGEditorSelectedParts.Count == 0) //no items in Selected Parts list, so just add selection
                                    {
                                        errLine = "19";
                                        AGEditorSelectedParts.AddRange(AGXAddSelectedPart(EditorActionGroups.Instance.GetSelectedParts().First(), SelPartsIncSym));
                                    }
                                    else if (AGEditorSelectedParts.First().AGPart.name == EditorActionGroups.Instance.GetSelectedParts().First().name) //selected part matches first part already in selected parts list, so just add selected part
                                    {
                                        errLine = "20";
                                        AGEditorSelectedParts.AddRange(AGXAddSelectedPart(EditorActionGroups.Instance.GetSelectedParts().First(), SelPartsIncSym));
                                    }
                                    else //part does not match first part in list, clear list before adding part
                                    {
                                        errLine = "21";
                                        AGEditorSelectedParts.Clear();
                                        AGEditorSelectedParts.AddRange(AGXAddSelectedPart(EditorActionGroups.Instance.GetSelectedParts().First(), SelPartsIncSym));
                                    }
                                }
                                errLine = "22";
                                PreviousSelectedPart = EditorActionGroups.Instance.GetSelectedParts().First(); //remember selected part so logic does not run unitl another part selected
                            }
                            errLine = "23";
                        }
                        errLine = "24";
                    }
                    errLine = "25";
                }
                errLine = "26";

                if (EditorLogic.fetch.editorScreen == EditorScreen.Actions && !AGXShow)
                {
                    errLine = "27";
                    MonitorDefaultActions();
                }
                errLine = "28";
                try
                {
                    if (AGXRoot != EditorLogic.RootPart)
                    {
                        //Log.dbg("AGX Root diff" + EditorLogic.RootPart.partInfo.name);
                        EditorLoadFromNode();
                        AGXRoot = EditorLogic.RootPart;
                        errLine = "29";
                    }
                }
                catch
                {
                    //silent fail, this gets hit if EditorLogic above is null, it's static so you can't null check it directly
                }
                Log.dbg("Editor Update end {0}", StaticData.CurrentVesselActions.Count());
                errLine = "30";
                Log.trace("update end {0}", Time.realtimeSinceStartup);
            }
            catch (Exception e)
            {
                Log.err("Editor Update FAIL {0}", errLine);
                Log.ex(this, e);
            }
        } //close Update()

        public void PrintSelectedPart()
        {
            if (DateTime.Now.Second.ToString().Substring(DateTime.Now.Second.ToString().Length - 1, 1) == "0")
            {
                Log.dbg("All true");
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    foreach (PartModule pm in p.Modules)
                    {
                        foreach (BaseAction ba in pm.Actions)
                        {
                            ba.active = true;
                        }
                    }
                }
            }
            if (DateTime.Now.Second.ToString().Substring(DateTime.Now.Second.ToString().Length - 1, 1) == "5")
            {
                Log.dbg("All false");
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    foreach (PartModule pm in p.Modules)
                    {
                        foreach (BaseAction ba in pm.Actions)
                        {
                            ba.active = false;
                        }
                    }
                }
            }
        }

        public void PrintPartActs()
        {
            try
            {
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    //List<BaseAction> partActs = new List<BaseAction>();
                    // partActs.AddRange(p.Actions);
                    foreach (PartModule pm in p.Modules)
                    {
                        Log.dbg(pm.moduleName);
                        //partActs.AddRange(pm.Actions);
                    }
                }
            }
            catch
            {
                Log.err("Print fail!");
            }
        }

        public void PrintPartPos()
        {
            try
            {
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    Log.detail("{0} {1} {2} {3}", p.partInfo.title, p.orgPos, EditorLogic.RootPart.transform.InverseTransformPoint(p.transform.position), p.orgRot);
                }
            }
            catch
            {
                Log.err("Print fail!");
            }
        }

        public void MonitorDefaultActions()
        {
            //Log.dbg("AGX Monitor default start " + StaticData.CurrentVesselActions.Count());
            KSPActionGroup KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom01;
            try //find which action group is selected in default ksp editor this pass
            {
                ////Log.dbg("Selected group " + KSPDefaultLastActionGroup);
                switch (EditorActionGroups.Instance.actionGroupList.SelectedIndex)
                {
                case 0:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Stage;
                break;
                case 1:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Gear;
                break;
                case 2:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Light;
                break;
                case 3:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.RCS;
                break;
                case 4:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.SAS;
                break;
                case 5:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Brakes;
                break;
                case 6:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Abort;
                break;
                case 7:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom01;
                break;
                case 8:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom02;
                break;
                case 9:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom03;
                break;
                case 10:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom04;
                break;
                case 11:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom05;
                break;
                case 12:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom06;
                break;
                case 13:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom07;
                break;
                case 14:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom08;
                break;
                case 15:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom09;
                break;
                case 16:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom10;
                break;
                default:
                KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom01;
                break;
                }
            }
            catch
            {
                Log.err("AGX Monitor default list fail");
            }

            string errLine = "1";
            try
            {
                errLine = "2";
                if (EditorActionGroups.Instance.GetSelectedParts() != null) //is a part selected?
                {
                    errLine = "3";
                    if (EditorActionGroups.Instance.GetSelectedParts().Count > 0) //list can exist with no parts in it if you selected then unselect one
                    {
                        errLine = "4";
                        if (SelectedWithSym.Count == 0 || SelectedWithSym.First() != EditorActionGroups.Instance.GetSelectedParts().First() || KSPDefaultActionGroupThisFrame != KSPDefaultLastActionGroup) //check if there is a previously selected part, if so check if its changed
                        {
                            errLine = "5";
                            //Log.dbg("2b");
                            //parts are different
                            SelectedWithSym.Clear(); //reset lastpart list
                            SelectedWithSym.AddRange(EditorActionGroups.Instance.GetSelectedParts());
                            SelectedWithSym.AddRange(EditorActionGroups.Instance.GetSelectedParts().First().symmetryCounterparts);
                            SelectedWithSymActions.Clear(); //reset actions list
                            //Log.dbg("2c");
                            errLine = "6";
                            foreach (Part prt in SelectedWithSym)
                            {
                                //  print("2d");
                                errLine = "7";
                                foreach (BaseAction bap in prt.Actions) //get part actions
                                {
                                    SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = bap, agrp = bap.actionGroup }); //add actiongroup separate otherwise it links and so have nothing to compare
                                }
                                errLine = "8";
                                foreach (PartModule pm in prt.Modules) //add actions from all partmodules
                                {
                                    errLine = "9";
                                    foreach (BaseAction bapm in pm.Actions)
                                    {
                                        errLine = "10";
                                        SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = bapm, agrp = bapm.actionGroup });
                                    }
                                }
                            }
                            errLine = "11";
                            int groupToAdd = 1;
                            switch (KSPDefaultActionGroupThisFrame)
                            {
                                case KSPActionGroup.Custom01:
                                    groupToAdd = 1;
                                    break;
                                case KSPActionGroup.Custom02:
                                    groupToAdd = 2;
                                    break;
                                case KSPActionGroup.Custom03:
                                    groupToAdd = 3;
                                    break;
                                case KSPActionGroup.Custom04:
                                    groupToAdd = 4;
                                    break;
                                case KSPActionGroup.Custom05:
                                    groupToAdd = 5;
                                    break;
                                case KSPActionGroup.Custom06:
                                    groupToAdd = 6;
                                    break;
                                case KSPActionGroup.Custom07:
                                    groupToAdd = 7;
                                    break;
                                case KSPActionGroup.Custom08:
                                    groupToAdd = 8;
                                    break;
                                case KSPActionGroup.Custom09:
                                    groupToAdd = 9;
                                    break;
                                case KSPActionGroup.Custom10:
                                    groupToAdd = 10;
                                    break;
                            }
                            errLine = "12";
                            foreach (AGXAction agact2 in StaticData.CurrentVesselActions.Where(ag => ag.group == groupToAdd))
                            {
                                SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = agact2.ba, agrp = agact2.ba.actionGroup });
                            }
                            errLine = "13";
                            KSPDefaultLastActionGroup = KSPDefaultActionGroupThisFrame;
                        }
                        else //selected part is the same a previously selected part
                        {
                            errLine = "14";
                            //Log.dbg("2f");
                            List<Part> PartsThisFrame = new List<Part>(); //get list of parts this update frame
                            PartsThisFrame.AddRange(EditorActionGroups.Instance.GetSelectedParts());
                            PartsThisFrame.AddRange(EditorActionGroups.Instance.GetSelectedParts().First().symmetryCounterparts);
                            // print("2g");
                            List<BaseAction> ThisFrameActions = new List<BaseAction>(); //get actions fresh again this update frame
                            foreach (Part prt in PartsThisFrame)
                            {
                                errLine = "15";
                                // print("2h"); 
                                foreach (BaseAction bap in prt.Actions)
                                {
                                    ThisFrameActions.Add(bap);
                                }
                                errLine = "16";
                                foreach (PartModule pm in prt.Modules)
                                {
                                    foreach (BaseAction bapm in pm.Actions)
                                    {
                                        ThisFrameActions.Add(bapm);
                                    }
                                }
                            }
                            errLine = "17";
                            int groupToAdd = 1;
                            switch (KSPDefaultActionGroupThisFrame)
                            {
                                case KSPActionGroup.Custom01:
                                    groupToAdd = 1;
                                    break;
                                case KSPActionGroup.Custom02:
                                    groupToAdd = 2;
                                    break;
                                case KSPActionGroup.Custom03:
                                    groupToAdd = 3;
                                    break;
                                case KSPActionGroup.Custom04:
                                    groupToAdd = 4;
                                    break;
                                case KSPActionGroup.Custom05:
                                    groupToAdd = 5;
                                    break;
                                case KSPActionGroup.Custom06:
                                    groupToAdd = 6;
                                    break;
                                case KSPActionGroup.Custom07:
                                    groupToAdd = 7;
                                    break;
                                case KSPActionGroup.Custom08:
                                    groupToAdd = 8;
                                    break;
                                case KSPActionGroup.Custom09:
                                    groupToAdd = 9;
                                    break;
                                case KSPActionGroup.Custom10:
                                    groupToAdd = 10;
                                    break;
                            }

                            foreach (AGXAction agact2 in StaticData.CurrentVesselActions.Where(ag => ag.group == groupToAdd))
                            {
                                ThisFrameActions.Add(agact2.ba);
                            }

                            errLine = "18";

                            //Log.dbg("2i");

                            foreach (BaseAction ba2 in ThisFrameActions) //check each action's actiongroup enum against last update frames actiongroup enum, this adds/removes a group to default KSP when added/removed in agx
                            {
                                //Log.dbg("2j");
                                errLine = "19";
                                AGXDefaultCheck ActionLastFrame = new AGXDefaultCheck();
                                //Log.dbg("2j1");
                                ActionLastFrame = SelectedWithSymActions.Find(a => a.ba == ba2);
                                // print("2j2");
                                errLine = "20";
                                if (ActionLastFrame.agrp != ba2.actionGroup) //actiongroup enum is different
                                {
                                    //  print("2j3");
                                    int NewGroup = 0; //which actiongroup changed?
                                    if (KSPActionGroup.Custom01 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 1;
                                    }
                                    else if (KSPActionGroup.Custom02 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 2;
                                    }
                                    else if (KSPActionGroup.Custom03 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 3;
                                    }
                                    else if (KSPActionGroup.Custom04 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 4;
                                    }
                                    else if (KSPActionGroup.Custom05 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 5;
                                    }
                                    else if (KSPActionGroup.Custom06 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 6;
                                    }
                                    else if (KSPActionGroup.Custom07 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 7;
                                    }
                                    else if (KSPActionGroup.Custom08 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 8;
                                    }
                                    else if (KSPActionGroup.Custom09 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 9;
                                    }
                                    else if (KSPActionGroup.Custom10 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 10;
                                    }

                                    // print("2k");
                                    errLine = "21";

                                    if (NewGroup != 0) //if one of the other actiongroups (gear, lights) has changed, ignore it. newgroup will be the actiongroup if I want to process it.
                                    {
                                        // print("Newgroup called on " + NewGroup);
                                        errLine = "22";
                                        if (Mouse.screenPos.x >= 130 && Mouse.screenPos.x <= 280)
                                        {
                                            //Log.dbg("remove actions");
                                            //AGXAction ToRemove = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = NewGroup, activated = false };
                                            StaticData.CurrentVesselActions.RemoveAll(ag3 => ag3.ba == ba2 && ag3.group == NewGroup);
                                        }
                                        else
                                        {
                                            errLine = "23";
                                            //Log.dbg("add actions");
                                            AGXAction ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = NewGroup, activated = false };
                                            List<AGXAction> Checking = new List<AGXAction>();
                                            Checking.AddRange(StaticData.CurrentVesselActions);
                                            Checking.RemoveAll(p => p.group != ToAdd.group);
                                            Checking.RemoveAll(p => p.prt != ToAdd.prt);
                                            Checking.RemoveAll(p => p.ba != ToAdd.ba);

                                            if (Checking.Count == 0)
                                            {
                                                StaticData.CurrentVesselActions.Add(ToAdd);
                                            }
                                        }

                                    }
                                    errLine = "24";
                                    ActionLastFrame.agrp = KSPActionGroup.None;
                                    ActionLastFrame.agrp = ActionLastFrame.agrp | ba2.actionGroup;
                                    //Log.dbg("2l");
                                }

                            }
                            SelectedWithSymActions.Clear(); //reset actions list as one of the enums changed.
                            //Log.dbg("2k");
                            errLine = "25";
                            foreach (Part prt in SelectedWithSym)
                            {
                                errLine = "26";
                                foreach (BaseAction bap in prt.Actions) //get part actions
                                {
                                    SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = bap, agrp = bap.actionGroup }); //add actiongroup separate otherwise it links and so have nothing to compare
                                }
                                errLine = "27";
                                foreach (PartModule pm in prt.Modules) //add actions from all partmodules
                                {
                                    foreach (BaseAction bapm in pm.Actions)
                                    {
                                        SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = bapm, agrp = bapm.actionGroup });
                                    }
                                }
                            }
                            errLine = "28";
                            foreach (AGXAction agact2 in StaticData.CurrentVesselActions.Where(ag => ag.group == groupToAdd))
                            {
                                SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = agact2.ba, agrp = agact2.ba.actionGroup });
                            }
                            errLine = "29";
                        }
                    }
                }
                //Log.dbg("AGX Monitor default end " + StaticData.CurrentVesselActions.Count());
            }
            catch (Exception e)
            {
                Log.err("Monitor Default Actions Error {0}", errLine);
                Log.ex(this, e);
            }
            //Log.dbg("2l " + Mouse.screenPos);
        }

        public static void EditorLoadDataFromPartModuleNewMethod() //new method loading from partModule
        {
            string errLine = "1";
            try
            {
                ModuleAGX loadingPM = new ModuleAGX();
                errLine = "2";
                try
                {
                    if (EditorLogic.RootPart.Modules.OfType<ModuleAGX>().First().hasData)
                    {
                        errLine = "3";
                        loadingPM = EditorLogic.RootPart.Modules.OfType<ModuleAGX>().First();
                    }
                    else
                    {
                        errLine = "4";
                        foreach (Part p in EditorLogic.SortedShipList)
                        {
                            errLine = "5";
                            if (p.Modules.OfType<ModuleAGX>().First().hasData)
                            {
                                errLine = "6";
                                loadingPM = p.Modules.OfType<ModuleAGX>().First();
                            }
                        }
                    }
                }
                catch
                {
                    //silent fail on no vessel loaded so RootPart nullrefs
                }
                errLine = "7";
                CurrentKeySet = loadingPM.currentKeyset;
                errLine = "8";
                LoadCurrentKeyBindings();
                errLine = "9";
                CurrentKeySetName = KeySetNames[CurrentKeySet - 1];

                errLine = "10";
                LoadGroupNames(loadingPM.groupNames);
                errLine = "11";
                LoadGroupVisibility(loadingPM.groupVisibility);
                errLine = "12";
                LoadGroupVisibilityNames(loadingPM.groupVisibilityNames);
                errLine = "13";
                LoadDirectActionState(loadingPM.DirectActionState);
                errLine = "14";
                try
                {
                    if (EditorLogic.fetch != null && EditorLogic.SortedShipList.Count > 0)
                    {
                        errLine = "14a";
                        foreach (Part p in EditorLogic.SortedShipList)
                        {
                            errLine = "15";
                            foreach (AGXAction agAct in p.Modules.OfType<ModuleAGX>().First().agxActionsThisPart)
                            {
                                errLine = "16";
                                if (agAct.ba != null && !StaticData.CurrentVesselActions.Contains(agAct))
                                {
                                    errLine = "17";
                                    StaticData.CurrentVesselActions.Add(agAct);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //silent fail, SortedShipList nullrefs with no parts loaded
                }

            }
            catch (Exception e)
            {
                //silently fail, will hit this if no parts placed
                Log.err("temporary error on new loading thing {0}", errLine);
                Log.ex(typeof(AGXEditor), e);
            }
        }

        public static void EditorLoadFromNode() //no longer loads from node as of agx 1.34, reusing the method for the basic load call now
        {
            //Log.dbg("LoadFromNode Called" + StaticData.CurrentVesselActions.Count());
            string errLine = "1";
            try
            {
                errLine = "10b";
                EditorLoadDataFromPartModuleNewMethod();
                errLine = "15b";
                List<KSPActionGroup> CustomActions = new List<KSPActionGroup>();
                CustomActions.Add(KSPActionGroup.Custom01); //how do you add a range from enum?
                CustomActions.Add(KSPActionGroup.Custom02);
                CustomActions.Add(KSPActionGroup.Custom03);
                CustomActions.Add(KSPActionGroup.Custom04);
                CustomActions.Add(KSPActionGroup.Custom05);
                CustomActions.Add(KSPActionGroup.Custom06);
                CustomActions.Add(KSPActionGroup.Custom07);
                CustomActions.Add(KSPActionGroup.Custom08);
                CustomActions.Add(KSPActionGroup.Custom09);
                CustomActions.Add(KSPActionGroup.Custom10);

                errLine = "16";
                // string AddGroup = "";
                List<BaseAction> partAllActions = new List<BaseAction>(); //is all vessel actions, copy pasting code
                try
                {
                    foreach (Part p in EditorLogic.SortedShipList)
                    {
                        partAllActions.AddRange(p.Actions);
                        foreach (PartModule pm in p.Modules)
                        {
                            partAllActions.AddRange(pm.Actions);
                        }
                    }

                    foreach (BaseAction baLoad in partAllActions)
                    {
                        foreach (KSPActionGroup agrp in CustomActions)
                        {
                            if ((baLoad.actionGroup & agrp) == agrp)
                            {
                                errLine = "17";
                                AGXAction ToAdd = new AGXAction() { prt = baLoad.listParent.part, ba = baLoad, group = CustomActions.IndexOf(agrp) + 1, activated = false };
                                List<AGXAction> Checking = new List<AGXAction>();
                                Checking.AddRange(StaticData.CurrentVesselActions);
                                Checking.RemoveAll(p => p.group != ToAdd.group);
                                Checking.RemoveAll(p => p.prt != ToAdd.prt);
                                Checking.RemoveAll(p => p.ba != ToAdd.ba);

                                if (Checking.Count == 0)
                                {
                                    StaticData.CurrentVesselActions.Add(ToAdd);
                                }
                                ModuleAGX toAddModule = ToAdd.ba.listParent.part.Modules.OfType<ModuleAGX>().First();
                                if (!toAddModule.agxActionsThisPart.Contains(ToAdd))
                                {
                                    toAddModule.agxActionsThisPart.Add(ToAdd);
                                }
                            }
                        }
                        errLine = "18";
                    }
                }
                catch
                {
                    //silently fail, if we hit this EditorLogic.sortedShipList is not valid
                }
                AGXRoot = EditorLogic.RootPart;
                //Log.dbg("LoadFromNode Called End" + StaticData.CurrentVesselActions.Count());
            }
            catch (Exception e)
            {
                print("AGX EditorLoadFromNode Fail " + errLine + " " + e);
            }
        }

        public static string SaveGroupNames(Part p, string str)
        {
            return str;
        }

        public static string SaveGroupNames(string str)
        {
            string errLine = "1";
            bool OkayToSave = true;
            try
            {
                errLine = "2";

                if (OkayToSave)
                {
                    errLine = "3";
                    string SaveStringNames = "";
                    errLine = "4";

                    errLine = "5";
                    int GroupCnt = 1;
                    errLine = "6";
                    while (GroupCnt <= 250)
                    {
                        errLine = "7";
                        if (AGXguiNames[GroupCnt].Length >= 1)
                        {
                            errLine = "8";
                            SaveStringNames = SaveStringNames + '\u2023' + GroupCnt.ToString("000") + AGXguiNames[GroupCnt];
                        }
                        errLine = "9";
                        GroupCnt = GroupCnt + 1;
                    }
                    errLine = "10";
                    return SaveStringNames;
                }
                else
                {
                    errLine = "11";
                    return str;
                }
            }
            catch (Exception e)
            {
                Log.err("Editor Fail (SaveGroupNames) {0}", errLine);
                Log.ex(typeof(AGXEditor), e);
                return str;
            }
        }

        public void UpdateActionsListCheck()
        {
            Log.trace("UpdateActions start {0}", StaticData.CurrentVesselActions.Count());
            List<AGXAction> KnownGood = new List<AGXAction>();

            foreach (AGXAction agxAct in StaticData.CurrentVesselActions) if (EditorLogic.SortedShipList.Contains(agxAct.prt))
                KnownGood.Add(agxAct);

            StaticData.CurrentVesselActions = KnownGood;
            RefreshDefaultActionsList();
            ActionsListDirty = false;
            Log.trace("UpdateActions end {0}", StaticData.CurrentVesselActions.Count());
        }

        public void AGXResetPartsList() //clear selected parts list and populate with newly selected part(s)
        {
            AGEEditorSelectedPartsSame = true;
            AGEditorSelectedParts.Clear();
            foreach (Part p in EditorLogic.SortedShipList) //add all parts to list
            {
                AGEditorSelectedParts.Add(new AGXPart(p));
            }

            AGXPart AGPcompare = new AGXPart();
            AGPcompare = AGEditorSelectedParts.First(); //set first part in selected parts list to compare

            foreach (AGXPart p in AGEditorSelectedParts)
            {
                if (p.AGPart.name != AGPcompare.AGPart.name) //remove all parts that are of a different type
                {
                    AGEEditorSelectedPartsSame = false;
                    break;  // No need to keep searching.
                }
            }
        }

        public static void LoadGroupVisibilityNames(string LoadString) //ver2 only
        {
            for (int i = 1; i <= 4; i++)
            {
                int KeyLength = LoadString.IndexOf('\u2023');
                ShowGroupInFlightNames[i] = LoadString.Substring(0, KeyLength);
                LoadString = LoadString.Substring(KeyLength + 1);
            }
            ShowGroupInFlightNames[5] = LoadString;
        }

        public static string SaveGroupVisibilityNames(Part p, string str)
        {
            return str;
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
                return StringToSave;
            }
            catch
            {
                return str;
            }
        }

        public static string SaveGroupVisibility(Part p, string str)
        {
            return str;
        }

        public static string SaveGroupVisibility(string str)
        {
            try
            {
                string ReturnStr = ShowGroupInFlightCurrent.ToString(); //add currently show flightgroup

                for (int i = 1; i <= 250; i++)
                {
                    ReturnStr = ReturnStr + Convert.ToInt16(IsGroupToggle[i]).ToString(); //add toggle state for group
                    for (int i2 = 1; i2 <= 5; i2++)
                    {
                        ReturnStr = ReturnStr + Convert.ToInt16(ShowGroupInFlight[i2, i]).ToString(); //add flight state visibility for each group
                    }
                }
                return ReturnStr;
            }
            catch
            {
                return str;
            }
        }

        public static void EditorSaveToFile()
        {
            EditorSaveToNode();
            EditorSaveGlobalInfo();
        }

        public static void EditorSaveGlobalInfo()
        {
            SaveCurrentKeyBindings();
            EditorSaveKeysetStuff();
        }

        public static void EditorSaveKeysetStuff()
        {
            AGExtNode.SetValue("KeySetName1", KeySetNames[0]);
            AGExtNode.SetValue("KeySetName2", KeySetNames[1]);
            AGExtNode.SetValue("KeySetName3", KeySetNames[2]);
            AGExtNode.SetValue("KeySetName4", KeySetNames[3]);
            AGExtNode.SetValue("KeySetName5", KeySetNames[4]);
            CurrentKeySetName = KeySetNames[CurrentKeySet - 1];
            AGXStaticData.SaveBaseConfigNode(AGExtNode);
        }

        public static void EditorSaveToNode() //no longer saves to node, now saves to partmodule, leaving name intact as i'm reusing the same method calls
        {
            //Log.dbg("AGX EditorSaveToFile called"); 
            string errLine = "1";
            bool okayToProceed = false;
            try
            {
                errLine = "2";
                if (EditorLogic.SortedShipList.Count > 0)
                {
                    okayToProceed = true;
                }
                else
                {
                    okayToProceed = false;
                }
                errLine = "3";
            }
            catch
            {
                errLine = "4";
                okayToProceed = false;
            }
            errLine = "5";
            try
            {
                if (okayToProceed)
                {
                    Log.dbg("let's save");
                    errLine = "14";
                    string groupNames = SaveGroupNames("");
                    errLine = "15";
                    string groupVisibility = SaveGroupVisibility("");
                    errLine = "16";
                    string groupVisibilityNames = SaveGroupVisibilityNames("");
                    errLine = "17";
                    string DirectActionState = SaveDirectActionState("");
                    errLine = "17a";
                    UpdateAGXActionGroupNames();
                    try
                    {
                        errLine = "17c";
                        foreach (Part p in EditorLogic.SortedShipList)
                        {
                            errLine = "17c1";
                            ModuleAGX thisPM = p.Modules.OfType<ModuleAGX>().First();
                            errLine = "17d";
                            thisPM.agxActionsThisPart.Clear();
                            thisPM.agxActionsThisPart.AddRange(StaticData.CurrentVesselActions.FindAll(p2 => p2.prt == p));
                            thisPM.currentKeyset = CurrentKeySet;
                            errLine = "18";
                        }
                    }
                    catch (Exception e)
                    {
                        Log.err("No parts to save {0}", errLine);
                        Log.ex(typeof(AGXEditor), e);
                    }
                }
            }
            catch (Exception e)
            {
                Log.err("AGX EditorSaveToFile FAIL {0} {1}", errLine, e);
                Log.ex(errLine, e);
            }
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;

//using UnityEngine;


//namespace ActionGroupsExtended //add scenario module for data storage
//{
//    //[KSPAddon(KSPAddon.Startup.SpaceCentre, false)] //install our scenario module. is a partmodule so all calculations can be done in it, no need for a seperate plugin class in the scene //removed in AGX1.34
//    //class AGExtMainMenu : MonoBehaviour
//    //{
//        //public void Start()
//        //{
//        //    var game = HighLogic.CurrentGame;
//        //    ProtoScenarioModule psm = game.scenarios.Find(s => s.moduleName == typeof(AGextScenario).Name);
//        //    if (psm == null)
//        //    {
//        //        psm = game.AddProtoScenarioModule(typeof(AGextScenario), GameScenes.FLIGHT);
//        //    }
//        //    ProtoScenarioModule psm2 = game.scenarios.Find(s2 => s2.moduleName == typeof(AGextScenarioEditor).Name);
//        //    if (psm2 == null)
//        //    {
//        //        psm2 = game.AddProtoScenarioModule(typeof(AGextScenarioEditor), GameScenes.EDITOR);
//        //    }
//        //    bool ClearOldSaves = true;
//        //    try
//        //    {
//        //        //ConfigNode AGXSettings = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
//        //        ConfigNode AGXSettings = AGXStaticData.LoadBaseConfigNode();
//        //        if (AGXSettings.GetValue("DeleteOldSaves") == "0")
//        //        {
//        //            ClearOldSaves = false;
//        //        }
//        //        else
//        //        {
//        //            ClearOldSaves = true;
//        //        }
//        //    }
//        //    catch
//        //    {
//        //        ClearOldSaves = true;
//        //    }
//        //    if (ClearOldSaves && !AGXStaticData.cleanupAlreadyRun)
//        //    {
//        //        DeleteOldSaveGames(); //delete old AGext000000.cfg files
//        //        AGXStaticData.cleanupAlreadyRun = true;
//        //    }
//        //}

//    //    public void DeleteOldSaveGames() //removed functionality in AGX 1.34
//    //    {
            
//    //        Log.Info("AGX Deleteing old save games start: " + DateTime.Now);

//    //        string[] existingGames = Directory.GetFiles(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder); //full path of all files in save dir
//    //        List<int> existingGamesNum = new List<int>(); //existing AGExt00000.cfg files, as number
//    //        List<int> persistentGamesNum = new List<int>(); //number in the .sfs save files
//    //        int dirLength = (new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder).Length; //character length of file path
//    //        foreach (string fileName in existingGames) //cycle through found files
//    //        {
//    //            //Log.Info("gamename " + fileName.Substring(dirLength + 1));
//    //            if (fileName.Substring(dirLength + 1, 5) == "AGExt" && fileName.Trim().EndsWith(".cfg")) //is file an AGX file?
//    //            {
//    //                //Log.Info("gamenameb " + fileName.Substring(dirLength + 6,5));
//    //                try //this will work if file fould is an AGX flight file
//    //                {
//    //                    int gameNum = Convert.ToInt32(fileName.Substring(dirLength + 6, 5));
//    //                    existingGamesNum.Add(gameNum);
//    //                    //Log.Info("gameNumb " + gameNum);
//    //                }
//    //                catch //did not work, was not an AGX flight file, but not actually an error so silently fail
//    //                {
//    //                }
//    //            }
//    //            else if (fileName.Trim().EndsWith(".sfs")) //is file an .sfs file?
//    //            {
//    //                try //this will work on KSP save files
//    //                {
//    //                    // Log.Info("sfsa");
//    //                    ConfigNode saveNode = ConfigNode.Load(fileName); //load the .sfs file
//    //                    //Log.Info("sfsb");
//    //                    if (saveNode.HasNode("GAME")) //is a KSP save file?//move from the 'root' to "GAME" node
//    //                    {
//    //                        ConfigNode saveNode2 = saveNode.GetNode("GAME");//move from the 'root' to "GAME" node
//    //                        //Log.Info("sfsc");
//    //                        foreach (ConfigNode scenNode in saveNode2.GetNodes("SCENARIO")) //cycle through all SCENARIO nodes
//    //                        {
//    //                            //Log.Info("sfsd");
//    //                            if (scenNode.HasValue("name") && scenNode.GetValue("name") == "AGextScenario") //stop at AGExtScenario node
//    //                            {
//    //                                //Log.Info("sfse");
//    //                                persistentGamesNum.Add(Convert.ToInt32(scenNode.GetValue("LastSave"))); //add lastsave num to list
//    //                                //Log.Info("sfsf");
//    //                            }
//    //                        }
//    //                    }
//    //                }
//    //                catch //did not work, not an KSP save file, silently fail
//    //                {
//    //                }
//    //            }
//    //        }
//    //        foreach (int iGame in existingGamesNum) //check each existing game
//    //        {
//    //            bool keep = false; // CCraigen - file should be kept
//    //            //Log.Info("Games " + iGame);
//    //            //if (!persistentGamesNum.Contains(i)) //is the AGX flight file found in a persistent file? if not, delete it. not sure what quicksave is doing, leave a one back file just in case
//    //            foreach (int iPersist in persistentGamesNum)
//    //            {
//    //                /*if (iGame != iPersist && iGame != iPersist - 1)
//    //                {
//    //                    //Log.Info("Deleted " + new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + iGame.ToString("00000") + ".cfg");
//    //                    File.Delete(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + iGame.ToString("00000") + ".cfg");
//    //                }*/


//    //                // CCraigen - slight change to logic to prevent a mass clobber of all files if there's significantly differing .sfs files present
//    //                if (iGame == iPersist || iGame == iPersist - 1)
//    //                {
//    //                    keep = true;
//    //                }
//    //            }

//    //            if (!keep)
//    //            {
//    //                File.Delete(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + iGame.ToString("00000") + ".cfg");
//    //            }

//    //            // CCraigen - new logic ends

//    //        }

//    //        Log.Info("AGX Deleteing old save games end: " + DateTime.Now);
//    //    }
//    //}

//    //public class AGextScenarioEditor : ScenarioModule //removed functionality in AGX 1.34
//    //{
//    //    public override void OnSave(ConfigNode node)
//    //    {
//    //        if (HighLogic.LoadedSceneIsEditor)
//    //        {
//    //            //Log.Info("Edit scen save called");
//    //            AGXEditor.EditorSaveToFile();
//    //        }
//    //    }
//    //}

//    //public class AGextScenario : ScenarioModule //this runs on flight scene start  //removed functionality in AGX 1.34
//    //{
//        ////public ConfigNode AGXBaseNode = new ConfigNode();
//        ////public ConfigNode AGXFlightNode = new ConfigNode();
//        ////public ConfigNode AGXEditorNode = new ConfigNode();

//        //int lastAGXSave = 1;
//        ////bool loadFin = false;
//        //ConfigNode currentFlightNode = new ConfigNode();

//        ////public void Start()
//        ////{
//        ////    GameEvents.onGameStateSaved.Add(GameEventSave);
//        ////}

//        ////public void GameEventSave(Game gm)
//        ////{
//        ////    Log.Info("game " + gm.linkCaption + " " + gm.linkURL);
//        ////}
//        //public override void OnLoad(ConfigNode node)
//        //{
            
           
//        //        //Log.Info("AGXFlight load");
//        //        if (node.HasValue("LastSave"))
//        //        {
//        //            lastAGXSave = Convert.ToInt32(node.GetValue("LastSave"));
//        //        }
//        //        else
//        //        {
//        //            lastAGXSave = 0;
//        //        }
//        //        if (File.Exists(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + lastAGXSave.ToString("00000") + ".cfg"))
//        //        {
//        //            currentFlightNode = ConfigNode.Load(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + lastAGXSave.ToString("00000") + ".cfg");
//        //        }
//        //        else
//        //        {
//        //            currentFlightNode = new ConfigNode("FLIGHT");
//        //            currentFlightNode.AddValue("name", "flight");
//        //        }
//        //        //Log.Info("AGXScenLoad " + lastAGXSave);
//        //        AGXFlight.AGXFlightNode = currentFlightNode;
//        //        //if (File.Exists(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtRootParts.cfg"))
//        //        //{
//        //        //    AGXFlight.RootParts = ConfigNode.Load(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtRootParts.cfg");
//        //        //}
//        //        //else
//        //        //{
//        //        //    AGXFlight.RootParts = new ConfigNode("ROOTPARTS");
//        //        //    AGXFlight.RootParts.AddValue("name", "rootParts");
//        //        //}

//        //        AGXFlight.flightNodeIsLoaded = true;
//        //        //Log.Info("Node laeded! "+ currentFlightNode);
            
//        //}
 

//        //public override void OnSave(ConfigNode node)
//        //{

//        //    //Log.Info("a");
            
//        //        //Log.Info("AGXFlightSave " + currentFlightNode);
//        //        if(node.HasValue("LastSave"))
//        //        {
//        //           // Log.Info("c");
//        //            lastAGXSave = Convert.ToInt32(node.GetValue("LastSave"));
//        //           // Log.Info("d");
//        //            node.RemoveValue("LastSave");
//        //        }
//        //        //Log.Info("e");
//        //        //Log.Info("scensave1 " + currentFlightNode);
//        //        ConfigNode flightToSave = AGXFlight.FlightSaveToFile(currentFlightNode);
//        //        //Log.Info("f");
//        //        lastAGXSave = lastAGXSave + 1;
//        //        while (File.Exists(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + lastAGXSave.ToString("00000") + ".cfg"))
//        //        {
//        //            lastAGXSave = lastAGXSave + 1;
//        //        }
//        //       // Log.Info("g " + flightToSave);
//        //        flightToSave.Save(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + lastAGXSave.ToString("00000") + ".cfg");
//        //       //Log.Info("scensave2 " + flightToSave);
//        //        node.AddValue("LastSave", lastAGXSave.ToString());
//        //        //Log.Info("i");
            
//        //   // Log.Info("j");
//        //}

//        ////public static ConfigNode LoadBaseNode()
//        ////{
//        ////    string errLine = "1";
//        ////    ConfigNode AGXBaseNode = new ConfigNode();
//        ////    try
//        ////    {
//        ////        if (File.Exists(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt.cfg"))
//        ////        {
//        ////            errLine = "3";
//        ////            AGXBaseNode = ConfigNode.Load(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt.cfg");
//        ////            //Log.Info("AGX ConfigNode Load Okay!");
//        ////        }
//        ////        else
//        ////        {
//        ////            errLine = "4";
//        ////            //Log.Info("AGX ConfigNode not found, creating.....");
//        ////            errLine = "5";
//        ////            AGXBaseNode.AddValue("name", "Action Groups Extended save file");
//        ////            AGXBaseNode.AddNode("FLIGHT");
//        ////            errLine = "6";
//        ////            AGXBaseNode.AddNode("EDITOR");
//        ////            errLine = "7";
//        ////            AGXBaseNode.Save(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt.cfg");
//        ////            errLine = "8";
//        ////        }
//        ////        return AGXBaseNode;
//        ////    }
//        ////    catch (Exception e)
//        ////    {
//        ////        Log.Info("AGXScen LoadBaseNode FAIL " + errLine + " " + e);
//        ////            return new ConfigNode();
//        ////    }
//        ////}

        

//        //public static string EditorHashShipName(string name, bool isVAB) //moved to StaticData in AGX1.34
//        //{
//        //    string hashedName = "";
//        //    if (isVAB)
//        //    {
//        //        hashedName = "VAB";
//        //    }
//        //    else
//        //    {
//        //        hashedName = "SPH";
//        //    }
//        //    foreach (Char ch in name)
//        //    {
//        //        hashedName = hashedName + (int)ch;
//        //    }
//        //    //Log.Info("hashName " + hashedName);

//        //    return hashedName;
//        //}

        

        
//    //}
//}

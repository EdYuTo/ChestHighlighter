using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using RoR2.UI;

namespace ChestHighlighter
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.edyuto.ChestHighlighter", "ChestHighlighter", "0.1.0")]
    public class ChestHighlighter : BaseUnityPlugin
    {
        private List<PingIndicator> pingIndicators;
        private bool isActive;

        public void Awake()
        {
            pingIndicators = new List<PingIndicator>();
            SceneManager.sceneUnloaded += this.OnSceneUnloaded;
            isActive = false;
            On.RoR2.Util.GetBestMasterName += (_, characterMaster) => {
                if (characterMaster)
                {
                    PlayerCharacterMasterController playerCharacterMasterController = characterMaster.playerCharacterMasterController;
                    string text;
                    if (playerCharacterMasterController == null)
                    {
                        text = null;
                    } else
                    {
                        NetworkUser networkUser = playerCharacterMasterController.networkUser;
                        text = ((networkUser != null) ? networkUser.userName : null);
                    }
                    string text2 = text;
                    if (text2 == null)
                    {
                        GameObject bodyPrefab = characterMaster.bodyPrefab;
                        text2 = Language.GetString((bodyPrefab != null) ? bodyPrefab.GetComponent<CharacterBody>().baseNameToken : null);
                    }
                    return text2;
                }
                return "";
            };
        }

        private void DisposeIndicators()
        {
            foreach (PingIndicator pingIndicator in pingIndicators)
            {
                if (pingIndicator != null && pingIndicator.gameObject != null)
                {
                    GameObject.Destroy(pingIndicator.gameObject);
                }
            }
            pingIndicators.Clear();
        }

        public void OnSceneUnloaded(Scene scene)
        {
            this.DisposeIndicators();
        }

        public void Update()
        {
            if (Input.GetKeyDown("b"))
            {
                this.DisposeIndicators();
                if (!isActive)
                {
                    foreach (ChestBehavior test in GameObject.FindObjectsOfType(typeof(ChestBehavior)))
                    {
                        GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/PingIndicator"));
                        PingIndicator pingIndicator = go.GetComponent<PingIndicator>();

                        pingIndicator.pingTarget = test.gameObject;
                        pingIndicator.pingOwner = go;
                        pingIndicator.interactablePingDuration = float.PositiveInfinity;

                        pingIndicator.RebuildPing();
                        pingIndicators.Add(pingIndicator);
                    }
                }
                isActive = !isActive;
            }
        }

    }
}

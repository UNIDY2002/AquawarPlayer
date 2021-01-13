﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GameHelper;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace GameImpl
{
    public static class GameAwakeImpl
    {
        public static void AwakeImpl(this GameUI gameUI)
        {
            for (var i = 0; i < 4; i++)
            {
                gameUI.Gom.MyFogs.Add(
                    Object.Instantiate(
                        gameUI.fogPrefab,
                        GameObjectManager.FishRelativePosition(false, i),
                        Quaternion.identity,
                        gameUI.allFishRoot
                    )
                );
                gameUI.Gom.EnemyFogs.Add(
                    Object.Instantiate(
                        gameUI.fogPrefab,
                        GameObjectManager.FishRelativePosition(true, i),
                        Quaternion.identity,
                        gameUI.allFishRoot
                    )
                );
            }

            gameUI.DissolveShaderProperty = Shader.PropertyToID("_cutoff");

            if (SharedRefs.Mode == Constants.GameMode.Offline)
            {
                var players = SharedRefs.ReplayJson[SharedRefs.ReplayCursor]["players"];
                var pickFish = SharedRefs.ReplayJson[SharedRefs.ReplayCursor++]["operation"][0]["Fish"];
                var myFishPicked = new List<int>();
                var enemyFishPicked = new List<int>();
                var myFishAvailable = new List<int>();
                var enemyFishAvailable = new List<int>();
                for (var i = 0; i < 4; i++)
                {
                    var myFishId = (int) pickFish[0][i]["id"] - 1;
                    var enemyFishId = (int) pickFish[1][i]["id"] - 1;
                    gameUI.GameState.MyFishId[i] = myFishId;
                    gameUI.GameState.EnemyFishId[i] = enemyFishId;
                    gameUI.myStatus[i].Full = (int) pickFish[0][i]["hp"];
                    gameUI.enemyStatus[i].Full = (int) pickFish[1][i]["hp"];
                    gameUI.myProfiles[i].SetupFish(myFishId, gameUI.myExtensions[i]);
                    gameUI.enemyProfiles[i].SetupFish(enemyFishId, gameUI.enemyExtensions[i]);
                    gameUI.myExtensions[i]
                        .UpdateText($"{Constants.FishName[myFishId]}\n主动：{Constants.SkillTable[myFishId]}");
                    gameUI.enemyExtensions[i]
                        .UpdateText($"{Constants.FishName[enemyFishId]}\n主动：{Constants.SkillTable[enemyFishId]}");
                    gameUI.myExtensions[i].gameObject.SetActive(false);
                    gameUI.enemyExtensions[i].gameObject.SetActive(false);
                    gameUI.myProfiles[i].SetHp(gameUI.myStatus[i].Full);
                    gameUI.enemyProfiles[i].SetHp(gameUI.enemyStatus[i].Full);
                    gameUI.myProfiles[i].SetAtk((int) pickFish[0][i]["atk"]);
                    gameUI.enemyProfiles[i].SetAtk((int) pickFish[1][i]["atk"]);
                    myFishPicked.Add(myFishId);
                    enemyFishPicked.Add(enemyFishId);
                }
                for (var i = 0; i < players[0]["my_fish"].Count; i++)
                    myFishAvailable.Add((int) players[0]["my_fish"][i]["id"] - 1);
                for (var i = 0; i < players[1]["my_fish"].Count; i++)
                    enemyFishAvailable.Add((int) players[1]["my_fish"][i]["id"] - 1);
                for (var i = 0; i < Constants.FishNum; i++)
                {
                    gameUI.myGlance.SetupFish(i,
                        myFishPicked.Contains(i)
                            ? Constants.FishState.Using
                            : myFishAvailable.Contains(i)
                                ? Constants.FishState.Free
                                : Constants.FishState.Used,
                        gameUI.myGlanceExt
                    );
                    gameUI.enemyGlance.SetupFish(i,
                        enemyFishPicked.Contains(i)
                            ? Constants.FishState.Using
                            : enemyFishAvailable.Contains(i)
                                ? Constants.FishState.Free
                                : Constants.FishState.Used,
                        gameUI.enemyGlanceExt
                    );
                }
                gameUI.myGlanceExt.gameObject.SetActive(false);
                gameUI.enemyGlanceExt.gameObject.SetActive(false);
                var rounds = (int) SharedRefs.ReplayJson[SharedRefs.ReplayCursor]["rounds"] + 1;
                gameUI.roundText.text = $"回合数：{rounds}/3";
                gameUI.scoreText.text = $"我方得分：{(int) SharedRefs.ReplayJson[SharedRefs.ReplayCursor]["score"]}";
                gameUI.resultText.gameObject.SetActive(false);
                gameUI.doneNextRoundButton.gameObject.SetActive(false);
                gameUI.logObject.SetActive(false);
                gameUI.Gom.Init(gameUI.unkFishPrefab, gameUI.allFishRoot);
                if (SharedRefs.AutoPlay) gameUI.MoveCursor();
                else
                {
                    gameUI.prevRoundButton.interactable = true;
                    gameUI.nextRoundButton.interactable = rounds < 3;
                    gameUI.nextStepButton.interactable = true;
                }
            }
            else
            {
                gameUI.GameState.GameStatus = Constants.GameStatus.WaitingAnimation;
                Task.Run(gameUI.NewRound);
            }
        }
    }
}
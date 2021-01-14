﻿using Utils;

namespace GameImpl
{
    public static class GameNewRoundImpl
    {
        /// <summary>
        ///   <para>First, resets the game states.<br/>
        /// After that, for online mode, listens to message from remote
        /// and determine what to show next; for offline mode, simply resets
        /// <code>_gameStatus</code> to <code>DoAssertion</code>.</para>
        /// </summary>
        public static void NewRound(this GameUI gameUI)
        {
            gameUI.GameState.MyFishSelected = -1;
            gameUI.GameState.EnemyFishSelected = -1;
            gameUI.GameState.NormalAttack = true;
            for (var i = 0; i < 4; i++)
                gameUI.GameState.MyFishSelectedAsTarget[i] = gameUI.GameState.EnemyFishSelectedAsTarget[i] = false;

            if (SharedRefs.Mode == Constants.GameMode.Offline)
            {
                gameUI.GameState.GameStatus = Constants.GameStatus.DoAssertion;
            }
            else
            {
                var result = SharedRefs.ActionInfo;
                var gameInfo = result["GameInfo"];
                gameUI.GameState.GameStatus = Constants.GameStatus.DoAssertion;
                for (var i = 0; i < 4; i++)
                {
                    gameUI.GameState.MyFishId[i] = SharedRefs.FishChosen[i];
                    if ((int) gameInfo["EnemyFish"][i] > 0)
                        gameUI.GameState.EnemyFishId[i] = (int) gameInfo["EnemyFish"][i] - 1;
                    gameUI.GameState.MyFishOnlineHp[i] = Constants.DefaultHp; // TODO: REALLY?
                    gameUI.GameState.EnemyFishOnlineHp[i] = (int) gameInfo["EnemyHP"][i];
                    if (!gameUI.Gom.Initialized)
                    {
                        gameUI.myStatus[i].Full = Constants.DefaultHp;
                        gameUI.enemyStatus[i].Full = Constants.DefaultHp;
                    }
                    gameUI.DisplayHpOnline();
                }

                if (!gameUI.Gom.Initialized) gameUI.Gom.Init(gameUI);

                gameUI.GameState.MyTurn = (string) result["Action"] == "Assert";
                if (gameUI.GameState.MyTurn)
                {
                    gameUI.assertionButtons.SetActive(true);
                    return;
                }

                gameUI.GameState.AssertionPlayer = 1;
                if (result["AssertPos"] == null)
                {
                    gameUI.GameState.Assertion = -1;
                    gameUI.GameState.OnlineAssertionHit = false;
                    gameUI.GameState.AssertionTarget = 0;
                    gameUI.ChangeStatus(); // Skips the next two stages
                    gameUI.ChangeStatus();
                    return;
                }

                gameUI.GameState.Assertion = (int) result["AssertPos"];
                gameUI.GameState.OnlineAssertionHit = (bool) result["AssertResult"];
                // gameUI._gameStates.AssertionTarget = (int) result["AssertContent"];
                gameUI.MakeAGuess(false, 1200);
                gameUI.SetTimeout(gameUI.ChangeStatus, 3000); // Just waits for the assertion animation to finish
            }
        }
    }
}
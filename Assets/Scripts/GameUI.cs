﻿using GameHelper;
using GameImpl;
using LitJson;
using UnityEngine.SceneManagement;
using Utils;

public class GameUI : GameBridge
{
    public readonly GameStates GameState;

    public readonly GameObjectManager Gom;

    // Dissolve effect

    internal int DissolveShaderProperty;

    private void UpdateFishStatus(JsonData players)
    {
        for (var i = 0; i < 4; i++)
        {
            myStatus[i].Current = (int) players[0]["fight_fish"][i]["state"] == 2
                ? 0
                : (int) players[0]["fight_fish"][i]["hp"];
            enemyStatus[i].Current = (int) players[1]["fight_fish"][i]["state"] == 2
                ? 0
                : (int) players[1]["fight_fish"][i]["hp"];
            myProfiles[i].SetAtk((int) players[0]["fight_fish"][i]["atk"]);
            enemyProfiles[i].SetAtk((int) players[1]["fight_fish"][i]["atk"]);
        }
    }

    public void AddLog(string s = "")
    {
        Instantiate(logItem, logContent).SetText(s);
    }

    public void DoneAndGoBackToPreparation()
    {
        if (SharedRefs.ErrorFlag)
        {
            SharedRefs.ErrorFlag = false;
            SceneManager.LoadScene("Scenes/Welcome");
            return;
        }
        SceneManager.LoadScene((int) SharedRefs.ReplayJson[SharedRefs.ReplayCursor]["rounds"] == 3
            ? "Scenes/Welcome"
            : "Scenes/Game");
    }

    public void BackHomeWrapper()
    {
        BackHome();
    }

    public void OnlineCancelBackHome()
    {
        exitConfirmMask.SetActive(false);
    }

    public void ToggleLog()
    {
        logActive = !logActive;
    }

    public void PrevStep()
    {
        Gom.CheckReviveOnBackwards(this);
        SharedRefs.ReplayCursor -= 2;
        if ((int) SharedRefs.ReplayJson[SharedRefs.ReplayCursor - 1]["gamestate"] == 2)
            prevStepButton.interactable = false;
        UpdateFishStatus(SharedRefs.ReplayJson[SharedRefs.ReplayCursor]["players"]);
    }

    public void NextStep()
    {
        prevStepButton.interactable = false;
        nextStepButton.interactable = false;
        prevRoundButton.interactable = false;
        nextRoundButton.interactable = false;
        if (GameState.GameStatus == Constants.GameStatus.PeekAssertion)
            this.ChangeStatus();
        else
            this.MoveCursor();
    }

    public void PrevRound()
    {
        if (SharedRefs.ReplayCursor > 2 &&
            (int) SharedRefs.ReplayJson[SharedRefs.ReplayCursor]["gamestate"] != 2 &&
            (int) SharedRefs.ReplayJson[SharedRefs.ReplayCursor - 1]["gamestate"] == 2)
            SharedRefs.ReplayCursor -= 3;
        while ((int) SharedRefs.ReplayJson[SharedRefs.ReplayCursor]["gamestate"] != 2)
            --SharedRefs.ReplayCursor;
        --SharedRefs.ReplayCursor;
        DoneAndGoBackToPreparation();
    }

    public void NextRound()
    {
        while ((int) SharedRefs.ReplayJson[SharedRefs.ReplayCursor]["gamestate"] != 2)
        {
            ++SharedRefs.ReplayCursor;
            if (ErrorParser.HandleErrorCheck(this)) return;
        }
        DoneAndGoBackToPreparation();
    }

    public void BackHome()
    {
        SceneManager.LoadScene("Scenes/Welcome");
    }

    public void ToggleAutoPlay()
    {
        SharedRefs.AutoPlay = !SharedRefs.AutoPlay;
        if (SharedRefs.AutoPlay && nextStepButton.interactable) NextStep();
    }

    // Extension methods

    // void MakeAGuess(bool enemy, int timeout)

    // async void NewRound()

    // void MoveCursor()

    // async void ChangeStatus()

    private void Awake()
    {
        this.AwakeImpl();
    }

    protected override void RunPerFrame()
    {
        this.RunPerFrameImpl();
    }

    public GameUI()
    {
        GameState = new GameStates();
        Gom = new GameObjectManager(GameState);
    }
}
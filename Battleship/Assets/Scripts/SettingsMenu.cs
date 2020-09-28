﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Button _backToStartMenuButton;
    [SerializeField] private Button _muteButton;
    [SerializeField] private Sprite[] _soundSprites;
    [SerializeField] private Sprite currentSoundButtonIcon;
    private DataManager _dataManager;

    public void Init(Action onBackToStartMenuAction)
    {
        _dataManager = new DataManager();
        _backToStartMenuButton.onClick.RemoveAllListeners();
        _backToStartMenuButton.onClick.AddListener(onBackToStartMenuAction.Invoke);
        _muteButton.onClick.RemoveAllListeners();
        _muteButton.onClick.AddListener(MuteFunction);
        _muteButton.onClick.AddListener(ChangeIcon);
    }

    private void MuteFunction()
    {
        _dataManager.UserData.IsMuted = !_dataManager.UserData.IsMuted;
    }

    private void ChangeIcon()
    {
        switch (_dataManager.UserData.IsMuted)
        {
            case false:
                currentSoundButtonIcon = _soundSprites[0];
                break;
            case true:
                currentSoundButtonIcon = _soundSprites[1];
                break;
        }

        _muteButton.GetComponent<Image>().sprite = currentSoundButtonIcon;
    }
}
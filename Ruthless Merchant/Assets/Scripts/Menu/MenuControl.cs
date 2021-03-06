﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Fabian Subat, Nikolas Pietrek
/// </summary>

namespace RuthlessMerchant
{
    public class MenuControl : MonoBehaviour
    {

        #region Fields
        private bool gameIsPaused = false;
        [SerializeField]
        private string gamePlaySceneName, mainMenuSceneName;
        [SerializeField]
        private GameObject BookPrefab, pauseMenu, settingsMenu, loadMenu, saveMenu;
        private GameObject currentState;
        private PageLogic book;
        [SerializeField]
        private Player player;
        #endregion

        private enum MenuStates
        {
            Pause,
            Settings,
            Load,
            Save
        };

        #region Methods
        public void Awake()
        {
            currentState = pauseMenu;
            book = GetComponentInChildren<PageLogic>();
        }
        public void PlayButton()
        {
            SceneManager.LoadScene(gamePlaySceneName);
        }

        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }

        public void QuitButton()
        {
            Debug.Log("Quit");
            Application.Quit();
        }

        public void Update()
        {
            //if(Input.GetKeyDown(KeyCode.Escape))
            //{
            //    if (gameIsPaused)
            //        Resume();
            //    else
            //    {
            //        Pause();
            //    }
            //}
        }

        public void Resume()
        {
            if (player != null)
            {
                player.gameObject.GetComponentInChildren<Animator>().SetBool("IsReading", false);
            }
            BookPrefab.SetActive(false);
            Time.timeScale = 1f;
            //SwitchMenu(MenuStates.Pause);
            RuthlessMerchant.Player.RestrictCamera = false;
            gameIsPaused = false;
        }

        private void Pause()
        {
            if (player != null)
            {
                player.gameObject.GetComponentInChildren<Animator>().SetBool("IsReading", true);
            }
            BookPrefab.SetActive(true);
            Time.timeScale = 0f;
            RuthlessMerchant.Player.RestrictCamera = true;
            gameIsPaused = true;
        }

        public void Settings()
        {
            book.OpenSettings();
        }

        public void SaveGameMenu()
        {
            SwitchMenu(MenuStates.Save);
        }

        public void LoadGameMenu()
        {
            SwitchMenu(MenuStates.Load);
        }

        public void SettingsMenu()
        {
            SwitchMenu(MenuStates.Settings);
        }

        public void PauseMenu()
        {
            SwitchMenu(MenuStates.Pause);
        }

        private void SwitchMenu(MenuStates menu)
        {
            GameObject newState;

            switch (menu)
            {
                case MenuStates.Pause:
                    newState = pauseMenu;
                    break;
                case MenuStates.Settings:
                    newState = settingsMenu;
                    break;
                case MenuStates.Load:
                    newState = loadMenu;
                    break;
                case MenuStates.Save:
                    newState = saveMenu;
                    break;
                default:
                    newState = pauseMenu;
                    break;
            }

            currentState.SetActive(false);
            currentState = newState;
            currentState.SetActive(true);
        }
        #endregion
    }
}

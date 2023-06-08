using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLogic : MonoBehaviour
{
    [SerializeField] GameData gameData;

    private void Start()
    {
        Screen.SetResolution(1280, 720, false);
    }
    public void StartGame()
    {
        gameData.currentDayNumber = 0;
        gameData.currentSceneIndex = 1;
        SceneHandler sceneHandler = FindObjectOfType<SceneHandler>();
        sceneHandler.LoadScene(gameData.currentSceneIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RestartButtonClick : MonoBehaviour {

    private void OnMouseUp()
    {
        GameStateAndScoreManager.currentLevel = 0;   //обнуления текущего уровня, чтобы игрок начал игру заново 
        GameStateAndScoreManager.score = 0;
        SceneManager.LoadScene("SampleScene");    //загрузка основной сцены
    }
}

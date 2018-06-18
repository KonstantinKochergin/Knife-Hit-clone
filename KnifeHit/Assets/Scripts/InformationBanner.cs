using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationBanner : MonoBehaviour {   //Данный скрипт просто отображает набранный игроком результат и рекорд.
                                                   //Рекорд записывается в PlayerPrefs, ниже сравнивается текущий результат с рекордом и в случае надобности рекорд перезаписывается
                                                   //Если игрок зашел в игру первый раз, то в PlayerPrefs создается поле record и туда записывается текущий результат


    [SerializeField]
    TextMesh scoreText;
    [SerializeField]
    TextMesh bestText;

    void Start () {
        scoreText.text = GameStateAndScoreManager.score.ToString();
        if (PlayerPrefs.HasKey("record"))
        {
            if (GameStateAndScoreManager.score > PlayerPrefs.GetInt("record"))
            {
                PlayerPrefs.SetInt("record", GameStateAndScoreManager.score);
            }
        }
        else
        {
            PlayerPrefs.SetInt("record", GameStateAndScoreManager.score);
        }
        bestText.text = PlayerPrefs.GetInt("record").ToString();
	}
	
	
}

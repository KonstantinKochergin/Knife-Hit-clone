using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour {

    [SerializeField]
    private Transform startPosition;


    [SerializeField]
    private GameStateAndScoreManager gameStateAndScoreManager;

    [SerializeField]
    private GameObject log; // бревно

    [SerializeField]
    private float speed;

    public int state; //состояние ножа: 1 -  стоит в начальной точке, после клика на экран полетит вперед
                       //2 - нож летит вперед к бревну
                       //3 - нож успешно воткнулся в бревно
                       //0 - анимация перезарядки

   
    void Start () {
        state = 0;
        startPosition = GameObject.Find("StartKnifePosition").transform;
        log = GameObject.Find("log"); //инициализируем бревно по имени
        gameStateAndScoreManager = GameObject.Find("GameManager").GetComponent<GameStateAndScoreManager>(); //инициализируем объект гейм менеджера
      
	}
	
	
	void Update () {
        if (state == 0)
        {
            knifeReloadAnimation();
        }
        if (state == 2)
        {
            moveForward();
            if (this.transform.position.y >= 1.1f)                          //1.1 это координата y, при которой нож втыкается в бревно 
            {
                if (gameStateAndScoreManager.isPlaceFree())
                {
                    state = 3;
                    stickingKnife();
                }
            }
        }
       

    }

    void stickingKnife()        // метод привязки успешно воткнувшегося ножа к бревну для их общего вращения 
    {
        this.transform.parent = log.transform;  
    }

    void moveForward()
    {
        this.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
    }

    void knifeReloadAnimation()  //метод создает небольшую анимацию перезарядки ноже (в момент когда новый нож создается, он немного приподнимается вверх)
    {
        //на это растояние нож приподнимается в начале  deltaY = 1f
        float speed = 10f; //скорость поднятия, не связана со скоростью запуска ножа
        if(this.transform.position.y < startPosition.transform.position.y)
        {
            this.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        }
        else
        {
            state = 1;
        }
    }
}

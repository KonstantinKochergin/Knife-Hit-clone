using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine;

public class GameStateAndScoreManager : MonoBehaviour {


    public List<Knife> knifes = new List<Knife>();  //список ножей

    private int knifesInLog;   //текущее кол-во ножей, воткнутых в бревно, понадобится для определия момента разрушения бревна

    [SerializeField]
    private GameObject knife;   //префаб ножа

    [SerializeField]
    private Transform startPosition;  //начальное положение перед запуском

    [SerializeField]
    private GameObject log;
    [SerializeField]
    private GameObject logPart1;
    [SerializeField]
    private GameObject logPart2;
    [SerializeField]
    private GameObject logPart3;

    [SerializeField]
    private TextMesh scoreLabel;


    [SerializeField]
    private GameObject blackKnifeIcon;
    [SerializeField]
    private GameObject grayKnifeIcon;
    List<GameObject> blackIcons = new List<GameObject>();
    List<GameObject> grayIcons = new List<GameObject>();  //списки иконок, для их последующих отрисовок


    int gameState; //1 - игра идет, любое другое значение игра приостановлена (например в момент разрешения бревна, при переходе на следующий уровень)

    public static int currentLevel = 0; //текущий уровень, с каждым последующим игроку придется воткнуть в бревно больше ножей, чтобы пройти уровень
                                        //новый уровень = +1 нож  

    public static int score = 0; //очки набранные игроком, один воткнутый нож = +1 очко

    public static bool isReloading; //нужно ли перезагружать сцену

    int a;
	
	void Start () {
        a = 0;
        gameState = 1;
        GameStateAndScoreManager.isReloading= false;
        knifesInLog = 0;  //инициализация
        knifes.Add(Instantiate(knife, startPosition.position, Quaternion.identity).GetComponent<Knife>()); // первый нож на поле
	}
	
	
	void Update () {
        knifeIconsDraw();

        scoreLabel.text = score.ToString();

      //  levelReload();
		if(knifesInLog == 5+GameStateAndScoreManager.currentLevel)
        {
            //Debug.Log(GameStateAndScoreManager.currentLevel);
            GameStateAndScoreManager.currentLevel += 1;
            Boom();
          
        }
        if (GameStateAndScoreManager.isReloading)
        {
            SceneManager.LoadScene("SampleScene");
        }
	}

    private void OnMouseUp()
    {
        if (gameState == 1)
        {
            // knifes.Add(Instantiate(knife, startPosition.position, Quaternion.identity).GetComponent<Knife>());  ///при нажатии на экран создается новый нож и сразу же записывается в лист
            knifes.Add(Instantiate(knife, new Vector3(startPosition.position.x, startPosition.position.y-1, startPosition.position.z), Quaternion.identity).GetComponent<Knife>());
            knifes[knifes.Count - 2].state = 2;    //нож, который стоял в начальном положении переходит в состояние 2 и летит к бревну
        }
    }

    void Boom()   //данный метод реализует разлет бревна и ножей, после уничтожения бревна
    {
        gameState = 2; //смена состояния игры, чтобы игрок не мог запускать ножи, во время разрушения бревна      
        //у всех воткнутых ножей включаем rigidbody2D, что они разлетелись и отвязываем их от бревна, чтобы они не исчезли вместе с ним
        foreach (Knife knife in knifes)
        {
            if (knife.state == 3)
            {
                knife.transform.SetParent(null); 
                knife.gameObject.GetComponent<Rigidbody2D>().simulated = true;
                Rigidbody2D rigidbody2d = knife.gameObject.GetComponent<Rigidbody2D>();
                rigidbody2d.AddForce(Vector2.up*10f*rigidbody2d.velocity.normalized*1000, ForceMode2D.Force); 
            }
        }
        //убираем целое бревно и заменяем его кусками
        log.SetActive(false);       
        logPart1.SetActive(true);
        logPart1.GetComponent<Rigidbody2D>().AddForce(-Vector2.up * 200f * logPart1.GetComponent<Rigidbody2D>().velocity.normalized, ForceMode2D.Impulse);
        logPart2.SetActive(true);
        logPart2.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 200f * logPart2.GetComponent<Rigidbody2D>().velocity.normalized, ForceMode2D.Impulse);
        logPart3.SetActive(true);
        logPart3.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 200f * logPart3.GetComponent<Rigidbody2D>().velocity.normalized, ForceMode2D.Impulse);
        Thread thread = new Thread(threadForWaitingReload);
        thread.Start();

    }

    static void threadForWaitingReload()  //поток создан, для того, чтобы перезагружать сцену с задержкой, чтобы игрок мог увидеть анимацию разлета бревна и ножей(анимация получилась очень кривой)
    {
        Thread.Sleep(1000);
        GameStateAndScoreManager.isReloading = true;
    }


    public bool isPlaceFree()  //метод отвечает за "втыкание ножа в бревно
    {
        if (gameState == 1)  // защита от бага ( чтобы во время разрушения бревна, нож случайно выпущенный нож(был замечен баг: иногда после смены состояния игры, ровно один кадр еще можно выпускать ножи в методе OnMouseUp) не вызвал гейм овер
        {
            foreach (Knife knife in knifes)
            {
                //далее смотрим по координатам, нет ли ножа в области "втыкания" (примечание: эта область всегда статична)    
                if (knife.state == 3) //смотрим только те ножи, которые уже воткнуты
                {

                    if ((knife.transform.position.y < 1.5) && (knife.transform.position.x > -0.15) && (knife.transform.position.x < 0.35))   //иногда случается непонятный баг и условие не проходит
                    {
                       

                        SceneManager.LoadScene("GameOverScene"); //в случае если нож врезается в другой, загружается сцена, на которой игроку объявляют об окончании игры
                        return false;                            //примeчание: при отладке в самой unity сцена, возможно, не будет подгружаться см. настройки билда
                    }
                }
            }
            knifesInLog++; //нож успешно втыкатется в бревно и мы это фиксируем
            GameStateAndScoreManager.score++; //увеличиваем кол-во баллов
        }
 
        return true;
    }

    void knifeIconsDraw()  /// метод отображает черные и серые иконки мечей слева, которые говорят игроку сколько еще ножей нужно воткнуть, для перехода на следующий уровень
    {
        a++; // а отвечает за то, чтобы иконки создались только один раз(данный метод я буду вызывать в Update)
        float deltaY = 0.5f;  //разница между иконками по оси Y, высчитона в эдиторе  
        float startY = -4f;  //Y самого нижнего элемента
        float iconsX = -3.16f; //координаты иконок по оси X, у всех иконок значение одинакого
        if (a == 1)  ///на следующем кадре a уже будет равна 2 и иконки не будут создаваться снова
        {
            for (int i = 0; i < (5 + currentLevel); i++)
            {
                blackIcons.Add(Instantiate(blackKnifeIcon, new Vector2(iconsX, startY + deltaY * i), Quaternion.identity));
                grayIcons.Add(Instantiate(grayKnifeIcon, new Vector2(iconsX, startY + deltaY * i), Quaternion.identity));
            }
        }
        if (knifesInLog > 0)
        {
            grayIcons[grayIcons.Count - knifesInLog].SetActive(false);   //оключаем серые иконки в зависимости от кол-ва воткнутых ножей, так игрок видит свой прогресс на текущем уровне
        }
        
    }


  

}

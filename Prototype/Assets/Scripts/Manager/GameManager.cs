using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public enum Stage {STAGE1, STAGE2, STAGE3, STAGEFINAL };
    public Stage GameStage = Stage.STAGE1;
    public bool isGameOver = false;

    public bool ZoomInCheck = true; 
    public bool CountCehck = false;
    public bool BossCheck = false;

    public bool TutorialCheck = false;
    public enum Tutorial { MOVING, DASH, ATTACK, SKILL, END }
    public Tutorial GameTutorial = Tutorial.MOVING;
    public Tutorial CopyGameTutorial;

    public GameObject DamageMeter;
    public RectTransform DamageMeterPos;
    public Canvas UI;

    public EnemySpawn[] EnemySpawner;
    public Animator DoorLeft;
    public Animator DoorRight;
    public GameObject Warp;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        EnemySpawner = GameObject.Find("EnemySpawner").GetComponentsInChildren<EnemySpawn>();
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        //        Time.timeScale = 0.1f;
    }

    // Use this for initialization
    void Start()
    {
        
        StartCoroutine(StageCheck());
        SoundManager.Instance.PlayBGM(GetComponent<AudioSource>(), SoundManager.Instance.StageRainBGM, true, PlayerPrefs.GetFloat("BGMVolume", 1));
        if (!UIManager.Instance.UISlider[1].interactable) GetComponent<AudioSource>().volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(UIManager.instance.FadeInOut.GetComponent<CanvasGroup>().alpha == 0)
        {
            TutorialProgress();
            if (GameTutorial == Tutorial.END) return;
            if (CopyGameTutorial != GameTutorial) return;
            if(!TutorialCheck) TutorialPlay();
        }
        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    if (EnemySpawner[(int)GameStage].NextStage.name == "Stage0") return;
        //    EnemySpawner[(int)GameStage].m_AllEnemy.Clear();
        //    EnemySpawner[(int)GameStage].AllEnemyTureActive();
        //    EnemySpawner[(int)GameStage].AllEnemyTureActiveSkillStage();
        //    EnemySpawner[(int)GameStage].OnEnemyCreate();
        //}
        //if (Input.GetKeyDown(KeyCode.F2)) GameObject.Find("Player").transform.position = Warp.transform.position;

        //        if (Input.GetButtonDown("Fire1")) Cursor.lockState = CursorLockMode.Locked;
    }

    // 1스테이지가 시작된다.
    // 1스테이지에서 몬스터를 다 잡으면 1스테이지의 구간에 있는 막혀있는 문을 열리고
    // 2스테이지에 있는 몬스터가 활성화가 된다.
    // >>>>>>>>>>>>>>>>
    // 마지막 스테이지의 몬스터를 다 잡으면 보스방의 입구가 열린다.

    public IEnumerator StageCheck()             // 스테이지 클리어 체크
    {
        
        while (!isGameOver)
        {
            if(EnemySpawner[(int)GameStage].m_AllEnemy.Count == 0)
            {
                if (EnemySpawner[(int)GameStage].NextStage.name == "Stage0") CountCehck = true;
                if (!CountCehck)                        // 구간 넘어 갈때 이벤트 발생
                {
                    CameraManager.Instance.DoorEventCheck = true;
                    if (!ZoomInCheck)
                    {
                        yield return new WaitForSeconds(2f);
                        UIManager.Instance.StartUGUIFadeInOut(UIManager.Instance.FadeInOut, 2, 0);
                        yield return new WaitForSeconds(0.5f);
                        CameraManager.Instance.DoorCameraChange();
                        StartCoroutine(CameraManager.Instance.DoorMove(EnemySpawner[(int)GameStage]));
                        CountCehck = true;
                    }
                }

                if (!CameraManager.Instance.DoorEventCheck)             // 메인카메라로 돌아올때 
                {
                    //if ((int)GameStage < UIManager.Instance.ProperyBlock.Length)
                    //    UIManager.Instance.ProperyBlock[(int)GameStage].SetActive(false);
                    if(GameStage == Stage.STAGE3)
                    {
                        DoorLeft = GameObject.Find("door_left").GetComponent<Animator>();
                        DoorRight = GameObject.Find("door_right").GetComponent<Animator>();
                    }
                    GameStage++;
                    if (GameStage == Stage.STAGEFINAL) break;
                    EnemySpawner[(int)GameStage].AllEnemyTureActive();
                    EnemySpawner[(int)GameStage].OnEnemyCreate();
                    ZoomInCheck = true;
                    CountCehck = false;
                }
                //                EnemySpawner[(int)GameStage].NextStage.SetActive(false);
            }
            

            yield return new WaitForEndOfFrame();
        }
        
    }

    public IEnumerator CreateDamageMeter1(string name)
    {
        GameObject newDamageMeter;
        newDamageMeter = Instantiate(DamageMeter);
        newDamageMeter.transform.SetParent(UI.transform, false);
        newDamageMeter.transform.position = DamageMeterPos.position;
        newDamageMeter.GetComponent<Text>().text = "안내 : " + name;
        float time = 3;
        while (time > 0)
        {
            time -= Time.deltaTime;
            newDamageMeter.transform.Translate(Vector3.up * 100 * Time.deltaTime);
            yield return null;
        }
        Destroy(newDamageMeter);
    }

    public EnemySpawn NowGameSpawner()
    {
        if (GameStage == Stage.STAGEFINAL) return null;
        return EnemySpawner[(int)GameStage];
    }

    public void DoorAnimPlay()
    {
        if (DoorLeft == null || DoorRight == null)
        {
            BossCheck = true;
            DoorLeft = GameObject.Find("door_left").GetComponent<Animator>();
            DoorRight = GameObject.Find("door_right").GetComponent<Animator>();
            DoorLeft.SetTrigger("Open");
            DoorRight.SetTrigger("Open");
        }
    }

    public void TutorialPlay()
    {
        TutorialCheck = true;
        CopyGameTutorial = GameTutorial;
        UIManager.Instance.Tutorial.SetActive(true);
        UIManager.Instance.TutorialText[(int)GameTutorial].SetActive(true);
        if(GameTutorial == Tutorial.SKILL)
            UIManager.instance.TutorialDash.SetActive(true);
        Time.timeScale = 0;
    }

    public void TutorialProgress()
    {
        if (!TutorialCheck) return;
        switch (GameTutorial)
        {
            case Tutorial.MOVING:
                if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W)||
                    Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))
                {
                    Time.timeScale = 1;
                    UIManager.Instance.Tutorial.SetActive(false);
                    UIManager.Instance.TutorialText[(int)GameTutorial].SetActive(false);
                    GameTutorial++;
                    TutorialCheck = false;
                }
                break;
            case Tutorial.DASH:
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Time.timeScale = 1;
                    UIManager.Instance.Tutorial.SetActive(false);
                    UIManager.Instance.TutorialText[(int)GameTutorial].SetActive(false);
                    GameTutorial++;
                    TutorialCheck = false;
                }
                break;
            case Tutorial.ATTACK:
                if (Input.GetButton("Fire1"))
                {
                    Time.timeScale = 1;
                    UIManager.Instance.Tutorial.SetActive(false);
                    UIManager.Instance.TutorialText[(int)GameTutorial].SetActive(false);
                    GameTutorial++;
                    TutorialCheck = false;
                }
                break;
            case Tutorial.SKILL:
                if (Input.GetKey(KeyCode.Q))
                {
                    Time.timeScale = 1;
                    UIManager.Instance.Tutorial.SetActive(false);
                    UIManager.Instance.TutorialText[(int)GameTutorial].SetActive(false);
                    UIManager.instance.TutorialDash.SetActive(false);
                    GameTutorial++;
                    TutorialCheck = false;
                }
                break;
            case Tutorial.END:
                break;
            default:
                break;
        }
    }

    public void TutorialChange()
    {
        CopyGameTutorial = GameTutorial;
    }
}
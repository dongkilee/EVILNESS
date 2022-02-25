using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIManager : Singleton<UIManager>
{
    public GameObject BossHPBar;
    public Image BossHP;
    public GameObject FadeInOut;

    public GameObject StopPop;
    public GameObject[] MenuPop; // 0 메뉴 1 옵션 2 게임 종료

    public Sprite[] Number;
    public Image[] ComboNum;

    public Slider[] UISlider; // 0 Mouse 2 BGM 3 Sound

    public GameObject GameFinish;
    public GameObject GameOver;
    public GameObject GameClear;

    //----------------

    public GameObject m_ComboUI;

    //   public GameObject m_nowComboUI;
    public bool Pause;
    public Image m_Gauge;
    public GameObject m_GaugeEffectUI;
    // 플레이어 위험할 때
    public GameObject m_Player_Danger;

    public MouseOrbit MainCamera;

    public AudioSource[] BGMAudio;

    public GameObject Arrow;

    public GameObject Tutorial;
    public GameObject[] TutorialText;
    public GameObject TutorialDash;

    public Toggle[] UIToggle; // 0 BGM 1 Sound
    public GameObject[] UIMute;

    // Use this for initialization
    void Start()
    {
        MainCamera = GameObject.FindWithTag("Camera").GetComponent<MouseOrbit>();
        UISlider[0].value = PlayerPrefs.GetFloat("Mouse", 1);
        UISlider[1].value = PlayerPrefs.GetFloat("BGMVolume", 1);
        UISlider[2].value = PlayerPrefs.GetFloat("SoundVolume", 1);

        string value = PlayerPrefs.GetString("BGMToggle", "false");
        bool isActive = System.Convert.ToBoolean(value);
        UIToggle[0].isOn = isActive;
        value = PlayerPrefs.GetString("SoundToggle", "false");
        isActive = System.Convert.ToBoolean(value);
        UIToggle[1].isOn = isActive;

        UISlider[1].interactable = !UIToggle[0].isOn;
        UIMute[0].SetActive(UIToggle[0].isOn);
        UISlider[2].interactable = !UIToggle[1].isOn;
        UIMute[1].SetActive(UIToggle[1].isOn);
        //       m_hpOnePixel = 340f / m_maxHP;
        //--------------------
        m_ComboUI.SetActive(false);
        if (!UISlider[1].interactable) BGMAudio[0].volume = BGMAudio[1].volume = 0;
        if (!UISlider[2].interactable) SoundManager.SoundVolume = 0;
    }

    // Update is called once per frame
    void Update()
    {

//        Debug.Log(Cursor.lockState);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIManager.Instance.FadeInOut.GetComponent<CanvasGroup>().alpha != 0) return;
            if (GameManager.Instance.TutorialCheck) return;
            if (StopPop == null) return;
            if (GameManager.Instance.BossCheck) return;
            if (CameraManager.Instance.DoorEventCheck) return;

            if (!Pause)
            {
                Pause = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                StopPop.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                BT_Resume();
            }
        }

    }

    public void StartUGUIFadeInOut(GameObject target, float time, float delay)
    {
        StartCoroutine(UGUIFadeInOut(target, time, delay));
    }

    public IEnumerator UGUIFadeInOut(GameObject target, float time, float delay)        // 페이드인아웃
    {
        CanvasGroup targetGroup = target.GetComponent<CanvasGroup>();
        targetGroup.interactable = true;
        float nowAlpha = targetGroup.alpha;

        while (nowAlpha <= 1)
        {
            nowAlpha += Time.deltaTime * time;
            targetGroup.alpha = nowAlpha;
            yield return null;
        }

        yield return new WaitForSeconds(delay);

        while (nowAlpha >= 0)
        {
            nowAlpha -= Time.deltaTime * time;
            targetGroup.alpha = nowAlpha;
            yield return null;
        }

        targetGroup.interactable = false;
        yield return null;
    }

    public void StartUGUIFadeIn(GameObject target, float alpha, float time)
    {
        StartCoroutine(UGUIFadeIn(target, alpha, time));
    }

    public IEnumerator UGUIFadeIn(GameObject target, float alpha, float time)       // 페이드 인 또는 아웃 한번만 됨
    {
        CanvasGroup targetGroup = target.GetComponent<CanvasGroup>();
        targetGroup.interactable = true;
        float nowAlpha = targetGroup.alpha;
        bool check = nowAlpha <= alpha ? true : false;
        if (check)
        {
            while (nowAlpha <= alpha)
            {
                nowAlpha += Time.deltaTime * time;
                targetGroup.alpha = nowAlpha;
                yield return null;
            }
        }
        else
        {
            while (nowAlpha >= alpha)
            {
                nowAlpha -= Time.deltaTime * time;
                targetGroup.alpha = nowAlpha;
                yield return null;
            }
        }

        targetGroup.interactable = false;
        yield return null;
    }

    public void StartOnScale(GameObject target, float alpha, float time)
    {
        StartCoroutine(OnScaleFadeIn(target, alpha, time));
    }

    public IEnumerator OnScaleFadeIn(GameObject target, float alpha, float time)
    {
        Vector3 newScale = target.transform.localScale;
        bool check = newScale.x <= alpha ? true : false;
        if (check)
        {
            target.transform.GetChild(0).gameObject.SetActive(true);
            while (newScale.x <= alpha)     // 켜진다.
            {
                newScale += (Vector3.one * Time.deltaTime * time);
                target.transform.localScale = newScale;
                yield return null;
            }
        }
        else
        {
            while (newScale.x >= alpha)     // 꺼진다.
            {
                newScale += (-Vector3.one * Time.deltaTime * time);
                target.transform.localScale = newScale;
                yield return null;
            }
            target.transform.localScale = Vector3.zero;
            target.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    //------------------------------------------------------------

    public void DangerView()
    {
        Image danger = m_Player_Danger.GetComponent<Image>();
        float a = 1;
        Color color = new Color(1, 1, 1, a);
        m_Player_Danger.SetActive(true);

        danger.color = color;


    }

    public void ComboUIView(int combo)
    {
        /*
        if (combo == 1)
        {
            //    m_ComboUI.SetActive(false);
            GameObject ComboText = Instantiate(m_ComboText);
            m_ComboText.GetComponentInChildren<Text>().text = m_player.GetStat().m_Combo.ToString();
            ComboText.transform.parent = m_ComboUI.transform;
            m_nowComboUI = ComboText;
            m_ComboUI.SetActive(true);
        }
        else
        {
            m_nowComboUI.GetComponent<Animator>().SetBool("Next", true);
            GameObject ComboText = Instantiate(m_ComboText);
            m_ComboText.GetComponentInChildren<Text>().text = m_player.GetStat().m_Combo.ToString();
            ComboText.transform.parent = m_ComboUI.transform;
            m_nowComboUI = ComboText;
        }
        */

        m_ComboUI.SetActive(false);
        ComboNumber(combo);
        m_ComboUI.SetActive(true);
    }
    public void ComboUIClose()
    {
        //      GameObject.Destroy(m_nowComboUI);
        ComboNum[0].sprite = null;
        ComboNum[1].sprite = null;
        ComboNum[2].sprite = null;
        m_ComboUI.SetActive(false);
    }

    public void BT_Resume()
    {
        if (StopPop.activeInHierarchy)
        {
            Pause = false;
            StopPop.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void BT_Option()
    {
        MenuPop[0].SetActive(false);
        MenuPop[1].SetActive(true);
        MenuPop[2].SetActive(false);
    }

    public void BT_GameExit()
    {
        MenuPop[0].SetActive(false);
        MenuPop[1].SetActive(false);
        MenuPop[2].SetActive(true);
    }

    public void BT_Menu()
    {
        MenuPop[0].SetActive(true);
        MenuPop[1].SetActive(false);
        MenuPop[2].SetActive(false);
    }

    public void BT_EXIT()
    {
        StartCoroutine(UGUIFadeInOut(FadeInOut, 2f));
//        Application.Quit();
    }

    public void BT_BackMenu()
    {

        UISlider[0].value = PlayerPrefs.GetFloat("Mouse", 1);
        MainCamera.Responsiveness = PlayerPrefs.GetFloat("Mouse", 1) * 0.01f;
        BGMAudio[0].volume = BGMAudio[1].volume = UISlider[1].value = SoundManager.BGMVolume = PlayerPrefs.GetFloat("BGMVolume", 1);
        UISlider[2].value = SoundManager.SoundVolume = PlayerPrefs.GetFloat("SoundVolume", 1);

        string value = PlayerPrefs.GetString("BGMToggle", "false");
        bool isActive = System.Convert.ToBoolean(value);
        UIToggle[0].isOn = isActive;
        value = PlayerPrefs.GetString("SoundToggle", "false");
        isActive = System.Convert.ToBoolean(value);
        UIToggle[1].isOn = isActive;

        UISlider[1].interactable = !UIToggle[0].isOn;
        UIMute[0].SetActive(UIToggle[0].isOn);
        UISlider[2].interactable = !UIToggle[1].isOn;
        UIMute[1].SetActive(UIToggle[1].isOn);

        if (!UISlider[1].interactable) BGMAudio[0].volume = BGMAudio[1].volume = 0;
        if (!UISlider[2].interactable) SoundManager.SoundVolume = 0;

        MenuPop[0].SetActive(true);
        MenuPop[1].SetActive(false);
        MenuPop[2].SetActive(false);
    }

    public void BT_Apply()
    {
        PlayerPrefs.SetFloat("Mouse", UISlider[0].value);
        PlayerPrefs.SetFloat("BGMVolume", UISlider[1].value);
        PlayerPrefs.SetFloat("SoundVolume", UISlider[2].value);

        bool isActive = UIToggle[0].isOn;
        PlayerPrefs.SetString("BGMToggle", isActive.ToString());
        isActive = UIToggle[1].isOn;
        PlayerPrefs.SetString("SoundToggle", isActive.ToString());

        UISlider[0].value = PlayerPrefs.GetFloat("Mouse", 1);
        MainCamera.Responsiveness = PlayerPrefs.GetFloat("Mouse", 1) * 0.01f;
        BGMAudio[0].volume = BGMAudio[1].volume = UISlider[1].value = SoundManager.BGMVolume = PlayerPrefs.GetFloat("BGMVolume", 1);
        UISlider[2].value = SoundManager.SoundVolume = PlayerPrefs.GetFloat("SoundVolume", 1);

        if (!UISlider[1].interactable) BGMAudio[0].volume = BGMAudio[1].volume = 0;
        if (!UISlider[2].interactable) SoundManager.SoundVolume = 0;
    }

    public void BGMToggle()
    {
        UISlider[1].interactable = !UIToggle[0].isOn;
        UIMute[0].SetActive(UIToggle[0].isOn);

        if (!UISlider[1].interactable) BGMAudio[0].volume = BGMAudio[1].volume = 0;
        else if (UISlider[1].interactable) BGMAudio[0].volume = BGMAudio[1].volume = UISlider[1].value;
    }

    public void SoundToggle()
    {
        UISlider[2].interactable = !UIToggle[1].isOn;
        UIMute[1].SetActive(UIToggle[1].isOn);

        if (!UISlider[2].interactable) SoundManager.SoundVolume = 0;
        else if (UISlider[2].interactable) SoundManager.SoundVolume = UISlider[2].value;
    }

    public void S_Mouse()
    {
        MainCamera.Responsiveness = UISlider[0].value * 0.01f;
    }

    public void S_BGM()
    {
        SoundManager.BGMVolume = UISlider[1].value;
        BGMAudio[0].volume = SoundManager.BGMVolume;
        BGMAudio[1].volume = SoundManager.BGMVolume;
    }

    public void S_Sound()
    {
        SoundManager.SoundVolume = UISlider[2].value;
    }

    public IEnumerator UGUIFadeInOut(GameObject target, float time)        // 페이드인아웃
    {
        target.SetActive(true);
        CanvasGroup targetGroup = target.GetComponent<CanvasGroup>();
        targetGroup.interactable = true;
        float nowAlpha = targetGroup.alpha;

        while (nowAlpha <= 1)
        {
            nowAlpha += Time.fixedDeltaTime * time;
            targetGroup.alpha = nowAlpha;
            yield return null;
        }
        targetGroup.interactable = false;
        SceneManager.LoadSceneAsync(0);
    }

    public void ComboNumber(int _combo)
    {
        if(_combo < 10)
        {
            ComboNum[0].gameObject.SetActive(true);
            ComboNum[1].gameObject.SetActive(false);
            ComboNum[2].gameObject.SetActive(false);
        }else if(_combo < 100)
        {
            ComboNum[0].gameObject.SetActive(true);
            ComboNum[1].gameObject.SetActive(true);
            ComboNum[2].gameObject.SetActive(false);
        }else
        {
            ComboNum[0].gameObject.SetActive(true);
            ComboNum[1].gameObject.SetActive(true);
            ComboNum[2].gameObject.SetActive(true);
        }
        ComboNum[0].sprite = Number[_combo % 10];
        ComboNum[1].sprite = Number[(int)((_combo % 100) / 10)];
        ComboNum[2].sprite = Number[(int)((_combo % 1000) / 100)];
    }

    public IEnumerator GameFinishPlay(GameObject Game)
    {
        GameFinish.SetActive(true);
        Game.SetActive(true);
        m_GaugeEffectUI.SetActive(false);
        m_ComboUI.SetActive(false);
        yield return new WaitForSeconds(5);

        BT_EXIT();
    }
}

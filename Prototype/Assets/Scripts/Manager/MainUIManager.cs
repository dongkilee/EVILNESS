using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUIManager : Singleton<MainUIManager> {

    public GameObject FadeInOut;
    public GameObject[] Option; // 0 메인화면 1 옵션 2 개발자 3 게임종료

    public Slider[] UISlider; // 0 Mouse 1 BGM 2 Sound

    public Toggle[] UIToggle; // 0 BGM 1 Sound
    public GameObject[] UIMute;

    public AudioSource BGMAudio;

    void Awake()
    {
        BGMAudio = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
        Time.timeScale = 1;
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

        if (!UISlider[1].interactable) BGMAudio.volume = 0;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartUGUIFadeInOut(GameObject target, float time)
    {
        StartCoroutine(UGUIFadeInOut(target, time));
    }

    public IEnumerator UGUIFadeInOut(GameObject target, float time)        // 페이드인아웃
    {
        target.SetActive(true);
        CanvasGroup targetGroup = target.GetComponent<CanvasGroup>();
        targetGroup.interactable = true;
        float nowAlpha = targetGroup.alpha;

        while (nowAlpha <= 1)
        {
            nowAlpha += Time.deltaTime * time;
            targetGroup.alpha = nowAlpha;
            yield return null;
        }
        targetGroup.interactable = false;
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadSceneAsync(1);
    }

    public void BT_Start()
    {
        StartUGUIFadeInOut(FadeInOut, 2f);
        
    }

    public void BT_Main()
    {
        Option[0].SetActive(true);
        Option[1].SetActive(false);
        Option[2].SetActive(false);
        Option[3].SetActive(false);
    }

    public void BT_Option()
    {
        Option[0].SetActive(false);
        Option[1].SetActive(true);
        Option[2].SetActive(false);
        Option[3].SetActive(false);
    }

    public void BT_Developer()
    {
        Option[0].SetActive(false);
        Option[1].SetActive(false);
        Option[2].SetActive(true);
        Option[3].SetActive(false);
    }

    public void BT_GameExit()
    {
        Option[0].SetActive(false);
        Option[1].SetActive(false);
        Option[2].SetActive(false);
        Option[3].SetActive(true);
    }

    public void BT_GameEXIT_OK()
    {
        Application.Quit();
    }

    public void BT_BackMenu()
    {

        UISlider[0].value = PlayerPrefs.GetFloat("Mouse", 1);
        BGMAudio.volume = UISlider[1].value = PlayerPrefs.GetFloat("BGMVolume", 1);
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

        if (!UISlider[1].interactable) BGMAudio.volume = 0;

        Option[0].SetActive(true);
        Option[1].SetActive(false);
        Option[2].SetActive(false);
        Option[3].SetActive(false);
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
        BGMAudio.volume = UISlider[1].value = PlayerPrefs.GetFloat("BGMVolume", 1);
        UISlider[2].value = PlayerPrefs.GetFloat("SoundVolume", 1);

        if (!UISlider[1].interactable) BGMAudio.volume = 0;
    }

    public void S_BGM()
    {
        BGMAudio.volume = UISlider[1].value;
    }

    public void BGMToggle()
    {
        UISlider[1].interactable = !UIToggle[0].isOn;
        UIMute[0].SetActive(UIToggle[0].isOn);
        Debug.Log(1);
        if (!UISlider[1].interactable) BGMAudio.volume = 0;
        else if (UISlider[1].interactable) BGMAudio.volume = UISlider[1].value;
    }

    public void SoundToggle()
    {
        UISlider[2].interactable = !UIToggle[1].isOn;
        UIMute[1].SetActive(UIToggle[1].isOn);
    }

}

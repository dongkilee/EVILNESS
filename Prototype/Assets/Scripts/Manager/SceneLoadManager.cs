using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager> {
    
    public int additionalScene;
    public int additionalScene2;
    // Use this for initialization
    IEnumerator Start () {
        Time.timeScale = 0;

        AsyncOperation a = SceneManager.LoadSceneAsync(additionalScene, LoadSceneMode.Additive);
        yield return a;
        Scene s = SceneManager.GetSceneByBuildIndex(additionalScene);
        SceneManager.SetActiveScene(s);
        
        UIManager.Instance.StartUGUIFadeInOut(UIManager.Instance.FadeInOut, 2, 0);
        Time.timeScale = 1;
        
        
        // Debug.Log("Scene Loaded");

        yield return null;
	}



    public IEnumerator BossScene()
    {
        GameManager.Instance.DoorAnimPlay();

        yield return new WaitForSeconds(3);

        UIManager.Instance.StartUGUIFadeInOut(UIManager.Instance.FadeInOut, 2f, 2f);

        yield return new WaitForSeconds(1);
        GameManager.Instance.GetComponent<AudioSource>().clip = null;

        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(additionalScene));
        AsyncOperation a = SceneManager.LoadSceneAsync(additionalScene2, LoadSceneMode.Additive);
        yield return a;
        
        Scene s = SceneManager.GetSceneByBuildIndex(additionalScene2);
        SceneManager.SetActiveScene(s);

        yield return null;
        CameraManager.instance.BossCameraChange();
        UIManager.Instance.Arrow.SetActive(false);
    }

	public void BossSceneStart()
    {
        
        GameObject Player = GameObject.Find("Player");
        GameObject Point = GameObject.Find("Point");
        GameObject CameraPoint = GameObject.Find("CameraPoint");
        GameObject Camera = GameObject.Find("Cameras");

        Player.transform.position = Point.transform.position;
        Player.transform.rotation = Point.transform.rotation;
        Camera.transform.position = CameraPoint.transform.position;
        Camera.GetComponent<MouseOrbit>().SetCameraPoint(180, 0);

        EffectManager.Instance.m_Raining.SetActive(false);
        EffectManager.Instance.m_RainingGround.SetActive(false);
        EffectManager.Instance.m_Rain_Fog.SetActive(false);
        EffectManager.Instance.m_FootStepCheck = false;
        GameManager.Instance.BossCheck = false;
    }
}

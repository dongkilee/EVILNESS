using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEventCamera : MonoBehaviour {

    public GameObject EventPlayer;
    public Animator BossAnim;
    public GameObject[] ChairAttack;
    public GameObject Camera;

    public float intensity = 0.01f;
    public float decay = 0.0001f;

    private bool Shaking;                      // 쉐이크 체크.
    private float ShakeDecay;                 // 
    private float ShakeIntensity;
    private Vector3 OriginalPos;
    private Quaternion OriginalRot;

    public void BossAnimStart()         // 보스 애니메이션 스타트
    {
        BossAnim.SetBool("IsEvent", true);
    }

    public void ChairAttackStart()      // 의자 날리기
    {
        StartCoroutine(CharirCoroutine());
    }

    public void GamePlayStart()     // 종료 후 게임 플레이
    {
        SceneLoadManager.Instance.BossSceneStart();
        BossAnim.SetBool("IsEvent", false);
        Destroy(EventPlayer);
        Destroy(gameObject);
        
    }

    public IEnumerator CharirCoroutine()
    {
        float speed = 10;
        while (speed < 15)
        {
            ChairAttack[0].transform.position = ChairAttack[0].transform.position + ChairAttack[0].transform.right * speed * Time.deltaTime;
            ChairAttack[1].transform.position = ChairAttack[1].transform.position - ChairAttack[1].transform.right * speed * Time.deltaTime;
            speed+= 0.1f;
            yield return new WaitForEndOfFrame();
        }

        Destroy(ChairAttack[0]);
        Destroy(ChairAttack[1]);
        yield return null;
    }

    public void FixedUpdate()
    {
        if (ShakeIntensity > 0)
        {
            Camera.transform.position = OriginalPos + Random.insideUnitSphere * ShakeIntensity;
            Camera.transform.rotation = new Quaternion(
                 OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity) * .1f,
                 OriginalRot.y + Random.Range(-ShakeIntensity, ShakeIntensity) * .1f,
                 OriginalRot.z + Random.Range(-ShakeIntensity, ShakeIntensity) * .1f,
                 OriginalRot.w + Random.Range(-ShakeIntensity, ShakeIntensity) * .1f);
            ShakeIntensity -= ShakeDecay;
            //                transform.LookAt(target);
        }
        else if (Shaking)
        {
            Shaking = false;
        }
    }

    public void DoShake()
    {
        OriginalPos = Camera.transform.position;
        
        OriginalRot = Camera.transform.rotation;
        ShakeIntensity = intensity;     // 0.1f
        ShakeDecay = decay;           // 0.001f
        Shaking = true;
    }
}

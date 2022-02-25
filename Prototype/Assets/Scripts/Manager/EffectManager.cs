using UnityEngine;
using System.Collections;

public class EffectManager : Singleton<EffectManager>
{
    public int m_maxAttack;
    public float PlayerEffectZOffset;

    public enum WeaponType { SWORD, HAMMER };
    [System.Serializable]
    public struct sowrdEffect
    {
        public WeaponType type;
        public GameObject[] attack;
    }


    public GameObject m_playerDashA;
    public GameObject m_playerDashB;
    public GameObject m_playerDashC;
    public sowrdEffect[] m_swordEffect;
    public GameObject m_playerFootStep;
    public bool m_FootStepCheck = true;

    public GameObject[] m_hitEffect;    //피격 이펙트 프리팹
    public bool[] m_effectCheck;
    public float m_playerHeight;        //effect 출력 높이

    public GameObject ratEffect;

    public GameObject BossSkillEffect;

    public bool m_isDash;

    public GameObject m_Raining;
    public GameObject m_RainingGround;
    public GameObject m_Rain_Fog;

    GameObject m_player;

    // Use this for initialization
    void Start()
    {
        m_player = GameObject.FindWithTag("Player");
        m_effectCheck = new bool[m_maxAttack]; //4타까지 있으니까 4개로 초기화
        StartCoroutine(PlayEffect());
        m_isDash = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator PlayEffect()
    {
        while (true)
        {
            for (int i = 0; i < m_effectCheck.Length; i++)
            {

                if (m_effectCheck[i])
                {
                    //속성별로 다른 검기를 출력
                    //이펙트 출력
                    CreatePlayerEffect(m_swordEffect[m_player.GetComponent<PlayerManager>().GetWeapon().WeaponID - 1].attack[i], false);
                    m_effectCheck[i] = false;
                }
            }

            if (m_isDash)
            {
                CreateDashEffect(m_playerDashA, true, false);
                CreateDashEffect(m_playerDashB, true, true);
                CreateDashEffect(m_playerDashC, false, false);
                m_isDash = false;
            }

            yield return new WaitForEndOfFrame();

        }

    }
    // 플레이어에게 HitEffect 생성
    public void EnemyPlayerHit(Transform t, GameObject hit)
    {
        GameObject newHitEffect;
        newHitEffect = Instantiate(hit);
        Vector3 newPos = t.position;
        //플레이어 키에 나오게 했는데 몬스터 중앙으로 하려면 다르게..
        newPos.y = m_player.transform.position.y + m_playerHeight;
        newHitEffect.transform.position = newPos;
        Destroy(newHitEffect, 1.0f);

    }

    //잡몹 히트 이펙트 (몬스터 중간에서 터지는거)
    public void PlayEnemyHitEffect(Transform t, int index)
    {
        GameObject newHitEffect;
        newHitEffect = Instantiate(m_hitEffect[index]);
        Vector3 newPos = t.position;
        //플레이어 키에 나오게 했는데 몬스터 중앙으로 하려면 다르게..
        newPos.y = m_player.transform.position.y + m_playerHeight;
        newHitEffect.transform.position = newPos;
        Destroy(newHitEffect, 1.0f);
    }

    public void EnemyRatSkillEffect(Transform t)
    {
        GameObject Effect;
        Effect = Instantiate(ratEffect);
        Effect.transform.parent = t;
        //       newPos.y = m_player.transform.position.y + m_playerHeight;
        Effect.transform.position = t.position;
        Effect.transform.rotation = t.rotation;
        Effect.transform.localScale = t.localScale;
        Destroy(Effect, 1.0f);
    }

    //덩치 큰 보스몹 이펙트 (때리는 곳에서 터지는 거)
    //보스몹일때 data 인수는 맞은 위치의 x 포지션을 넘겨주세요
    public void PlayBossHitEffect(Transform t, float data)
    {
        GameObject newHitEffect;
        Vector3 newPos = new Vector3(data, m_player.transform.position.y + m_playerHeight, t.position.z);
        newHitEffect = Instantiate(m_hitEffect[1]);
        //data.z = t.position.z;
        //data.y = m_playerTr.position.y + m_playerHeight;
        newHitEffect.transform.position = newPos;

        Destroy(newHitEffect, 1.0f);
    }



    void CreatePlayerEffect(GameObject e, bool isFollow)
    {
        GameObject Effect;
        Effect = Instantiate(e);
        if (isFollow)
        {
            Effect.transform.parent = m_player.transform;
            Effect.transform.rotation = m_player.transform.rotation;
            Effect.transform.position = m_player.transform.position - m_player.transform.forward;
        }
        else
        {
            Effect.transform.rotation = m_player.transform.rotation;
            Effect.transform.position = m_player.transform.position + m_player.transform.forward.normalized * PlayerEffectZOffset;
        }
    }

    void CreateDashEffect(GameObject e, bool isFollow, bool isPos)
    {
        GameObject Effect;
        Effect = Instantiate(e);
        if (isFollow)
        {
            
            
            //            Effect.transform.rotation = m_player.transform.rotation;
            if (isPos)
            {
                Effect.transform.parent = m_player.transform;
                Effect.transform.rotation = m_player.transform.rotation;
                Effect.transform.position = m_player.transform.position - m_player.transform.forward;
            }
            else
            {
                ParticleSystem[] Part = Effect.GetComponentsInChildren<ParticleSystem>();

                for (int i = 0; i < Part.Length; i++)
                {
                    var main = Part[i].main;
                    var startY = main.startRotationY;
                    //Part[i].startRotation3D = new Vector3(Part[i].main.startRotationX.constant,
                    //    -m_player.transform.rotation.eulerAngles.y * Mathf.Deg2Rad, Part[i].main.startRotationZ.constant);
                    startY.constantMax = -m_player.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
                    startY.constantMin = -m_player.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
                    main.startRotationY = startY;
                }
                Effect.transform.position = m_player.transform.position;
                StartCoroutine(EffectLerp(Effect));
            }
        }
        else
        {
            Effect.transform.rotation = m_player.transform.rotation;
            Effect.transform.position = m_player.transform.position;
        }
        Destroy(Effect, 1);
    }

    public IEnumerator EffectLerp(GameObject Effect)
    {
        bool _loop = true;
        float time = 0;
        while (_loop)
        {
            Effect.transform.position = Vector3.Lerp(Effect.transform.position, m_player.transform.position, 1);
            time += Time.deltaTime;
            if (time >= 0.8f) _loop = false;
            yield return null;
        }
    }

}

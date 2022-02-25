using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hook {
    public float Speed;
    public float CoolTime;
    public float Size;
    public int Count;
    public GameObject SkillObject;
    public GameObject SkillHit;
    public GameObject SkillHitK;
    public GameObject SkillCasting;
    public float ReturnTime;
    public bool DamageChack;
};

public class PlayerSkill : MonoBehaviour {
    public Hook m_Hook;
    public bool m_SkillCheck;
    public float m_WaitSkillTime;

    Animator m_Animator;
    // Use this for initialization
    void Start () {
        m_Animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!m_SkillCheck)
        {
                Skill();
        }
    }

    void Skill()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (GetComponent<PlayerManager>().GetStat().UseGaugeSkill())
            {
                m_Animator.SetTrigger("IsSkill");
                float time;
                time = m_Hook.CoolTime;
                StartCoroutine(HookStart());
                StartCoroutine(SkillCoolTime(time));
                m_SkillCheck = true;
            }
        }
    }

    IEnumerator SkillCoolTime(float time)
    {
        yield return new WaitForSeconds(time);
        m_SkillCheck = false;
        m_Hook.DamageChack = false;
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position + Vector3.up, m_Hook.Size);
    //}
    /////////////////////////////////////////////////////////////
    IEnumerator HookStart()
    {
//        Debug.Log("스킬 시작");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + Vector3.up, m_Hook.Size);
        List<Collider> Colliders = new List<Collider>();

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Enemy")
            {
                if(!hitColliders[i].GetComponent<BossEnemy>())
                    Colliders.Add(hitColliders[i]);
            }
        }
        if (Colliders.Count > 0)
        {
//            m_Animator.SetTrigger("IsSkill");
            m_SkillCheck = true;

            Colliders.Sort(delegate (Collider a, Collider b)
            {
                return (Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));
            });

            GameObject[] Obj = new GameObject[Colliders.Count];
            GameObject[] ObjCasting = new GameObject[Colliders.Count];

            for (int k = 0; k < Colliders.Count; k++)
            {
                if (m_Hook.Count - 1 < k) break;
                ObjCasting[k] = Instantiate(m_Hook.SkillCasting, transform.position + Vector3.up - (transform.position - Colliders[k].transform.position).normalized, Quaternion.Euler(0, Colliders[k].transform.eulerAngles.y+90, 0));
                Destroy(ObjCasting[k], m_WaitSkillTime);
            }

            yield return new WaitForSeconds(m_WaitSkillTime);
            for (int j = 0; j < Colliders.Count; j++)
            {
                if (m_Hook.Count - 1 < j) break;
                Obj[j] = Instantiate(m_Hook.SkillObject);
                Obj[j].transform.position = transform.position + Vector3.up;
                StartCoroutine(HookMissile(transform, Colliders[j].transform, Obj[j].transform));
                Colliders[j].GetComponent<AIController>().FootStunCoroutine();
            }
            StartCoroutine(Return());
        }
        yield return null;
    }

    IEnumerator HookMissile(Transform playerTr, Transform targetTr, Transform HookTr)
    {
        bool HitCheck = false;
        Rigidbody targetRi = targetTr.GetComponent<AIManager>().m_Rigidbody;
   //     
        while (true)
        {
            if (targetTr != null)
            {

                if (!m_Hook.DamageChack)
                {
                    HookTr.position = Vector3.Lerp(HookTr.position, targetTr.position + Vector3.up, m_Hook.Speed);
                    if(Vector3.Distance(HookTr.position, (targetTr.position + Vector3.up)) < 1f)
                    {
                        if (!HitCheck)
                        {
                            GameObject SkillHit = Instantiate(m_Hook.SkillHit, targetTr.position + Vector3.up, Quaternion.identity);
                            GameObject SkillHitK = Instantiate(m_Hook.SkillHitK, targetTr.position + Vector3.up, Quaternion.identity);
                            Destroy(SkillHit, 1);
                            Destroy(SkillHitK, 1);
                            HitCheck = true;
                        }
                    }
                }
                else
                {
                    HookTr.position = Vector3.Lerp(HookTr.position, playerTr.position, m_Hook.Speed);
                    if (Vector3.Distance(targetTr.position, playerTr.position) > 1)
                        targetRi.MovePosition(Vector3.Lerp(targetTr.position, HookTr.position, m_Hook.Speed));

                }
            }
            if (m_Hook.DamageChack)
            {
                if (Vector3.Distance(playerTr.position, HookTr.position) < 1)
                {
                    Destroy(HookTr.gameObject);
                    break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    IEnumerator Return()
    {
        yield return new WaitForSeconds(m_Hook.ReturnTime);
        m_Hook.DamageChack = true;
//        Debug.Log("리턴");
    }
    /////////////////////////////////////////////////////////////
}

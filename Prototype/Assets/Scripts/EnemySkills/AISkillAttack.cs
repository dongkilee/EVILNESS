using UnityEngine;
using System.Collections;

[System.Serializable]
public class AISkillAttack
{
    public GameObject MagicObject;    // 투척 오브젝트.
    public int SkillNumber;
    public float CoolTime;          // 쿨타임.
    public float AttackDist;         // 어택 거리.
    public int SkillAdditionDamage;     // 퍼센트 데미지.
    public int AttackCount;        // 몇번 공격할건지.
    public float AttackDelay;        // 날아가는 물체를 내보내는 딜레이.
    public float AttackStartDelay;  // 애니메이션에 맞춰서 스킬 사용.
    public bool SkillCheck = true; // 현재 쿨타임이 끝났는지.
    public bool SkillCancel = false;

    EnemySkillBowl Bowl;
    Transform Pos;
    GameObject Target;
    AIController Enemy;
    public enum AttackKinds
    {
        BASIC,
        MAGIC,
        MAGICORDER,
        MAGICSTRAIGHT,
        ONRUSH
    }
    public AttackKinds m_AttackKind;

    public void Skill()
    {
        SkillCheck = true;
        iTween.RotateTo(Enemy.gameObject, new Vector3(0, lookTarget(), 0), 0.1f);
        switch (m_AttackKind)
        {
            case AttackKinds.BASIC:
                BasicAttack();
                break;
            case AttackKinds.MAGIC:
                Enemy.StartCoroutine(Magic());
                break;
            case AttackKinds.MAGICORDER:
                Enemy.StartCoroutine(MagicOrder());
                break;
            case AttackKinds.MAGICSTRAIGHT:
                Enemy.StartCoroutine(MagicStraight());
                break;
            case AttackKinds.ONRUSH:
                Enemy.StartCoroutine(OnRush());
                break;
            default:
                break;
        }
        Enemy.StartCoroutine(CoolTimeReset());
    }

    public IEnumerator CoolTimeReset()
    {
        yield return new WaitForSeconds(CoolTime);
        //        UnityEngine.Object.Destroy(Bowl);
        SkillCheck = false;
//        Debug.Log("스킬 초기화~");
    }

    public void BasicAttack()       // 일반 공격.
    {
        if (SkillCheck)
        {
            Bowl = Enemy.gameObject.GetComponentInChildren<EnemySkillBowl>();
            Bowl.SkillAdditionDamage = SkillAdditionDamage;
            Bowl.Enemy = Enemy;
            if (Enemy is MiddleEnemy) SoundManager.Instance.RandomizeSFX(Enemy.m_Audio, SoundManager.Instance.Rat_Attack);

            
        }
    }

    public IEnumerator Magic()         // 정면으로 나가는 스킬.
    {
        if (SkillCheck)
        {
            if (AttackCount % 2 == 0)
                Pos.localRotation = Quaternion.Euler(0, (-12.5f * AttackCount / 2), 0);
            else
                Pos.localRotation = Quaternion.Euler(0, (-12.5f * (AttackCount - 1)), 0);
            
            yield return new WaitForSeconds(AttackStartDelay);
            Quaternion newRote;
            newRote = Pos.rotation;
            for (int i = 0; i < AttackCount; i++)
            {
                if (SkillCancel) break;
                GameObject newSkill = UnityEngine.Object.Instantiate(MagicObject);
                GameObject ColliderObject = newSkill.GetComponentInChildren<Collider>().gameObject;
                if (ColliderObject.GetComponent<EnemySkillBowl>() == null)
                    ColliderObject.AddComponent<EnemySkillBowl>();
                Bowl = ColliderObject.GetComponent<EnemySkillBowl>();
                Bowl.SkillAdditionDamage = SkillAdditionDamage;
                Bowl.Enemy = Enemy;

                Vector3 newPos;
                newPos = Enemy.transform.FindChild("pos").position;
                newSkill.transform.position = newPos;
                newSkill.transform.rotation = newRote;

                newRote.eulerAngles = new Vector3(0, newRote.eulerAngles.y + 25, 0);
                Object.Destroy(newSkill, 5);
                if (Enemy is BossEnemy) SoundManager.Instance.PlaySingle(newSkill.GetComponent<AudioSource>(), SoundManager.Instance.BossSkill[1]);
//                if (Enemy is MiddleEnemy) SoundManager.Instance.PlaySingle(Enemy.m_Audio, SoundManager.Instance.Hydra_Attack);
                yield return new WaitForSeconds(AttackDelay);
            }
        }
    }

    public IEnumerator MagicStraight()
    {
        if (SkillCheck)
        {
            yield return new WaitForSeconds(AttackStartDelay);
            for (int i = 0; i < AttackCount; i++)
            {
                if (SkillCancel) break;
                GameObject newSkill = Object.Instantiate(MagicObject);
                //newSkill.AddComponent<EnemySkillBowl>();
                //Bowl = newSkill.GetComponent<EnemySkillBowl>();
                GameObject ColliderObject = newSkill.GetComponentInChildren<Collider>().gameObject;
                if(ColliderObject.GetComponent<EnemySkillBowl>() == null)
                    ColliderObject.AddComponent<EnemySkillBowl>();
                Bowl = ColliderObject.GetComponent<EnemySkillBowl>();
                Bowl.SkillAdditionDamage = SkillAdditionDamage;
                Bowl.Enemy = Enemy;

                Vector3 newPos;
                Vector3 newRote;
                newPos = Enemy.transform.position + Vector3.up;
                newRote = new Vector3(0, lookTarget(), 0);
                newSkill.transform.position = newPos;
                newSkill.transform.eulerAngles = newRote;
                Object.Destroy(newSkill, 5);
                if (Enemy is BossEnemy) SoundManager.Instance.PlaySingle(newSkill.GetComponent<AudioSource>(), SoundManager.Instance.BossSkill[1]);
                yield return new WaitForSeconds(AttackDelay);
            }
        }
    }

    public IEnumerator MagicOrder()        // 타겟을 향한 스킬.
    {
        if (SkillCheck)
        {
            yield return new WaitForSeconds(AttackStartDelay);
            for (int i = 0; i < AttackCount; i++)
            {
                if (SkillCancel) break;
                GameObject newSkill = Object.Instantiate(MagicObject);
                //newSkill.AddComponent<EnemySkillBowl>();
                //Bowl = newSkill.GetComponent<EnemySkillBowl>();
                GameObject ColliderObject = newSkill.GetComponentInChildren<Collider>().gameObject;
                if (ColliderObject.GetComponent<EnemySkillBowl>() == null)
                    ColliderObject.AddComponent<EnemySkillBowl>();
                Bowl = ColliderObject.GetComponent<EnemySkillBowl>();
                Bowl.SkillAdditionDamage = SkillAdditionDamage;
                Bowl.Enemy = Enemy;

                Vector3 newPos;
                newPos = Target.transform.position;
                newPos.y = 0;
                newSkill.transform.position = newPos;
                Object.Destroy(newSkill, 5);
                yield return new WaitForSeconds(AttackDelay);
            }
        }
    }

    public IEnumerator OnRush()
    {
        if (SkillCheck)
        {
            yield return new WaitForSeconds(AttackStartDelay);

            Bowl = Enemy.transform.FindChild("Rush").GetComponent<EnemySkillBowl>();    // 러쉬 게임오브젝트
            Bowl.GetComponent<Collider>().enabled = true;
            Bowl.SkillAdditionDamage = SkillAdditionDamage;
            Bowl.Enemy = Enemy;

            iTween.RotateTo(Enemy.gameObject, new Vector3(0, lookTarget(), 0), 0f);

            Vector3 newPos;
            newPos = Target.transform.position;
            Vector3 newForwordPos = Vector3.zero;
            float Speed = Enemy.m_TraceSpeed * 2;

            Enemy.m_OnRush = true;

            while (Enemy.m_OnRush)
            {
                if (Vector3.Distance(Enemy.transform.position, newPos) > 1)
                {
                    Enemy.m_EnemyMove.m_Rigidbody.MovePosition(Vector3.Lerp(Enemy.transform.position, newPos, Time.deltaTime * Speed));
                }else
                {
                    newForwordPos = Enemy.transform.position + (Enemy.transform.forward * 3);

                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            while (Enemy.m_OnRush)
            {
                if (Speed <= 1) break;
                Enemy.m_EnemyMove.m_Rigidbody.MovePosition(Vector3.Lerp(Enemy.transform.position, newForwordPos, Time.deltaTime * Speed));
                Speed -= 0.1f;
                yield return new WaitForEndOfFrame();
            }
            Enemy.WaitForAnimation();

        }
    }

    float lookTarget()      // LookAt함수.
    {
        Quaternion lookAt = Quaternion.identity;    // Querternion 함수 선언.
        Vector3 lookatVec = (Target.transform.position - Enemy.transform.position).normalized; //타겟 위치 - 자신 위치 -> 노멀라이즈.
        lookAt.SetLookRotation(lookatVec);  // 쿼터니언의 SetLookRotaion 함수 적용.

        return lookAt.eulerAngles.y;
    }

    public void SetEnemy(AIController sauce)
    {
        Enemy = sauce;
        Pos = Enemy.transform.FindChild("pos");

    }

    public void SetTarget(GameObject target)
    {
        Target = target;

    }
        

}

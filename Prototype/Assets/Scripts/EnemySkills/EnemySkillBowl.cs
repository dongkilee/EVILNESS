using UnityEngine;
using System.Collections;

public class EnemySkillBowl : MonoBehaviour
{
    public int SkillAdditionDamage;     // 퍼센트 데미지.
    public AIController Enemy;
    public GameObject AttackHitEffect;

    public void PlayerAttackHit(Transform target)
    {
        if(AttackHitEffect != null)
        {
            EffectManager.Instance.EnemyPlayerHit(target, AttackHitEffect);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(gameObject.name == "Rush")
        {
            if (other.CompareTag("Player"))
            {
                Enemy.m_OnRush = false;
                Enemy.WaitForAnimation();
                GetComponent<Collider>().enabled = false;
            }
            else if (other.CompareTag("Maps"))
            {
                Enemy.GroggyStunCoroutine();
                GetComponent<Collider>().enabled = false;
            }
        }
    }

}

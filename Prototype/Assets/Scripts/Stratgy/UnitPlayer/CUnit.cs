using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class CUnit
{

    public float m_HP;           // 매니저에서 받아올 HP.
    public float m_HPMax;      // 매니저에서 받아올 최대HP.
    public float m_DamageMin;               // 플레이어의 기본 데미지 (무기 X).
    public float m_DamageMax;              // 기본 데미지 맥스.
    public Image m_HPBar;                   // 받아온 HPUI게임오브젝트에서 HPUIBar를 찾아서 넣기.

    protected CWeapon m_Weapon;           // 사용중인 무기.
    protected float m_UnitDamageMin;            // 혹시 변동될 수도 있으니깐.
    protected float m_UnitDamageMax;
    protected float m_TempUnitDamageMax;        // 기본데미지 저장.
    protected float m_TempUnitDamageMin;        // 기본데미지 저장.
    protected float m_PropertyDamage;

    public abstract void HPdisplay();        // 플레이어의 HP상태를 나타낸다.
    public abstract void Init();            // 최초 초기화.
    public void FromWeapon() { m_Weapon.WeaponDisplay(); }       // 현재 무기가 어떤것인지 알려준다.

    public float OnAttack()
    {               // 평타 데미지 계산.
        float ResultDamage = (int)Random.Range(m_UnitDamageMin, m_UnitDamageMax + 1);
        return ResultDamage;
    }
    public float DurationOnAttack()
    {       // 지속 데미지 계산.
        float ResultDamage = 0;

        return ResultDamage;
    }

    public void FromOnDamage(float damage)      // 받은 데미지 정산.
    {
//        Debug.Log("현재 증폭데미지" + m_PropertyDamage);
//        damage = Mathf.Round(damage * m_PropertyDamage / 100);     // 데미지 = 데미지 * 퍼센트데미지 / 100.
//        Debug.Log("적이 받은 데미지" + damage);
        Health -= damage;         // 데미지를 받아서 HP를 깍는다.
//        if (!(this is CMan))
//            GameManager.Instance.StartCreateDamageMeter(damage);

        //        Debug.Log(name + " : 남은 HP : " + Health + "받은 데미지 : " + damage);
        HPdisplay();                // 체력UI를 변경해준다.
    }

    public void setWeapon(CWeapon weapon)
    {
        m_Weapon = weapon;
        m_TempUnitDamageMin = m_UnitDamageMin = m_DamageMin + m_Weapon.W_DamageMin;
        m_TempUnitDamageMax = m_UnitDamageMax = m_DamageMax + m_Weapon.W_DamageMax;
    }         // 플레이어의 무기를 설정한다.

    public float Health       // HP.
    {
        get { return m_HP; }
        set { m_HP = value; }
    }
    public float HealthMax        // MaxHP.
    {
        get { return m_HPMax; }
        set { m_HPMax = value; }

    }

    public float PropertyDamage
    {
        get { return m_PropertyDamage; }
        set { m_PropertyDamage = value; }
    }
}

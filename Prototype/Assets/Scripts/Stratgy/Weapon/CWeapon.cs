using UnityEngine;
using System.Collections;

public abstract class CWeapon
{
    protected int m_WeaponID;
    protected float m_DamageMin;  // 무기 디폴트 데미지.
    protected float m_DamageMax;  // 무기 디폴트 데미지.
    public CWeapon() { m_DamageMin = m_DamageMax = 0; }      // 무기 데미지 초기화.
    public abstract void WeaponDisplay();      // 어떤 무기인지 보여준다.
    public abstract void AttackDisplay();        // 공격을 한다. (예상 공격 UI를 변경시킨다.)

    static public int m_WeaponNumber;

    public float W_DamageMin
    {
        get { return m_DamageMin; }
        set { m_DamageMin = value; }
    }

    public float W_DamageMax
    {
        get { return m_DamageMax; }
        set { m_DamageMax = value; }
    }

    public int WeaponID
    {
        get { return m_WeaponID; }
    }
}

using UnityEngine;
using System.Collections;

public class CW_Default : CWeapon
{

    public CW_Default()
    {
        m_WeaponID = -1;
        m_DamageMin = m_DamageMax = 0;
    }

    public override void WeaponDisplay()
    {
        //        Debug.Log("맨손");
    }

    public override void AttackDisplay()
    {
        //        Debug.Log("주먹 어택");
    }
}

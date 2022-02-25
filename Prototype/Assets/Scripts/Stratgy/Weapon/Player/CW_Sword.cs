using UnityEngine;
using System.Collections;
using System;

public class CW_Sword : CWeapon
{
    public CW_Sword()
    {
        m_WeaponID = 1;
        m_DamageMin = 20;
        m_DamageMax = 30;
    }

    public override void WeaponDisplay()
    {
        //       Debug.Log("검");

    }

    public override void AttackDisplay()
    {
        //        Debug.Log("소드 어택");
    }
}

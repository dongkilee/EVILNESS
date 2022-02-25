using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CW_Hammer : CWeapon {

    public CW_Hammer()
    {
        m_WeaponID = 2;
        m_DamageMin = 100;
        m_DamageMax = 200;
    }


    public override void WeaponDisplay()
    {
        // Debug.Log("해머");
    }

    public override void AttackDisplay()
    {
        //        Debug.Log("해머 어택");
    }
}

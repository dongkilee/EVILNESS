using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class CEnemy : CUnit
{

    public override void Init()
    {
//        Debug.Log("몬스터");
        HPdisplay();
    }

    public override void HPdisplay()
    {
//        Debug.Log("현재 HP : " + Health);
        if (m_HPBar != null)
            m_HPBar.fillAmount = (float)Health / (float)HealthMax;

    }
}

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[Serializable]
public class CMan : CUnit
{
    public float m_Skill_Use_Gauge;     // 스킬을 사용하는 게이지의 수
    public int m_Combo;     // 매니저에서 받아올 콤보 증가 수.
    public GameObject m_Combo_UI;     // 콤보 UI를 가져온다.
    public float m_GaugeMax;    // 스킬 게이지 최대치
    public float m_Gauge;       // 현재 게이지의 수치
    public float AnimationSpeed;

    private float m_ComboUITime = 3;
    private float m_ComboUITimeMax = 3;

    public override void Init()
    {
//        Debug.Log("기본 플레이어");
        m_Combo = 0;
        m_Gauge = 0;
        m_Combo_UI.SetActive(false);
        HPdisplay();
        Gaugedisplay();
    }

    public override void HPdisplay()
    {
//        Debug.Log("현재 HP : " + Health);
        if (m_HPBar != null)
            m_HPBar.fillAmount = (float)Health / (float)HealthMax;

        if(m_HPBar.fillAmount <= 0.3f)
        {
            UIManager.Instance.DangerView();
        }
    }

    // *************** 플레이어용 **********************.
    public void Combodisplay()    // 콤보 화인.
    {
        Gaugedisplay();
        // UI를 True하고나서;
        ++m_Combo;                // 콤보 증가.
        
        m_ComboUITime = m_ComboUITimeMax;       // 시간 초기화.
        UIManager.Instance.ComboUIView(m_Combo);
        
        //    Feverdisplay();
        //    m_Fever_UI.GetComponent<Image>().fillAmount = m_Fever / m_FeverMax;
    }

    public void Gaugedisplay()          // 게이지 보여주게 하기
    {
        if(m_Gauge < m_GaugeMax)
            ++m_Gauge;
        UIManager.Instance.m_Gauge.fillAmount = m_Gauge / m_GaugeMax;
        UIManager.Instance.m_GaugeEffectUI.GetComponent<RectTransform>().localScale =
            new Vector3(m_Gauge / m_GaugeMax + (m_Gauge / m_GaugeMax) / 2, 1, 1.5f);
        UIManager.Instance.m_GaugeEffectUI.GetComponent<RectTransform>().anchoredPosition3D =
            new Vector3(-75 + (m_Gauge / m_GaugeMax + (m_Gauge / m_GaugeMax)/2)*60, -15, 0);
    }

    public bool UseGaugeSkill()     // 게이지 만큼 스킬 사용하기
    {
        if(m_Gauge >= m_Skill_Use_Gauge)
        {
            m_Gauge -= m_Skill_Use_Gauge;
            UIManager.Instance.m_Gauge.fillAmount = m_Gauge / m_GaugeMax;
            return true;
        }
        return false;
    }

    public void CancelCombo()              // 콤보 캔슬.
    {
        UIManager.Instance.ComboUIClose();
        m_Combo = 0;
    }
    public void TimeCancelCombo()           // 콤보가 캔슬 되는 시간.
    {       // 콤보 테스트.
        m_ComboUITime -= Time.deltaTime;
        if (m_ComboUITime <= 0) CancelCombo();
    }

    // *************** 플레이어용 **********************.
}

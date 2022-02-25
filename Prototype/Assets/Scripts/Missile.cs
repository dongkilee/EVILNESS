using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{

    public float m_Speed;
    public GameObject m_EffectBoom;
    public GameObject m_ObjectEffectBoom;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (m_EffectBoom == null) return;
            GameObject newEffectBoom = Instantiate(m_EffectBoom);
            Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
            newEffectBoom.transform.position = pos;
            SoundManager.Instance.PlaySingle(newEffectBoom.GetComponent<AudioSource>(), SoundManager.Instance.BossMisilleBoom);
            Destroy(newEffectBoom, 2);
            Destroy(gameObject);
        }
        else {
            if (m_ObjectEffectBoom == null) return;
            GameObject newEffectBoom = Instantiate(m_ObjectEffectBoom);
            Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            newEffectBoom.transform.position = pos;
            Destroy(newEffectBoom, 2);
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 게임매니저에서 컨트롤 할거임

[System.Serializable]
public class EnemyStat
{
    public GameObject Enemy;            // 몬스터 프리팹
    public Vector3[] EnemyPostion;      // 생성 시킬 몬스터 포지션
    public int EnemyCount { get; set; } // 몬스터 카운트

    public void Count()
    {
        EnemyCount = EnemyPostion.Length;
    }

};

//[ExecuteInEditMode]
public class EnemySpawn : MonoBehaviour
{
    public GameObject PrevStage;        // 이전 스테이지 정보
    public GameObject NextStage;        // 다음 스테이지 정보
    public EnemyStat[] m_Enemy;         // 몬스터 생성 정보
    public EnemyStat[] m_Enemy_2;
    public List<GameObject> m_AllEnemy;     // 현재 내가 가지고 있는 총 몬스터
    public GameObject[] WayPoint;
    int WayCount;

    public Renderer[] Body;
    Material[] m;
    float[] hide;

    void Awake()
    {
        m_AllEnemy = new List<GameObject>();
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < m_Enemy.Length; i++)
        {
            m_Enemy[i].Count();
            StartCoroutine(Spawner(m_Enemy[i]));
        }

        WayCount = WayPoint.Length;
        Body = NextStage.GetComponentsInChildren<MeshRenderer>();
        m = new Material[Body.Length];
        hide = new float[Body.Length];
        for (int i = 0; i < Body.Length; i++)
        {
            m[i] = Body[i].material;
            hide[i] = m[i].GetFloat("_Hide");
        }
            
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Spawner(EnemyStat enemy)
    {
        for (int i = 0; i < enemy.EnemyCount; i++)
        {
            GameObject newEnemy;
            newEnemy = Instantiate(enemy.Enemy);
            
            newEnemy.transform.parent = transform; 
            newEnemy.transform.position = enemy.EnemyPostion[i];
            m_AllEnemy.Add(newEnemy);
            //            newEnemy.GetComponent<AIController>().m_WayPoint = new GameObject[WayCount];
            AIController newAIEnemy = newEnemy.GetComponent<AIController>();
            newAIEnemy.m_WayPoint = WayPoint;
            
            if (PrevStage.activeInHierarchy)
                newEnemy.SetActive(false);
            else 
                ObstacleManager.Obstacles.Add(newEnemy.GetComponent<Obstacle>());
        }

        yield return null;
    }

    public void AllEnemyTureActive()                // 구간 통과시 몬스터 활성화
    {
//        if (NextStage.name == "Stage0") return;
        for (int i = 0; i < m_AllEnemy.Count; i++)
        {
            m_AllEnemy[i].SetActive(true);
            ObstacleManager.Obstacles.Add(m_AllEnemy[i].GetComponent<Obstacle>());
        }
    }

    public void AllEnemyTureActiveSkillStage()                // 스킬 몬스터 활성화
    {
        for (int i = 0; i < m_AllEnemy.Count; i++)
        {
            m_AllEnemy[i].SetActive(true);
            ObstacleManager.Obstacles.Add(m_AllEnemy[i].GetComponent<Obstacle>());
        }
    }

    public void OnEnemyCreate()
    {
        if (m_Enemy_2 == null) return;
        for (int i = 0; i < m_Enemy_2.Length; i++)
        {
            m_Enemy_2[i].Count();
            StartCoroutine(Spawner(m_Enemy_2[i]));
        }
    }

    public void OnTriggerEnemySpawn()
    {
        AllEnemyTureActive();
    }

    public IEnumerator ShaderDie()
    {
        while (hide[Body.Length-1] >= -1)
        {
            for (int i = 0; i < Body.Length; i++)
            {
                hide[i] -= 0.004f;
                m[i].SetFloat("_Hide", hide[i]);
            }
            yield return new WaitForEndOfFrame();
        }


        NextStage.SetActive(false);
        yield return null;
    }
}

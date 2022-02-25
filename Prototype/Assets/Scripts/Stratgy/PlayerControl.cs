using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerManager))]
public class PlayerControl : MonoBehaviour
{

    private PlayerManager m_Character;
    private Transform m_Cam;
    private Vector3 m_CamForward;
    private Vector3 m_Move;
    private bool m_Jump;
    private bool m_Attack;
    private bool m_Dash;
    private bool DashCoolCheck;
    public float m_DashCoolTime = 1;

    private void Awake()
    {
        
    }

    private void Start()
    {
        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform.root;
        }

        m_Character = GetComponent<PlayerManager>();
        //Transform[] AllData = GetComponentsInChildren<Transform>();

        //int cnt = 0;
        //foreach (Transform Obj in AllData)
        //{
        //    if (Obj.tag == "Weapon")
        //    {
        //        m_WeaponCollider[cnt] = Obj.GetComponent<Collider>();
        //        cnt++;
        //    }
        //}
    }


    private void Update()
    {

        if (UIManager.Instance.FadeInOut.GetComponent<CanvasGroup>().alpha != 0) return;
        if (GameManager.Instance.BossCheck) return;
        if (GameManager.Instance.isGameOver) return;
        if (CameraManager.Instance.DoorEventCheck) return;
        if (UIManager.Instance.Pause) return;
        

        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(SceneLoadManager.Instance.BossScene());
        }


        //if (!m_Jump)
        //{
        //    m_Jump = Input.GetButtonDown("Jump");
        //}


        if (!m_Dash && !DashCoolCheck)
        {
            //            if (!m_Character.m_Animator.GetCurrentAnimatorStateInfo(m_Character.GetWeapon().WeaponID).IsTag("Attack"))
            {
                m_Dash = Input.GetKeyDown(KeyCode.LeftShift);
                if (m_Dash)
                {
                    StartCoroutine(DashCoolTime());
                    if (Input.GetKey(KeyCode.D))
                    {
                        iTween.RotateTo(gameObject, new Vector3(0, Camera.main.transform.root.eulerAngles.y + 90, 0), 0);
                    }
                    else if (Input.GetKey(KeyCode.A))
                    {
                        iTween.RotateTo(gameObject, new Vector3(0, Camera.main.transform.root.eulerAngles.y - 90, 0), 0);
                    }
                    else if (Input.GetKey(KeyCode.W))
                    {
                        iTween.RotateTo(gameObject, new Vector3(0, Camera.main.transform.root.eulerAngles.y, 0), 0);
                    }
                    else if (Input.GetKey(KeyCode.S))
                    {
                        iTween.RotateTo(gameObject, new Vector3(0, Camera.main.transform.root.eulerAngles.y + 180, 0), 0);
                    }

                }
            }
        }

    }


    private void FixedUpdate()
    {
        if (UIManager.Instance.FadeInOut.GetComponent<CanvasGroup>().alpha != 0) return;
        if (GameManager.Instance.BossCheck)
        {
            m_Character.Move(Vector3.zero, false, false, false);
            return;
        }
        if (GameManager.Instance.isGameOver) return;

        if (CameraManager.Instance.DoorEventCheck)
        {
            m_Character.Move(Vector3.zero, false, false, false);
            return;
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool crouch = Input.GetKey(KeyCode.C);
        
        if (m_Cam != null)
        {
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v * m_CamForward + h * m_Cam.right;
        }
        else
        {
            m_Move = v * Vector3.forward + h * Vector3.right;
        }
        if (!m_Attack)
        {
            m_Attack = Input.GetButtonDown("Fire1");
            if(m_Attack) m_Move = Vector3.zero;

        }
        if (m_Dash) m_Move = Vector3.zero;
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{ //m_Move *= 0.5f;
        //    m_Character.m_Animator.SetTrigger("IsDash");
        //}

        m_Character.Move(m_Move, false, m_Attack, m_Dash);
        //    m_Character.Move(Vector3.zero, m_Jump, m_Attack, m_Dash);
        m_Jump = false;
        m_Dash = false;
//        if (!Input.GetButtonDown("Fire1"))
            m_Attack = false;
    }

    public void OnAttackCheck() {
        m_Character.m_WeaponCollider.enabled = true;
        m_Character.m_HammerCollider.enabled = true;
    }
    public void CloseAttackCheck() {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        m_Character.m_WeaponCollider.enabled = false;
        m_Character.m_HammerCollider.enabled = false;
    }

    public IEnumerator DashCoolTime()
    {
        DashCoolCheck = true;
        yield return new WaitForSeconds(m_DashCoolTime);
        DashCoolCheck = false;
    }
}

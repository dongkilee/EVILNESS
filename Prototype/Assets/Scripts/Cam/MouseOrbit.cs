using UnityEngine;
using System.Collections;

public class MouseOrbit : MonoBehaviour
{

    public Transform target;

    public float distance = 3.0f;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20;
    public float yMaxLimit = 80;
    public float LerpSpeed = 5;
    public float LerpMoveSpeed = 1;
    
    public float ZoomInMaxTime;
    public float ZoomInTime;

    public float Responsiveness = 0.01f;    // 마우스 민감도

    float x = 0;
    float y = 0;
    float LerpDistance;
    Transform Pivot;                // 맵이 있을 경우 앞으로 땡기는 얘
    Transform Cam;


    /// ////////////////////////////////////////////////////////////////////
    private bool Shaking;                      // 쉐이크 체크.
    private float ShakeDecay;                 // 
    private float ShakeIntensity;
    private Vector3 OriginalPos;
    private Quaternion OriginalRot;
    /// ////////////////////////////////////////////////////////////////////

    // Use this for initialization
    void Start()
    {
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

        Pivot = transform.FindChild("Pivot");
        Cam = transform.FindChild("Pivot").FindChild("posPivot").FindChild("Cam");
        Shaking = false;
    }

    // Update is called once per frame
    void Update()
    {
        ////////////////////////////////////////////////////////////////////////
        RaycastHit hit;
        Transform CameraTr = Cam;
//        Debug.DrawRay(target.position, CameraTr.position - target.position, Color.blue);
        if (Physics.Raycast(target.position, CameraTr.position - target.position, out hit, distance))
        {
            if (hit.transform.CompareTag("Maps") || hit.transform.CompareTag("Ground"))
            {
                //                Pivot.position = Vector3.Lerp(Pivot.position, hit.point, 1f);
                CameraTr.position = new Vector3(Mathf.Lerp(CameraTr.position.x, hit.point.x, Time.deltaTime * LerpSpeed),
                    Mathf.Lerp(CameraTr.position.y, hit.point.y, Time.deltaTime * LerpSpeed),
                    Mathf.Lerp(CameraTr.position.z, hit.point.z, Time.deltaTime * LerpSpeed));
            }
        }
        else
        {
            CameraTr.localPosition = new Vector3(Mathf.Lerp(CameraTr.localPosition.x, 0, Time.deltaTime * LerpSpeed),
                    Mathf.Lerp(CameraTr.localPosition.y, 0, Time.deltaTime * LerpSpeed),
                    Mathf.Lerp(CameraTr.localPosition.z, 0, Time.deltaTime * LerpSpeed));
            //           Pivot.localPosition = Vector3.zero;
        }

        ////////////////////////////////////////////////////////////////////////

        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            if (Responsiveness > 0.001f)
            {
                Responsiveness -= 0.001f;
                StartCoroutine(GameManager.Instance.CreateDamageMeter1("마우스 민감도 : " + Mathf.Round(Responsiveness * 1000)));

            }
            else
            {
                StartCoroutine(GameManager.Instance.CreateDamageMeter1("마우스 민감도는 1이 최소치입니다."));
            }
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            if (Responsiveness < 0.02f)
            {
                Responsiveness += 0.001f;
                StartCoroutine(GameManager.Instance.CreateDamageMeter1("마우스 민감도 : " + Mathf.Round(Responsiveness * 1000)));
            }
            else
            {
                StartCoroutine(GameManager.Instance.CreateDamageMeter1("마우스 민감도는 20이 최대치입니다."));
            }
        }

    }

    void FixedUpdate()
    {
        
        if (target)
        {
            
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    //Mouse wheel down
                    if (distance < 4f)
                        distance += 0.1f;
                    else
                        distance = 4f;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    //Mouse wheel up
                    if (distance > 2f)
                        distance -= 0.1f;
                    else
                        distance = 2f;
                }
            if (UIManager.Instance.FadeInOut.GetComponent<CanvasGroup>().alpha == 0)
            {
            
                if (!CameraManager.Instance.DoorEventCheck)
                {
                    x += Input.GetAxis("Mouse X") * xSpeed * Responsiveness;
                    y -= Input.GetAxis("Mouse Y") * ySpeed * Responsiveness;

                }
            }
            y = ClampAngle(y, yMinLimit, yMaxLimit);
            
            var rotation = Quaternion.Euler(y, x, 0);
            transform.rotation = rotation;

            Pivot.localPosition = new Vector3(0, 0, -distance);
            //            LerpDistance += LerpMoveSpeed;


            //            var position = rotation * target.position;
            var FinalPostion = Vector3.Lerp(transform.position, target.position, Time.deltaTime * LerpMoveSpeed);
//            LerpDistance = 0;
            if (GameManager.Instance.ZoomInCheck &&
                CameraManager.Instance.DoorEventCheck)
            {
//                Time.timeScale = 0.4f;
                ZoomInTime += Time.deltaTime;
                FinalPostion += ((transform.forward*0.8f) * ZoomInTime);
                if (ZoomInTime > ZoomInMaxTime)
                {
                    GameManager.Instance.ZoomInCheck = false;
                    ZoomInTime = 0;
//                    Time.timeScale = 1f;
                }
            }
            OriginalPos = transform.position = FinalPostion;
            /// ////////////////////////////////////////////////////////////////////
            if (ShakeIntensity > 0)
            {
                transform.position = OriginalPos + Random.insideUnitSphere * ShakeIntensity;
                transform.rotation = new Quaternion(
                     OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity) * .1f,
                     OriginalRot.y + Random.Range(-ShakeIntensity, ShakeIntensity) * .1f,
                     OriginalRot.z + Random.Range(-ShakeIntensity, ShakeIntensity) * .1f,
                     OriginalRot.w + Random.Range(-ShakeIntensity, ShakeIntensity) * .1f);
                ShakeIntensity -= ShakeDecay;
//                transform.LookAt(target);
            }
            else if (Shaking)
            {
                Shaking = false;
            }
            /// ////////////////////////////////////////////////////////////////////
//            transform.LookAt(target);
        }
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle <= -360)
            angle += 360;
        if (angle >= 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public void DoShake(float intensity, float decay)
    {
        //        OriginalPos = transform.position;

        OriginalRot = transform.rotation;
        ShakeIntensity = intensity;     // 0.1f
        ShakeDecay = decay;           // 0.001f
        Shaking = true;
    }

    public void SetCameraPoint(float _x, float _y)
    {
        x = _x; y = _y;
    }
}

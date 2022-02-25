using UnityEngine;
using System.Collections;

public class CameraManager : Singleton<CameraManager> {

    public GameObject MainCamera;
    public GameObject BossMainCamera;
    public GameObject DoorCamera;

    public float DoorMove1Speed;
    public float DoorMove2Speed;

    public bool DoorEventCheck = false;
    public GameObject TestCamera;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    if (!TestCamera.activeInHierarchy)
        //    {
        //        MainCamera.SetActive(false);
        //        TestCamera.SetActive(true);
        //    }else
        //    {
        //        MainCamera.SetActive(true);
        //        TestCamera.SetActive(false);
        //    }
        //}
	}

    public void BossCameraChange()
    {
        DoorCamera.SetActive(false);
        MainCamera.SetActive(false);
        BossMainCamera.SetActive(true);
    }

    public void DoorCameraChange()
    {
        MainCamera.SetActive(false);
        DoorCamera.SetActive(true);
    }

    public void MainCameraChange()
    {
        DoorEventCheck = false;
        DoorCamera.SetActive(false);
        MainCamera.SetActive(true);
    }

    public IEnumerator DoorMove(EnemySpawn enenmyspawner)
    {
        Transform Pos = enenmyspawner.NextStage.transform.FindChild("pos");
        Transform Move1 = enenmyspawner.NextStage.transform.transform.FindChild("Move1");
        Transform Move2 = enenmyspawner.NextStage.transform.transform.FindChild("Move2");

        Vector3 MovePos1 = Move1.position;
        Vector3 MovePos2 = Move2.position;
//        bool check = false;
        
        DoorCamera.transform.position = Pos.position;
        DoorCamera.transform.LookAt(enenmyspawner.NextStage.transform.FindChild("Pivot"));
        StartCoroutine(enenmyspawner.ShaderDie());
        while (true)
        {
//            enenmyspawner.NextStage.GetComponent<Animator>().SetInteger("IsThron", (int)GameManager.Instance.GameStage);
            //            enenmyspawner.NextStage.GetComponent<Animator>().SetBool("IsThron", true);
            //            enenmyspawner.NextStage.transform.Translate(Vector3.up * Time.deltaTime);
            DoorCamera.transform.position = Vector3.Lerp(DoorCamera.transform.position, MovePos1, Time.deltaTime * DoorMove1Speed);
            if (Vector3.Distance(DoorCamera.transform.position, MovePos1) < 1f) break;
            //            if (!check)
            //            {
            ////                DoorCamera.transform.position = Vector3.Lerp(DoorCamera.transform.position, MovePos1, Time.deltaTime * DoorMove1Speed);
            //                if (Vector3.Distance(DoorCamera.transform.position, MovePos1) < 1f) check = true;
            //            }
            //            else
            //            {
            ////                DoorCamera.transform.position = Vector3.Lerp(DoorCamera.transform.position, MovePos2, Time.deltaTime * DoorMove2Speed);
            //                //                DoorCamera.transform.position = Vector3.Lerp(DoorCamera.transform.position, Move2.position, DoorMove2Speed * Time.deltaTime);
            //                if (Vector3.Distance(DoorCamera.transform.position, MovePos2) < 1f) break;
            //            }

            yield return new WaitForEndOfFrame();
        }
        enenmyspawner.NextStage.GetComponent<Collider>().isTrigger = true;
//        enenmyspawner.NextStage.SetActive(false);
//        yield return new WaitForSeconds(1);
        MainCameraChange();
        yield return null;
    }


}

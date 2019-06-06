using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    private BezierWalker walkerComp;

    

    public GameObject cam;

    private bool isConnect = false;
    float progess;
    private float speed = 0f;
    private bool byPass = false;
    private bool restart = false;

    void Start()
    {
        print("Born");
        cam.SetActive(false);
        animator = GetComponentInChildren(typeof(Animator), true) as Animator;
        walkerComp = GetComponent<BezierWalker>();
        progess = 0f;
        isConnect = false;
        //DontDestroyOnLoad(this.gameObject);
        if(isLocalPlayer)
        {
          
            GetComponent<BezierWalker>().enabled = true;
            GetComponent<BezierWalker>().SetSlineID(int.Parse(this.netId.ToString()) - 1);
            print(GameObject.Find("BLE"));
            SetConection(true);
            if (GameObject.Find("BLE"))
            {
                GameObject.Find("BLE").GetComponent<ArduinoHM10Test>().player = this;
                if (GameObject.Find("BLE").GetComponent<ArduinoHM10Test>().byPass)
                {
                    byPass = true;
                }


            }
           

        }


    }

    // Update is called once per frame
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetConection(bool param)
    {
        Debug.Log("Param:" + param);
        isConnect = param;
        if(param)
        {
            
            cam.SetActive(true);
        }
    }


    public bool GetConnectionFlag()
    {
        return isConnect;
    }

    IEnumerator HostStart(int time)
    {
        yield return new WaitForSeconds(time);
        this.speed = 0.75f;
    }

    IEnumerator Restart(int time)
    {
        restart = true;
        yield return new WaitForSeconds(time);
        this.progess = 0;
        restart = false;

    }
    void Update()
    {
        print(NetworkClient.allClients.Count);
        //print(NetworkServer.connections.Count);
        if(NetworkServer.connections.Count > 1 && byPass)
        {
            StartCoroutine(HostStart(5));
        }

        if(NetworkServer.connections.Count == 0  && NetworkClient.allClients.Count == 1 && byPass)
        {
            StartCoroutine(HostStart(0));
        }
        //this.speed = 3f;
        // if(Input.GetKey("space")){
        //     this.SetSpeed(0.75f);
        //     print("INN");
        // }
        // else {
        //     this.SetSpeed(0f);
        // }
        //if(isLocalPlayer && !cam.active)
        //{
        //    cam.SetActive(true);
        //}
        this.SetSpeed(this.speed);



        Debug.Log("Speed: " + this.speed);
        animator.speed = this.speed;
        //this.transform.position += new Vector3(0,0, 0.1f)
        this.progess += 0.02319f * this.speed * Time.deltaTime;
        if(this.progess >= 1 && !restart)
        {
            StartCoroutine(Restart(41));
        }
        walkerComp.SetProgress(this.progess);
        //walkerComp.SetProgress(Mathf.Lerp(walkerComp.GetProgress(), this.progess, Time.deltaTime));
    }
}

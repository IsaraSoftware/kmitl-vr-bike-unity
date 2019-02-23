using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    private BezierWalker walkerComp;

    public GameObject cam;

    private bool isConnect = false;
    float progess;
    private float speed = 0f;
    void Start()
    {
        cam.SetActive(false);
        animator = GetComponentInChildren(typeof(Animator), true) as Animator;
        walkerComp = GetComponent<BezierWalker>();
        progess = 0f;
        isConnect = false;

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
    void Update()
    {
        // if(Input.GetKey("space")){
        //     this.SetSpeed(1f);
        //     print("INN");
        // }
        // else {
        //     this.SetSpeed(0f);
        // }
        if(SceneManager.GetActiveScene().name == "ArduinoHM10Test" && gameObject.GetComponent<ArduinoHM10Test>().enabled == false)
        {
            Debug.Log("Create BLE");
            gameObject.GetComponent<ArduinoHM10Test>().enabled = true;
        }
        Debug.Log("Speed: " + this.speed);
        animator.speed = this.speed;
        //this.transform.position += new Vector3(0,0, 0.1f)
        this.progess += 0.02319f * this.speed * Time.deltaTime;
        walkerComp.SetProgress(this.progess);
        //walkerComp.SetProgress(Mathf.Lerp(walkerComp.GetProgress(), this.progess, Time.deltaTime));
    }
}

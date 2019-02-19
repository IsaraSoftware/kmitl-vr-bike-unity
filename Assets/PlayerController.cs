using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    private BezierWalker walkerComp;
    float progess;
    private float speed = 0f;
    void Start()
    {
        animator = GetComponentInChildren(typeof(Animator), true) as Animator;
        walkerComp = GetComponent<BezierWalker>();
        progess = 0f;

    }

    // Update is called once per frame
    public void SetSpeed(float speed)
    {
        this.speed = speed;
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
        Debug.Log("Speed: " + this.speed);
        animator.speed = this.speed;
        //this.transform.position += new Vector3(0,0, 0.1f)
        this.progess += 0.02319f * this.speed * Time.deltaTime;
        walkerComp.SetProgress(this.progess);
        //walkerComp.SetProgress(Mathf.Lerp(walkerComp.GetProgress(), this.progess, Time.deltaTime));
    }
}

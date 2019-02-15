using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    private BezierWalker walkerComp;
    float progess;
    void Start()
    {
        animator = GetComponentInChildren(typeof(Animator), true) as Animator;
        walkerComp = GetComponent<BezierWalker>();
        progess = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        animator.speed = 3f;
        //this.transform.position += new Vector3(0,0, 0.1f)
        this.progess += 0.02319f * Time.deltaTime;
        walkerComp.SetProgress(this.progess);
        //walkerComp.SetProgress(Mathf.Lerp(walkerComp.GetProgress(), this.progess, Time.deltaTime));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iktutorial : MonoBehaviour
{
    Animator anim;
    public float ikWeight =1;
    public Transform leftIkTarget;
    public Transform rightIktarget;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        Debug.Log(transform.forward);
    }
    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, ikWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, ikWeight);

        anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftIkTarget.position);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rightIktarget.position);
    }
}

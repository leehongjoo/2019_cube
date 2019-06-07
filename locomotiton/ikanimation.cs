using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ikanimation : MonoBehaviour
{
    Animator anim;
    IKSnapshot ikBase;
    IKSnapshot current = new IKSnapshot();
    IKSnapshot next = new IKSnapshot();
    IKGoals goals = new IKGoals();

    public float w_rh, w_lh, w_rf, w_lf;
    Vector3 rh, lh, rf, lf;
    Vector3 premovedir;
    Transform h;   //helper transform
    bool isMirror;
    bool isLeft; // delete
    public float lerpSpeed = 1f;
    public float wallOffset = 0.1f;
    float delta;
    public void Init(climb c, Transform helper)
    {
        anim = c.anim;
        ikBase = c.baseIKSnapshot;
        h = helper;
    }
    public void CreatePosition(Vector3 origin, Vector3 moveDir, bool isMid) // origin = helper위치 ismid = 애니메이션 idle,up
    {
        delta = Time.deltaTime;
        HandlerAnimation(moveDir, isMid);
        if(!isMid) // can delete
        {
            UpdateGoals(premovedir);
        }
        else  ///ismid first
        {
            UpdateGoals(moveDir);
            premovedir = moveDir;
        }
        IKSnapshot ik = CreateSnapshot(origin);
        CopySnapshot(ref current, ik);

        SetIkPosition(isMid, goals.lf, current.lf, AvatarIKGoal.LeftFoot);   //can delete
        SetIkPosition(isMid, goals.rf, current.rf, AvatarIKGoal.RightFoot);
        SetIkPosition(isMid, goals.lh, current.lh, AvatarIKGoal.LeftHand);
        SetIkPosition(isMid, goals.rh, current.rh, AvatarIKGoal.RightHand);

        UpdateIKWeight(AvatarIKGoal.LeftFoot, 1);
        UpdateIKWeight(AvatarIKGoal.LeftHand, 1);
        UpdateIKWeight(AvatarIKGoal.RightFoot, 1);
        UpdateIKWeight(AvatarIKGoal.RightHand, 1);
    }
    void UpdateGoals(Vector3 moveDir) // can return
    {
        isLeft = (moveDir.x <= 0);
        if(moveDir.x !=0)
        {
            goals.lh = isLeft;
            goals.rh = !isLeft;
            goals.lf = isLeft;
            goals.rf = !isLeft;
        }
        else
        {
            bool isEnabled = isMirror;
            if(moveDir.y < 0)
            {
                isEnabled = !isEnabled;
            }
            goals.lh = isEnabled;
            goals.rh = !isEnabled;
            goals.lf = isEnabled;
            goals.rf = !isEnabled;
        }
            
    }
    void HandlerAnimation(Vector3 moveDir,bool isMid)
    {
        if(isMid)
        {
            if (moveDir.y != 0)
            {
                isMirror = !isMirror;
                anim.SetBool("mirror", isMirror);
                anim.CrossFade("climb_up", 0.2f);
            }
        }
        else
        {
            anim.CrossFade("climb_idle", 0.2f);
        }
    }
    public void UpdateIKPosition(AvatarIKGoal goal, Vector3 pos)
    {
        switch(goal)
        {
            case AvatarIKGoal.LeftFoot:
                lf = pos;
                break;
            case AvatarIKGoal.LeftHand:
                lh = pos;
                break;
            case AvatarIKGoal.RightFoot:
                rf = pos;
                break;
            case AvatarIKGoal.RightHand:
                rh = pos;
                break;
        }
    }
    public void UpdateIKWeight(AvatarIKGoal goal, float w)
    {
        switch (goal)
        {
            case AvatarIKGoal.LeftFoot:
                w_lf = w;
                break;
            case AvatarIKGoal.LeftHand:
                w_lh = w;
                break;
            case AvatarIKGoal.RightFoot:
                w_rf = w;
                break;
            case AvatarIKGoal.RightHand:
                w_rh = w;
                break;
        }
    }

    public IKSnapshot CreateSnapshot(Vector3 o)
    {
        IKSnapshot r = new IKSnapshot();
        Vector3 r_lh = LocalToWorld(ikBase.lh);
        r.lh = ActualPos(r_lh, AvatarIKGoal.LeftHand);
        Vector3 r_rh = LocalToWorld(ikBase.rh);
        r.rh = ActualPos(r_rh, AvatarIKGoal.RightHand);
        Vector3 r_lf = LocalToWorld(ikBase.lf);
        r.lf = ActualPos(r_lf, AvatarIKGoal.LeftFoot);
        Vector3 r_rf = LocalToWorld(ikBase.rf);
        r.rf = ActualPos(r_rf, AvatarIKGoal.RightFoot);
        return r;
    }
    void SetIkPosition(bool isMid, bool isTrue, Vector3 pos, AvatarIKGoal goal)  // can return
    {
        if(isMid)// ismid일때는
        {
            if(isTrue) //left만 updateikposition 해줌
            {
                Vector3 p = pos;  //GetposActual(pos)
                UpdateIKPosition(goal, p);
            }
        }
        else 
        {
            if(!isTrue) // right만 update함
            {
                Vector3 p = pos;
                UpdateIKPosition(goal, p);
            }
        }
    }
    private void OnAnimatorIK(int layerIndex)
    {
        delta = Time.deltaTime;
        SetIKPos(AvatarIKGoal.LeftFoot, lf, w_lf);
        SetIKPos(AvatarIKGoal.LeftHand, lh, w_lh);
        SetIKPos(AvatarIKGoal.RightFoot, rf, w_rf);
        SetIKPos(AvatarIKGoal.RightHand, rh, w_rh);

        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, h.localRotation); //오발,왼발 로테이션 벽쪽으로
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
        anim.SetIKRotation(AvatarIKGoal.LeftFoot, h.localRotation);
    }
    void SetIKPos(AvatarIKGoal goal,Vector3 tp, float w)
    {
        IKStates ikState = GetIKStates(goal);
        if(ikState == null)
        {
            ikState = new IKStates();
            ikState.goal = goal;
            ikStates.Add(ikState);
        }
        if(w ==0)
        {
            ikState.isSet = false;
        }
        if(ikState.isSet)
        {
            ikState.position = GoalTobodyBones(goal).position;
            ikState.isSet = true;
        }
        ikState.positionWeight = w;
        ikState.position = Vector3.Lerp(ikState.position, tp, delta * lerpSpeed);
        anim.SetIKPositionWeight(goal, ikState.positionWeight);
        anim.SetIKPosition(goal, ikState.position);
    }
    Transform GoalTobodyBones(AvatarIKGoal goal)
    {
        switch (goal)
        {
            case AvatarIKGoal.LeftFoot:
                return anim.GetBoneTransform(HumanBodyBones.LeftFoot);
            case AvatarIKGoal.LeftHand:
                return anim.GetBoneTransform(HumanBodyBones.LeftHand);
            case AvatarIKGoal.RightFoot:
                return anim.GetBoneTransform(HumanBodyBones.RightFoot);
            default:
            case AvatarIKGoal.RightHand:
                return anim.GetBoneTransform(HumanBodyBones.RightHand);
        }

    }
    Vector3 LocalToWorld(Vector3 p)
    {
        Vector3 r = h.position;
        r += h.right * p.x;
        r += h.up * p.y;
        r += h.forward * p.z;
        return r;
    }
    Vector3 ActualPos(Vector3 o, AvatarIKGoal goal)
    {
        Vector3 r = o;
        Vector3 dir = h.forward;
        RaycastHit hit;
        if(Physics.Raycast(o,dir,out hit, 0.1f))
        {
            Vector3 _r = hit.point + (hit.normal * 0.1f);
            r = _r;
        }
        return r;
    }
    public void CopySnapshot(ref IKSnapshot to, IKSnapshot from)
    {
        to.rh = from.rh;
        to.lh = from.lh;
        to.rf = from.rf;
        to.lf = from.lf;
    }
    IKStates GetIKStates(AvatarIKGoal goal)
    {
        IKStates r = null;
        foreach(IKStates i in ikStates)
        {
            if(i.goal == goal)
            {
                r = i;
                break;
            }
        }
        return r;
    }
    List<IKStates> ikStates = new List<IKStates>();
    class IKStates
    {
        public AvatarIKGoal goal;
        public Vector3 position;
        public float positionWeight;
        public bool isSet;
    }

}
public class IKGoals
{
    public bool rh;
    public bool rf;
    public bool lh;
    public bool lf;
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class climb : MonoBehaviour
{
    public Animator anim;
    Transform helper;
    //Transform helper_helper;
    public float offsetfromwall;
    public float climbspeed =5f;
    public float rotaionspeed = 5f;
    public float trans_helper = 1.2f;
    Vector3 startpos;
    Vector3 targetpos;
    float playerotation;
    public LayerMask hanglayermask;

    Vector3 baserh, baselh, baserf, baself;

    public IKSnapshot baseIKSnapshot;
    public ikanimation ikani;
    public PlayerController pct;

    float t;
    float delta;
    float vertical;
    bool inposition;
    bool islerping;
    bool isRotation;
    public bool isMid;


    void Start()
    {
        pct = GetComponent<PlayerController>();
        helper = new GameObject().transform;
        helper.position = transform.position;
        helper.rotation = transform.rotation;
        helper.name = "climb_helper";
        ikani.Init(this, helper);
        
        islerping = false;
    }
    public bool Checkclimb()
    {
        Vector3 origin = transform.position;
        Vector3 dir = transform.forward;
        RaycastHit hit;
        if(Physics.Raycast(origin, dir, out hit, 0.7f,hanglayermask)) //캐릭터 앞으로 레이캐스트 쏨   0.7로 줄일꺼
        {
            ikani.enabled = true;
            helper.position = hit.point + (hit.normal * offsetfromwall);
            helper.rotation = transform.rotation;
            startpos = transform.position;
            targetpos = helper.position;
            inposition = false;
            anim.CrossFade("climb_idle", 0.5f);
            return true;
        }
        return false;
    }
    public void Tick()
    {
        if(!inposition)
        {
            Getinposition(); //climb 할수 있는 벽으로 붙음, climb상태로 변경
            return;
        }
        if(!islerping)
        {
            vertical = Input.GetAxis("Vertical"); // 위,아래 입력만 받음
            Vector3 v = new Vector3(0, vertical, 0);
            v = v.normalized; //v는 방향벡터
            if(isMid) // 한번이 아닌 2번에 걸쳐 이동
            {
                if (v == Vector3.zero) //움직임 없음
                    return;
            }
            else
            { //v가 +일경우 위로 갈수있는지 판단, -일경우 아래로 내려갈수있는지 판단
                bool canmove = CanMove(v); //위 또는 아래로 갈수 있는 경우 helper의 위치를 바꾸어줌
                if (!canmove || v == Vector3.zero)
                    return;
            }
            isMid = !isMid;
            t = 0;
            islerping = true;      
            startpos = transform.position;  // 현 캐릭터 위치
            Vector3 d = helper.position - transform.position;
            d = d * 0.5f + transform.position;    // helper와 현재캐릭터의 2분의 1(0.5f)
            targetpos = (isMid) ? d : helper.position; // helper위치의 반씩 이동한다
            ikani.CreatePosition(targetpos, v, isMid); // ikanimation 설정
        }
        else
        {
            t += delta * climbspeed;
            if(t >1) // t가 1이 넘어갈때까지 else문
            {
                t = 1;
                islerping = false;
            }
            Vector3 cp = Vector3.Lerp(startpos, targetpos, t);  //climbing position
            transform.position = cp; //캐릭터를 이동 및 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotaionspeed);
            GoGround(); //jump버튼 입력시 climb상태 해제
        }
    }
    bool CanMove(Vector3 moveDir)
    {
        Vector3 origin = transform.position;
        Vector3 dir = moveDir; // movedir 은 (0,1,0), 또는 (0,-1,0)
        RaycastHit hit;
        float dis;
        if (moveDir == Vector3.up)
        {
            dis = 2f;
            dir = helper.up;
            Debug.DrawRay(origin, dir, Color.blue);
            if (Physics.Raycast(origin, dir, out hit, dis, hanglayermask) && dir == Vector3.up)
            {
                if(hit.transform.name == "door")
                {
                    return false;
                }
                return CanCloudBridge(dir);
            }
            dis = 1.7f;
            origin += dir * dis;
            dir = helper.forward;
            Debug.DrawRay(origin, dir*dis, Color.white);
            dis = 1f;
            if (Physics.Raycast(origin, dir, out hit, dis, hanglayermask))
            {
                helper.position = PosOffset(origin, hit.point) - helper.up * trans_helper;
                return true;
            }              
            origin += dir * dis;
            dir = -helper.up;
            Debug.DrawRay(origin, dir*dis, Color.red);
            if (Physics.Raycast(origin, dir, out hit, dis, hanglayermask)) // 1. 고대로 올라가기 2.반대편으로 한바퀴 돌면서 가기
            {
                Quaternion q = Quaternion.LookRotation(dir);
                float eq = q.eulerAngles.x;
                if (dir == Vector3.down) //그대로 올라가기
                {
                    helper.rotation = Quaternion.Euler(eq, helper.rotation.eulerAngles.y, helper.rotation.eulerAngles.z);
                }
                else //한바퀴 돌면서 가기
                {
                    helper.rotation = Quaternion.Euler(eq, helper.rotation.eulerAngles.y + 180f, helper.rotation.eulerAngles.z);
                }
                helper.position = PosOffset(origin, hit.point) - helper.up * trans_helper;
                return true;
            }


        }
        if(moveDir == -Vector3.up)
        { //위아래 둘다 확인
            dis = 0.5f;
            dir = -helper.up;
            Vector3 origin2 = origin;
            Vector3 dir2 = helper.up;
            //float dis2 = 1.8f;
            if(Physics.Raycast(origin, dir, out hit, dis, hanglayermask))// 아래 장애물 있으면 false
            {
                return false;
            }
            origin += dir * dis;
            dis = 1f;
            dir = helper.forward;
            Debug.DrawRay(origin, dir * dis, Color.green); // 앞쪽으로 쏨
            if (Physics.Raycast(origin, dir,out hit, dis , hanglayermask))
            {
                helper.position = PosOffset(origin, hit.point);
                return true;
            }
            origin += dir * dis;
            dir = helper.up;
            if(Physics.Raycast(origin, dir , out hit, dis,hanglayermask))
            {
                Quaternion q = Quaternion.LookRotation(dir);
                float eq = q.eulerAngles.x;
                helper.rotation = Quaternion.Euler(eq, helper.rotation.eulerAngles.y, helper.rotation.eulerAngles.z);
                helper.position = PosOffset(origin, hit.point) - helper.up * trans_helper;
                return true;
            }
            /*   보류   시간있으면 수정
            if(Physics.Raycast(origin2,dir2,out hit,dis2,hanglayermask)) //위에 장애물 있으면 false;
            {
                //cloud climb
                return false;
            }
            origin += dir2 * dis2;
            dir2 = helper.forward;
            dis2 = 1f;
            if(Physics.Raycast(origin,dir2,out hit, dis2,hanglayermask))
            {
                helper.position = PosOffset(origin, hit.point) - helper.up * 2.3f;
                return true;
            }*/

        }
        return false;
    }
    bool CanCloudBridge(Vector3 moveDir) // movedir 필요없을지도
    {
        Debug.Log("cloudBridge_start");
        Vector3 origin;
        Vector3 dir;
        float dis;
        RaycastHit hit;
        origin = helper.position;
        dir = -helper.forward;
        dis = 0.5f;
        if (!isRotation) //회전상태가 아니면 큐브기 때문에 회전함
        {
            if (Physics.Raycast(origin, dir, out hit, dis, hanglayermask))
            {
                return false;
            }
            origin += dir * dis;
            dir = helper.up;
            dis = 2.2f;
            if (Physics.Raycast(origin, dir, out hit, dis, hanglayermask))
            {
                helper.position = hit.point - helper.up * 1.8f;
                helper.rotation = Quaternion.Euler(helper.rotation.eulerAngles.x, helper.rotation.eulerAngles.y + 180f, helper.rotation.eulerAngles.z);
                isRotation = true;
                return true;
            }
            return false;
        }
        else
        {
            origin = helper.position;
            dir = helper.forward;
            dis = 0.5f;
            if (Physics.Raycast(origin, dir, out hit, dis, hanglayermask)) // 앞쪽 레이케스트
            {
                return false;
            }
            origin += dir * dis;
            dir = helper.up;
            dis = 2.2f;
            if (Physics.Raycast(origin, dir, out hit, dis, hanglayermask)) //위에 맞으면 이동
            {
                helper.position = hit.point - helper.up * 1.8f;
                return true;
            }
            origin += dir * dis;
            dir = -helper.forward;
            dis = 0.5f;
            if (Physics.Raycast(origin, dir, out hit, dis, hanglayermask))
            {
                helper.position = PosOffset(origin, hit.point) - helper.up * trans_helper;
                helper.rotation = Quaternion.Euler(helper.rotation.eulerAngles.x, helper.rotation.eulerAngles.y + 180f, helper.rotation.eulerAngles.z);
                isRotation = false;
                return true;
            }
        }

        return false;
    }
    // Update is called once per frame
    void Update()
    {
        //Tick();
        delta = Time.deltaTime;
    }
    void Getinposition()
    {
        t += delta * 1f;
        if(t >1)
        {
            inposition = true;
            ikani.CreatePosition(targetpos, Vector3.zero, false);
        }
        Vector3 tp = Vector3.Lerp(startpos, targetpos,t);
        transform.position = tp;
        transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, t);
    }
    Vector3 PosOffset(Vector3 origin, Vector3 target)
    {
        Vector3 direction = origin - target;
        direction.Normalize();
        Vector3 offset = direction * offsetfromwall;
        return offset + target;
    }

    void GoGround()
    {
        if(Input.GetButtonUp("Jump"))
        {
            pct.isClimbing = false;
            pct.EnableController();
            ikani.enabled = false;
        }
    }
}
[System.Serializable]
public class IKSnapshot
{
    public Vector3 rh, lh, rf, lf;
}
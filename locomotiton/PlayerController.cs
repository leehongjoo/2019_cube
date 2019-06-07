using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    float horizontal;
    float vertical;
    Vector3 moveDir;
    float moveamount;
    Animator anim;
    Collider col;
    Rigidbody rigid;
    public int health = 1;
    public Transform respawn;
    public float moveSpeed = 4;
    public float rotSpeed = 9;
    public float jumpSpeed = 15;
    
    climb climb;

    bool isDie;
    bool onGround;
    bool keepOffGround;
    bool climbOff;
    public bool isClimbing;
    float climbtimer;
    float savedTime;
    

    public Transform acquirechan;
    Transform maincamera;

    public Invenotry inven;

    public Transform right_hand;
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        col = GetComponent<Collider>();
        anim = GetComponentInChildren<Animator>();
        climb = GetComponent<climb>();
        maincamera = GameObject.FindGameObjectWithTag("MainCamera").transform;

    }
    private void FixedUpdate() // 물리 연산
    {
        if (health == 0) //죽으면 입력을 받지 않음
            return;
        if (isClimbing)
            return;
        onGround = OnGround();
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Vector3 v = vertical * maincamera.forward;
        Vector3 h = horizontal * maincamera.right;
        moveDir = (v + h).normalized;
        moveamount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        Vector3 targetDir = moveDir;
        targetDir.y = 0;
        if (targetDir == Vector3.zero)
            targetDir = transform.forward;
        Quaternion lookDIr = Quaternion.LookRotation(targetDir);
        Quaternion targetRot = Quaternion.Slerp(transform.rotation, lookDIr, Time.deltaTime * rotSpeed);
        transform.rotation = targetRot;

        Vector3 dir = transform.forward * (moveSpeed * moveamount);
        dir.y = rigid.velocity.y;
        rigid.velocity = dir;
    }
    void Update()
    {
        if(health ==0)
        {
            if (!isDie)
                Die();
            return;
        }
        acquirechan.position  = transform.position; // air 모션일때 캐릭터 넘어가는현상 방지
        if(Input.GetMouseButton(0))
        {
            Throw();
        }
        #region locomotion
        if (isClimbing)
        {
            climb.Tick();
            return;
        }
        onGround = OnGround();
        if(keepOffGround)
        {
            if (Time.realtimeSinceStartup - savedTime > 0.5f)
            {
                keepOffGround = false;
            }
        }
        jump();
        if(!onGround && !keepOffGround)
        {
            if(!climbOff)
            {
                isClimbing = climb.Checkclimb();
                if(isClimbing)
                {
                    DisableController();
                }
            }
        }
        if(climbOff)
        {
            if(Time.realtimeSinceStartup - climbtimer >1)
            {
                climbOff = false;
            }
        }
        anim.SetFloat("move", moveamount);
        anim.SetBool("onAir", !onGround);
        #endregion 

    }
    void jump()
    {
        if(onGround)
        {
            if(Input.GetButtonUp("Jump"))
            {
                Vector3 v = rigid.velocity;
                v.y = jumpSpeed;
                rigid.velocity = v;
                savedTime = Time.realtimeSinceStartup;
                keepOffGround = true;
            }
        }
    }
    bool OnGround()
    {
        if (keepOffGround)
            onGround = false;
        Vector3 origin = transform.position;
        Vector3 dir = -transform.up;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, 0.3f)) // 0.2f 이하로는 air 상태가 자꾸 됨
        {
            return true;
        }
        return false;
    }
    public void DisableController()
    {
        rigid.isKinematic = true;
        //col.enabled = false;
    }
    public void EnableController()
    {
        rigid.isKinematic = false;
        //col.enabled = true;
        anim.CrossFade("onAir",1f);
        climbOff = true;
        climbtimer = Time.realtimeSinceStartup;
        isClimbing = false;
    }
    public void Pickup(int pickid) // 아이템 줍는 animation && 인벤토리에 추가해줌
    {
        anim.CrossFade("pick_up", 1f);
        inven.Additem(pickid, 1);
    }
    void Drop()
    {

    }
    void Throw() //종속 관계 해제 후 setequip(false), rigid 던지기, 애니매이션
    {
        GameObject throwitem = GameObject.FindGameObjectWithTag("equip_point").GetComponentInChildren<itempickup>().gameObject;
        if (throwitem)
        {
            StartCoroutine(delaythrow(throwitem));
        }
    }
    public void Setequip(GameObject equipitem, bool isEquip)
    {
        Collider[] itemcolliders = equipitem.GetComponents<Collider>();
        Rigidbody rigid = equipitem.GetComponent<Rigidbody>();
        foreach(Collider itemcollider in itemcolliders)
        {
            itemcollider.enabled = !isEquip;
        }
        rigid.isKinematic = isEquip;
    }
    IEnumerator delaythrow(GameObject throwitem) // animation과 동기화 시켜주는 delay함수
    {
        anim.CrossFade("throw", 0.1f);
        yield return new WaitForSeconds(0.7f); //delay를 줌
        Rigidbody rigid = throwitem.GetComponent<Rigidbody>();
        Setequip(throwitem, false);
        throwitem.transform.parent = null;
        Vector3 throwangle = transform.forward + transform.up;
        throwangle = throwangle.normalized;
        throwangle.y = 2f;
        rigid.AddForce(throwangle * 4f, ForceMode.Impulse);
    }
    void Die()
    {
        isDie = true;
        rigid.velocity = Vector2.zero;
        anim.Play("Die");
        Invoke("restart", 4f);
    }
    void restart() //scene 소환
    {
        health = 1;
        transform.position = respawn.position;
    }
}

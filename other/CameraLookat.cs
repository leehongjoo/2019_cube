using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookat : MonoBehaviour
{
    public bool following = true;
    public float sensitivity; //마우스 감도 
    public float ZoomSpeed;
    GameObject Player;
    Vector3 lastPosition;

    private float dis;
    private float r, r2;
    private float i, i2;
    private Vector3 p, o, dis_vec;

    LayerMask door;
    private void Start()
    {
        door = 1 << LayerMask.NameToLayer("Ignore Raycast");
        door = ~door;
        Player = GameObject.FindGameObjectWithTag("Player");
        r = Mathf.Sqrt(Mathf.Pow(transform.position.x - Player.transform.position.x, 2f) + Mathf.Pow(transform.position.z - Player.transform.position.z, 2f)); //기본 반지름 
        //mathf.pow = 거듭제곱 
        i = 0; i2 = 0;
        i -= 30; //카메라 초기 상태 x축90도 회전 
        i2 -= 30; //y축 30도 회전 
    }

    // 구는 원의 모임 -> y값 따라서 반지름이 달라짐 
    void Update()
    {
        if (following)
        {
            //Cursor.visible = false; 
            float xmouse = Input.GetAxis("Mouse X");
            float ymouse = Input.GetAxis("Mouse Y");
            float mousewheel = Input.GetAxis("Mouse ScrollWheel");

            float wheel = mousewheel * ZoomSpeed * -1;
            o = Player.transform.position + new Vector3(0,1.5f,0); //원의 중심 
            r = r + wheel;

            i = (i + xmouse) % 360; //0~360도 
            i2 = Mathf.Clamp(i2 + ymouse, -90 / sensitivity + 1, 90 / sensitivity - 1); //머리 위부터 다리 아래까지로 제한 

            r2 = Mathf.Abs(Mathf.Cos(i2 * sensitivity * Mathf.Deg2Rad)) * r; // 기본 반지름~ 0 까지 값  
            float rx = Mathf.Cos(i * sensitivity * Mathf.Deg2Rad) * r2 * (-1); //x축 좌표 
            float rz = Mathf.Sin(i * sensitivity * Mathf.Deg2Rad) * r2; //z축 좌표 
            float ry = Mathf.Sin(i2 * sensitivity * Mathf.Deg2Rad) * r * (-1); //y축 좌표 

            Vector3 camera_transform = new Vector3(o.x + rx, o.y + ry, o.z + rz); // 플레이어좌표 + 각좌표 
            transform.position = camera_transform; //set position 
        }
        #region Hitcontroll 
        RaycastHit hitinfo;
        dis = Vector3.Distance(transform.position, o); // 플레이어와 카메라 거리 새로구함 
        dis_vec = new Vector3(transform.position.x - o.x, transform.position.y - o.y, transform.position.z - o.z);
        // 플레이어와 카메라 방향 
        dis_vec = dis_vec.normalized; // 방향 표준화 
        bool isHit = Physics.Raycast(o, dis_vec, out hitinfo, dis, door); //플레이어에서 카메라방향으로 레이저 쏨 
        if (isHit) //맞으면 
        {
            transform.position = hitinfo.point; //맞은위치로 카메라 새로 이동 
        }
        #endregion
        transform.LookAt(o); // 플레이어 바라봄 
        if (Input.GetKeyDown(KeyCode.LeftControl)) // 커서
            following = !following;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //스피드 조정 변수
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] float crouchSpeed;
    private float applySpeed;

    [SerializeField] private float jumpForce;

    //상태변수
    private bool isRun = false;
    private bool isGround = true;//땅에 있을경우에만 점프가능
    private bool isCrouch = false;//서있을 때만 쭈그리기 가능

    //얼마나 앉을지 결정하는 변수
    [SerializeField] private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    //땅 착지여부확인위해(땅의 meshcollider와 닿아있는지 확인)
    private CapsuleCollider capsuleCollider;
    

    //카메라 민감도,한계
    [SerializeField] private float lookSensitivity;//카메라의 민감도
    [SerializeField] private float cameraRotationLimit;//카메라 움직임 각도 제한
     private float currentCameraRotationX= 0f;//일단 정면보도록!

    //필요한 컴포넌트
    [SerializeField]
    Camera theCamera;//player안에 있으므로 직접 드래그하도록
    private Rigidbody myRigid;
    private StatusController theStatusController;
    
    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;//player기준높이이므로 localPosition
        applyCrouchPosY = originPosY; //앉은위치는 카메라의 상하위치 변동으로 나타낸다
        theStatusController = FindObjectOfType<StatusController>();

    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();
        if (!Inventory.inventoryActivated)
        {
            CharacterRotation();//좌우 케릭터 회전
            CameraRotation();//상하 카메라 회전
        }
    }

    //지면에 닿아있는지 체크
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y+0.5f);
        //현재위치에서 아래로 캡슐(player)의 y축의 반절만큼(+오차) 레이저를 쏴 땅에 닿아있으면 true이다
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&isGround==true&&(theStatusController.GetCurrentSP()>0))
        {
            //Jump();
            if (isCrouch) Crouch();//앉아있을경우 점프하면 일어남!
            myRigid.velocity = transform.up * jumpForce;
            theStatusController.DecreaseStamina(100);
        }
    }

   /* private void Jump()
    {
        myRigid.velocity = transform.up * jumpForce;
    }
    */
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && (theStatusController.GetCurrentSP() > 0))
        {
            if (isCrouch) Crouch();//앉아있을경우 달리면 일어남!
            //Running();
            isRun = true;
            applySpeed = runSpeed;
            theStatusController.DecreaseStamina(10);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || (theStatusController.GetCurrentSP() <= 0))
        {
            //RunningCancel();
            isRun = false;
            applySpeed = walkSpeed;
        }
    }

 /*   private void Running()
    {
        isRun = true;
        applySpeed = runSpeed;
    }

    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }
 */
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        isCrouch = !isCrouch;//bool상태를 역으로 바꿔주는 것

        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }
        theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, applyCrouchPosY, theCamera.transform.localPosition.z);
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");//양옆
        float _moveDirZ = Input.GetAxisRaw("Vertical");//앞뒤

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;
        //normalized 통해 합이 1 이 되므로 1초에 얼마나 이동시킬지 계산이 편해진다

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
        //현재위치에서 _velocity까지 한번에 deltaTime씩 움직인다
        //Time.deltaTime:Update()가 1초에 60번 호출되므로 약 0.016?

    }

    //케릭터 회전(좌우회전)
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0)*lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        //벡터값(3원소)을 Quaternion(4원소)으로 변환시켜서 인자로 넣어준다

    }

    //카메라 회전(상하회전)
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");//마우스를 움직여서 고개를 들고 내리는 효과
        float _cameraRotationX = _xRotation * lookSensitivity;//움직임 정도를 제한
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX,-cameraRotationLimit,cameraRotationLimit);
        //고개 회전각 limit안에 가두기

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);//상하로만 움직임
    }
}

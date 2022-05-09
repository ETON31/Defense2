using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용자 입력에 따라 플레이어 캐릭터를 움직이는 스크립트
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;    // 이동 속도 변수
    public float gravity = -3f;     // 중력 변수
    public float yVelocity = 0;     // 수직 속력 변수
    public float jumpPower = 1f;    // 점프력 변수
    public bool isJumping = false;  // 점프 상태 변수

    private PlayerInput playerInput;        // 플레이어 입력을 알려주는 컴포넌트
    private CharacterController playerCC;   // 플레이어 캐릭터 컨트롤러
    private Animator playerAnimator;        // 플레이어 캐릭터의 애니메이터

    // 플레이어 회전을 위한 변수
    private Ray cameraRay;      // 카메라 상의 마우스의 위치
    private Plane GroupPlane = new Plane(Vector3.up, Vector3.zero); // 교차했는지 검증을 위한 평면
    private float rayLength;    // 위치값 반환을 위한 변수

    private void Start()
    {
        // 사용할 컴포넌트의 참조 가져오기
        playerInput = GetComponent<PlayerInput>();
        playerCC = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {

    }

    // 물리 갱신 주기에 맞춰 실행되는 함수
    private void FixedUpdate()
    {
        Rotate();   // 회전 실행
        Move();     // 움직임 실행
    }

    private void Rotate()
    {
        // 마우스 포인터 방향으로 플레이어를 회전시킨다.
        cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(GroupPlane.Raycast(cameraRay, out rayLength))
        {		
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);
            transform.LookAt(new Vector3(pointTolook.x, transform.position.y, pointTolook.z));
        }
    }

    private void Move()
    {
        // Space로 점프한다.
        // 바닥에 닿아있을 때 점프하는 상태라면 상태 변경
        if (playerCC.collisionFlags == CollisionFlags.Below)
        {
            if (isJumping)
            {
                isJumping = false;
                yVelocity = 0;
            }
        }
        // 점프중이 아닐 때 space를 누르면 
        if (playerInput.jump && !isJumping)
        {
            yVelocity = jumpPower;
            isJumping = true;
            playerAnimator.SetTrigger("Jump");
        }
        // 점프중이라면 중력 적용
        if (isJumping)
        {
            yVelocity += gravity * Time.deltaTime;
        }

        // 방향을 설정하여 이동
        Vector3 direction = 
            new Vector3(playerInput.hMove, yVelocity, playerInput.vMove).normalized;
        playerCC.Move(direction * moveSpeed * Time.deltaTime);

        // 애니메이션 파라미터 설정
        playerAnimator.SetFloat("Direction", transform.rotation.eulerAngles.y);
        playerAnimator.SetFloat("Horizontal", playerInput.hMove);
        playerAnimator.SetFloat("Vertical", playerInput.vMove);
    }
}

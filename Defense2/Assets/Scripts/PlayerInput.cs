using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 캐릭터를 조작하기 위한 사용자 입력을 감지
// 감지된 입력값을 다른 컴포넌트가 사용할 수 있도록 제공
public class PlayerInput : MonoBehaviour
{
    public string vMoveAxisName = "Vertical";       // 앞뒤 움직임을 위한 입력축 이름
    public string hMoveAxisName = "Horizontal";     // 좌우 움직임을 위한 입력축 이름
    public string jumpButton = "Jump";              // 점프를 위한 버튼
    public string fire1ButtonName = "Fire1";        // 발사를 위한 입력 버튼 이름 1
    public string fire2ButtonName = "Fire2";        // 발사를 위한 입력 버튼 이름 2
    public string reloadButtonName = "Reload";      // 재장전을 위한 입력 버튼 이름
    public string escButtonName = "Escape";
    public string marketButtonName = "Tab";

    public float vMove {get; private set;}      // 감지된 앞뒤 움직임 입력값
    public float hMove {get; private set;}      // 감지된 좌우 움직임 입력값
    public bool jump {get; private set;}        // 감지된 점프 입력값
    public bool fire1 {get; private set;}       // 감지된 발사 입력값 1
    public bool fire2 {get; private set;}       // 감지된 발사 입력값 2
    public bool reload {get; private set;}      // 감지된 재장전 입력값
    public bool esc {get; private set;}
    public bool tab {get; private set;}

    private void Start()
    {
        
    }

    // 매 프레임마다 사용자 입력을 감지
    private void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            vMove = 0;
            hMove = 0;
            jump = false;
            fire1 = false;
            fire2 = false;
            reload = false;
            esc = false;
            tab = false;
            return;
        }

        vMove = Input.GetAxis(vMoveAxisName);           // 앞뒤 move에 관한 입력 감지
        hMove = Input.GetAxis(hMoveAxisName);           // 좌우 move에 관한 입력 감지
        jump = Input.GetButton(jumpButton);             // 점프에 관한 입력 감지
        fire1 = Input.GetButton(fire1ButtonName);       // fire1에 관한 입력 감지
        fire2 = Input.GetButtonDown(fire2ButtonName);       // fire2에 관한 입력 감지
        reload = Input.GetButtonDown(reloadButtonName); // reload에 관한 입력 감지
        esc = Input.GetButtonDown(escButtonName);
        tab = Input.GetButtonDown(marketButtonName);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리자 관련 코드
using UnityEngine.UI; // UI 관련 코드

// 필요한 UI에 즉시 접근하고 변경할 수 있도록 허용하는 UI 매니저
public class UIManager : MonoBehaviour {
    // 싱글턴 접근용 프로퍼티
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }

            return m_instance;
        }
    }

    private static UIManager m_instance; // 싱글톤이 할당될 변수

    public Text ammoText;   // 탄약 표시용 텍스트
    public Text scoreText;  // 점수 표시용 텍스트
    public Text cashText;   // 재화 표시용 텍스트
    public Text waveText;   // 적 웨이브 표시용 텍스트
    public GameObject gameoverUI; // 게임 오버시 활성화할 UI 
    public GameObject marketUI;   // 상점 UI
    public GameObject holdUI;     // 홀드 UI
    public GameObject OptionUI;

    // 탄약 텍스트 갱신
    public void UpdateAmmoText(int magAmmo, int remainAmmo) 
    {
        ammoText.text = magAmmo + "/" + remainAmmo;
    }

    // 점수 텍스트 갱신
    public void UpdateScoreText(int newScore) 
    {
        scoreText.text = "Score : " + newScore;
    }

    // 재화 텍스트 갱신
    public void UpdateCashText(int newCash) 
    {
        cashText.text = "Cash : " + newCash;
    }

    // 적 웨이브 텍스트 갱신
    public void UpdateWaveText(int waves, int count) 
    {
        waveText.text = "Wave : " + waves + "\nEnemy Left : " + count;
    }

    // 타이틀 화면으로 이동
    public void ToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    // Hold UI 활성화
    public void SetActiveHoldUI(bool active) 
    {
        holdUI.SetActive(active);
    }

    public void ToGame()
    {
        SetActiveHoldUI(false);
        Time.timeScale = 1;
        GameManager.instance.isHold = false;
    }

    public void GoBack()
    {
        SetActiveOptionUI(false);
    }

    // 옵션 UI 활성화
    public void SetActiveOptionUI(bool active)
    {
        OptionUI.SetActive(active);
    }

    // 상점 UI 활성화
    public void SetActiveMarketUI(bool active)
    {
        marketUI.SetActive(active);
    }

    // 게임 오버 UI 활성화
    public void SetActiveGameoverUI(bool active) 
    {
        gameoverUI.SetActive(active);
    }

    public void WindowedToggle(bool active)
    {
        if (active)
        {
            Screen.SetResolution(1600, 900, false);
        }
        else
        {
            Screen.SetResolution(1920, 1080, true);
        }
    }

    // 게임 재시작
    public void GameRestart() 
    {
        SceneManager.LoadScene("MainScene");
    }
}
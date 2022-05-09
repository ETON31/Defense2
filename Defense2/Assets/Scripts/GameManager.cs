using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    // 싱글톤 접근용 프로퍼티
    public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    public enum GameState
    {
        Ready,
        OnWave,
    }

    private static GameManager m_instance; // 싱글톤이 할당될 static 변수
    private PlayerInput playerInput;

    public Slider BGMSlider;
    public Slider EMSlider;
    public AudioSource BGM;
    public AudioSource gunSound;
    public AudioSource enemySound;
    private float BGMvol = 1f;
    private float EMvol = 1f;

    public static int score = 0;  // 현재 게임 점수
    public static int cash = 0;   // 현재 잔여 재화
    public GameState gameState;
    public bool isHold = false;
    public bool onMarket = false;
    public bool isGameover { get; private set; } // 게임 오버 상태

    private void Awake() 
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }

        playerInput = GetComponent<PlayerInput>();
    }

    private void Start() 
    {
        // 배경음 조절
        BGMvol = PlayerPrefs.GetFloat("BGMSlider", 1f);
        BGMSlider.value = BGMvol;
        BGM.volume = BGMSlider.value;
        // 효과음 조절
        EMvol = PlayerPrefs.GetFloat("EMSlider", 1f);
        EMSlider.value = EMvol;
        gunSound.volume = EMSlider.value;
        enemySound.volume = EMSlider.value;

        gameState = GameState.Ready;
        // 시간 초기화
        Time.timeScale = 1;
        // 플레이어 캐릭터의 사망 이벤트 발생시 게임 오버
        FindObjectOfType<PlayerHealth>().onDeath += EndGame;
    }

    private void Update()
    {
        // 소리 조절
        SoundController();

        // 게임오버되지 않았을 경우
        if (!isGameover)
        {
            // esc 버튼을 눌렀을 시 홀드 버튼
            if (playerInput.esc && !isHold)
            {
                UIManager.instance.SetActiveHoldUI(true);
                Time.timeScale = 0;
                isHold = true;
            }

            if (playerInput.tab && gameState == GameState.Ready && !isHold)
            {
                if (!onMarket)
                {
                    UIManager.instance.SetActiveMarketUI(true);
                    onMarket = true;
                }
                else 
                {
                    UIManager.instance.SetActiveMarketUI(false);
                    onMarket = false;
                }
            }
        }
    }

    public void SoundController()
    {
        BGMController();
        EMController();
    }

    public void BGMController()
    {
        BGM.volume = BGMSlider.value;

        BGMvol = BGMSlider.value;
        PlayerPrefs.SetFloat("BGMvol", BGMvol);
    }

    public void EMController()
    {
        gunSound.volume = EMSlider.value;
        enemySound.volume = EMSlider.value;

        EMvol = EMSlider.value;
        PlayerPrefs.SetFloat("EMvol", EMvol);
    }

    // 점수를 추가하고 UI 갱신
    public void AddScore(int newScore) 
    {
        // 게임 오버가 아닌 상태에서만 점수 증가 가능
        if (!isGameover)
        {
            // 점수 추가
            score += newScore;
            // 점수 UI 텍스트 갱신
            UIManager.instance.UpdateScoreText(score);
        }
    }

    public void AddCash(int newCash) 
    {
        // 게임 오버가 아닌 상태에서만 재화 증가 가능
        if (!isGameover)
        {
            // 재화 추가
            cash += newCash;
            // 재화 UI 텍스트 갱신
            UIManager.instance.UpdateCashText(cash);
        }
    }

    public void BuyProduct(int newCash)
    {
        if (!isGameover)
        {
            cash -= newCash;
            UIManager.instance.UpdateCashText(cash);
        }
    }

    // 게임 오버 처리
    public void EndGame() 
    {
        // 게임 오버 상태를 참으로 변경
        isGameover = true;
        // 게임 오버 UI를 활성화
        UIManager.instance.SetActiveGameoverUI(true);
    }
}
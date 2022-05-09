using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 적 게임 오브젝트를 주기적으로 생성
public class EnemySpawner : MonoBehaviour {
    public Button waveButton;
    private Button button;

    public Enemy enemyPrefab; // 생성할 적 AI

    public GameObject[] spawnAreas; // 적 AI를 소환할 위치들

    public float damageMax = 40f; // 최대 공격력
    public float damageMin = 20f; // 최소 공격력

    public float healthMax = 200f; // 최대 체력
    public float healthMin = 100f; // 최소 체력

    public float speedMax = 3f; // 최대 속도
    public float speedMin = 1f; // 최소 속도

    // public Color strongEnemyColor = Color.red; // 강한 적 AI가 가지게 될 피부색

    private List<Enemy> enemies = new List<Enemy>(); // 생성된 적들을 담는 리스트
    private int wave = 0; // 현재 웨이브

    private void Awake()
    {
        button = waveButton.GetComponent<Button>();
    }

    private void Update() {
        // 게임 오버 상태일때는 생성하지 않음
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        // 웨이브의 모든 적을 처치하면 다음 웨이브 진행 가능
        if (enemies.Count == 0)
        {
            button.interactable = true;
            GameManager.instance.gameState = GameManager.GameState.Ready;
        }

        // UI 갱신
        UpdateUI();
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI() {
        // 현재 웨이브와 남은 적의 수 표시
        UIManager.instance.UpdateWaveText(wave, enemies.Count);
    }

    // 현재 웨이브에 맞춰 적을 생성
    public void SpawnWave() {
        GameManager.instance.gameState = GameManager.GameState.OnWave;
        // 웨이브 1 증가
        wave++;

        button.interactable = false;

        // 현재 웨이브 * 1.5를 반올림한 수만큼 적 생성
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);

        // spawnCount만큼 적 생성
        for (int i = 0; i < spawnCount; i++)
        {
            // 적의 세기를 0%에서 100% 사이에서 랜덤 결정
            // float enemyIntensity = Random.Range(0f, 1f);
            // 적 생성 처리 실행
            CreateEnemy();
            Debug.Log("created");
            StartCoroutine(ZenDelay());
            Debug.Log("Coroutine");
        }
    }

    private IEnumerator ZenDelay()
    {
        yield return new WaitForSeconds(1.0f);
    }

    // 적을 생성하고 생성한 적에게 추적할 대상을 할당
    private void CreateEnemy() {
        /*
        // intensity를 기반으로 적의 능력치 결정
        float health = Mathf.Lerp(healthMin, healthMax, intensity);
        float damage = Mathf.Lerp(damageMin, damageMax, intensity);
        float speed = Mathf.Lerp(speedMin, speedMax, intensity);

        // intensity를 기반으로 하얀색과 enemyStrength 사이에서 적의 피부색 결정
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);
        */

        // 적 랜덤 생성 위치 지정
        Vector3 spawnPoint = GetRandomPosition();

        // 적 프리팹으로부터 적 생성
        Enemy enemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);

        // 생성한 적의 능력치와 추적 대상 설정
        // enemy.Setup(health, damage, speed, skinColor);

        // 생성된 적을 리스트에 추가
        enemies.Add(enemy);

        // 적의 onDeath 이벤트에 익명 메서드 등록
        // 사망한 적을 리스트에서 제거
        enemy.onDeath += () => enemies.Remove(enemy);
        // 사망한 적을 3초 뒤에 파괴
        enemy.onDeath += () => Destroy(enemy.gameObject, 3f);
        // 적 사망 시 점수 상승
        enemy.onDeath += () => GameManager.instance.AddScore(100);
    }

    Vector3 GetRandomPosition()
    {
        GameObject rangeObject = spawnAreas[Random.Range(0, spawnAreas.Length)];
        BoxCollider rangeCollider = rangeObject.GetComponent<BoxCollider>();

        Vector3 originPosition = rangeObject.transform.position;
        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float xRange = rangeCollider.bounds.size.x;
        float zRange = rangeCollider.bounds.size.z;
        
        xRange = Random.Range((xRange/2) * -1, xRange/2);
        zRange = Random.Range((zRange/2) * -1, zRange/2);
        Vector3 randomPosition = new Vector3(xRange, 0f, zRange);

        Vector3 respawnPosition = originPosition + randomPosition;
        return respawnPosition;
    }
}
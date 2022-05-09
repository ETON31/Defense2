using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour {
    // 게임 시작
    public void GameStart() {
        SceneManager.LoadScene("MainScene");
    }

    // 게임 종료
    public void Exit()
    {
        Application.Quit();
    }
}
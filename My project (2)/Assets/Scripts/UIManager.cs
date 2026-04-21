using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject HelpPanel;

    public void GameStartButtonAction()
    {
        // 본인 첫 씬 이름 쓰기
        SceneManager.LoadScene("Level_1");
    }

    public void OpenHelpPanel()
    {
        HelpPanel.SetActive(true);
    }

    public void CloseHelpPanel()
    {
        HelpPanel.SetActive(false);
    }

    // 게임 종료 버튼 기능
    public void GameExit()
    {
        // 실제 빌드된 게임(PC, 모바일 등)을 종료
        Application.Quit();

        // 유니티 에디터에서 테스트 중일 때도 꺼지도록 설정
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

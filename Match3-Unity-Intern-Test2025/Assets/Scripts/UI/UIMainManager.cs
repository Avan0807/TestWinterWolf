using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIMainManager : MonoBehaviour
{
    private IMenu[] m_menuList;
    private GameManager m_gameManager;

    private void Awake()
    {
        m_menuList = GetComponentsInChildren<IMenu>(true);
    }

    private void Start()
    {
        for (int i = 0; i < m_menuList.Length; i++)
            m_menuList[i].Setup(this);
    }

    internal void Setup(GameManager gameManager)
    {
        m_gameManager = gameManager;
        m_gameManager.StateChangedAction += OnGameStateChange;
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.MAIN_MENU:
                ShowMenu<UIPanelMain>();
                break;
            case GameManager.eStateGame.GAME_STARTED:
                ShowMenu<UIPanelGame>();
                break;
            case GameManager.eStateGame.PAUSE:
                ShowMenu<UIPanelPause>();
                break;
            case GameManager.eStateGame.GAME_OVER:
                var panelWin = GameObject.Find("PanelWin");
                if (panelWin) panelWin.SetActive(false);

                ShowMenu<UIPanelGameOver>();
                break;
        }
    }

    private void ShowMenu<T>() where T : IMenu
    {
        for (int i = 0; i < m_menuList.Length; i++)
        {
            var menu = m_menuList[i];
            if (menu is T) menu.Show();
            else menu.Hide();
        }
    }

    internal void ShowMainMenu()
    {
        m_gameManager.ClearLevel();
        m_gameManager.SetState(GameManager.eStateGame.MAIN_MENU);
    }

    internal void ShowPauseMenu() => m_gameManager.SetState(GameManager.eStateGame.PAUSE);
    internal void ShowGameMenu() => m_gameManager.SetState(GameManager.eStateGame.GAME_STARTED);

    internal void LoadLevelMoves() => m_gameManager.LoadNewGameplay();
    internal void LoadLevelTimer() => m_gameManager.LoadNewGameplay();

    internal void LoadNewGameplay(bool autoplay = false, bool autoLose = false, bool timeAttack = false)
        => m_gameManager.LoadNewGameplay(autoplay, autoLose, timeAttack);

    internal Text GetLevelConditionView()
    {
        var game = m_menuList.Where(x => x is UIPanelGame).Cast<UIPanelGame>().FirstOrDefault();
        return game ? game.LevelConditionView : null;
    }

    internal GameManager GetGameManager() => m_gameManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_gameManager.State == GameManager.eStateGame.GAME_STARTED)
                m_gameManager.SetState(GameManager.eStateGame.PAUSE);
            else if (m_gameManager.State == GameManager.eStateGame.PAUSE)
                m_gameManager.SetState(GameManager.eStateGame.GAME_STARTED);
        }

        // phím tắt test
        if (Input.GetKeyDown(KeyCode.A)) m_gameManager.LoadNewGameplay(autoplay: true);
        if (Input.GetKeyDown(KeyCode.L)) m_gameManager.LoadNewGameplay(autoLose: true);

    }
}

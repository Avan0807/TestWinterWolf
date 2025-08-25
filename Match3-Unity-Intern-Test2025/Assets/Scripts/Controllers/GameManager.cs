using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };

    public enum eLevelMode { TIMER, MOVES }
    public enum eStateGame { SETUP, MAIN_MENU, GAME_STARTED, PAUSE, GAME_OVER }

    private eStateGame m_state;
    public eStateGame State
    {
        get => m_state;
        private set { m_state = value; StateChangedAction(m_state); }
    }

    private GameSettings m_gameSettings;

    private BoardController m_boardController;
    private LevelCondition m_levelCondition;

    private BottomMatchController m_bottomController;
    public bool LastWin { get; private set; }

    private UIMainManager m_uiMenu;

    private void Awake()
    {
        State = eStateGame.SETUP;
        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);

        m_uiMenu = FindObjectOfType<UIMainManager>();
        m_uiMenu.Setup(this);
    }

    private void Start() => State = eStateGame.MAIN_MENU;

    private void Update()
    {
        if (m_boardController != null) m_boardController.Update();
    }

    internal void SetState(eStateGame state)
    {
        State = state;
        if (State == eStateGame.PAUSE) DOTween.PauseAll();
        else DOTween.PlayAll();
    }

    public void LoadLevel(eLevelMode mode)
    {
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings);

        if (mode == eLevelMode.MOVES)
        {
            m_levelCondition = gameObject.AddComponent<LevelMoves>();
            m_levelCondition.Setup(m_gameSettings.LevelMoves, m_uiMenu.GetLevelConditionView(), m_boardController);
        }
        else
        {
            m_levelCondition = gameObject.AddComponent<LevelTime>();
            m_levelCondition.Setup(m_gameSettings.LevelMoves, m_uiMenu.GetLevelConditionView(), this);
        }

        m_levelCondition.ConditionCompleteEvent += GameOver;
        State = eStateGame.GAME_STARTED;
    }

    public void GameOver() => StartCoroutine(WaitBoardController());

    private IEnumerator WaitBoardController()
    {
        while (m_boardController != null && m_boardController.IsBusy)
            yield return null;

        yield return new WaitForSeconds(1f);
        State = eStateGame.GAME_OVER;

        if (m_levelCondition != null)
        {
            m_levelCondition.ConditionCompleteEvent -= GameOver;
            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
    }

    public void LoadNewGameplay(bool autoplay = false, bool autoLose = false, bool timeAttack = false, float stepDelay = 0.5f)
    {
        ClearLevel();

        m_bottomController = new GameObject("BottomMatchController").AddComponent<BottomMatchController>();
        m_bottomController.transform.SetParent(transform, false);
        m_bottomController.StartGame(this, m_gameSettings, timeAttack);
        m_bottomController.OnGameFinished += HandleBottomFinished;

        SetState(eStateGame.GAME_STARTED);

        if (autoplay) StartCoroutine(m_bottomController.AutoPlay(stepDelay));
        else if (autoLose) StartCoroutine(m_bottomController.AutoLose(stepDelay));
    }

    private void HandleBottomFinished(bool win)
    {
        LastWin = win;
        Debug.Log("[Bottom] Finished -> LastWin = " + win);
        StartCoroutine(WaitBottomController());
    }

    private IEnumerator WaitBottomController()
    {
        yield return new WaitForSeconds(0.2f);
        SetState(eStateGame.GAME_OVER);

        if (m_bottomController != null)
        {
            m_bottomController.OnGameFinished -= HandleBottomFinished;
            m_bottomController.Clear();
            Destroy(m_bottomController.gameObject);
            m_bottomController = null;
        }
    }

    internal void ClearLevel()
    {
        if (m_boardController)
        {
            m_boardController.Clear();
            Destroy(m_boardController.gameObject);
            m_boardController = null;
        }

        if (m_bottomController)
        {
            m_bottomController.Clear();
            Destroy(m_bottomController.gameObject);
            m_bottomController = null;
        }
    }
}

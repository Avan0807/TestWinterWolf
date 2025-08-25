using UnityEngine;
using UnityEngine.UI;

public class UIPanelGame : MonoBehaviour, IMenu
{
    [Header("HUD")]
    public Text LevelConditionView;     

    [SerializeField] private Button btnPause;

    // >>> Thêm 3 nút
    [SerializeField] private Button btnAutoplay;
    [SerializeField] private Button btnAutoLose;
    [SerializeField] private Button btnTimeAttack;

    private UIMainManager m_mngr;

    private void Awake()
    {
        if (btnPause) btnPause.onClick.AddListener(() => m_mngr.ShowPauseMenu());

        // Các chế độ
        if (btnAutoplay) btnAutoplay.onClick.AddListener(() => m_mngr.LoadNewGameplay(autoplay: true));
        if (btnAutoLose) btnAutoLose.onClick.AddListener(() => m_mngr.LoadNewGameplay(autoLose: true));
        if (btnTimeAttack) btnTimeAttack.onClick.AddListener(() => m_mngr.LoadNewGameplay(timeAttack: true));
    }

    private void OnDestroy()
    {
        if (btnPause) btnPause.onClick.RemoveAllListeners();
        if (btnAutoplay) btnAutoplay.onClick.RemoveAllListeners();
        if (btnAutoLose) btnAutoLose.onClick.RemoveAllListeners();
        if (btnTimeAttack) btnTimeAttack.onClick.RemoveAllListeners();
    }

    public void Setup(UIMainManager mngr) { m_mngr = mngr; }
    public void Show() { gameObject.SetActive(true); }
    public void Hide() { gameObject.SetActive(false); }
}

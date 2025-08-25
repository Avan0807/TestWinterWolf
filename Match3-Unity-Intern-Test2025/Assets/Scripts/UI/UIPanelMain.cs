using UnityEngine;
using UnityEngine.UI;

public class UIPanelMain : MonoBehaviour, IMenu
{
    [SerializeField] private Button btnTimer;
    [SerializeField] private Button btnMoves;

    private UIMainManager m_mngr;

    private void Awake()
    {
        if (btnMoves) btnMoves.onClick.AddListener(() => m_mngr.LoadLevelMoves());
        if (btnTimer) btnTimer.onClick.AddListener(() => m_mngr.LoadLevelTimer());
    }

    private void OnDestroy()
    {
        if (btnMoves) btnMoves.onClick.RemoveAllListeners();
        if (btnTimer) btnTimer.onClick.RemoveAllListeners();
    }

    public void Setup(UIMainManager mngr) { m_mngr = mngr; }
    public void Show() { gameObject.SetActive(true); }
    public void Hide() { gameObject.SetActive(false); }
}

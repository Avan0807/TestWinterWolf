using UnityEngine;
using UnityEngine.UI;

public class UIPanelGameOver : MonoBehaviour, IMenu
{
    [SerializeField] private Button btnClose;
    [SerializeField] private Text title; 
    [SerializeField] private Text sub;   

    private UIMainManager m_mngr;

    private void Awake()
    {
        if (btnClose) btnClose.onClick.AddListener(OnClickClose);
    }

    private void OnDestroy()
    {
        if (btnClose) btnClose.onClick.RemoveAllListeners();
    }

    public void Setup(UIMainManager mngr) { m_mngr = mngr; }

    public void Show()
    {
        gameObject.SetActive(true);

        if (!title) title = GetComponentInChildren<Text>(true);

        var gm = m_mngr.GetGameManager();
        bool isWin = gm != null && gm.LastWin;

        if (title) title.text = isWin ? "LEVEL WIN" : "LEVEL LOSE";
        if (sub) sub.text = isWin ? "You cleared the board!" : "Bottom bar is full!";
    }

    public void Hide() { gameObject.SetActive(false); }

    private void OnClickClose() { m_mngr.ShowMainMenu(); }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BottomMatchController : MonoBehaviour
{
    public event Action<bool> OnGameFinished = delegate { };

    private GameManager m_gameManager;
    private GameSettings m_gameSettings;
    private Board m_board;
    private Camera m_cam;

    private const int BOTTOM_CAPACITY = 5;
    private readonly List<Item> m_bottomItems = new List<Item>();
    private readonly List<Vector3> m_bottomSlots = new List<Vector3>();
    private GameObject[] m_bottomBGs;

    private Cell[] m_allCells;
    private bool m_isBusy;
    private int m_remainingOnBoard;
    private bool m_gameEnded;

    private bool m_timeAttack;
    private float m_timeLeft;
    private Text m_timeView;
    private readonly Dictionary<Item, Cell> m_origin = new Dictionary<Item, Cell>();

    public void StartGame(GameManager gm, GameSettings settings, bool timeAttack = false)
    {
        m_gameManager = gm;
        m_gameSettings = settings;
        m_timeAttack = timeAttack;

        m_cam = Camera.main;

        m_board = new Board(transform, m_gameSettings);
        m_board.Fill();

        EnsureAllTypesPresent();

        m_allCells = GetComponentsInChildren<Cell>(true);
        m_remainingOnBoard = m_allCells.Count(c => !c.IsEmpty);

        BuildBottomRow();

        EnsureCountsAreMultiplesOf3();

        if (m_timeAttack)
        {
            m_timeLeft = 60f;
            m_timeView = FindObjectOfType<UIMainManager>()?.GetLevelConditionView();
            if (m_timeView) m_timeView.text = "60";
        }
    }

    private void EnsureAllTypesPresent()
    {
        var allTypes = (NormalItem.eNormalType[])Enum.GetValues(typeof(NormalItem.eNormalType));
        var cells = GetComponentsInChildren<Cell>(true)
                    .Where(c => !c.IsEmpty && c.Item is NormalItem).ToList();
        var present = new HashSet<NormalItem.eNormalType>(cells.Select(c => ((NormalItem)c.Item).ItemType));
        var missing = allTypes.Where(t => !present.Contains(t)).ToList();
        if (missing.Count == 0) return;

        var rnd = new System.Random();
        for (int i = 0; i < missing.Count; i++)
        {
            var pick = cells[rnd.Next(cells.Count)];
            var newItem = new NormalItem { ItemType = missing[i] };
            newItem.SetView();
            newItem.SetViewRoot(transform);
            newItem.SetViewPosition(pick.transform.position);

            pick.Clear();
            pick.Assign(newItem);
            pick.ApplyItemPosition(true);
        }
    }

    private void BuildBottomRow()
    {
        float y = (-m_gameSettings.BoardSizeY * 0.5f - 1f);
        m_bottomSlots.Clear();
        m_bottomBGs = new GameObject[BOTTOM_CAPACITY];

        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);

        for (int i = 0; i < BOTTOM_CAPACITY; i++)
        {
            float x = -2f + i; // -2..2
            Vector3 pos = new Vector3(x, y, 0f);
            m_bottomSlots.Add(pos);

            if (prefabBG) m_bottomBGs[i] = Instantiate(prefabBG, pos, Quaternion.identity, transform);
        }
    }

    private void EnsureCountsAreMultiplesOf3()
    {
        var groups = GetComponentsInChildren<Cell>(true)
            .Where(c => !c.IsEmpty && c.Item is NormalItem)
            .GroupBy(c => ((NormalItem)c.Item).ItemType)
            .ToDictionary(g => g.Key, g => g.ToList());

        int totalRemainder = groups.Values.Select(l => l.Count % 3).Sum();
        if (totalRemainder == 0) return;

        var dominant = groups.OrderByDescending(kv => kv.Value.Count).First();
        var dominantType = dominant.Key;

        foreach (var kv in groups)
        {
            int r = kv.Value.Count % 3;
            if (r == 0) continue;

            for (int i = 0; i < r; i++)
            {
                var cell = kv.Value[i];
                var newItem = new NormalItem { ItemType = dominantType };
                newItem.SetView();
                newItem.SetViewPosition(cell.transform.position);
                newItem.SetViewRoot(transform);

                cell.Clear();
                cell.Assign(newItem);
                cell.ApplyItemPosition(true);
            }
        }
    }

    private void Update()
    {
        if (m_isBusy) return;
        if (m_gameManager.State != GameManager.eStateGame.GAME_STARTED) return;

        if (m_timeAttack && !m_gameEnded)
        {
            m_timeLeft -= Time.deltaTime;
            if (m_timeView) m_timeView.text = Mathf.CeilToInt(Mathf.Max(0f, m_timeLeft)).ToString();
            if (m_timeLeft <= 0f)
            {
                m_gameEnded = true;
                FinishGame(false);
                return;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                var cell = hit.collider.GetComponent<Cell>();
                if (cell != null && !cell.IsEmpty)
                {
                    MoveCellToBottom(cell);
                    return;
                }
            }

            if (m_timeAttack)
            {
                Vector2 mouse = m_cam.ScreenToWorldPoint(Input.mousePosition);
                var pick = m_bottomItems
                    .OrderBy(it => ((Vector2)it.View.position - mouse).sqrMagnitude)
                    .FirstOrDefault();

                if (pick != null && (((Vector2)pick.View.position - mouse).magnitude <= 0.7f))
                    ReturnItemToBoard(pick);
            }
        }
    }

    private void MoveCellToBottom(Cell cell)
    {
        if (m_gameEnded) return;

        if (!m_timeAttack && m_bottomItems.Count >= BOTTOM_CAPACITY)
        {
            Debug.Log($"[Bottom] LOSE: bottom={m_bottomItems.Count}/5, remaining={m_remainingOnBoard}");
            FinishGame(false);
            return;
        }

        Item item = cell.Item;

        if (!m_origin.ContainsKey(item)) m_origin[item] = cell;

        cell.Free();
        if (item != null) item.SetCell(null);
        m_remainingOnBoard = Mathf.Max(0, m_remainingOnBoard - 1);

        int slotIndex = GetFirstFreeBottomIndex();
        Vector3 slotPos = m_bottomSlots[slotIndex];

        m_isBusy = true;
        item.View.DOMove(slotPos, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            if (m_gameEnded) { m_isBusy = false; return; }

            m_bottomItems.Add(item);

            CheckBottomForTriples();

            if (!m_timeAttack && m_bottomItems.Count >= BOTTOM_CAPACITY)
            {
                m_gameEnded = true;
                Debug.Log($"[Bottom] LOSE: bottom={m_bottomItems.Count}/5, remaining={m_remainingOnBoard}");
                FinishGame(false);
                m_isBusy = false;
                return;
            }

            if (m_remainingOnBoard <= 0 && m_bottomItems.Count == 0)
            {
                m_gameEnded = true;
                FinishGame(true);
                m_isBusy = false;
                return;
            }

            m_isBusy = false;
        });
    }

    private void ReturnItemToBoard(Item item)
    {
        if (!m_origin.ContainsKey(item)) return;
        var home = m_origin[item];
        if (!home.IsEmpty) return;

        m_isBusy = true;
        item.View.DOMove(home.transform.position, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            m_bottomItems.Remove(item);
            home.Assign(item);
            item.SetCell(home);
            home.ApplyItemPosition(true);
            m_remainingOnBoard++;

            for (int i = 0; i < m_bottomItems.Count; i++)
                m_bottomItems[i].View.DOMove(m_bottomSlots[i], 0.15f);

            m_isBusy = false;
        });
    }

    private int GetFirstFreeBottomIndex()
    {
        for (int i = 0; i < BOTTOM_CAPACITY; i++)
        {
            Vector3 p = m_bottomSlots[i];
            bool occupied = m_bottomItems.Any(it => (it.View.position - p).sqrMagnitude < 0.0001f);
            if (!occupied) return i;
        }
        return 0;
    }

    private void CheckBottomForTriples()
    {
        var groups = m_bottomItems.Where(it => it is NormalItem)
                                  .GroupBy(it => ((NormalItem)it).ItemType);

        List<Item> toRemove = new List<Item>();
        foreach (var g in groups) if (g.Count() == 3) toRemove.AddRange(g);

        if (toRemove.Count > 0)
        {
            foreach (var it in toRemove)
            {
                if (it.View) it.View.DOScale(0f, 0.15f).OnComplete(() =>
                {
                    if (it.View) Destroy(it.View.gameObject);
                });

                m_bottomItems.Remove(it);
            }

            for (int i = 0; i < m_bottomItems.Count; i++)
                m_bottomItems[i].View.DOMove(m_bottomSlots[i], 0.15f);
        }

        if (!m_gameEnded && m_remainingOnBoard <= 0 && m_bottomItems.Count == 0)
        {
            m_gameEnded = true;
            FinishGame(true);
        }
    }

    public IEnumerator AutoPlay(float stepDelay)
    {
        while (m_remainingOnBoard > 0)
        {
            m_allCells = GetComponentsInChildren<Cell>(true).Where(c => !c.IsEmpty).ToArray();

            var grp = m_allCells.Where(c => c.Item is NormalItem)
                                .GroupBy(c => ((NormalItem)c.Item).ItemType)
                                .OrderByDescending(g => g.Count())
                                .FirstOrDefault();

            var picks = (grp != null && grp.Count() >= 3) ? grp.Take(3)
                                                          : m_allCells.Take(Mathf.Min(3, m_allCells.Length));

            foreach (var c in picks)
            {
                MoveCellToBottom(c);
                yield return new WaitForSeconds(stepDelay);
                if (m_remainingOnBoard <= 0 || (!m_timeAttack && m_bottomItems.Count >= BOTTOM_CAPACITY)) yield break;
            }
        }
    }

    public IEnumerator AutoLose(float stepDelay)
    {
        var pool = GetComponentsInChildren<Cell>(true).Where(c => !c.IsEmpty && c.Item is NormalItem).ToArray();
        var buckets = pool.GroupBy(c => ((NormalItem)c.Item).ItemType)
                          .ToDictionary(g => g.Key, g => new Queue<Cell>(g));

        var used = new HashSet<NormalItem.eNormalType>();
        while (m_bottomItems.Count < BOTTOM_CAPACITY && m_remainingOnBoard > 0)
        {
            var pickType = buckets.Keys.FirstOrDefault(t => !used.Contains(t) && buckets[t].Count > 0);
            Cell pick = EqualityComparer<NormalItem.eNormalType>.Default.Equals(pickType, default)
                        ? pool.FirstOrDefault(c => !c.IsEmpty)
                        : buckets[pickType].Dequeue();

            if (pick == null) break;
            used.Add(((NormalItem)pick.Item).ItemType);

            MoveCellToBottom(pick);
            yield return new WaitForSeconds(stepDelay);
        }
    }

    private void FinishGame(bool win) => OnGameFinished(win);

    public void Clear()
    {
        if (m_board != null) { m_board.Clear(); m_board = null; }

        if (m_bottomBGs != null)
        {
            foreach (var go in m_bottomBGs) if (go) Destroy(go);
            m_bottomBGs = null;
        }

        foreach (var it in m_bottomItems.ToArray())
            if (it != null && it.View != null) Destroy(it.View.gameObject);

        m_bottomItems.Clear();
        m_origin.Clear();
    }
}

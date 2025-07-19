using UnityEngine;
using UnityEngine.UI;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance;

    [Header("Hiển thị")]
    public Text timerText;

    private float timeRemaining;
    private bool isQuestActive = false;
    private bool hasWon = false;

    private int rewardExp = 100;
    private int rewardGold = 50;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!isQuestActive || hasWon) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isQuestActive = false;
            GameManager.Instance?.ShowLoseCanvas();
        }

        UpdateTimerUI();
    }

    public void StartQuest(float customTimeLimit = 30f, int exp = 100, int gold = 50)
    {
        timeRemaining = customTimeLimit;
        rewardExp = exp;
        rewardGold = gold;

        isQuestActive = true;
        hasWon = false;
        UpdateTimerUI();
    }

    public void OnLevelWin()
    {
        if (!isQuestActive || hasWon) return;

        hasWon = true;
        isQuestActive = false;

        if (timeRemaining > 0)
        {
            GrantReward();
        }
    }

    void GrantReward()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddExp(rewardExp);
            PlayerStats.Instance.AddGold(rewardGold);
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.SyncCurrency(PlayerStats.Instance.GetGold());
            }
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time in: " + Mathf.CeilToInt(timeRemaining).ToString() + "s";
        }
    }

    public bool IsQuestActive()
    {
        return isQuestActive;
    }
}
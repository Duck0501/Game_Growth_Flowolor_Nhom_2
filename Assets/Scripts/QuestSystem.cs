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
        }

        UpdateTimerUI();
    }

    /// <summary>
    /// Gọi từ GameManager khi load level
    /// </summary>
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
        Debug.Log($"✅ Nhận thưởng: +{rewardExp} EXP, +{rewardGold} Gold");

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddExp(rewardExp);
            PlayerStats.Instance.AddGold(rewardGold);
        }
    }


    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time in: " + Mathf.CeilToInt(timeRemaining).ToString() + "s";
        }
    }
}

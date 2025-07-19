using UnityEngine;
using UnityEngine.UI;
using System;

public class DailyBonus : MonoBehaviour
{
    [Header("UI Elements")]
    public Button exitButton;
    public Button collectButton;
    public Image[] rewardImages;

    private int consecutiveDays;
    private DateTime lastLoginDate;
    private bool[] rewardsClaimed = new bool[5];
    private readonly int[] rewards = { 100, 100, 100, 100, 500 };

    private readonly Color availableColor = Color.red;
    private readonly Color claimedColor = Color.gray;
    private readonly Color lockedColor = Color.blue;

    void Start()
    {
        LoadData();
        UpdateConsecutiveDays();
        UpdateUI();
        SetupButtons();
    }

    void LoadData()
    {
        string lastLoginStr = PlayerPrefs.GetString("LastLoginDate", "");
        if (!string.IsNullOrEmpty(lastLoginStr))
        {
            lastLoginDate = DateTime.Parse(lastLoginStr);
        }
        else
        {
            lastLoginDate = DateTime.Today.AddDays(-1);
        }

        consecutiveDays = PlayerPrefs.GetInt("ConsecutiveDays", 0);

        for (int i = 0; i < rewardsClaimed.Length; i++)
        {
            rewardsClaimed[i] = PlayerPrefs.GetInt($"RewardClaimed_Day{i}", 0) == 1;
        }
    }

    void UpdateConsecutiveDays()
    {
        DateTime today = DateTime.Today;
        if (lastLoginDate.Date == today.AddDays(-1).Date)
        {
            consecutiveDays = Mathf.Min(consecutiveDays + 1, 5);
        }
        else if (lastLoginDate.Date < today.AddDays(-1).Date)
        {
            consecutiveDays = 1;
            for (int i = 0; i < rewardsClaimed.Length; i++)
            {
                rewardsClaimed[i] = false;
            }
        }

        lastLoginDate = today;
        PlayerPrefs.SetString("LastLoginDate", lastLoginDate.ToString("yyyy-MM-dd"));
        PlayerPrefs.SetInt("ConsecutiveDays", consecutiveDays);
        PlayerPrefs.Save();
    }

    void UpdateUI()
    {
        for (int i = 0; i < rewardImages.Length; i++)
        {
            if (rewardImages[i] == null) continue;

            if (rewardsClaimed[i])
            {
                rewardImages[i].color = claimedColor;
            }
            else if (i < consecutiveDays)
            {
                rewardImages[i].color = availableColor;
            }
            else
            {
                rewardImages[i].color = lockedColor;
            }

            Text rewardText = rewardImages[i].GetComponentInChildren<Text>();
            if (rewardText != null)
            {
                rewardText.text = $"+{rewards[i]}$";
            }
        }

        if (collectButton != null)
        {
            collectButton.interactable = !rewardsClaimed[consecutiveDays - 1] && consecutiveDays > 0;
        }
    }

    void SetupButtons()
    {
        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnBonusCanvasHidden();
                }
            });
        }

        if (collectButton != null)
        {
            collectButton.onClick.RemoveAllListeners();
            collectButton.onClick.AddListener(() =>
            {
                ClaimReward();
                gameObject.SetActive(false);
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnBonusCanvasHidden();
                }
            });
        }
    }

    void ClaimReward()
    {
        if (consecutiveDays <= 0 || rewardsClaimed[consecutiveDays - 1]) return;

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddGold(rewards[consecutiveDays - 1]);
        }

        rewardsClaimed[consecutiveDays - 1] = true;
        PlayerPrefs.SetInt($"RewardClaimed_Day{consecutiveDays - 1}", 1);

        if (consecutiveDays >= 5)
        {
            consecutiveDays = 0;
            for (int i = 0; i < rewardsClaimed.Length; i++)
            {
                rewardsClaimed[i] = false;
                PlayerPrefs.SetInt($"RewardClaimed_Day{i}", 0);
            }
            PlayerPrefs.SetInt("ConsecutiveDays", consecutiveDays);
        }

        PlayerPrefs.Save();
        UpdateUI();
    }
}
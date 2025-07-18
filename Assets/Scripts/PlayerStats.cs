using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("UI Text Legacy")]
    public Text goldText;
    public Text expText;
    public Text levelText;

    private int gold = 0;
    private int exp = 0;
    private int level = 1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }

    public void AddExp(int amount)
    {
        exp += amount;

        while (exp >= ExpRequiredForLevel(level))
        {
            exp -= ExpRequiredForLevel(level);
            level++;
        }

        UpdateUI();
    }

    int ExpRequiredForLevel(int lv)
    {
        return lv * 100;
    }

    void UpdateUI()
    {
        if (goldText) goldText.text = "Gold: " + gold;
        if (expText) expText.text = "Exp: " + exp + " / " + ExpRequiredForLevel(level);
        if (levelText) levelText.text = "Level: " + level;
    }
}

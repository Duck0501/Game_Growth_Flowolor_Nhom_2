using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public Button button;
        public Mesh mesh;
        public int price = 100;
        public bool isPurchased = false;
        public bool isEquipped = false;
    }

    [Header("Shop Settings")]
    public List<ShopItem> shopItems = new List<ShopItem>();
    public Text currencyText;
    public GameObject appearObject;
    private int playerCurrency = 0;
    private ShopItem equippedItem;

    public static ShopManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeShop();
        UpdateCurrencyText();
    }

    void InitializeShop()
    {
        if (shopItems.Count > 0)
        {
            shopItems[0].isPurchased = true;
            shopItems[0].isEquipped = true;
            equippedItem = shopItems[0];
        }

        for (int i = 0; i < shopItems.Count; i++)
        {
            int index = i;
            ShopItem item = shopItems[i];

            Text buttonText = item.button.GetComponentInChildren<Text>();
            UpdateButtonText(item, buttonText);

            item.button.onClick.RemoveAllListeners();
            item.button.onClick.AddListener(() => OnShopButtonClick(index));
        }
    }

    void UpdateButtonText(ShopItem item, Text buttonText)
    {
        if (buttonText == null) return;

        if (item.isEquipped)
        {
            buttonText.text = "Equipped";
        }
        else if (item.isPurchased)
        {
            buttonText.text = "Use";
        }
        else
        {
            buttonText.text = $"{item.price}$";
        }
    }

    void OnShopButtonClick(int index)
    {
        ShopItem item = shopItems[index];
        Text buttonText = item.button.GetComponentInChildren<Text>();

        if (!item.isPurchased)
        {
            if (playerCurrency >= item.price)
            {
                playerCurrency -= item.price;
                item.isPurchased = true;
                UpdateCurrencyText();
                UpdateButtonText(item, buttonText);
                if (PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.AddGold(-item.price);
                }
            }
        }
        else if (!item.isEquipped)
        {
            EquipItem(item);
        }
    }

    void EquipItem(ShopItem item)
    {
        if (equippedItem != null)
        {
            equippedItem.isEquipped = false;
            Text prevButtonText = equippedItem.button.GetComponentInChildren<Text>();
            UpdateButtonText(equippedItem, prevButtonText);
        }

        item.isEquipped = true;
        equippedItem = item;
        UpdateButtonText(item, item.button.GetComponentInChildren<Text>());

        if (appearObject != null)
        {
            MeshFilter meshFilter = appearObject.GetComponent<MeshFilter>();
            if (meshFilter != null && item.mesh != null)
            {
                meshFilter.mesh = item.mesh;
            }
        }
    }

    void UpdateCurrencyText()
    {
        if (currencyText != null)
        {
            currencyText.text = $"{playerCurrency}$";
        }
    }

    public void SetAppearObject(GameObject obj)
    {
        appearObject = obj;
        if (equippedItem != null && appearObject != null)
        {
            MeshFilter meshFilter = appearObject.GetComponent<MeshFilter>();
            if (meshFilter != null && equippedItem.mesh != null)
            {
                meshFilter.mesh = equippedItem.mesh;
            }
        }
    }

    public void SyncCurrency(int newCurrency)
    {
        playerCurrency = newCurrency;
        UpdateCurrencyText();
    }
}
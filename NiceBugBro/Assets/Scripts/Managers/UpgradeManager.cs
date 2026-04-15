using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [SerializeField] private GameObject upgradeMenu;
    [SerializeField] private GameObject upgradePanelPrefab;
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private List<Upgrade> upgrades;
    private List<Upgrade> thisRunUpgrades;


    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        thisRunUpgrades = new List<Upgrade>(upgrades);
        
    }

    private void OnEnable()
    {
        EventManager.CollectedUpgrade += ShowUpgradeMenu;
    }

    private void OnDisable()
    {
        EventManager.CollectedUpgrade -= ShowUpgradeMenu;
    }

    private void ShowUpgradeMenu()
    {
        upgradeMenu.SetActive(true);
        PopulateUpgradeMenu();
    }

    private void PopulateUpgradeMenu()
    {
        GameObject leftUpgrade = Instantiate(upgradePanelPrefab, leftPanel.transform);
        TMP_Text upgradeTitle = leftUpgrade.transform.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text upgradeDescription = leftUpgrade.transform.GetChild(1).GetComponent<TMP_Text>();
        UpgradeButton upgradeButton = leftUpgrade.transform.GetChild(2).GetComponent<UpgradeButton>();

        Upgrade randomUpgrade = thisRunUpgrades[Random.Range(0, thisRunUpgrades.Count)];
        upgradeTitle.text = randomUpgrade.upgradeName;
        upgradeDescription.text = randomUpgrade.description;
        upgradeButton.Initialize(randomUpgrade);
        thisRunUpgrades.Remove(randomUpgrade);

        GameObject rightUpgrade = Instantiate(upgradePanelPrefab, rightPanel.transform);
        TMP_Text upgradeTitle2 = rightUpgrade.transform.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text upgradeDescription2 = rightUpgrade.transform.GetChild(1).GetComponent<TMP_Text>();
        UpgradeButton upgradeButton2 = rightUpgrade.transform.GetChild(2).GetComponent<UpgradeButton>();

        Upgrade randomUpgrade2 = thisRunUpgrades[Random.Range(0, thisRunUpgrades.Count)];
        upgradeTitle2.text = randomUpgrade2.upgradeName;
        upgradeDescription2.text = randomUpgrade2.description;
        upgradeButton2.Initialize(randomUpgrade2);
        thisRunUpgrades.Remove(randomUpgrade2);
    }
}
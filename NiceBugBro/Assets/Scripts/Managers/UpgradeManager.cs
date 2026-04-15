using System.Collections;
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
        if (thisRunUpgrades.Count < 2)
        {
            Debug.LogError("Not enough upgrades");
            return;
        }
        GameObject leftUpgrade = Instantiate(upgradePanelPrefab, leftPanel.transform);
        UpgradePanelScript leftUpgradePanel = leftUpgrade.GetComponent<UpgradePanelScript>();

        Upgrade randomUpgrade = thisRunUpgrades[Random.Range(0, thisRunUpgrades.Count)];
        leftUpgradePanel.Initialize(randomUpgrade);
        thisRunUpgrades.Remove(randomUpgrade);

        GameObject rightUpgrade = Instantiate(upgradePanelPrefab, rightPanel.transform);
        UpgradePanelScript rightUpgradePanel = rightUpgrade.GetComponent<UpgradePanelScript>();

        Upgrade randomUpgrade2 = thisRunUpgrades[Random.Range(0, thisRunUpgrades.Count)];
        rightUpgradePanel.Initialize(randomUpgrade2);
        thisRunUpgrades.Remove(randomUpgrade2);
    }
    

}
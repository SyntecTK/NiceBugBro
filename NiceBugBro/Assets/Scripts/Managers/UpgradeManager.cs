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
    [SerializeField] private GameObject currentLeftPanel;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject currentRightPanel;
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
        currentLeftPanel = Instantiate(upgradePanelPrefab, leftPanel.transform);
        UpgradePanelScript leftUpgradePanelScript = currentLeftPanel.GetComponent<UpgradePanelScript>();

        Upgrade leftUpgrade = thisRunUpgrades[Random.Range(0, thisRunUpgrades.Count)];
        leftUpgradePanelScript.Initialize(leftUpgrade);

        currentRightPanel = Instantiate(upgradePanelPrefab, rightPanel.transform);
        UpgradePanelScript rightUpgradePanelScript = currentRightPanel.GetComponent<UpgradePanelScript>();

        Upgrade rightUpgrade = thisRunUpgrades[Random.Range(0, thisRunUpgrades.Count)];
        rightUpgradePanelScript.Initialize(rightUpgrade);
    }


    public void ExitUpgradeMenu(Upgrade upgrade)
    {
        if(upgrade.unique) thisRunUpgrades.Remove(upgrade);
        Destroy(currentLeftPanel);
        Destroy(currentRightPanel);
    }
    

}
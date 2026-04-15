using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject upgradeMenu;
    [SerializeField] private GameObject upgradePanelPrefab;
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private List<Upgrade> upgrades;

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

        Upgrade randomUpgrade = upgrades[Random.Range(0, upgrades.Count)];
        upgradeTitle.text = randomUpgrade.upgradeName;
        upgradeDescription.text = randomUpgrade.description;

        GameObject rightUpgrade = Instantiate(upgradePanelPrefab, rightPanel.transform);
        TMP_Text upgradeTitle2 = rightUpgrade.transform.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text upgradeDescription2 = rightUpgrade.transform.GetChild(1).GetComponent<TMP_Text>();

        Upgrade randomUpgrade2 = upgrades[Random.Range(0, upgrades.Count)];
        upgradeTitle2.text = randomUpgrade2.upgradeName;
        upgradeDescription2.text = randomUpgrade2.description;
    }

}
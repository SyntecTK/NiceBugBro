using TMPro;
using UnityEngine;

public class UpgradePanelScript : MonoBehaviour
{
    [SerializeField] private Upgrade _upgrade;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text alienDescription;

    public void Initialize(Upgrade upgrade)
    {
        _upgrade = upgrade;
        title.text = _upgrade.upgradeName;
        description.text = _upgrade.description;
        alienDescription.text = _upgrade.description;
    }

    public void ButtonPressed()
    {
        GameManager.Instance.UpgradeChosen(_upgrade);
    }
}

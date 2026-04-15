using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    public Upgrade _upgrade;

    public void Initialize(Upgrade upgrade)
    {
        _upgrade = upgrade;
    }

    public void ButtonPressed()
    {
        GameManager.Instance.UpgradeChosen(_upgrade);
    }
}

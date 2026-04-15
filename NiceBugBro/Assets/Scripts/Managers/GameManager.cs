using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _upgradeScreen;

    private void Awake()
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
    }

    public void EnterUpgradeMode()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _upgradeScreen.SetActive(true);
    }

    public void ExitUpgradeMode()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _upgradeScreen.SetActive(false);
    }

    public void UpgradeChosen(Upgrade upgrade)
    {
        //TODO: Hier upgrade anwenden auf player
        if (upgrade.playerSpeedUpgrade != 0) PlayerController.Instance.UpgradePlayerSpeed(upgrade.playerSpeedUpgrade);
        if (upgrade.bulletSpeedUpgrade != 0) PlayerController.Instance.UpgradeBulletSpeed(upgrade.bulletSpeedUpgrade);
        if (upgrade.bulletDamageUpgrade != 0) PlayerController.Instance.UpgradeBulletDamage(upgrade.bulletDamageUpgrade);
        if (upgrade.healthUpgrade != 0) PlayerController.Instance.UpgradeHealth(upgrade.healthUpgrade);
        if (upgrade.jumpUpgrade != 0) PlayerController.Instance.UpgradeJump(upgrade.jumpUpgrade);

        if (upgrade.minimap) PlayerController.Instance.MinimapUpgrade();
        if (upgrade.fiveShot) PlayerController.Instance.FiveShotUpgrade();

        ExitUpgradeMode();
    }
}
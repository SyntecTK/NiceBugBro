using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _upgradeScreen;
    [SerializeField] private GameObject _gameOverScreen;

    [Header("UI References")]
    [SerializeField] private TMP_Text timeTXT;
    [SerializeField] private TMP_Text healthTXT;
    [SerializeField] private TMP_Text killsTXT;

    private int killCount;
    private bool isMovementLocked;
    public bool IsMovementLocked => isMovementLocked;
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

        killCount = 0;
    }

    private void FixedUpdate()
    {
        UpdateHUD();
    }

    public void EnterUpgradeMode()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _upgradeScreen.SetActive(true);
    }

    public void ExitUpgradeMode(Upgrade upgrade)
    {
        UpgradeManager.Instance.ExitUpgradeMenu(upgrade);
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
        if (upgrade.bulletLifeTimeUpgrade != 0) PlayerController.Instance.UpgradeBulletLifeTime(upgrade.bulletLifeTimeUpgrade);
        if (upgrade.healthUpgrade != 0) PlayerController.Instance.UpgradeHealth(upgrade.healthUpgrade);
        if (upgrade.jumpUpgrade != 0) PlayerController.Instance.UpgradeJump(upgrade.jumpUpgrade);

        if (upgrade.minimap) PlayerController.Instance.MinimapUpgrade();
        if (upgrade.fiveShot) PlayerController.Instance.BurstShotUpgrade();
        if (upgrade.ricochet) PlayerController.Instance.RicochetUpgrade();

        ExitUpgradeMode(upgrade);
    }

    private void UpdateHUD()
    {
        //Time Display
        float time = Time.timeSinceLevelLoad;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timeTXT.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        //Health Display
        healthTXT.text = PlayerController.Instance.CurrentHealth.ToString();
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _gameOverScreen.SetActive(true);
        isMovementLocked = true;
    }

    public void KillEnemy()
    {
        killCount++;
        killsTXT.text = killCount.ToString();
    }
}
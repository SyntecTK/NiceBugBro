using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _upgradeScreen;
    [SerializeField] private GameObject _gameOverScreen;

    [Header("UI References")]
    [SerializeField] private TMP_Text timeTXT;
    [SerializeField] private TMP_Text healthTXT;
    [SerializeField] private TMP_Text killsTXT;

    [SerializeField] private TMP_Text endGameKillsTXT;
    [SerializeField] private TMP_Text endGameTimeTXT;

    private PlayerController player;

    private string currentTime;

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
        }

        killCount = 0;
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
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
        AudioManager.Instance.Play2DSound(SoundType.PowerUp);
    }

    public void ExitUpgradeMode(Upgrade upgrade)
    {
        UpgradeManager.Instance.ExitUpgradeMenu(upgrade);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _upgradeScreen.SetActive(false);
    }

    public void UpdateHUD()
    {
        //Time Display
        float time = Time.timeSinceLevelLoad;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        currentTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        timeTXT.text = currentTime;

        //Health Display
        if (player != null)
        {
            healthTXT.text = player.CurrentHealth.ToString();
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _gameOverScreen.SetActive(true);
        isMovementLocked = true;

        endGameTimeTXT.text = currentTime;
        endGameKillsTXT.text = killCount.ToString();

    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isMovementLocked = false;
        SceneManager.LoadScene(0);
    }

    public void KillEnemy()
    {
        killCount++;
        killsTXT.text = killCount.ToString();
    }

    public void UpgradeChosen(Upgrade upgrade)
    {
        ExitUpgradeMode(upgrade);
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public bool PlayerExists()
    {
        return player != null;
    }
}
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrades/Upgrade")]
public class Upgrade : ScriptableObject
{
    [Header("General")]
    public string upgradeName;
    public string description;
    public bool unique;
    
    
    [Header("Stats")]
    public int playerSpeedUpgrade;
    public float playerLookSensitivityUpgrade;
    public int bulletSpeedUpgrade;
    public int bulletDamageUpgrade;
    public float bulletLifeTimeUpgrade;
    public int healthUpgrade;
    public int jumpUpgrade;
    public float gravityUpgrade;

    [Header("Gameplay Changes")]
    public bool minimap;
    public bool fiveShot;
    public bool ricochet;
    public bool spreadShot;
    
}
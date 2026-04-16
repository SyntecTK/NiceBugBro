using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrades/Upgrade")]
public class Upgrade : ScriptableObject
{
    [Header("General")]
    public string upgradeName;
    public string description;
    public bool unique;
    
    
    [Header("Stats")]
    public int playerSpeedUpgrade;
    public int bulletSpeedUpgrade;
    public int bulletDamageUpgrade;
    public float bulletLifeTimeUpgrade;
    public int healthUpgrade;
    public int jumpUpgrade;

    [Header("Gameplay Changes")]
    public bool minimap;
    public bool fiveShot;
    public bool ricochet;
    
}
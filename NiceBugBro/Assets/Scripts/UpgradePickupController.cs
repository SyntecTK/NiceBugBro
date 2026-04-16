using UnityEngine;

public class UpgradePickupController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.OnCollectedUpgrade();
            GameManager.Instance.EnterUpgradeMode();
            Destroy(this.gameObject);
        }
    }
}

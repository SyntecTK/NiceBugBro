using UnityEngine;

public class MapBarrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBody"))
        {
            other.transform.position = new Vector3(other.transform.position.x, 200, other.transform.position.z);
        }
    }
}

using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action CollectedUpgrade;

    public static void OnCollectedUpgrade()
    {
        CollectedUpgrade?.Invoke();
    }
}

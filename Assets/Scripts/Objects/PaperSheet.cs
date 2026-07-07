using UnityEngine;
using Ubiq.Spawning;
using Ubiq.Messaging;

/// <summary>
/// Paper Sheet behaviour
/// </summary>
public class PaperSheet : MonoBehaviour, INetworkSpawnable
{
    public NetworkId NetworkId { get; set; }
    // It is an interactable component, that interacts with key to open
    private GameObject _lockObject;
    

    void Start()
    {
    }

}
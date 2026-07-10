using UnityEngine;
using Ubiq.Messaging;

/// <summary>
/// Spawns specified element
/// </summary>
public class Notepad : MonoBehaviour
{
    private Spawner _paperSpawner;
    private NetworkContext _context;

    void Start()
    {
        _context = NetworkScene.Register(this);
        _paperSpawner = GetComponent<Spawner>();
    }

    // ----------------------------------------
    // Network control

    // Message to synchronize selected powers across all copies
    private struct UpdateMsg
    {
        public ScenePowerManager.Power power;
        public string paperOwner;
    }
    public void SendUpdateMessage( ScenePowerManager.Power power, string paperOwner)
    {   // Propagate Update on network
        var message = new UpdateMsg();
        message.power = power;
        message.paperOwner = paperOwner;
        _context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse message
        var m = message.FromJson<UpdateMsg>();
        _paperSpawner.ProcessMessage(m.power, m.paperOwner);
    }

}

using UnityEngine;
using Ubiq.Avatars;
using Ubiq.Messaging;

public class AvatarGraphicsSynchronizer : MonoBehaviour
{
    private NetworkContext _context;
    private AvatarManager _avatarManager;

    void Start()
    {
        // Initialize context
        _context = NetworkScene.Register(this);
        // Find the avatar manager
        _avatarManager = FindFirstObjectByType<AvatarManager>();
    }

    // ------------------------------------------------------------------------------------
    // Network sync management

    private struct UpdateGraphicsMsg
    {
        // The target Avatar identifier to update its graphics
        public string uuid;
        public ScenePowerManager.Power avatarPower;
        // eventual parameter
        public float param;
    }

    /// <summary>
    /// Sends message to update the graphics of the avatar with the specified uuid.
    /// The update happens as follows: the avatar power component is found and (if present)
    /// the UpdateGraphics function is called. 
    /// </summary>
    /// <param name="uuidAvatar"> uuid of the avatar that has to be updated </param>
    public void SendTrackMessage(string uuidAvatar, ScenePowerManager.Power power, float param)
    {
        var message = new UpdateGraphicsMsg();
        message.uuid = uuidAvatar;
        message.avatarPower = power;
        message.param = param;
        
        _context.SendJson(message);
    }

    // Network Ubiq: update remote avatars
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse message
        var m = message.FromJson<UpdateGraphicsMsg>();
        Debug.Log(m.uuid);
        // Find the remote avatar to change
        Ubiq.Avatars.Avatar[] avatars = _avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();
        foreach (Ubiq.Avatars.Avatar item in avatars)
        {
            if (item.Peer.uuid == m.uuid)
            {
                // Get right component and invoke method
                switch (m.avatarPower)
                {
                    case ScenePowerManager.Power.jellyVision:
                        JellyEyesScript jellyComponent = item.gameObject.GetComponent<JellyEyesScript>();
                        if (jellyComponent != null)
                        {
                            jellyComponent.UpdateGraphics(m.param);
                        }
                        break;
                    case ScenePowerManager.Power.flyingPower:
                        break;
                    case ScenePowerManager.Power.sizeManipulationPower:
                        SizeController sizeComponent = item.gameObject.GetComponent<SizeController>();
                        if (sizeComponent != null)
                        {
                            sizeComponent.UpdateGraphics(m.param);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
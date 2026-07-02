using UnityEngine;

/**
 * This class is used to track socket interactors state for Network synchronization og objects
 */
public class SocketStatusTracker : MonoBehaviour
{
    // Whether object interacting with socket has snapped into place
    private bool socketActivated = false;

    public bool isSocketActivated() {
        return socketActivated;
    }

    public void setSocketStatus(bool status) {
        socketActivated = status;
    }

}
using UnityEngine;

/// <summary>
/// This <see langword="class"/> is used to help network tracking with socket interactors   
/// </summary>
public class SocketStatusTracker : MonoBehaviour
{
    // Whether object interacting with socket has snapped into place
    private bool _socketActivated = false;

    /// <summary>
    /// Check socket interactor status
    /// </summary>
    /// <returns>True if <see langword="object"/> has snapped <see langword="into"/> the socket,
    /// <see langword="false"/> otherwise   </returns>
    public bool IsSocketActivated()
    {
        return _socketActivated;
    }

    /// <summary>
    /// Modify socket <paramref name="status"/>.
    /// </summary>
    /// <param name="status"> New socket status to be updated </param>
    public void SetSocketStatus(bool status)
    {
        _socketActivated = status;
    }

}
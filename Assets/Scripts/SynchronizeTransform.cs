using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public class SynchronizeTransform : NetworkBehaviour
{
    [ClientRpc]
    public void RpcUpdatePositionClientRpc(Vector3 position)
    {
        // Update the position and rotation of the networked object on all clients
        transform.position = position;
    }
}

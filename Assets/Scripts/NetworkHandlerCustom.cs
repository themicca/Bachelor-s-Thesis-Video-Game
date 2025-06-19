using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkHandlerCustom : NetworkBehaviour
{
    public static NetworkHandlerCustom Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (NetworkManager.Singleton.IsHost)
        {
            Utilities.SpawnNetworkObject(gameObject);
        }
    }

    [ServerRpc(RequireOwnership=false)]
    public void DisconnectPlayerServerRpc(ulong clientId)
    {
        RelayManager.Instance.DisconenctPlayer(clientId);
    }
}

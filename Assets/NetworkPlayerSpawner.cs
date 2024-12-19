using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private Transform[] spawnPoints;

    private void Start()
    {
        
        if (this.transform.childCount >= 2)
        {
            spawnPoints = new Transform[this.transform.childCount];
            for (int i = 0; i < this.transform.childCount; i++)
            {
                spawnPoints[i] = this.transform.GetChild(i);
            }
        }
        else
        {
            Debug.LogError("Not enough spawn points configured.");
            spawnPoints = new Transform[0];
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn points not initialized.");
            return;
        }

        
        int spawnIndex = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;

      
        Vector3 spawnPosition = spawnPoints[spawnIndex].position;
        Quaternion spawnRotation = spawnPoints[spawnIndex].rotation;

       
        GameObject playerPrefab = PhotonNetwork.Instantiate("NetworkPlayer", spawnPosition, spawnRotation);

       
        playerPrefab.name = "Player_" + PhotonNetwork.LocalPlayer.ActorNumber;
        PhotonNetwork.LocalPlayer.NickName = playerPrefab.name;

        Debug.Log($"Player instantiated at spawn point {spawnIndex}: {spawnPosition}");
    }
}




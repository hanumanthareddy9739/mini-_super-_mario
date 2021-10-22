using System.Collections;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Class used by the missiles generator.
/// </summary>
public class MissileGenerator : MonoBehaviourPun
{
    /// <summary>
    /// Function that generates a random position for the missiles.
    /// </summary>
    /// <returns>Vector 2 of the random position.</returns>
    Vector2 SpawnPosition()
    {
        int randonNumber = Random.Range(1, 4);

        switch (randonNumber)
        {
            case 1:
                return new Vector2(11f, 2.23f);
            case 2:
                return new Vector2(11f, -3.5f);
            case 3:
                return new Vector2(-11f, 2.23f);
            default:
                return new Vector2(-11f, -3.5f);
        }
    }

    void OnEnable()
    {
        if (NetworkManager.networkManager.playerNumber != 2)
        {
            StartCoroutine(SpawnMissiles(NetworkManager.networkManager.isConnected));
        }
    }

    /// <summary>
    /// Function that is responsible for generating missiles.
    /// </summary>
    void GenerateMissiles()
    {
        GameObject missile = ObjectPooler.SharedInstance.GetPooledObject("Game1/Missile");
        
        if (missile != null)
        {
            missile.transform.position = SpawnPosition();
            missile.transform.rotation = Quaternion.identity;
            missile.SetActive(true);
        }
    }

    /// <summary>
    /// Function that is responsible for instantiating missiles on the server.
    /// </summary>
    [PunRPC] void InstantiateMissiles(Vector2 spawnPosition)
    {
        GameObject missile = ObjectPooler.SharedInstance.GetPooledObject("Game1/Missile");

        if (missile != null)
        {
            missile.transform.position = spawnPosition;
            missile.transform.rotation = Quaternion.identity;
            missile.SetActive(true);
        }
    }

    /// <summary>
    /// Coroutine that calls the function to generate missiles after a few seconds.
    /// </summary>
    /// <param name="multiplayer">True if multiplayer is active.</param>
    /// <returns></returns>
    IEnumerator SpawnMissiles(bool multiplayer)
    {
        yield return new WaitForSeconds(3);

        while (true)
        {
            if (multiplayer)
            {
                photonView.RPC("InstantiateMissiles", RpcTarget.AllViaServer, SpawnPosition());
            }

            else
            {
                GenerateMissiles();
            }

            yield return new WaitForSeconds(Random.Range(4, 7));
        }
    }
}

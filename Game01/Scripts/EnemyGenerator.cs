using System.Collections;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Class used by the enemies generator.
/// </summary>
public class EnemyGenerator : MonoBehaviourPun
{
    void OnEnable()
    {
        if (NetworkManager.networkManager.playerNumber != 2)
        {
            StartCoroutine(Spawner(NetworkManager.networkManager.isConnected));
        }
    }

    /// <summary>
    /// Function that is responsible for generating enemies.
    /// </summary>
    void GenerateEnemy()
    {
        GameObject goomba = ObjectPooler.SharedInstance.GetPooledObject("Game1/Enemy");
        
        if (goomba != null)
        {
            goomba.transform.position = transform.position;
            goomba.transform.rotation = Quaternion.identity;
            goomba.SetActive(true);
        }
    }

    /// <summary>
    /// Function that is responsible for instantiating enemies on the server.
    /// </summary>
    [PunRPC] void InstantiateEnemy(Vector3 spawnPosition)
    {
        GameObject goomba = ObjectPooler.SharedInstance.GetPooledObject("Game1/Enemy");

        if (goomba != null)
        {
            goomba.transform.position = spawnPosition;
            goomba.transform.rotation = Quaternion.identity;
            goomba.SetActive(true);
        }
    }

    /// <summary>
    /// Coroutine that calls the function to generate enemies after a few seconds.
    /// </summary>
    /// <param name="multiplayer">True if multiplayer is active.</param>
    /// <returns></returns>
    IEnumerator Spawner(bool multiplayer)
    {
        while (true)
        {
            if (multiplayer)
            {
                photonView.RPC("InstantiateEnemy", RpcTarget.AllViaServer, transform.position);
            }

            else
            {
                GenerateEnemy();
            }

            yield return new WaitForSeconds(Random.Range(3, 6));
        }
    }
}
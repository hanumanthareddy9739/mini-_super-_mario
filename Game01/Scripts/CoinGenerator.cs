using System.Collections;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Class used by the coins generator.
/// </summary>
public class CoinGenerator : MonoBehaviour
{
    Vector2 SpawnPosition()
    {
        int randonNumber = Random.Range(1, 7);

        switch (randonNumber)
        {
            case 1:
                return new Vector2(Random.Range(-7.65f, -3.13f), 4.5f);
            case 2:
                return new Vector2(Random.Range(3.13f, 7.65f), 4.5f);
            case 3:
                return new Vector2(Random.Range(-1.75f, 1.75f), 2f);
            case 4:
                return new Vector2(Random.Range(-8.8f, -3f), -1f);
            case 5:
                return new Vector2(Random.Range(3f, 8.8f), -1f);
            case 6:
                return new Vector2(-1.75f, -3.5f);
            default:
                return new Vector2(1.75f, -3.5f);
        }
    }

    void OnEnable()
    {
        if (NetworkManager.networkManager.playerNumber != 2)
        {
            StartCoroutine(SpawnCoins(NetworkManager.networkManager.isConnected));
        }
    }

    /// <summary>
    /// Function that is responsible for generating coins.
    /// </summary>
    void GenerateCoin()
    {
        GameObject coin = ObjectPooler.SharedInstance.GetPooledObject("Game1/Coin");

        if (coin != null)
        {
            coin.transform.position = SpawnPosition();
            coin.transform.rotation = Quaternion.identity;
            coin.SetActive(true);
        }
    }

    /// <summary>
    /// Function that is responsible for instantiate coins on the server.
    /// </summary>
    void InstantiateCoin()
    {
        PhotonNetwork.InstantiateRoomObject("1Coin", SpawnPosition(), Quaternion.identity);
    }

    /// <summary>
    /// Coroutine that calls the function to generate enemies after a few seconds.
    /// </summary>
    /// <param name="multiplayer">True if multiplayer is active.</param>
    /// <returns></returns>
    IEnumerator SpawnCoins(bool multiplayer)
    {
        yield return new WaitForSeconds(1);

        while (true)
        {
            if (multiplayer)
            {
                InstantiateCoin();

                yield return new WaitForSeconds(Random.Range(3, 6));
            }

            else
            {
                GenerateCoin();

                yield return new WaitForSeconds(Random.Range(5, 10));
            }
        }
    }
}

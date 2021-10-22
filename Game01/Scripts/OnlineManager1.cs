using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Class that manages the online functions of Game 01.
/// </summary>
public class OnlineManager1 : MonoBehaviourPunCallbacks
{
    public static OnlineManager1 onlineManager;

    [Header("Panels")]
    [SerializeField] GameObject panelMenu = null;
    [SerializeField] GameObject panelPause = null;
    [SerializeField] GameObject panelLostPlayer1 = null;
    [SerializeField] GameObject waitingMessage = null;

    [Header("Generators")]
    [SerializeField] GameObject[] generators = null;
    
    [Header("Score")]
    int score1 = 0;
    [SerializeField] Text score1Text = null;
    int score2 = 0;
    [SerializeField] Text score2Text = null;

    [Header("Player")]
    GameObject player;

    [Header("Sounds")]
    [SerializeField] AudioSource hurtSound = null;
    [SerializeField] AudioSource coinSound = null;

    private void Awake()
    {
        onlineManager = this;
    }

    /// <summary>
    /// Function that starts the game online.
    /// </summary>
    public void StartGame()
    {
        photonView.RPC("CleanScore", RpcTarget.All, true);
        photonView.RPC("CleanScore", RpcTarget.All, false);

        InstantiatePlayer();

        panelMenu.SetActive(false);
    }

    /// <summary>
    /// Function that returns the player to the original position after dying.
    /// </summary>
    public void Respawn()
    {
        player = null;

        photonView.RPC("PlaySound", RpcTarget.All, "hurt");

        StartCoroutine(WaitForRespawn());
    }

    /// <summary>
    /// Function that instantiates the player in the starting position.
    /// </summary>
    void InstantiatePlayer()
    {
        if (player != null)
        {
            return;
        }

        switch (NetworkManager.networkManager.playerNumber)
        {
            case 1:
                player = PhotonNetwork.Instantiate("1Player1", new Vector2(-6.3f, -5.4f), Quaternion.identity);
                photonView.RPC("CleanScore", RpcTarget.All, true);
                break;

            case 2:
                player = PhotonNetwork.Instantiate("1Player2", new Vector2(6.3f, -5.4f), Quaternion.identity);
                photonView.RPC("CleanScore", RpcTarget.All, false);
                break;
        }
    }

    /// <summary>
    /// Function that activates the generators (enemies and coins) on the server.
    /// </summary>
    [PunRPC] void EnableGenerators()
    {
        for (int i = 0; i < generators.Length; i++)
        {
            generators[i].SetActive(true);
        }
    }

    /// <summary>
    /// Function that deactivates the generators on the server.
    /// </summary>
    void DisableGenerators()
    {
        for (int i = 0; i < generators.Length; i++)
        {
            generators[i].SetActive(false);
        }
    }

    /// <summary>
    /// Function that removes all objects on the screen.
    /// </summary>
    void CleanScene()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Game1/Enemy");
        if (enemies != null)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetActive(false);
            }
        }

        GameObject[] coins = GameObject.FindGameObjectsWithTag("Game1/Coin");
        if (coins != null)
        {
            for (int i = 0; i < coins.Length; i++)
            {
                DestroyCoin(coins[i].GetComponent<PhotonView>().ViewID, 0);
            }
        }

        GameObject[] missiles = GameObject.FindGameObjectsWithTag("Game1/Missile");
        if (missiles != null)
        {
            for (int i = 0; i < missiles.Length; i++)
            {
                missiles[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Function that plays the sounds on the server.
    /// </summary>
    /// <param name="soundToPlay">The sound we want to play.</param>
    [PunRPC] void PlaySound(string soundToPlay)
    {
        switch (soundToPlay)
        {
            case "coin":
                coinSound.Play();
                break;
            case "hurt":
                hurtSound.Play();
                break;
        }
    }

    /// <summary>
    /// Function we call to destroy a coin.
    /// </summary>
    /// <param name="coin">The Photon View ID of the coin that we are going to destroy.</param>
    /// <param name="playerNumber">Player who has collected the coin. 0 if null.</param>
    public void DestroyCoin(int coin, int playerNumber)
    {
        photonView.RPC("DestroyCoinServer", RpcTarget.MasterClient, coin, playerNumber);
    }

    /// <summary>
    /// Function that destroy a coin on the server and increases the score.
    /// </summary>
    /// <param name="coin">The Photon View ID of the coin that we are going to destroy.</param>
    /// <param name="playerNumber">Player who has collected the coin. 0 if null.</param>
    [PunRPC] void DestroyCoinServer(int coin, int playerNumber)
    {
        if (PhotonView.Find(coin) != null)
        {
            PhotonNetwork.Destroy(PhotonView.Find(coin));

            switch (playerNumber)
            {
                case 1:
                    photonView.RPC("UpdateScore", RpcTarget.All, true);
                    photonView.RPC("PlaySound", RpcTarget.All, "coin");
                    break;
                case 2:
                    photonView.RPC("UpdateScore", RpcTarget.All, false);
                    photonView.RPC("PlaySound", RpcTarget.All, "coin");
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Function that increases the score on the server.
    /// </summary>
    /// <param name="isPlayer1">True if player 1 scores.</param>
    [PunRPC] void UpdateScore(bool isPlayer1)
    {
        if (isPlayer1)
        {
            score1 += 1;
            score1Text.text = "Score: " + score1;
        }

        else 
        {
            score2 += 1;
            score2Text.text = "Score: " + score2;
        }
    }

    /// <summary>
    /// Function that resets a player's score to zero on the server.
    /// </summary>
    /// <param name="isPlayer1">True if is for player 1.</param>
    [PunRPC] void CleanScore(bool isPlayer1)
    {
        if (isPlayer1)
        {
            score1 = 0;
            score1Text.text = "Score: " + score1;
        }

        else
        {
            score2 = 0;
            score2Text.text = "Score: " + score2;
        }
    }

    /// <summary>
    /// Function that pauses and resumes the game.
    /// </summary>
    public void PauseGame()
    {
        if (!panelPause.activeSelf)
        {
            panelPause.SetActive(true);
        }
        
        else if (panelPause.activeSelf)
        {
            panelPause.SetActive(false);
        }
    }

    /// <summary>
    /// Coroutine that makes a wait before instantiating the player again.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(2);
        {
            InstantiatePlayer();
        }
    }

    /// <summary>
    /// Function that is called when we enter a room.
    /// </summary>
    public override void OnJoinedRoom()
    {
        NetworkManager.networkManager.SetValues();

        if (NetworkManager.networkManager.players.Length == 2)
        {
            waitingMessage.SetActive(false);
        }

        StartGame();
    }

    /// <summary>
    /// Function called when another player joins the room.
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        NetworkManager.networkManager.SetValues();

        if (NetworkManager.networkManager.players.Length == 2)
        {
            waitingMessage.SetActive(false);

            photonView.RPC("EnableGenerators", RpcTarget.All);
        }
    }

    /// <summary>
    /// Function called when a player leaves the room.
    /// </summary>
    /// <param name="otherPlayer">The player who left the room.</param>
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (otherPlayer.NickName == "1")
        {
            PhotonNetwork.LeaveRoom();

            panelLostPlayer1.SetActive(true);
        }

        else
        {
            NetworkManager.networkManager.SetValues();

            waitingMessage.SetActive(true);
        }

        DisableGenerators();

        CleanScene();
    }

    /// <summary>
    /// Function that is called when we disconnect from the server.
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        GameManager1.manager.LoadGame(1);
    }
}

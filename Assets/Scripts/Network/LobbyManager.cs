using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_EMPIRE_NAME = "EmpireName";
    public const string KEY_PLAYER_COLOR_RED = "PlayerColorRed";
    public const string KEY_PLAYER_COLOR_GREEN = "PlayerColorGreen";
    public const string KEY_PLAYER_COLOR_BLUE = "PlayerColorBlue";
    public const string KEY_PLAYER_COLOR_ALFA = "PlayerColorAlfa";

    public const string KEY_START_GAME = "StartGame";

    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyPollTimer;
    private string playerName;
    private string empireName = "Empire";
    private bool gameStarted = false;

    private string playerColorRed = "0";
    private string playerColorGreen = "0";
    private string playerColorBlue = "255";
    private string playerColorAlfa = "255";

    public event EventHandler OnLeftLobby;

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public event EventHandler<PlayerEventArgs> OnGameStarted;
    public class PlayerEventArgs : EventArgs
    {
        public Player player;
    }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        OnGameStarted += GameStarted_Event;
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
    }

    public async void Authenticate(string playerName)
    {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn) { return; }

        this.playerName = playerName;

        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () => {
            // do nothing
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);

            RefreshLobbyList();
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (joinedLobby != null && IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private async void HandleLobbyPolling()
    {
        try
        {
            if (joinedLobby != null)
            {
                lobbyPollTimer -= Time.deltaTime;
                if (lobbyPollTimer < 0f)
                {
                    float lobbyPollTimerMax = 1.1f;
                    lobbyPollTimer = lobbyPollTimerMax;
                
                    joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                    OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    if (!IsPlayerInLobby())
                    {
                        // Player was kicked out of this lobby
                        Debug.Log("Kicked from Lobby!");

                        OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                        joinedLobby = null;
                    }

                    if (joinedLobby.Data[KEY_START_GAME].Value != "0" && !gameStarted)
                    {
                        // Game started, joining the client
                        if (!IsLobbyHost())
                        {
                            await RelayManager.Instance.JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                        }

                        OnGameStarted?.Invoke(this, new PlayerEventArgs { player = GetPlayer() });
                    }
                }
            }
        }
        catch (LobbyServiceException e)
        {
            print(e);
        }
    }

    public Lobby GetJoinedLobby()
    {
        return joinedLobby;
    }

    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private bool IsPlayerInLobby()
    {
        if (joinedLobby != null && joinedLobby.Players != null)
        {
            foreach (Player player in joinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    // This player is in this lobby
                    return true;
                }
            }
        }
        return false;
    }

    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
    {
        try
        {
            Player player = GetPlayer();

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = player,
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject> {
                    { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0") } 
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });

            Debug.Log("Created Lobby " + lobby.Name);
        }
        catch (LobbyServiceException e)
        {
            print(e);
        }
    }

    public async void RefreshLobbyList()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 15;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder> {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {

                }
            };

            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            foreach (Lobby lobby in response.Results)
            {
                print(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            print(e);
        }
    }

    public async void JoinLobby(Lobby lobby) {
        Player player = GetPlayer();

        joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions {
            Player = player
        });

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        Player player = GetPlayer();

        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions
        {
            Player = player
        });

        joinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

    public async void UpdatePlayerName(string playerName)
    {
        this.playerName = playerName;

        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_NAME, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void UpdateEmpireName(string empireName)
    {
        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_EMPIRE_NAME, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: empireName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
    public async void UpdatePlayerColor(Color32 color)
    {
        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_COLOR_RED, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: color.r.ToString())
                    },
                    {
                        KEY_PLAYER_COLOR_BLUE, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: color.b.ToString())
                    },
                    {
                        KEY_PLAYER_COLOR_GREEN, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: color.g.ToString())
                    },
                    {
                        KEY_PLAYER_COLOR_ALFA, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: color.a.ToString())
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;

                OnLeftLobby?.Invoke(this, EventArgs.Empty);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void StartGame()
    {
        if(IsLobbyHost())
        {
            try
            {
                Debug.Log("Start game");

                string relayCode = await RelayManager.Instance.CreateRelay(joinedLobby.Players.Count);

                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    }
                });

                joinedLobby = lobby;

                OnGameStarted?.Invoke(this, new PlayerEventArgs { player = GetPlayer()});
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    private Player GetPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
            { KEY_EMPIRE_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, empireName) },
            { KEY_PLAYER_COLOR_RED, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerColorRed) },
            { KEY_PLAYER_COLOR_GREEN, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerColorGreen) },
            { KEY_PLAYER_COLOR_BLUE, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerColorBlue) },
            { KEY_PLAYER_COLOR_ALFA, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerColorAlfa) }
        });
    }

    public string GetPlayerName() {return playerName; }

    public Color32 GetPlayerColor(Player player)
    {
        byte red = byte.Parse(player.Data[KEY_PLAYER_COLOR_RED].Value);
        byte green = byte.Parse(player.Data[KEY_PLAYER_COLOR_GREEN].Value);
        byte blue = byte.Parse(player.Data[KEY_PLAYER_COLOR_BLUE].Value);
        byte alfa = byte.Parse(player.Data[KEY_PLAYER_COLOR_ALFA].Value);

        return new Color32(red, green, blue, alfa);
    }

    private void GameStarted_Event(object sender, PlayerEventArgs e)
    {
        Settings.empireName = e.player.Data[KEY_EMPIRE_NAME].Value;
        Settings.playerColor = GetPlayerColor(e.player);
        Settings.singlePlayer = false;
        Settings.players = joinedLobby.Players;

        gameStarted = true;

        SceneManager.LoadScene(1);

        OnGameStarted -= GameStarted_Event;
    }
}

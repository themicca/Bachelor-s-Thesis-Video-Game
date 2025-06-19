using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Unity.Netcode;

public class Clock : NetworkBehaviour
{
    [SerializeField] private bool pause = false;
    private int speed;
    private float passDay;
    private float passDayBase;
    private int daysTotal;
    private int days;
    private int months;
    private int years;
    [SerializeField] private TMP_Text text;
    static Clock instance;

    [SerializeField] private Speed1UI speed1UI;
    [SerializeField] private Speed2UI speed2UI;
    [SerializeField] private Speed3UI speed3UI;
    [SerializeField] private Speed4UI speed4UI;

    NetworkVariable<int> speedNetwork = new();
    NetworkVariable<int> daysNetwork = new();
    NetworkVariable<int> monthsNetwork = new();
    NetworkVariable<int> yearsNetwork = new();
    NetworkVariable<int> daysTotalNetwork = new();
    NetworkVariable<float> passDayNetwork = new();

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            speedNetwork.Value = speed;
            daysNetwork.Value = days;
            monthsNetwork.Value = months;
            yearsNetwork.Value = years;
            daysTotalNetwork.Value = daysTotal;
            passDayNetwork.Value = passDay;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    void Awake()
    {
        text = GameObject.Find("Time Text").GetComponent<TMP_Text>();

        speed1UI = FindObjectOfType<Speed1UI>();
        speed2UI = FindObjectOfType<Speed2UI>();
        speed3UI = FindObjectOfType<Speed3UI>();
        speed4UI = FindObjectOfType<Speed4UI>();

        instance = this;
        speed = 9;
        Utilities.SetSpeedSelected(FindObjectOfType<Speed3UI>().GetComponent<Button>());

        days = 1;
        months = 1;
        years = 1;
        passDayBase = 1;
        passDay = passDayBase;
        daysTotal = 0;
        text.text = "Time: " + days + ":" + months + ":" + years;

        if (NetworkManager.Singleton.IsHost)
        {
            Utilities.SpawnNetworkObject(gameObject);
        }
    }

    private void Start()
    {
        speedNetwork.OnValueChanged += OnSpeedChange;
    }

    void Update()
    {
        if (IsHost && Input.GetKeyDown("space"))
            pause = !pause;
        if (!pause)
        {
            passDay -= Time.deltaTime * speed;
            if (NetworkManager.Singleton.IsHost)
            {
                passDayNetwork.Value = passDay;
            }
            if (passDay <= 0)
            {

                days += 1;
                daysTotal += 1;
                passDay = passDayBase;

                if (NetworkManager.Singleton.IsHost)
                {

                    daysNetwork.Value = days;
                    daysTotalNetwork.Value = daysTotal;
                }

                DailyCalculations();
                if (days > 30)
                {
                    days = 1;
                    months += 1;

                    if (NetworkManager.Singleton.IsHost)
                    {
                        daysNetwork.Value = days;
                        monthsNetwork.Value = months;
                    }

                    MonthlyCalculations();
                    if (months > 12)
                    {
                        months = 1;
                        years += 1;

                        if (NetworkManager.Singleton.IsHost)
                        {
                            monthsNetwork.Value = months;
                            yearsNetwork.Value = years;
                        }
                    }
                }
            }
            text.text = "Time: " + days + ":" + months + ":" + years;
        }
    }

    private void DailyCalculations()
    {
        RunMovements(GameManager.GetMovements());
        RunCombats();
    }

    private void MonthlyCalculations()
    {
        CalculateResources();
        GameManager.IncreasePopulation();
    }

    private void RunMovements(Dictionary<Unit, MovementController> movements)
    {
        foreach (MovementController movement in movements.Values.ToList())
        {
            movement.HandleMovement();
        }
    }

    private void RunCombats()
    {
        List<Combat> combats = GameManager.GetCombats();
        foreach (Combat combat in combats.ToList())
        {
            combat.Fight();
            if (CombatDisplay.GetActiveInstance() == combat)
            {
                PlayerActionCanvas.UpdateCombatCard(combat);
            }
        }
    }

    private void CalculateResources()
    {
        PlayerManager.ResourceCalculation();
    }

    public static Clock GetInstance() { return instance; }
    public static int GetGameSpeed() { return instance.speed; }

    public int GetTotalDays() { return daysTotal; }

    public static void SetGameSpeed(int speed) { instance.speedNetwork.Value = speed; instance.speed = speed; }

    private void OnSpeedChange(int oldValue, int newValue)
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            speed = newValue;

            speed1UI.GetComponent<Button>().interactable = false;
            speed2UI.GetComponent<Button>().interactable = false;
            speed3UI.GetComponent<Button>().interactable = false;
            speed4UI.GetComponent<Button>().interactable = false;

            switch (speed)
            {
                case 1:
                    speed1UI.GetComponent<Button>().interactable = true;
                    Utilities.SetSpeedSelected(speed1UI.GetComponent<Button>());
                    break;
                case 5:
                    speed2UI.GetComponent<Button>().interactable = true;
                    Utilities.SetSpeedSelected(speed2UI.GetComponent<Button>());
                    break;
                case 9:
                    speed3UI.GetComponent<Button>().interactable = true;
                    Utilities.SetSpeedSelected(speed3UI.GetComponent<Button>());
                    break;
                case 13:
                    speed4UI.GetComponent<Button>().interactable = true;
                    Utilities.SetSpeedSelected(speed4UI.GetComponent<Button>());
                    break;
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        SendDataToClientServerRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendDataToClientServerRpc(ulong clientId)
    {
        // Send the data to the client using a ClientRPC
        SendDataToClientClientRpc(clientId);
    }

    [ClientRpc]
    private void SendDataToClientClientRpc(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            speed = speedNetwork.Value;
            daysTotal = daysNetwork.Value;
            months = monthsNetwork.Value;
            years = yearsNetwork.Value;
            daysTotal = daysTotalNetwork.Value;
            passDay = passDayNetwork.Value;

            switch (speed)
            {
                case 1:
                    speed1UI.GetComponent<Button>().interactable = true;
                    Utilities.SetSpeedSelected(speed1UI.GetComponent<Button>());
                    break;
                case 5:
                    speed2UI.GetComponent<Button>().interactable = true;
                    Utilities.SetSpeedSelected(speed2UI.GetComponent<Button>());
                    break;
                case 9:
                    speed3UI.GetComponent<Button>().interactable = true;
                    Utilities.SetSpeedSelected(speed3UI.GetComponent<Button>());
                    break;
                case 13:
                    speed4UI.GetComponent<Button>().interactable = true;
                    Utilities.SetSpeedSelected(speed4UI.GetComponent<Button>());
                    break;
            }
        }
    }
}

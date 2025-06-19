using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;

public class Settings : MonoBehaviour
{
    public static int gridSizeX = 15, gridSizeY = 15;
    public static Color playerColor = Color.blue;
    public static string empireName = "Empire";
    public static bool singlePlayer = true;
    public static List<Player> players = null;
}

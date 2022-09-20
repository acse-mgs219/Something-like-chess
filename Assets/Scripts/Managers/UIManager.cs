using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEssentials.Extensions;
using static System.Collections.Specialized.BitVector32;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI winnerText;
    public GameObject winnerBackground;
    public GameObject checkMarker;

    private void Awake()
    {
        instance = this;
    }

    public void AnnounceWinner()
    {
        Image background = winnerBackground.GetComponent<Image>();
        Color backgroundColor = Color.black;
        List<Player> survivingPlayers = PlayerManager.instance.Players.Where(p => p.IsInCheck == false).ToList();

        if (survivingPlayers.Count == 1)
        {
            Player winner = survivingPlayers.First();

            backgroundColor = ColorHelper.Instance.GetColor(winner.Color);
            winnerText.text = $"The winner is {winner.Name}";
        }
        else
        {
            List<Color> colors = survivingPlayers.Select(p => ColorHelper.Instance.GetColor(p.Color)).ToList();
            backgroundColor = colors.Average();
            winnerText.text = "The game ends in a tie!";
        }

        background.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, background.color.a);
        winnerBackground.SetActive(true);
    }

    public void ShowCheck(Player player, bool inCheck)
    {

        if (inCheck)
        {
            Color playerColor = ColorHelper.Instance.GetColor(player.Color);
            Image background = checkMarker.GetComponent<Image>();
            background.color = new Color(playerColor.r, playerColor.g, playerColor.b, background.color.a);
        }

        checkMarker.SetActive(inCheck);
    }
}
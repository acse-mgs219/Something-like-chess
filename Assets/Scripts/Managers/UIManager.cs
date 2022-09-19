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

    public void AnnounceWinner(Player player)
    {
        Color playerColor = ColorHelper.Instance.GetColor(player.Color);
        Image background = winnerBackground.GetComponent<Image>();
        background.color = new Color(playerColor.r, playerColor.g, playerColor.b, background.color.a);
        winnerText.text = $"The winner is {player.Name}";

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
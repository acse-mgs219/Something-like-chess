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
    public GameObject startupPanel;

    [HideInInspector] public List<TextMeshProUGUI> playerLabels;
    [HideInInspector] public List<Toggle> playerHumanToggles;
    //public TextMeshProUGUI player1Label;
    //public TextMeshProUGUI player2Label;

    //public Toggle player1IsHuman;
    //public Toggle player2IsHuman;

    private void Start()
    {
        instance = this;

        playerLabels = startupPanel.GetComponentsInChildren<TextMeshProUGUI>().Where(tmp => tmp.name == "Label").ToList();

        for (int i = 0; i < playerLabels.Count; i++)
        {
            List<Player> players = PlayerManager.instance.Players;

            playerLabels[i].color = ColorHelper.Instance.GetColor(players[i].Color);
        }

        playerHumanToggles = startupPanel.GetComponentsInChildren<Toggle>().ToList();

        for (int i = 0; i < playerHumanToggles.Count; i++)
        {
            Toggle toggle = playerHumanToggles[i];
            toggle.onValueChanged.AddListener((value) =>
            {
                PlayerManager.instance.SetPlayerHuman(playerHumanToggles.IndexOf(toggle), value);
            });
        }
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

    public void StartGame()
    {
        startupPanel.gameObject.SetActive(false);
        GameManager.instance.ChangeState(GameState.GenerateGrid);
    }
}
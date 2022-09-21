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
    [HideInInspector] public List<TMP_Dropdown> colorsPickers;

    private void Start()
    {
        instance = this;

        playerLabels = startupPanel.GetComponentsInChildren<TextMeshProUGUI>().Where(tmp => tmp.name == "NameLabel").ToList();

        for (int i = 0; i < playerLabels.Count; i++)
        {
            List<Player> players = PlayerManager.instance.Players;

            playerLabels[i].color = ColorHelper.Instance.GetColor(players[i].Color);
        }

        playerHumanToggles = startupPanel.GetComponentsInChildren<Toggle>().ToList();

        foreach (Toggle toggle in playerHumanToggles)
        {
            toggle.onValueChanged.AddListener((value) =>
            {
                PlayerManager.instance.SetPlayerHuman(playerHumanToggles.IndexOf(toggle), value);
            });
        }

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (string colorName in Enum.GetNames(typeof(ColorHelper.NamedColor)))
        {
            options.Add(new TMP_Dropdown.OptionData(colorName));
        }
        colorsPickers = startupPanel.GetComponentsInChildren<TMP_Dropdown>().ToList();

        foreach (TMP_Dropdown colorsPicker in colorsPickers)
        {
            // Cannot do a "for int i = ..." loop because the delegates all get stuck with the last value of i.
            int index = colorsPickers.IndexOf(colorsPicker);

            colorsPicker.options = options;
            colorsPicker.value = (int) PlayerManager.instance.Players[index].Color;
            colorsPicker.onValueChanged.AddListener((value) =>
            {
                OnColorPicked(index, value);
            });
        }
    }

    public void OnColorPicked(int playerIndex, int colorIndex)
    {
        ColorHelper.NamedColor namedColor = (ColorHelper.NamedColor) colorIndex;
        Color color = ColorHelper.Instance.GetColor(namedColor);
        playerLabels[playerIndex].color = color;

        PlayerManager.instance.SetPlayerColor(playerIndex, namedColor);
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
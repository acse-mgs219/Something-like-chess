using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    [HideInInspector] public List<TMP_Dropdown> aiPickers;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    public void ShowStartupPanel()
    {
        startupPanel.SetActive(true);
    }

    public void Init()
    {
        playerLabels = startupPanel.GetComponentsInChildren<TextMeshProUGUI>().Where(tmp => tmp.name == "NameLabel").ToList();

        for (int i = 0; i < playerLabels.Count; i++)
        {
            List<Player> players = PlayerManager.instance.Players;

            playerLabels[i].color = ColorHelper.Instance.GetColor(players[i].Color);
        }

        playerHumanToggles = startupPanel.GetComponentsInChildren<Toggle>().ToList();

        foreach (Toggle toggle in playerHumanToggles)
        {
            // Cannot do a "for int i = ..." loop because the delegates all get stuck with the last value of i.
            int index = playerHumanToggles.IndexOf(toggle);

            toggle.onValueChanged.AddListener((value) =>
            {
                OnHumanToggled(index, value);
            });

            toggle.isOn = PlayerPrefs.GetInt($"player{index}Human") == 1;
        }

        List<TMP_Dropdown.OptionData> colorOptions = new List<TMP_Dropdown.OptionData>();
        foreach (string colorName in Enum.GetNames(typeof(ColorHelper.NamedColor)))
        {
            colorOptions.Add(new TMP_Dropdown.OptionData(colorName));
        }
        colorsPickers = startupPanel.GetComponentsInChildren<TMP_Dropdown>().Where(tmp => tmp.name == "ColorsPicker").ToList();

        foreach (TMP_Dropdown colorsPicker in colorsPickers)
        {
            // Cannot do a "for int i = ..." loop because the delegates all get stuck with the last value of i.
            int index = colorsPickers.IndexOf(colorsPicker);

            colorsPicker.options = colorOptions;
            colorsPicker.onValueChanged.AddListener((value) =>
            {
                OnColorPicked(index, value);
            });

            colorsPicker.value = PlayerPrefs.GetInt($"color{index}");
        }

        List<TMP_Dropdown.OptionData> aiOptions = new List<TMP_Dropdown.OptionData>();
        foreach (string aiName in Enum.GetNames(typeof(AITypes.AIType)))
        {
            aiOptions.Add(new TMP_Dropdown.OptionData(aiName));
        }
        aiPickers = startupPanel.GetComponentsInChildren<TMP_Dropdown>().Where(tmp => tmp.name == "AIPicker").ToList();

        foreach (TMP_Dropdown aiPicker in aiPickers)
        {
            // Cannot do a "for int i = ..." loop because the delegates all get stuck with the last value of i.
            int index = aiPickers.IndexOf(aiPicker);

            aiPicker.options = aiOptions;
            aiPicker.onValueChanged.AddListener((value) =>
            {
                OnAIPicked(index, value);
            });

            aiPicker.enabled = playerHumanToggles[index].isOn == false;
            aiPicker.value = PlayerPrefs.GetInt($"AI{index}");
            OnAIPicked(index, aiPicker.value); // Need to call manually because won't be called if value above is set to 0 which is its default, but we still want to init.
        }
    }

    public void OnHumanToggled(int playedIndex, bool human)
    {
        PlayerPrefs.SetInt($"player{playedIndex}Human", human ? 1 : 0);
        PlayerManager.instance.SetPlayerHuman(playedIndex, human);
        if (playedIndex < aiPickers.Count)
        {
            aiPickers[playedIndex].enabled = !human;
        }
    }

    public void OnColorPicked(int playerIndex, int colorIndex)
    {
        PlayerPrefs.SetInt($"color{playerIndex}", colorIndex);
        ColorHelper.NamedColor namedColor = (ColorHelper.NamedColor) colorIndex;
        Color color = ColorHelper.Instance.GetColor(namedColor);
        playerLabels[playerIndex].color = color;

        PlayerManager.instance.SetPlayerColor(playerIndex, namedColor);
    }

    public void OnAIPicked(int playerIndex, int aiIndex)
    {
        PlayerPrefs.SetInt($"AI{playerIndex}", aiIndex);
        AITypes.AIType aiType = (AITypes.AIType) aiIndex;

        PlayerManager.instance.SetPlayerAI(playerIndex, aiType);
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

    public void ReStartGame()
    {
        winnerBackground.SetActive(false);

        foreach (Player player in PlayerManager.instance.Players)
        {
            player.Destroy();
        }

        GameManager.instance.ChangeState(GameState.PickPlayers);
    }

    public void StartGame()
    {
        startupPanel.gameObject.SetActive(false);
        GameManager.instance.ChangeState(GameState.GenerateGrid);
    }
}
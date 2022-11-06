using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using UnityEssentials.Extensions;

public class ScoreLabel
{
    private string _key;
    public string Key => _key;

    private TextMeshProUGUI _playerLabel;
    private TextMeshProUGUI _scoreText;
    private int _scoreValue;

    public void IncreaseScoreValue(int delta)
    {
        _scoreValue += delta;
        _scoreText.text = _scoreValue.ToString();
    }

    public ScoreLabel(string key, TextMeshProUGUI playerLabel, TextMeshProUGUI scoreText, int scoreValue)
    {
        _key = key;
        _playerLabel = playerLabel;
        _scoreText = scoreText;
        _scoreValue = scoreValue;
    }
}

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI winnerText;
    public GameObject winnerBackground;
    public GameObject checkMarker;
    public GameObject startupPanel;
    public GameObject scorePanel;

    [HideInInspector] public List<TextMeshProUGUI> playerLabels;
    [HideInInspector] public List<ScoreLabel> scoreLabels;
    [HideInInspector] public List<Toggle> playerHumanToggles;
    [HideInInspector] public List<TMP_Dropdown> colorsPickers;
    [HideInInspector] public List<TMP_Dropdown> aiPickers;

    public Color averagePlayerColor;

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

    private void AddScoreLabel(string key, Color color)
    {
        TextMeshProUGUI playerLabel = scorePanel.GetComponentsInChildren<TextMeshProUGUI>().Where(tmp => tmp.name == $"Name_{key}").First();
        TextMeshProUGUI scoreLabel = scorePanel.GetComponentsInChildren<TextMeshProUGUI>().Where(tmp => tmp.name == $"Score_{key}").First();
        playerLabel.color = color;
        scoreLabel.color = color;

        scoreLabels.Add(new ScoreLabel(key, playerLabel, scoreLabel, 0));
    }

    public void Init()
    {
        playerLabels = startupPanel.GetComponentsInChildren<TextMeshProUGUI>().Where(tmp => tmp.name == "NameLabel").ToList();

        List<Color> allPlayerColors = new List<Color>();
        List<Player> players = PlayerManager.instance.Players;

        for (int i = 0; i < playerLabels.Count; i++)
        {
            Color playerColor = ColorHelper.Instance.GetColor(players[i].Color);
            playerLabels[i].color = playerColor;
            allPlayerColors.Add(playerColor);
        }

        averagePlayerColor = allPlayerColors.Average();

        scoreLabels = new List<ScoreLabel>();

        foreach (Player player in players)
        {
            AddScoreLabel(player.Name.ReplaceWhitespace(""), ColorHelper.Instance.GetColor(player.Color));
        }

        AddScoreLabel("Ties", averagePlayerColor);

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
            winner.IncreaseScore(2);
            scoreLabels.First(sl => sl.Key == winner.Name).IncreaseScoreValue(1);

            backgroundColor = ColorHelper.Instance.GetColor(winner.Color);
            winnerText.text = $"The winner is {winner.Name}";
        }
        else
        {
            List<Color> colors = survivingPlayers.Select(p => ColorHelper.Instance.GetColor(p.Color)).ToList();
            backgroundColor = colors.Average();
            winnerText.text = "The game ends in a tie!";
            survivingPlayers.ForEach(p => p.IncreaseScore(1));
            scoreLabels.First(sl => sl.Key == "Ties").IncreaseScoreValue(1);
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
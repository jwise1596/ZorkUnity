using Zork;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private UnityInputService InputService;

    [SerializeField]
    private UnityOutputService OutputService;

    [SerializeField]
    private TextMeshProUGUI CurrentLocationText;

    [SerializeField]
    private TextMeshProUGUI ScoreText;

    [SerializeField]
    private TextMeshProUGUI MovesText;

    void Start()
    {
        TextAsset gameTextAsset = Resources.Load<TextAsset>("Zork");
        _game = JsonConvert.DeserializeObject<Game>(gameTextAsset.text);
        _game.Player.LocationChanged += (sender, location) => CurrentLocationText.text = location.ToString();
        _game.Player.MovesChanged += (sender, moves) => MovesText.text = moves.ToString();
        _game.Player.ScoreChanged += (sender, score) => ScoreText.text = score.ToString();
        
        _game.Start(InputService, OutputService);
        OutputService.WriteLine(string.IsNullOrWhiteSpace(_game.WelcomeMessage) ? "Welcome to Zork!" : _game.WelcomeMessage);
        //_game.Commands["LOOK"].Action(_game);
        _game.Commands["NORTH"].Action(_game);
    }

    private Game _game;
}
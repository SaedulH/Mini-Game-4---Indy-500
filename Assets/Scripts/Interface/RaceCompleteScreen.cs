using System.Threading.Tasks;
using AudioSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

[RequireComponent(typeof(UIDocument))]
public class RaceCompleteScreen : NonPersistentSingleton<RaceCompleteScreen>
{
    [field: SerializeField] public VisualElement Root { get; set; }
    [field: SerializeField] public VisualElement RaceCompleteElement { get; set; }
    [field: SerializeField] public VisualElement RaceCompleteBackground { get; set; }
    [field: SerializeField] public Label WinningPlayer { get; set; }
    [field: SerializeField] public Label WinningText { get; set; }
    [field: SerializeField] public Button RestartButton { get; set; }
    [field: SerializeField] public Button QuitButton { get; set; }

    [field: Header("Audio")]
    [field: SerializeField] public AudioData RaceWinAudio { get; set; }
    [field: SerializeField] public AudioData RaceLoseAudio { get; set; }
    [field: SerializeField] public AudioData RaceResultsBGM { get; set; }
    private AudioData _currentAudio;
    [field: SerializeField] public AudioData RestartAudio { get; set; }
    [field: SerializeField] public AudioData QuitAudio { get; set; }
    [field: SerializeField] public AudioData HoverAudio { get; set; }

    protected override void Awake()
    {
        base.Awake();
        Root = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

        RaceCompleteElement = Root.Q<VisualElement>("RaceComplete");
        RaceCompleteElement.AddToClassList("hideUI");
        RaceCompleteElement.style.display = DisplayStyle.None;
        RaceCompleteBackground = Root.Q<VisualElement>("Background");

        WinningPlayer = RaceCompleteElement.Q<Label>("WinningPlayer");
        WinningText = RaceCompleteElement.Q<Label>("WinningText");

        RestartButton = RaceCompleteElement.Q<Button>("Restart");
        RestartButton.clicked += OnRestartClicked;

        QuitButton = RaceCompleteElement.Q<Button>("Quit");
        QuitButton.clicked += OnQuitClicked;

        SetupHoverAudio();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;

        RestartButton.clicked -= OnRestartClicked;
        QuitButton.clicked -= OnQuitClicked;
    }

    private async void HandleGameStateChanged(GameState state)
    {
        if(state == GameState.GameOver)
        {

            AudioManager.Instance.CreateAudioBuilder()
                .Play(_currentAudio);

            await Task.Delay(1000);

            MusicManager.Instance.PlayMusic(RaceResultsBGM);
            await ShowRaceCompleteScreen();
        }
        else
        {
            await HideRaceCompleteScreen();
        }
    }

    public async Task ShowRaceCompleteScreen()
    {
        Debug.Log("Show Race Complete Screen");
        //GlobalVolumeManager.Instance.Blur();
        RaceCompleteElement.style.display = DisplayStyle.Flex;
        await Task.Yield();
        RaceCompleteElement.RemoveFromClassList("hideUI");

        await Task.Delay(200);
    }

    public async Task HideRaceCompleteScreen()
    {
        Debug.Log("Hide Race Complete Screen");
        //GlobalVolumeManager.Instance.Unblur();
        RaceCompleteElement.AddToClassList("hideUI");
        await Task.Delay(200);
        RaceCompleteElement.style.display = DisplayStyle.None;
    }

    public void SetRaceCompleteDetails(RaceCompleteDetails details)
    {
        string winningPlayer = "";
        string winningText = "";
        bool isRaceWin = true;
        if (details.GameMode == GameMode.Race)
        {
            if (details.PlayerCount == 1)
            {
                RaceCompleteBackground.style.width = new StyleLength(new Length(150, LengthUnit.Percent));
                winningPlayer = details.WinningPlayer == "Player One" ? Constants.SOLO_RACE_WIN : Constants.SOLO_RACE_LOSE;
                isRaceWin = details.WinningPlayer == "Player One";
            } 
            else
            {
                RaceCompleteBackground.style.width = new StyleLength(new Length(170, LengthUnit.Percent));
                winningPlayer = $"{Constants.VERSUS_WINNER_TEXT}{details.WinningPlayer}";
            }
        }
        else if (details.GameMode == GameMode.Timed)
        {
            winningText = $"{Constants.BEST_LAP_TEXT}{details.WinningTime}";
            if (details.PlayerCount == 1)
            {
                RaceCompleteBackground.style.width = new StyleLength(new Length(150, LengthUnit.Percent));
                winningPlayer = details.AwardedMedal != Medal.Failed  ? $"{Constants.SOLO_TIMED_WIN}{details.AwardedMedal}" : Constants.SOLO_TIMED_LOSE;
                isRaceWin = details.AwardedMedal != Medal.Failed;
            }
            else
            {
                RaceCompleteBackground.style.width = new StyleLength(new Length(170, LengthUnit.Percent));
                winningPlayer = $"{Constants.VERSUS_WINNER_TEXT}{details.WinningPlayer}";
            }
        }
        _currentAudio = isRaceWin ? RaceWinAudio : RaceLoseAudio;
        WinningPlayer.text = winningPlayer;
        WinningText.text = winningText;
    }

    private void OnRestartClicked()
    {
        PlayRestartAudio();
        GameManager.Instance.RestartLevel();
    }

    private void OnQuitClicked()
    {
        PlayQuitAudio();
        GameManager.Instance.ReturnToMainMenu();
    }

    #region Audio 
    private void SetupHoverAudio()
    {
        Root.Query<Button>().ForEach(button =>
        {
            bool hovered = false;
            button.RegisterCallback<PointerEnterEvent>(_ =>
            {
                if (hovered || !button.enabledSelf)
                    return;

                hovered = true;

                PlayHoverAudio();
            });

            button.RegisterCallback<PointerLeaveEvent>(_ =>
            {
                hovered = false;
            });
        });
    }

    private void PlayRestartAudio(bool playSound = true)
    {
        if (!playSound) return;
        AudioManager.Instance.CreateAudioBuilder()
            .WithRandomPitch(-0.05f, 0.05f)
            .Play(RestartAudio);
    }
    private void PlayQuitAudio(bool playSound = true)
    {
        if (!playSound) return;
        AudioManager.Instance.CreateAudioBuilder()
            .Play(QuitAudio);
    }

    private void PlayHoverAudio(bool playSound = true)
    {
        if (!playSound) return;
        AudioManager.Instance.CreateAudioBuilder()
            .Play(HoverAudio);
    }

    #endregion
}

using System.Threading.Tasks;
using AudioSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

[RequireComponent(typeof(UIDocument))]
public class PauseScreen : NonPersistentSingleton<PauseScreen>
{
    [field: SerializeField] public VisualElement Root { get; set; }
    [field: SerializeField] public VisualElement PauseMenu { get; set; }
    [field: SerializeField] public Button PlayButton { get; set; }
    [field: SerializeField] public Button RestartButton { get; set; }
    [field: SerializeField] public Button QuitButton { get; set; }

    [field: Header("Audio")]
    [field: SerializeField] public AudioData ResumeAudio { get; set; }
    [field: SerializeField] public AudioData RestartAudio { get; set; }
    [field: SerializeField] public AudioData QuitAudio { get; set; }
    [field: SerializeField] public AudioData HoverAudio { get; set; }

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        Root = GetComponent<UIDocument>().rootVisualElement;

        PauseMenu = Root.Q<VisualElement>("PauseMenu");
        PauseMenu.AddToClassList("hideUI");
        PauseMenu.style.display = DisplayStyle.None;

        PlayButton = PauseMenu.Q<Button>("Play");
        PlayButton.clicked += OnPlayClicked;

        RestartButton = PauseMenu.Q<Button>("Restart");
        RestartButton.clicked += OnRestartClicked;

        QuitButton = PauseMenu.Q<Button>("Quit");
        QuitButton.clicked += OnQuitClicked;

        SetupHoverAudio();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;

        PlayButton.clicked -= OnPlayClicked;
        RestartButton.clicked -= OnRestartClicked;
        QuitButton.clicked -= OnQuitClicked;
    }

    private async void HandleGameStateChanged(GameState state)
    {
        if(state == GameState.Paused)
        {
            await ShowPauseMenu();
        }
        else
        {
            await HidePauseMenu();
        }
    }

    public async Task ShowPauseMenu()
    {
        Debug.Log("Show Pause Menu");
        //GlobalVolumeManager.Instance.Blur();
        PauseMenu.style.display = DisplayStyle.Flex;
        await Task.Yield();
        PauseMenu.RemoveFromClassList("hideUI");

        await Task.Delay(200);
    }

    public async Task HidePauseMenu()
    {
        Debug.Log("Hide Pause Menu");
        //GlobalVolumeManager.Instance.Unblur();
        PauseMenu.AddToClassList("hideUI");
        await Task.Delay(200);
        PauseMenu.style.display = DisplayStyle.None;
    }

    private void OnPlayClicked()
    {
        PlayResumeAudio();
        GameManager.Instance.UnpauseGame();
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

    private void PlayResumeAudio(bool playSound = true)
    {
        if (!playSound) return;
        AudioManager.Instance.CreateAudioBuilder()
            .WithRandomPitch(-0.05f, 0.05f)
            .Play(ResumeAudio);
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

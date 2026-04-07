using System.Threading.Tasks;
using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

[RequireComponent(typeof(UIDocument))]
public class LoadingScreen : NonPersistentSingleton<LoadingScreen>
{
    [field: SerializeField] public VisualElement Root { get; set; }
    [field: SerializeField] public VisualElement LoadingScreenElement { get; set; }
    [field: SerializeField] public Label TrackTitle { get; set; }
    [field: SerializeField] public Label TrackDescription { get; set; }
    [field: SerializeField] public Label TrackMode { get; set; }
    [field: SerializeField] public Label TrackPlayerCount { get; set; }
    [field: SerializeField] public VisualElement TrackImage { get; set; }
    [field: SerializeField] public Slider LoadingBar { get; protected set; }
    [field: SerializeField, Min(0f)] public float CurrentProgress { get; protected set; }
    [field: SerializeField, Min(0f)] public float MaxProgress { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        Root = GetComponent<UIDocument>().rootVisualElement;
        LoadingScreenElement = Root.Q<VisualElement>("LoadingScreen");
        LoadingScreenElement.AddToClassList("hideUI");
        LoadingScreenElement.style.display = DisplayStyle.None;

        TrackTitle = LoadingScreenElement.Q<Label>("TrackTitle");
        TrackMode = LoadingScreenElement.Q<Label>("TrackMode");
        TrackPlayerCount = LoadingScreenElement.Q<Label>("TrackPlayerCount");
        TrackImage= LoadingScreenElement.Q<VisualElement>("TrackImage");
        LoadingBar = LoadingScreenElement.Q<Slider>("LoadingBar");
    }

    public async Task SetLevelInfo(string title, string description, Sprite image, TrackContext context)
    {
        if(title == "Main Menu")
        {
            TrackMode.AddToClassList("hideUI");
            TrackPlayerCount.AddToClassList("hideUI");
            TrackImage.AddToClassList("hideUI");
        } 
        else
        {
            TrackMode.RemoveFromClassList("hideUI");
            TrackPlayerCount.RemoveFromClassList("hideUI");
            TrackImage.RemoveFromClassList("hideUI");
        }
        TrackTitle.text = title;
        TrackMode.text = $"Mode: {context.GameMode}";
        TrackPlayerCount.text = $"Players: {context.PlayerCount}";
        TrackImage.style.backgroundImage = new StyleBackground(image);

        CurrentProgress = 0f;
        MaxProgress = context.TotalWeight;

        LoadingBar.value = 0f;

        await Task.CompletedTask;
    }

    public async Task ShowLoadingScreen()
    {
        Debug.Log("Show Loading Screen");

        LoadingScreenElement.style.display = DisplayStyle.Flex;
        await Task.Yield();
        LoadingScreenElement.RemoveFromClassList("hideUI");

        await Task.Delay(400);
    }

    public async Task HideLoadingScreen()
    {
        Debug.Log("Hide Loading Screen");
        
        LoadingScreenElement.AddToClassList("hideUI");
        await Task.Delay(400);
        LoadingScreenElement.style.display = DisplayStyle.None;
    }

    public void UpdateLoadingProgress(float weight)
    {
        CurrentProgress += weight;
        LoadingBar.value = (CurrentProgress / MaxProgress) * 100;
    }
}

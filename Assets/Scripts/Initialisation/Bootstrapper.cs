using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace CoreSystem
{
    /// <summary>
    /// Bootstrapper is responsible for loading the core scenes of the game.
    /// It first loads the PersistentScene additively to ensure that essential managers and systems remain active throughout the game.
    /// After that, it loads the MainMenu scene, which serves as the entry point for players.
    /// </summary>
    [DefaultExecutionOrder(-100)] // Ensure this runs before other scripts
    public class Bootstrapper : MonoBehaviour
    {
        [field: SerializeField] private bool UsePreset = false;
        [field: SerializeField] private ScenePreset ScenePreset;

        private async void Start()
        {
            Scene bootstrapScene = SceneManager.GetActiveScene();
            if (bootstrapScene.name != "Bootstrapper")
            {
                Debug.LogError("Bootstrapper script is not in the Bootstrapper scene.");
                return;
            }

            // Load CoreScene
            AsyncOperationHandle<SceneInstance> coreHandle = Addressables.LoadSceneAsync("CoreScene", LoadSceneMode.Additive);
            await coreHandle.Task;

            if (coreHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to load CoreScene.");
                return;
            }
            SceneRegistry.CoreScene = coreHandle;
            Debug.Log("CoreScene loaded.");

            if (UsePreset && ScenePreset != null)
            {
                // Load Preset Scene
                await GameManager.Instance.InitialiseScene(ScenePreset.TrackInfo, ScenePreset.TrackContext);

                //// 3. Unload the Bootstrapper scene
                await SceneManager.UnloadSceneAsync(bootstrapScene);
            }
            else
            {
                // Load MainMenu
                AsyncOperationHandle<SceneInstance> menuHandle = Addressables.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
                await menuHandle.Task;

                if (menuHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError("Failed to load MainMenu.");
                    return;
                }
                SceneRegistry.CurrentContent = menuHandle;
                SceneManager.SetActiveScene(menuHandle.Result.Scene);
                Debug.Log("MainMenu loaded.");

                // 3. Unload the Bootstrapper scene
                await SceneManager.UnloadSceneAsync(bootstrapScene);
            }
        }
    }
}
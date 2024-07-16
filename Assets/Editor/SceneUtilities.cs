using UnityEditor;
using UnityEditor.SceneManagement;

namespace CovertCrew.Editor
{
	public static class SceneUtilities 
	{
		// METHODS
        [MenuItem("Scene/Open/Test Scene", priority = 0)]
        static void OpenEnvironmentScene()
        {
            EditorSceneManager.OpenScene($"Assets/Rhinox Studio/RPG - Stylized Fantasy Environment URP/Demo/WorldDemo 1.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene($"Assets/Scenes/Levels/GameScene.unity", OpenSceneMode.Additive);
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ApplicationManager : MonoBehaviour {
	
    public void GoToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

	public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}

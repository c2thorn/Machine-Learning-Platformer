using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class ApplicationManager : MonoBehaviour {

    void Start()
    {
        //For demo purposes only
        GameObject.Find("Story").GetComponent<Selectable>().interactable = false;
    }
	
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

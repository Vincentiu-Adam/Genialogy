using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNavigation : MonoBehaviour {

	public void LoadSceneMode (string sceneName)
	{
		SceneManager.LoadScene (sceneName);
	}

	public void QuitGame ()
	{
		Application.Quit();
	}
}

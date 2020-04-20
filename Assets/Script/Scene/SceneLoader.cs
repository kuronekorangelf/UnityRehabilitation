using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	/// <summary>
	/// 次に読み込むシーンの名称
	/// </summary>
	[InspectorName("SceneName")]
	public string _SceneName = "";

    // Update is called once per frame
    void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == gameObject)
		{
			if (Input.GetButtonDown(GameConstants.Key_ButtonNameSubmit))
			{
				loadTargetScene();
			}
		}
    }

	/// <summary>
	/// シーンロード実行
	/// </summary>
	public void loadTargetScene()
	{
		SceneManager.LoadScene(_SceneName);
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameManager : MonoBehaviour
{
	private string _NextSceneName = "ComResultScene";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

#if Debug

		// リザルトデバッグ
		if (Input.GetKeyDown(KeyCode.Return))
		{
			SceneManager.LoadScene(_NextSceneName);
		}

		// ゲーム終了
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
#endif

    }
}

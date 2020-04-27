using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStandard : MonoBehaviour
{

	#region フィールド

	/// <summary>
	/// 移動速度
	/// </summary>
	[InspectorName("移動速度")]
	public float _MoveSpeed;

	/// <summary>
	/// 削除時間定義(秒)
	/// </summary>
	[InspectorName("削除時間(秒)")]
	public float _KillTime;

	/// <summary>
	/// 削除時間計測
	/// </summary>
	private float _KillTimer = 0.0f;

	#endregion // フィールド

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		// 削除時間計測
		if (updateKillTime())
		{
			Destroy(this.gameObject);
			return;
		}

		// 移動
		updateMove();
    }

	/// <summary>
	/// 削除時間計測更新
	/// </summary>
	private bool updateKillTime()
	{
		// deltaTime == update()内でコールした場合、可変フレーム秒
		// fixedDeltaTime == fixedUpdate()内でコールした場合、可変フレーム秒
		_KillTimer += Time.deltaTime;

		if (_KillTimer >= _KillTime)
		{
			return true;
		}

		// 計測中
		return false;
	}

	/// <summary>
	/// 移動更新
	/// </summary>
	private void updateMove()
	{
		var front = transform.forward;
		var speed = _MoveSpeed * Time.deltaTime;
		var dir = front * speed;
		transform.position += dir;
	}
}

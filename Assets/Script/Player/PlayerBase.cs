using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
	#region フィールド

	/// <summary>
	/// 視点移動スピード
	/// </summary>
	[InspectorName("視点移動スピード")]
	public float _RotationSpeed = 2.0f;

	/// <summary>
	/// 移動量スピード
	/// </summary>
	[InspectorName("移動スピード")]
	public float _MoveSpeed = 2.2f;

	/// <summary>
	/// ジャンプスピード
	/// </summary>
	[InspectorName("ジャンプスピード")]
	public float _JumpSpeed = 5.0f;

	/// <summary>
	/// 重力
	/// </summary>
	[InspectorName("重力加速度")]
	public float _Gravity = 0.05f;

	/// <summary>
	/// 回転更新を行うか否か
	/// </summary>
	[InspectorName("回転更新を行うか否か")]
	public bool _IsRotationUpdate = true;

	/// <summary>
	/// 現在の回転
	/// </summary>
	private Vector3 _CurRotation = Vector3.zero;

	/// <summary>
	/// 現在の移動量
	/// </summary>
	private Vector3 _CurMoveDir = Vector3.zero;

	private CharacterController _CharacterController = null;

	public GameObject _PlayerCamera = null;

	private Transform _RotationObject = null;

	#endregion // フィールド

	/// <summary>
	/// 初期設定
	/// </summary>
	void Start()
	{
		_CharacterController = GetComponent<CharacterController>();

		_RotationObject = gameObject.transform.Find("RotationObject");
	}

	/// <summary>
	/// 更新処理
	/// </summary>
	void Update()
	{
		// 回転処理
		if (_IsRotationUpdate)
		{
			updateRotation();
		}

		// 移動更新
		updateMove();
	}

	#region 非公開メソッド

	/// <summary>
	/// Rotaion更新処理
	/// </summary>
	private void updateRotation()
	{
		// マウス移動量の取得
		_CurRotation.x += Input.GetAxis("Mouse Y") * _RotationSpeed;
		_CurRotation.y += Input.GetAxis("Mouse X") * _RotationSpeed;

		// Componentの追加されたGameObejctに対して回転
		_RotationObject.rotation = Quaternion.Euler(_CurRotation.x, _CurRotation.y, _CurRotation.z);
	}

	/// <summary>
	/// 移動更新処理
	/// </summary>
	private void updateMove()
	{
		// 地面に設置
		if (_CharacterController.isGrounded)
		{
			var h = Input.GetAxis("Horizontal");
			var v = Input.GetAxis("Vertical");

			// 移動方向
			_CurMoveDir = v * _PlayerCamera.transform.forward + h * _PlayerCamera.transform.right;
			_CurMoveDir = Vector3.Scale(_CurMoveDir, new Vector3(1, 0, 1).normalized);
			//移動方向をローカルからワールド空間に変換する
			_CurMoveDir = transform.TransformDirection(_CurMoveDir);
			//移動速度を掛ける
			_CurMoveDir *= _MoveSpeed;

			// ジャンプ
			if (Input.GetButton("Jump"))
			{
				_CurMoveDir.y = _JumpSpeed;
			}
		}
		// 重力計算
		_CurMoveDir.y -= _Gravity;

		// 適用
		_CharacterController.Move(_CurMoveDir * Time.deltaTime);
	}

	#endregion
}

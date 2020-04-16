using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
	#region 定義

	/// <summary>
	/// 弾最大値
	/// </summary>
	[SerializeField]
	private const uint BulletMax = 20;

	/// <summary>
	/// 発砲間隔（時間）
	/// </summary>
	[SerializeField]
	private const float ShotTimeInterval = 0.2f;

	/// <summary>
	/// オーバーヒート待機時間（秒）
	/// </summary>
	[SerializeField]
	private const float OverheatWaitTime = 3.0f;

	/// <summary>
	/// 装填までにかかる時間
	/// 
	[SerializeField]
	private const float BulletChargeWaitTime = 1.0f;

	/// <summary>
	/// 装填間隔
	/// </summary>
	[SerializeField]
	private const float BulletChargeInterval = 0.1f;

	#endregion // 定義

	#region フィールド

	/// <summary>
	/// camera
	/// </summary>
	[InspectorName("ゲームメインカメラ")]
	public GameObject _PlayerCamera = null;

	/// <summary>
	/// 上下左右視点を切り替えると同時に回転させるオブジェクト
	/// </summary>
	private Transform _RotationObject = null;

	/// <summary>
	/// キャラクターコントローラー
	/// </summary>
	private CharacterController _CharacterController = null;

	//----------------------------------------------------
	// 移動管理
	//----------------------------------------------------

	/// <summary>
	/// 視点移動スピード
	/// </summary>
	[InspectorName("視点移動スピード")]
	[SerializeField]
	private float _RotationSpeed = 2.0f;

	/// <summary>
	/// 移動量スピード
	/// </summary>
	[InspectorName("移動スピード")]
	[SerializeField]
	private float _MoveSpeed = 2.2f;

	/// <summary>
	/// ジャンプスピード
	/// </summary>
	[InspectorName("ジャンプスピード")]
	[SerializeField]
	private float _JumpSpeed = 5.0f;

	/// <summary>
	/// 重力
	/// </summary>
	[InspectorName("重力加速度")]
	[SerializeField]
	private float _Gravity = 0.05f;

	/// <summary>
	/// 回転更新を行うか否か
	/// </summary>
	[InspectorName("回転更新を行うか否か")]
	[SerializeField]
	private bool _IsRotationUpdate = true;

	/// <summary>
	/// 現在の回転
	/// </summary>
	private Vector3 _CurRotation = Vector3.zero;

	/// <summary>
	/// 現在の移動量
	/// </summary>
	private Vector3 _CurMoveDir = Vector3.zero;

	//----------------------------------------------------
	// 弾管理
	//----------------------------------------------------

	/// <summary>
	/// 残弾数と最大装填数の割合（残弾数/装填数）
	/// </summary>
	public float BulletNumRate
	{
		get
		{
			return (float)_BulletNum / (float)BulletMax;
		}
	}

	/// <summary>
	/// 現在の弾数
	/// </summary>
	[InspectorName("現在の弾数")]
	[SerializeField]
	private uint _BulletNum = BulletMax;

	/// <summary>
	/// 発砲間隔タイマー
	/// </summary>
	private float _ShotTimer = 0.0f;

	/// <summary>
	/// オーバーヒートタイマー
	/// </summary>
	private float _OverheatTimer = 0.0f;

	/// <summary>
	/// 装填待機タイマー
	/// </summary>
	private float _BulletChargeTimer = 0.0f;

	/// <summary>
	/// 装填間隔タイマー
	/// </summary>
	private float _BulletChargeIntervalTimer = 0.0f;

	/// <summary>
	/// 発砲更新処理管理
	/// </summary>
	private delegate void UpdateShotCallback();
	private UpdateShotCallback _UpdateShotCallback = null;

	#endregion // フィールド

	/// <summary>
	/// 初期設定
	/// </summary>
	void Start()
	{
		_CharacterController = GetComponent<CharacterController>();

		_RotationObject = gameObject.transform.Find("RotationObject");

		// 初期化
		_BulletNum = BulletMax;
		_ShotTimer = 0.0f;
		_OverheatTimer = 0.0f;
		_BulletChargeTimer = 0.0f;
		_BulletChargeIntervalTimer = 0.0f;

		_UpdateShotCallback = updateShotIdleCallback;
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

		// 弾
		updateShot();
	}

	#region 非公開メソッド

	//----------------------------------------------------
	// camera管理
	//----------------------------------------------------

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

	//----------------------------------------------------
	// Move管理
	//----------------------------------------------------

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

	//----------------------------------------------------
	// Shot管理
	//----------------------------------------------------

	/// <summary>
	/// 発砲
	/// </summary>
	private void updateShot()
	{
		if (_UpdateShotCallback == null) 
		{
			Debug.Assert(false, "_UpdateShotCallback is null...");
			return;
		}

		_UpdateShotCallback.Invoke();
	}

	/// <summary>
	/// 発砲可能状態
	/// </summary>
	private void updateShotIdleCallback()
	{
		bool isInput = false;

		// Shot
		if (Input.GetMouseButton(0))
		{
			isInput = true;
			_BulletChargeTimer = 0.0f;
			_BulletChargeIntervalTimer = 0.0f;

			_ShotTimer += Time.deltaTime;
			if (_ShotTimer > ShotTimeInterval)
			{
				Debug.Log("ShotTest!BulletNum : " + _BulletNum.ToString());
				_ShotTimer = 0.0f;

				// 残弾を減らす
				_BulletNum--;
			}
		}

		// 残弾数無し
		if(_BulletNum <= 0)
		{
			_ShotTimer = 0.0f;
			_BulletChargeTimer = 0.0f;
			_BulletChargeIntervalTimer = 0.0f;

			// オーバーヒート
			_UpdateShotCallback = updateShotOverheatCallback;
			return;
		}
		// 装填
		else if (_BulletNum < BulletMax)
		{
			// 入力なし
			if (isInput == false)
			{
				// 装填までの待機タイマー
				_BulletChargeTimer += Time.deltaTime;
				if (_BulletChargeTimer > BulletChargeWaitTime)
				{
					// 装填
					updateBulletCharge();
				}
			}
		}
	}

	/// <summary>
	/// オーバーヒート状態
	/// </summary>
	private void updateShotOverheatCallback()
	{
		// オーバーヒート待機タイマー
		_OverheatTimer += Time.deltaTime;
		if (_OverheatTimer > OverheatWaitTime)
		{
			// 装填
			updateBulletCharge();

			// 装填完了したらIdle状態へ
			if (_BulletNum >= BulletMax)
			{
				_OverheatTimer = 0.0f;
				_UpdateShotCallback = updateShotIdleCallback;
			}
		}
	}

	/// <summary>
	/// 装填
	/// </summary>
	private void updateBulletCharge()
	{
		// 装填間隔タイマー
		_BulletChargeIntervalTimer += Time.deltaTime;
		if (_BulletChargeIntervalTimer > BulletChargeInterval)
		{
			_BulletNum++;
			_BulletChargeIntervalTimer = 0.0f;
		}
	}

	#endregion
}

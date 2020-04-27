using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControl : MonoBehaviour
{
	#region 定義

	/// <summary>
	/// 燃料カラー情報変更用のRenderer格納クラス
	/// </summary>
	class FuelRendererData
	{
		public int _MaterialIndex;
		public Renderer _Renderer;

		public FuelRendererData(int index, Renderer r)
		{
			_MaterialIndex = index;
			_Renderer = r;
		}
	}

	/// <summary>
	/// 燃料タンクオブジェクト最大数
	/// </summary>
	private static readonly uint FuelMax = 4;

	/// <summary>
	/// 燃料タンク１個分の割合
	/// </summary>
	private static readonly float FuelRateOnce = 1.0f / (float)FuelMax;

	#endregion // 定義

	#region フィールド

	/// <summary>
	/// 燃料タンクオブジェクトのリスト
	/// </summary>
	[InspectorName("燃料タンクオブジェクト格納")]
	public GameObject[] _FuelObjArray;

	/// <summary>
	/// 燃料タンクが動く移動量
	/// </summary>
	[InspectorName("燃料タンクの移動量")]
	[SerializeField]
	private Vector3 _FuelObjMoveTarget = new Vector3(0, 0.2f, 0);

	/// <summary>
	/// カラー変更対象の燃料タンク用マテリアル
	/// </summary>
	[InspectorName("カラー変更対象の燃料タンク用マテリアル")]
	[SerializeField]
	private Material _FuelOverheatMaterial;

	/// <summary>
	/// カラー変更対象の燃料タンクのカラーグラデーション
	/// </summary>
	[InspectorName("燃料カラーグラデーション")]
	[SerializeField, GradientUsage(true)]
	private Gradient _FuelOverheatGradient;

	/// <summary>
	/// 燃料タンクオブジェクト初期位置
	/// </summary>
	private Vector3[] _FuelObjDefaultLocalPosition = new Vector3[FuelMax];

	/// <summary>
	/// 燃料タンクのRendererリスト
	/// </summary>
	private List<FuelRendererData> _FuelRenderList;

	/// <summary>
	/// 燃料タンクのカラー情報
	/// </summary>
	private MaterialPropertyBlock _FuelOverheatMaterialPropertyBlock;

	/// <summary>
	/// PlayerComponent
	/// </summary>
	private PlayerBase _PlayerBaseComponent;

	/// <summary>
	/// マゼルフラッシュTransform
	/// </summary>
	[InspectorName("マゼルフラッシュTransform")]
	[SerializeField]
	private Transform _MuzelFlashTransform;

	/// <summary>
	/// 弾Prefab
	/// </summary>
	[InspectorName("弾Prefab")]
	[SerializeField]
	private BulletBase _BulletPrefab;

	#endregion // 定義

	/// <summary>
	/// 外部からの発砲命令
	/// </summary>
	public void Shot()
	{
		var obj = Instantiate(_BulletPrefab, _MuzelFlashTransform.position, Quaternion.LookRotation(_MuzelFlashTransform.forward));
		obj.Init(this);
	}

	// Start is called before the first frame update
    void Start()
    {
		var length = _FuelObjArray.Length;
		Debug.Assert(length > 0, "_FuelObjArray is none...");

		for (var i = 0; i < length; i++)
		{
			// 初期位置
			_FuelObjDefaultLocalPosition[i] = _FuelObjArray[i].transform.localPosition;
		}

		// Renderer
		var renderers = GetComponentsInChildren<Renderer>(true);
		if (renderers != null)
		{
			_FuelRenderList = new List<FuelRendererData>();
			foreach (var renderer in renderers)
			{
				var materialLength = renderer.sharedMaterials.Length;
				for (var i = 0; i < materialLength; i++)
				{
					if (renderer.sharedMaterials[i] == _FuelOverheatMaterial)
					{
						_FuelRenderList.Add(new FuelRendererData(i, renderer));
						Debug.Log("FuelRendererList Add " + renderer.name);
					}
				}
			}
		}
		_FuelOverheatMaterialPropertyBlock = new MaterialPropertyBlock();

		_PlayerBaseComponent = gameObject.GetComponent<PlayerBase>();
    }

    // Update is called once per frame
	void Update()
	{
		// 使用済み割合（1.0 - 残弾数 / 装填数）
		var rate = 1.0f - _PlayerBaseComponent.BulletNumRate;

		// 燃料位置アニメーション
		updateFuelAnim(rate);

		// 燃料カラーアニメーション
		updateFuelColorAnim(rate);
	}

	/// <summary>
	/// 残弾数に応じて燃料タンクのアニメーション制御(装填数MAXで1.0)
	/// </summary>
	/// <param name="rate"></param>
	private void updateFuelAnim(float rate)
	{
		var length = _FuelObjArray.Length;
		for (var i = 0; i < length; i++)
		{
			var r = 0.0f;
			if (rate >= FuelRateOnce)
			{
				r = FuelRateOnce;
			}
			else
			{
				r = rate;
			}

			var obj = _FuelObjArray[i];

			// アニメーション
			var fuelRate = r / FuelRateOnce; // 1個単位の割合に変換
			obj.transform.localPosition = _FuelObjDefaultLocalPosition[i] + _FuelObjMoveTarget * fuelRate;

			rate -= r;
		}
	}

	/// <summary>
	/// 残弾数に応じて燃料タンクのカラー制御
	/// </summary>
	/// <param name="rate"></param>
	private void updateFuelColorAnim(float rate)
	{
		foreach (var item in _FuelRenderList)
		{
			// 前設定情報を取得
			item._Renderer.GetPropertyBlock(_FuelOverheatMaterialPropertyBlock);

			// カラーを設定
			_FuelOverheatMaterialPropertyBlock.SetColor("_EmissionColor", _FuelOverheatGradient.Evaluate(rate));

			// RendererへPropertyBlockの設定
			item._Renderer.SetPropertyBlock(_FuelOverheatMaterialPropertyBlock, item._MaterialIndex);
		}
	}
}

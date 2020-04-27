using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
	/// <summary>
	/// 所有者
	/// </summary>
	private WeaponControl _Owner;

	/// <summary>
	/// 初期設定
	/// </summary>
	/// <param name="owner"></param>
	public void Init(WeaponControl owner)
	{
		_Owner = owner;
	}
}

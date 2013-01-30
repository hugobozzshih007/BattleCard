// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class FxmTestSingleMain : MonoBehaviour
{
	// -------------------------------------------------------------------------------------------
	public		GameObject[]	m_EffectPrefabs;
	public		int				m_nIndex;
	public		int				m_nCreateCount;
	public		float			m_fRandomRange;

	// -------------------------------------------------------------------------------------------
	void Awake()
	{
	}

	void OnEnable()
	{
	}

	void Start()
	{
// 		m_EffectPrefab = (GameObject)Resources.Load("test", typeof(GameObject));
// 		NcEffectBehaviour.PreloadTexture(m_EffectPrefab);
		Resources.UnloadUnusedAssets();
		Invoke("CreateEffect", 1);
	}

	void CreateEffect()
	{
		if (m_EffectPrefabs[m_nIndex] == null)
			return;

		float fRandomRange = 0;
		if (1 < m_nCreateCount)
			fRandomRange = m_fRandomRange;

		for (int n = 0; n < GetInstanceRoot().transform.GetChildCount(); n++)
			Destroy(GetInstanceRoot().transform.GetChild(n).gameObject);
		for (int n = 0; n < m_nCreateCount; n++)
		{
			GameObject createObj = (GameObject)Instantiate(m_EffectPrefabs[m_nIndex], new Vector3(Random.Range(-fRandomRange, fRandomRange), 0, Random.Range(-fRandomRange, fRandomRange)), Quaternion.identity);
			NcEffectBehaviour.PreloadTexture(createObj);
			createObj.transform.parent = GetInstanceRoot().transform;
#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9)
			NgObject.SetActiveRecursively(createObj, true);
#endif
		}
	}

	void Update()
	{
	}

	void OnGUI()
	{
		if (GUI.Button(GetButtonRect(0), "Next"))
		{
			if (m_nIndex < m_EffectPrefabs.Length-1)
				m_nIndex++;
			else m_nIndex = 0;
			CreateEffect();
		}
		if (GUI.Button(GetButtonRect(1), "Recreate"))
			CreateEffect();
	}

	public GameObject GetInstanceRoot()
	{
		return NcEffectBehaviour.GetRootInstanceEffect();
	}

	public static Rect GetButtonRect()
	{
		int		nButtonCount = 2;
		return new Rect(Screen.width-Screen.width/10*nButtonCount, Screen.height-Screen.height/10, Screen.width/10*nButtonCount, Screen.height/10);
	}
	public static Rect GetButtonRect(int nIndex)
	{
		return new Rect(Screen.width-Screen.width/10*(nIndex+1), Screen.height-Screen.height/10, Screen.width/10, Screen.height/10);
	}
}



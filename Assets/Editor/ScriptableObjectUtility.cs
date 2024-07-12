using UnityEngine;
using UnityEditor;

public static class ScriptableObjectUtility {
	
	/// <summary>
	/// 유니티에서 스크립터블 오브젝트를 생성하는 유틸리티 클래스 > 프로젝트 윈도우에서 선택된 폴더에 생성할 수 있음 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static void CreateAsset<T>() where T : ScriptableObject {
		var asset = ScriptableObject.CreateInstance<T>();
		ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
	}
	
}
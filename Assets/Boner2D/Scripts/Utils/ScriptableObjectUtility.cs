﻿/*
The MIT License (MIT)

Copyright (c) 2013 - 2019 Banbury & Play-Em

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace Boner2D {
	public static class ScriptableObjectUtility {
		/// <summary>
		//	This makes it easy to create, name and place unique new ScriptableObject asset files.
		/// </summary>
	#if UNITY_EDITOR
		public static void CreateAsset(Object asset) {
			CreateAsset(asset, "/New " + asset.GetType().ToString());
		}

		public static void CreateAsset(Object asset, string filename) {
			string path = (Selection.activeObject != null ? AssetDatabase.GetAssetPath(Selection.activeObject) : "Assets");

			if (string.IsNullOrEmpty(path)) {
				path = "Assets";
			}
			else if (!string.IsNullOrEmpty(Path.GetExtension(path))) {
				path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
			}

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + filename + ".asset");

			if (string.IsNullOrEmpty(assetPathAndName)) {
				assetPathAndName = "Assets/" + filename + ".asset";
			}

			AssetDatabase.CreateAsset(asset, assetPathAndName);


			AssetDatabase.SaveAssets();

			Selection.activeObject = asset;
		}

		public static void CreateAsset<T>() where T : ScriptableObject {
			T asset = ScriptableObject.CreateInstance<T>();

			string path = (Selection.activeObject != null ? AssetDatabase.GetAssetPath(Selection.activeObject) : "Assets");

			if (string.IsNullOrEmpty(path)) {
				path = "Assets";
			}
			else if (!string.IsNullOrEmpty(Path.GetExtension(path))) {
				path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
			}

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

			if (string.IsNullOrEmpty(assetPathAndName)) {
				assetPathAndName = "Assets/New " + typeof(T).ToString() + ".asset";
			}

			AssetDatabase.CreateAsset(asset, assetPathAndName);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
		}
	#endif
	}
}
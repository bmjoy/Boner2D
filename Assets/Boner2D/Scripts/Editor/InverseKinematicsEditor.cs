﻿/*
The MIT License (MIT)

Copyright (c) 2013 - 2018 Banbury & Play-Em & SirKurt

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
using UnityEditor;
using System.Collections;

namespace Boner2D {
	[InitializeOnLoad]
	[CustomEditor(typeof(InverseKinematics))]
	public class InverseKinematicsEditor : Editor {

		private InverseKinematics ik;

		static private Transform transform;
		static private Vector3 position;

		static private float handleSize;
		static private float discSize;


		static private Bone pb;
		static private float parentRotation;

		static private Vector3 from;
		static private Vector3 to;

		static private Vector3 toChild;

		static InverseKinematicsEditor() {
			SceneView.onSceneGUIDelegate += OnScene;
		}

		void OnEnable() {
			ik = (InverseKinematics)target;
		}

		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			if (GUILayout.Button("Create Target Helper")) {
				CreateHelper();
			}

			if (((InverseKinematics)target).target == null) {
				EditorGUILayout.HelpBox("Please select a target.", MessageType.Error);
			}
		}

		//Create a Helper for the IK Component and sets it as the IK's Target
		private void CreateHelper(){
			//Create the Helper GameObject named after the bone
			GameObject o = new GameObject (ik.name + "_IK");

			Undo.RegisterCreatedObjectUndo (o, "Create helper");

			o.AddComponent<Helper>();

			o.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

			//set the helpers position to match the bones position
			if (ik.bone != null) {
				o.transform.position = ik.bone.Head;
			}
			else {
				o.transform.position = ik.transform.position;
			}

			//set the helper as a child of the skeleton
			o.transform.parent = ik.bone.skeleton.transform;

			//set the helper as the target
			ik.target = o.transform;

			//selects the transform of the Helper
			Selection.activeTransform = o.transform;
		}

		//Angle Limit code adapted from Veli-Pekka Kokkonen's SimpleCCDEditor http://goo.gl/6oSzDx

		// Scales scene view gizmo, feel free to change ;)
		const float gizmoSize = 0.5f;

		static void OnScene(SceneView sceneview) {
			var targets = GameObject.FindObjectsOfType<InverseKinematics>();

			foreach (var target in targets) {
				if (Selection.activeGameObject != null) {
					if (target.gameObject.Equals(Selection.activeGameObject)) {
						foreach (var node in target.angleLimits) {
							if (node.Transform == null) {
								continue;
							}

							transform = node.Transform;
							position = transform.position;

							handleSize = HandleUtility.GetHandleSize(position);
							discSize = handleSize * gizmoSize;


							pb = transform.parent.GetComponent<Bone>();
							parentRotation = pb ? pb.transform.eulerAngles.z : 0;

							from = Quaternion.Euler(0, 0, Mathf.Min(node.from, node.to) + parentRotation) * Vector3.up;
							to = Quaternion.Euler(0, 0, Mathf.Max(node.from, node.to) + parentRotation) * Vector3.up;

							Handles.color = new Color(0, 1, 0, 0.1f);
							Handles.DrawWireDisc(position, Vector3.back, discSize);
							Handles.DrawSolidArc(position, Vector3.forward, (node.from < node.to) ? from : to, (360 + node.to - node.from) % 360, discSize);

							Handles.color = Color.green;
							Handles.DrawLine(position, position + from * discSize);
							Handles.DrawLine(position, position + to * discSize);

							toChild = transform.rotation * Vector3.up;
							Handles.DrawLine(position, position + toChild * discSize);
						}
					}
				}
			}
		}
	}
}

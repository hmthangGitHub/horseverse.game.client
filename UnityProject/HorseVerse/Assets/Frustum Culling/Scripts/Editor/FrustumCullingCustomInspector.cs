using UnityEngine;
using UnityEditor;

namespace FrustumCullingSpace
{
    [CustomEditor(typeof(FrustumCulling))]
    [CanEditMultipleObjects]
    public class FrustumCullingCustomInspector : Editor
    {
        SerializedProperty autoCatchCamera,
        mainCam,
        cameraLeftPad,
        cameraRightPad,
        cameraTopPad,
        cameraBottomPad,
        autoBuildObjects,
        distanceCull,
        distanceToCull,
        prioritizeDistance,
        distanceCullOnly;
        FrustumCulling[] scripts;
        
        void OnEnable()
        {
            autoCatchCamera = serializedObject.FindProperty("autoCatchCamera");
            mainCam = serializedObject.FindProperty("mainCam");
            cameraLeftPad = serializedObject.FindProperty("cameraLeftPad");
            cameraRightPad = serializedObject.FindProperty("cameraRightPad");
            cameraTopPad = serializedObject.FindProperty("cameraTopPad");
            cameraBottomPad = serializedObject.FindProperty("cameraBottomPad");
            autoBuildObjects = serializedObject.FindProperty("autoBuildObjects");
            distanceCull = serializedObject.FindProperty("distanceCull");
            distanceToCull = serializedObject.FindProperty("distanceToCull");
            prioritizeDistance = serializedObject.FindProperty("prioritizeDistance");
            distanceCullOnly = serializedObject.FindProperty("distanceCullOnly");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update ();

            var button = GUILayout.Button(Resources.Load("FrustumCullingArtwork") as Texture, GUILayout.Width(355), GUILayout.Height(200));
            EditorGUILayout.HelpBox("Don't forget to leave a nice review if you like this tool. It helps alot! Press on the image to get to the store page.", MessageType.Info);

            if (button) Application.OpenURL("http://u3d.as/2cpk");

            FrustumCulling script = (FrustumCulling) target;
            
            Object[] monoObjects = targets;
            scripts = new FrustumCulling[monoObjects.Length];
            for (int i = 0; i < monoObjects.Length; i++) {
                scripts[i] = monoObjects[i] as FrustumCulling;
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(autoCatchCamera, new GUIContent("Auto Catch Camera", "Automatically get the main active camera on script awake."));
            EditorGUI.BeginDisabledGroup(script.autoCatchCamera == true);
                EditorGUILayout.PropertyField(mainCam, new GUIContent("Main Cam", "Manually drag and drop the game camera here, better for performance on game start"));
            EditorGUI.EndDisabledGroup ();

            EditorGUILayout.Space(3);

            EditorGUI.BeginDisabledGroup(script.distanceCullOnly == true);
                EditorGUILayout.PropertyField(cameraLeftPad, new GUIContent("Camera Left Pad", "The padding on the left side of the camera. When an object exceeds this padding it will be disabled."));
                EditorGUILayout.PropertyField(cameraRightPad, new GUIContent("Camera Right Pad", "The padding on the right side of the camera. When an object exceeds this padding it will be disabled."));
                EditorGUILayout.PropertyField(cameraTopPad, new GUIContent("Camera Top Pad", "The padding on the top side of the camera. When an object exceeds this padding it will be disabled."));
                EditorGUILayout.PropertyField(cameraBottomPad, new GUIContent("Camera Bottom Pad", "The padding on the bottom side of the camera. When an object exceeds this padding it will be disabled."));
            EditorGUI.EndDisabledGroup ();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Objects Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(autoBuildObjects, new GUIContent("Auto Build Objects", "Automatically builds all the secondary game objects."));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Distance Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(distanceCull, new GUIContent("Distance Cull", "Check whether culling should take distance into consideration. Distance culling only happens when object is outside view."));
            
            EditorGUI.BeginDisabledGroup(script.distanceCull == false);
                EditorGUILayout.PropertyField(distanceToCull, new GUIContent("Distance To Cull", "The distance if exceeded the object will always be culled"));
                EditorGUILayout.PropertyField(prioritizeDistance, new GUIContent("Prioritize Distance", "If set to true, the object will disable instantly when it exceeds the distance even when still in view."));
                EditorGUILayout.PropertyField(distanceCullOnly, new GUIContent("Distance Cull Only", "If set to true, culling will only occur when distance is exceeded. Objects out of view won't be disabled."));
            EditorGUI.EndDisabledGroup ();

            EditorGUI.BeginDisabledGroup(script.autoBuildObjects == true);
                if(GUILayout.Button("Build Objects")) {
                    foreach (var item in scripts)
                    {
                        item.BuildObjects();
                    }
                }
            EditorGUI.EndDisabledGroup ();

            serializedObject.ApplyModifiedProperties();
        }
    }
}

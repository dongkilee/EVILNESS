//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(Obstacle), true)]
//[CanEditMultipleObjects]
//public class ObstacleEditor : Editor
//{

//    public override void OnInspectorGUI()
//    {
//        Obstacle obst = (Obstacle)target;
//        DrawDefaultInspector();

//        switch (obst.obstacleType)
//        {
//            case ObstacleType.Line:
//                {
//                    EditorGUI.indentLevel++;
//                    obst.pointA = (Transform)EditorGUILayout.ObjectField("Point A", obst.pointA, typeof(Transform), true);
//                    obst.pointB = (Transform)EditorGUILayout.ObjectField("Point B", obst.pointB, typeof(Transform), true);
//                    EditorGUI.indentLevel--;
//                }
//                break;
//            case ObstacleType.Sphere:
//                {
//                    EditorGUI.indentLevel++;
//                    obst.radius = EditorGUILayout.FloatField("Radius", obst.radius);
//                    EditorGUI.indentLevel--;
//                }
//                break;

//        }

//        if (GUI.changed)
//        {
//            EditorUtility.SetDirty(obst);
//            serializedObject.ApplyModifiedProperties();
//        }
//    }
//}
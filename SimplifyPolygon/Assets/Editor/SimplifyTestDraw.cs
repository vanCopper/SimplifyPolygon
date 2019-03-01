using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimplifyTest))]
public class SimplifyTestDraw : Editor
{
    private GUIStyle m_labelStyle = new GUIStyle();
    private void OnSceneGUI()
    {
        SimplifyTest simplifyTest = (SimplifyTest)target;
        if (simplifyTest == null || simplifyTest.simplifyMesh == null) return;

        m_labelStyle.fontSize = 14;
        m_labelStyle.normal.textColor = Color.cyan;
        //m_labelStyle.
        
        //labelStyle.fontStyle
        Handles.BeginGUI();
        GUILayout.BeginVertical();
        
        GUILayout.Label("RenderVerticesNum: " + simplifyTest.simplifyMesh.renderVerticesNum, m_labelStyle);
        GUILayout.Space(2);
        GUILayout.Label("RenderTrianglesNum: " + simplifyTest.simplifyMesh.renderTrianglesNum, m_labelStyle);
        GUILayout.Space(2);
        GUILayout.Label("OriginVerticesNum: " + simplifyTest.simplifyMesh.originVerticesNum, m_labelStyle);
        GUILayout.Space(2);
        GUILayout.Label("OriginTrianglesNum: " + simplifyTest.simplifyMesh.originTrianglesNum, m_labelStyle);


        GUILayout.EndVertical();
        Handles.EndGUI();
    }
}

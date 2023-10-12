#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundler : EditorWindow
{
    string path = "Assets/StreamingAssets";
    BuildAssetBundleOptions buildOption = BuildAssetBundleOptions.None;
    BuildTarget target = BuildTarget.StandaloneWindows;
    string logs = "";

    [MenuItem("Custom Tools/Build Asset Bundles")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AssetBundler));
    }

    private void OnGUI()
    {
        GUILayout.Label("Asset Bundles Builder", EditorStyles.boldLabel);

        path = EditorGUILayout.TextField("Bundle Result Directory", path);
        buildOption = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("Build Option", buildOption);
        target = (BuildTarget)EditorGUILayout.EnumFlagsField("Target Platform", target);

        // Help
        if (GUILayout.Button("Asset Bundles Builder Help"))
        {
            logs = "Asset Bundles Builder Manual";
            logs += "\n\nBundle Result Directory";
            logs += "\n >> result file will save at ~~";
            logs += "\n\nBuild Option";
            logs += "\n >> None == LZMA";
            logs += "\n >> ChunkBasedCompression == LZ4(most used)";
            logs += "\n\nTarget Platform";
            logs += "\n >> result file will available at ~~";
        }
        if (GUILayout.Button("Asset Bundles Builder Help(KOR)"))
        {
            logs = "Asset Bundles Builder ����";
            logs += "\n\nBundle Result Directory";
            logs += "\n >> ��� ���ϵ��� �����Ǵ� ��ġ";
            logs += "\n\nBuild Option";
            logs += "\n >> None == LZMA ǥ�� ����";
            logs += "\n >> ChunkBasedCompression == LZ4 ûũ�� ����(�ַ� ����)";
            logs += "\n\nTarget Platform";
            logs += "\n >> �÷��� ���߱�";
        }
        GUILayout.Label(logs);

        if (GUILayout.Button("Build Asset Bundles"))
        {
            BuildAsset(path, buildOption, target);
            logs = "Asset Bundles Builder: Job Done.";
        }
    }

    static void BuildAsset(string path_, BuildAssetBundleOptions buildOption_, BuildTarget target_)
    {
        BuildPipeline.BuildAssetBundles(path_, buildOption_, target_);
    }
}
#endif
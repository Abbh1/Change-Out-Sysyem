using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 角色预制件保存器 - 编辑器工具，用于将当前角色配置保存为预制件
    /// </summary>
    public class CharacterPrefabSaver : MonoBehaviour
    {
#if UNITY_EDITOR
        public GameObject characterRoot;      // 角色根对象
        public string saveFolderPath = "Assets/SavedPrefabs";  // 保存路径

        /// <summary>
        /// 将当前角色配置保存为预制件
        /// </summary>
        public void SaveAsPrefab()
        {
            // 确保保存目录存在
            if (!Directory.Exists(saveFolderPath))
                Directory.CreateDirectory(saveFolderPath);

            // 生成带时间戳的预制件路径
            var prefabPath = saveFolderPath + "/Character_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".prefab";

            // 创建角色副本并重置旋转
            var characterCopy = Instantiate(characterRoot);
            characterCopy.transform.rotation = Quaternion.identity;
            
            // 获取所有子对象
            var objects = characterCopy.transform.GetComponentsInChildren<Transform>(true);

            // 移除所有未激活的对象（未选中的部位）
            for (int i = objects.Length - 1; i >= 0; i--)
            {
                if (!objects[i].gameObject.activeSelf)
                {
                    DestroyImmediate(objects[i].gameObject);
                }
            }

            // 移除CharacterPart脚本组件
            var scriptsToRemove = characterCopy.GetComponentsInChildren<CharacterPart>(true);
            foreach (var script in scriptsToRemove)
            {
                if (script.transform.childCount <= 0)
                {
                    DestroyImmediate(script.gameObject);  // 删除空对象
                }
                else
                {
                    DestroyImmediate(script);   // 仅删除脚本组件
                }
            }

            // 保存为预制件
            PrefabUtility.SaveAsPrefabAsset(characterCopy, prefabPath);
            DestroyImmediate(characterCopy);

            // 在Project窗口中选中并高亮显示保存的预制件
            AssetDatabase.Refresh();
            Object savedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            Selection.activeObject = savedPrefab;
            EditorGUIUtility.PingObject(savedPrefab);

            Debug.Log("Prefab Saved: " + prefabPath);
        }
#endif
    }
}
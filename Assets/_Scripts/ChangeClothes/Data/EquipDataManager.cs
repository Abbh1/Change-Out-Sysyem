using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace ChangeClothes.Data
{
    public class EquipDataManager : MonoBehaviour
    {
        public static EquipDataManager Instance { get; private set; }

        private const string EquipDataKey = "ChangeClothes_EquipData";

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 保存换装配置到 PlayerPrefs
        /// </summary>
        /// <param name="equipData">部位类型 -> 索引 的字典</param>
        public void SaveEquipData(Dictionary<PartType, int> equipData)
        {
            if (equipData == null || equipData.Count == 0)
            {
                Debug.LogWarning("SaveEquipData: 换装数据为空");
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var kvp in equipData)
            {
                if (sb.Length > 0) sb.Append("|");
                sb.Append($"{(int)kvp.Key}:{kvp.Value}");
            }

            PlayerPrefs.SetString(EquipDataKey, sb.ToString());
            PlayerPrefs.Save();
            Debug.Log($"换装配置已保存: {sb.ToString()}");
        }

        /// <summary>
        /// 检查是否有保存的换装配置
        /// </summary>
        public bool HasSavedEquip()
        {
            return PlayerPrefs.HasKey(EquipDataKey);
        }

        /// <summary>
        /// 加载保存的换装配置
        /// </summary>
        public Dictionary<PartType, int> LoadEquipData()
        {
            var result = new Dictionary<PartType, int>();

            if (!PlayerPrefs.HasKey(EquipDataKey))
            {
                Debug.Log("LoadEquipData: 没有保存的换装配置");
                return result;
            }

            string savedData = PlayerPrefs.GetString(EquipDataKey);
            if (string.IsNullOrEmpty(savedData))
            {
                Debug.LogWarning("LoadEquipData: 保存的数据为空");
                return result;
            }

            string[] parts = savedData.Split('|');
            foreach (string part in parts)
            {
                string[] kv = part.Split(':');
                if (kv.Length == 2 && int.TryParse(kv[0], out int typeInt) && int.TryParse(kv[1], out int index))
                {
                    PartType partType = (PartType)typeInt;
                    result[partType] = index;
                }
            }

            return result;
        }

        /// <summary>
        /// 加载并应用换装配置
        /// </summary>
        public void LoadAndApplyEquip()
        {
            var equipData = LoadEquipData();
            if (equipData.Count == 0) return;

            foreach (var kvp in equipData)
            {
                EventController.Instance.RaisePartChanged(kvp.Key, kvp.Value);
            }

            Debug.Log("换装配置已加载并应用");
        }

        /// <summary>
        /// 清除保存的换装配置
        /// </summary>
        public void ClearSavedEquip()
        {
            PlayerPrefs.DeleteKey(EquipDataKey);
            PlayerPrefs.Save();
            Debug.Log("换装配置已清除");
        }
    }
}
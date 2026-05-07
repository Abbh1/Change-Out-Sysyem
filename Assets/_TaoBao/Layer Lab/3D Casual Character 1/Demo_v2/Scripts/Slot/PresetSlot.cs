using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 预设槽位 - 保存和加载角色配置预设
    /// </summary>
    public class PresetSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image imageCharacter;  // 预设预览图
        private Dictionary<PartType, int> _itemList = new();  // 预设配置数据
        private int _index;                               // 预设索引
        public bool isPreset;                             // 是否已保存预设

        /// <summary>
        /// 初始化预设槽位
        /// </summary>
        public void InitSlot(int index)
        {
            _index = index;
            LoadPreset();
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 点击处理
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
#if UNITY_EDITOR
            // 右键点击清除预设
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                ClearPreset();
                return;
            }

            // 如果没有预设，保存当前配置为预设
            if (!isPreset)
            {
                isPreset = true;
                TakePreset();
                return;
            }
#endif

            // 应用预设配置
            foreach (var item in _itemList)
            {
                Demo2Character.Instance.OnPreset.Invoke();
                Demo2Character.Instance.OnPartChanged.Invoke(item.Key, item.Value);
            }

            Debug.Log($"Preset {_index} equiped successfully: {string.Join(", ", _itemList)}");
        }

        /// <summary>
        /// 保存当前配置为预设（带截图）
        /// </summary>
        public async void TakePreset()
        {
            await ScreenShot.Instance.ScreenShotClickAsync($"{_index}");

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            _itemList = Demo2Character.Instance.CurrentPartsTypeAndNameList();
            imageCharacter.sprite = UIControl.GetSprite($"{ScreenShot.Instance.projectNameFolderName}/Preset/{_index}");
            DemoControl.Instance.PresetData.SavePreset(_index, _itemList);
        }

        /// <summary>
        /// 从文件加载Sprite
        /// </summary>
        private Sprite LoadSprite(string path)
        {
            if (!System.IO.File.Exists(path)) return null;

            byte[] fileData = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// 加载预设数据
        /// </summary>
        private void LoadPreset()
        {
            _itemList = DemoControl.Instance.PresetData.LoadPreset(_index);
            if (_itemList.Count > 0)
            {
                isPreset = true;
                imageCharacter.sprite = UIControl.GetSprite($"{DemoControl.Instance.ItemImagePath}/Preset/{_index}");
                Debug.Log($"Preset {_index} loaded successfully: {string.Join(", ", _itemList)}");
            }
            else
            {
                isPreset = false;
                Debug.LogWarning($"No preset found for index {_index}");
            }
        }

        /// <summary>
        /// 清除预设
        /// </summary>
        private void ClearPreset()
        {
            isPreset = false;
            imageCharacter.sprite = null;
            DemoControl.Instance.PresetData.ClearPreset(_index);
        }
    }
}
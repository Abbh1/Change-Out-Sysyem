using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChangeClothes.Avatar
{
    /// <summary>
    /// AvatarSystem 专用角色部位数据对象
    /// </summary>
    public class CharacterPart
    {
        public PartType PartType { get; set; } = PartType.None;
        public List<GameObject> CurrentPartsObjects { get; set; } = new();
        public List<int> PartIndexes { get; set; } = new();
        public bool IsOnlyEquip { get; set; }
        public float EquipChance { get; set; } = 50f;
        public int CurrentIndex { get; set; } = -1;

        public string CurrentName()
        {
            return CurrentIndex < 0 || CurrentIndex >= CurrentPartsObjects.Count
                ? string.Empty
                : CurrentPartsObjects[CurrentIndex].name;
        }

        public int CurrentPartNumber =>
            CurrentIndex < 0 || CurrentIndex >= PartIndexes.Count
                ? -1
                : PartIndexes[CurrentIndex];

        public void ApplyCurrentPart()
        {
            if (AvatarSystem.Instance == null) return;
            int partNumber = CurrentPartNumber;
            if (partNumber < 0) return;
            AvatarSystem.Instance.ChangeMesh(PartType.ToString(), partNumber);
            AvatarSystem.Instance.RaisePartChangedEvent(PartType, partNumber);
        }

        public void SetCurrentIndex(int index)
        {
            if (index < 0 || index >= CurrentPartsObjects.Count)
            {
                CurrentIndex = -1;
                AvatarSystem.Instance?.RaisePartChangedEvent(PartType, -1);
                return;
            }

            CurrentIndex = index;
            ApplyCurrentPart();
        }

        public void SetupFromDictionary(Dictionary<int, SkinnedMeshRenderer> partArray)
        {
            CurrentPartsObjects.Clear();
            PartIndexes = partArray.Keys.OrderBy(x => x).ToList();
            foreach (int partNumber in PartIndexes)
            {
                CurrentPartsObjects.Add(partArray[partNumber].gameObject);
            }
            CurrentIndex = PartIndexes.Count > 0 ? 0 : -1;
        }
    }
}

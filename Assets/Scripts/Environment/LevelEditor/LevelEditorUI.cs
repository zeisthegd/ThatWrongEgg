using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using Penwyn.Game;

namespace Penwyn.LevelEditor
{
    public class LevelEditorUI : MonoBehaviour
    {
#if UNITY_EDITOR
        public Button TileButtonPrefab;
        public Transform TileButtonsContainer;


        void Start()
        {
            LoadPlaceableObjects(LevelEditor.Instance.PlaceableObjects);
        }

        public void LoadPlaceableObjects(List<LevelBlock> levelBlocks)
        {
            foreach (LevelBlock block in levelBlocks)
            {
                Button newBtn = Instantiate(TileButtonPrefab, TileButtonsContainer.position, Quaternion.identity, TileButtonsContainer);
                newBtn.GetComponent<RawImage>().texture = AssetPreview.GetAssetPreview(block.gameObject);
                newBtn.onClick.AddListener(() =>
                {
                    LevelEditor.Instance.ChooseBlockToPlace(block);
                });
            }
        }
#endif
    }
}


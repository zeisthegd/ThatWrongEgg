using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

using NaughtyAttributes;
using DG.Tweening;

using Penwyn.Tools;
using Penwyn.LevelEditor;

namespace Penwyn.Game
{
    public class LevelManager : SingletonMonoBehaviour<LevelManager>
    {
        public List<LevelBlock> PlaceableObjects;
        public List<LevelBlock> PlacedBlocks;
        public LevelDataList LevelDataList;

        public int CurrentLevelIndex = 0;

        public virtual void LoadNextLevel()
        {
            if (CurrentLevelIndex + 1 < LevelDataList.List.Count)
                LoadLevelByIndex(CurrentLevelIndex + 1);
            else
                Announcer.Instance.Announce("There no level left! Thank you for playing!");
        }

        public virtual void LoadLevelByIndex(int index)
        {
            LoadLevel(LevelDataList.List[index]);
            CurrentLevelIndex = index;
        }

        public virtual void LoadLevel(LevelData data)
        {
            StartCoroutine(LoadLevelCoroutine(data));
        }

        /// <summary>
        /// Delete all blocks, create new ones, move players into positions.
        /// </summary>
        protected virtual IEnumerator LoadLevelCoroutine(LevelData data)
        {
            CleanAllBlocks();
            CreateBlocks(data);
            yield return new WaitForSeconds(0.25F);
            MoveLocalPlayerIntoPosition();
        }

        protected virtual void CreateBlocks(LevelData data)
        {
            foreach (TileData tile in data.BlockMap)
            {
                LevelBlock block = GetBlockByID(tile.BlockID);
                LevelBlock blockObj = Instantiate(block);
                blockObj.transform.position = tile.Position;
                blockObj.transform.eulerAngles = new Vector3(block.transform.eulerAngles.x, tile.RotationAngleY, block.transform.eulerAngles.z);
                PlacedBlocks.Add(blockObj);
            }
        }

        protected virtual void MoveLocalPlayerIntoPosition()
        {
            Debug.Log(PlacedBlocks.Count);
            string spawnTag = "";
            Player[] teamMembers;

            if (PlayerManager.Instance.LocalPlayerIsFirstTeam)
            {
                spawnTag = "FirstTeamSpawn";
                teamMembers = CombatManager.Instance.FirstTeam.Players;
            }
            else
            {
                spawnTag = "SecondTeamSpawn";
                teamMembers = CombatManager.Instance.SecondTeam.Players;
            }

            GameObject[] spawns = GameObject.FindGameObjectsWithTag(spawnTag);
            PlayerManager.Instance.LocalPlayer.transform.DOMove(spawns[Array.IndexOf(teamMembers, PhotonNetwork.LocalPlayer)].transform.position, 1);
        }

        /// <summary>
        /// Get a block by its ID.
        /// </summary>
        public virtual LevelBlock GetBlockByID(string blockID)
        {
            for (int i = 0; i < PlaceableObjects.Count; i++)
            {
                if (PlaceableObjects[i].BlockID == blockID)
                    return PlaceableObjects[i];
            }
            return null;
        }

        /// <summary>
        /// Destroy all placed blocks
        /// </summary>
        [Button("Clean All Blocks", EButtonEnableMode.Playmode)]
        public virtual void CleanAllBlocks()
        {
            for (int i = 0; i < PlacedBlocks.Count; i++)
            {
                Destroy(PlacedBlocks[i].gameObject);
            }
            PlacedBlocks.Clear();
        }
    }
}

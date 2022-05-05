using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penwyn.LevelEditor
{
    [CreateAssetMenu(menuName ="Level Editor/Level Data List")]
    public class LevelDataList : ScriptableObject
    {
        public List<LevelData> List;
        public void Add(LevelData settings)
        {
            LevelData gameSettings = List.Find(x => (x.LevelName == settings.LevelName));
            if (gameSettings != null)
                List.Remove(gameSettings);
            List.Add(settings);

            List = List.OrderBy(x => Convert.ToInt32(x.LevelName)).ToList();
        }
    }

}

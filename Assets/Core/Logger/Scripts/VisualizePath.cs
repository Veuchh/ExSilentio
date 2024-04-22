using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Editor.Tools
{
    public class VisualizePath : MonoBehaviour
    {
        [SerializeField] TextAsset playerPositionsFile;
        [SerializeField] Color color = Color.red;

        List<Vector3> playerPositions = new List<Vector3>();

        [Button]
        void LoadPlayerPath()
        {

            var lines = playerPositionsFile.text.Split('\n');
            foreach (var line in lines)
            {
                var coordinates = line.Split(';');

                if (line.Length > 1)
                {
                    playerPositions.Add(new Vector3(float.Parse(coordinates[0]), float.Parse(coordinates[1]), float.Parse(coordinates[2])));
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            for (int i = 0; i < playerPositions.Count - 1; i++)
            {
                Gizmos.color = color;
                Gizmos.DrawLine(playerPositions[i], playerPositions[i + 1]);
            }
        }
#endif
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace LW.Data
{
    public class HintDatabase : ScriptableObject
    {
        [SerializeField] List<string> hintKeys;

        public List<string> HintKeys => hintKeys;
    }
}
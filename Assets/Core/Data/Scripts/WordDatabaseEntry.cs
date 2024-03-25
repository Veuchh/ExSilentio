using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace LW.Data
{
    [Serializable]
    public class WordDatabaseEntry
    {
        [SerializeField] WordID id;
        [SerializeField] List<WordID> additionalAcceptedIds;
        [SerializeField] List<WordID> closelySemanticIds;
        [TextArea, SerializeField] string gdNotes;

        public WordID ID { get => id; set => id = value; }
        public List<WordID> AdditionalAcceptedIds { get => additionalAcceptedIds; set => additionalAcceptedIds = value; }
        public List<WordID> CloselySemanticIds { get => closelySemanticIds; set => closelySemanticIds = value; }
        public string GdNotes { get => gdNotes; set => gdNotes = value; }
    }
}
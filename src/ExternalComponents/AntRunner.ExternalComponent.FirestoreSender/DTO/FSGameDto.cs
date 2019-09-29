using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntRunner.ExternalComponent.FirestoreSender.DTO
{
    [FirestoreData]
    public class FSGameDto
    {
        [FirestoreProperty]
        public DateTime CreatedDate { get; set; }
        [FirestoreProperty]
        public string GameID { get; set; }

        [FirestoreProperty]
        public bool IsMapLegacy { get; set; } = true;

        [FirestoreProperty]
        public Blob MapData { get; set; }

        [FirestoreProperty]
        public bool IsGameRunning { get; set; }

        [FirestoreProperty]
        public long CurrentTick { get; set; }
    }
}

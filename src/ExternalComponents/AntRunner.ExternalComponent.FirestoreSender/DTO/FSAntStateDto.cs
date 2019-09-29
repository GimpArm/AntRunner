using AntRunner.Interface;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntRunner.ExternalComponent.FirestoreSender.DTO
{
    [FirestoreData]
    public class FSAntStateDto
    {
        [FirestoreProperty]
        public int? PositionX { get; set; }
        [FirestoreProperty]
        public int? PositionY { get; set; }

        [FirestoreProperty]
        public int Health { get; set; }
        [FirestoreProperty]
        public int Shields { get; set; }

        [FirestoreProperty]
        public AntAction Action { get; set; }
    }
}

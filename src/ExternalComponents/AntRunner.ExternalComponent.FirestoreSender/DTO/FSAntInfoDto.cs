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
    public class FSAntInfoDto
    {
        [FirestoreProperty]
        public string ID { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Color { get; set; }

        [FirestoreProperty]
        public FSAntStateDto LastState { get; set; }

        [FirestoreProperty]
        public FSAntStateDto[] States { get; set; }
    }
}

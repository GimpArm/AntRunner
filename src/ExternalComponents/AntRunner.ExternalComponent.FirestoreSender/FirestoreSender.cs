using AntRunner.ExternalComponent.FirestoreSender.DTO;
using AntRunner.Game.Interface;
using AntRunner.Interface;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AntRunner.ExternalComponent.FirestoreSender
{
    public class FirestoreSender : IExternalComponent, IGameEventHook
    {
        #region Interface Members
        public string DisplayText => string.Empty;

        public IGameEventHook GameEventHook => this;

        public bool IsActiv => true;

        public bool IsAutoRun => true;
        #endregion

        private FirestoreDb _storeDB;
        private string _projectID = "antrunner-display";

        private Guid? _currentGameID;
        private CollectionReference _currentGameFSCollection;

        private FSGameDto _currentGameDto;
        private DocumentReference _currentGameFSReference;

        private Dictionary<Guid, DocumentReference> _playerFSReferenceDict = new Dictionary<Guid, DocumentReference>();

        #region IExternalComponent
        public void Start()
        {
            StartFirestoreClient();

        }

        public void Stop()
        {
        }
        #endregion

        #region IGameEventHook
        public async void CreateGame(Guid gameID)
        {
            _currentGameID = gameID;
            _currentGameFSCollection = _storeDB.Collection($"{DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm")}_{gameID}");

            _currentGameDto = new FSGameDto
            {
                CreatedDate = DateTime.UtcNow,
                GameID = gameID.ToString()
            };

            _currentGameFSReference = _currentGameFSCollection.Document("GameInfo");
            await _currentGameFSReference.SetAsync(_currentGameDto);
        }

        public async void SetMap(byte[] mapArray)
        {
            _currentGameDto.MapData = Blob.CopyFrom(mapArray);
            await _currentGameFSReference.SetAsync(_currentGameDto, SetOptions.Overwrite);
        }

        public async void SetPlayerAction(AntState antState)
        {
            DocumentReference playerFSReference;
            if (!_playerFSReferenceDict.TryGetValue(antState.ID, out playerFSReference))
            {
                playerFSReference = _currentGameFSCollection.Document($"Player_{antState.ID}");
                _playerFSReferenceDict.Add(antState.ID, playerFSReference);
                var sendObject = new FSAntInfoDto
                {
                    ID = antState.ID.ToString(),
                    Name = antState.Name
                };
                await playerFSReference.SetAsync(sendObject);
            }

            await playerFSReference.UpdateAsync($"States.{antState.CurrentTick}", new FSAntStateDto
            {
                Action = antState.LastAction,
                PositionX = antState.PositionX,
                PositionY = antState.PositionY,
                Health = antState.Health,
                Shields = antState.Shields
            });

            Dictionary<string, object> updateData = new Dictionary<string, object>();
            updateData.Add($"States.{antState.CurrentTick}", new FSAntStateDto
            {
                Action = antState.LastAction,
                PositionX = antState.PositionX,
                PositionY = antState.PositionY,
                Health = antState.Health,
                Shields = antState.Shields
            });
            updateData.Add("LastState", new FSAntStateDto
            {
                Action = antState.LastAction,
                PositionX = antState.PositionX,
                PositionY = antState.PositionY,
                Health = antState.Health,
                Shields = antState.Shields
            });

            await playerFSReference.UpdateAsync(updateData);

            // Update Tick counter in GameInfo
            if (_currentGameDto.CurrentTick < antState.CurrentTick)
            {
                _currentGameDto.CurrentTick = antState.CurrentTick;
                await _currentGameFSReference.UpdateAsync("CurrentTick", antState.CurrentTick);
            }
        }

        public async void StartGame(Guid gameID)
        {
            _currentGameDto.IsGameRunning = true;
            await _currentGameFSReference.UpdateAsync("IsGameRunning", true);
        }

        public async void StopGame(Guid gameID)
        {
            _currentGameDto.IsGameRunning = false;
            await _currentGameFSReference.UpdateAsync("IsGameRunning", false);
        }
        #endregion

        private string LoadFireStoreData()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "AntRunner.ExternalComponent.FirestoreSender.Properties.FireStoreData.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private void StartFirestoreClient()
        {
            var credential = GoogleCredential.FromJson(LoadFireStoreData());
            var channelCredentials = credential.ToChannelCredentials();
            var channel = new Channel(FirestoreClient.DefaultEndpoint.ToString(), channelCredentials);
            var client = FirestoreClient.Create(channel);

            _storeDB = FirestoreDb.Create(_projectID, client);
        }
    }
}

using Assets.Scripts.Game.Shared;
using Assets.Scripts.Messages;
using Assets.Scripts.Messages.ClientOrigin;
using Assets.Scripts.Messages.ServerOrigin;
using Assets.Scripts.Network.Messages.ClientOrigin.Inventory;
using Assets.Scripts.Network.Messages.ServerOrigin;
using Assets.Scripts.Network.Messages.ServerOrigin.Inventory;
using Assets.Scripts.Network.Messages.ServerOrigin.PickUp;
using Assets.Scripts.Network.Messages.ServerOrigin.PickUp.PickUpData;
using Assets.Scripts.Network.Messages.ServerOrigin.PlayerState;
using Assets.Scripts.Network.Messages.ServerOrigin.Weapon;
using Assets.Scripts.Network.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityMultiplayer.Shared.Networking.Datagrams;
using UnityMultiplayer.Shared.Networking.Serializers;

namespace Assets.Scripts.Network.Shared.Serializers
{
    class PacketSerializer : BaseGameObjectSerializer
    {
        private readonly static HashSet<DatagramType> special = new HashSet<DatagramType>
        {
            DatagramType.ClientHandshakeRequest,
            DatagramType.ServerHandshakeResponse,
            DatagramType.Disconnect,
            DatagramType.UnreliableKeepAlive
        };

        private Dictionary<DatagramType, Action<DatagramHolder, BinaryWriter>> serializers;
        private Dictionary<DatagramType, Func<BinaryReader, object>> deserializers;

        public override void Initialize()
        {
            serializers = new Dictionary<DatagramType, Action<DatagramHolder, BinaryWriter>>()
            {
                { DatagramType.TransformsUpdate, WriteTransformsUpdateMessage },
                { DatagramType.RequestJoin, WriteRequestJoin },
                { DatagramType.PlayerJoin, WritePlayerJoin },
                { DatagramType.WorldState, WriteOnJoinWorldState },
                { DatagramType.MovePlayer, WriteMovePlayer },
                { DatagramType.PlayerShoot, WritePlayerShoot },
                { DatagramType.PlayersUpdate, WritePlayersUpdate },
                { DatagramType.LinkNameToID, WriteLinkPlayerNameToID },
                { DatagramType.PlayerHealthChange, WritePlayerHealthChange },
                { DatagramType.PlayerDeath, WritePlayerDeath },
                { DatagramType.PlayerSpawn, WritePlayerSpawn },
                { DatagramType.PlayerHit, WritePlayerHit },
                { DatagramType.PlayerKill, WritePlayerKill },
                { DatagramType.ClientReloadRequest, WriteClientReloadRequest },
                { DatagramType.ServerReloadResponse, WriteServerReloadResponse },
                { DatagramType.PickUpSpawned, WritePickUpSpawnedMessage },
                { DatagramType.PickUpPickedUp, WritePickUpPickedUpMessage },
                { DatagramType.InventoryDropSlotRequest, WriteInventorySlotMessage },
                { DatagramType.InventorySelectSlotRequest, WriteInventorySlotMessage },
                { DatagramType.InventoryDropSlotConfirm, WriteServerInventoryDropItemMessage },
                { DatagramType.InventorySelectSlotConfirm, WriteServerInventorySlotMessage },
            };

            deserializers = new Dictionary<DatagramType, Func<BinaryReader, object>>()
            {
                { DatagramType.TransformsUpdate, ReadTransformsUpdateMessage },
                { DatagramType.RequestJoin, ReadRequestJoin },
                { DatagramType.PlayerJoin, ReadPlayerJoin },
                { DatagramType.WorldState, ReadOnJoinWorldState },
                { DatagramType.MovePlayer, ReadMovePlayer },
                { DatagramType.PlayerShoot, ReadPlayerShoot },
                { DatagramType.PlayersUpdate, ReadPlayersUpdate },
                { DatagramType.LinkNameToID, ReadLinkPlayerNameToIDMessage },
                { DatagramType.PlayerHealthChange, ReadPlayerHealthChange },
                { DatagramType.PlayerDeath, ReadPlayerDeath },
                { DatagramType.PlayerSpawn, ReadPlayerSpawn },
                { DatagramType.PlayerHit, ReadPlayerHit },
                { DatagramType.PlayerKill, ReadPlayerKill },
                { DatagramType.ClientReloadRequest, ReadClientReloadRequest },
                { DatagramType.ServerReloadResponse, ReadServerReloadResponse },
                { DatagramType.PickUpSpawned, ReadPickUpSpawnedMessage },
                { DatagramType.PickUpPickedUp, ReadPickUpPickedUpMessage },
                { DatagramType.InventorySelectSlotRequest, ReadInventorySlotMessage },
                { DatagramType.InventoryDropSlotRequest, ReadInventorySlotMessage },
                { DatagramType.InventorySelectSlotConfirm, ReadServerInventorySlotMessage },
                { DatagramType.InventoryDropSlotConfirm, ReadServerInventoryDropItemMessage },

            };

            //TestSerializer();
        }

        public void TestSerializer()
        {
            DatagramHolder playerJoin = new DatagramHolder(DatagramType.PlayerJoin, new PlayerJoinMessage(1, "emboi", new Vector3(2.3f, 2.5f, 2.7f)));
            DatagramHolder desPlayerJoin = Deserialize(Serialize(playerJoin));
            PlayerJoinMessage pjm = (PlayerJoinMessage)desPlayerJoin.Data;
            bool equal = desPlayerJoin.DatagramType == DatagramType.PlayerJoin && pjm.clientId == 1 && pjm.playerName == "emboi" && pjm.Position.Equals(new Vector3(2.3f, 2.5f, 2.7f));
            Debug.Log("Player join serialization test passed: " + equal);

            DatagramHolder requestJoin = new DatagramHolder(DatagramType.RequestJoin, new RequestJoinMessage("emboi"));
            DatagramHolder desRequestJoin = Deserialize(Serialize(requestJoin));
            RequestJoinMessage rjm = (RequestJoinMessage)desRequestJoin.Data;
            equal = desRequestJoin.DatagramType == DatagramType.RequestJoin && rjm.name == "emboi";
            Debug.Log("Request join serialization test passed: " + equal);

            DatagramHolder worldState = new DatagramHolder(DatagramType.WorldState, new OnJoinWorldState(new PlayerInformation[] { new PlayerInformation(55, Vector3.one * 4, Quaternion.Euler(new Vector3(45, 50, 55))) }));
            DatagramHolder desWorldState = Deserialize(Serialize(worldState));
            OnJoinWorldState wsm = (OnJoinWorldState)desWorldState.Data;
            equal = desWorldState.DatagramType == DatagramType.WorldState && wsm.playerInformation.Length == 1 && wsm.playerInformation[0].networkID == 55 && wsm.playerInformation[0].Position == Vector3.one * 4 && wsm.playerInformation[0].Rotation == Quaternion.Euler(new Vector3(45, 50, 55));
            Debug.Log("World state serialization test passed: " + equal);

            DatagramHolder movePlayer = new DatagramHolder(DatagramType.MovePlayer, new MovePlayerMessage(Vector3.one * 0.72f, Quaternion.Euler(new Vector3(23, 106, 190))));
            DatagramHolder desMovePlayer = Deserialize(Serialize(movePlayer));
            MovePlayerMessage mpm = (MovePlayerMessage)desMovePlayer.Data;
            equal = desMovePlayer.DatagramType == DatagramType.MovePlayer && mpm.Position == Vector3.one * 0.72f && mpm.Rotation == Quaternion.Euler(new Vector3(23, 106, 190));
            Debug.Log("Move player serialization test passed: " + equal);

            DatagramHolder playerShoot = new DatagramHolder(DatagramType.PlayerShoot, new PlayerShootMessage(106, Quaternion.Euler(Vector3.down * 59)));
            DatagramHolder desplayerShoot = Deserialize(Serialize(playerShoot));
            PlayerShootMessage psm = (PlayerShootMessage)desplayerShoot.Data;
            equal = desplayerShoot.DatagramType == DatagramType.PlayerShoot && psm.clientId == 106 && psm.Rotation == Quaternion.Euler(Vector3.down * 59);
            Debug.Log("Player shoot serialization test passed: " + equal);

            DatagramHolder playersUpdate = new DatagramHolder(DatagramType.PlayersUpdate, new PlayersUpdateMessage(new PlayerInformation[] { new PlayerInformation(551, Vector3.down * 4, Quaternion.Euler(new Vector3(46, 70, 45))) }));
            DatagramHolder desplayersUpdate = Deserialize(Serialize(playersUpdate));
            PlayersUpdateMessage pum = (PlayersUpdateMessage)desplayersUpdate.Data;
            equal = desplayersUpdate.DatagramType == DatagramType.PlayersUpdate && pum.playerInformation.Length == 1 && pum.playerInformation[0].networkID == 551 && pum.playerInformation[0].Position == Vector3.down * 4 && pum.playerInformation[0].Rotation == Quaternion.Euler(new Vector3(46, 70, 45));
            Debug.Log("Players update serialization test passed: " + equal);
        }

        public override DatagramHolder Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(bytes);
                ms.Seek(0, SeekOrigin.Begin);

                using (BinaryReader reader = new BinaryReader(ms))
                {
                    DatagramType datagramType = (DatagramType)reader.ReadInt32();

                    object data = null;
                    if (!special.Contains(datagramType))
                        data = deserializers[datagramType](reader);
                    else if (datagramType == DatagramType.ServerHandshakeResponse)
                        data = reader.ReadClientID();

                    return new DatagramHolder(datagramType, data);
                }
            }

        }

        public override byte[] Serialize(DatagramHolder datagramHolder)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(((int)datagramHolder.DatagramType));

                    if (!special.Contains(datagramHolder.DatagramType))
                        serializers[datagramHolder.DatagramType](datagramHolder, writer);
                    else if (datagramHolder.DatagramType == DatagramType.ServerHandshakeResponse)
                        writer.WriteClientID((int)datagramHolder.Data);

                    return ms.ToArray();
                }
            }
        }

        private void WriteMovePlayer(DatagramHolder dgram, BinaryWriter writer)
        {
            MovePlayerMessage msg = (MovePlayerMessage)dgram.Data;
            writer.Write(msg.Position);
            writer.Write(msg.Rotation);
        }

        private object ReadMovePlayer(BinaryReader reader)
        {
            return new MovePlayerMessage(reader.ReadVector3(), reader.ReadQuaternion());
        }

        private void WritePlayerShoot(DatagramHolder dgram, BinaryWriter writer)
        {
            PlayerShootMessage msg = (PlayerShootMessage)dgram.Data;
            writer.WriteClientID(msg.clientId);
            writer.Write(msg.Rotation);
        }

        private object ReadPlayerShoot(BinaryReader reader)
        {
            return new PlayerShootMessage(reader.ReadInt32(), reader.ReadQuaternion());
        }

        private void WriteRequestJoin(DatagramHolder dgram, BinaryWriter writer)
        {
            RequestJoinMessage msg = (RequestJoinMessage)dgram.Data;
            writer.Write(msg.name);
        }

        private object ReadRequestJoin(BinaryReader reader)
        {
            return new RequestJoinMessage(reader.ReadString());
        }

        private void WritePlayerDeath(DatagramHolder dgram, BinaryWriter writer)
        {
            PlayerDeathMessage msg = (PlayerDeathMessage)dgram.Data;
            writer.WriteClientID(msg.clientId);
        }

        private object ReadPlayerDeath(BinaryReader reader)
        {
            return new PlayerDeathMessage(reader.ReadClientID());
        }

        private void WritePlayerHealthChange(DatagramHolder dgram, BinaryWriter writer)
        {
            PlayerHealthChangeMessage msg = (PlayerHealthChangeMessage)dgram.Data;
            writer.WriteClientID(msg.clientId);
            writer.Write(msg.currentHealth);
        }

        private object ReadPlayerHealthChange(BinaryReader reader)
        {
            return new PlayerHealthChangeMessage(reader.ReadClientID(), reader.ReadInt32());
        }

        private void WriteLinkPlayerNameToID(DatagramHolder dgram, BinaryWriter writer)
        {
            LinkPlayerNameToIDMessage msg = (LinkPlayerNameToIDMessage)dgram.Data;
            writer.WriteClientID(msg.id);
            writer.Write(msg.name);
        }

        private object ReadLinkPlayerNameToIDMessage(BinaryReader reader)
        {
            return new LinkPlayerNameToIDMessage(reader.ReadClientID(), reader.ReadString());
        }

        private void WriteOnJoinWorldState(DatagramHolder dgram, BinaryWriter writer)
        {
            OnJoinWorldState msg = (OnJoinWorldState)dgram.Data;
            writer.Write(msg.playerInformation.Length);
            foreach (PlayerInformation info in msg.playerInformation)
            {
                writer.WriteClientID(info.networkID);
                writer.Write(info.Position);
                writer.Write(info.Rotation);
            }
        }

        private object ReadOnJoinWorldState(BinaryReader reader)
        {
            int infoLength = reader.ReadInt32();
            PlayerInformation[] infos = new PlayerInformation[infoLength];
            for (int i = 0; i < infoLength; i++)
            {
                infos[i] = new PlayerInformation(reader.ReadClientID(), reader.ReadVector3(), reader.ReadQuaternion());
            }
            return new OnJoinWorldState(infos);
        }

        private void WritePlayerJoin(DatagramHolder dgram, BinaryWriter writer)
        {
            PlayerJoinMessage msg = (PlayerJoinMessage)dgram.Data;
            writer.WriteClientID(msg.clientId);
            writer.Write(msg.playerName);
            writer.Write(msg.Position);
        }

        private object ReadPlayerJoin(BinaryReader reader)
        {
            PlayerJoinMessage joinMessage = new PlayerJoinMessage(reader.ReadClientID(), reader.ReadString(), reader.ReadVector3());
            return joinMessage;
        }

        private void WritePlayersUpdate(DatagramHolder dgram, BinaryWriter writer)
        {
            // Reusing the same serialization code. Wont be noted further in this file.
            PlayersUpdateMessage msg = (PlayersUpdateMessage)dgram.Data;
            WriteOnJoinWorldState(new DatagramHolder(DatagramType.WorldState, new OnJoinWorldState(msg.playerInformation)), writer);
        }

        private object ReadPlayersUpdate(BinaryReader reader)
        {
            // Reusing the same DEserialization code. Wont be noted further in this file.
            OnJoinWorldState msg = (OnJoinWorldState)ReadOnJoinWorldState(reader);
            return new PlayersUpdateMessage(msg.playerInformation);
        }

        public void WritePlayerSpawn(DatagramHolder dgram, BinaryWriter writer)
        {
            PlayerSpawnMessage msg = (PlayerSpawnMessage)dgram.Data;
            writer.WriteClientID(msg.clientId);
            writer.Write(msg.Position);
        }

        public object ReadPlayerSpawn(BinaryReader reader)
        {
            return new PlayerSpawnMessage(reader.ReadClientID(), reader.ReadVector3());
        }

        public void WritePlayerHit(DatagramHolder dgram, BinaryWriter writer)
        {
            PlayerHitMessage msg = (PlayerHitMessage)dgram.Data;
            writer.WriteClientID(msg.clientHitId);
            writer.WriteClientID(msg.attackerId);
            writer.Write(msg.currentHealth);
            writer.Write(msg.projectileHitsRelativeToHitPlayer);
        }

        public object ReadPlayerHit(BinaryReader reader)
        {            
            return new PlayerHitMessage(reader.ReadClientID(), reader.ReadClientID(),
                reader.ReadInt32(), reader.ReadVector3Array());
        }

        public void WritePlayerKill(DatagramHolder dgram, BinaryWriter writer)
        {
            PlayerKillMessage msg = (PlayerKillMessage)dgram.Data;
            writer.WriteClientID(msg.deadClientId);
            writer.WriteClientID(msg.killerId);
        }

        public object ReadPlayerKill(BinaryReader reader)
        {
            return new PlayerKillMessage(reader.ReadClientID(), reader.ReadClientID());
        }

        public void WriteClientReloadRequest(DatagramHolder dgram, BinaryWriter writer)
        {
            // dgram is null.            
        }

        public object ReadClientReloadRequest(BinaryReader reader)
        {
            return null;
        }

        public void WriteServerReloadResponse(DatagramHolder dgram, BinaryWriter writer)
        {
            ServerReloadResponseMessage msg = (ServerReloadResponseMessage)dgram.Data;
            writer.Write(msg.reloadSuccessful);
        }

        public object ReadServerReloadResponse(BinaryReader reader)
        {
            return new ServerReloadResponseMessage(reader.ReadBoolean());
        }

        public void WritePickUpSpawnedMessage(DatagramHolder dgram, BinaryWriter writer)
        {
            PickUpSpawnedMessage msg = (PickUpSpawnedMessage)dgram.Data;
            writer.Write(msg.pickUpId);
            writer.Write((int)msg.pickUpType);
            writer.Write(msg.position);
        }

        public object ReadPickUpSpawnedMessage(BinaryReader reader)
        {
            PickUpSpawnedMessage message = new PickUpSpawnedMessage(reader.ReadInt32(),
                (PickUpType)reader.ReadInt32(), reader.ReadVector3());
            return message;
        }

        public void WritePickUpPickedUpMessage(DatagramHolder dgram, BinaryWriter writer)
        {
            PickUpPickedUpMessage msg = (PickUpPickedUpMessage)dgram.Data;

            writer.WriteClientID(msg.clientId);
            writer.Write(msg.pickUpId);

            if (msg.pickUpData == null)
                writer.Write(false);
            else
            {
                writer.Write(true);
                writer.Write((int)msg.pickUpData.pickUpType);
                if (msg.pickUpData is AmmoMagazinePickUpData ammoData)
                {
                    writer.Write(ammoData.magazinesRecovered);
                }
                else if (msg.pickUpData is HealthPickUpData healthData)
                {
                    writer.Write(healthData.healAmount);
                }
            }
        }

        public object ReadPickUpPickedUpMessage(BinaryReader reader)
        {
            var clientId = reader.ReadClientID();
            int pickUpId = reader.ReadInt32();
            bool isPickUpDataInPacket = reader.ReadBoolean();

            BasePickUpData pickUpData = null;
            if (isPickUpDataInPacket)
            {
                PickUpType pickUpType = (PickUpType)reader.ReadInt32();
                if (pickUpType == PickUpType.AmmoMagazine)
                {
                    pickUpData = new AmmoMagazinePickUpData(reader.ReadInt32());
                }
                else if (pickUpType == PickUpType.Health)
                {
                    pickUpData = new HealthPickUpData(reader.ReadInt32());
                }
            }

            return new PickUpPickedUpMessage(clientId, pickUpId, pickUpData: pickUpData);
        }

        public void WriteInventorySlotMessage(DatagramHolder dgram, BinaryWriter writer)
        {
            InventorySlotMessage msg = (InventorySlotMessage)dgram.Data;
            writer.Write(msg.slotIndex);
        }

        public object ReadInventorySlotMessage(BinaryReader reader)
        {
            return new InventorySlotMessage(reader.ReadInt32());
        }

        public void WriteServerInventorySlotMessage(DatagramHolder dgram, BinaryWriter writer)
        {
            ServerInventorySlotMessage msg = (ServerInventorySlotMessage)dgram.Data;
            WriteInventorySlotMessage(dgram, writer);
            writer.WriteClientID(msg.senderId);
        }

        public object ReadServerInventorySlotMessage(BinaryReader reader)
        {
            InventorySlotMessage baseMsg = (InventorySlotMessage)ReadInventorySlotMessage(reader);
            return new ServerInventorySlotMessage(reader.ReadClientID(), baseMsg.slotIndex);
        }

        public void WriteServerInventoryDropItemMessage(DatagramHolder dgram, BinaryWriter writer)
        {
            ServerInventoryDropItemMessage msg = (ServerInventoryDropItemMessage)dgram.Data;
            WriteServerInventorySlotMessage(dgram, writer);
            writer.Write(msg.droppedItemTransformHash);
        }

        public object ReadServerInventoryDropItemMessage(BinaryReader reader)
        {
            ServerInventorySlotMessage baseMsg = (ServerInventorySlotMessage)ReadServerInventorySlotMessage(reader);
            return new ServerInventoryDropItemMessage(baseMsg.senderId, baseMsg.slotIndex, reader.ReadInt32());
        }

        public void WriteTransformsUpdateMessage(DatagramHolder dgram, BinaryWriter writer)
        {
            TransformsUpdateMessage msg = (TransformsUpdateMessage)dgram.Data;
            writer.Write(msg.transformHashes.Length);
            for (int i = 0; i < msg.transformHashes.Length; i++)
            {
                writer.Write(msg.transformHashes[i]);
                writer.Write(msg.Positions[i]);
                writer.Write(msg.Rotations[i]);
            }
        }

        public object ReadTransformsUpdateMessage(BinaryReader reader)
        {
            List<int> hashes = new List<int>();
            List<Vector3> positions = new List<Vector3>();
            List<Quaternion> rotations = new List<Quaternion>();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                hashes.Add(reader.ReadInt32());
                positions.Add(reader.ReadVector3());
                rotations.Add(reader.ReadQuaternion());
            }

            return new TransformsUpdateMessage(hashes, positions, rotations);
        }
    }
}

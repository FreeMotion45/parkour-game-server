using Assets.Scripts.Messages;
using Assets.Scripts.Messages.ClientOrigin;
using Assets.Scripts.Messages.ServerOrigin;
using Assets.Scripts.Network.Messages.ServerOrigin;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            };

            deserializers = new Dictionary<DatagramType, Func<BinaryReader, object>>()
            {
                { DatagramType.RequestJoin, ReadRequestJoin },
                { DatagramType.PlayerJoin, ReadPlayerJoin },
                { DatagramType.WorldState, ReadOnJoinWorldState },
                { DatagramType.MovePlayer, ReadMovePlayer },
                { DatagramType.PlayerShoot, ReadPlayerShoot },
                { DatagramType.PlayersUpdate, ReadPlayersUpdate },
                { DatagramType.LinkNameToID, ReadLinkPlayerNameToIDMessage },
                { DatagramType.PlayerHealthChange, ReadPlayerHealthChange },
                { DatagramType.PlayerDeath, ReadPlayerDeath },
                { DatagramType.PlayerSpawn, ReadPlayerSpawn }
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
                        data = ReadClientID(reader);

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
                        WriteClientID((int)datagramHolder.Data, writer);

                    return ms.ToArray();
                }
            }
        }

        private void WriteMovePlayer(DatagramHolder dgram, BinaryWriter writer)
        {
            MovePlayerMessage msg = (MovePlayerMessage)dgram.Data;
            WriteVector3(msg.Position, writer);
            WriteQuaternion(msg.Rotation, writer);
        }

        private object ReadMovePlayer(BinaryReader reader)
        {
            return new MovePlayerMessage(ReadVector3(reader), ReadQuaternion(reader));
        }

        private void WritePlayerShoot(DatagramHolder dgram, BinaryWriter writer)
        {
            PlayerShootMessage msg = (PlayerShootMessage)dgram.Data;
            WriteClientID(msg.clientId, writer);
            WriteQuaternion(msg.Rotation, writer);
        }

        private object ReadPlayerShoot(BinaryReader reader)
        {
            return new PlayerShootMessage(reader.ReadInt32(), ReadQuaternion(reader));
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
            WriteClientID(msg.clientId, writer);
        }

        private object ReadPlayerDeath(BinaryReader reader)
        {
            return new PlayerDeathMessage(ReadClientID(reader));
        }

        private void WritePlayerHealthChange(DatagramHolder dgram, BinaryWriter writer)
        {
            PlayerHealthChangeMessage msg = (PlayerHealthChangeMessage)dgram.Data;
            WriteClientID(msg.clientId, writer);
            writer.Write(msg.currentHealth);
        }

        private object ReadPlayerHealthChange(BinaryReader reader)
        {
            return new PlayerHealthChangeMessage(ReadClientID(reader), reader.ReadInt32());
        }

        private void WriteLinkPlayerNameToID(DatagramHolder dgram, BinaryWriter writer)
        {
            LinkPlayerNameToIDMessage msg = (LinkPlayerNameToIDMessage)dgram.Data;
            WriteClientID(msg.id, writer);
            writer.Write(msg.name);
        }

        private object ReadLinkPlayerNameToIDMessage(BinaryReader reader)
        {
            return new LinkPlayerNameToIDMessage(ReadClientID(reader), reader.ReadString());
        }

        private void WriteOnJoinWorldState(DatagramHolder dgram, BinaryWriter writer)
        {
            OnJoinWorldState msg = (OnJoinWorldState)dgram.Data;
            writer.Write(msg.playerInformation.Length);
            foreach (PlayerInformation info in msg.playerInformation)
            {
                WriteClientID(info.networkID, writer);
                WriteVector3(info.Position, writer);
                WriteQuaternion(info.Rotation, writer);
            }
        }

        private object ReadOnJoinWorldState(BinaryReader reader)
        {
            int infoLength = reader.ReadInt32();
            PlayerInformation[] infos = new PlayerInformation[infoLength];
            for (int i = 0; i < infoLength; i++)
            {
                infos[i] = new PlayerInformation(ReadClientID(reader), ReadVector3(reader), ReadQuaternion(reader));
            }
            return new OnJoinWorldState(infos);
        }

        private void WritePlayerJoin(DatagramHolder dgram, BinaryWriter writer)
        {
            PlayerJoinMessage msg = (PlayerJoinMessage)dgram.Data;
            WriteClientID(msg.clientId, writer);
            writer.Write(msg.playerName);
            WriteVector3(msg.Position, writer);
        }

        private object ReadPlayerJoin(BinaryReader reader)
        {
            PlayerJoinMessage joinMessage = new PlayerJoinMessage(ReadClientID(reader), reader.ReadString(), ReadVector3(reader));
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
            WriteClientID(msg.clientId, writer);
            WriteVector3(msg.Position, writer);
        }

        public object ReadPlayerSpawn(BinaryReader reader)
        {
            return new PlayerSpawnMessage(ReadClientID(reader), ReadVector3(reader));
        }

        private void WriteVector3(Vector3 vec, BinaryWriter writer)
        {
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
        }

        private Vector3 ReadVector3(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

        private void WriteQuaternion(Quaternion q, BinaryWriter writer)
        {
            writer.Write(q.x);
            writer.Write(q.y);
            writer.Write(q.z);
            writer.Write(q.w);
        }

        private Quaternion ReadQuaternion(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float w = reader.ReadSingle();
            return new Quaternion(x, y, z, w);
        }

        private void WriteClientID(int clientId, BinaryWriter writer)
        {
            writer.Write(clientId);
        }

        private int ReadClientID(BinaryReader reader)
        {
            return reader.ReadInt32();
        }
    }
}

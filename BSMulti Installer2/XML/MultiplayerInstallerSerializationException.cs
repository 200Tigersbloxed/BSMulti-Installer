using System;
using System.Runtime.Serialization;

namespace BSMulti_Installer2.XML
{
#pragma warning disable CA2237 // Mark ISerializable types with serializable
    public class MultiplayerInstallerSerializationException : SerializationException
#pragma warning restore CA2237 // Mark ISerializable types with serializable
    {
        public MultiplayerInstallerSerializationException(string message) : base(message)
        {
        }

        public MultiplayerInstallerSerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MultiplayerInstallerSerializationException()
        {
        }
    }
}

using System.Runtime.Serialization;

namespace Library.DataStructure.Model
{
    [DataContract]
    internal class Branche : ModelNode
    {
        [DataMember]
        internal string GrabMethode;
    }
}

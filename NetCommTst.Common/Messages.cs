using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.IO;

using ProtoBuf;

namespace NetCommTst.Common
{
    [Serializable]
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(80, typeof(MSgComplexA))]
    [ProtoBuf.ProtoInclude(81, typeof(MsgSimpleText))]
    [ProtoBuf.ProtoInclude(82, typeof(MsgSimpleInt))]
    [ProtoBuf.ProtoInclude(83, typeof(MsgComplexB))]
    [ProtoBuf.ProtoInclude(84, typeof(MsgReallyComplexA))]
    public class MessageBase
    {
    }


    [Serializable]
    [ProtoBuf.ProtoContract]
    public class MsgSimpleText : MessageBase
    {
        [ProtoBuf.ProtoMember(1)]
        public string Text { get; set; }
    }

    [Serializable]
    [ProtoBuf.ProtoContract]
    public class MsgSimpleInt : MessageBase
    {
        [ProtoBuf.ProtoMember(1)]
        public int Number { get; set; }
    }

    [Serializable]
    [ProtoBuf.ProtoContract]
    public class MSgComplexA : MessageBase
    {
        [ProtoBuf.ProtoMember(1)]
        public string Text { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public int Number { get; set; }
    }

    [Serializable]
    [ProtoBuf.ProtoContract]
    public class MsgComplexB : MessageBase
    {
        [ProtoBuf.ProtoMember(1)]
        public double Balance { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public bool Alert { get; set; }
    }

    [Serializable]
    [ProtoBuf.ProtoContract]
    public class MsgReallyComplexA : MessageBase
    {
        [ProtoBuf.ProtoMember(1)]
        public int Year { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public MSgComplexA mca { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public MsgSimpleInt msi { get; set; }
    }
}



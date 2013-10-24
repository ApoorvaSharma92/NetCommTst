using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using ProtoBuf;
using NetCommTst.Common;

namespace NetCommTst.ServerRPC
{
    [ProtoContract]
    class MagicEightBall : IMagicEightBall
    {
        [ProtoMember(1)]
        private bool answerQueue = false;



        public bool Shake()
        {
            return true;
        }

        public string AskQuestion(string q)
        {
            return "Do You Think?";
        }

        public string Reset()
        {
            if (answerQueue == false)
                answerQueue = true;
            else
                answerQueue = false;

            return "I should have better answers for you now.";
        }

        public string Complain(string complaint)
        {
            return "Complaining will not make me give better answers";
        }

        public MsgSimpleText AskComplexNumberQuestion(MsgSimpleInt i)
        {
            int val;
            val = i.Number * 100 + 25 + (i.Number * i.Number);
            MsgSimpleText mst = new MsgSimpleText();
            mst.Text = "Complex questions yield complex answers.  Your answer is: " + val.ToString();
            return mst;
        }

        public void Throw()
        {
            throw new NotImplementedException();
        }
    }
}

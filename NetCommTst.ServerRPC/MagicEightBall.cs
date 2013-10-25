using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
           Thread.Sleep(2300);
            return true;
        }

        public string AskQuestion(string q)
        {
            List<string> answers = new List<string>();
            answers.Add("Do you think?");
            answers.Add("I don't think that will happen");
            answers.Add("Probably so.");
            answers.Add("Most likely that will occur.");
            answers.Add("Im not sure I understand what you are asking");
            answers.Add("I don't know.  Can I ask you a question?");
            answers.Add("Yes it will.");
            Random r = new Random();
            int i = r.Next(0, answers.Count()-1);

            return answers[i];
            //return "Do You Think?";
        }

        public string Reset()
        {
            if (answerQueue == false)
                answerQueue = true;
            else
                answerQueue = false;

            // simulate processing delay
            Thread.Sleep(2000);
            return "I should have better answers for you now.";
        }

        public string Complain(string complaint)
        {
            Thread.Sleep (3000);
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
            Thread.Sleep(4000);
            //throw new NotImplementedException();
        }
    }
}

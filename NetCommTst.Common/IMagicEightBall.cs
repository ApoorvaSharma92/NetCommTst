using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetCommTst.Common;
using ProtoBuf;

namespace NetCommTst.Common
{
    [ProtoContract]
    public interface IMagicEightBall
    {

        /// <summary>
        /// Shakes the magic 8ball, hopefully getting different answers
        /// </summary>
        /// <returns></returns>
        bool Shake();

        /// <summary>
        /// Ask the Magic 8 Ball a question
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        string AskQuestion(string q);

        /// <summary>
        /// Not getting the results you want?  Give it a reset.
        /// </summary>
        /// <returns></returns>
        string Reset();

        /// <summary>
        /// If you do not like the answer.  complain to the 8 ball.
        /// </summary>
        /// <param name="complaint"></param>
        string Complain(string complaint);

        /// <summary>
        /// Ask it something more difficult.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        MsgSimpleText AskComplexNumberQuestion(MsgSimpleInt i);

        /// <summary>
        /// Mad at the Magic 8 Ball.  Throw it!
        /// </summary>
        void Throw();

    }
}

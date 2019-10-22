using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationProtocol.Code
{
    public class MatchCode
    {
        public const int MATCH_ENTER_CREQ = 0;

        public const int MATCH_ENTER_SREP = 1;

        public const int MATCH_EXIT_CREQ = 2;

        public const int MATCH_EXIT_SREP = 3;

        public const int MATCH_EXIT_BROA = 4;

        public const int MATCH_START_BROA = 5;

        public const int MATCH_READY_CREQ = 6;

        public const int MATCH_READY_BROA = 7;
    }
}

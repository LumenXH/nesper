///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;

using com.espertech.esper.client;

namespace com.espertech.esper.example.marketdatafeed
{
    public class TicksFalloffStatement
    {
        public static EPStatement Create(EPAdministrator admin)
        {
            String stmt = "select feed, avg(cnt) as avgCnt, cnt as feedCnt from TicksPerSecond.win:time(10 sec) " +
                          "group by feed, cnt " +
                          "having cnt < avg(cnt) * 0.75 ";

            return admin.CreateEPL(stmt);
        }
    }
}

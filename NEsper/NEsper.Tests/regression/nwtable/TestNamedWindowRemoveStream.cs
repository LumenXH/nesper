///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;
using com.espertech.esper.client.scopetest;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.metrics.instrumentation;
using com.espertech.esper.support.bean;
using com.espertech.esper.support.client;

using NUnit.Framework;

namespace com.espertech.esper.regression.nwtable
{
    [TestFixture]
    public class TestNamedWindowRemoveStream 
    {
        private EPServiceProvider epService;
    
        [SetUp]
        public void SetUp()
        {
            Configuration config = SupportConfigFactory.GetConfiguration();
            epService = EPServiceProviderManager.GetDefaultProvider(config);
            epService.Initialize();
            if (InstrumentationHelper.ENABLED) { InstrumentationHelper.StartTest(epService, this.GetType(), GetType().FullName);}
        }
    
        [TearDown]
        public void TearDown() {
            if (InstrumentationHelper.ENABLED) { InstrumentationHelper.EndTest();}
        }
    
        [Test]
        public void TestRemoveStream()
        {
            string[] fields = new string[] {"TheString"};
            epService.EPAdministrator.Configuration.AddEventType<SupportBean>();
            EPStatement stmt1 = epService.EPAdministrator.CreateEPL("create window W1.win:length(2) as select * from SupportBean");
            EPStatement stmt2 = epService.EPAdministrator.CreateEPL("create window W2.win:length(2) as select * from SupportBean");
            EPStatement stmt3 = epService.EPAdministrator.CreateEPL("create window W3.win:length(2) as select * from SupportBean");
            epService.EPAdministrator.CreateEPL("insert into W1 select * from SupportBean");
            epService.EPAdministrator.CreateEPL("insert rstream into W2 select rstream * from W1");
            epService.EPAdministrator.CreateEPL("insert rstream into W3 select rstream * from W2");
    
            epService.EPRuntime.SendEvent(new SupportBean("E1", 1));
            epService.EPRuntime.SendEvent(new SupportBean("E2", 1));
            EPAssertionUtil.AssertPropsPerRowAnyOrder(stmt1.GetEnumerator(), fields, new object[][] { new object[] { "E1" }, new object[] { "E2" } });
            
            epService.EPRuntime.SendEvent(new SupportBean("E3", 1));
            EPAssertionUtil.AssertPropsPerRowAnyOrder(stmt1.GetEnumerator(), fields, new object[][] { new object[] { "E2" }, new object[] { "E3" } });
            EPAssertionUtil.AssertPropsPerRowAnyOrder(stmt2.GetEnumerator(), fields, new object[][] { new object[] { "E1" } });
    
            epService.EPRuntime.SendEvent(new SupportBean("E4", 1));
            epService.EPRuntime.SendEvent(new SupportBean("E5", 1));
            EPAssertionUtil.AssertPropsPerRowAnyOrder(stmt1.GetEnumerator(), fields, new object[][] { new object[] { "E4" }, new object[] { "E5" } });
            EPAssertionUtil.AssertPropsPerRowAnyOrder(stmt2.GetEnumerator(), fields, new object[][] { new object[] { "E2" }, new object[] { "E3" } });
            EPAssertionUtil.AssertPropsPerRowAnyOrder(stmt3.GetEnumerator(), fields, new object[][] { new object[] { "E1" } });
        }
    }
}

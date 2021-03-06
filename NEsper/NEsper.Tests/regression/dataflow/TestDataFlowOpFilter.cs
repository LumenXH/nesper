///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;

using com.espertech.esper.client;
using com.espertech.esper.client.dataflow;
using com.espertech.esper.compat.collections;
using com.espertech.esper.dataflow.util;
using com.espertech.esper.metrics.instrumentation;
using com.espertech.esper.support.bean;
using com.espertech.esper.support.client;

using NUnit.Framework;

namespace com.espertech.esper.regression.dataflow
{
    [TestFixture]
    public class TestDataFlowOpFilter
    {
        private EPServiceProvider _epService;
    
        [SetUp]
        public void SetUp() {
            _epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.GetConfiguration());
            _epService.Initialize();
            if (InstrumentationHelper.ENABLED) { InstrumentationHelper.StartTest(_epService, GetType(), GetType().FullName); }
    
            _epService.EPAdministrator.Configuration.AddNamespaceImport<DefaultSupportSourceOp>();
            _epService.EPAdministrator.Configuration.AddEventType<SupportBean>();
        }

        [TearDown]
        public void TearDown()
        {
            if (InstrumentationHelper.ENABLED) { InstrumentationHelper.EndTest(); }
        }
    
        [Test]
        public void TestInvalid()
        {
    
            // invalid: no filter
            SupportDataFlowAssertionUtil.TryInvalidInstantiate(_epService, "DF1", "create dataflow DF1 BeaconSource -> instream<SupportBean> {} Filter(instream) -> abc {}",
                    "Failed to instantiate data flow 'DF1': Failed validation for operator 'Filter': Required parameter 'filter' providing the filter expression is not provided");
    
            // invalid: too many output streams
            SupportDataFlowAssertionUtil.TryInvalidInstantiate(_epService, "DF1", "create dataflow DF1 BeaconSource -> instream<SupportBean> {} Filter(instream) -> abc,def,efg { filter : true }",
                    "Failed to instantiate data flow 'DF1': Failed initialization for operator 'Filter': Filter operator requires one or two output Stream(s) but produces 3 streams");
    
            // invalid: too few output streams
            SupportDataFlowAssertionUtil.TryInvalidInstantiate(_epService, "DF1", "create dataflow DF1 BeaconSource -> instream<SupportBean> {} Filter(instream) { filter : true }",
                    "Failed to instantiate data flow 'DF1': Failed initialization for operator 'Filter': Filter operator requires one or two output Stream(s) but produces 0 streams");
    
            // invalid filter expressions
            TryInvalidInstantiate("TheString = 1",
                    "Failed to instantiate data flow 'MySelect': Failed validation for operator 'Filter': Failed to validate filter dataflow operator expression 'TheString=1': Implicit conversion from datatype '" + typeof(int?).FullName + "' to 'System.String' is not allowed");
    
            TryInvalidInstantiate("prev(TheString, 1) = 'abc'",
                    "Failed to instantiate data flow 'MySelect': Failed validation for operator 'Filter': Invalid filter dataflow operator expression 'prev(TheString,1)=\"abc\"': Aggregation, sub-select, previous or prior functions are not supported in this context");
        }
    
        [Test]
        public void TestAllTypes()
        {
            DefaultSupportGraphEventUtil.AddTypeConfiguration(_epService);
    
            RunAssertionAllTypes("MyXMLEvent", DefaultSupportGraphEventUtil.XMLEvents);
            RunAssertionAllTypes("MyOAEvent", DefaultSupportGraphEventUtil.OAEvents);
            RunAssertionAllTypes("MyMapEvent", DefaultSupportGraphEventUtil.MapEvents);
            RunAssertionAllTypes("MyEvent", DefaultSupportGraphEventUtil.PONOEvents);
    
            // test doc sample
            String epl = "create dataflow MyDataFlow\n" +
                    "  create schema SampleSchema(tagId string, locX double),\t// sample type\n" +
                    "  BeaconSource -> samplestream<SampleSchema> {}\n" +
                    "  \n" +
                    "  // Filter all events that have a tag id of '001'\n" +
                    "  Filter(samplestream) -> tags_001 {\n" +
                    "    filter : tagId = '001' \n" +
                    "  }\n" +
                    "  \n" +
                    "  // Filter all events that have a tag id of '001', putting all other tags into the second stream\n" +
                    "  Filter(samplestream) -> tags_001, tags_other {\n" +
                    "    filter : tagId = '001' \n" +
                    "  }";
            _epService.EPAdministrator.CreateEPL(epl);
            _epService.EPRuntime.DataFlowRuntime.Instantiate("MyDataFlow");
    
            // test two streams
            DefaultSupportCaptureOpStatic.Instances.Clear();
            String graph = "create dataflow MyFilter\n" +
                    "Emitter -> sb<SupportBean> {name : 'e1'}\n" +
                    "Filter(sb) -> out.ok, out.fail {filter: TheString = 'x'}\n" +
                    "DefaultSupportCaptureOpStatic(out.ok) {}" +
                    "DefaultSupportCaptureOpStatic(out.fail) {}";
            _epService.EPAdministrator.CreateEPL(graph);
    
            EPDataFlowInstance instance = _epService.EPRuntime.DataFlowRuntime.Instantiate("MyFilter");
            EPDataFlowInstanceCaptive captive = instance.StartCaptive();
    
            captive.Emitters.Get("e1").Submit(new SupportBean("x", 10));
            captive.Emitters.Get("e1").Submit(new SupportBean("y", 11));
            Assert.AreEqual(10, ((SupportBean) DefaultSupportCaptureOpStatic.Instances[0].GetCurrent()[0]).IntPrimitive);
            Assert.AreEqual(11, ((SupportBean) DefaultSupportCaptureOpStatic.Instances[1].GetCurrent()[0]).IntPrimitive);
            DefaultSupportCaptureOpStatic.Instances.Clear();
        }
    
        private void TryInvalidInstantiate(String filter, String message) {
            String graph = "create dataflow MySelect\n" +
                    "DefaultSupportSourceOp -> instream<SupportBean>{}\n" +
                    "Filter(instream as ME) -> outstream {filter: " + filter + "}\n" +
                    "DefaultSupportCaptureOp(outstream) {}";
            EPStatement stmtGraph = _epService.EPAdministrator.CreateEPL(graph);
    
            try {
                _epService.EPRuntime.DataFlowRuntime.Instantiate("MySelect");
                Assert.Fail();
            }
            catch (EPDataFlowInstantiationException ex) {
                Assert.AreEqual(message, ex.Message);
            }
    
            stmtGraph.Dispose();
        }
    
        private void RunAssertionAllTypes(String typeName, Object[] events)
        {
            String graph = "create dataflow MySelect\n" +
                    "DefaultSupportSourceOp -> instream.with.dot<" + typeName + ">{}\n" +
                    "Filter(instream.with.dot) -> outstream.dot {filter: myString = 'two'}\n" +
                    "DefaultSupportCaptureOp(outstream.dot) {}";
            EPStatement stmtGraph = _epService.EPAdministrator.CreateEPL(graph);
    
            DefaultSupportSourceOp source = new DefaultSupportSourceOp(events);
            DefaultSupportCaptureOp capture = new DefaultSupportCaptureOp(2);
            EPDataFlowInstantiationOptions options = new EPDataFlowInstantiationOptions();
            options.SetDataFlowInstanceUserObject("myuserobject");
            options.SetDataFlowInstanceId("myinstanceid");

            options.OperatorProvider(new DefaultSupportGraphOpProvider(source, capture));
            EPDataFlowInstance instance = _epService.EPRuntime.DataFlowRuntime.Instantiate("MySelect", options);
            Assert.AreEqual("myuserobject", instance.UserObject);
            Assert.AreEqual("myinstanceid", instance.InstanceId);
    
            instance.Run();
    
            Object[] result = capture.GetAndReset()[0].ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreSame(events[1], result[0]);
    
            instance.Cancel();
    
            stmtGraph.Dispose();
        }
    }
}

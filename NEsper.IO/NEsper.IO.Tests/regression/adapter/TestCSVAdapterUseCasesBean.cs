///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.client;
using com.espertech.esperio.csv;
using com.espertech.esperio.support.util;

using NUnit.Framework;

namespace com.espertech.esperio.regression.adapter
{
    /// <summary>
    /// Cause all parent class unit tests to be run but sending beans instead of Maps
    /// </summary>
    /// <author>Jerry Shea</author>
    [TestFixture]
    public class TestCSVAdapterUseCasesBean
    {
        private readonly TestCSVAdapterUseCases _baseUseCase;

    	public TestCSVAdapterUseCasesBean()
    	{
    	    _baseUseCase = new TestCSVAdapterUseCases(true);
    	}
    
        [Test]
        public void TestReadWritePropsBean()
        {
            Configuration configuration = new Configuration();
            configuration.AddEventType("ReadWrite", typeof(ExampleMarketDataBeanReadWrite));

            _baseUseCase.EPService = EPServiceProviderManager.GetProvider("testExistingTypeNoOptions", configuration);
            _baseUseCase.EPService.Initialize();

            EPStatement stmt = _baseUseCase.EPService.EPAdministrator.CreateEPL("select * from ReadWrite.win:length(100)");
            SupportUpdateListener listener = new SupportUpdateListener();
            stmt.Events += listener.Update;

            var inputAdapter = new CSVInputAdapter(_baseUseCase.EPService, new AdapterInputSource(
                TestCSVAdapterUseCases.CSV_FILENAME_ONELINE_TRADE), "ReadWrite");
            inputAdapter.Start();
    
            Assert.AreEqual(1, listener.GetNewDataList().Count);
            EventBean eb = listener.GetNewDataList()[0][0];
            Assert.IsTrue(typeof(ExampleMarketDataBeanReadWrite) == eb.Underlying.GetType());
            Assert.AreEqual(55.5 * 1000, eb.Get("value"));
        }
    }
}

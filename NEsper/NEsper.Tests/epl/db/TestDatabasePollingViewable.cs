///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.core.support;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.expression.core;
using com.espertech.esper.epl.join.pollindex;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.support.bean;
using com.espertech.esper.support.epl;

using NUnit.Framework;

using SupportEventAdapterService = com.espertech.esper.support.events.SupportEventAdapterService;

namespace com.espertech.esper.epl.db
{
    [TestFixture]
    public class TestDatabasePollingViewable 
    {
        private DatabasePollingViewable _pollingViewable;
        private PollResultIndexingStrategy _indexingStrategy;
    
        [SetUp]
        public void SetUp()
        {
            var inputProperties = new[] {"s0.IntPrimitive"};
    
            var dataCache = new DataCacheLRUImpl(100);
    
            var resultProperties = new Dictionary<String, Object>();
            resultProperties["myvarchar"] = typeof(string);
            var resultEventType = SupportEventAdapterService.Service.CreateAnonymousMapType("test", resultProperties, true);
    
            var pollResults = new Dictionary<MultiKey<Object>, IList<EventBean>>();
            pollResults.Put(new MultiKey<Object>(new Object[] {-1}), new List<EventBean>());
            pollResults.Put(new MultiKey<Object>(new Object[] {500}), new List<EventBean>());
            var supportPollingStrategy = new SupportPollingStrategy(pollResults);
    
            _pollingViewable = new DatabasePollingViewable(1, inputProperties, supportPollingStrategy, dataCache, resultEventType);
    
            var sqlParameters = new Dictionary<int, IList<ExprNode>>();
            sqlParameters.Put(1, ((ExprNode) new ExprIdentNodeImpl("IntPrimitive", "s0")).AsSingleton());
            _pollingViewable.Validate(null, new SupportStreamTypeSvc3Stream(), null, null, null, null, null, null, null, null, null, sqlParameters, null, SupportStatementContextFactory.MakeContext());
    
            _indexingStrategy = new ProxyPollResultIndexingStrategy
            {
                ProcIndex = (pollResult, isActiveCache, statementContext) => new EventTable[] { new UnindexedEventTableList(pollResult, -1) },
                ProcToQueryPlan = () => GetType().Name + " unindexed"
            };        
        }
    
        [Test]
        public void TestPoll()
        {
            var input = new EventBean[2][];
            input[0] = new[] {MakeEvent(-1), null};
            input[1] = new[] {MakeEvent(500), null};
            EventTable[][] resultRows = _pollingViewable.Poll(input, _indexingStrategy, null);
    
            // should have joined to two rows
            Assert.AreEqual(2, resultRows.Length);
            Assert.IsTrue(resultRows[0][0].IsEmpty());
            Assert.IsTrue(resultRows[1][0].IsEmpty());
        }
    
        private static EventBean MakeEvent(int intPrimitive)
        {
            var bean = new SupportBean {IntPrimitive = intPrimitive};
            return SupportEventAdapterService.Service.AdapterForObject(bean);
        }
    }
}

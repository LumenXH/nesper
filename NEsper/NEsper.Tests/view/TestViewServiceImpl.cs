///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.core.context.util;
using com.espertech.esper.core.support;
using com.espertech.esper.support.bean;
using com.espertech.esper.support.view;

using NUnit.Framework;

namespace com.espertech.esper.view
{
    [TestFixture]
    public class TestViewServiceImpl 
    {
        private ViewServiceImpl _viewService;
    
        private Viewable _viewOne;
        private Viewable _viewTwo;
        private Viewable _viewThree;
        private Viewable _viewFour;
        private Viewable _viewFive;
    
        private EventStream _streamOne;
        private EventStream _streamTwo;
    
        [SetUp]
        public void SetUp()
        {
            _streamOne = new SupportStreamImpl(typeof(SupportBean), 1);
            _streamTwo = new SupportStreamImpl(typeof(SupportBean_A), 1);
    
            _viewService = new ViewServiceImpl();
    
            AgentInstanceViewFactoryChainContext context = SupportStatementContextFactory.MakeAgentInstanceViewFactoryContext();
    
            _viewOne = _viewService.CreateViews(_streamOne, SupportViewSpecFactory.MakeFactoryListOne(_streamOne.EventType), context, false).FinalViewable;
            _viewTwo = _viewService.CreateViews(_streamOne, SupportViewSpecFactory.MakeFactoryListTwo(_streamOne.EventType), context, false).FinalViewable;
            _viewThree = _viewService.CreateViews(_streamOne, SupportViewSpecFactory.MakeFactoryListThree(_streamOne.EventType), context, false).FinalViewable;
            _viewFour = _viewService.CreateViews(_streamOne, SupportViewSpecFactory.MakeFactoryListFour(_streamOne.EventType), context, false).FinalViewable;
            _viewFive = _viewService.CreateViews(_streamTwo, SupportViewSpecFactory.MakeFactoryListFive(_streamTwo.EventType), context, false).FinalViewable;
        }
    
        [Test]
        public void TestCheckChainReuse()
        {
            // Child views of first and second level must be the same
            Assert.AreEqual(2, _streamOne.Views.Length);
            View child1_1 = _streamOne.Views[0];
            View child2_1 = _streamOne.Views[0];
            Assert.IsTrue(child1_1 == child2_1);
    
            Assert.AreEqual(2, child1_1.Views.Length);
            View child1_1_1 = child1_1.Views[0];
            View child2_1_1 = child2_1.Views[0];
            Assert.IsTrue(child1_1_1 == child2_1_1);
    
            Assert.AreEqual(2, child1_1_1.Views.Length);
            Assert.AreEqual(2, child2_1_1.Views.Length);
            Assert.IsTrue(child2_1_1.Views[0] != child2_1_1.Views[1]);
    
            // Create one more view chain
            View child3_1 = _streamOne.Views[0];
            Assert.IsTrue(child3_1 == child1_1);
            Assert.AreEqual(2, child3_1.Views.Length);
            View child3_1_1 = child3_1.Views[1];
            Assert.IsTrue(child3_1_1 != child2_1_1);
        }
    
        [Test]
        public void TestRemove()
        {
            Assert.AreEqual(2, _streamOne.Views.Length);
            Assert.AreEqual(1, _streamTwo.Views.Length);
    
            _viewService.Remove(_streamOne, _viewOne);
            _viewService.Remove(_streamOne, _viewTwo);
            _viewService.Remove(_streamOne, _viewThree);
            _viewService.Remove(_streamOne, _viewFour);
    
            _viewService.Remove(_streamTwo, _viewFive);
    
            Assert.AreEqual(0, _streamOne.Views.Length);
            Assert.AreEqual(0, _streamTwo.Views.Length);
        }
    
        [Test]
        public void TestRemoveInvalid()
        {
            try
            {
                _viewService.Remove(_streamOne, _viewOne);
                _viewService.Remove(_streamOne, _viewOne);
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                // Expected
            }
        }
    }
}

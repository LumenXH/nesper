///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using com.espertech.esper.client;
using com.espertech.esper.epl.spec;
using com.espertech.esper.pattern.guard;
using com.espertech.esper.pattern.observer;
using com.espertech.esper.support.pattern;
using com.espertech.esper.view;

using NUnit.Framework;

namespace com.espertech.esper.pattern
{
    [TestFixture]
    public class TestPatternObjectResolutionServiceImpl 
    {
        private PatternObjectResolutionServiceImpl service;
    
        [SetUp]
        public void SetUp()
        {
            IList<ConfigurationPlugInPatternObject> init = new List<ConfigurationPlugInPatternObject>();
            init.Add(MakeGuardSpec("g", "h", typeof(SupportGuardFactory).FullName));
            init.Add(MakeObserverSpec("a", "b", typeof(SupportObserverFactory).FullName));
            PluggableObjectCollection desc = new PluggableObjectCollection();
            desc.AddPatternObjects(init);
            desc.AddObjects(PatternObjectHelper.BuiltinPatternObjects);
            service = new PatternObjectResolutionServiceImpl(desc);
        }
    
        [Test]
        public void TestMake()
        {
            Assert.IsTrue(service.Create(new PatternGuardSpec("g", "h", TestViewSupport.ToExprListBean(new Object[] {100}))) is SupportGuardFactory);
            Assert.IsTrue(service.Create(new PatternObserverSpec("a", "b", TestViewSupport.ToExprListBean(new Object[] {100}))) is SupportObserverFactory);
            Assert.IsTrue(service.Create(new PatternGuardSpec("timer", "within", TestViewSupport.ToExprListBean(new Object[] {100}))) is TimerWithinGuardFactory);
            Assert.IsTrue(service.Create(new PatternObserverSpec("timer", "interval", TestViewSupport.ToExprListBean(new Object[] {100}))) is TimerIntervalObserverFactory);
        }
    
        [Test]
        public void TestInvalidConfig()
        {
            IList<ConfigurationPlugInPatternObject> init = new List<ConfigurationPlugInPatternObject>();
            init.Add(MakeGuardSpec("x", "y", "a"));
            TryInvalid(init);
    
            init.Clear();
            init.Add(MakeGuardSpec("a", "b", null));
            TryInvalid(init);
        }
    
        private void TryInvalid(IEnumerable<ConfigurationPlugInPatternObject> config)
        {
            try
            {
                PluggableObjectCollection desc = new PluggableObjectCollection();
                desc.AddPatternObjects(config.ToList());
                service = new PatternObjectResolutionServiceImpl(desc);
                Assert.Fail();
            }
            catch (ConfigurationException ex)
            {
                // expected
            }
        }
    
    
        private static ConfigurationPlugInPatternObject MakeGuardSpec(String @namespace, String name, String factory)
        {
            ConfigurationPlugInPatternObject guardSpec = new ConfigurationPlugInPatternObject();
            guardSpec.Namespace = @namespace;
            guardSpec.Name = name;
            guardSpec.PatternObjectType = ConfigurationPlugInPatternObject.PatternObjectTypeEnum.GUARD;
            guardSpec.FactoryClassName = factory;
            return guardSpec; 
        }
    
        private static ConfigurationPlugInPatternObject MakeObserverSpec(String @namespace, String name, String factory)
        {
            ConfigurationPlugInPatternObject obsSpec = new ConfigurationPlugInPatternObject();
            obsSpec.Namespace = @namespace;
            obsSpec.Name = name;
            obsSpec.PatternObjectType = ConfigurationPlugInPatternObject.PatternObjectTypeEnum.OBSERVER;
            obsSpec.FactoryClassName = factory;
            return obsSpec;
        }
    }
}

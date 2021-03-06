///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.core.service;
using com.espertech.esper.epl.spec;
using com.espertech.esper.filter;

namespace com.espertech.esper.core.start
{
    /// <summary>
    /// Starts and provides the stop method for EPL statements.
    /// </summary>
    public class EPPreparedExecuteIUDSingleStreamDelete : EPPreparedExecuteIUDSingleStream
    {
        public EPPreparedExecuteIUDSingleStreamDelete(StatementSpecCompiled statementSpec, EPServicesContext services, StatementContext statementContext)
            : base(statementSpec, services, statementContext)
        {
            
        }
    
        public override EPPreparedExecuteIUDSingleStreamExec GetExecutor(FilterSpecCompiled filter, string aliasName)
        {
            return new EPPreparedExecuteIUDSingleStreamExecDelete(filter, StatementSpec.FilterRootNode, StatementSpec.Annotations, StatementSpec.TableNodes, Services);
        }
    }
}

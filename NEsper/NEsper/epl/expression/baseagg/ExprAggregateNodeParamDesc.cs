///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.epl.expression.core;

namespace com.espertech.esper.epl.expression.baseagg
{
	public class ExprAggregateNodeParamDesc
    {
	    public ExprAggregateNodeParamDesc(ExprNode[] positionalParams, ExprAggregateLocalGroupByDesc optLocalGroupBy)
        {
	        PositionalParams = positionalParams;
	        OptLocalGroupBy = optLocalGroupBy;
	    }

	    public ExprNode[] PositionalParams { get; private set; }

	    public ExprAggregateLocalGroupByDesc OptLocalGroupBy { get; private set; }
    }
} // end of namespace

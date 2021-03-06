///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.client;
using com.espertech.esper.epl.expression.core;
using com.espertech.esper.epl.expression;

namespace com.espertech.esper.view.internals
{
    /// <summary>
    /// Handler for incoming events for split-stream syntax, encapsulates where-clause
    /// evaluation strategies.
    /// </summary>
    public interface RouteResultViewHandler
    {
        /// <summary>
        /// Handle event.
        /// </summary>
        /// <param name="theEvent">to handle</param>
        /// <param name="exprEvaluatorContext">The expression evaluator context.</param>
        /// <returns>
        /// true if at least one match was found, false if not
        /// </returns>
        bool Handle(EventBean theEvent, ExprEvaluatorContext exprEvaluatorContext);
    }
}

///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client.hook;

namespace com.espertech.esper.support.client
{
    public class SupportExceptionHandlerFactoryRethrow : ExceptionHandlerFactory
    {
        public ExceptionHandler GetHandler(ExceptionHandlerFactoryContext context)
        {
            return Handle;
        }

        public void Handle(ExceptionHandlerContext context)
        {
            throw new ApplicationException(string.Format("Unexpected exception in statement '{0}': {1}",
                context.StatementName, 
                context.Exception.Message), 
                context.Exception);
        }
    }
}

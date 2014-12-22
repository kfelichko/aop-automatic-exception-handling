aop-automatic-exception
=======================

I wrote this example back in the 2003-2004 timeframe for the old GotDotNet site.  It demonstrates a way to implement aspect oriented programming (AOP) in .NET with the ContextBoundObject class.

This example creates an attribute that can be applied to methods on a ContextBoundObject that will automatically perform exception handling without having to add try...catch blocks. 

[ExceptionMethodContextAttribute(WriteToEventLog = true, SwallowException = false, ExceptionReturnValue = "")]

In the above example, the method will add the exception to the Application event log, intercept the return value with an empty string, and throw the exception to the caller.  If we did not want to throw the exception to the caller, we would set SwallowException to true.

Check out the form application which runs through a few scenarios.

Orignally, I did not write any automated test cases which is why there are none included.  Maybe in another 10 years I will add test cases if they are not obsolete.

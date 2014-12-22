using System;

namespace ContextBoundSample
{
	/// <summary>
	/// Our ContextBoundObject-derived class that will test the automatic exception handling.
	/// </summary>
	[ExceptionContext()]
	public class SampleObj : ContextBoundObject
	{
		public String NoCatch()
		{
			throw new ArgumentException("Cannot catch me!!");
		}

		[ExceptionMethodContextAttribute(WriteToEventLog = false, SwallowException = true, ExceptionReturnValue = "Free your mind.")]
		public String SwallowException()
		{
			throw new ArgumentException("Am I caught or not?");
		}

		[ExceptionMethodContextAttribute(WriteToEventLog = true, SwallowException = false, ExceptionReturnValue = "Whoa!")]
		public String WriteException()
		{
			throw new ArgumentException("You've got me?!?!  Who's got you?!?!");
		}

		[ExceptionMethodContextAttribute(WriteToEventLog = true, SwallowException = true, ExceptionReturnValue = "What? Me Worry?")]
		public String Both()
		{
			throw new ArgumentException("Somebody stop me!!");
		}
	}
}

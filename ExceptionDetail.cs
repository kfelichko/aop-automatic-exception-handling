using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;

namespace ContextBoundSample
{
	/// <summary>
	/// The ExceptionContextAttribute attribute should be applied to classes that wish to have 
	/// automatic exception handling for method calls.  The attribute is only responsible for
	/// pushing to load to the ExceptionContextProperty object which creates the sink that runs
	/// as a server object sink.  Only classes that derive from ContextBoundObject can use this
	/// attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ExceptionContextAttribute : Attribute, IContextAttribute
	{
		/// <summary>
		/// Defined by the IContextAttribute interface.  Assigns properties to the new
		/// context.  In this case, a new ExceptionContextProperty object.
		/// </summary>
		/// <param name="ccm">
		/// The construction call message headed for the context bound object that this
		/// attribute is applied to.
		/// </param>
		public void GetPropertiesForNewContext(IConstructionCallMessage ccm)
		{
			// Add the context property
			ccm.ContextProperties.Add(new ExceptionContextProperty());
		}

		/// <summary>
		/// Defined by the IContextAttribute interface.  Checks to make sure that
		/// the current context is OK for the sink.
		/// </summary>
		/// <param name="ctx">
		/// The context whose properties are to be checked.
		/// </param>
		/// <param name="ccm">
		/// Ignored.
		/// </param>
		/// <returns>
		/// A boolean value.
		/// </returns>
		public Boolean IsContextOK(Context ctx, IConstructionCallMessage ccm)
		{
			// Return a boolean value based on whether or not the context currently
			// defines the ExceptionContextProperty key.
			return (ctx.GetProperty("ExceptionContextProperty") != null);
		}
	}

	/// <summary>
	/// The ExceptionMethodContextAttribute attribute should be applied to methods within a 
	/// ContextBoundObject-derived class that contains the ExceptionContextAttribute attribute so that
	/// the methods can take part in automatic exception handling.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ExceptionMethodContextAttribute : Attribute
	{
		/// <summary>
		/// Internal variables that store the properties associated with the method that
		/// this class attributes.
		/// </summary>
		protected Boolean _SwallowException = false;
		protected Boolean _WriteToEventLog = false;
		protected Object _ExceptionReturnValue = null;

		/// <summary>
		/// SwallowException should be set to true to avoid having an exception thrown back
		/// to the client.  The default value is false.
		/// </summary>
		public Boolean SwallowException
		{
			get { return _SwallowException; }
			set { _SwallowException = value; }
		}

		/// <summary>
		/// WriteToEventLog should be set to true when it is necessary to log all exceptions
		/// generated on the server-side to the NT event log.  The default value is false.
		/// </summary>
		public Boolean WriteToEventLog
		{
			get { return _WriteToEventLog; }
			set { _WriteToEventLog = value; }
		}

		/// <summary>
		/// ExceptionReturnValue is only used when an exception is swallowed but a value must be 
		/// returned to the client.  The default value is null.
		/// </summary>
		public Object ExceptionReturnValue
		{
			get { return _ExceptionReturnValue; }
			set { _ExceptionReturnValue = value; }
		}
	}

	/// <summary>
	/// The ExceptionContextProperty defines the necessary methods to create a server object sink
	/// that will handle the exception management.  This object can be changed to exist context-wide
	/// or as an envoy sink.  Refer to .NET documentation for details on how to accomplish that task.
	/// </summary>
	[Serializable()]
	public class ExceptionContextProperty : IContextProperty, IContributeObjectSink
	{
		/// <summary>
		/// Defined by the IContextProperty interface.  This method is called when no more information
		/// should be added to the context.
		/// </summary>
		/// <param name="ctx">
		/// Ignored.
		/// </param>
		public void Freeze(Context ctx)
		{
			// Do not add any more context properties to the context.
		}

		/// <summary>
		/// Defined by the IContextProperty interface.  Always returns true to signal that the current
		/// context is OK for the automatic exception handling to run.
		/// </summary>
		/// <param name="ctx">
		/// Ignored.
		/// </param>
		/// <returns>
		/// A boolean value.
		/// </returns>
		public Boolean IsNewContextOK(Context ctx)
		{
			// Always return true
			return true;
		}

		/// <summary>
		/// Defined by the IContextProperty interface.  Returns a string indicating the name of
		/// the property that this class represents.
		/// </summary>
		public String Name
		{
			get 
			{ 
				return "ExceptionContextProperty"; 
			}
		}

		/// <summary>
		/// Defined by the IContributeObjectSink interface.  This method is called when the server
		/// object sink chain is created and the sink needs to be added.
		/// </summary>
		/// <param name="mro">
		/// Ignored.
		/// </param>
		/// <param name="next">
		/// The next sink in the chain.
		/// </param>
		/// <returns>
		/// An IMessageSink that is to be added to the server object sink chain.
		/// </returns>
		public IMessageSink GetObjectSink(MarshalByRefObject mro, IMessageSink next)
		{
			return new ExceptionContextSink(next);
		}
	}

	/// <summary>
	/// The ExceptionContextSink is responsible for performing the bulk of the work associated with
	/// handling exceptions at the context-level.  When handling the exception, the sink checks the
	/// method's attributes for an attribute of type ExceptionMethodContextAttribute.  The properties
	/// of that attribute, if present, are checked for certain information to determine what steps
	/// can be taken.
	/// </summary>
	public class ExceptionContextSink : IMessageSink
	{
		/// <summary>
		/// We have to store a reference to the next sink in the sink chain.
		/// </summary>
		protected IMessageSink _NextSink;

		/// <summary>
		/// The default constructor is marked as protected to avoid having other classes call it.  This
		/// forces callers to be aware that the class expects a sink.  Whether or not they provide it
		/// is completely up to them.
		/// </summary>
		protected ExceptionContextSink()
		{
		}
        
		/// <summary>
		/// The constructor that all classes creating an instance of this must use.  The internal 
		/// member variable that references the next sink in the chain will be setup.
		/// </summary>
		/// <param name="next">
		/// The next sink in the sink chain.
		/// </param>
		public ExceptionContextSink(IMessageSink next)
		{
			_NextSink = next;
		}

		/// <summary>
		/// Defined by the IMessageSink interface as a read-only property that returns the internal
		/// reference to the next sink in the sink chain.
		/// </summary>
		public IMessageSink NextSink
		{
			get 
			{ 
				return _NextSink; 
			}
		}

		/// <summary>
		/// Defined by the IMessageSink interface.  Synchronously processes the given message.
		/// </summary>
		/// <param name="msg">
		/// The message to process.
		/// </param>
		/// <returns>
		/// A reply message in response to the request.
		/// </returns>
		public IMessage SyncProcessMessage(IMessage msg)
		{
			// Get the return message by passing the processing down the chain.
			IMessage msgRet = _NextSink.SyncProcessMessage(msg);
			
			// Cast the return message to a ReturnMessage object.
			IMethodReturnMessage rmReturn = (IMethodReturnMessage) msgRet;

			// Keep processing only if the return message has an exception associated with it.
			if (rmReturn.Exception != null) HandleMessageException(rmReturn);

			// Pass the message back up the chain.
			return msgRet;
		}

		/// <summary>
		/// Defined by the IMessageSink interface.  Asynchronously processes the given message.
		/// </summary>
		/// <param name="msg">
		/// The message to process.
		/// </param>
		/// <param name="replySink">
		/// The reply sink for the reply message.
		/// </param>
		/// <returns>
		/// Returns an IMessageCtrl interface that provides a way to control asynchronous 
		/// messages after they have been dispatched.
		/// </returns>
		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			// Create a delegate reference to the callback function that will handle async calls.
			AsyncExceptionContextSink.AsyncExceptionDelegate aed = 
				new AsyncExceptionContextSink.AsyncExceptionDelegate(this.AsyncCallback);

			// Create a new AsyncExceptionContextSink object to act as the reply sink.
			replySink = (IMessageSink) new AsyncExceptionContextSink(replySink, aed);

			// Pass the call on to the next sink in the chain.
			return _NextSink.AsyncProcessMessage(msg, replySink);
		}

		/// <summary>
		/// Defined by the IMessageSink interface.  Synchronously processes the given message for
		/// an asynchronous server call.
		/// </summary>
		/// <param name="msg">
		/// The message to process.
		/// </param>
		/// <returns>
		/// A reply message in response to the request.
		/// </returns>
		public IMessage AsyncCallback(IMessage msg)
		{		
			// Cast the return message to a ReturnMessage object.
			IMethodReturnMessage rmReturn = (IMethodReturnMessage) msg;

			// Keep processing only if the return message has an exception associated with it.
			if (rmReturn.Exception != null) HandleMessageException(rmReturn);

			// Pass the message back up the chain.
			return msg;
		}

		/// <summary>
		/// A helper function that retrieves custom method attributes and handles a return method 
		/// exception based on the custom attribute properties.
		/// </summary>
		/// <param name="msgRet">
		/// The return message information that contains the exception to handle.
		/// </param>
		protected void HandleMessageException(IMethodReturnMessage msgRet)
		{
			// Get the method information for the method that was originally called.
			Type calledType = Type.GetType(msgRet.TypeName);
			MethodInfo calledMethod = calledType.GetMethod(msgRet.MethodName);
			
			// Retrieve the ExceptionMethodContextAttribute attribute, if present.  Do not search 
			// down to the base class for inherited attributes.
			Object [] customAttributes = calledMethod.GetCustomAttributes(
				typeof(ExceptionMethodContextAttribute), false);

			// Skip out if the attribute does not exist for the method.
			if (customAttributes.Length == 0) return;
			
			// Perform a casting operation to get the type that we need to check.
			Debug.Assert(customAttributes[0].GetType() == typeof(ExceptionMethodContextAttribute));
			ExceptionMethodContextAttribute emca = (ExceptionMethodContextAttribute) customAttributes[0];
                    
			// Write to the event log if attributed to do so.
			if (emca.WriteToEventLog)
			{
				EventLog.WriteEntry("Application", String.Format("Exception thrown: {0}", 
					msgRet.Exception.ToString()));
			}

			// Remove the exception from the return message if attributed to do so.
			if (emca.SwallowException)
			{
				// WARNING: This uses reflection to wipe out a reference in a private variable.  Since
				// the variable is not documented, it could change in future releases of the .NET
				// Framework.  As much as I wanted to get around having to do this, I could not find
				// any other way at this time.
				Type rmType = Type.GetType("System.Runtime.Remoting.Messaging.ReturnMessage");
				FieldInfo rmException = rmType.GetField("_e", BindingFlags.NonPublic | BindingFlags.Instance);
				if (rmException != null) rmException.SetValue(msgRet, null);

				// Update the default return value when the exception is swallowed using reflection.
				FieldInfo rmReturnValue = rmType.GetField("_ret", BindingFlags.NonPublic | BindingFlags.Instance);
				if (rmReturnValue != null) rmReturnValue.SetValue(msgRet, emca.ExceptionReturnValue);
			}
		}
	}

	/// <summary>
	/// The AsyncExceptionContextSink gets called when an asynchronous method call returns from being
	/// processed by the context-bound object.
	/// </summary>
	public class AsyncExceptionContextSink : IMessageSink
	{
		/// <summary>
		/// The delegate that must be implemented in order to get a callback for async processing.
		/// </summary>
		public delegate IMessage AsyncExceptionDelegate(IMessage msg);

		/// <summary>
		/// Store our protected reference to the next reply sink and the delegate to call when this
		/// sink is called.
		/// </summary>
		protected IMessageSink _NextSink;
		protected AsyncExceptionDelegate _Delegate;

		/// <summary>
		/// The default constructor is marked as protected to avoid having other classes call it.  This
		/// forces callers to be aware that the class expects a sink and a delegate.  Whether or not 
		/// they provide it is completely up to them.
		/// </summary>
		protected AsyncExceptionContextSink()
		{
		}

		/// <summary>
		/// The constructor that all classes creating an instance of this must use.  This class should
		/// only be used for handling asynchronous message calls.
		/// </summary>
		/// <param name="next">
		/// The next sink in the sink chain.
		/// </param>
		/// <param name="aed">
		/// The delegate that will be called when an async call returns.
		/// </param>
		public AsyncExceptionContextSink(IMessageSink next, AsyncExceptionDelegate aed)
		{
			_NextSink = next;
			_Delegate = aed;
		}

		/// <summary>
		/// Defined by the IMessageSink interface as a read-only property that returns the internal
		/// reference to the next sink in the sink chain.
		/// </summary>
		public IMessageSink NextSink
		{
			get 
			{ 
				return _NextSink; 
			}
		}

		/// <summary>
		/// Defined by the IMessageSink interface.  Synchronously processes the given message.
		/// </summary>
		/// <param name="msg">
		/// The message to process.
		/// </param>
		/// <returns>
		/// A reply message in response to the request.
		/// </returns>
		public IMessage SyncProcessMessage(IMessage msg)
		{
			// Process the message via a delegate call if the delegate is not null.
			if (_Delegate != null) return _Delegate(msg);

			// Otherwise, return the original message.
			return msg;
		}

		/// <summary>
		/// Defined by the IMessageSink interface.  Asynchronously processes the given message.
		/// </summary>
		/// <param name="msg">
		/// Ignored.
		/// </param>
		/// <param name="replySink">
		/// Ignored.
		/// </param>
		/// <returns>
		/// Ignored.
		/// </returns>
		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			// Return a null reference since this method cannot be called on this object since
			// it is meant to only handle processing of asynchronous message calls.
			return null;
		}
	}
}

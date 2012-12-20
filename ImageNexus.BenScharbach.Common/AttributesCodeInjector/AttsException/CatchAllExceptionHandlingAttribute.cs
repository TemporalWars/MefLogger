using System;
using System.Reflection;
using PostSharp.Aspects;

namespace ImageNexus.BenScharbach.Common.AttributesCodeInjector.AttsException
{
    // 11/16/2010
    /// <summary>
    /// The <see cref="CatchAllExceptionHandlingAttribute"/> is used to capture
    /// any misc exception errors and then log them into windows event logger.
    /// </summary>
    [Serializable]
    public class CatchAllExceptionHandlingAttribute : OnExceptionAspect
    {
        // Stores the declaring target method name
        private string _entireMethodName;

        // Should return some value?
        private readonly bool _returnValue;

        // 12/12/2010
        private readonly Type _valueType;
        private readonly object _valueToReturn;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="returnValue">Should return default value?</param>
        /// <param name="valueType">Type to return; for example, string or bool types.</param>
        /// <param name="valueToReturn">Default value to return</param>
        public CatchAllExceptionHandlingAttribute(bool returnValue, Type valueType, object valueToReturn)
        {
            _returnValue = returnValue;
            _valueType = valueType;
            _valueToReturn = valueToReturn;
        }

        // Invoked only once at runtime from the static constructor of type declaring the target method.
        public override void RuntimeInitialize(MethodBase method)
        {
            base.RuntimeInitialize(method);

            _entireMethodName = method.DeclaringType.Name + "." + method.Name;
        }

        // Method invoked at run time.
        public override void OnException(MethodExecutionArgs args)
        {
            // Format the exception message.
            var message = string.Format("The {0} threw the exception - {1} ",
                                        _entireMethodName, args.Exception.Message);

            
            if (args.Exception.GetType() == typeof(Exception))
            {
                Console.WriteLine(message); // 7/30/2011
            }

            // Set to False, since exception thrown
            if (_returnValue)
                args.ReturnValue = Convert.ChangeType(_valueToReturn, _valueType);

            // Do not rethrow the exception.
            args.FlowBehavior = FlowBehavior.Return;
        }

    }
}

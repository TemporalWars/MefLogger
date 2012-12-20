using System;
using System.IO;
using System.Reflection;
using System.Security;
using PostSharp.Aspects;

namespace ImageNexus.BenScharbach.Common.AttributesCodeInjector.AttsException
{
    // 11/15/2010
    [Serializable]
    public class DirectoryInfoExceptionHandlingAttribute : OnExceptionAspect
    {
        // Stores the declaring target method name
        private string _entireMethodName;

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


            if (args.Exception.GetType() == typeof(UnauthorizedAccessException))
            {
                Console.WriteLine(message);
            }
            else if (args.Exception.GetType() == typeof(SecurityException))
            {
                Console.WriteLine(message);
            }
            else if (args.Exception.GetType() == typeof(IOException))
            {
                Console.WriteLine(message);
            }
            else if (args.Exception.GetType() == typeof(Exception))
            {
                throw new Exception(message);
            }

            // Do not rethrow the exception.
            args.FlowBehavior = FlowBehavior.Return;
        }

    }
}

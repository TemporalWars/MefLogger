using System;
using System.Windows.Threading;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace ImageNexus.BenScharbach.Common.AttributesCodeInjector.AttsThreading
{
    // 7/17/2011- Updated to include the new 'MulticastAttributes.Static' to allow to be placed on Static methods!
    // 11/15/2010
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static)]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class OnGuiThreadAttribute : MethodInterceptionAspect
    {
        /// <summary>
        /// Describes the priorities at which operations can be invoked by way of the System.Windows.Threading.Dispatcher.
        /// </summary>
        public DispatcherPriority Priority { get; set; }

        /// <summary>
        /// Is asynchronous call?
        /// </summary>
        public bool Asynchronous { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dispatcherPriority"> Describes the priorities at which operations can be invoked by way of the System.Windows.Threading.Dispatcher.</param>
        public OnGuiThreadAttribute(DispatcherPriority dispatcherPriority)
        {
            Priority = dispatcherPriority;
        }

        // 12/2/2010
        // Method invoked at build time. It validates that the aspect has been applied to an acceptable method.
        /*public override bool CompileTimeValidate(MethodBase method)
        {
            // The method must be in a type derived from DispatcherObject.
            if (!typeof(DispatcherObject).IsAssignableFrom(method.DeclaringType))
            {
                Message.Write(SeverityType.Error, "CUSTOM02",
                               "Cannot apply [GuiThread] to method {0} because it is not a member of a type " +
                               "derived from DispatcherObject.", method);
                return false;
            }

            // If the call is asynchronous, there should not be any return value or ByRef parameter.
            if (Asynchronous)
            {
                if (((MethodInfo)method).ReturnType == typeof(void) ||
                    method.GetParameters().Any(parameter => parameter.ParameterType.IsByRef))
                {
                    Message.Write(SeverityType.Error, "CUSTOM02",
                                  "Cannot apply [GuiThread(Asynchronous=true)] to method {0} because it is not a member " +
                                  "of a type derived from DispatcherObject.", method);
                    return false;
                }
            }

            return true;
        }*/


        /// <summary>
        /// Method invoked <i>instead</i> of the method to which the aspect has been applied.
        /// </summary>
        /// <param name="args">Advice arguments.</param>
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            // Get the graphic object.
            var dispatcherObject = (DispatcherObject)args.Instance;

            // Check whether the current thread has access to this object.
            if (dispatcherObject.CheckAccess())
            {
                // We have access. Proceed with invocation.
                args.Proceed();
            }
            else
            {
                // We don't have access. Invoke the method synchronously.
                if (Asynchronous)
                {
                    dispatcherObject.Dispatcher.BeginInvoke(Priority, new Action(args.Proceed));
                }
                else
                {
                    dispatcherObject.Dispatcher.Invoke(Priority, new Action(args.Proceed));
                }
            }

        }
    }
}

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PostSharp.Aspects;

namespace ImageNexus.BenScharbach.Common.AttributesCodeInjector.AttsMisc
{
    // 11/15/2010
    /// <summary>
    /// The <see cref="ShowWaitCursorAttribute"/> is used to show the <see cref="Cursors.Wait"/> during
    /// the excecution of the method decorated with this attribute.  Once method operation is complete, the
    /// default <see cref="Cursors.Arrow"/> is set.
    /// </summary>
    [Serializable]
    public class ShowWaitCursorAttribute : OnMethodBoundaryAspect
    {
        /// <summary>
        /// Set when making an asynchronous begin/end call pattern. 
        /// </summary>
        public enum AsynchronousCallEnum
        {
            /// <summary>
            /// Default - Will Show and Remove WaitCursor.
            /// </summary>
            None,
            /// <summary>
            /// Keeps showing WaitCursor Icon.
            /// </summary>
            BeginCall,
            /// <summary>
            /// Completes asnychronous process by removing WaitCursor Icon.
            /// </summary>
            EndCall
        }

        /// <summary>
        /// Used to set signify an asynchronous calling pattern.  When set, the
        /// WaitCursor Icon will be Shown until the 'EndCall' method completes. 
        /// </summary>
        public AsynchronousCallEnum AsynchronousCallType = AsynchronousCallEnum.None;

        [NonSerialized]
        private Dispatcher _dispatcher;

        /// <summary>
        /// Saves the WPF Application's <see cref="Dispatcher"/> instance during the first runtime initilization.
        /// </summary>
        /// <param name="method"></param>
        public override void RuntimeInitialize(System.Reflection.MethodBase method)
        {
            base.RuntimeInitialize(method);

            if (Application.Current != null) 
                _dispatcher = Application.Current.Dispatcher;
        }

        /// <summary>
        /// Sets the Applicatoin Cursor to the <see cref="Cursors.Wait"/> user choice.
        /// </summary>
        /// <param name="args">Instance of <see cref="MethodExecutionArgs"/>.</param>
        public override void OnEntry(MethodExecutionArgs args)
        {
            // check if null
            if (_dispatcher == null)
            {
                if (Application.Current != null)
                    _dispatcher = Application.Current.Dispatcher;
                
                if (_dispatcher == null)
                    SetWaitCursor();

                return;
            }

            // Check whether the current thread has access to this object.
            if (_dispatcher.CheckAccess())
            {
                SetWaitCursor();
            }
            else
            {
                _dispatcher.Invoke(DispatcherPriority.Normal, new Action(SetWaitCursor));
            }
           
        }


        /// <summary>
        /// Sets the Application cursor back to the default <see cref="Cursors.Arrow"/>.
        /// </summary>
        /// <param name="args">Instance of <see cref="MethodExecutionArgs"/>.</param>
        public override void OnExit(MethodExecutionArgs args)
        {
            // 2/11/2011 - Check if Async Call
            if (AsynchronousCallType == AsynchronousCallEnum.BeginCall)
                return;

            // check if null
            if (_dispatcher == null)
            {
                if (Application.Current != null)
                    _dispatcher = Application.Current.Dispatcher;

                if (_dispatcher == null)
                    SetWaitCursor();

                return;
            }

            // Check whether the current thread has access to this object.
            if (_dispatcher.CheckAccess())
            {
                SetDefaultCursor();
            }
            else
            {
                _dispatcher.Invoke(DispatcherPriority.Normal, new Action(SetDefaultCursor));
            }
           
        }

        /// <summary>
        ///  Sets the Application cursor to the 'Wait'.
        /// </summary>
        private static void SetWaitCursor()
        {
            // Show cursor type choosen.
            if (Application.Current != null)
                Application.Current.MainWindow.Cursor = Cursors.Wait;
        }

        /// <summary>
        ///  Sets the Application cursor to the 'Arrow'.
        /// </summary>
        private static void SetDefaultCursor()
        {
            // Show cursor type choosen.
            if (Application.Current != null)
                Application.Current.MainWindow.Cursor = Cursors.Arrow;
        }
    }
}

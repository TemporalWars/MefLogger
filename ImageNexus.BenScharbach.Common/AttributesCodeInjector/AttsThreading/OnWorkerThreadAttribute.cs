using System;
using System.ComponentModel;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;

namespace ImageNexus.BenScharbach.Common.AttributesCodeInjector.AttsThreading
{
    // 11/15/2010
    [Serializable]
    public class OnWorkerThreadAttribute : MethodInterceptionAspect, IInstanceScopedAspect
    {
        // At runtime, this field is set to a delegate of the method RunWorkerCompletedEventHandler before we override it.
        [ImportMember("RunWorkerCompleted", IsRequired = false, Order = ImportMemberOrder.BeforeIntroductions)] 
        public RunWorkerCompletedEventHandler RunWorkCompleted;

        /// <summary>
        /// Method invoked <i>instead</i> of the method to which the aspect has been applied.
        /// </summary>
        /// <param name="methodInterceptionArgs">Advice arguments.</param>
        public override void OnInvoke(MethodInterceptionArgs methodInterceptionArgs)
        {
            //ThreadPool.QueueUserWorkItem(state => args.Proceed());

            // Create BackgroundWorker thread.
            using (var worker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true})
            {
                worker.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs e)
                                                            {
                                                                using (var workerSent = sender as BackgroundWorker)
                                                                {
                                                                    // Invoke method to process.
                                                                    methodInterceptionArgs.Proceed();

                                                                    // Returns results.
                                                                    e.Result = methodInterceptionArgs.ReturnValue;
                                                                   
                                                                }
                                                            });

                // Set the ProgressChanged delegate callback
                worker.ProgressChanged += delegate(object s, ProgressChangedEventArgs args)
                                              {
                                                  // update progressbar
                                                  //_progressBar.Value = (double)args.UserState;
                                              };

                // Set the WorkCompleted delegate callback
                worker.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
                                                 {
                                                     // set bar to 100%>
                                                     //_progressBar.Value = _progressBar.Maximum;
                                                     if (RunWorkCompleted != null)
                                                         RunWorkCompleted(sender1, e1);
                                                     
                                                 };

                // Start work.
                worker.RunWorkerAsync();

            } // End Using Worker
        }


        public object CreateInstance(AdviceArgs adviceArgs)
        {
            return MemberwiseClone();
        }

        public void RuntimeInitializeInstance()
        {
            // empty
        }
    }
}

using System.Collections.Generic;
using System.Fabric;

using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;


namespace Gateway
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Gateway : StatelessService
    {
        private UdpCommunicationListener listener;

        public Gateway(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            yield return new ServiceInstanceListener(initParams =>
            {
                this.listener = new UdpCommunicationListener();
                this.listener.Initialize(initParams.CodePackageActivationContext);

                return this.listener;
            });
        }

        ///// <summary>
        ///// This is the main entry point for your service instance.
        ///// </summary>
        ///// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        //protected override async Task RunAsync(CancellationToken cancellationToken)
        //{
        //    // TODO: Replace the following sample code with your own logic 
        //    //       or remove this RunAsync override if it's not needed in your service.

        //    long iterations = 0;

        //    while (true)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

        //        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        //    }
        //}
    }
}

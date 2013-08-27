using System.ServiceModel;

namespace AdSage.Concert.Test.Framework
{
    public class WCFServiceProxy<T> : ClientBase<T> where T : class
    {
        public WCFServiceProxy()
        {

        }

        public WCFServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        { }


        public WCFServiceProxy(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }


        public WCFServiceProxy(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public WCFServiceProxy(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        public T Service
        {
            get { return Channel; }
        }
    }
}

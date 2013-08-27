using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdSage.Concert.Test.Framework;
using HostingService.Test.Framework.SyncDataService;

namespace HostingService.Test.SyncDataService
{
    public class SyncDataServiceTestBase : TestBase
    {
        protected SyncDataServiceHelper SyncDataServiceHelper;

        public override void OnTestInitialize()
        {
            base.OnTestInitialize();
            SyncDataServiceHelper = Get<SyncDataServiceHelper>();
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdSage.Concert.Hosting.Service.Facebook;
using AdSage.Concert.Hosting.Application.DTO.Facebook;
using AdSage.Concert.Test.Framework;
using AdSage.Concert.Hosting.Service.AdCenter;
using AdSage.Concert.Hosting.Application.DTO;
using AdSage.Concert.Hosting.Service.Google;
using AdSage.Concert.Hosting.Service.Tracking;
using AdSage.Concert.Hosting.Service.Command;
using AdSage.Concert.Hosting.Command;
using AdSage.Concert.Hosting.Service.Advertiser;
using AdSage.Concert.Hosting.Service;

namespace HostingService.Test.SmokeTest
{
    [TestClass]
    public class SmokeTest
    {
        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verify facebook service deployment")]
        public void FacebookServiceSmokeTest()
        {
            WCFHelper.Using<WCFServiceProxy<IFacebookService>>(new WCFServiceProxy<IFacebookService>("FacebookService"), client =>
                {
                    try
                    {
                        client.Service.GetUserById(0);
                    }
                    catch(Exception ex)
                    {
                        Assert.AreEqual("Value cannot be null.\r\nParameter name: source", ex.Message);
                        return;
                    }
                    Assert.Fail("Should throw an exception says 'Value cannot be null.\r\nParameter name: source'");
                });
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verify adcenter service deployment")]
        public void AdCenterServiceSmokeTest()
        {
            WCFHelper.Using<WCFServiceProxy<IAdCenterService>>(new WCFServiceProxy<IAdCenterService>("AdCenterService"), client =>
                {
                    client.Service.GetParent(0, AdSage.Concert.Hosting.Common.SEMObjectDetailType.Unknown, false);
                });
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verify google service deployment")]
        public void GoogleServiceSmokeTest()
        {
            WCFHelper.Using<WCFServiceProxy<IGoogleService>>(new WCFServiceProxy<IGoogleService>("GoogleService"), client =>
                {
                    client.Service.GetParent(0, AdSage.Concert.Hosting.Common.SEMObjectDetailType.Unknown, false);
                });
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verify tracking service deployment")]
        public void TrackingServiceSmokeTest()
        {
            WCFHelper.Using<WCFServiceProxy<ITrackingService>>(new WCFServiceProxy<ITrackingService>("TrackingService"), client =>
                {
                    try
                    {
                        client.Service.GetConversionsByAdvertiser(null);
                    }
                    catch (Exception ex)
                    {
                        Assert.AreEqual("Value cannot be null.\r\nParameter name: source\r\n", ex.Message);
                        return;
                    }
                    Assert.Fail("Should throw an exception says 'Value cannot be null.\r\nParameter name: source\r\n'");
                });
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verify command service deployment")]
        public void CommandServiceSmokeTest()
        {
            WCFHelper.Using<WCFServiceProxy<ICommandService>>(new WCFServiceProxy<ICommandService>("CommandService"), client =>
                {
                    client.Service.SendCommands(new List<HostingCommand>());
                });
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verify advertiser service deployment")]
        public void AdvertiserServiceSmokeTest()
        {
            WCFHelper.Using<WCFServiceProxy<IAdvertiserService>>(new WCFServiceProxy<IAdvertiserService>("AdvertiserService"), client =>
                {
                    try
                    {
                        client.Service.GetAccountsAndUserById(null);
                    }
                    catch (Exception ex)
                    {
                        Assert.AreEqual("Value cannot be null.\r\nParameter name: advertiser\r\n", ex.Message);
                        return;
                    }
                    Assert.Fail("Value cannot be null.\r\nParameter name: advertiser\r\n");
                });
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verify query event service deployment")]
        public void QueryEventServiceSmokeTest()
        {
            WCFHelper.Using<WCFServiceProxy<IQueryEventService>>(new WCFServiceProxy<IQueryEventService>("QueryEventService"), client =>
                {
                    client.Service.GetAllCommandInfoList();
                });
        }
    }
}

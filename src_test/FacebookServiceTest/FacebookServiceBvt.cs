using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdSage.Concert.Hosting.Service.Facebook;
using AdSage.Concert.SEMObjects;
using HostingService.Test.Databases.SEMObjects;
using HostingService.Test.Framework;
using AdSage.Concert.Test.Framework;
using HostingService.Test.Framework.FacebookService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommandResponse = AdSage.Concert.Hosting.Command;

namespace HostingService.Test.FacebookService
{
    /// <summary>
    /// Summary description for FacebookServiceBvt
    /// </summary>
    [TestClass]
    public class FacebookServiceBvt : FacebookServiceTestBase
    {
        [TestMethod]
        [Priority(0)]
        [Owner("ZhuYing")]
        [Description("Verify that AddUser Interface can successfully update a exist user by confirming the fetched user information")]
        [TestCategory("User")]
        public void TestAddUser_FacebookUser()
        {
            CommandResponse.Facebook.AddCommand.AddFacebookUserCommand addFacebookUserCommand;
            TblFacebookUser responseFacebookUser = FacebookServiceHelper.TestAddUser_FacebookUser(out addFacebookUserCommand);

            Assert.IsNotNull(responseFacebookUser, "The responseRow object cannot be NULL!!");
            Assert.AreEqual(addFacebookUserCommand.Id, responseFacebookUser.Id, "The response facebookUser's Id should be equal to the one before Insertint the Database!!");
            Assert.AreEqual(addFacebookUserCommand.LocalId, responseFacebookUser.LocalId, "The response facebookUser's LocalId should be equal to the one before Insertint the Database!!");
            Assert.AreEqual(addFacebookUserCommand.Email, responseFacebookUser.Email, "The response facebookUser's Email should be equal to the one before Insertint the Database!!");
            Assert.AreEqual(addFacebookUserCommand.ApiKey, responseFacebookUser.ApiKey, "The response facebookUser's ApiKey should be equal to the one before Insertint the Database!!");
            Assert.AreEqual(addFacebookUserCommand.ApplicationSecret, responseFacebookUser.ApplicationSecret, "The response facebookUser's ApplicationSecret should be equal to the one before Insertint the Database!!");
            Assert.AreEqual(addFacebookUserCommand.ApplicationId, responseFacebookUser.ApplicationId, "The response facebookUser's ApplicationId should be equal to the one before Insertint the Database!!");
            Assert.AreEqual(addFacebookUserCommand.AccessToken, responseFacebookUser.Accesstoken, "The response facebookUser's AccessToken should be equal to the one before Insertint the Database!!");
            Assert.AreEqual(addFacebookUserCommand.UserName, responseFacebookUser.UserName, "The response facebookUser's UserName should be equal to the one before Insertint the Database!!");
            Assert.AreEqual(addFacebookUserCommand.ImagePath, responseFacebookUser.ImagePath, "The response facebookUser's ImagePath should be equal to the one before Insertint the Database!!");
            Assert.AreEqual(addFacebookUserCommand.ImageUrl, responseFacebookUser.ImageUrl, "The response facebookUser's ImageUrl should be equal to the one before Insertint the Database!!");
        }

        [TestMethod]
        [Priority(0)]
        [Owner("ZhuYing")]
        [Description("Verify that UpdateUser Interface can successfully update a exist user by confirming the fetched user information")]
        [TestCategory("User")]
        public void TestUpdateUser_FacebookUser()
        {

        }

    }
}

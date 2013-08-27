using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using AdSage.Concert.Hosting.Command.Facebook.AddCommand;
using AdSage.Concert.Hosting.Service.Facebook;
using AdSage.Concert.SEMObjects;
using AdSage.Concert.Test.Framework;
using HostingService.Test.Databases.SEMObjects;
using HostingService.Test.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommandResponse = AdSage.Concert.Hosting.Command;

namespace HostingService.Test.Framework.FacebookService
{
    public class FacebookServiceHelper : TestHelper
    {
        public SEMObjects SEMObjects { get; private set; }
        public RandomData RandomData { get; private set; }

        private IList<TblFacebookUser> createdFacebookUserEntitiesForCleanup;

        #region Business Methods

        public TblFacebookUser TestAddUser_FacebookUser(out AddFacebookUserCommand addFacebookUserCommand)
        {
            addFacebookUserCommand = NextFacebookUserCommand();
            TblFacebookUser facebookUserEntity = NextFacebookUserEntity();
            facebookUserEntity.Id = addFacebookUserCommand.Id;
            facebookUserEntity.LocalId = addFacebookUserCommand.LocalId;

            RegisterCreatedFacebookUserEntitiesForCleanup(facebookUserEntity);
            InsertFacebookUserIntoDatabsase(facebookUserEntity);
            AddFacebookUserCommand facebookUserCommandForLambdaExpression = addFacebookUserCommand;
            WCFHelper.Using<WCFServiceProxy<IFacebookService>>(new WCFServiceProxy<IFacebookService>("FacebookService"),
                client =>
                {
                    client.Service.AddUser(facebookUserCommandForLambdaExpression);
                });

            long facebookUserId = addFacebookUserCommand.Id;
            facebookUserEntity = GetFacebookUserEntity(facebookUserId);
            return facebookUserEntity;
        }

        #endregion

        #region Assist Methods

        public AddFacebookUserCommand NextFacebookUserCommand()
        {
            CommandResponse.Facebook.AddCommand.AddFacebookUserCommand addFacebookUserCommand=new AddFacebookUserCommand();

            addFacebookUserCommand.LocalId = RandomData.NextUInt32();
            addFacebookUserCommand.Id = RandomData.NextUInt32();
            addFacebookUserCommand.Email = RandomData.NextUnicodeEmail(30);
            addFacebookUserCommand.ApiKey = RandomData.NextAsciiWord(20, 35);
            addFacebookUserCommand.ApplicationSecret = RandomData.NextAsciiWord(28, 32);
            addFacebookUserCommand.ApplicationId = RandomData.NextDigits(16);
            addFacebookUserCommand.AccessToken = RandomData.NextAsciiWord(100, 112);
            addFacebookUserCommand.UserName = RandomData.NextUnicodeWord(15);
            addFacebookUserCommand.ImagePath = RandomData.NextUnicodeUrl(30);
            addFacebookUserCommand.ImageUrl = RandomData.NextUnicodeUrl(50);
            addFacebookUserCommand.OperatorID = RandomData.NextDigits(16);
            addFacebookUserCommand.CommandID = RandomData.NextGuid();
            addFacebookUserCommand.AdvertiserID = RandomData.NextDigits(16);

            return addFacebookUserCommand;
        }

        public TblFacebookUser NextFacebookUserEntity()
        {
            TblFacebookUser tblFacebookUser = new TblFacebookUser();
            tblFacebookUser.LocalStatus = RandomData.NextByte();
            tblFacebookUser.LocalState = RandomData.NextByte();
            tblFacebookUser.LastUpdateTime = RandomData.NextDateTime(new TimeSpan(1, 0, 0, 0));
            tblFacebookUser.LocalId = RandomData.NextUInt32();
            tblFacebookUser.Id = RandomData.NextUInt32();
            tblFacebookUser.Email = RandomData.NextUnicodeEmail(30);
            tblFacebookUser.Password = RandomData.NextDigits(20);
            tblFacebookUser.ApiKey = RandomData.NextAsciiWord(20, 35);
            tblFacebookUser.ApplicationSecret = RandomData.NextAsciiWord(28, 32);
            tblFacebookUser.ApplicationId = RandomData.NextDigits(16);
            tblFacebookUser.SessionSecret = RandomData.NextAsciiWord(28, 32);
            tblFacebookUser.SessionKey = RandomData.NextAsciiWord(20, 35);
            tblFacebookUser.Accesstoken = RandomData.NextAsciiWord(100, 112);
            tblFacebookUser.UserName = RandomData.NextUnicodeWord(15);
            tblFacebookUser.ImagePath = RandomData.NextUnicodeUrl(30);
            tblFacebookUser.ImageUrl = RandomData.NextUnicodeUrl(50);
            tblFacebookUser.Version = RandomData.NextUInt16();

            return tblFacebookUser;
        }

        public void RegisterCreatedFacebookUserEntitiesForCleanup(TblFacebookUser facebookUserEntity)
        {
            createdFacebookUserEntitiesForCleanup.Add(facebookUserEntity);
        }

        public void InsertFacebookUserIntoDatabsase(TblFacebookUser facebookUserEntity)
        {
            SEMObjects.TblFacebookUser.InsertOnSubmit(facebookUserEntity);
            SEMObjects.SubmitChanges();
        }

        public TblFacebookUser GetFacebookUserEntity(long userId)
        {
            this.SEMObjects.Refresh(RefreshMode.OverwriteCurrentValues, SEMObjects.TblFacebookUser);
            var result = (from i in SEMObjects.TblFacebookUser where i.Id == userId select i).Single();
            return result;
        }

        #endregion

        #region Recycle Methods

        public override void OnHelperCreation(TestBase test)
        {
            base.OnHelperCreation(test);
            RandomData = test.Get<RandomData>();
            SEMObjects = test.Get<SEMObjects>();

            createdFacebookUserEntitiesForCleanup = test.Get<List<TblFacebookUser>>();

            test.AddTestCleanup("Cleanup SEMObjectDB", () => { onDBCleanup(); });
        }

        private void onDBCleanup()
        {
            //clean up facebook user
            SqlParameter facebookUserSqlParameter = new SqlParameter("@LocalId", SqlDbType.BigInt);
            foreach (var entity in createdFacebookUserEntitiesForCleanup)
            {
                facebookUserSqlParameter.Value = entity.LocalId;
                SEMObjects.GetOpenSqlConnection().ExecuteNonQuery(
                    "delete from [SEMObjects].[dbo].[tblFacebookUser] where [LocalId] = @LocalId",
                     facebookUserSqlParameter);
            }

            //cleanup all other entries
            SEMObjects.GetOpenSqlConnection().ExecuteNonQuery("delete from [SEMObjects].[dbo].[tblFacebookUser]");
        }

        #endregion

        #region abandoned Code

        //private DataRow[] FetchFacebookUserRecordFromDB(long userId)
        //{
        //    DataRow[] specifiedFacebookUser;
        //    using (DataTable facebookUserTable = new DataTable())
        //    using (SqlDataAdapter facebookUserAdapter = new SqlDataAdapter("SELECT * FROM [SEMObjects].[dbo].[tblFacebookUser]", SEMObjects.GetOpenSqlConnection()))
        //    {
        //        facebookUserAdapter.Fill(facebookUserTable);
        //        specifiedFacebookUser = facebookUserTable.Select("Id=" + userId.ToString());
        //    }
        //    return specifiedFacebookUser;
        //}

        #endregion
    }
}

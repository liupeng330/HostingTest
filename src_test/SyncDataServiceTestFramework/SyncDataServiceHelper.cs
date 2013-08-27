using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using AdSage.Concert.Hosting.Client.Core.Utility;
using AdSage.Concert.SEMObjects;
using HostingService.Test.Databases.SEMObjects;
using AdSage.Concert.Test.Framework;
using ProxyDTO = AdSage.Concert.Hosting.Application.DTO;
using ProxySync = AdSage.Concert.Hosting.Application.DTO.Sync;
using AdSage.Concert.Hosting.Application.DTO.Facebook;
using AdSage.Concert.Hosting.Api.Facebook.MgrObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HostingService.Test.Framework.SyncDataService
{
    public class SyncDataServiceHelper : TestHelper
    {
        public RandomData RandomData { get; private set; }
        public SEMObjects SEMObjects { get; private set; }

        private IList<TblFacebookCampaign> createdFacebookCampaignEntitiesForCleanup;
        private IList<TblSequenceNumber> createdSequenceNumberEntitiesForCleanup;
        private IList<TblFacebookAdGroup> createdFacebookAdGroupEntitiesForCleanup;
        private IList<TblFacebookAccount> createdFacebookAccountEntitiesForCleanup;
        private IList<TblFacebookUser> createdFacebookUserEntitiesFroCleanup;

        public override void OnHelperCreation(TestBase test)
        {
            base.OnHelperCreation(test);
            RandomData = test.Get<RandomData>();
            SEMObjects = test.Get<SEMObjects>();

            createdFacebookCampaignEntitiesForCleanup = test.Get<List<TblFacebookCampaign>>();
            createdSequenceNumberEntitiesForCleanup = test.Get<List<TblSequenceNumber>>();
            createdFacebookAdGroupEntitiesForCleanup=test.Get<List<TblFacebookAdGroup>>();
            createdFacebookAccountEntitiesForCleanup = test.Get<List<TblFacebookAccount>>();
            createdFacebookUserEntitiesFroCleanup = test.Get<List<TblFacebookUser>>();

            test.AddTestCleanup("Cleanup SEMObjectDB", () => { onDBCleanup(); });
        }

        public static Guid GenerateLocalId(ProxySync.ObjectState objectState)
        {
            if (objectState.ObjectDetailType == (int)SEMObjectDetailType.FacebookAccount ||
                objectState.ObjectDetailType == (int)SEMObjectDetailType.FacebookUser ||
                objectState.ObjectDetailType == (int)SEMObjectDetailType.GoogleAccountInfo ||
                objectState.ObjectDetailType == (int)SEMObjectDetailType.GoogleUser ||
                objectState.ObjectDetailType == (int)SEMObjectDetailType.AdCenterAccount ||
                objectState.ObjectDetailType == (int)SEMObjectDetailType.AdCenterUser)
            {
                return GuidGenerationHelper.StringToGUID(
                    objectState.EngineType.ToString() +
                    objectState.ObjectId.ToString());
            }
            else
            {
                return GuidGenerationHelper.StringToGuiD(
                       (int)objectState.EngineType,
                       (int)objectState.ObjectDetailType,
                       objectState.ParentId,
                       objectState.HostingId);
            }
        }

        /// <summary>
        ///随机产生ObjectState对象，不包括计算LocalId
        /// </summary>
        /// <returns></returns>
        public ProxySync.ObjectState NextObjectState()
        {
            ProxySync.ObjectState objectState = new ProxySync.ObjectState();

            objectState.ObjectDetailType = (int)NextSEMObjectDetailType();
            objectState.LocalVersion = (Int64)RandomData.NextUInt32();
            objectState.Operation = NextOperationType();
            objectState.EngineType = (int)NextSearchEngineType();
            objectState.IsVersionCompare = RandomData.NextBoolean();
            objectState.HostingId = (Int64)RandomData.NextUInt32();
            objectState.ParentId = (Int64)RandomData.NextUInt32();
            objectState.ObjectId = (Int64)RandomData.NextUInt32();

            return objectState;
        }

        private SEMObjectDetailType NextSEMObjectDetailType()
        {
            return RandomData.NextChoice<SEMObjectDetailType>(
                SEMObjectDetailType.FacebookAccount,
                SEMObjectDetailType.FacebookUser,
                SEMObjectDetailType.FacebookCampaign,
                SEMObjectDetailType.FacebookAdGroup,

                SEMObjectDetailType.AdCenterAccount,
                SEMObjectDetailType.AdCenterUser,
                SEMObjectDetailType.AdCenterCampaign,
                SEMObjectDetailType.AdCenterAdGroup,

                SEMObjectDetailType.GoogleAccountInfo,
                SEMObjectDetailType.GoogleUser,
                SEMObjectDetailType.GoogleCampaign,
                SEMObjectDetailType.GoogleAdGroup);
        }

        private ProxySync.OperationType NextOperationType()
        {
            return RandomData.NextChoice<ProxySync.OperationType>(
                ProxySync.OperationType.Delete,
                ProxySync.OperationType.Insert,
                ProxySync.OperationType.Update,
                ProxySync.OperationType.UpdateProperties);
        }

        private SearchEngineType NextSearchEngineType()
        {
            return RandomData.NextChoice<SearchEngineType>(
                SearchEngineType.AdCenter,
                SearchEngineType.Facebook,
                SearchEngineType.Google);
        }

        public TblFacebookCampaign NextFacebookCampaignEntity()
        {
            DateTime now = DateTime.Now;

            TblFacebookCampaign tblFacebookCampaign = new TblFacebookCampaign();
            tblFacebookCampaign.LocalStatus = RandomData.NextByte();
            tblFacebookCampaign.LocalState = RandomData.NextByte();
            tblFacebookCampaign.LastUpdateTime = RandomData.NextDateTime(new TimeSpan(1,0,0,0));
            tblFacebookCampaign.LocalParentId = (Int64)RandomData.NextUInt32();
            tblFacebookCampaign.LocalId = (Int64)RandomData.NextUInt32();
            tblFacebookCampaign.Id = (Int64)RandomData.NextUInt32();
            tblFacebookCampaign.ParentId = (Int64)RandomData.NextUInt32();
            tblFacebookCampaign.Name = RandomData.NextUnicodeWord(500);
            tblFacebookCampaign.Budget =(decimal)RandomData.NextDouble(0.02, 1);
            tblFacebookCampaign.BudgetType = RandomData.NextInt32();
            tblFacebookCampaign.StartTime = tblFacebookCampaign.LastUpdateTime + new TimeSpan(1, 0, 0);
            tblFacebookCampaign.StopTime = tblFacebookCampaign.StartTime + new TimeSpan(1, 0, 0);
            tblFacebookCampaign.Status = RandomData.NextInt32();
            tblFacebookCampaign.Version = (Int64)RandomData.NextInt16();
            return tblFacebookCampaign;
        }

        public TblFacebookAdGroup NextFacebookAdGroupEntity()
        {
            TblFacebookAdGroup tblFacebookAdGroup = new TblFacebookAdGroup();
            tblFacebookAdGroup.AccountId = (Int64)RandomData.NextUInt32();
            tblFacebookAdGroup.AdId = (Int64)RandomData.NextUInt32();
            tblFacebookAdGroup.BidType = RandomData.NextInt32();
            tblFacebookAdGroup.DisapproveReason = RandomData.NextUnicodeWord(500);
            tblFacebookAdGroup.EnableTracking = RandomData.NextBoolean();
            tblFacebookAdGroup.LastUpdateTime= RandomData.NextDateTime(new TimeSpan(1,0,0,0));
            tblFacebookAdGroup.LocalId = (Int64)RandomData.NextUInt32();
            tblFacebookAdGroup.LocalParentId = (Int64)RandomData.NextUInt32();
            tblFacebookAdGroup.LocalState = RandomData.NextByte();
            tblFacebookAdGroup.LocalStatus = RandomData.NextByte();
            tblFacebookAdGroup.MaxBid =1;
            tblFacebookAdGroup.Name = RandomData.NextUnicodeWord(500);
            tblFacebookAdGroup.OriginalStatus = RandomData.NextInt32();
            tblFacebookAdGroup.ParentId = (Int64)RandomData.NextUInt32();
            tblFacebookAdGroup.Status = RandomData.NextInt32();
            tblFacebookAdGroup.Version = (Int64)RandomData.NextInt16();
            return tblFacebookAdGroup;
        }

        public TblFacebookAccount NextFacebookAccountEntity()
        {
            TblFacebookAccount tblFacebookAccount = new TblFacebookAccount();
            tblFacebookAccount.Currency=RandomData.NextUnicodeWord(500);
            tblFacebookAccount.DailyAccountLimit = RandomData.NextInt32();
            tblFacebookAccount.Id = (Int64)RandomData.NextUInt32();
            tblFacebookAccount.LastDownloadDate = RandomData.NextDateTime(new TimeSpan(1,0,0,0));
            tblFacebookAccount.LastUpdateTime= RandomData.NextDateTime(new TimeSpan(1,0,0,0));
            tblFacebookAccount.LocalId = (Int64)RandomData.NextUInt32();
            tblFacebookAccount.LocalParentId = (Int64)RandomData.NextUInt32();
            tblFacebookAccount.LocalState = RandomData.NextByte();
            tblFacebookAccount.LocalStatus = RandomData.NextByte();
            tblFacebookAccount.Name = RandomData.NextUnicodeWord(500);
            tblFacebookAccount.ParentId = (Int64)RandomData.NextUInt32();
            tblFacebookAccount.Status = RandomData.NextInt32();
            tblFacebookAccount.TimeZone = RandomData.NextInt32();
            tblFacebookAccount.Version = (Int64)RandomData.NextInt16();
            return tblFacebookAccount;
        }

        public TblFacebookUser NextFacebookUserEntity()
        {
            TblFacebookUser tblFacebookUser = new TblFacebookUser();
            tblFacebookUser.Accesstoken = RandomData.NextUnicodeWord(500);
            tblFacebookUser.ApiKey = RandomData.NextUnicodeWord(500);
            tblFacebookUser.ApplicationId = RandomData.NextUnicodeWord(500);
            tblFacebookUser.ApplicationSecret=RandomData.NextUnicodeWord(500);
            tblFacebookUser.Email = RandomData.NextAsciiEmail(20);
            tblFacebookUser.Id = (Int64)RandomData.NextUInt32();
            tblFacebookUser.ImagePath = RandomData.NextEnglishWordLowercase(10);
            tblFacebookUser.ImageUrl = RandomData.NextAsciiUrl(20);
            tblFacebookUser.LastUpdateTime=RandomData.NextDateTime(new TimeSpan(1,0,0,0));
            tblFacebookUser.LocalId = (Int64)RandomData.NextUInt32();
            tblFacebookUser.LocalState = RandomData.NextByte();
            tblFacebookUser.LocalStatus = RandomData.NextByte();
            tblFacebookUser.Password = RandomData.NextUnicodeWord(20);
            tblFacebookUser.SessionKey = RandomData.NextUnicodeWord(20);
            tblFacebookUser.SessionSecret = RandomData.NextUnicodeWord(20);
            tblFacebookUser.UserName = RandomData.NextUnicodeWord(20);
            tblFacebookUser.Version = (Int64)RandomData.NextInt16();
            return tblFacebookUser;
        }

        public void InsertFacebookCampaignIntoDB(TblFacebookCampaign facebookCampaignEntity)
        {
            SEMObjects.TblFacebookCampaign.InsertOnSubmit(facebookCampaignEntity);
            SEMObjects.SubmitChanges();
        }

        public void RegisterCreatedFacebookCampaignEntityForCleanup(TblFacebookCampaign facebookCampaignEntity)
        {
            this.createdFacebookCampaignEntitiesForCleanup.Add(facebookCampaignEntity);
        }

        public void InsertFacebookAdGroupIntoDB(TblFacebookAdGroup facebookAdGroupEntity)
        {
            SEMObjects.TblFacebookAdGroup.InsertOnSubmit(facebookAdGroupEntity);
            SEMObjects.SubmitChanges();
        }

        public void RegisterCreatedFacebookAdGroupEntityForCleanup(TblFacebookAdGroup facebookAdGroupEntity)
        {
            this.createdFacebookAdGroupEntitiesForCleanup.Add(facebookAdGroupEntity);
        }

        public void InsertFacebookAccountIntoDB(TblFacebookAccount facebookAccountEntity)
        {
            SEMObjects.TblFacebookAccount.InsertOnSubmit(facebookAccountEntity);
            SEMObjects.SubmitChanges();
        }

        public void RegisterCreatedFacebookAccountEntityForCleanup(TblFacebookAccount facebookAccountEntity)
        {
            this.createdFacebookAccountEntitiesForCleanup.Add(facebookAccountEntity);
        }

        public void InsertFacebookUserIntoDB(TblFacebookUser facebookUserEntity)
        {
            SEMObjects.TblFacebookUser.InsertOnSubmit(facebookUserEntity);
            SEMObjects.SubmitChanges();
        }

        public void RegisterCreatedFacebookUserEntityForCleanup(TblFacebookUser facebookUserEntity)
        {
            this.createdFacebookUserEntitiesFroCleanup.Add(facebookUserEntity);
        }

        public void InsertSequenceNumberIntoDB(TblSequenceNumber sequenceNumberEntity)
        {
            SEMObjects.TblSequenceNumber.InsertOnSubmit(sequenceNumberEntity);
            SEMObjects.SubmitChanges();
        }

        public void RegisterCreatedSequenceNumberEntityForCleanup(TblSequenceNumber sequenceNumberEntity)
        {
            this.createdSequenceNumberEntitiesForCleanup.Add(sequenceNumberEntity);
        }

        private void onDBCleanup()
        {
            //clean up facebook adgroup
            SqlParameter facebookAdGroupSqlParameter = new SqlParameter("@LocalId", SqlDbType.BigInt);
            foreach(var entity in createdFacebookAdGroupEntitiesForCleanup)
            {
                facebookAdGroupSqlParameter.Value = entity.LocalId;
                SEMObjects.GetOpenSqlConnection().ExecuteNonQuery(
                    "delete from [SEMObjects].[dbo].[tblFacebookAdGroup] where [LocalId] = @LocalId",
                     facebookAdGroupSqlParameter);
            }
            //clean up facebook campaign
            SqlParameter facebookCampaignSqlParameter = new SqlParameter("@LocalId", SqlDbType.BigInt);
            foreach(var entity in createdFacebookCampaignEntitiesForCleanup)
            {
                facebookCampaignSqlParameter.Value = entity.LocalId;
                SEMObjects.GetOpenSqlConnection().ExecuteNonQuery(
                    "delete from [SEMObjects].[dbo].[tblFacebookCampaign] where [LocalId] = @LocalId",
                    facebookCampaignSqlParameter);
            }
            //clean up facebook account
            SqlParameter facebookAccountSqlParameter = new SqlParameter("@LocalId", SqlDbType.BigInt);
            foreach (var entity in createdFacebookAccountEntitiesForCleanup)
            {
                facebookAccountSqlParameter.Value = entity.LocalId;
                SEMObjects.GetOpenSqlConnection().ExecuteNonQuery(
                    "delete from [SEMObjects].[dbo].[tblFacebookAccount] where [LocalId] = @LocalId",
                     facebookAccountSqlParameter);
            }
            //clean up facebook user
            SqlParameter facebookUserSqlParameter = new SqlParameter("@LocalId", SqlDbType.BigInt);
            foreach (var entity in createdFacebookUserEntitiesFroCleanup)
            {
                facebookUserSqlParameter.Value = entity.LocalId;
                SEMObjects.GetOpenSqlConnection().ExecuteNonQuery(
                    "delete from [SEMObjects].[dbo].[tblFacebookUser] where [LocalId] = @LocalId",
                     facebookUserSqlParameter);
            }

            //cleanup all other entries
            SEMObjects.GetOpenSqlConnection().ExecuteNonQuery("delete from [SEMObjects].[dbo].[tblFacebookAdGroup]");
            SEMObjects.GetOpenSqlConnection().ExecuteNonQuery("delete from [SEMObjects].[dbo].[tblFacebookCampaign]");
            SEMObjects.GetOpenSqlConnection().ExecuteNonQuery("delete from [SEMObjects].[dbo].[tblFacebookAccount]");
            SEMObjects.GetOpenSqlConnection().ExecuteNonQuery("delete from [SEMObjects].[dbo].[tblFacebookUser]");
            SEMObjects.GetOpenSqlConnection().ExecuteNonQuery("delete from [SEMObjects].[dbo].[tblFacebookAdGroup]");
            SEMObjects.GetOpenSqlConnection().ExecuteNonQuery("delete from [SEMObjects].[dbo].[tblSequenceNumber]");
        }

        public IEnumerable<ProxySync.PreCommitResponse> TestPreCommitVersionCompare_FacebookCampaign(long differentBetweenHostingVersionAndLocalVersion, out ProxySync.ObjectState facebookCampaignObjectState)
        {
            facebookCampaignObjectState = NextObjectState();
            facebookCampaignObjectState.HostingId = -1;
            facebookCampaignObjectState.ObjectDetailType = (int)SEMObjectDetailType.FacebookCampaign;
            facebookCampaignObjectState.IsVersionCompare = true;
            facebookCampaignObjectState.EngineType = (int)SearchEngineType.Facebook;
            facebookCampaignObjectState.LocalId = SyncDataServiceHelper.GenerateLocalId(facebookCampaignObjectState);

            TblFacebookCampaign facebookCampaignEntity = NextFacebookCampaignEntity();
            facebookCampaignEntity.Id = facebookCampaignObjectState.ObjectId;
            facebookCampaignEntity.ParentId = facebookCampaignObjectState.ParentId;
            facebookCampaignEntity.LocalId = facebookCampaignObjectState.HostingId;

            facebookCampaignEntity.Version = facebookCampaignObjectState.LocalVersion + differentBetweenHostingVersionAndLocalVersion;

            RegisterCreatedFacebookCampaignEntityForCleanup(facebookCampaignEntity);
            InsertFacebookCampaignIntoDB(facebookCampaignEntity);

            ProxySync.PreCommitResponse[] responses = null;
            ProxySync.ObjectState facebookCampaignObejctStateForLambdaExpression = facebookCampaignObjectState;
            WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
            {
                responses = client.PreCommit(new ProxySync.ObjectState[] { facebookCampaignObejctStateForLambdaExpression });
            });
            return responses;
        }

        public IEnumerable<ProxySync.PreCommitResponse> TestPreCommitVersionCompare_FacebookAdGroup(long differentBetweenHostingVersionAndLocalVersion, out ProxySync.ObjectState facebookAdGroupObjectState)
        {
            facebookAdGroupObjectState = NextObjectState();
            facebookAdGroupObjectState.HostingId = -1;
            facebookAdGroupObjectState.ObjectDetailType = (int)SEMObjectDetailType.FacebookAdGroup;
            facebookAdGroupObjectState.IsVersionCompare = true;
            facebookAdGroupObjectState.EngineType = (int)SearchEngineType.Facebook;
            facebookAdGroupObjectState.LocalId = SyncDataServiceHelper.GenerateLocalId(facebookAdGroupObjectState);

            TblFacebookAdGroup facebookAdGroupEntity = NextFacebookAdGroupEntity();
            facebookAdGroupEntity.AdId = facebookAdGroupObjectState.ObjectId;
            facebookAdGroupEntity.ParentId = facebookAdGroupObjectState.ParentId;

            facebookAdGroupEntity.Version = facebookAdGroupObjectState.LocalVersion + differentBetweenHostingVersionAndLocalVersion;

            RegisterCreatedFacebookAdGroupEntityForCleanup(facebookAdGroupEntity);
            InsertFacebookAdGroupIntoDB(facebookAdGroupEntity);

            ProxySync.PreCommitResponse[] responses = null;
            ProxySync.ObjectState facebookAdGroupObejctStateForLambdaExpression = facebookAdGroupObjectState;
            WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
            {
                responses = client.PreCommit(new ProxySync.ObjectState[] { facebookAdGroupObejctStateForLambdaExpression });
            });
            return responses;
        }

        public IEnumerable<ProxySync.PreCommitResponse> TestPreCommitVersionCompare_FacebookAccount(long differentBetweenHostingVersionAndLocalVersion, out ProxySync.ObjectState facebookAccountObjectState)
        {
            facebookAccountObjectState = NextObjectState();
            facebookAccountObjectState.HostingId = -1;
            facebookAccountObjectState.ObjectDetailType = (int)SEMObjectDetailType.FacebookAccount;
            facebookAccountObjectState.IsVersionCompare = true;
            facebookAccountObjectState.EngineType = (int)SearchEngineType.Facebook;
            facebookAccountObjectState.LocalId = SyncDataServiceHelper.GenerateLocalId(facebookAccountObjectState);

            TblFacebookAccount facebookAccountEntity = NextFacebookAccountEntity();
            facebookAccountEntity.Id= facebookAccountObjectState.ObjectId;
            facebookAccountEntity.ParentId = facebookAccountObjectState.ParentId;

            facebookAccountEntity.Version = facebookAccountObjectState.LocalVersion + differentBetweenHostingVersionAndLocalVersion;

            RegisterCreatedFacebookAccountEntityForCleanup(facebookAccountEntity);
            InsertFacebookAccountIntoDB(facebookAccountEntity);

            ProxySync.PreCommitResponse[] responses = null;
            ProxySync.ObjectState facebookAccountObejctStateForLambdaExpression = facebookAccountObjectState;
            WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
            {
                responses = client.PreCommit(new ProxySync.ObjectState[] { facebookAccountObejctStateForLambdaExpression });
            });
            return responses;
        }

        public IEnumerable<ProxySync.PreCommitResponse> TestPreCommitVersionCompare_FacebookUser(long differentBetweenHostingVersionAndLocalVersion, out ProxySync.ObjectState facebookUserObjectState)
        {
            facebookUserObjectState = NextObjectState();
            facebookUserObjectState.HostingId = -1;
            facebookUserObjectState.ObjectDetailType = (int)SEMObjectDetailType.FacebookUser;
            facebookUserObjectState.IsVersionCompare = true;
            facebookUserObjectState.EngineType = (int)SearchEngineType.Facebook;
            facebookUserObjectState.LocalId = SyncDataServiceHelper.GenerateLocalId(facebookUserObjectState);

            TblFacebookUser facebookUserEntity = NextFacebookUserEntity();
            facebookUserEntity.Id = facebookUserObjectState.ObjectId;

            facebookUserEntity.Version = facebookUserObjectState.LocalVersion + differentBetweenHostingVersionAndLocalVersion;

            RegisterCreatedFacebookUserEntityForCleanup(facebookUserEntity);
            InsertFacebookUserIntoDB(facebookUserEntity);

            ProxySync.PreCommitResponse[] responses = null;
            ProxySync.ObjectState facebookUserObejctStateForLambdaExpression = facebookUserObjectState;
            WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
            {
                responses = client.PreCommit(new ProxySync.ObjectState[] { facebookUserObejctStateForLambdaExpression });
            });
            return responses;
        }

        public ProxySync.PreCommitResponse DoPreCommitInitialize_FacebookCampaign(out ProxySync.ObjectState facebookCampaignObjectState)
        {
            return TestPreCommitVersionCompare_FacebookCampaign(0, out facebookCampaignObjectState).First();
        }

        public ProxySync.SyncData NextSyncData(string JsonSerializedData)
        {
            ProxySync.SyncData syncData = NextSyncData();
            syncData.Data = JsonSerializedData;
            return syncData;
        }

        public ProxySync.SyncData NextSyncData()
        {
            ProxySync.SyncData syncData = new ProxySync.SyncData();

            syncData.EngineType = (int)NextSearchEngineType();
            syncData.GrainId = RandomData.NextGuid();
            syncData.HostingCampaignId = (Int64)RandomData.NextUInt32();
            syncData.HostingId = (Int64)RandomData.NextUInt32();
            syncData.LocalId = RandomData.NextGuid();
            syncData.ObjectDetailType = (int)NextSEMObjectDetailType();
            syncData.ObjectId = (Int64)RandomData.NextUInt32();
            syncData.Operation = (int)NextOperationType();
            syncData.ParentId = (Int64)RandomData.NextUInt32();
            syncData.TimeStamp = RandomData.NextDateTime(new TimeSpan(1, 0, 0, 0)).Ticks;
            syncData.TimeTick = syncData.TimeStamp;

            return syncData;
        }

        public ProxyDTO.SEMBaseDTO NextSEMBaseDTO()
        {
            ProxyDTO.SEMBaseDTO semBaseDTO = NextFacebookCampaignDTO() as ProxyDTO.SEMBaseDTO;
            return semBaseDTO;
        }

        public ProxyDTO.SyncState NextSyncState()
        {
            return RandomData.NextChoice<ProxyDTO.SyncState>(
                ProxyDTO.SyncState.Created,
                ProxyDTO.SyncState.Deleted,
                ProxyDTO.SyncState.Updated);
        }

        public ProxyDTO.Facebook.FacebookCampaignDTO NextFacebookCampaignDTO()
        {
            ProxyDTO.Facebook.FacebookCampaignDTO facebookCampaignDTO = new ProxyDTO.Facebook.FacebookCampaignDTO();
            facebookCampaignDTO.Budget = (decimal)RandomData.NextDouble(0.02, 1);
            facebookCampaignDTO.BudgetType = (int)NextFacebookBudgetType();
            facebookCampaignDTO.LastUpdateTime = RandomData.NextDateTime(new TimeSpan(1, 0, 0, 0));
            facebookCampaignDTO.Name = RandomData.NextUnicodeWord(500);
            facebookCampaignDTO.StartTime = facebookCampaignDTO.LastUpdateTime + new TimeSpan(1, 0, 0);
            facebookCampaignDTO.StopTime = facebookCampaignDTO.StartTime + new TimeSpan(1, 0, 0);
            facebookCampaignDTO.Status = RandomData.NextInt32();

            //for base
            facebookCampaignDTO.AccountId = (Int64)RandomData.NextUInt32();

            //for root 
            facebookCampaignDTO.EngineType = (int)NextSearchEngineType();
            facebookCampaignDTO.Id = (Int64)RandomData.NextUInt32();
            facebookCampaignDTO.LocalId = (Int64)RandomData.NextUInt32();
            facebookCampaignDTO.LocalParentId = (Int64)RandomData.NextUInt32();
            facebookCampaignDTO.LocalState = RandomData.NextByte();
            facebookCampaignDTO.LocalStatus = RandomData.NextByte();
            facebookCampaignDTO.SynState = NextSyncState();
            facebookCampaignDTO.Version = (Int64)RandomData.NextUInt32();
            facebookCampaignDTO.ParentId = (Int64)RandomData.NextUInt32();

            return facebookCampaignDTO;
        }

        public BudgetType NextFacebookBudgetType()
        {
            return RandomData.NextChoice<BudgetType>(
                BudgetType.Daily,
                BudgetType.Lifetime);
        }

        public TblSequenceNumber NextSequenceNumberEntity()
        {
            return new TblSequenceNumber
            {
                Id = (Int64)RandomData.NextUInt32(),
                EngineType = (int)NextSearchEngineType(),
            };
        }

        public bool DoesExistSequenceNumberEntityInDB(TblSequenceNumber sequenceNumberEntity)
        {
            var result = from entity in SEMObjects.TblSequenceNumber where entity.Id == sequenceNumberEntity.Id && entity.EngineType == sequenceNumberEntity.EngineType select entity;
            return result.ToArray().Length == 1;
        }

        public bool DoesExistFacebookCampaignEntityInDB(long id, out TblFacebookCampaign facebookCampaignEntity)
        {
            var result = from entity in SEMObjects.TblFacebookCampaign where entity.Id == id select entity;
            if (result.ToArray().Length != 1)
            {
                facebookCampaignEntity = null;
                return false;
            }
            else
            {
                facebookCampaignEntity = result.ToArray()[0];
                return true;
            }
        }

        public void CompareFacebookCampaignDTOAndEntityObject(ProxyDTO.Facebook.FacebookCampaignDTO facebookCampaignDTO, TblFacebookCampaign facebookCampaignEntity, long localId, long localParentId, int version)
        {
            Assert.AreEqual(facebookCampaignDTO.Id, facebookCampaignEntity.Id, "The campaign id should be equal!!");
            Assert.AreEqual(localId, facebookCampaignEntity.LocalId, "The campaign local id should be equal!!");
            Assert.AreEqual(localParentId, facebookCampaignEntity.LocalParentId, "The campaign local parent id should be equal!!");
            Assert.AreEqual(facebookCampaignDTO.ParentId, facebookCampaignEntity.ParentId, "The campaign parent id should be equal!!");
            Assert.AreEqual(facebookCampaignDTO.LocalStatus, facebookCampaignEntity.LocalStatus, "The campaign local status should be equal!!");
            Assert.AreEqual(facebookCampaignDTO.LocalState, facebookCampaignEntity.LocalState, "The campaign local state should be equal!!");
            Assert.AreEqual(facebookCampaignDTO.Name, facebookCampaignEntity.Name, "The campaign name should be equal!!");
            Assert.AreEqual(facebookCampaignDTO.Status, facebookCampaignEntity.Status, "The campaign status should be equal!!");
            Assert.AreEqual(version, facebookCampaignEntity.Version, "The campaign version should be equal!!");
        }
    }
}

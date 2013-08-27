using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdSage.Concert.SEMObjects;
using HostingService.Test.Databases.SEMObjects;
using AdSage.Concert.Test.Framework;
using HostingService.Test.Framework.SyncDataService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxySync = AdSage.Concert.Hosting.Application.DTO.Sync;
using ProxyDTO = AdSage.Concert.Hosting.Application.DTO;

namespace HostingService.Test.SyncDataService
{
    [TestClass]
    public class SyncDataServiceBvt : SyncDataServiceTestBase
    {
        #region PreCommit

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verfiy that precommit interface can return correct result when ObjectState.LocalVersion is equal to Hosting Version")]
        public void TestPreCommitVersionEqualCompare_FacebookCampaign()
        {
            ProxySync.ObjectState facebookCampaignObjectState;
            ProxySync.PreCommitResponse[] responses = SyncDataServiceHelper.TestPreCommitVersionCompare_FacebookCampaign(0, out facebookCampaignObjectState).ToArray();

            Assert.IsNotNull(responses, "The PreCommitResponse object should be NOT NULL!!");
            Assert.AreEqual(1, responses.Length, "The length of PreCommitResponse array should be equal 1!!");
            Assert.AreEqual(facebookCampaignObjectState.LocalId, responses[0].GrainId, "The inputed objectstate object's LocalId should be equal to PreCommitResponse's GrainId returned from Server!!");
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verfiy that precommit interface return NULL when ObjectState.LocalVersion is less than Hosting Version")]
        public void TestPreCommitVersionLessThanCompare_FacebookCampaign()
        {
            ProxySync.ObjectState facebookCampaignObjectState;
            ProxySync.PreCommitResponse[] responses = SyncDataServiceHelper.TestPreCommitVersionCompare_FacebookCampaign(1, out facebookCampaignObjectState).ToArray();

            Assert.AreEqual(0, responses.Length, "The length of PreCommitResponse object array should be 0!!");
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verfiy that precommit interface return NULL when ObjectState.LocalVersion is greater than Hosting Version")]
        public void TestPreCommitVersionGreatThanCompare_FacebookCampaign()
        {
            ProxySync.ObjectState facebookCampaignObjectState;
            ProxySync.PreCommitResponse[] responses = SyncDataServiceHelper.TestPreCommitVersionCompare_FacebookCampaign(-1, out facebookCampaignObjectState).ToArray();

            Assert.AreEqual(0, responses.Length, "The length of PreCommitResponse object array should be 0!!");
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verify that just one client can get correct result from precommit interface, when there are two clients do precommit with one same object")]
        public void TestMultipleClientsDoPreCommit_FacebookCampaign()
        {
            ProxySync.ObjectState facebookCampaignObjectState = SyncDataServiceHelper.NextObjectState();
            facebookCampaignObjectState.HostingId = -1;
            facebookCampaignObjectState.ObjectDetailType = (int)SEMObjectDetailType.FacebookCampaign;
            facebookCampaignObjectState.IsVersionCompare = true;
            facebookCampaignObjectState.EngineType = (int)SearchEngineType.Facebook;
            facebookCampaignObjectState.LocalId = SyncDataServiceHelper.GenerateLocalId(facebookCampaignObjectState);

            TblFacebookCampaign facebookCampaignEntity = SyncDataServiceHelper.NextFacebookCampaignEntity();
            facebookCampaignEntity.Id = facebookCampaignObjectState.ObjectId;
            facebookCampaignEntity.ParentId = facebookCampaignObjectState.ParentId;

            facebookCampaignEntity.Version = facebookCampaignObjectState.LocalVersion;

            SyncDataServiceHelper.RegisterCreatedFacebookCampaignEntityForCleanup(facebookCampaignEntity);
            SyncDataServiceHelper.InsertFacebookCampaignIntoDB(facebookCampaignEntity);

            var taskA = Task.Factory.StartNew<ProxySync.PreCommitResponse[]>(() =>
                {
                    ProxySync.PreCommitResponse[] responsesA = null;
                    ProxySync.ObjectState facebookCampaignObejctStateForLambdaExpressionA = facebookCampaignObjectState;
                    WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
                    {
                        responsesA = client.PreCommit(new ProxySync.ObjectState[] { facebookCampaignObejctStateForLambdaExpressionA });
                    });
                    return responsesA;
                });

            var taskB = Task.Factory.StartNew<ProxySync.PreCommitResponse[]>(() =>
                {
                    ProxySync.PreCommitResponse[] responsesB = null;
                    ProxySync.ObjectState facebookCampaignObejctStateForLambdaExpressionB = facebookCampaignObjectState;
                    WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
                    {
                        responsesB = client.PreCommit(new ProxySync.ObjectState[] { facebookCampaignObejctStateForLambdaExpressionB });
                    });
                    return responsesB;
                });
            Task.WaitAll(taskA, taskB);

            //Verify two task results
            if (taskA.Result == null && taskB.Result == null)
            {
                Assert.Fail("The two clients all return NULL!!");
            }
            if (taskA.Result != null && taskB.Result != null)
            {
                Assert.Fail("The two clients all don't return NULL!!");
            }
            if (taskA.Result != null && taskB.Result == null)
            {
                Assert.AreEqual(1, taskA.Result.Length, "The length of PreCommitResponse array should be equal 1!!");
                Assert.AreEqual(facebookCampaignObjectState.LocalId, taskA.Result[0].GrainId, "The inputed objectstate object's LocalId should be equal to PreCommitResponse's GrainId returned from Server!!");
            }
            if (taskA.Result == null && taskB.Result != null)
            {
                Assert.AreEqual(1, taskB.Result.Length, "The length of PreCommitResponse array should be equal 1!!");
                Assert.AreEqual(facebookCampaignObjectState.LocalId, taskB.Result[0].GrainId, "The inputed objectstate object's LocalId should be equal to PreCommitResponse's GrainId returned from Server!!");
            }
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verfiy that precommit interface can return correct result when ObjectState.LocalVersion is equal to Hosting Version")]
        public void TestPreCommitVersionEqualCompare_FacebookAdGroup()
        {
            ProxySync.ObjectState facebookAdGroupObjectState;
            ProxySync.PreCommitResponse[] responses = SyncDataServiceHelper.TestPreCommitVersionCompare_FacebookAdGroup(0, out facebookAdGroupObjectState).ToArray();

            Assert.IsNotNull(responses, "The PreCommitResponse object should be NOT NULL!!");
            Assert.AreEqual(1, responses.Length, "The length of PreCommitResponse array should be equal 1!!");
            Assert.AreEqual(facebookAdGroupObjectState.LocalId, responses[0].GrainId, "The inputed objectstate object's LocalId should be equal to PreCommitResponse's GrainId returned from Server!!");
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verfiy that precommit interface return NULL when ObjectState.LocalVersion is less than Hosting Version")]
        public void TestPreCommitVersionLessThanCompare_FacebookAdGroup()
        {
            ProxySync.ObjectState facebookAdGroupObjectState;
            ProxySync.PreCommitResponse[] responses = SyncDataServiceHelper.TestPreCommitVersionCompare_FacebookAdGroup(1, out facebookAdGroupObjectState).ToArray();

            Assert.AreEqual(0, responses.Length, "The length of PreCommitResponse object array should be 0!!");
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verfiy that precommit interface return NULL when ObjectState.LocalVersion is greater than Hosting Version")]
        public void TestPreCommitVersionGreatThanCompare_FacebookAdGroup()
        {
            ProxySync.ObjectState facebookAdGroupObjectState;
            ProxySync.PreCommitResponse[] responses = SyncDataServiceHelper.TestPreCommitVersionCompare_FacebookAdGroup(-1, out facebookAdGroupObjectState).ToArray();

            Assert.AreEqual(0, responses.Length, "The length of PreCommitResponse object array should be 0!!");
        }

        #endregion

        #region Commit

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verify that the correct response object can be returned and the content of db table [tblSequenceNumber] and [tblFacebookCampaign] are correct, when commit insert one facebook campaign")]
        public void TestCommitOneSyncDataObject_FacebookCampaign()
        {
            //Initialize by precommit
            ProxySync.ObjectState facebookCampaignObjectState;
            ProxySync.PreCommitResponse precommitResponse = SyncDataServiceHelper.DoPreCommitInitialize_FacebookCampaign(out facebookCampaignObjectState);
            Assert.IsNotNull(precommitResponse, "The PreCommitResponse object should be NOT NULL!!");

            //Insert Facebook Account entity
            TblFacebookAccount facebookAccountEntity = SyncDataServiceHelper.NextFacebookAccountEntity();
            facebookAccountEntity.Id = facebookCampaignObjectState.ParentId;
            SyncDataServiceHelper.RegisterCreatedFacebookAccountEntityForCleanup(facebookAccountEntity);
            SyncDataServiceHelper.InsertFacebookAccountIntoDB(facebookAccountEntity);

            //Generate randomized syncdata object
            ProxyDTO.Facebook.FacebookCampaignDTO facebookCampaignDTO = SyncDataServiceHelper.NextFacebookCampaignDTO();
            facebookCampaignDTO.ParentId = facebookAccountEntity.Id;

            //string JsonSerializedFacebookCampaignDTO = ServiceStack.Text.JsonSerializer.SerializeToString(facebookCampaignDTO, typeof(ProxyDTO.Facebook.FacebookCampaignDTO));
            ProxySync.SyncData facebookCampaignSyncData = SyncDataServiceHelper.NextSyncData();

            //Setting property of syncdata object
            facebookCampaignSyncData.ObjectDTO = facebookCampaignDTO;
            facebookCampaignSyncData.ObjectId = facebookCampaignObjectState.ObjectId;
            facebookCampaignSyncData.ParentId = facebookCampaignObjectState.ParentId;
            facebookCampaignSyncData.LocalId = precommitResponse.GrainId;
            facebookCampaignSyncData.EngineType = (int)SearchEngineType.Facebook;
            facebookCampaignSyncData.ObjectDetailType = (int)SEMObjectDetailType.FacebookCampaign;
            facebookCampaignSyncData.Operation = 0;
            facebookCampaignSyncData.TimeStamp = precommitResponse.PreCommitToken;
            facebookCampaignSyncData.GrainId = facebookCampaignSyncData.LocalId;
            facebookCampaignSyncData.HostingCampaignId = -1;
            facebookCampaignSyncData.HostingId = -1;
            facebookCampaignSyncData.HostingParentId = -1;

            //Insert sequenceNumber into db
            TblSequenceNumber sequenceNumberEntity = SyncDataServiceHelper.NextSequenceNumberEntity();
            sequenceNumberEntity.EngineType = (int)facebookCampaignSyncData.EngineType;
            SyncDataServiceHelper.RegisterCreatedSequenceNumberEntityForCleanup(sequenceNumberEntity);
            SyncDataServiceHelper.InsertSequenceNumberIntoDB(sequenceNumberEntity);

            ProxySync.CommitResponse[] response = null;
            WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
                {
                    response = client.Commit(new ProxySync.SyncData[] { facebookCampaignSyncData });
                });
            Assert.IsNotNull(response, "The response object should not be NULL!!");
            Assert.AreEqual(sequenceNumberEntity.Id + 1, response[0].HostingId, "The response object's hosting id should be equal to [tblSequenceNumber].[Id]+1");
            Assert.IsTrue(SyncDataServiceHelper.DoesExistSequenceNumberEntityInDB(
                new TblSequenceNumber
                {
                    Id = sequenceNumberEntity.Id + 1,
                    EngineType = sequenceNumberEntity.EngineType
                }));

            TblFacebookCampaign actualFacebookCampaignEntity;
            Assert.IsTrue(SyncDataServiceHelper.DoesExistFacebookCampaignEntityInDB(facebookCampaignDTO.Id, out actualFacebookCampaignEntity), "There should be one facebook campaign enty in db!!");
            SyncDataServiceHelper.CompareFacebookCampaignDTOAndEntityObject(facebookCampaignDTO, actualFacebookCampaignEntity, response[0].HostingId, response[0].HostingParentId, 1);
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verify that just one client can get correct result from commit interface, when there are two clients do commit with one same object")]
        public void TestMultipleClientsDoCommit_FacebookCampaign()
        {
            //Initialize by precommit
            ProxySync.ObjectState facebookCampaignObjectState;
            ProxySync.PreCommitResponse precommitResponse = SyncDataServiceHelper.DoPreCommitInitialize_FacebookCampaign(out facebookCampaignObjectState);
            Assert.IsNotNull(precommitResponse, "The PreCommitResponse object should be NOT NULL!!");

            //Insert Facebook Account entity
            TblFacebookAccount facebookAccountEntity = SyncDataServiceHelper.NextFacebookAccountEntity();
            facebookAccountEntity.Id = facebookCampaignObjectState.ParentId;
            SyncDataServiceHelper.RegisterCreatedFacebookAccountEntityForCleanup(facebookAccountEntity);
            SyncDataServiceHelper.InsertFacebookAccountIntoDB(facebookAccountEntity);

            //Generate randomized syncdata object
            ProxyDTO.Facebook.FacebookCampaignDTO facebookCampaignDTO = SyncDataServiceHelper.NextFacebookCampaignDTO();
            facebookCampaignDTO.ParentId = facebookAccountEntity.Id;

            //string JsonSerializedFacebookCampaignDTO = ServiceStack.Text.JsonSerializer.SerializeToString(facebookCampaignDTO, typeof(ProxyDTO.Facebook.FacebookCampaignDTO));
            ProxySync.SyncData facebookCampaignSyncData = SyncDataServiceHelper.NextSyncData();

            //Setting property of syncdata object
            facebookCampaignSyncData.ObjectDTO = facebookCampaignDTO;
            facebookCampaignSyncData.ObjectId = facebookCampaignObjectState.ObjectId;
            facebookCampaignSyncData.ParentId = facebookCampaignObjectState.ParentId;
            facebookCampaignSyncData.LocalId = precommitResponse.GrainId;
            facebookCampaignSyncData.EngineType = (int)SearchEngineType.Facebook;
            facebookCampaignSyncData.ObjectDetailType = (int)SEMObjectDetailType.FacebookCampaign;
            facebookCampaignSyncData.Operation = 0;
            facebookCampaignSyncData.TimeStamp = precommitResponse.PreCommitToken;
            facebookCampaignSyncData.GrainId = facebookCampaignSyncData.LocalId;
            facebookCampaignSyncData.HostingCampaignId = -1;
            facebookCampaignSyncData.HostingId = -1;
            facebookCampaignSyncData.HostingParentId = -1;

            //Insert sequenceNumber into db
            TblSequenceNumber sequenceNumberEntity = SyncDataServiceHelper.NextSequenceNumberEntity();
            sequenceNumberEntity.EngineType = (int)facebookCampaignSyncData.EngineType;
            SyncDataServiceHelper.RegisterCreatedSequenceNumberEntityForCleanup(sequenceNumberEntity);
            SyncDataServiceHelper.InsertSequenceNumberIntoDB(sequenceNumberEntity);

            var taskA = Task.Factory.StartNew<ProxySync.CommitResponse[]>(() =>
                {
                    ProxySync.CommitResponse[] responseA = null;
                    WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
                    {
                        responseA = client.Commit(new ProxySync.SyncData[] { facebookCampaignSyncData });
                    });
                    return responseA;
                });

            var taskB = Task.Factory.StartNew<ProxySync.CommitResponse[]>(() =>
            {
                ProxySync.CommitResponse[] responseB = null;
                WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
                {
                    responseB = client.Commit(new ProxySync.SyncData[] { facebookCampaignSyncData });
                });
                return responseB;
            });
            Task.WaitAll(taskA, taskB);

            //Verify two task results
            if (taskA.Result == null && taskB.Result == null)
            {
                Assert.Fail("The two clients all return NULL!!");
            }
            if (taskA.Result != null && taskB.Result != null)
            {
                Assert.Fail("The two clients all don't return NULL!!");
            }
            if (taskA.Result != null && taskB.Result == null)
            {
                Assert.IsNotNull(taskA.Result, "The response object should not be NULL!!");
                Assert.AreEqual(sequenceNumberEntity.Id + 1, taskA.Result[0].HostingId, "The response object's hosting id should be equal to [tblSequenceNumber].[Id]+1");
                Assert.IsTrue(SyncDataServiceHelper.DoesExistSequenceNumberEntityInDB(
                    new TblSequenceNumber
                    {
                        Id = sequenceNumberEntity.Id + 1,
                        EngineType = sequenceNumberEntity.EngineType
                    }));

                TblFacebookCampaign actualFacebookCampaignEntity;
                Assert.IsTrue(SyncDataServiceHelper.DoesExistFacebookCampaignEntityInDB(facebookCampaignDTO.Id, out actualFacebookCampaignEntity), "There should be one facebook campaign enty in db!!");
                SyncDataServiceHelper.CompareFacebookCampaignDTOAndEntityObject(facebookCampaignDTO, actualFacebookCampaignEntity, taskA.Result[0].HostingId, taskA.Result[0].HostingParentId, 1);
            }
            if (taskA.Result == null && taskB.Result != null)
            {
                Assert.IsNotNull(taskB.Result, "The response object should not be NULL!!");
                Assert.AreEqual(sequenceNumberEntity.Id + 1, taskB.Result[0].HostingId, "The response object's hosting id should be equal to [tblSequenceNumber].[Id]+1");
                Assert.IsTrue(SyncDataServiceHelper.DoesExistSequenceNumberEntityInDB(
                    new TblSequenceNumber
                    {
                        Id = sequenceNumberEntity.Id + 1,
                        EngineType = sequenceNumberEntity.EngineType
                    }));

                TblFacebookCampaign actualFacebookCampaignEntity;
                Assert.IsTrue(SyncDataServiceHelper.DoesExistFacebookCampaignEntityInDB(facebookCampaignDTO.Id, out actualFacebookCampaignEntity), "There should be one facebook campaign enty in db!!");
                SyncDataServiceHelper.CompareFacebookCampaignDTOAndEntityObject(facebookCampaignDTO, actualFacebookCampaignEntity, taskB.Result[0].HostingId, taskB.Result[0].HostingParentId, 1);
            }
        }

        #endregion

        #region NewDataCommit

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verify that the correct response object can be returned and the content of db table [tblSequenceNumber] and [tblFacebookCampaign] are correct, when newdatacommit insert one facebook campaign")]
        public void TestNewDataCommitOneSyncDataObject_FacebookCampaign()
        {
            //Initialize by precommit
            ProxySync.ObjectState facebookCampaignObjectState;
            ProxySync.PreCommitResponse precommitResponse = SyncDataServiceHelper.DoPreCommitInitialize_FacebookCampaign(out facebookCampaignObjectState);
            Assert.IsNotNull(precommitResponse, "The PreCommitResponse object should be NOT NULL!!");

            //Insert Facebook Account entity
            TblFacebookAccount facebookAccountEntity = SyncDataServiceHelper.NextFacebookAccountEntity();
            facebookAccountEntity.Id = facebookCampaignObjectState.ParentId;
            SyncDataServiceHelper.RegisterCreatedFacebookAccountEntityForCleanup(facebookAccountEntity);
            SyncDataServiceHelper.InsertFacebookAccountIntoDB(facebookAccountEntity);

            //Generate randomized syncdata object
            ProxyDTO.Facebook.FacebookCampaignDTO facebookCampaignDTO = SyncDataServiceHelper.NextFacebookCampaignDTO();
            facebookCampaignDTO.ParentId = facebookAccountEntity.Id;

            //string JsonSerializedFacebookCampaignDTO = ServiceStack.Text.JsonSerializer.SerializeToString(facebookCampaignDTO, typeof(ProxyDTO.Facebook.FacebookCampaignDTO));
            ProxySync.SyncData facebookCampaignSyncData = SyncDataServiceHelper.NextSyncData();

            //Setting property of syncdata object
            facebookCampaignSyncData.ObjectDTO = facebookCampaignDTO;
            facebookCampaignSyncData.ObjectId = facebookCampaignObjectState.ObjectId;
            facebookCampaignSyncData.ParentId = facebookCampaignObjectState.ParentId;
            facebookCampaignSyncData.LocalId = precommitResponse.GrainId;
            facebookCampaignSyncData.EngineType = (int)SearchEngineType.Facebook;
            facebookCampaignSyncData.ObjectDetailType = (int)SEMObjectDetailType.FacebookCampaign;
            facebookCampaignSyncData.Operation = 0;
            facebookCampaignSyncData.TimeStamp = precommitResponse.PreCommitToken;
            facebookCampaignSyncData.GrainId = facebookCampaignSyncData.LocalId;
            facebookCampaignSyncData.HostingCampaignId = -1;
            facebookCampaignSyncData.HostingId = -1;
            facebookCampaignSyncData.HostingParentId = -1;

            //Insert sequenceNumber into db
            TblSequenceNumber sequenceNumberEntity = SyncDataServiceHelper.NextSequenceNumberEntity();
            sequenceNumberEntity.EngineType = (int)facebookCampaignSyncData.EngineType;
            SyncDataServiceHelper.RegisterCreatedSequenceNumberEntityForCleanup(sequenceNumberEntity);
            SyncDataServiceHelper.InsertSequenceNumberIntoDB(sequenceNumberEntity);

            ProxySync.CommitResponse[] response = null;
            WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
                {
                    response = client.NewDataCommit(new ProxySync.SyncData[] { facebookCampaignSyncData });
                });
            Assert.IsNotNull(response, "The response object should not be NULL!!");
            Assert.AreEqual(sequenceNumberEntity.Id + 1, response[0].HostingId, "The response object's hosting id should be equal to [tblSequenceNumber].[Id]+1");
            Assert.IsTrue(SyncDataServiceHelper.DoesExistSequenceNumberEntityInDB(
                new TblSequenceNumber
                {
                    Id = sequenceNumberEntity.Id + 1,
                    EngineType = sequenceNumberEntity.EngineType
                }));

            TblFacebookCampaign actualFacebookCampaignEntity;
            Assert.IsTrue(SyncDataServiceHelper.DoesExistFacebookCampaignEntityInDB(facebookCampaignDTO.Id, out actualFacebookCampaignEntity), "There should be one facebook campaign enty in db!!");
            SyncDataServiceHelper.CompareFacebookCampaignDTOAndEntityObject(facebookCampaignDTO, actualFacebookCampaignEntity, response[0].HostingId, response[0].HostingParentId, 1);
        }

        #endregion

        #region Update

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verfiy that update interface can return correct result when ObjectState.LocalVersion is equal to server version")]
        public void TestUpdateOneObjectStateWithClientVersionEqualToServerVersion_FacebookCampaign()
        {
            //Initialize by precommit
            ProxySync.ObjectState facebookCampaignObjectState;
            ProxySync.PreCommitResponse precommitResponse = SyncDataServiceHelper.DoPreCommitInitialize_FacebookCampaign(out facebookCampaignObjectState);
            Assert.IsNotNull(precommitResponse, "The PreCommitResponse object should be NOT NULL!!");

            //Insert Facebook Account entity
            TblFacebookAccount facebookAccountEntity = SyncDataServiceHelper.NextFacebookAccountEntity();
            facebookAccountEntity.Id = facebookCampaignObjectState.ParentId;
            SyncDataServiceHelper.RegisterCreatedFacebookAccountEntityForCleanup(facebookAccountEntity);
            SyncDataServiceHelper.InsertFacebookAccountIntoDB(facebookAccountEntity);

            TblFacebookCampaign facebookCampaignEntity;
            SyncDataServiceHelper.DoesExistFacebookCampaignEntityInDB(facebookCampaignObjectState.ObjectId, out facebookCampaignEntity);

            var inputDictionary = new Dictionary<long, ProxySync.ObjectState[]>();
            inputDictionary.Add(facebookCampaignEntity.LocalParentId, new ProxySync.ObjectState[] { facebookCampaignObjectState });

            ProxyDTO.SEMBaseDTO[] response = null;
            WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
                {
                    response = client.Update(inputDictionary);
                });
            Assert.AreEqual(0, response.Length, "The length of response  should be 0!!");
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verfiy that update interface can return correct result when ObjectState.LocalVersion is less than server version")]
        public void TestUpdateOneObjectStateWithClientVersionLessThanServerVersion_FacebookCampaign()
        {
            //Initialize by precommit
            ProxySync.ObjectState facebookCampaignObjectState;
            ProxySync.PreCommitResponse precommitResponse = SyncDataServiceHelper.DoPreCommitInitialize_FacebookCampaign(out facebookCampaignObjectState);
            Assert.IsNotNull(precommitResponse, "The PreCommitResponse object should be NOT NULL!!");

            //Insert Facebook Account entity
            TblFacebookAccount facebookAccountEntity = SyncDataServiceHelper.NextFacebookAccountEntity();
            facebookAccountEntity.Id = facebookCampaignObjectState.ParentId;
            SyncDataServiceHelper.RegisterCreatedFacebookAccountEntityForCleanup(facebookAccountEntity);
            SyncDataServiceHelper.InsertFacebookAccountIntoDB(facebookAccountEntity);

            TblFacebookCampaign facebookCampaignEntity;
            SyncDataServiceHelper.DoesExistFacebookCampaignEntityInDB(facebookCampaignObjectState.ObjectId, out facebookCampaignEntity);

            var inputDictionary = new Dictionary<long, ProxySync.ObjectState[]>();
            facebookCampaignObjectState.LocalVersion--;
            inputDictionary.Add(facebookCampaignEntity.LocalParentId, new ProxySync.ObjectState[] { facebookCampaignObjectState });

            ProxyDTO.SEMBaseDTO[] response = null;
            WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
                {
                    response = client.Update(inputDictionary);
                });
            Assert.AreEqual(1, response.Length, "The length of response should be 1!!");
            Assert.AreEqual(facebookCampaignObjectState.LocalVersion + 1, response[0].Version, "The response version should be equal to localversion+1!!");
        }

        [TestMethod]
        [Priority(0)]
        [Owner("LiuPeng")]
        [Description("Verfiy that update interface can return correct result when ObjectState.LocalVersion is greater than server version")]
        public void TestUpdateOneObjectStateWithClientVersionGreatThanServerVersion_FacebookCampaign()
        {
            //Initialize by precommit
            ProxySync.ObjectState facebookCampaignObjectState;
            ProxySync.PreCommitResponse precommitResponse = SyncDataServiceHelper.DoPreCommitInitialize_FacebookCampaign(out facebookCampaignObjectState);
            Assert.IsNotNull(precommitResponse, "The PreCommitResponse object should be NOT NULL!!");

            //Insert Facebook Account entity
            TblFacebookAccount facebookAccountEntity = SyncDataServiceHelper.NextFacebookAccountEntity();
            facebookAccountEntity.Id = facebookCampaignObjectState.ParentId;
            SyncDataServiceHelper.RegisterCreatedFacebookAccountEntityForCleanup(facebookAccountEntity);
            SyncDataServiceHelper.InsertFacebookAccountIntoDB(facebookAccountEntity);

            TblFacebookCampaign facebookCampaignEntity;
            SyncDataServiceHelper.DoesExistFacebookCampaignEntityInDB(facebookCampaignObjectState.ObjectId, out facebookCampaignEntity);

            var inputDictionary = new Dictionary<long, ProxySync.ObjectState[]>();
            facebookCampaignObjectState.LocalVersion++;
            inputDictionary.Add(facebookCampaignEntity.LocalParentId, new ProxySync.ObjectState[] { facebookCampaignObjectState });

            ProxyDTO.SEMBaseDTO[] response = null;
            WCFHelper.Using<SyncDataServiceClient>(new SyncDataServiceClient(), client =>
                {
                    response = client.Update(inputDictionary);
                });
            Assert.AreEqual(1, response.Length, "The length of response should be 1!!");
            Assert.AreEqual(facebookCampaignObjectState.LocalVersion - 1, response[0].Version, "The response version should be equal to localversion-1!!");
        }

        #endregion
    }
}

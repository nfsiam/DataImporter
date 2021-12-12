using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoMapper;
using DataImporter.Core.BusinessObjects;
using DataImporter.Core.Exceptions;
using DataImporter.Core.Factories;
using DataImporter.Core.Repositories;
using DataImporter.Core.Services;
using DataImporter.Core.Services.Queue;
using DataImporter.Core.UnitOfWorks;
using DataImporter.Core.Utilities;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DataImporter.Core.Tests
{
    [ExcludeFromCodeCoverage]
    public class ImportServiceTests
    {
        private AutoMock _mock;
        private Mock<ICoreUnitOfWork> _coreUnitOfWorkMock;
        private Mock<IGroupRepository> _groupRepositoryMock;
        private IImportService _importService;
        private Mock<IExcelUtility> _excelUtilityMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IImportRepository> _importRepositoryMock;
        private Mock<IHeaderRepository> _headerRepositoryMock;
        private Mock<IQueueService> _queueService;


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mock = AutoMock.GetLoose();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _mock?.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _coreUnitOfWorkMock = _mock.Mock<ICoreUnitOfWork>();
            _groupRepositoryMock = _mock.Mock<IGroupRepository>();
            _excelUtilityMock = _mock.Mock<IExcelUtility>();
            _mapperMock = _mock.Mock<IMapper>();
            _importRepositoryMock = _mock.Mock<IImportRepository>();
            _headerRepositoryMock = _mock.Mock<IHeaderRepository>();
            _importService = _mock.Create<ImportService>();
            _queueService = _mock.Mock<IQueueService>();
        }

        [TearDown]
        public void TearDown()
        {
            _coreUnitOfWorkMock.Reset();
            _groupRepositoryMock.Reset();
        }

        #region GetImportPreviewAsync

        [Test]
        public void GetImportPreviewAsync_ImportBONotProvided_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Import import = null;

            // Act & Assert
            Should.Throw<InvalidOperationException>
                (() => _importService.GetImportPreviewAsync(userId, import));
        }

        [Test]
        public void GetImportPreviewAsync_DefaultUserId_ThrowsException()
        {
            // Arrange
            var userId = default(Guid);
            var import = new Import();

            // Act & Assert
            Should.Throw<InvalidOperationException>
                (() => _importService.GetImportPreviewAsync(userId, import));
        }

        [Test]
        public void GetImportPreviewAsync_GroupEONotFound_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var import = new Import
            {
                GroupId = 0
            };
            var list = new List<Entities.Group>();
            var groupEntity = new Entities.Group
            {
                Id = 0,
                ApplicationUserId = userId,
                Headers = new List<Entities.Header>()
            };

            _coreUnitOfWorkMock.Setup(x => x.Groups)
                .Returns(_groupRepositoryMock.Object);


            _groupRepositoryMock.Setup(x => x.Get(
                    It.Is<Expression<Func<Entities.Group, bool>>>(y => y.Compile()(groupEntity)), "Headers"))
                .Returns(list);

            // Act & Assert
            this.ShouldSatisfyAllConditions(
                () => _coreUnitOfWorkMock.Verify(),
                () => _groupRepositoryMock.Verify(),
                () => Should.Throw<InvalidOperationException>(
                    () => _importService.GetImportPreviewAsync(userId, import))
            );
        }

        [Test]
        public void GetImportPreviewAsync_ConflictingColumnNames_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var import = new Import();
            var groupEntity = new Entities.Group
            {
                ApplicationUserId = userId,
                Headers = new List<Entities.Header>
                {
                    new()
                    {
                        Name = "A",
                    }
                }
            };
            var list = new List<Entities.Group> {groupEntity};
            var incomingHeaders = new List<string> {"B"};

            _coreUnitOfWorkMock.Setup(x => x.Groups)
                .Returns(_groupRepositoryMock.Object);

            _groupRepositoryMock.Setup(x => x.Get(
                    It.Is<Expression<Func<Entities.Group, bool>>>(y => y.Compile()(groupEntity)), "Headers"))
                .Returns(list);

            _excelUtilityMock.Setup(x => x.ParseExcelFileAsync(import.FullFilePath, 10))
                .ReturnsAsync((incomingHeaders, default, default, default));

            // Act & Assert
            this.ShouldSatisfyAllConditions(
                () => _coreUnitOfWorkMock.Verify(),
                () => _groupRepositoryMock.Verify(),
                () => Should.Throw<ColumnNameMismatchException>(() =>
                    _importService.GetImportPreviewAsync(userId, import))
            );
        }


        [Test]
        public async Task GetImportPreviewAsync_ValidNewTask_ReturnsPreviewData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var import = new Import();
            var groupEntity = new Entities.Group
            {
                ApplicationUserId = userId,
                Headers = new List<Entities.Header>
                {
                    new()
                    {
                        Name = "A",
                    }
                }
            };
            var list = new List<Entities.Group> {groupEntity};
            var incomingHeaders = new List<string> {"A"};
            (List<string> headers, List<List<string>> rows, int totalRows, int totalCells)
                extractedData = (incomingHeaders, default, default, default);
            var importEntity = new Entities.Import();

            _coreUnitOfWorkMock.Setup(x => x.Groups)
                .Returns(_groupRepositoryMock.Object);

            _groupRepositoryMock.Setup(x => x.Get(
                    It.Is<Expression<Func<Entities.Group, bool>>>(y => y.Compile()(groupEntity)), "Headers"))
                .Returns(list);

            _excelUtilityMock.Setup(x => x.ParseExcelFileAsync(import.FullFilePath, 10))
                .ReturnsAsync(extractedData);

            _mapperMock.Setup(x => x.Map(import, It.IsAny<Entities.Import>()))
                .Returns(importEntity);

            _coreUnitOfWorkMock.Setup(x => x.Imports)
                .Returns(_importRepositoryMock.Object);

            _importRepositoryMock.Setup(x => x.Add(importEntity)).Verifiable();

            _coreUnitOfWorkMock.Setup(x => x.Save()).Verifiable();


            // Act
            var result = await _importService.GetImportPreviewAsync(userId, import);

            // Assert
            this.ShouldSatisfyAllConditions(
                () => _coreUnitOfWorkMock.Verify(),
                () => _groupRepositoryMock.Verify(),
                () => _importRepositoryMock.VerifyAll(),
                () => result.ShouldBe(extractedData)
            );
        }

        #endregion

        #region ConfirmImportAsync

        [Test]
        public void ConfirmImportAsync_DefaultUserId_ThrowsException()
        {
            // Arrange
            var userId = default(Guid);
            var id = 1;

            // Act & Assert
            Should.ThrowAsync<InvalidOperationException>
                (() => _importService.ConfirmImportAsync(userId, id));
        }

        [Test]
        public void ConfirmImportAsync_InvalidImportId_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var id = 0;

            // Act & Assert
            Should.ThrowAsync<InvalidOperationException>
                (() => _importService.ConfirmImportAsync(userId, id));
        }

        [Test]
        public void ConfirmImportAsync_FalseParamsToFindImport_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var id = 1;
            var list = new List<Entities.Import>();
            var importEntity = new Entities.Import
            {
                Id = 1,
                Group = new()
                {
                    ApplicationUserId = userId
                }
            };

            _coreUnitOfWorkMock.Setup(x => x.Imports)
                .Returns(_importRepositoryMock.Object);

            _importRepositoryMock.Setup(x => x.Get(
                    It.Is<Expression<Func<Entities.Import, bool>>>(y => y.Compile()(importEntity)), "Group"))
                .Returns(list);

            // Act & Assert
            Should.ThrowAsync<InvalidOperationException>
                (async () => await _importService.ConfirmImportAsync(userId, id));
        }


        [Test]
        public async Task ConfirmImportAsync_MetAllCriteria_EnqueueTask()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var id = 1;
            var importEntity = new Entities.Import
            {
                Id = 1,
                Group = new()
                {
                    ApplicationUserId = userId
                }
            };
            var list = new List<Entities.Import>() {importEntity};

            _coreUnitOfWorkMock.Setup(x => x.Imports)
                .Returns(_importRepositoryMock.Object).Verifiable();

            _importRepositoryMock.Setup(x => x.Get(
                    It.Is<Expression<Func<Entities.Import, bool>>>(y => y.Compile()(importEntity)), "Group"))
                .Returns(list).Verifiable();

            _queueService.Setup(x => x.EnqueueTaskAsync(importEntity.Id, Enums.QueueTaskType.Import))
                .Returns(Task.FromResult(default(object)))
                .Verifiable();

            _coreUnitOfWorkMock.Setup(x => x.Save()).Verifiable();


            // Act
            await _importService.ConfirmImportAsync(userId, id);

            // Assert
            this.ShouldSatisfyAllConditions(
                () => _coreUnitOfWorkMock.VerifyAll(),
                () => _importRepositoryMock.VerifyAll(),
                () => importEntity.Status.ShouldBe("Queued")
            );
        }

        #endregion

        #region CancelImport

        [Test]
        public void CancelImport_RemovableHeaders_RemoveHeaders()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var id = 1;
            var groupEntity = new Entities.Group
            {
                ApplicationUserId = userId
            };
            var importEntity = new Entities.Import
            {
                Id = 1,
                Group = groupEntity
            };
            var importEntity2 = new Entities.Import
            {
                Id = 2,
                Group = groupEntity
            };
            var headerEntity = new Entities.Header
            {
                Group = groupEntity,
            };

            var list = new List<Entities.Import>() {importEntity};

            _coreUnitOfWorkMock.Setup(x => x.Imports)
                .Returns(_importRepositoryMock.Object).Verifiable();

            _importRepositoryMock.Setup(x => x.Get(
                    It.Is<Expression<Func<Entities.Import, bool>>>(y => y.Compile()(importEntity)), "Group"))
                .Returns(list).Verifiable();

            _importRepositoryMock.Setup(x => x.GetCount(
                    It.Is<Expression<Func<Entities.Import, bool>>>(y => y.Compile()(importEntity2))))
                .Returns(0).Verifiable();

            _coreUnitOfWorkMock.Setup(x => x.Headers)
                .Returns(_headerRepositoryMock.Object).Verifiable();

            _headerRepositoryMock.Setup(x => x.Remove(
                    It.Is<Expression<Func<Entities.Header, bool>>>(y => y.Compile()(headerEntity))))
                .Verifiable();

            _importRepositoryMock.Setup(x => x.Remove(importEntity))
                .Verifiable();

            _coreUnitOfWorkMock.Setup(x => x.Save()).Verifiable();


            // Act
            _importService.CancelImport(userId, id);

            // Assert
            this.ShouldSatisfyAllConditions(
                () => _coreUnitOfWorkMock.VerifyAll(),
                () => _importRepositoryMock.VerifyAll(),
                () => _headerRepositoryMock.VerifyAll()
            );
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Concrete;

namespace OnlibeLibrary.Services.UnitTests.Concrete_Tests.CategoryService_Tests
{
    [TestFixture]
    class CreateCategoryTestClass
    {
        private ILibraryDbContext _dbContext;
        private DbSet<Category> _categoriesSet;

        [OneTimeSetUp]
        public void Init()
        {
            _categoriesSet = Substitute.For<DbSet<Category>>();

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.Categories.Returns(_categoriesSet);

        }

        [Test]
        public void Should_AddNewCategory()
        {
            // Arrange.           
            var categoryService = new CategoryService(_dbContext);
            var categoryName = "Programming";

            // Act.
            var createdCategory = categoryService.CreateCategory(categoryName);

            // Assert.
            var delegateOne = new TestDelegate(() => _categoriesSet.Received(1).Add(Arg.Any<Category>()));
            var delegateTwo = new TestDelegate(() => _dbContext.Received(1).SaveChanges());

            Assert.DoesNotThrow(delegateOne);
            Assert.DoesNotThrow(delegateTwo);
        }
    }
}

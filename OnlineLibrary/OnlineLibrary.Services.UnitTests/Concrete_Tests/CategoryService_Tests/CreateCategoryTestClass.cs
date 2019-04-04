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

namespace OnlineLibrary.Services.UnitTests.Concrete_Tests.CategoryService_Tests
{
    [TestFixture]
    class CreateCategoryTestClass
    {
        private ILibraryDbContext _dbContext;
        private DbSet<Category> _categoriesSet;

        [OneTimeSetUp]
        public void Init()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "History" }
            }
            .AsQueryable();

            _categoriesSet = Substitute.For<DbSet<Category>, IQueryable<Category>>();
            ((IQueryable<Category>)_categoriesSet).Provider.Returns(categories.Provider);
            ((IQueryable<Category>)_categoriesSet).Expression.Returns(categories.Expression);
            ((IQueryable<Category>)_categoriesSet).ElementType.Returns(categories.ElementType);
            ((IQueryable<Category>)_categoriesSet).GetEnumerator().Returns(categories.GetEnumerator());

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

        [Test]
        public void Should_ThrowArgumentException()
        {
            // Arrange.           
            var categoryService = new CategoryService(_dbContext);
            var categoryName = "History";

            // Act.
            var sutDelegate = new TestDelegate(() => categoryService.CreateCategory(categoryName));

            // Assert.
            Assert.Throws<ArgumentException>(sutDelegate);
        }
    }
}

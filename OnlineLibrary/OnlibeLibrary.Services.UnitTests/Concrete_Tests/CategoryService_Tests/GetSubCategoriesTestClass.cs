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
using OnlineLibrary.Services.Concrete;

namespace OnlibeLibrary.Services.UnitTests.Concrete_Tests.CategoryService_Tests
{
    [TestFixture]
    class GetSubCategoriesTestClass
    {
        private ILibraryDbContext _dbContext;

        [OneTimeSetUp]
        public void Init()
        {
            var subCategories = new List<SubCategory>
            {
                new SubCategory { Id = 1, Name = "New Age", Category = new Category { Id = 1, Name = "History" } },
                new SubCategory { Id = 2, Name = "Middle Age", Category = new Category { Id = 1, Name = "History" } },
                new SubCategory { Id = 3, Name = "Old Age", Category = new Category { Id = 1, Name = "History" } },

                new SubCategory { Id = 4, Name = "C# Programming", Category = new Category { Id = 2, Name = "Programming" } },
            }
            .AsQueryable();

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "History", SubCategories = new List<SubCategory> { subCategories.Single(sc => sc.Id == 1), subCategories.Single(sc => sc.Id == 2), subCategories.Single(sc => sc.Id == 3), } },
                new Category { Id = 2, Name = "Programming", SubCategories = new List<SubCategory> { subCategories.Single(sc => sc.Id == 4) } },
                new Category { Id = 3, Name = "Mystery" },
            }
            .AsQueryable();

            var categoriesSet = Substitute.For<DbSet<Category>, IQueryable<Category>>();
            categoriesSet.Find(Arg.Any<object[]>()).Returns(callinfo =>
            {
                object[] idValues = callinfo.Arg<object[]>();
                if (idValues != null && idValues.Length == 1)
                {
                    int requestedId = (int)idValues[0];
                    return categories.SingleOrDefault(c => c.Id == requestedId);
                }

                return null;
            });
            ((IQueryable<Category>)categoriesSet).Provider.Returns(categories.Provider);
            ((IQueryable<Category>)categoriesSet).Expression.Returns(categories.Expression);
            ((IQueryable<Category>)categoriesSet).ElementType.Returns(categories.ElementType);
            ((IQueryable<Category>)categoriesSet).GetEnumerator().Returns(categories.GetEnumerator());

            var subCategoriesSet = Substitute.For<DbSet<SubCategory>, IQueryable<SubCategory>>();
            ((IQueryable<SubCategory>)subCategoriesSet).Provider.Returns(subCategories.Provider);
            ((IQueryable<SubCategory>)subCategoriesSet).Expression.Returns(subCategories.Expression);
            ((IQueryable<SubCategory>)subCategoriesSet).ElementType.Returns(subCategories.ElementType);
            ((IQueryable<SubCategory>)subCategoriesSet).GetEnumerator().Returns(subCategories.GetEnumerator());

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.Categories.Returns(categoriesSet);
            _dbContext.SubCategories.Returns(subCategoriesSet);
        }

        [Test]
        public void Should_ReturnListOfSubCategories_Given_ProperCategoryId()
        {
            // Arrange.
            int categoryId = 1;
            var categoryService = new CategoryService(_dbContext);

            // Act.
            IEnumerable<SubCategory> subcategories = categoryService.GetSubCategories(categoryId);

            // Assert.
            Assert.AreEqual(3, subcategories.Count());
        }

        [Test]
        public void Should_ThrowKeyNotFoundExceptionException_Given_InvalidCategoryId()
        {
            // Arrange.
            int categoryId = -1;
            var categoryService = new CategoryService(_dbContext);

            // Act.
            var sutDelegate = new TestDelegate(() => 
            {
                IEnumerable<SubCategory> subcategories = categoryService.GetSubCategories(categoryId);
            });

            // Assert.
            Assert.Throws<KeyNotFoundException>(sutDelegate);
        }
    }
}

using NSubstitute;
using NUnit.Framework;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlibeLibrary.Services.UnitTests.Concrete_Tests.CategoryService_Tests
{
    [TestFixture]
    public class DeleteBookCategoryTestClass
    {
        private ILibraryDbContext _dbContext;

        [OneTimeSetUp]
        public void init()
        {

            var categories = new List<Category>
            {
                new Category { Id = 1 },
                new Category { Id = 2 },
                new Category { Id = 3 }
            };
            var categoriesQueryable = categories.AsQueryable();
            
            // CategoryId to set for a subcategory
            var categoryId = 3;
            var subcateogories = new List<SubCategory>
            {
                new SubCategory { Id = 1 },
                new SubCategory { Id = 2, CategoryId = categoryId }
            }
            .AsQueryable();

            var categoriesSet = Substitute.For<DbSet<Category>, IQueryable<Category>>();
            ((IQueryable<Category>)categoriesSet).Provider.Returns(categoriesQueryable.Provider);
            ((IQueryable<Category>)categoriesSet).Expression.Returns(categoriesQueryable.Expression);
            ((IQueryable<Category>)categoriesSet).ElementType.Returns(categoriesQueryable.ElementType);
            ((IQueryable<Category>)categoriesSet).GetEnumerator().Returns(categoriesQueryable.GetEnumerator());

            // Category to remove.
            var categoryIdToRemove = 2;
            // Mock Remove method.
            categoriesSet.When(x => x.Remove(categories.FirstOrDefault(c => c.Id == categoryIdToRemove)))
                         .Do(x => categories.Remove(categories.FirstOrDefault(c => c.Id == categoryIdToRemove)));


            var subcategoriesSet = Substitute.For<DbSet<SubCategory>, IQueryable<SubCategory>>();
            ((IQueryable<SubCategory>)subcategoriesSet).Provider.Returns(subcateogories.Provider);
            ((IQueryable<SubCategory>)subcategoriesSet).Expression.Returns(subcateogories.Expression);
            ((IQueryable<SubCategory>)subcategoriesSet).ElementType.Returns(subcateogories.ElementType);
            ((IQueryable<SubCategory>)subcategoriesSet).GetEnumerator().Returns(subcateogories.GetEnumerator());

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.SubCategories.Returns(subcategoriesSet);
            _dbContext.Categories.Returns(categoriesSet);
        }

        [Test]
        public void Should_DeleteCategory_When_IsCategoryRemovable_ReturnTrue()
        {
            // Arrange.
            var sut = Substitute.For<CategoryService>( _dbContext);

            // Act.
            // Current number of category minus one to be removed.
            var expectedResult = _dbContext.Categories.Count() - 1;
            sut.DeleteBookCategory(2);
            // Number of categories after remove method.
            var result = _dbContext.Categories.Count();

            // Assert.
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Should_ThrowBookCategoryIsNotRemovableException_When_IsCategoryRemovable_ReturnFalse()
        {
            // Arrange.
            var sut = Substitute.For<CategoryService>(_dbContext);

            // Act.
            var testDelegate = new TestDelegate(() => sut.DeleteBookCategory(3));

            // Assert.
            Assert.Throws<BookCategoryIsNotRemovableException>(testDelegate);
            
        }
    }
}

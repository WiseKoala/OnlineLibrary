﻿using NSubstitute;
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

namespace OnlineLibrary.Services.UnitTests.Concrete_Tests.CategoryService_Tests
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
            var categoryIdRemovable = 2;
            var categoryIdNotRemovable = 3;
            // Mock Remove method.
            categoriesSet.When(x => x.Remove(categories.FirstOrDefault(c => c.Id == categoryIdRemovable)))
                         .Do(x => categories.Remove(categories.FirstOrDefault(c => c.Id == categoryIdRemovable)));

            categoriesSet.Find(categoryIdRemovable).Returns(x => categories.Find(c => c.Id == categoryIdRemovable));
            categoriesSet.Find(categoryIdNotRemovable).Returns(x => categories.Find(c => c.Id == categoryIdNotRemovable));


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
            var categoryId = 2;
            // Act.
            // Current number of category minus one to be removed.
            var expectedResult = _dbContext.Categories.Count() - 1;
            sut.DeleteBookCategory(categoryId);
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
            var categoryId = 3;
            // Act.
            var testDelegate = new TestDelegate(() => sut.DeleteBookCategory(categoryId));

            // Assert.
            Assert.Throws<BookCategoryIsNotRemovableException>(testDelegate);            
        }

        [Test]
        public void Should_ThrowBookCategoryNotFoundException_When_IsCategoryRemovable_ReturnFalse()
        {
            // Arrange.
            var sut = Substitute.For<CategoryService>(_dbContext);
            var categoryId = 100;
            // Act.
            var testDelegate = new TestDelegate(() => sut.DeleteBookCategory(categoryId));

            // Assert.
            Assert.Throws<BookCategoryNotFoundException>(testDelegate);
        }
    }
}

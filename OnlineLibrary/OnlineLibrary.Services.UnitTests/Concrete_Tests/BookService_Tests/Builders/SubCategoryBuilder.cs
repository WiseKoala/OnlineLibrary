using OnlineLibrary.DataAccess.Entities;

namespace OnlineLibrary.Services.UnitTests.Concrete_Tests.BookService_Tests.Builders
{
    public class SubCategoryBuilder
    {
        private int _categoryId = 0;

        public SubCategory Build()
        {
            return new SubCategory
            {
                CategoryId = _categoryId
            };
        }

        public SubCategoryBuilder WithCategoryId(int categoryId)
        {
            _categoryId = categoryId;
            return this;
        }
    }
}

using System;
using System.Web;
using OnlineLibrary.Services.Models.BookServiceModels;
using OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels;

namespace OnlineLibrary.Web
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            AutoMapper.Mapper.Initialize(config =>
            {
                // Service models to View models.
                config.CreateMap<CreateEditBookViewModel, CreateEditBookServiceModel>();
                config.CreateMap<FrontCoverViewModel, FrontCoverServiceModel>();
                config.CreateMap<HttpPostedFileBase, HttpPostedFileServiceModel>();
                config.CreateMap<BookAuthorViewModel, BookAuthorServiceModel>();
                config.CreateMap<AuthorNameViewModel, AuthorNameServiceModel>();
                config.CreateMap<BookCopyViewModel, BookCopyServiceModel>();
                config.CreateMap<CategoryViewModel, CategoryServiceModel>();
                config.CreateMap<SubCategoryViewModel, SubCategoryServiceModel>();

                // View models to Service models.
                config.CreateMap<CreateEditBookServiceModel, CreateEditBookViewModel>();
                config.CreateMap<FrontCoverServiceModel, FrontCoverViewModel>()
                      .ForMember(ce => ce.Image, t => t.Ignore());
                config.CreateMap<BookAuthorServiceModel, BookAuthorViewModel>();
                config.CreateMap<AuthorNameServiceModel, AuthorNameViewModel>();
                config.CreateMap<BookCopyServiceModel, BookCopyViewModel>();
                config.CreateMap<CategoryServiceModel, CategoryViewModel>();
                config.CreateMap<SubCategoryServiceModel, SubCategoryViewModel>();
            });
        }
    }
}
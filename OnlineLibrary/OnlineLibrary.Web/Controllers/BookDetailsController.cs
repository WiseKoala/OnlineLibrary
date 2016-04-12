using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models;

namespace OnlineLibrary.Web.Controllers
{
    public class BookDetailsController : BaseController
    {
        private string[] conditionStrArray = { "", "", "", "", "", "" };
        private string conditionStr = "";

        public ActionResult Index(int id)
        {
            if (!IsUserNameSessionVariableSet())
            {
                InitializeUserNameSessionVariable();
            }

            var book = DbContext.Books.First(b => b.Id == id);
            var bookcopies = new List<BookCopy>();
            int[] condition =
            {
                (DbContext.BookCopies.Where(n => n.BookId == id && n.Condition == DataAccess.Enums.BookCondition.New).Count()),
                (DbContext.BookCopies.Where(n => n.BookId == id && n.Condition == DataAccess.Enums.BookCondition.Fine).Count()),
                (DbContext.BookCopies.Where(n => n.BookId == id && n.Condition == DataAccess.Enums.BookCondition.VeryGood).Count()),
                (DbContext.BookCopies.Where(n => n.BookId == id && n.Condition == DataAccess.Enums.BookCondition.Good).Count()),
                (DbContext.BookCopies.Where(n => n.BookId == id && n.Condition == DataAccess.Enums.BookCondition.Fair).Count()),
                (DbContext.BookCopies.Where(n => n.BookId == id && n.Condition == DataAccess.Enums.BookCondition.Poor).Count())
            };

                if (condition[0] != 0) { conditionStrArray[0] = condition[0] + " New"; };
                if (condition[1] != 0) { conditionStrArray[1] = condition[1] + " Fine"; };
                if (condition[2] != 0) { conditionStrArray[2] = condition[2] + " Very Good"; };
                if (condition[3] != 0) { conditionStrArray[3] = condition[3] + " Good"; };
                if (condition[4] != 0) { conditionStrArray[4] = condition[4] + " Fair"; };
                if (condition[5] != 0) { conditionStrArray[5] = condition[5] + " Poor"; };

            foreach (var s in conditionStrArray)
            { if (s.Length != 0) { conditionStr = conditionStr + s + ", "; } };
           if (conditionStr.Length > 3) { conditionStr = conditionStr.Substring(0, conditionStr.Length - 2); };

            var book_view = new BookDetailsViewModel
            {
                Id = book.Id,
                Title = book.Title,
                PublishDate = book.PublishDate,
                FrontCover = book.FrontCover,
                Authors = book.Authors.Select(a =>
                        string.Join(" ", a.FirstName, (a.MiddleName ?? ""), a.LastName)),
                Description = book.Description,
                ISBN = book.ISBN,
                NrOfBooks = DbContext.BookCopies.Count(n => n.BookId == id),
                HowManyInThisCondition = conditionStr,
                Categories = book.SubCategories.Select(sc => new CategoryViewModel
                {
                    Category = sc.Category.Name,
                    SubCategory = sc.Name
                })
            };
            return View(book_view);
        }
    }
}

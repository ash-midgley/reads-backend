using NUnit.Framework;
using Api;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using FakeItEasy;

namespace Tests
{
    public class CategoriesControllerShould
    {
        private CategoryValidator Validator => new CategoryValidator();
        private readonly List<Category> TestCategories = new List<Category>
        {
            new Category 
            {
                Id = 1,
                UserId = 1,
                Description = "Fiction",
                Code = "🧟"
            },
            new Category 
            { 
                Id = 2,
                UserId = 1,
                Description = "Non-fiction",
                Code = "🧠"
            }
        };
        private readonly Category CategorySuccess = new Category
        {
            Description = "Sci-fi",
            Code = "🚀"
        };
        private readonly Category CategoryFail = new Category();

        [Test]
        public void ReturnAllCategories()
        {
            const int userId = 1;
            var repository = A.Fake<ICategoryRepository>();
            A.CallTo(() => repository.GetUserCategories(userId)).Returns(TestCategories);
            var controller = new CategoriesController(repository, Validator);

            var categories = controller.GetUserCategories(userId);
            
            Assert.AreEqual(TestCategories, categories);
        }

        [Test]
        public void CreateNewCategory()
        {
            const int id = 1;
            var result = CategorySuccess;
            result.Id = id;
            var repository = A.Fake<ICategoryRepository>();
            A.CallTo(() => repository.Add(CategorySuccess)).Returns(id);
            A.CallTo(() => repository.GetCategory(id)).Returns(result);
            var controller = new CategoriesController(repository, Validator);

            var responseOne = controller.Post(CategorySuccess);
            var responseTwo = controller.Post(CategoryFail);

            Assert.AreEqual(result, responseOne.Value);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((BadRequestObjectResult)responseTwo.Result).StatusCode);
        }

        [Test]
        public void UpdateExistingCategory()
        {
            const int id = 1;
            var updatedCategory = CategorySuccess;
            updatedCategory.Id = id;
            updatedCategory.Description = "Updated description...";
            var repository = A.Fake<ICategoryRepository>();
            A.CallTo(() => repository.GetCategory(id)).Returns(updatedCategory);
            var controller = new CategoriesController(repository, Validator);

            var responseOne = controller.Put(updatedCategory);
            var responseTwo = controller.Put(CategoryFail);

            Assert.AreEqual(updatedCategory, responseOne.Value);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((BadRequestObjectResult)responseTwo.Result).StatusCode);
        }

        [Test]
        public void DeleteCategory()
        {
            var result = CategorySuccess;
            result.Id = 1;
            var repository = A.Fake<ICategoryRepository>();
            A.CallTo(() => repository.GetCategory(A<int>.Ignored)).Returns(result);
            var controller = new CategoriesController(repository, Validator);

            var responseOne = controller.Delete(CategorySuccess);
            var responseTwo = controller.Delete(CategoryFail);
            
            Assert.AreEqual(result, responseOne.Value);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((BadRequestObjectResult)responseTwo.Result).StatusCode);
        }
    }
}
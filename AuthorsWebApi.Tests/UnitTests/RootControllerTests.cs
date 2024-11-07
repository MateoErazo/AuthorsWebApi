using AuthorsWebApi.Controllers.V1;
using AuthorsWebApi.Tests.Mocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthorsWebApi.Tests.UnitTests
{
    [TestClass]
    public class RootControllerTests
    {
        [TestMethod]
        public async Task IfUserIsAdmin_ReturnTenLinks()
        {
            //preparation
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.AuthorizationResult = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            //execution
            var result = await rootController.GetRoots();

            //verification
            Assert.AreEqual(10, result.Value.Count());
        }

        [TestMethod]
        public async Task IfUserIsNotAdmin_ReturnSixLinks()
        {
            //preparation
            var authorizationServiceMock = new Mock<IAuthorizationService>();

            authorizationServiceMock.Setup(x =>
            x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .Returns(Task.FromResult(AuthorizationResult.Failed()));

            authorizationServiceMock.Setup(x =>
            x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()))
                .Returns(Task.FromResult(AuthorizationResult.Failed()));

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(x => x.Link(It.IsAny<string>(),It.IsAny<object>()))
                .Returns(string.Empty);

            var rootController = new RootController(authorizationServiceMock.Object);
            rootController.Url = urlHelperMock.Object;

            //execution
            var result = await rootController.GetRoots();

            //verification
            Assert.AreEqual(6, result.Value.Count());
        }
    }
}

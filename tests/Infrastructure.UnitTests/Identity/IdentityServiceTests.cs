using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using TimeKeeper.Application.Common.Interfaces;
using TimeKeeper.Infrastructure.Identity;

namespace Infrastructure.UnitTests.Identity
{
    public class IdentityServiceTests
    {
        private Mock<IUserManagerAdapter<ApplicationUser>> userManagerAdapter;
        private Mock<ICurrentUserService> currentUserService;
        private Mock<IConfiguration> configuration;
        private IdentityService identityService;

        [SetUp]
        public void Setup()
        {
            userManagerAdapter = new Mock<IUserManagerAdapter<ApplicationUser>>();
            currentUserService = new Mock<ICurrentUserService>();
            configuration = new Mock<IConfiguration>();
            identityService = new IdentityService(userManagerAdapter.Object, currentUserService.Object, configuration.Object);
        }

        [Test]
        public async Task GetUserByNameShouldReturnUser()
        {
            //Arrange
            var userName = "UserName";
            var userId = "UserId";
            var role = "Role";
            var user = new ApplicationUser { UserName = userName, Id = userId };
            userManagerAdapter.Setup(m => m.FindByNameAsync(userName)).ReturnsAsync(user);
            userManagerAdapter.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new[] { role });

            //Act
            var result = await identityService.GetUserByNameAsync(userName);

            //Assert
            result.UserName.Should().Be(userName);
            result.UserId.Should().Be(userId);
            result.Role.Should().Be(role);
        }

        [Test]
        public async Task GetUserByNameShouldReturnNullIfUserDoesntExist()
        {
            //Arrange
            var userName = "UserName";
            userManagerAdapter.Setup(m => m.FindByNameAsync(userName)).ReturnsAsync((ApplicationUser)null);

            //Act
            var result = await identityService.GetUserByNameAsync(userName);

            //Assert
            result.Should().BeNull();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TimeKeeper.Application.Common.Interfaces;
using TimeKeeper.Application.Common.Models;
using TimeKeeper.Application.WorkingHours.CreateWorkingHours;
using TimeKeeper.Domain.Entities;
using MockQueryable.Moq;
using TimeKeeper.Application.Common.Exceptions;

namespace TimeKeeper.Application.UnitTests.WorkingHours
{
    public class CreateWorkingHoursTests
    {
        private CreateWorkingHoursCommandHandler _handler;
        private Mock<IIdentityService> _identityService;
        private Mock<IApplicationDbContext> _applicationDbContext;
        private CancellationToken _cancellationToken;
        private DateTime _date = DateTime.UtcNow.Date;
        private string _userName = "UserName";
        private string _userId = "UserId";

        [SetUp]
        public void Setup()
        {
            _identityService = new Mock<IIdentityService>();
            _applicationDbContext = new Mock<IApplicationDbContext>();
            _handler = new CreateWorkingHoursCommandHandler(_identityService.Object, _applicationDbContext.Object);
        }

        [Test]
        public async Task ShouldThrowValidationExceptionWhenUserIsNull()
        {
            //Arrange
            var request = new CreateWorkingHoursCommand
            {
                Date = _date,
                Description = "Description",
                Duration = TimeSpan.FromHours(4),
                UserName = _userName
            };
            _identityService.Setup(s => s.GetUserByNameAsync(_userName)).ReturnsAsync((UserDto)null);
            _identityService.Setup(s => s.CheckUserIsNotNull(null)).Throws<ValidationException>();

            //Act & Assert
            FluentActions.Invoking(() => _handler.Handle(request, _cancellationToken)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldCreateWorkingHours()
        {
            //Arrange
            var request = new CreateWorkingHoursCommand
            {
                Date = _date,
                Description = "Description",
                Duration = TimeSpan.FromHours(4),
                UserName = _userName
            };
            var expectedCreatedId = 10;
            var expectedWorkingHour = new UserWorkingHours(_userId, "Description", _date, TimeSpan.FromHours(4)) { Id = expectedCreatedId };
            var user = new UserDto(_userName, _userId, "Role");
            _identityService.Setup(s => s.GetUserByNameAsync(_userName)).ReturnsAsync(user);

            var mockWorkingHoursDbSet = new List<UserWorkingHours>
            {
                new UserWorkingHours(_userId, "Description", _date, TimeSpan.FromHours(10))
            }.AsQueryable().BuildMockDbSet();
            _applicationDbContext.Setup(c => c.WorkingHoursSet).Returns(mockWorkingHoursDbSet.Object);
            _applicationDbContext.Setup(c => c.WorkingHoursSet.Add(It.IsAny<UserWorkingHours>()))
                .Callback<UserWorkingHours>(i => i.Id = expectedCreatedId);

            //Act
            var id = await _handler.Handle(request, _cancellationToken);

            //Assert
            id.Should().Be(expectedCreatedId);
            _applicationDbContext.Verify(c =>
                c.WorkingHoursSet.Add(It.Is<UserWorkingHours>(i => MatchWorkingHours(i, expectedWorkingHour))));
            _applicationDbContext.Verify(c => c.SaveChangesAsync(_cancellationToken), Times.Once);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenSumOfWorkingHoursForDayBiggerThan24()
        {
            //Arrange
            var request = new CreateWorkingHoursCommand
            {
                Date = _date,
                Description = "Description",
                Duration = TimeSpan.FromHours(4),
                UserName = _userName
            };
            var expectedCreatedId = 10;
            var user = new UserDto(_userName, _userId, "Role");
            _identityService.Setup(s => s.GetUserByNameAsync(_userName)).ReturnsAsync(user);

            var mockWorkingHoursDbSet = new List<UserWorkingHours>
            {
                new UserWorkingHours(_userId, "Description", _date, TimeSpan.FromHours(10)),
                new UserWorkingHours(_userId, "Description2", _date, TimeSpan.FromHours(12)),
            }.AsQueryable().BuildMockDbSet();
            _applicationDbContext.Setup(c => c.WorkingHoursSet).Returns(mockWorkingHoursDbSet.Object);
            _applicationDbContext.Setup(c => c.WorkingHoursSet.Add(It.IsAny<UserWorkingHours>()))
                .Callback<UserWorkingHours>(i => i.Id = expectedCreatedId);

            //Act & Assert
            FluentActions.Invoking(() => _handler.Handle(request, _cancellationToken)).Should().Throw<ValidationException>()
                .Which.Errors.First().Value.First().Should().Be("Sum of working hours per day can't be bigger than 24 hours");
        }

        private static bool MatchWorkingHours(UserWorkingHours userWorkingHours, UserWorkingHours expectedWorkingHour)
        {
            userWorkingHours.Should().BeEquivalentTo(expectedWorkingHour);
            return true;
        }
    }
}

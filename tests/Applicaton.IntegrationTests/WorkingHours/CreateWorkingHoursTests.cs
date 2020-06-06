using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TimeKeeper.Application.Common.Exceptions;
using TimeKeeper.Application.WorkingHours.CreateWorkingHours;
using TimeKeeper.Domain.Entities;

namespace TimeKeeper.Application.IntegrationTests.WorkingHours
{
    using static Testing;

    public class CreateWorkingHoursTests : TestBase
    {
        [Test]
        public void ShouldRequireMinimumFields()
        {
            //Arrange
            var command = new CreateWorkingHoursCommand();

            //Act & Assert
            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldCreateWorkingHours()
        {
            //Arrange
            var userId = await RunAsDefaultUserAsync();
            var command = new CreateWorkingHoursCommand
            {
                Date = DateTime.UtcNow,
                Duration = TimeSpan.FromHours(3),
                UserName = CurrentUser.UserName,
                Description = "Some description"
            };

            //Act
            var id = await SendAsync(command);

            //Assert
            var item = await FindAsync<UserWorkingHours>(id);
            item.Should().NotBeNull();
            item.Date.Should().Be(command.Date.Date);
            item.Description.Should().Be(command.Description);
            item.Duration.Should().Be(command.Duration);
            item.UserId.Should().Be(userId);
            item.CreatedBy.Should().Be(userId);
            item.Created.Should().BeCloseTo(DateTime.Now, 10000);
            item.LastModifiedBy.Should().BeNull();
            item.LastModified.Should().BeNull();
        }
    }
}

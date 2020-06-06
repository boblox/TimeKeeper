using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TimeKeeper.Application.Common.Models;
using TimeKeeper.Application.WorkingHours.GetWorkingHours;
using TimeKeeper.Domain.Entities;

namespace TimeKeeper.Application.IntegrationTests.WorkingHours
{
    using static Testing;

    public class GetWorkingHoursTests : TestBase
    {
        [Test]
        public async Task ShouldFilterByUserName()
        {
            //Arrange
            var userId = await RunAsDefaultUserAsync();
            var otherUserId = (await CreateUserAsync("otherUser", "password", Roles.User)).Id;
            var date = DateTime.UtcNow.Date;
            var workingHours = new[]
            {
                new UserWorkingHours(userId, "Description1", date, TimeSpan.FromHours(4)),
                new UserWorkingHours(userId, "Description2", date.AddDays(1), TimeSpan.FromHours(10)),
                new UserWorkingHours(userId, "Description3", date.AddDays(-1), TimeSpan.FromHours(12)),
                new UserWorkingHours(otherUserId, "Description4", date.AddDays(-1), TimeSpan.FromHours(8)),
            };
            workingHours.ToList().ForEach(async i => await AddAsync(i));
            var expectedItems = new[]
            {
                new WorkingHoursDto(0, "Description1", date, TimeSpan.FromHours(4), false),
                new WorkingHoursDto(1, "Description2", date.AddDays(1), TimeSpan.FromHours(10), false),
                new WorkingHoursDto(2, "Description3", date.AddDays(-1), TimeSpan.FromHours(12), false),
            };

            var command = new GetWorkingHoursQuery(CurrentUser.UserName, null, null);

            //Act
            var result = await SendAsync(command);

            //Assert
            result.UserName.Should().Be(CurrentUser.UserName);
            result.WorkingHoursList.Should().HaveCount(3);
            result.WorkingHoursList.Should().BeEquivalentTo(expectedItems, o => o.Excluding(i => i.Id));
        }

        [Test]
        public async Task ShouldFilterByDate()
        {
            //Arrange
            var userId = await RunAsDefaultUserAsync();
            var date = DateTime.UtcNow.Date;
            var workingHours = new[]
            {
                new UserWorkingHours(userId, "Description1", date, TimeSpan.FromHours(4)),
                new UserWorkingHours(userId, "Description2", date.AddDays(2), TimeSpan.FromHours(10)),
                new UserWorkingHours(userId, "Description3", date.AddDays(-1), TimeSpan.FromHours(12)),
            };
            workingHours.ToList().ForEach(async i => await AddAsync(i));
            var expectedItems = new[]
            {
                new WorkingHoursDto(0, "Description1", date, TimeSpan.FromHours(4), false),
            };

            var command = new GetWorkingHoursQuery(CurrentUser.UserName, date, date.AddDays(1));

            //Act
            var result = await SendAsync(command);

            //Assert
            result.UserName.Should().Be(CurrentUser.UserName);
            result.WorkingHoursList.Should().HaveCount(1);
            result.WorkingHoursList.Should().BeEquivalentTo(expectedItems, o => o.Excluding(i => i.Id));
        }

        [Test]
        [TestCase(10, false)]
        [TestCase(23, true)]
        public async Task IsUnderPreferredWorkingHoursDurationIsCalculatedProperly(int preferredDurationInHours, bool isUnder)
        {
            //Arrange
            var userId = await RunAsDefaultUserAsync();
            var date = DateTime.UtcNow.Date;
            var workingHours = new[]
            {
                new UserWorkingHours(userId, "Description1", date, TimeSpan.FromHours(4)),
                new UserWorkingHours(userId, "Description2", date, TimeSpan.FromHours(10)),
                new UserWorkingHours(userId, "Description3", date, TimeSpan.FromHours(8)),
            };
            workingHours.ToList().ForEach(async i => await AddAsync(i));
            await AddAsync(new UserPreferredWorkingHours(userId, TimeSpan.FromHours(preferredDurationInHours)));

            var expectedItems = new[]
            {
                new WorkingHoursDto(0, "Description1", date, TimeSpan.FromHours(4), isUnder),
                new WorkingHoursDto(0, "Description2", date, TimeSpan.FromHours(10), isUnder),
                new WorkingHoursDto(0, "Description3", date, TimeSpan.FromHours(8), isUnder),
            };

            var command = new GetWorkingHoursQuery(CurrentUser.UserName, null, null);

            //Act
            var result = await SendAsync(command);

            //Assert
            result.PreferredWorkingHoursDuration.Should().Be(TimeSpan.FromHours(preferredDurationInHours));
            result.WorkingHoursList.Should().HaveCount(3);
            result.WorkingHoursList.Should().BeEquivalentTo(expectedItems, o => o.Excluding(i => i.Id));
        }
    }
}
using Conduit.Business.Services;
using Conduit.Data.Entities;
using Conduit.Data.EntityFramework;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Conduit.Business.Tests.Services
{
    public class TagsServiceTests
    {
        private readonly TagsService _tagsService;
        private readonly ApplicationDbContext _dbContext;

        public TagsServiceTests()
        {
            _dbContext = DbContextProvider.GetInMemoryDbContext();
            _tagsService = new TagsService(_dbContext);
        }

        [Fact]
        public async Task GetAll_Returns_None_When_There_Are_No_Tags()
        {
            // Arrange
            _dbContext.Tags.RemoveRange(_dbContext.Tags);
            await _dbContext.SaveChangesAsync();

            // Act
            var tags = await _tagsService.GetAllAsync();

            // Assert
            tags.Length.ShouldBe(0);
        }

        [Theory]
        [CustomAutoData]
        public async Task GetAll_Returns_Correct_Data(string[] tagNamesToAdd)
        {
            // Arrange
            var tagsToAdd = tagNamesToAdd
                .Select(n => new Tag { Name = n })
                .ToArray();

            _dbContext.Tags.AddRange(tagsToAdd);
            await _dbContext.SaveChangesAsync();

            // Act
            var tags = await _tagsService.GetAllAsync();

            // Assert
            tags.Length.ShouldBe(tagsToAdd.Length);
            tags.ShouldAllBe(tagName => tagsToAdd.Any(addedTag => addedTag.Name == tagName));
        }
    }
}

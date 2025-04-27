using System.Linq;
using System.Threading.Tasks;
using Moq;
using Moq.AutoMock;
using TalkyTiles.Core.Models;
using TalkyTiles.Core.Services.Interfaces;
using TalkyTiles.Core.ViewModels;
using TalkyTiles.Tests.Infrastructure;
using Xunit;

namespace TalkyTiles.Tests.ViewModels
{
    public class TileCanvasViewModelTests
    {
        [Fact]
        public async Task LoadPageAsync_Populates_Buttons()
        {
            // Arrange
            var mocker = new AutoMocker();
            var page = TestUtility.CreatePage("Page1", "A", "B");
            mocker.GetMock<ITileStorageService>()
                  .Setup(x => x.LoadPageAsync("Page1"))
                  .ReturnsAsync(page);

            var sut = mocker.CreateInstance<TileCanvasViewModel>();

            // Act
            await sut.LoadPageAsync("Page1");

            // Assert
            Assert.Equal(2, sut.Buttons.Count);
            Assert.Equal("A", sut.Buttons[0].DisplayText);
            Assert.Equal("B", sut.Buttons[1].DisplayText);
        }

        [Fact]
        public async Task AddNewTileAsync_Adds_Tile_And_Saves()
        {
            // Arrange
            var mocker = new AutoMocker();
            var page = TestUtility.CreatePage("P", "X");
            // seed _currentPage by calling LoadPageAsync stub:
            mocker.GetMock<ITileStorageService>()
                  .Setup(x => x.LoadPageAsync(It.IsAny<string>()))
                  .ReturnsAsync(page);

            var sut = mocker.CreateInstance<TileCanvasViewModel>();
            await sut.LoadPageAsync("P");
            int before = sut.Buttons.Count;

            // Act
            await sut.AddNewTileAsync();

            // Assert
            Assert.Equal(before + 1, sut.Buttons.Count);
            mocker.GetMock<ITileStorageService>()
                  .Verify(x => x.SavePageAsync(page), Times.Once);
        }

        [Fact]
        public async Task SaveCanvasAsync_Persists_All_Tiles()
        {
            // Arrange
            var mocker = new AutoMocker();
            var page = TestUtility.CreatePage("P", "T1", "T2");
            mocker.GetMock<ITileStorageService>()
                  .Setup(x => x.LoadPageAsync(It.IsAny<string>()))
                  .ReturnsAsync(page);

            var sut = mocker.CreateInstance<TileCanvasViewModel>();
            await sut.LoadPageAsync("P");

            // Mutate one button’s position:
            sut.Buttons[0].X = 123;
            sut.Buttons[0].Y = 456;

            // Act
            await sut.SaveCanvasAsync();

            // Assert that the underlying model’s Buttons got updated
            Assert.Equal(123, page.Buttons[0].X);
            Assert.Equal(456, page.Buttons[0].Y);

            // And persisted
            mocker.GetMock<ITileStorageService>()
                  .Verify(x => x.SavePageAsync(page), Times.Once);
        }
    }
}

// Tests/ViewModels/SoundButtonViewModelTests.cs

using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Moq;
using TalkyTiles.Core.Models;
using TalkyTiles.Core.Services.Interfaces;
using TalkyTiles.Core.ViewModels;
using TalkyTiles.Tests.Infrastructure;
using Xunit;

namespace TalkyTiles.Tests.ViewModels
{
    public class SoundButtonViewModelTests
    {
        [Fact]
        public async Task PlayAudioCommand_WithAudioPath_CallsAudioService()
        {
            // Arrange
            var model = new SoundButton { Text = "Hello", AudioPath = "file.wav" };
            var sut = TestUtility.CreateSut(model, out var audioMock, out _, out _);

            // Act
            await ((IAsyncRelayCommand)sut.PlayAudioCommand).ExecuteAsync(null);

            // Assert
            audioMock.Verify(x => x.PlayAudioAsync("file.wav"), Times.Once);
        }

        [Fact]
        public async Task PlayAudioCommand_WithoutAudioPath_DoesNotCallAudioService()
        {
            // Arrange
            var model = new SoundButton { Text = "Hello", AudioPath = string.Empty };
            var sut = TestUtility.CreateSut(model, out var audioMock, out _, out _);

            // Act
            await ((IAsyncRelayCommand)sut.PlayAudioCommand).ExecuteAsync(null);

            // Assert
            audioMock.Verify(x => x.PlayAudioAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void X_Setter_UpdatesModel_And_RaisesPropertyChanged()
        {
            // Arrange
            var model = new SoundButton { X = 0 };
            var sut = TestUtility.CreateSut(model, out _, out _, out _);

            bool raised = false;
            sut.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(sut.X))
                    raised = true;
            };

            // Act
            sut.X = 42;

            // Assert
            Assert.Equal(42, model.X);
            Assert.True(raised);
        }

        [Fact]
        public void X_Setter_WithSameValue_DoesNotRaisePropertyChanged()
        {
            // Arrange
            var model = new SoundButton { X = 7 };
            var sut = TestUtility.CreateSut(model, out _, out _, out _);

            int count = 0;
            sut.PropertyChanged += (s, e) => count++;

            // Act
            sut.X = 7;
            sut.X = 7;

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void Y_Setter_UpdatesModel_And_RaisesPropertyChanged()
        {
            // Arrange
            var model = new SoundButton { Y = 5 };
            var sut = TestUtility.CreateSut(model, out _, out _, out _);

            bool raised = false;
            sut.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(sut.Y))
                    raised = true;
            };

            // Act
            sut.Y = 99;

            // Assert
            Assert.Equal(99, model.Y);
            Assert.True(raised);
        }

        [Fact]
        public void Y_Setter_WithSameValue_DoesNotRaisePropertyChanged()
        {
            // Arrange
            var model = new SoundButton { Y = 13 };
            var sut = TestUtility.CreateSut(model, out _, out _, out _);

            int count = 0;
            sut.PropertyChanged += (s, e) => count++;

            // Act
            sut.Y = 13;
            sut.Y = 13;

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task RecordAudioCommand_Should_StartRecording_And_SetFlags()
        {
            // Arrange
            var id    = Guid.NewGuid().ToString();
            var model = new SoundButton { Id = id };
            var sut   = TestUtility.CreateSut(model, out var audio, out _, out _);

            audio.Setup(x => x.StartRecordingAsync($"{id}.wav"))
                 .ReturnsAsync("recording.wav");

            // Pre-assert
            Assert.False(sut.IsRecording);
            Assert.Equal(0, sut.SecondsRemaining);

            // Act
            await sut.RecordAudioCommand.ExecuteAsync(null);

            // Assert
            audio.Verify(x => x.StartRecordingAsync($"{id}.wav"), Times.Once);
            Assert.True(sut.IsRecording);
            Assert.Equal(10, sut.SecondsRemaining);
        }

        [Fact]
        public async Task StopRecordingCommand_Should_Stop_And_Play_And_ClearFlags()
        {
            // Arrange
            var id    = Guid.NewGuid().ToString();
            var model = new SoundButton { Id = id };
            var sut   = TestUtility.CreateSut(model, out var audio, out _, out _);

            audio.Setup(x => x.StartRecordingAsync($"{id}.wav"))
                 .ReturnsAsync("recording.wav");
            audio.Setup(x => x.StopRecordingAsync())
                 .Returns(Task.CompletedTask);
            audio.Setup(x => x.PlayAudioAsync("recording.wav"))
                 .Returns(Task.CompletedTask);

            // Start a recording so _pendingAudioPath is set
            await sut.RecordAudioCommand.ExecuteAsync(null);
            Assert.True(sut.IsRecording);

            // Act
            await sut.StopRecordingCommand.ExecuteAsync(null);

            // Assert
            audio.Verify(x => x.StopRecordingAsync(), Times.Once);
            audio.Verify(x => x.PlayAudioAsync("recording.wav"), Times.Once);

            Assert.False(sut.IsRecording);
            Assert.Equal("recording.wav", sut.AudioPath);
        }

        [Fact]
        public async Task CountdownTimer_Should_Decrease_SecondsRemaining()
        {
            // Arrange
            var id    = Guid.NewGuid().ToString();
            var model = new SoundButton { Id = id };
            var sut = TestUtility.CreateTestableSut(model
                                                  , out var audioMock
                                                  , out var storageMock
                                                  , out var uiMock);

            audioMock.Setup(x => x.StartRecordingAsync($"{id}.wav"))
                     .ReturnsAsync("recording.wav");
            audioMock.Setup(x => x.StopRecordingAsync())
                     .Returns(Task.CompletedTask);
            audioMock.Setup(x => x.PlayAudioAsync(It.IsAny<string>()))
                     .Returns(Task.CompletedTask);

            // Act
            await (sut.RecordAudioCommand).ExecuteAsync(null);

            // Assert
            Assert.Equal(0, sut.SecondsRemaining); // Should have counted down to 0
            audioMock.Verify(x => x.StopRecordingAsync()
                           , Times.Once);
        }

        [Fact]
        public async Task CountdownTimer_Should_Record_Sequence_10_to_1()
        {
            // Arrange
            var id    = Guid.NewGuid().ToString();
            var model = new SoundButton { Id = id };

            // Use our TestUtility factory
            var sut = TestUtility.CreateTestableSut(model
                                                  , out var audioMock
                                                  , out var storageMock
                                                  , out var uiMock);

            audioMock.Setup(x => x.StartRecordingAsync($"{id}.wav"))
                     .ReturnsAsync("recording.wav");
            audioMock.Setup(x => x.StopRecordingAsync())
                     .Returns(Task.CompletedTask);
            audioMock.Setup(x => x.PlayAudioAsync(It.IsAny<string>()))
                     .Returns(Task.CompletedTask);

            // Act
            await sut.RecordAudioCommand.ExecuteAsync(null);

            // Assert: RecordedSeconds should be [10,9,8,...,1]
            var expected = Enumerable.Range(1, 10).Reverse().ToList();
            Assert.Equal(expected
                       , sut.RecordedSeconds);

            // And ensure final stop was called
            audioMock.Verify(audioService => audioService.StopRecordingAsync()
                           , Times.Once);
        }
    }
}

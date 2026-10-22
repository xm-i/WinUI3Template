using AppTemplate.Composition.Constants;
using AppTemplate.Core.Services;
using Shouldly;

namespace AppTemplate.Core.Tests;

public class AppPathProviderTests {
	[Fact]
	public void AppPathProvider_ReturnsExpectedPaths() {
		var provider = new AppPathProvider();

		provider.BaseDirectory.ShouldBe(FilePathConstants.BaseDirectory);
		provider.ConfigFilePath.ShouldBe(FilePathConstants.ConfigFilePath);
		provider.StateFilePath.ShouldBe(FilePathConstants.StateFilePath);
		provider.LogDirectoryPath.ShouldBe(FilePathConstants.LogDirectoryPath);
	}

	[Fact]
	public void FilePathConstants_OverrideBaseDirectory_ChangesPath() {
		var tempDir = Path.Combine(Path.GetTempPath(), "AppTemplateTest_" + Guid.NewGuid().ToString("N"));
		try {
			FilePathConstants.OverrideBaseDirectory(tempDir);
			FilePathConstants.BaseDirectory.ShouldBe(tempDir);
			FilePathConstants.ConfigFilePath.ShouldBe(Path.Combine(tempDir, "AppTemplate.config"));
			FilePathConstants.StateFilePath.ShouldBe(Path.Combine(tempDir, "AppTemplate.states"));
		} finally {
			if (Directory.Exists(tempDir)) {
				Directory.Delete(tempDir, true);
			}
		}
	}
}
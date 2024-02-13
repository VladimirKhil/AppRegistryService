namespace AppRegistry.ComponentTests.Services;

internal sealed class FamiliesServiceTests : TestsBase
{
    [Test]
    public async Task GetFamilies_Ok()
    {
        var families = await FamiliesService.GetFamiliesAsync();

        Assert.That(families, Has.Length.GreaterThanOrEqualTo(2));
        var family = families.FirstOrDefault(f => f.Id == FamilyId);

        Assert.That(family, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(family.Name, Is.EqualTo("TestAppFamily " + FamilyId));
            Assert.That(family.Description, Is.EqualTo("Test description"));
            Assert.That(family.LogoUri, Is.EqualTo("http://test.logo"));
        });
    }

    [Test]
    public async Task GetFamilyApps_Ok()
    {
        var apps = await FamiliesService.GetFamilyAppsAsync(FamilyId);

        Assert.That(apps, Has.Length.GreaterThanOrEqualTo(2));
        var app = apps.FirstOrDefault(a => a.Id == AppId);

        Assert.That(app, Is.Not.Null);
        Assert.That(app.Name, Is.EqualTo("App " + AppId));
    }
}

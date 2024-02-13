namespace AppRegistry.IntegrationTests;

internal sealed class FamiliesTests : TestsBase
{
    [Test]
    public async Task GetFamiliesAndApps_Ok()
    {
        var families = await FamiliesApi.GetFamiliesAsync();

        Assert.That(families, Is.Not.Null);
        Assert.That(families, Has.Length.GreaterThanOrEqualTo(2));

        var apps = await FamiliesApi.GetFamilyAppsAsync(families[0].Id);

        Assert.That(apps, Is.Not.Null);
        Assert.That(apps, Has.Length.GreaterThanOrEqualTo(1));
        Assert.That(apps.Select(a => a.AppFamilyId), Is.All.EqualTo(families[0].Id));
    }
}

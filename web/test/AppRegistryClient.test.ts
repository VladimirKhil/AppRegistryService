import AppRegistryClient from '../src/AppRegistryClient';
import AppRegistryClientOptions from '../src/AppRegistryClientOptions';
import Architecture from '../src/requests/Architecture';

const options: AppRegistryClientOptions = {
	//serviceUri: 'http://localhost:5165'
	serviceUri: 'http://vladimirkhil.com/appregistry'
};

const appRegistryClient = new AppRegistryClient(options);

test('Post app usage', async () => {
	const now = new Date();

	const response = await appRegistryClient.postAppUsageAsync(
		'1d83d2b8-908f-422f-b3de-4febf96f9665', {
		appVersion: '8.0.0',
		osArchitecture: Architecture.X64,
		osVersion: '10.0.0'
	});

	expect(response).not.toBeNull();
	expect(response?.release).not.toBeNull();
	expect(response?.installer).not.toBeNull();
	expect(response?.release?.version).toBe('8.0.0');
	expect(response?.installer?.title).toBe('Windows application');
});

test('Get families', async () => {
	const families = await appRegistryClient.getFamiliesAsync();
	expect(families.length).toBeGreaterThan(0);
});

test('Get family apps', async () => {
	const families = await appRegistryClient.getFamiliesAsync();
	const apps = await appRegistryClient.getFamilyAppsAsync(families[0].id);
	expect(apps.length).toBeGreaterThan(0);
});

test('Get app screenshots', async () => {
	const families = await appRegistryClient.getFamiliesAsync();
	const apps = await appRegistryClient.getFamilyAppsAsync(families[0].id);
	const screenshots = await appRegistryClient.getAppScreenshotsAsync(apps[0].id);
	expect(screenshots.screenshotUris.length).toBeGreaterThan(0);
});

test('Get app releases page', async () => {
	const families = await appRegistryClient.getFamiliesAsync();
	const apps = await appRegistryClient.getFamilyAppsAsync(families[0].id);
	const releases = await appRegistryClient.getAppReleasesPageAsync(apps[0].id, 0, 10);
	expect(releases.results.length).toBeGreaterThan(0);
	expect(releases.total).toBeGreaterThan(0);
});

test('Get app installers', async () => {
	const families = await appRegistryClient.getFamiliesAsync();
	const apps = await appRegistryClient.getFamilyAppsAsync(families[0].id);
	const installers = await appRegistryClient.getAppInstallersAsync(apps[0].id);
	expect(installers.length).toBeGreaterThan(0);
});

test('Get app latest installer', async () => {
	const families = await appRegistryClient.getFamiliesAsync();
	const apps = await appRegistryClient.getFamilyAppsAsync(families[0].id);
	const installer = await appRegistryClient.getAppLatestInstallerAsync(apps[0].id, '10.0.0');
	expect(installer).not.toBeNull();
	expect(installer?.installer).not.toBeNull();
	expect(installer?.release).not.toBeNull();
});
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

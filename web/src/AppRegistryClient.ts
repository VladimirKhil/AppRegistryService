import AppRegistryClientOptions from './AppRegistryClientOptions';
import { AppRegistryServiceError } from './models/AppRegistryServiceError';
import WellKnownAppRegistryServiceErrorCode from './models/WellKnownAppRegistryServiceErrorCode';
import { AppUsageInfo } from './requests/AppUsageInfo';
import { AppInstallerReleaseInfoResponse } from './responses/AppInstallerReleaseInfoResponse';

/** Defines AppRegistry service client. */
export default class AppRegistryClient {
	/**
	 * Initializes a new instance of AppRegistryClient class.
	 * @param options Client options.
	 */
	constructor(public options: AppRegistryClientOptions) { }

	/** Posts application usage info and receives latest installer info for this application.
	 * @param appId Application identifier.
	 * @param appUsageInfo Application usage info.
	 */
	async postAppUsageAsync(appId: string, appUsageInfo: AppUsageInfo): Promise<AppInstallerReleaseInfoResponse | null> {
		const response = await fetch(`${this.options.serviceUri}/api/v1/apps/${appId}/usage`, {
			method: 'POST',
			headers: {
				'Accept': 'application/json',
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(appUsageInfo),
		});

		if (!response.ok) {
			const error = await response.json() as AppRegistryServiceError;

			if (error.errorCode === WellKnownAppRegistryServiceErrorCode.AppReleaseNotFound) {
				return null;
			}

			throw new Error(response.status + ' ' + error?.errorCode);
		}

		return await response.json() as AppInstallerReleaseInfoResponse;
	}

	/**
	 * Gets resource by Uri.
	 * @param uri Resource Uri.
	 */
	async getAsync<T>(uri: string) {
		const response = await fetch(`${this.options.serviceUri}/api/v1/${uri}`);

		if (!response.ok) {
			throw new Error(`Error while retrieving ${uri}: ${response.status} ${await response.text()}`);
		}

		return <T>(await response.json());
	}
}
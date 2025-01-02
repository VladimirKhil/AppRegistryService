import AppRegistryClientOptions from './AppRegistryClientOptions';
import { AppRegistryServiceError } from './models/AppRegistryServiceError';
import WellKnownAppRegistryServiceErrorCode from './models/WellKnownAppRegistryServiceErrorCode';
import { AppUsageInfo } from './requests/AppUsageInfo';
import { AppFamilyInfo } from './responses/AppFamilyInfo';
import { AppInfo } from './responses/AppInfo';
import { AppInstallerReleaseInfoResponse } from './responses/AppInstallerReleaseInfoResponse';
import { AppReleaseInfo } from './responses/AppReleaseInfo';
import { ResultsPage } from './responses/ResultsPage';
import { ScreenshotsResponse } from './responses/ScreenshotsResponse';

/** Defines AppRegistry service client. */
export default class AppRegistryClient {
	/**
	 * Initializes a new instance of AppRegistryClient class.
	 * @param options Client options.
	 */
	constructor(public options: AppRegistryClientOptions) { }

	/** Gets all application families. */
	getFamiliesAsync(): Promise<AppFamilyInfo[]> {
		return this.getAsync<AppFamilyInfo[]>('families');
	}

	/** Gets applications for a family.
	 * @param appFamilyId Application family identifier.
	 */
	getFamilyAppsAsync(appFamilyId: string): Promise<AppInfo[]> {
		return this.getAsync<AppInfo[]>(`families/${appFamilyId}/apps`);
	}

	/** Gets application screenshots.
	 * @param appId Application identifier.
	 */
	getAppScreenshotsAsync(appId: string): Promise<ScreenshotsResponse> {
		return this.getAsync<ScreenshotsResponse>(`apps/${appId}/screenshots`);
	}

	/** Gets application releases page.
	 * @param appId Application identifier.
	 * @param from Starting index.
	 * @param count Number of items to retrieve.
	 */
	getAppReleasesPageAsync(appId: string, from: number, count: number): Promise<ResultsPage<AppReleaseInfo>> {
		return this.getAsync<ResultsPage<AppReleaseInfo>>(`apps/${appId}/releases?from=${from}&count=${count}`);
	}

	/** Gets application installers.
	 * @param appId Application identifier.
	 */
	getAppInstallersAsync(appId: string): Promise<AppInstallerReleaseInfoResponse[]> {
		return this.getAsync<AppInstallerReleaseInfoResponse[]>(`apps/${appId}/installers`);
	}

	/** Gets the latest application installer.
	 * @param appId Application identifier.
	 * @param osVersion Operating system version.
	 */
	getAppLatestInstallerAsync(appId: string, osVersion?: string): Promise<AppInstallerReleaseInfoResponse> {
		return this.getAsync<AppInstallerReleaseInfoResponse>(`apps/${appId}/installers/latest?osVersion=${osVersion}`);
	}

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
			try {
				const error = await response.json() as AppRegistryServiceError;

				if (error.errorCode === WellKnownAppRegistryServiceErrorCode.AppReleaseNotFound) {
					return null;
				}

				throw new Error(response.status + ' ' + error?.errorCode);
			} catch {
				throw new Error(response.status + ' ' + response.text());
			}
		}

		return await response.json() as AppInstallerReleaseInfoResponse;
	}

	/**
	 * Gets resource by Uri.
	 * @param uri Resource Uri.
	 */
	async getAsync<T>(uri: string) {
		const response = await fetch(`${this.options.serviceUri}/api/v1/${uri}`, this.options.culture ? {
			headers: {
				'Accept-Language': this.options.culture
			}
		} : undefined);

		if (!response.ok) {
			throw new Error(`Error while retrieving ${uri}: ${response.status} ${await response.text()}`);
		}

		return <T>(await response.json());
	}
}
/**
 * Defines an application release installer.
 */
export interface AppInstallerInfo {
	/**
	 * Unique installer identifier.
	 */
	id: string;

	/**
	 * Release identifier link.
	 */
	releaseId: string;

	/**
	 * Installer file uri.
	 */
	uri?: string;

	/**
	 * Installer title.
	 */
	title?: string;

	/**
	 * Installer description.
	 */
	description?: string;

	/**
	 * Installer file size in bytes.
	 */
	size?: number;

	/**
	 * Installer additional (optional) files size in bytes.
	 */
	additionalSize?: number;
}

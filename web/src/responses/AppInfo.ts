/** Defines an application info. */
export interface AppInfo {
	/**
	 * Application unique identifier.
	 */
	id: string;

	/**
	 * Application family identifier link.
	 */
	familyId: string;

	/**
	 * Application name.
	 */
	name?: string;

	/**
	 * Application known issues info.
	 */
	knownIssues?: string;
}
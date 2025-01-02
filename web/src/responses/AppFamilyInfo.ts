/**
 * Defines an application family info.
 * Applications in a family share the same name and logo but have separate codebases, releases and supported platforms.
 */
export interface AppFamilyInfo {
	/**
	 * Unique application family identifier.
	 */
	id: string;

	/**
	 * Application family name.
	 */
	name?: string;

	/**
	 * Application family description.
	 */
	description?: string;

	/**
	 * Application family logo uri.
	 */
	logoUri?: string;
}

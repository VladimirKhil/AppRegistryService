import Architecture from "./Architecture";

/** Defines application usage info. */
export interface AppUsageInfo {
	/** Application version. */
	appVersion: string;

	/** OS version. */
	osVersion: string;

	/** OS architecture. */
	osArchitecture: Architecture;
}
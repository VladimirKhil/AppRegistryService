import ReleaseLevel from '../models/ReleaseLevel';

/**
 * Defines an application release information.
 */
export interface AppReleaseInfo {
    /**
     * Unique release identifier.
     */
    id: string;

    /**
     * Application identifier link.
     */
    appId: string;

    /**
     * Release version.
     */
    version?: string;

    /**
     * Minimum supported operating system version.
     */
    minimumOSVersion?: string;

    /**
     * Release publish date.
     */
    publishDate?: Date;

    /**
     * Release notes.
     */
    notes?: string;

    /**
     * Release level.
     */
    level: ReleaseLevel;

    /**
     * Is this release mandatory.
     */
    isMandatory: boolean;
}
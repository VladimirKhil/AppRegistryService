import { AppInstallerInfo } from "./AppInstallerInfo";
import { AppReleaseInfo } from "./AppReleaseInfo";

/**
 * Defines an application installer info response.
 */
export interface AppInstallerReleaseInfoResponse {
    /**
     * Release info.
     */
    release?: AppReleaseInfo;

    /**
     * Installer info.
     */
    installer?: AppInstallerInfo;
}
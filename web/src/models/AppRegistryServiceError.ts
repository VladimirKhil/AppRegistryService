import WellKnownAppRegistryServiceErrorCode from './WellKnownAppRegistryServiceErrorCode';

/**
 * Defines an AppRegistry service error.
 */
export interface AppRegistryServiceError {
    /**
     * Error code.
     */
    errorCode: WellKnownAppRegistryServiceErrorCode;
}
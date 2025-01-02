/**
 * Represents a page of results.
 *
 * @template T The type of the results.
 */
export interface ResultsPage<T> {
	/** Results collection. */
	results: T[];

	/** Total number of results in repository. */
	total: number;
}
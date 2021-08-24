const ReferenceIdPrefix = "#";
const AccessPathSeparator = ".";

class AccessPathsClass {
  /**
   * Checks if the is a reference identifier.
   * @param {string} accessPath
   * @returns {boolean}
   */
  isReferenceId(accessPath) {
    return !!accessPath && accessPath.startsWith(ReferenceIdPrefix);
  }

  /**
   * Gets the reference identifier from the access path.
   * @param {string} accessPath
   * @returns {string}
   */
  getReferenceId(accessPath) {
    return accessPath?.substring(ReferenceIdPrefix.length);
  }

  /**
   * Get access path from the specified reference identifier.
   * @param {string} referenceId The object reference identifier.
   * @returns {string} The access path.
   */
  fromReferenceId(referenceId) {
    return ReferenceIdPrefix + referenceId;
  }

  /**
   * Combine multiple access paths.
   * @param {string[]} accessPaths
   * @returns {string}
   */
  combine(...accessPaths) {
    return accessPaths?.filter(accessPath => typeof accessPath === "string" && accessPath.length)?.join(AccessPathSeparator);
  }

  /**
   * Splits the access path based on the access path separator.
   * @param {string} accessPath
   * @returns {string[]}
   */
  split(accessPath) {
    return accessPath?.split(AccessPathSeparator);
  }
}

const AccessPaths = new AccessPathsClass();
export default AccessPaths;
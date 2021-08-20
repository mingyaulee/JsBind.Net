const ReferenceIdPrefix = "#";
const AccessPathSeparator = ".";

class JsObjectHandlerClass {
  _objectReferences = {};
  _objectReferencesCount = 0;

  /**
   * Combine multiple access paths.
   * @param {string[]} accessPaths
   * @returns {string}
   */
  combineAccessPaths(...accessPaths) {
    return accessPaths.filter(accessPath => typeof accessPath === "string" && accessPath.length).join(AccessPathSeparator);
  }

  /**
   * Get access path from the specified reference identifier.
   * @param {string} referenceId The object reference identifier.
   * @returns {string} The access path.
   */
  getAccessPathFromReferenceId(referenceId) {
    return ReferenceIdPrefix + referenceId;
  }

  /**
   * Get an object from access path.
   * @param {string} accessPath The access path.
   * @returns {any} The object reference.
   */
  getObjectFromAccessPath(accessPath) {
    if (!accessPath) {
      return globalThis;
    }

    let targetObject = globalThis;
    const accessPaths = accessPath.split(AccessPathSeparator);

    for (let i = 0; i < accessPaths.length; i++) {
      if (i === 0 && accessPaths[i].startsWith(ReferenceIdPrefix)) {
        targetObject = this._objectReferences[accessPaths[i].substring(1)];
      } else {
        targetObject = targetObject?.[accessPaths[i]];
      }
    }

    return targetObject;
  }

  /**
   * Add an object reference with the specified identifier.
   * @param {string} referenceId The object reference identifier.
   * @param {any} objectReference The object reference.
   */
  addObjectReference(referenceId, objectReference) {
    if (!this._objectReferences[referenceId]) {
      this._objectReferencesCount++;
    }
    this._objectReferences[referenceId] = objectReference;
  }

  /**
   * Remove object reference.
   * @param {string} referenceId The object reference identifier.
   */
  removeObjectReference(referenceId) {
    if (this._objectReferences[referenceId]) {
      this._objectReferencesCount--;
      this._objectReferences[referenceId] = null;
      try {
        delete this._objectReferences[referenceId];
      } catch { }
    }
  }

  /**
   * Get the object references.
   * @returns {object} The object references.
   */
  getObjectReferences() { return this._objectReferences; }

  /**
   * Get the count of the object references.
   * @returns {number} The count of the object references.
   */
  getObjectReferencesCount() { return this._objectReferencesCount; }
}

const JsObjectHandler = new JsObjectHandlerClass();
export default JsObjectHandler;
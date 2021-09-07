import AccessPaths from "./AccessPaths.js";

class JsObjectHandlerClass {
  constructor() {
    this._objectReferences = {};
    this._objectReferencesCount = 0;
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
    const accessPaths = AccessPaths.split(accessPath);

    for (let i = 0; i < accessPaths.length; i++) {
      if (i === 0 && AccessPaths.isReferenceId(accessPaths[i])) {
        const referenceId = AccessPaths.getReferenceId(accessPaths[i]);
        targetObject = this._objectReferences[referenceId];
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
   * Remove all the object references.
   */
  clearObjectReferences() {
    this._objectReferences = {};
    this._objectReferencesCount = 0;
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
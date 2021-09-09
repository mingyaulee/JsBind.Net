import DotNetDelegateProxy from "./DotNetDelegateProxy.js";

/**
 * @typedef {import("./References/DelegateReference.js").default} DelegateReference
 */

class DelegateReferenceHandlerClass {
  constructor() {
    /** @type {Object<string, DotNetDelegateProxy>} */
    this._delegateReferences = {};
    this._delegateReferencesCount = 0;
  }

  /**
   * Get or create the delegate proxy from the delegate reference.
   * @param {DelegateReference} delegateReference
   * @returns {DotNetDelegateProxy}
   */
  getOrCreateDelegateProxy(delegateReference) {
    if (this._delegateReferences[delegateReference.delegateId]) {
      return this._delegateReferences[delegateReference.delegateId];
    }

    this._delegateReferencesCount++;
    const delegateProxy = new DotNetDelegateProxy(delegateReference);
    this._delegateReferences[delegateReference.delegateId] = delegateProxy;
    return delegateProxy;
  }

  /**
   * Remove the delegate reference by delegate ID.
   * @param {string} delegateId 
   */
  removeDelegateReference(delegateId) {
    if (this._delegateReferences[delegateId]) {
      this._delegateReferencesCount--;
      this._delegateReferences[delegateId] = null;
      try {
        delete this._delegateReferences[delegateId];
      } catch { }
    }
  }

  /**
   * Remove all the delegate references.
   */
  clearDelegateReferences() {
    this._delegateReferences = {};
    this._delegateReferencesCount = 0;
  }

  /**
   * Get the delegate references.
   * @returns {object} The delegate references.
   */
  getDelegateReferences() { return this._delegateReferences; }

  /**
   * Get the count of the delegate references.
   * @returns {number} The count of the delegate references.
   */
  getDelegateReferencesCount() { return this._delegateReferencesCount; }
}

const DelegateReferenceHandler = new DelegateReferenceHandlerClass();
export default DelegateReferenceHandler;
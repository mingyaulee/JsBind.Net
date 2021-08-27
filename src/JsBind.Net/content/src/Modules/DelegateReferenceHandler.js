import DotNetDelegateProxy from "./DotNetDelegateProxy.js";

/**
 * @typedef {import("./References/DelegateReference.js").default} DelegateReference
 */

class DelegateReferenceHandlerClass {
  constructor() {
    /** @type {Object<string, DotNetDelegateProxy>} */
    this._delegateReferences = {};
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
      this._delegateReferences[delegateId] = null;
      try {
        delete this._delegateReferences[delegateId];
      } catch { }
    }
  }
}

const DelegateReferenceHandler = new DelegateReferenceHandlerClass();
export default DelegateReferenceHandler;
import DelegateReferenceHandler from "../DelegateReferenceHandler.js";
import { IsDelegateReference } from "../References/DelegateReference.js";

class DelegateReferenceReviverClass {
  /**
   * Converts a DelegateReference JSON object to a proxy function.
   * @param {any} _key
   * @param {any} value
   */
  revive(_key, value) {
    if (IsDelegateReference(value)) {
      return DelegateReferenceHandler.getOrCreateDelegateProxy(value).proxyFunction;
    }

    return value;
  }
}

const DelegateReferenceReviver = new DelegateReferenceReviverClass();
export default DelegateReferenceReviver;